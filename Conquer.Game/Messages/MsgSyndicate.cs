using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Messages;

public enum SyndicateAction : uint
{
    None = 0,
    ApplyJoin = 1,
    InviteJoin = 2,
    LeaveSyn = 3,
    KickOutMember = 4,
    QuerySynName = 6,
    SetAlly = 7,
    ClearAlly = 8,
    SetAntagonize = 9,
    ClearAntagonize = 10,
    DonateMoney = 11,
    QuerySynAttr = 12,
    SetSyn = 14,
    UniteSubSyn = 15,
    UniteSyn = 16,
    SetWhiteSyn = 17,
    SetBlackSyn = 18,
    DestroySyn = 19,
    SetMantle = 20
}

public record MsgSyndicate : IMessage
{
    public SyndicateAction Action { get; set; }
    public uint Data { get; set; }
    public ushort Size => 12;
    public MessageType Type => MessageType.MsgSyndicate;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Action = (SyndicateAction)ReadUInt32LittleEndian(buffer[4..]);
        Data = ReadUInt32LittleEndian(buffer[8..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], (uint)Action);
        WriteUInt32LittleEndian(buffer[8..], Data);
    }

    public Task HandleAsync(GameClient client, GameDbContext db, ILogger<MsgSyndicate> logger)
    {
        return Action switch
        {
            SyndicateAction.QuerySynName => QuerySynName(),
            SyndicateAction.QuerySynAttr => QuerySynAttr(),
            _ => Default()
        };

        async Task QuerySynName()
        {
            var names = await db.Guilds
                .Where(guild => guild.Id == Data)
                .Select(guild => new { ParentName = "Parent", guild.Name })
                .FirstOrDefaultAsync();

            if (names is null)
            {
                return;
            }

            var strings = new List<string>();

            if (names.ParentName is { })
            {
                strings.Add(names.ParentName);
            }

            strings.Add(names.Name);

            await client.WriteAsync(new MsgName
            {
                Id = Data,
                Action = NameAction.Syndicate,
                Strings = strings
            });
        }

        async Task QuerySynAttr()
        {
            if (!client.Server.Clients.TryGetValue(Data, out var other) || other.Player.GuildId is null)
            {
                return;
            }

            var guild = await db.Guilds
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
                .FirstAsync(guild => guild.Id == other.Player.GuildId);

            await client.WriteAsync(new MsgSyndicateAttributeInfo
            {
                GuildId = guild.Id,
                Donation = other.Player.GuildDonation,
                Fund = guild.Fund,
                Members = (uint)guild.Members,
                GuildRank = other.Player.GuildRank,
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

        Task Default()
        {
            logger.LogWarning("Unhandled action {Action}.", Action);
            return Task.CompletedTask;
        }
    }
}