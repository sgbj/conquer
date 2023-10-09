using System.Collections.Concurrent;
using Microsoft.AspNetCore.Connections;

namespace Conquer.Game;

public class GameClient : Client
{
    public GameClient(ConnectionContext connection, GameServer server) : base(connection) => Server = server;

    public GameServer Server { get; }
    public string? UserId { get; set; }
    public Player Player { get; set; } = null!;
    public GameMap GameMap { get; set; } = null!;
    public ConcurrentDictionary<uint, IEntity> Screen { get; } = new();

    public async Task WriteScreenAsync(IMessage message, bool includeSelf = true)
    {
        if (includeSelf)
        {
            await WriteAsync(message);
        }

        foreach (var entity in Screen.Values)
        {
            if (entity is Player player)
            {
                await player.Client.WriteAsync(message);
            }
        }
    }

    public async Task OnDisconnectedAsync()
    {
        if (Player is { })
        {
            if (GameMap is { })
            {
                await GameMap.RemoveAsync(Player);
            }

            Server.Clients.TryRemove(Player.Id, out _);
        }
    }
}