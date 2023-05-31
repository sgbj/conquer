namespace Conquer.Account.Messages;

public record MsgConnect : IMessage
{
    public uint Id { get; set; }
    public uint Data { get; set; }
    public string? Info { get; set; }
    public ushort Size => 28;
    public MessageType Type => MessageType.MsgConnect;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Data = ReadUInt32LittleEndian(buffer[8..]);
        Info = ReadString(buffer[12..28]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], Data);
        WriteString(buffer[12..28], Info);
    }

    public Task HandleAsync() => Task.CompletedTask;
}