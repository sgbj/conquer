namespace Conquer.Game.Messages;

public enum ItemInfoExAction : byte
{
    Booth = 1,
    Equipment = 2,
    OtherPlayerEquipment = 4
}

public record MsgItemInfoEx : IMessage
{
    public uint Id { get; set; }
    public uint OwnerId { get; set; }
    public uint Price { get; set; }
    public uint ItemTypeId { get; set; }
    public ushort Amount { get; set; }
    public ushort AmountLimit { get; set; }
    public ItemInfoExAction Action { get; set; }
    public byte Ident { get; set; }
    public byte Position { get; set; }
    public byte Gem1 { get; set; }
    public byte Gem2 { get; set; }
    public byte Magic1 { get; set; }
    public byte Magic2 { get; set; }
    public byte Magic3 { get; set; }
    public byte Bless { get; set; }
    public byte Enchant { get; set; }
    public uint Restrain { get; set; }
    public ushort Size => 38;
    public MessageType Type => MessageType.MsgItemInfoEx;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        OwnerId = ReadUInt32LittleEndian(buffer[8..]);
        Price = ReadUInt32LittleEndian(buffer[12..]);
        ItemTypeId = ReadUInt32LittleEndian(buffer[16..]);
        Amount = ReadUInt16LittleEndian(buffer[20..]);
        AmountLimit = ReadUInt16LittleEndian(buffer[22..]);
        Action = (ItemInfoExAction)buffer[24];
        Ident = buffer[25];
        Position = buffer[26];
        Gem1 = buffer[27];
        Gem2 = buffer[28];
        Magic1 = buffer[29];
        Magic2 = buffer[30];
        Magic3 = buffer[31];
        Bless = buffer[32];
        Enchant = buffer[33];
        Restrain = ReadUInt32LittleEndian(buffer[34..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], OwnerId);
        WriteUInt32LittleEndian(buffer[12..], Price);
        WriteUInt32LittleEndian(buffer[16..], ItemTypeId);
        WriteUInt16LittleEndian(buffer[20..], Amount);
        WriteUInt16LittleEndian(buffer[22..], AmountLimit);
        buffer[24] = (byte)Action;
        buffer[25] = Ident;
        buffer[26] = Position;
        buffer[27] = Gem1;
        buffer[28] = Gem2;
        buffer[29] = Magic1;
        buffer[30] = Magic2;
        buffer[31] = Magic3;
        buffer[32] = Bless;
        buffer[33] = Enchant;
        WriteUInt32LittleEndian(buffer[34..], Restrain);
    }
}