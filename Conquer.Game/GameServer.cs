using System.Collections.Concurrent;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;

namespace Conquer.Game;

public class GameServerOptions
{
    public string Name { get; set; } = null!;
}

public class GameServer : ConnectionHandler
{
    private readonly MessageService _messageService;
    private readonly GameServerOptions _options;

    public GameServer(MessageService messageService, IOptions<GameServerOptions> options)
    {
        _messageService = messageService;
        _options = options.Value;
    }

    public string Name => _options.Name;
    public ConcurrentDictionary<uint, GameClient> Clients { get; } = new();

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        await using var client = new GameClient(connection, this);

        while (true)
        {
            var message = await client.ReadAsync();

            if (message is null)
            {
                break;
            }

            await _messageService.HandleAsync(client, message);
        }

        await client.OnDisconnectedAsync();
    }
}