namespace Conquer.Game.Messages;

public enum AttributeType : uint
{
    Life = 0,
    MaxLife = 1,
    Mana = 2,
    MaxMana = 3,
    Money = 4,
    Exp = 5,
    PkPoints = 6,
    Profession = 7,
    SizeAdd = 8,
    Energy = 9,
    AddPoints = 11,
    Look = 12,
    Level = 13,
    Spirit = 14,
    Vitality = 15,
    Strength = 16,
    Agility = 17,
    BlessTime = 18,
    DblExpTime = 19,
    CurseTime = 21,
    TimeAdd = 22,
    Metempsychosis = 23,
    Flags = 26,
    Hair = 27,
    Xp = 28,
    LuckyTime = 29,
    Cps = 30,
    TrainingPoints = 32
}

public record MsgUserAttrib : IMessage
{
    public MsgUserAttrib() => Attributes = new();

    public MsgUserAttrib(uint id, AttributeType type, ulong value) =>
        (Id, Attributes) = (id, new() { (type, value) });

    public uint Id { get; set; }
    public List<(AttributeType, ulong)> Attributes { get; set; }
    public ushort Size => (ushort)(12 + Attributes.Count * 12);
    public MessageType Type => MessageType.MsgUserAttrib;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        var count = ReadInt32LittleEndian(buffer[8..]);
        var index = 12;
        for (var i = 0; i < count; i++)
        {
            var type = (AttributeType)ReadUInt32LittleEndian(buffer[index..]);
            index += sizeof(uint);
            var value = ReadUInt64LittleEndian(buffer[index..]);
            index += sizeof(ulong);
            Attributes.Add((type, value));
        }
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteInt32LittleEndian(buffer[8..], Attributes.Count);
        var index = 12;
        foreach (var (type, value) in Attributes)
        {
            WriteUInt32LittleEndian(buffer[index..], (uint)type);
            index += sizeof(uint);
            WriteUInt64LittleEndian(buffer[index..], value);
            index += sizeof(ulong);
        }
    }
}