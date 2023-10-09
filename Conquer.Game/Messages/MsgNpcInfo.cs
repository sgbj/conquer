namespace Conquer.Game.Messages;

public record MsgNpcInfo : IMessage
{
    public MsgNpcInfo()
    {
    }

    public MsgNpcInfo(Npc npc) =>
        (Id, X, Y, LookFace, NpcType, NpcSort, Strings) =
        (npc.Id, npc.X, npc.Y, (ushort)npc.LookFace, npc.Type, npc.Sort, new() { npc.Name });

    public uint Id { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public ushort LookFace { get; set; }
    public ushort NpcType { get; set; }
    public ushort NpcSort { get; set; }
    public List<string> Strings { get; set; } = null!;

    public ushort Size => (ushort)(18 + GetByteCount(Strings));
    public MessageType Type => MessageType.MsgNpcInfo;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        X = ReadUInt16LittleEndian(buffer[8..]);
        Y = ReadUInt16LittleEndian(buffer[10..]);
        LookFace = ReadUInt16LittleEndian(buffer[12..]);
        NpcType = ReadUInt16LittleEndian(buffer[14..]);
        NpcSort = ReadUInt16LittleEndian(buffer[16..]);
        Strings = ReadStrings(buffer[18..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt16LittleEndian(buffer[8..], X);
        WriteUInt16LittleEndian(buffer[10..], Y);
        WriteUInt16LittleEndian(buffer[12..], LookFace);
        WriteUInt16LittleEndian(buffer[14..], NpcType);
        WriteUInt16LittleEndian(buffer[16..], NpcSort);
        WriteStrings(buffer[18..], Strings);
    }
}