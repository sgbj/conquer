using System.Drawing;

namespace Conquer.Game.Messages;

public enum TalkChannel : ushort
{
    Normal = 2000,
    Private = 2001,
    Action = 2002,
    Team = 2003,
    Syndicate = 2004,
    System = 2005,
    Family = 2006,
    Talk = 2007,
    Yelp = 2008,
    Friend = 2009,
    Global = 2010,
    Gm = 2011,
    Whisper = 2012,
    Ghost = 2013,
    Serve = 2014,
    Register = 2100,
    Entrance = 2101,
    Shop = 2102,
    PetTalk = 2103,
    CryOut = 2104,
    WebPage = 2105,
    NewMessage = 2106,
    Task = 2107,
    SynWarFirst = 2108,
    SynWarNext = 2109,
    LeaveWord = 2110,
    SynAnnounce = 2111,
    MessageBox = 2112,
    Reject = 2113,
    SynTenet = 2114,
    MsgTrade = 2201,
    MsgFriend = 2202,
    MsgTeam = 2203,
    MsgSyn = 2204,
    MsgOther = 2205,
    MsgSystem = 2206,
    Broadcast = 2500
}

[Flags]
public enum TalkStyles : ushort
{
    Normal = 0b_0000,
    Scroll = 0b_0001,
    Flash = 0b_0010,
    Blast = 0b_1000
}

public record MsgTalk : IMessage
{
    public MsgTalk()
    {
    }

    public MsgTalk(TalkChannel channel, params string[] strings) =>
        (Channel, Strings) = (channel, strings.ToList());

    public Color Color { get; set; } = Color.White;
    public TalkChannel Channel { get; set; }
    public TalkStyles Styles { get; set; } = TalkStyles.Normal;
    public uint Timestamp { get; set; } = (uint)Environment.TickCount;
    public List<string> Strings { get; set; } = null!;

    public ushort Size => (ushort)(16 + GetByteCount(Strings));
    public MessageType Type => MessageType.MsgTalk;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Color = Color.FromArgb(ReadInt32LittleEndian(buffer[4..]));
        Channel = (TalkChannel)ReadUInt16LittleEndian(buffer[8..]);
        Styles = (TalkStyles)ReadUInt16LittleEndian(buffer[10..]);
        Timestamp = ReadUInt32LittleEndian(buffer[12..]);
        Strings = ReadStrings(buffer[16..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteInt32LittleEndian(buffer[4..], Color.FromArgb(0, Color).ToArgb());
        WriteUInt16LittleEndian(buffer[8..], (ushort)Channel);
        WriteUInt16LittleEndian(buffer[10..], (ushort)Styles);
        WriteUInt32LittleEndian(buffer[12..], Timestamp);
        WriteStrings(buffer[16..], Strings);
    }

    public async Task HandleAsync(GameClient client, CommandService commandService)
    {
        // Command
        if (Strings.Count >= 4 && Strings[3].StartsWith('/'))
        {
            await commandService.HandleAsync(client, Strings[3]);
        }
    }
}