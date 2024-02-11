namespace Conquer.Game.Messages;

public enum PackageAction
{
    QueryList = 0,
    CheckIn = 1,
    CheckOut = 2,
    QueryList2 = 3
}

public record MsgPackage : IMessage
{
    public MsgPackage() => Items = new();
    public uint Id { get; set; }
    public PackageAction Action { get; set; }
    public byte PackageType { get; set; }
    public uint ItemId { get; set; }
    public List<Item> Items { get; set; } = null!;

    public ushort Size => 16;
    public MessageType Type => MessageType.MsgPackage;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Action = (PackageAction)buffer[8];
        PackageType = buffer[9];
        ItemId = ReadUInt32LittleEndian(buffer[10..]);
        var count = ReadUInt16LittleEndian(buffer[14..]);
        var index = 16;
        for (var i = 0; i < count; i++)
        {
            var id = ReadUInt32LittleEndian(buffer[index..]);
            index += sizeof(uint);
            var type = ReadUInt32LittleEndian(buffer[index..]);
            index += sizeof(uint);
            var ident = buffer[index];
            index += sizeof(byte);
            var gem1 = buffer[index];
            index += sizeof(byte);
            var gem2 = buffer[index];
            index += sizeof(byte);
            var magic1 = buffer[index];
            index += sizeof(byte);
            var magic2 = buffer[index];
            index += sizeof(byte);
            var magic3 = buffer[index];
            index += sizeof(byte);
            var bless = ReadUInt16LittleEndian(buffer[index..]);
            index += sizeof(ushort);
            var enchant = ReadUInt16LittleEndian(buffer[index..]);
            index += sizeof(ushort);
            var restrain = ReadUInt16LittleEndian(buffer[index..]);
            index += sizeof(ushort);
            Items.Add(new()
            {
                Id = id,
                ItemTypeId = type,
                Ident = ident,
                Gem1 = (Gem)gem1,
                Gem2 = (Gem)gem2,
                Magic1 = magic1,
                Magic2 = magic2,
                Magic3 = magic3,
                Bless = (byte)bless,
                Enchant = (byte)enchant,
                Restrain = restrain
            });
        }
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt32LittleEndian(buffer[4..], Id);
        buffer[8] = (byte)Action;
        buffer[9] = PackageType;
        WriteUInt32LittleEndian(buffer[10..], ItemId);
        WriteUInt16LittleEndian(buffer[14..], (ushort)Items.Count);
        var index = 16;
        foreach (var item in Items)
        {
            WriteUInt32LittleEndian(buffer[index..], item.Id);
            index += sizeof(uint);
            WriteUInt32LittleEndian(buffer[index..], item.ItemTypeId);
            index += sizeof(uint);
            buffer[index] = item.Ident;
            index += sizeof(byte);
            buffer[index] = (byte)item.Gem1;
            index += sizeof(byte);
            buffer[index] = (byte)item.Gem2;
            index += sizeof(byte);
            buffer[index] = item.Magic1;
            index += sizeof(byte);
            buffer[index] = item.Magic2;
            index += sizeof(byte);
            buffer[index] = item.Magic3;
            index += sizeof(byte);
            WriteUInt16LittleEndian(buffer[index..], item.Bless);
            index += sizeof(ushort);
            WriteUInt16LittleEndian(buffer[index..], item.Enchant);
            index += sizeof(ushort);
            WriteUInt16LittleEndian(buffer[index..], (ushort)item.Restrain);
            index += sizeof(ushort);
        }
    }
}