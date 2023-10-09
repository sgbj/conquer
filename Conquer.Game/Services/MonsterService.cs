using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Services;

public class MonsterService
{
    private const uint MonsterIdFirst = 400001;
    private const uint MonsterIdLast = 499999;

    private readonly IDbContextFactory<GameDbContext> _dbContextFactory;

    private readonly object _lockObj = new();

    private uint _nextMonsterId = MonsterIdFirst;

    public MonsterService(IDbContextFactory<GameDbContext> dbContextFactory) => _dbContextFactory = dbContextFactory;

    public ConcurrentDictionary<uint, MonsterType> MonsterTypes { get; private set; } = null!;
    public ConcurrentDictionary<uint, Generator> Generators { get; private set; } = null!;
    public ConcurrentDictionary<uint, Monster> Monsters { get; } = new();

    public uint NextMonsterId()
    {
        lock (_lockObj)
        {
            var id = _nextMonsterId++;

            if (_nextMonsterId > MonsterIdLast)
            {
                _nextMonsterId = MonsterIdFirst;
            }

            return id;
        }
    }

    public async Task InitializeAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        MonsterTypes = new(await db.MonsterTypes.ToDictionaryAsync(monsterType => monsterType.Id));
        Generators = new(await db.Generators.ToDictionaryAsync(generator => generator.Id));
    }
}