namespace Conquer.Game.Messages;

public record MsgNpcInfoEx : IMessage
{
    public MsgNpcInfoEx()
    {
    }

    public MsgNpcInfoEx(DynaNpc dynaNpc) =>
        (Id, MaxLife, Life, X, Y, Look, NpcType, Sort, Strings) = (dynaNpc.Id, dynaNpc.Life, dynaNpc.Life, dynaNpc.X,
            dynaNpc.Y, (ushort)dynaNpc.LookFace, dynaNpc.Type, dynaNpc.Sort, new() { dynaNpc.Name });

    public uint Id { get; set; }
    public uint MaxLife { get; set; }
    public uint Life { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public ushort Look { get; set; }
    public ushort NpcType { get; set; }
    public ushort Sort { get; set; }
    public List<string> Strings { get; set; } = null!;
    public ushort Size => (ushort)(27 + GetByteCount(Strings));
    public MessageType Type => MessageType.MsgNpcInfoEx;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        MaxLife = ReadUInt32LittleEndian(buffer[8..]);
        Life = ReadUInt32LittleEndian(buffer[12..]);
        X = ReadUInt16LittleEndian(buffer[16..]);
        Y = ReadUInt16LittleEndian(buffer[18..]);
        Look = ReadUInt16LittleEndian(buffer[20..]);
        NpcType = ReadUInt16LittleEndian(buffer[22..]);
        Sort = ReadUInt16LittleEndian(buffer[24..]);
        Strings = ReadStrings(buffer[26..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], MaxLife);
        WriteUInt32LittleEndian(buffer[12..], Life);
        WriteUInt16LittleEndian(buffer[16..], X);
        WriteUInt16LittleEndian(buffer[18..], Y);
        WriteUInt16LittleEndian(buffer[20..], Look);
        WriteUInt16LittleEndian(buffer[22..], NpcType);
        WriteUInt16LittleEndian(buffer[24..], Sort);
        WriteStrings(buffer[26..], Strings);
    }
}