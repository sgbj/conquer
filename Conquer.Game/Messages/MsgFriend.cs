using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Messages;

public enum FriendAction : byte
{
    FriendApply = 10,
    FriendAccept = 11,
    FriendOnline = 12,
    FriendOffline = 13,
    FriendBreak = 14,
    GetInfo = 15,
    EnemyOnline = 16,
    EnemyOffline = 17,
    EnemyDel = 18,
    EnemyAdd = 19
}

public record MsgFriend : IMessage
{
    public MsgFriend()
    {
    }

    public MsgFriend(uint id, FriendAction action, bool isOnline, string name) =>
        (Id, Action, IsOnline, Name) = (id, action, isOnline, name);

    public uint Id { get; set; }
    public FriendAction Action { get; set; }
    public bool IsOnline { get; set; }
    public string Name { get; set; } = null!;
    public ushort Size => 28;
    public MessageType Type => MessageType.MsgFriend;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Action = (FriendAction)buffer[8];
        IsOnline = buffer[9] != 0;
        _ = ReadUInt16LittleEndian(buffer[10..]);
        Name = ReadString(buffer[12..28]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        buffer[8] = (byte)Action;
        buffer[9] = (byte)(IsOnline ? 1 : 0);
        WriteUInt16LittleEndian(buffer[10..], 0);
        WriteString(buffer[12..28], Name);
    }

    public Task HandleAsync(GameClient client, GameDbContext db, ILogger<MsgFriend> logger)
    {
        return Action switch
        {
            FriendAction.FriendApply => FriendApply(),
            FriendAction.FriendBreak => FriendBreak(),
            _ => Default()
        };

        async Task FriendApply()
        {
            if (client.Server.Clients.TryGetValue(Id, out var other))
            {
                await other.WriteAsync(new MsgFriend(client.Player.Id, FriendAction.FriendApply, true,
                    client.Player.Name));
            }
        }

        async Task FriendBreak()
        {
            await db.Friends
                .Where(friend => (friend.PlayerId == client.Player.Id && friend.FriendPlayerId == Id) ||
                                 (friend.PlayerId == Id && friend.FriendPlayerId == client.Player.Id))
                .ExecuteDeleteAsync();

            if (client.Server.Clients.TryGetValue(Id, out var other))
            {
                await other.WriteAsync(new MsgFriend(client.Player.Id, FriendAction.FriendBreak, true,
                    client.Player.Name));
            }

            await client.WriteAsync(this);
        }

        async Task Default()
        {
            logger.LogWarning("Unhandled action {Action}.", Action);
            await client.WriteAsync(this);
        }
    }
}