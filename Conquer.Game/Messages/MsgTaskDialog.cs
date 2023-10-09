namespace Conquer.Game.Messages;

public enum DialogType
{
    Text = 1,
    Link = 2,
    Edit = 3,
    Pic = 4,
    ListLine = 5,
    Create = 100,
    Answer = 101,
    TaskId = 102,
    UpdateNotice = 112
}

public record MsgTaskDialog : IMessage
{
    public uint Id { get; set; }
    public ushort Avatar { get; set; }
    public byte Option { get; set; }
    public DialogType DialogType { get; set; }
    public List<string> Strings { get; set; } = new();
    public ushort Size => (ushort)(13 + GetByteCount(Strings));
    public MessageType Type => MessageType.MsgTaskDialog;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Avatar = ReadUInt16LittleEndian(buffer[8..]);
        Option = buffer[10];
        DialogType = (DialogType)buffer[11];
        Strings = ReadStrings(buffer[12..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt16LittleEndian(buffer[8..], Avatar);
        buffer[10] = Option;
        buffer[11] = (byte)DialogType;
        WriteStrings(buffer[12..], Strings);
    }
}