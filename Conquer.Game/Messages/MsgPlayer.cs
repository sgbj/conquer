namespace Conquer.Game.Messages;

public record MsgPlayer : IMessage
{
    public MsgPlayer()
    {
    }

    public MsgPlayer(Player player)
    {
        Id = player.Id;
        LookFace = player.LookFace;
        Head = player.GetItemInPosition(ItemPosition.Head)?.ItemTypeId ?? 0;
        Armor = player.GetItemInPosition(ItemPosition.Armor)?.ItemTypeId ?? 0;
        Right = player.GetItemInPosition(ItemPosition.Right)?.ItemTypeId ?? 0;
        Left = player.GetItemInPosition(ItemPosition.Left)?.ItemTypeId ?? 0;
        Health = player.Health;
        Level = player.Level;
        X = player.X;
        Y = player.Y;
        Hair = player.Hair;
        Direction = player.Direction;
        Reborn = player.Rebirths;
        Strings = new() { player.Name, player.Spouse?.Name ?? "None" };
    }

    public MsgPlayer(Monster monster)
    {
        Id = monster.Id;
        LookFace = monster.MonsterType.Look;
        Health = monster.Health;
        Level = monster.MonsterType.Level;
        X = monster.X;
        Y = monster.Y;
        Direction = monster.Direction;
        Strings = new() { monster.MonsterType.Name };
    }

    public uint Id { get; set; }
    public uint LookFace { get; set; }
    public uint Status { get; set; }
    public uint OwnerId { get; set; } // GuildId, GuildRank
    public uint Head { get; set; }
    public uint Armor { get; set; }
    public uint Right { get; set; }
    public uint Left { get; set; }
    public ushort Health { get; set; }
    public ushort Level { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public ushort Hair { get; set; }
    public Direction Direction { get; set; }
    public byte Pose { get; set; }
    public byte Reborn { get; set; }
    public List<string> Strings { get; set; } = new();

    public ushort Size => (ushort)(53 + GetByteCount(Strings));
    public MessageType Type => MessageType.MsgPlayer;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        LookFace = ReadUInt32LittleEndian(buffer[8..]);
        Status = ReadUInt32LittleEndian(buffer[12..]);
        OwnerId = ReadUInt32LittleEndian(buffer[16..]);
        Head = ReadUInt32LittleEndian(buffer[20..]);
        Armor = ReadUInt32LittleEndian(buffer[24..]);
        Right = ReadUInt32LittleEndian(buffer[28..]);
        Left = ReadUInt32LittleEndian(buffer[32..]);
        _ = ReadUInt32LittleEndian(buffer[36..]);
        Health = ReadUInt16LittleEndian(buffer[40..]);
        Level = ReadUInt16LittleEndian(buffer[42..]);
        X = ReadUInt16LittleEndian(buffer[44..]);
        Y = ReadUInt16LittleEndian(buffer[46..]);
        Hair = ReadUInt16LittleEndian(buffer[48..]);
        Direction = (Direction)buffer[50];
        Pose = buffer[51];
        Reborn = buffer[52];
        Strings = ReadStrings(buffer[53..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], LookFace);
        WriteUInt32LittleEndian(buffer[12..], Status);
        WriteUInt32LittleEndian(buffer[16..], OwnerId);
        WriteUInt32LittleEndian(buffer[20..], Head);
        WriteUInt32LittleEndian(buffer[24..], Armor);
        WriteUInt32LittleEndian(buffer[28..], Right);
        WriteUInt32LittleEndian(buffer[32..], Left);
        WriteUInt32LittleEndian(buffer[36..], 0);
        WriteUInt16LittleEndian(buffer[40..], Health);
        WriteUInt16LittleEndian(buffer[42..], Level);
        WriteUInt16LittleEndian(buffer[44..], X);
        WriteUInt16LittleEndian(buffer[46..], Y);
        WriteUInt16LittleEndian(buffer[48..], Hair);
        buffer[50] = (byte)Direction;
        buffer[51] = Pose;
        buffer[52] = Reborn;
        WriteStrings(buffer[53..], Strings);
    }
}