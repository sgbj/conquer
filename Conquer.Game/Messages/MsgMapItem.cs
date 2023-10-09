namespace Conquer.Game.Messages;

public enum MapItemAction : uint
{
    Create = 1,
    Delete = 2,
    Pick = 3,
    CastTrap = 10,
    SynchroTrap = 11,
    DropTrap = 12
}

public record MsgMapItem : IMessage
{
    public MsgMapItem()
    {
    }

    public MsgMapItem(MapItem mapItem) =>
        (Id, ItemTypeId, X, Y, Action) =
        (mapItem.Id, mapItem.Item.ItemTypeId, mapItem.X, mapItem.Y, MapItemAction.Create);

    public uint Id { get; set; }
    public uint ItemTypeId { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public MapItemAction Action { get; set; }

    public ushort Size => 20;
    public MessageType Type => MessageType.MsgMapItem;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        ItemTypeId = ReadUInt32LittleEndian(buffer[8..]);
        X = ReadUInt16LittleEndian(buffer[12..]);
        Y = ReadUInt16LittleEndian(buffer[14..]);
        Action = (MapItemAction)ReadUInt32LittleEndian(buffer[16..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], ItemTypeId);
        WriteUInt16LittleEndian(buffer[12..], X);
        WriteUInt16LittleEndian(buffer[14..], Y);
        WriteUInt32LittleEndian(buffer[16..], (uint)Action);
    }

    public Task HandleAsync(GameClient client, ILogger<MsgMapItem> logger)
    {
        return Action switch
        {
            MapItemAction.Pick => Pick(),
            _ => Default()
        };

        Task Pick() => Task.CompletedTask;

        Task Default()
        {
            logger.LogWarning("Unhandled action {Action}.", Action);
            return Task.CompletedTask;
        }
    }
}