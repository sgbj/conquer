namespace Conquer.Game.Messages;

public enum TradeAction : uint
{
    Apply = 1,
    Quit = 2,
    Open = 3,
    Success = 4,
    False = 5,
    AddItem = 6,
    AddMoney = 7,
    AllMoney = 8,
    SelfAllMoney = 9,
    Ok = 10,
    AddItemFail = 11
}

public record MsgTrade : IMessage
{
    public uint Id { get; set; }
    public TradeAction Action { get; set; }
    public ushort Size => 12;
    public MessageType Type => MessageType.MsgTrade;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Action = (TradeAction)ReadUInt32LittleEndian(buffer[8..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], (uint)Action);
    }

    public Task HandleAsync(GameClient client, ILogger<MsgTrade> logger)
    {
        return Action switch
        {
            _ => Default()
        };

        Task Default()
        {
            logger.LogWarning("Unhandled action {Action}.", Action);
            return Task.CompletedTask;
        }
    }
}