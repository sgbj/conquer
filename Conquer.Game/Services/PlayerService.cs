using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Services;

public class PlayerService
{
    private readonly IDbContextFactory<GameDbContext> _dbContextFactory;

    public PlayerService(IDbContextFactory<GameDbContext> dbContextFactory) => _dbContextFactory = dbContextFactory;

    public ConcurrentDictionary<byte, LevelExp> LevelExps { get; private set; } = null!;
    public ConcurrentDictionary<uint, MagicType> MagicTypes { get; private set; } = null!;
    public ConcurrentDictionary<uint, PointAllot> PointAllots { get; private set; } = null!;
    public ConcurrentDictionary<byte, WeaponSkillExp> WeaponSkillExps { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        LevelExps = new(await db.LevelExps.ToDictionaryAsync(levelExp => levelExp.Level));
        MagicTypes = new(await db.MagicTypes.ToDictionaryAsync(magicType => magicType.Id));
        PointAllots = new(await db.PointAllots.ToDictionaryAsync(pointAllot => pointAllot.Id));
        WeaponSkillExps = new(await db.WeaponSkillExps.ToDictionaryAsync(weaponSkillExp => weaponSkillExp.Level));
    }
}