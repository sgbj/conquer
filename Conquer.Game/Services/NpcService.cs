using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Services;

public class NpcService
{
    private readonly IDbContextFactory<GameDbContext> _dbContextFactory;
    private readonly MapService _mapService;

    public NpcService(IDbContextFactory<GameDbContext> dbContextFactory, MapService mapService) =>
        (_dbContextFactory, _mapService) = (dbContextFactory, mapService);

    public ConcurrentDictionary<uint, Npc> Npcs { get; private set; } = null!;
    public ConcurrentDictionary<uint, DynaNpc> DynaNpcs { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        Npcs = new(await db.Npcs.ToDictionaryAsync(npc => npc.Id));
        DynaNpcs = new(await db.DynaNpcs.ToDictionaryAsync(dynaNpc => dynaNpc.Id));

        foreach (var npc in Npcs.Values)
        {
            if (_mapService.GameMaps.TryGetValue(npc.MapId, out var gameMap))
            {
                await gameMap.AddAsync(npc);
            }
        }

        foreach (var dynaNpc in DynaNpcs.Values)
        {
            if (_mapService.GameMaps.TryGetValue(dynaNpc.MapId, out var gameMap))
            {
                await gameMap.AddAsync(dynaNpc);
            }
        }
    }
}