namespace Conquer.Game.Messages;

public enum WalkMode
{
    Walk,
    Run
}

public record MsgWalk : IMessage
{
    public uint Id { get; set; }
    public Direction Direction { get; set; }
    public WalkMode Mode { get; set; }
    public ushort Size => 12;
    public MessageType Type => MessageType.MsgWalk;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Direction = (Direction)buffer[8];
        Mode = (WalkMode)buffer[9];
        _ = ReadUInt16LittleEndian(buffer[10..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        buffer[8] = (byte)Direction;
        buffer[9] = (byte)Mode;
        WriteUInt16LittleEndian(buffer[10..], 0);
    }

    public async Task HandleAsync(GameClient client)
    {
        client.Player.Direction = (Direction)((byte)Direction % 8);

        var (x, y) = client.Player.Direction switch
        {
            Direction.South => (0, 1),
            Direction.SouthWest => (-1, 1),
            Direction.West => (-1, 0),
            Direction.NorthWest => (-1, -1),
            Direction.North => (0, -1),
            Direction.NorthEast => (1, -1),
            Direction.East => (1, 0),
            Direction.SouthEast => (1, 1),
            _ => (0, 0)
        };

        client.Player.X += (ushort)x;
        client.Player.Y += (ushort)y;

        await client.WriteAsync(this);
    }
}