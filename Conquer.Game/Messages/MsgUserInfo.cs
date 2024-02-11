namespace Conquer.Game.Messages;

public record MsgUserInfo : IMessage
{
    public MsgUserInfo()
    {
    }

    public MsgUserInfo(Player player)
    {
        Id = player.Id;
        LookFace = player.LookFace;
        Hair = player.Hair;
        Money = player.Money;
        Experience = player.Experience;
        Strength = player.Strength;
        Dexterity = player.Dexterity;
        Vitality = player.Vitality;
        Mana = player.Mana;
        AttributePoints = player.AttributePoints;
        Health = player.Health;
        Magic = player.Magic;
        PkPoints = player.PkPoints;
        Level = player.Level;
        Profession = player.Profession;
        AutoAllot = player.Rebirths == 0;
        Rebirths = player.Rebirths;
        Strings = new() { player.Name, player.Spouse?.Name ?? "None" };
    }

    public uint Id { get; set; }
    public uint LookFace { get; set; }
    public ushort Hair { get; set; }
    public uint Money { get; set; }
    public ulong Experience { get; set; }
    public ushort Strength { get; set; }
    public ushort Dexterity { get; set; }
    public ushort Vitality { get; set; }
    public ushort Mana { get; set; }
    public ushort AttributePoints { get; set; }
    public ushort Health { get; set; }
    public ushort Magic { get; set; }
    public short PkPoints { get; set; }
    public byte Level { get; set; }
    public Profession Profession { get; set; }
    public bool AutoAllot { get; set; }
    public byte Rebirths { get; set; }
    public List<string> Strings { get; set; } = null!;

    public ushort Size => (ushort)(61 + GetByteCount(Strings));
    public MessageType Type => MessageType.MsgUserInfo;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        LookFace = ReadUInt32LittleEndian(buffer[8..]);
        Hair = ReadUInt16LittleEndian(buffer[12..]);
        _ = ReadUInt16LittleEndian(buffer[14..]);
        Money = ReadUInt32LittleEndian(buffer[16..]);
        Experience = ReadUInt64LittleEndian(buffer[20..]);
        _ = ReadUInt64LittleEndian(buffer[28..]);
        _ = ReadUInt32LittleEndian(buffer[36..]);
        Strength = ReadUInt16LittleEndian(buffer[40..]);
        Dexterity = ReadUInt16LittleEndian(buffer[42..]);
        Vitality = ReadUInt16LittleEndian(buffer[44..]);
        Mana = ReadUInt16LittleEndian(buffer[46..]);
        AttributePoints = ReadUInt16LittleEndian(buffer[48..]);
        Health = ReadUInt16LittleEndian(buffer[50..]);
        Magic = ReadUInt16LittleEndian(buffer[52..]);
        PkPoints = ReadInt16LittleEndian(buffer[54..]);
        Level = buffer[56];
        Profession = (Profession)buffer[57];
        AutoAllot = buffer[58] != 0;
        Rebirths = buffer[59];
        var hasName = buffer[60] != 0;
        if (hasName)
        {
            Strings = ReadStrings(buffer[61..]);
        }
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], LookFace);
        WriteUInt16LittleEndian(buffer[12..], Hair);
        WriteUInt16LittleEndian(buffer[14..], 0);
        WriteUInt32LittleEndian(buffer[16..], Money);
        WriteUInt64LittleEndian(buffer[20..], Experience);
        WriteUInt64LittleEndian(buffer[28..], 0);
        WriteUInt32LittleEndian(buffer[36..], 0);
        WriteUInt16LittleEndian(buffer[40..], Strength);
        WriteUInt16LittleEndian(buffer[42..], Dexterity);
        WriteUInt16LittleEndian(buffer[44..], Vitality);
        WriteUInt16LittleEndian(buffer[46..], Mana);
        WriteUInt16LittleEndian(buffer[48..], AttributePoints);
        WriteUInt16LittleEndian(buffer[50..], Health);
        WriteUInt16LittleEndian(buffer[52..], Magic);
        WriteInt16LittleEndian(buffer[54..], PkPoints);
        buffer[56] = Level;
        buffer[57] = (byte)Profession;
        buffer[58] = (byte)(AutoAllot ? 1 : 0);
        buffer[59] = Rebirths;
        buffer[60] = (byte)(Strings.Count != 0 ? 1 : 0);
        WriteStrings(buffer[61..], Strings);
    }
}