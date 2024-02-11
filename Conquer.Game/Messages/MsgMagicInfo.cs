namespace Conquer.Game.Messages;

public record MsgMagicInfo : IMessage
{
    public MsgMagicInfo()
    {
    }

    public MsgMagicInfo(Magic magic) => (Experience, MagicType, Level) = (magic.Experience, magic.Type, magic.Level);

    public uint Experience { get; set; }
    public ushort MagicType { get; set; }
    public ushort Level { get; set; }
    public ushort Size => 12;
    public MessageType Type => MessageType.MsgMagicInfo;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Experience = ReadUInt32LittleEndian(buffer[4..]);
        MagicType = ReadUInt16LittleEndian(buffer[8..]);
        Level = ReadUInt16LittleEndian(buffer[10..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Experience);
        WriteUInt16LittleEndian(buffer[8..], MagicType);
        WriteUInt16LittleEndian(buffer[10..], Level);
    }
}