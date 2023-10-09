namespace Conquer.Game.Messages;

public enum ItemInfoAction : byte
{
    None = 0,
    AddItem = 1,
    Trade = 2,
    Update = 3,
    OtherPlayerEquipment = 4,
    Auction = 5
}

public record MsgItemInfo : IMessage
{
    public MsgItemInfo()
    {
    }

    public MsgItemInfo(Item item, ItemInfoAction action)
    {
        ItemId = item.Id;
        ItemTypeId = item.ItemTypeId;
        Amount = item.Amount;
        AmountLimit = item.AmountLimit;
        Action = action;
        Ident = item.Ident;
        Position = item.Position;
        Gem1 = item.Gem1;
        Gem2 = item.Gem2;
        Magic1 = item.Magic1;
        Magic2 = item.Magic2;
        Magic3 = item.Magic3;
        Bless = item.Bless;
        Enchant = item.Enchant;
        Data = item.Restrain;
    }

    public uint ItemId { get; set; }
    public uint ItemTypeId { get; set; }
    public ushort Amount { get; set; }
    public ushort AmountLimit { get; set; }
    public ItemInfoAction Action { get; set; }
    public byte Ident { get; set; }
    public ItemPosition Position { get; set; }
    public Gem Gem1 { get; set; }
    public Gem Gem2 { get; set; }
    public byte Magic1 { get; set; }
    public byte Magic2 { get; set; }
    public byte Magic3 { get; set; }
    public byte Bless { get; set; }
    public byte Enchant { get; set; }
    public uint Data { get; set; }

    public ushort Size => 30;
    public MessageType Type => MessageType.MsgItemInfo;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        ItemId = ReadUInt32LittleEndian(buffer[4..]);
        ItemTypeId = ReadUInt32LittleEndian(buffer[8..]);
        Amount = ReadUInt16LittleEndian(buffer[12..]);
        AmountLimit = ReadUInt16LittleEndian(buffer[14..]);
        Action = (ItemInfoAction)buffer[16];
        Ident = buffer[17];
        Position = (ItemPosition)buffer[18];
        Gem1 = (Gem)buffer[19];
        Gem2 = (Gem)buffer[20];
        Magic1 = buffer[21];
        Magic2 = buffer[22];
        Magic3 = buffer[23];
        Bless = buffer[24];
        Enchant = buffer[25];
        Data = ReadUInt32LittleEndian(buffer[26..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], ItemId);
        WriteUInt32LittleEndian(buffer[8..], ItemTypeId);
        WriteUInt16LittleEndian(buffer[12..], Amount);
        WriteUInt16LittleEndian(buffer[14..], AmountLimit);
        buffer[16] = (byte)Action;
        buffer[17] = Ident;
        buffer[18] = (byte)Position;
        buffer[19] = (byte)Gem1;
        buffer[20] = (byte)Gem2;
        buffer[21] = Magic1;
        buffer[22] = Magic2;
        buffer[23] = Magic3;
        buffer[24] = Bless;
        buffer[25] = Enchant;
        WriteUInt32LittleEndian(buffer[26..], Data);
    }
}