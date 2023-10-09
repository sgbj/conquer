using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Messages;

public enum NameAction : byte
{
    Fireworks = 1,
    CreateSyn = 2,
    Syndicate = 3,
    ChangeTitle = 4,
    DelRole = 5,
    Spouse = 6,
    QueryNpc = 7,
    Wanted = 8,
    MapEffect = 9,
    RoleEffect = 10,
    MemberList = 11,
    KickOutSynMem = 12,
    QueryWanted = 13,
    QueryPoliceWanted = 14,
    PoliceWanted = 15,
    QuerySpouse = 16,
    AddDicePlayer = 17,
    DelDicePlayer = 18,
    DiceBonus = 19,
    Sound = 20,
    SynEnemy = 21,
    SynAlly = 22
}

public record MsgName : IMessage
{
    private readonly byte[] _data = new byte[8];

    public uint Id
    {
        get => ReadUInt32LittleEndian(_data);
        set => WriteUInt32LittleEndian(_data, value);
    }

    public ushort X
    {
        get => ReadUInt16LittleEndian(_data);
        set => WriteUInt16LittleEndian(_data, value);
    }

    public ushort Y
    {
        get => ReadUInt16LittleEndian(_data.AsSpan(2));
        set => WriteUInt16LittleEndian(_data.AsSpan(2), value);
    }

    public NameAction Action { get; set; }
    public List<string> Strings { get; set; } = null!;

    public ushort Size => (ushort)(9 + GetByteCount(Strings));
    public MessageType Type => MessageType.MsgName;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        buffer[4..8].CopyTo(_data);
        Action = (NameAction)buffer[8];
        Strings = ReadStrings(buffer[9..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        _data.CopyTo(buffer[4..]);
        buffer[8] = (byte)Action;
        WriteStrings(buffer[9..], Strings);
    }

    public Task HandleAsync(GameClient client, GameDbContext db, ILogger<MsgName> logger)
    {
        return Action switch
        {
            NameAction.MemberList => MemberList(),
            _ => Default()
        };

        async Task MemberList()
        {
            var members = await db.Players
                .Where(player => player.GuildId == client.Player.GuildId)
                .Select(player => new { player.Id, player.Name, player.Level, player.GuildRank })
                .ToListAsync();

            var strings = members
                .Select(member => new
                {
                    member.Id, member.Name, member.Level, member.GuildRank,
                    IsOnline = client.Server.Clients.ContainsKey(member.Id)
                })
                .OrderByDescending(member => member.IsOnline)
                .ThenByDescending(member => member.GuildRank)
                .Select(member => $"{member.Name} {member.Level} {(member.IsOnline ? 1 : 0)}")
                .ToList();

            await client.WriteAsync(new MsgName
            {
                Action = NameAction.MemberList,
                Strings = strings
            });
        }

        Task Default()
        {
            logger.LogWarning("Unhandled action {Action}", Action);
            return Task.CompletedTask;
        }
    }
}