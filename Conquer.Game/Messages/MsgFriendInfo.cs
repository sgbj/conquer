namespace Conquer.Game.Messages;

public record MsgFriendInfo : IMessage
{
    public MsgFriendInfo()
    {
    }

    public MsgFriendInfo(Player player)
    {
        Id = player.Id;
        Avatar = player.Avatar;
        Level = player.Level;
        Profession = player.Profession;
        PkPoints = player.PkPoints;
        GuildRank = GuildRank.Leader;
        GuildId = 1;
        Spouse = player.Spouse?.Name ?? "None";
    }

    public uint Id { get; set; }
    public uint Avatar { get; set; }
    public byte Level { get; set; }
    public Profession Profession { get; set; }
    public short PkPoints { get; set; }
    public GuildRank GuildRank { get; set; }
    public uint GuildId { get; set; }
    public string Spouse { get; set; } = null!;

    public ushort Size => 36;
    public MessageType Type => MessageType.MsgFriendInfo;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Avatar = ReadUInt32LittleEndian(buffer[8..]);
        Level = buffer[12];
        Profession = (Profession)buffer[13];
        PkPoints = ReadInt16LittleEndian(buffer[14..]);
        var guildIdRank = ReadUInt32LittleEndian(buffer[16..]);
        GuildRank = (GuildRank)(guildIdRank >> 24);
        GuildId = guildIdRank & 0x00FFFFFF;
        Spouse = ReadString(buffer[20..36]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        buffer[12] = Level;
        buffer[13] = (byte)Profession;
        WriteInt16LittleEndian(buffer[14..], PkPoints);
        var guildIdRank = ((uint)GuildRank << 24) | (GuildId & 0x00FFFFFF);
        GuildRank = (GuildRank)(guildIdRank >> 24);
        GuildId = guildIdRank & 0x00FFFFFF;
        WriteString(buffer[20..36], Spouse);
    }
}