using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Services;

public class MapService
{
    private readonly IDbContextFactory<GameDbContext> _dbContextFactory;
    private readonly DMapService _dMapService;

    public MapService(IDbContextFactory<GameDbContext> dbContextFactory, DMapService dMapService)
    {
        _dbContextFactory = dbContextFactory;
        _dMapService = dMapService;
    }

    public ConcurrentDictionary<uint, Map> Maps { get; private set; } = null!;
    public ConcurrentDictionary<uint, Passway> Passways { get; private set; } = null!;
    public ConcurrentDictionary<uint, Portal> Portals { get; private set; } = null!;
    public ConcurrentDictionary<uint, GameMap> GameMaps { get; } = new();

    public Portal? GetPortal(uint mapId, ushort x, ushort y)
    {
        if (!GameMaps.TryGetValue(mapId, out var gameMap))
        {
            return null;
        }

        var portal = gameMap.DMap.Portals.FirstOrDefault(portal => portal.PortalX == x && portal.PortalY == y);

        if (portal is null)
        {
            return null;
        }

        var passway =
            Passways.Values.FirstOrDefault(passway => passway.MapId == mapId && passway.PasswayIdx == portal.Idx);

        return passway is { }
            ? Portals.Values.FirstOrDefault(p => p.MapId == passway.PortalMapId && p.Idx == passway.PortalIdx)
            : null;
    }

    public async Task InitializeAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        Maps = new(await db.Maps.ToDictionaryAsync(map => map.Id));
        Passways = new(await db.Passways.ToDictionaryAsync(passway => passway.Id));
        Portals = new(await db.Portals.ToDictionaryAsync(portal => portal.Id));

        foreach (var map in Maps.Values)
        {
            if (!_dMapService.DMaps.TryGetValue(map.DocId, out var dMap))
            {
                continue;
            }

            GameMaps.TryAdd(map.Id, new(map, dMap));
        }
    }
}