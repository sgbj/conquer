namespace Conquer.Game.Messages;

public enum ActionType : ushort
{
    GetPosition = 74,
    GetItemSet = 75,
    GetGoodFriend = 76,
    GetWeaponSkillSet = 77,
    GetMagicSet = 78,
    ChgDir = 79,
    Emotion = 81,
    ChgMap = 85,
    EnterMap = 86,
    UpLev = 92,
    XpClear = 93,
    Reborn = 94,
    DelRole = 95,
    SetPkMode = 96,
    GetSynAttr = 97,
    Mine = 99,
    BotCheckA = 100,
    QueryPlayer = 102,
    MapArgb = 104,
    TeamMemberPos = 106,
    DropMagic = 109,
    DropSkill = 110,
    CreateBooth = 111,
    DestroyBooth = 114,
    PostCmd = 116,
    QueryEquipment = 117,
    AbortTransform = 118,
    TakeOff = 120,
    GetMoney = 121,
    QueryEnemyInfo = 123,
    OpenDialog = 126,
    LoginCompleted = 130,
    LeaveMap = 132,
    Jump = 133,
    Ghost = 137,
    Synchro = 138,
    QueryFriendInfo = 140,
    ChangeFace = 142
}

public record MsgAction : IMessage
{
    private readonly byte[] _data = new byte[8];
    public uint Timestamp { get; set; } = (uint)Environment.TickCount;
    public uint Id { get; set; }

    public uint DataUInt1
    {
        get => ReadUInt32LittleEndian(_data);
        set => WriteUInt32LittleEndian(_data, value);
    }

    public ushort DataUShort1
    {
        get => ReadUInt16LittleEndian(_data);
        set => WriteUInt16LittleEndian(_data, value);
    }

    public ushort DataUShort2
    {
        get => ReadUInt16LittleEndian(_data.AsSpan(2));
        set => WriteUInt16LittleEndian(_data.AsSpan(2), value);
    }

    public uint DataUInt2
    {
        get => ReadUInt32LittleEndian(_data.AsSpan(4));
        set => WriteUInt32LittleEndian(_data.AsSpan(4), value);
    }

    public ushort DataUShort3
    {
        get => ReadUInt16LittleEndian(_data.AsSpan(4));
        set => WriteUInt16LittleEndian(_data.AsSpan(4), value);
    }

    public ushort DataUShort4
    {
        get => ReadUInt16LittleEndian(_data.AsSpan(6));
        set => WriteUInt16LittleEndian(_data.AsSpan(6), value);
    }

    public ushort Direction { get; set; }
    public ActionType Action { get; set; }

    public ushort Size => 28;
    public MessageType Type => MessageType.MsgAction;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Timestamp = ReadUInt32LittleEndian(buffer[4..]);
        Id = ReadUInt32LittleEndian(buffer[8..]);
        buffer[12..20].CopyTo(_data);
        Direction = ReadUInt16LittleEndian(buffer[20..]);
        Action = (ActionType)ReadUInt16LittleEndian(buffer[22..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Timestamp);
        WriteUInt32LittleEndian(buffer[8..], Id);
        _data.CopyTo(buffer[12..20]);
        WriteUInt16LittleEndian(buffer[20..], Direction);
        WriteUInt16LittleEndian(buffer[22..], (ushort)Action);
    }

    public Task HandleAsync(GameClient client, ILogger<MsgAction> logger)
    {
        return Action switch
        {
            ActionType.GetPosition => GetPosition(),
            ActionType.ChgMap => ChgMap(),
            ActionType.Jump => Jump(),
            _ => Default()
        };

        async Task GetPosition()
        {
            Id = client.Player.Id;
            DataUInt1 = client.Player.MapId;
            DataUShort3 = client.Player.X;
            DataUShort4 = client.Player.Y;
            await client.WriteAsync(this);
        }

        async Task ChgMap()
        {
            client.Player.MapId = 1002;
            client.Player.X = 438;
            client.Player.Y = 377;
            await client.WriteAsync(new MsgAction
            {
                Id = client.Player.Id,
                DataUInt1 = client.Player.MapId,
                DataUShort3 = client.Player.X,
                DataUShort4 = client.Player.Y,
                Action = ActionType.EnterMap
            });
        }

        async Task Jump()
        {
            client.Player.X = DataUShort1;
            client.Player.Y = DataUShort2;
            await client.WriteAsync(this);
        }

        async Task Default()
        {
            logger.LogWarning("Unhandle action {Action}.", Action);
            await client.WriteAsync(this);
        }
    }
}