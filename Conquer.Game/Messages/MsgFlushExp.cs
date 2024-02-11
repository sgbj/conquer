namespace Conquer.Game.Messages;

public enum FlushExpAction : ushort
{
    WeaponSkill = 0,
    Magic = 1,
    Skill = 2
}

public record MsgFlushExp : IMessage
{
    public uint Experience { get; set; }
    public ushort SkillType { get; set; }
    public FlushExpAction Action { get; set; }
    public ushort Size => 12;
    public MessageType Type => MessageType.MsgFlushExp;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Experience = ReadUInt32LittleEndian(buffer[4..]);
        SkillType = ReadUInt16LittleEndian(buffer[8..]);
        Action = (FlushExpAction)ReadUInt16LittleEndian(buffer[10..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Experience);
        WriteUInt16LittleEndian(buffer[8..], SkillType);
        WriteUInt16LittleEndian(buffer[10..], (ushort)Action);
    }
}