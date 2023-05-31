namespace Conquer;

public interface IMessage
{
    ushort Size { get; }
    MessageType Type { get; }

    void Read(ReadOnlySpan<byte> buffer);
    void Write(Span<byte> buffer);
}