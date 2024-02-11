using Microsoft.EntityFrameworkCore;

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

    public Task HandleAsync(GameClient client, GameDbContext db, MapService mapService, ILogger<MsgAction> logger)
    {
        return Action switch
        {
            ActionType.GetPosition => GetPosition(),
            ActionType.GetItemSet => GetItemSet(),
            ActionType.GetGoodFriend => GetGoodFriend(),
            ActionType.GetWeaponSkillSet => GetWeaponSkillSet(),
            ActionType.GetMagicSet => GetMagicSet(),
            ActionType.ChgDir => ChgDir(),
            ActionType.Emotion => Emotion(),
            ActionType.ChgMap => ChgMap(),
            ActionType.DelRole => DelRole(),
            ActionType.SetPkMode => SetPkMode(),
            ActionType.GetSynAttr => GetSynAttr(),
            ActionType.DestroyBooth => DestroyBooth(),
            ActionType.QueryEnemyInfo => QueryEnemyInfo(),
            ActionType.LoginCompleted => LoginCompleted(),
            ActionType.QueryFriendInfo => QueryFriendInfo(),
            ActionType.Jump => Jump(),
            ActionType.ChangeFace => ChangeFace(),
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

        async Task GetItemSet()
        {
            foreach (var item in client.Player.Items)
            {
                await client.WriteAsync(new MsgItemInfo(item, ItemInfoAction.AddItem));
            }

            await client.WriteAsync(this);
        }

        async Task GetGoodFriend()
        {
            var friends = await db.Friends
                .Where(friend => friend.PlayerId == client.Player.Id)
                .Select(friend => new { friend.FriendPlayerId, FriendPlayerName = friend.FriendPlayer.Name })
                .ToListAsync();

            foreach (var friend in friends)
            {
                await client.WriteAsync(new MsgFriend(friend.FriendPlayerId, FriendAction.GetInfo,
                    client.Server.Clients.ContainsKey(friend.FriendPlayerId), friend.FriendPlayerName));
            }

            var enemies = await db.Enemies
                .Where(enemy => enemy.PlayerId == client.Player.Id)
                .Select(enemy => new { enemy.EnemyPlayerId, EnemyPlayerName = enemy.EnemyPlayer.Name })
                .ToListAsync();

            foreach (var enemy in enemies)
            {
                await client.WriteAsync(new MsgFriend(enemy.EnemyPlayerId, FriendAction.EnemyAdd,
                    client.Server.Clients.ContainsKey(enemy.EnemyPlayerId), enemy.EnemyPlayerName));
            }

            await client.WriteAsync(this);
        }

        async Task GetWeaponSkillSet()
        {
            foreach (var weaponSkill in client.Player.WeaponSkills)
            {
                await client.WriteAsync(new MsgWeaponSkill(weaponSkill));
            }

            await client.WriteAsync(this);
        }

        async Task GetMagicSet()
        {
            foreach (var magic in client.Player.Magics)
            {
                await client.WriteAsync(new MsgMagicInfo(magic));
            }

            await client.WriteAsync(this);
        }

        async Task ChgDir()
        {
            client.Player.Direction = (Direction)Direction;

            await client.WriteScreenAsync(this);
        }

        async Task Emotion()
        {
            await client.WriteScreenAsync(this);
        }

        async Task ChgMap()
        {
            var portal = mapService.GetPortal(client.Player.MapId, DataUShort1, DataUShort2);

            if (portal is null)
            {
                return;
            }

            if (mapService.GameMaps.TryGetValue(client.Player.MapId, out var gameMap))
            {
                await client.GameMap.RemoveAsync(client.Player);

                client.Player.MapId = portal.MapId;
                client.Player.X = portal.PortalX;
                client.Player.Y = portal.PortalY;

                await client.WriteAsync(new MsgAction
                {
                    Id = client.Player.Id,
                    DataUInt1 = client.Player.MapId,
                    DataUShort3 = client.Player.X,
                    DataUShort4 = client.Player.Y,
                    Action = ActionType.EnterMap
                });

                await gameMap.AddAsync(client.Player);
            }
        }

        async Task DelRole()
        {
            db.Remove(client.Player);
            await db.SaveChangesAsync();
            client.Abort();
        }

        async Task SetPkMode()
        {
            client.Player.PkMode = (PkMode)DataUInt1;
            await client.WriteAsync(this);
        }

        async Task GetSynAttr()
        {
            if (client.Player.GuildId.HasValue)
            {
                var guild = db.Guilds
                    .Select(guild => new
                    {
                        guild.Id,
                        guild.Name,
                        guild.Fund,
                        Leader = guild.Members.FirstOrDefault(member => member.GuildRank == GuildRank.Leader)!.Name,
                        Members = guild.Members.Count,
                        guild.Allies,
                        guild.Enemies
                    })
                    .AsSplitQuery()
                    .First(guild => guild.Id == client.Player.GuildId);

                await client.WriteAsync(new MsgSyndicateAttributeInfo
                {
                    GuildId = guild.Id,
                    Donation = client.Player.GuildDonation,
                    Fund = guild.Fund,
                    Members = (uint)guild.Members,
                    GuildRank = client.Player.GuildRank,
                    Leader = guild.Leader
                });

                foreach (var ally in guild.Allies)
                {
                    await client.WriteAsync(new MsgSyndicate
                    {
                        Data = ally.AllyGuildId,
                        Action = SyndicateAction.SetAlly
                    });
                }

                foreach (var enemy in guild.Enemies)
                {
                    await client.WriteAsync(new MsgSyndicate
                    {
                        Data = enemy.EnemyGuildId,
                        Action = SyndicateAction.SetAntagonize
                    });
                }
            }

            await client.WriteAsync(this);
        }

        async Task DestroyBooth()
        {
            await client.WriteAsync(this);
        }

        async Task QueryEnemyInfo()
        {
            if (client.Server.Clients.TryGetValue(DataUInt1, out var other))
            {
                await client.WriteAsync(new MsgFriendInfo(other.Player));
            }
        }

        async Task LoginCompleted()
        {
            if (mapService.GameMaps.TryGetValue(client.Player.MapId, out var gameMap))
            {
                await gameMap.AddAsync(client.Player);
            }
        }

        async Task Jump()
        {
            client.Player.X = DataUShort1;
            client.Player.Y = DataUShort2;

            await client.WriteScreenAsync(this);
            await client.GameMap.UpdateAsync(client.Player);
        }

        async Task QueryFriendInfo()
        {
            if (client.Server.Clients.TryGetValue(DataUInt1, out var other))
            {
                await client.WriteAsync(new MsgFriendInfo(other.Player));
            }
        }

        async Task ChangeFace()
        {
            client.Player.Avatar = DataUShort1;
            await client.WriteScreenAsync(this);
        }

        async Task Default()
        {
            logger.LogWarning("Unhandled action {Action}.", Action);
            await client.WriteAsync(this);
        }
    }
}