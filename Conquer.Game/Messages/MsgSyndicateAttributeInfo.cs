namespace Conquer.Game.Messages;

public enum GuildRank : byte
{
    None = 0,
    Member = 50,
    InternMgr = 60,
    DeputyMgr = 70,
    BranchMgr = 80,
    SubLeader = 90,
    Leader = 100
}

public record MsgSyndicateAttributeInfo : IMessage
{
    public uint GuildId { get; set; }
    public uint Donation { get; set; }
    public uint Fund { get; set; }
    public uint Members { get; set; }
    public GuildRank GuildRank { get; set; }
    public string Leader { get; set; } = null!;
    public ushort Size => 37;
    public MessageType Type => MessageType.MsgSyndicateAttributeInfo;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        GuildId = ReadUInt32LittleEndian(buffer[4..]);
        Donation = ReadUInt32LittleEndian(buffer[8..]);
        Fund = ReadUInt32LittleEndian(buffer[12..]);
        Members = ReadUInt32LittleEndian(buffer[16..]);
        GuildRank = (GuildRank)buffer[20];
        Leader = ReadString(buffer[21..37]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], GuildId);
        WriteUInt32LittleEndian(buffer[8..], Donation);
        WriteUInt32LittleEndian(buffer[12..], Fund);
        WriteUInt32LittleEndian(buffer[16..], Members);
        buffer[20] = (byte)GuildRank;
        WriteString(buffer[21..37], Leader);
    }
}