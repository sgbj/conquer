namespace Conquer.Game.Models;

public class GameMap
{
    private const int MaximumDistance = 17;
    private readonly QuadTree<IEntity> _quadTree;

    public GameMap(Map map, DMap dMap)
    {
        Map = map;
        DMap = dMap;
        _quadTree = new() { Bounds = new(0, 0, (int)DMap.Width, (int)DMap.Height) };
    }

    public Map Map { get; }
    public DMap DMap { get; }

    public async Task AddAsync(IEntity entity)
    {
        _quadTree.Insert(entity, new(entity.X, entity.Y, 1, 1));

        if (entity is Player player)
        {
            player.Client.GameMap = this;
        }

        await UpdateScreensInRange(entity.X, entity.Y, MaximumDistance);
    }

    public async Task UpdateAsync(IEntity entity)
    {
        _quadTree.Remove(entity);
        _quadTree.Insert(entity, new(entity.X, entity.Y, 1, 1));

        if (entity is Player player)
        {
            player.Client.GameMap = this;
        }

        await UpdateScreensInRange(entity.X, entity.Y, MaximumDistance);
    }

    public async Task RemoveAsync(IEntity entity)
    {
        _quadTree.Remove(entity);

        await UpdateScreensInRange(entity.X, entity.Y, MaximumDistance);
    }

    private async Task UpdateScreensInRange(int x, int y, int range)
    {
        var entities = _quadTree.GetNodesInside(new(x - range, y - range, range * 2, range * 2));

        foreach (var entity in entities)
        {
            if (entity is Player player)
            {
                await UpdateScreenAsync(player.Client);
            }
        }
    }

    private async Task UpdateScreenAsync(GameClient client)
    {
        foreach (var entity in client.Screen.Values)
        {
            if (entity.MapId != client.Player.MapId || GetDistance(entity, client.Player) > MaximumDistance)
            {
                client.Screen.TryRemove(entity.Id, out _);
            }
        }

        var entities = _quadTree.GetNodesInside(new(client.Player.X - MaximumDistance,
            client.Player.Y - MaximumDistance,
            MaximumDistance * 2, MaximumDistance * 2));

        foreach (var entity in entities)
        {
            if (entity == client.Player)
            {
                continue;
            }

            if (client.Screen.TryAdd(entity.Id, entity))
            {
                if (entity is Player player)
                {
                    await client.WriteAsync(new MsgPlayer(player));
                }
                else if (entity is Monster monster)
                {
                    await client.WriteAsync(new MsgPlayer(monster));
                }
                else if (entity is Npc npc)
                {
                    await client.WriteAsync(new MsgNpcInfo(npc));
                }
                else if (entity is DynaNpc dynaNpc)
                {
                    await client.WriteAsync(new MsgNpcInfoEx(dynaNpc));
                }
                else if (entity is MapItem mapItem)
                {
                    await client.WriteScreenAsync(new MsgMapItem(mapItem));
                }
            }
        }
    }

    private static int GetDistance(IEntity a, IEntity b) =>
        (int)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

    public IEntity? Find(uint id) => null;

    public IEnumerable<IEntity> FindRange(ushort x, ushort y, int range) => default!;

    public bool IsValid(ushort x, ushort y) => x < DMap.Width && y < DMap.Height && DMap.Cells[x, y].Mask > 0;
}