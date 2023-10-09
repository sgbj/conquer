namespace Conquer.Game.Messages;

public record MsgMagicEffect : IMessage
{
    private readonly byte[] _data = new byte[4];
    public uint Sender { get; set; }

    public uint Target
    {
        get => ReadUInt32LittleEndian(_data);
        set => WriteUInt32LittleEndian(_data, value);
    }

    public ushort X
    {
        get => ReadUInt16LittleEndian(_data);
        set => WriteUInt16LittleEndian(_data, value);
    }

    public ushort Y
    {
        get => ReadUInt16LittleEndian(_data.AsSpan(2));
        set => WriteUInt16LittleEndian(_data.AsSpan(2), value);
    }

    public ushort MagicType { get; set; }
    public ushort Level { get; set; }
    public List<(uint Target, uint Data)> Targets { get; set; } = new();
    public ushort Size => (ushort)(24 + Targets.Count * 12);
    public MessageType Type => MessageType.MsgMagicEffect;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Sender = ReadUInt32LittleEndian(buffer[4..]);
        buffer[8..12].CopyTo(_data);
        MagicType = ReadUInt16LittleEndian(buffer[12..]);
        Level = ReadUInt16LittleEndian(buffer[14..]);
        var count = ReadUInt32LittleEndian(buffer[16..]);
        Targets = new();
        var index = 20;
        for (var i = 0; i < count; i++)
        {
            var target = ReadUInt32LittleEndian(buffer[index..]);
            index += sizeof(uint);
            var data = ReadUInt32LittleEndian(buffer[index..]);
            index += sizeof(uint);
            _ = ReadUInt32LittleEndian(buffer[index..]);
            index += sizeof(uint);
            Targets.Add((target, data));
        }
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Sender);
        _data.CopyTo(buffer[8..12]);
        WriteUInt16LittleEndian(buffer[12..], MagicType);
        WriteUInt16LittleEndian(buffer[14..], Level);
        WriteUInt32LittleEndian(buffer[16..], (uint)Targets.Count);
        var index = 20;
        foreach (var (target, data) in Targets)
        {
            WriteUInt32LittleEndian(buffer[index..], target);
            index += sizeof(uint);
            WriteUInt32LittleEndian(buffer[index..], data);
            index += sizeof(uint);
            WriteUInt32LittleEndian(buffer[index..], 0);
            index += sizeof(uint);
        }
    }
}