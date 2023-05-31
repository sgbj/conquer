namespace Conquer.Game.Messages;

public enum ItemAction : uint
{
    Buy = 1,
    Sell = 2,
    Drop = 3,
    Use = 4,
    Equip = 5,
    Unequip = 6,
    SplitItem = 7,
    CombineItem = 8,
    QueryMoneySaved = 9,
    SaveMoney = 10,
    DrawMoney = 11,
    DropMoney = 12,
    SpendMoney = 13,
    Repair = 14,
    RepairAll = 15,
    Ident = 16,
    Durability = 17,
    DropEquipment = 18,
    Improve = 19,
    UpLevel = 20,
    BoothQuery = 21,
    BoothAdd = 22,
    BoothDel = 23,
    BoothBuy = 24,
    SynchroAmount = 25,
    Fireworks = 26,
    CompleteTask = 27,
    Enchant = 28,      
}

public record MsgItem : IMessage
{
    private readonly byte[] _data = new byte[4];
    public uint Id { get; set; }

    public uint Data
    {
        get => ReadUInt32LittleEndian(_data);
        set => WriteUInt32LittleEndian(_data, value);
    }

    public ushort X
    {
        get => ReadUInt16LittleEndian(_data);
        set => WriteUInt16LittleEndian(_data, value);
    }

    public ushort Y
    {
        get => ReadUInt16LittleEndian(_data.AsSpan(2));
        set => WriteUInt16LittleEndian(_data.AsSpan(2), value);
    }

    public ItemAction Action { get; set; }
    public uint Timestamp { get; set; } = (uint)Environment.TickCount;

    public ushort Size => 20;
    public MessageType Type => MessageType.MsgItem;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        buffer[8..12].CopyTo(_data);
        Action = (ItemAction)ReadUInt32LittleEndian(buffer[12..]);
        Timestamp = ReadUInt32LittleEndian(buffer[16..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        _data.CopyTo(buffer[8..]);
        WriteUInt32LittleEndian(buffer[12..], (uint)Action);
        WriteUInt32LittleEndian(buffer[16..], Timestamp);
    }

    public Task HandleAsync(GameClient client, GameDbContext db, ILogger<MsgItem> logger)
    {
        return Action switch
        {
            ItemAction.CompleteTask => CompleteTask(),
            _ => Default()
        };

        async Task CompleteTask()
        {
            db.Update(client.Player);
            await db.SaveChangesAsync();
            await client.WriteAsync(this);
        }

        async Task Default()
        {
            logger.LogWarning("Unhandled action {Action}.", Action);
            await client.WriteAsync(this);
        }
    }
}