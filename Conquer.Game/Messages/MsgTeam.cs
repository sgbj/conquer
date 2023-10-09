namespace Conquer.Game.Messages;

public enum TeamAction
{
    Create = 0,
    ApplyJoin = 1,
    Leave = 2,
    AcceptInvite = 3,
    Invite = 4,
    AgreeJoin = 5,
    Dismiss = 6,
    KickOut = 7,
    CloseTeam = 8,
    OpenTeam = 9,
    CloseMoneyAccess = 10,
    OpenMoneyAccess = 11,
    CloseItemAccess = 12,
    OpenItemAccess = 13
}

public record MsgTeam : IMessage
{
    public TeamAction Action { get; set; }
    public uint Id { get; set; }
    public ushort Size => 12;
    public MessageType Type => MessageType.MsgTeam;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Action = (TeamAction)ReadUInt32LittleEndian(buffer[4..]);
        Id = ReadUInt32LittleEndian(buffer[8..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], (uint)Action);
        WriteUInt32LittleEndian(buffer[8..], Id);
    }
}