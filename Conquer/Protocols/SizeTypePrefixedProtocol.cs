using System.Buffers;
using System.Text;

namespace Conquer.Protocols;

public class SizeTypePrefixedProtocol : IProtocol
{
    private const int MaximumMessageSize = 1024;
    private readonly MessageService _messageService;

    public SizeTypePrefixedProtocol(MessageService messageService) => _messageService = messageService;

    public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed,
        ref SequencePosition examined, out IMessage? message)
    {
        var reader = new SequenceReader<byte>(input);

        if (!reader.TryReadLittleEndian(out short sizeShort) ||
            !reader.TryReadLittleEndian(out short typeShort))
        {
            examined = input.End;
            message = null;
            return false;
        }

        var size = (ushort)sizeShort;
        var type = (MessageType)typeShort;

        if (size > MaximumMessageSize)
        {
            throw new InvalidDataException($"The maximum message size of {MaximumMessageSize}B was exceeded.");
        }

        if (input.Length < size)
        {
            examined = input.End;
            message = null;
            return false;
        }

        consumed = input.GetPosition(size);
        examined = consumed;

        message = _messageService.Create(type);

        if (message is null)
        {
            return false;
        }

        var messageSlice = input.Slice(0, size);

        if (messageSlice.IsSingleSegment)
        {
            message.Read(messageSlice.FirstSpan);
        }
        else
        {
            Span<byte> buffer = stackalloc byte[size];
            messageSlice.CopyTo(buffer);
            message.Read(buffer);
        }

        return true;
    }

    public void WriteMessage(IMessage message, IBufferWriter<byte> output)
    {
        var size = message.Size;
        var buffer = output.GetSpan(size);
        message.Write(buffer[..size]);
        output.Advance(size);
    }

    public static int GetByteCount(IEnumerable<string> values)
    {
        return 1 + values.Sum(value => 1 + Encoding.Latin1.GetByteCount(value));
    }

    public static string ReadString(ReadOnlySpan<byte> buffer)
    {
        var index = buffer.IndexOf((byte)0);
        return Encoding.Latin1.GetString(index < 0 ? buffer : buffer[..index]);
    }

    public static List<string> ReadStrings(ReadOnlySpan<byte> buffer)
    {
        var index = 0;
        var count = buffer[index++];
        var values = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            var byteCount = buffer[index++];
            values.Add(Encoding.Latin1.GetString(buffer.Slice(index, byteCount)));
            index += byteCount;
        }

        return values;
    }

    public static void WriteString(Span<byte> buffer, string? value)
    {
        Encoding.Latin1.GetBytes(value, buffer);
    }

    public static void WriteStrings(Span<byte> buffer, List<string> values)
    {
        var index = 0;
        buffer[index++] = (byte)values.Count;

        foreach (var value in values)
        {
            var byteCount = (byte)Encoding.Latin1.GetByteCount(value);
            buffer[index++] = byteCount;
            Encoding.Latin1.GetBytes(value, buffer.Slice(index, byteCount));
            index += byteCount;
        }
    }
}