using Conquer.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Conquer.Game.Messages;

public record MsgConnect : IMessage
{
    public uint Id { get; set; }
    public uint Data { get; set; }
    public string Info { get; set; } = null!;
    public ushort Size => 28;
    public MessageType Type => MessageType.MsgConnect;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Data = ReadUInt32LittleEndian(buffer[8..]);
        Info = ReadString(buffer[12..28]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], Data);
        WriteString(buffer[12..28], Info);
    }

    public async Task HandleAsync(GameClient client, GameServer server, GameDbContext db, IMemoryCache cache)
    {
        // Update cipher keys
        if (client.ConnectionCipher is TqConnectionCipher connectionCipher)
        {
            connectionCipher.SetKeys(Id, Data);
        }

        var token = ((ulong)Data << 32) | Id;
        client.UserId = cache.Get<string>($"Transfer:{token}");

        if (client.UserId is null)
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Entrance, "SYSTEM", "ALLUSERS", "", "Invalid user."));
            return;
        }

        var player = await db.Players
            .Include(p => p.Spouse)
            .Include(p => p.Items)
            .Include(p => p.Magics)
            .Include(p => p.WeaponSkills)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.UserId == client.UserId);

        // New player
        if (player is null)
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Entrance, "SYSTEM", "ALLUSERS", "", "NEW_ROLE"));
            return;
        }

        client.Player = player;
        player.Client = client;

        // Already logged in
        if (server.Clients.TryGetValue(player.Id, out var existing))
        {
            existing.Abort();
        }

        server.Clients.TryAdd(player.Id, client);

        await client.WriteAsync(new MsgTalk(TalkChannel.Entrance, "SYSTEM", "ALLUSERS", "", "ANSWER_OK"));
        await client.WriteAsync(new MsgUserInfo(player));
        await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", $"Welcome to {server.Name}!"));
    }
}