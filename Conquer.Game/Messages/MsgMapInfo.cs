namespace Conquer.Game.Messages;

public record MsgMapInfo : IMessage
{
    public MsgMapInfo()
    {
    }

    public MsgMapInfo(Map map) => (MapId, MapDoc, MapType) = (map.Id, map.DocId, map.Type);
    public uint MapId { get; set; }
    public uint MapDoc { get; set; }
    public uint MapType { get; set; }

    public ushort Size => 16;
    public MessageType Type => MessageType.MsgMapInfo;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        MapId = ReadUInt32LittleEndian(buffer[4..]);
        MapDoc = ReadUInt32LittleEndian(buffer[8..]);
        MapType = ReadUInt32LittleEndian(buffer[12..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], MapId);
        WriteUInt32LittleEndian(buffer[8..], MapDoc);
        WriteUInt32LittleEndian(buffer[12..], MapType);
    }
}