namespace Conquer.Game.Messages;

public record MsgWeaponSkill : IMessage
{
    public MsgWeaponSkill()
    {
    }

    public MsgWeaponSkill(WeaponSkill weaponSkill) =>
        (WeaponSkillType, Level, Experience) = (weaponSkill.Type, weaponSkill.Level, weaponSkill.Experience);

    public uint WeaponSkillType { get; set; }
    public uint Level { get; set; }
    public uint Experience { get; set; }
    public ushort Size => 16;
    public MessageType Type => MessageType.MsgWeaponSkill;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        WeaponSkillType = ReadUInt32LittleEndian(buffer[4..]);
        Level = ReadUInt32LittleEndian(buffer[8..]);
        Experience = ReadUInt32LittleEndian(buffer[12..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], WeaponSkillType);
        WriteUInt32LittleEndian(buffer[8..], Level);
        WriteUInt32LittleEndian(buffer[12..], Experience);
    }
}