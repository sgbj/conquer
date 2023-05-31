using Microsoft.AspNetCore.Connections;

namespace Conquer.Game;

public class GameClient : Client
{
    public GameClient(ConnectionContext connection, GameServer server) : base(connection) => Server = server;

    public GameServer Server { get; }
    public string? UserId { get; set; }
    public Player Player { get; set; } = null!;
}