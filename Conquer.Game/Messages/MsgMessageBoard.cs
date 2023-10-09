namespace Conquer.Game.Messages;

public enum MessageBoardChannel : ushort
{
}

public enum MessageBoardAction : byte
{
    Del = 1,
    GetList = 2,
    List = 3,
    GetWords = 4
}

public record MsgMessageBoard : IMessage
{
    public ushort Index { get; set; }
    public MessageBoardChannel Channel { get; set; }
    public MessageBoardAction Action { get; set; }
    public List<string> Strings { get; set; } = null!;
    public ushort Size => (ushort)(10 + GetByteCount(Strings));
    public MessageType Type => MessageType.MsgMessageBoard;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Index = ReadUInt16LittleEndian(buffer[4..]);
        Channel = (MessageBoardChannel)ReadUInt16LittleEndian(buffer[6..]);
        Action = (MessageBoardAction)buffer[8];
        Strings = ReadStrings(buffer[9..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt16LittleEndian(buffer[4..], Index);
        WriteUInt16LittleEndian(buffer[6..], (ushort)Channel);
        buffer[8] = (byte)Action;
        WriteStrings(buffer[9..], Strings);
    }
}