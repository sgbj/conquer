using System.Buffers;

namespace Conquer.Protocols;

public interface IProtocol
{
    bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined,
        out IMessage? message);

    void WriteMessage(IMessage message, IBufferWriter<byte> output);
}