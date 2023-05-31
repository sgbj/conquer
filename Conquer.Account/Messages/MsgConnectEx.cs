using System.Net;

namespace Conquer.Account.Messages;

public enum RejectionCode : uint
{
    InvalidAccount = 1,
    ServerDown = 10,
    TryAgainLater = 11,
    Banned = 12,
    ServerBusy = 20,
    ServerFull = 21,
    UnknownError = 999
}

public record MsgConnectEx : IMessage
{
    public MsgConnectEx()
    {
    }

    public MsgConnectEx(ulong token, IPAddress ipAddress, int port) =>
        (Id, Data, Info, Port) = ((uint)token, (uint)(token >> 32), ipAddress.ToString(), (ushort)port);

    public MsgConnectEx(RejectionCode code)
    {
        Data = (uint)code;
        Info = code switch
        {
            RejectionCode.InvalidAccount => "ÕÊºÅÃû»ò¿ÚÁî´í",
            RejectionCode.ServerDown => "·þÎñÆ÷Î´Æô¶¯",
            RejectionCode.TryAgainLater => "ÇëÉÔºóÖØÐÂµÇÂ¼",
            RejectionCode.Banned => "¸ÃÕÊºÅ±»·âºÅ",
            RejectionCode.ServerBusy => "·þÎñÆ÷ÈËÊýÒÑÂú",
            RejectionCode.ServerFull => "·þÎñÆ÷Ã¦ÇëÉÔºò",
            _ => null
        };
    }

    public uint Id { get; set; }
    public uint Data { get; set; }
    public string? Info { get; set; }
    public ushort Port { get; set; }
    public ushort Size => 30;
    public MessageType Type => MessageType.MsgConnectEx;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Data = ReadUInt32LittleEndian(buffer[8..]);
        Info = ReadString(buffer[12..28]);
        Port = ReadUInt16LittleEndian(buffer[28..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], Data);
        WriteString(buffer[12..28], Info);
        WriteUInt16LittleEndian(buffer[28..], Port);
    }
}