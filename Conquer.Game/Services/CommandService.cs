namespace Conquer.Game.Services;

public class CommandService
{
    private readonly GameServer _server;

    public CommandService(GameServer server) => _server = server;

    public Task HandleAsync(GameClient client, string command)
    {
        var args = command.Split();

        return args[0] switch
        {
            "/quit" => Quit(),
            "/online" => Online(),
            "/map" => Map(),
            "/avatar" => Avatar(),
            _ => Default()
        };

        Task Quit()
        {
            client.Abort();
            return Task.CompletedTask;
        }

        async Task Online()
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "",
                $"{_server.Clients.Count} players online."));
        }

        async Task Map()
        {
            if (args.Length != 4 ||
                !uint.TryParse(args[1], out var mapId) ||
                !ushort.TryParse(args[2], out var x) ||
                !ushort.TryParse(args[3], out var y))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            client.Player.MapId = mapId;
            client.Player.X = x;
            client.Player.Y = y;

            await client.WriteAsync(new MsgAction
            {
                Id = client.Player.Id,
                DataUInt1 = mapId,
                DataUShort3 = x,
                DataUShort4 = y,
                Action = ActionType.EnterMap
            });
        }

        async Task Avatar()
        {
            if (args.Length != 2 ||
                !ushort.TryParse(args[1], out var avatar))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            client.Player.Avatar = avatar;

            await client.WriteAsync(new MsgAction
            {
                Id = client.Player.Id,
                DataUInt1 = avatar,
                Action = ActionType.ChangeFace
            });
        }

        async Task Default()
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid command."));
        }
    }
}