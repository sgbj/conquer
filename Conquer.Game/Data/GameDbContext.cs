using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<DynaNpc> DynaNpcs => Set<DynaNpc>();
    public DbSet<Enemy> Enemies => Set<Enemy>();
    public DbSet<Friend> Friends => Set<Friend>();
    public DbSet<Generator> Generators => Set<Generator>();
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<GuildAlly> GuildAllies => Set<GuildAlly>();
    public DbSet<GuildEnemy> GuildEnemies => Set<GuildEnemy>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemAddition> ItemAdditions => Set<ItemAddition>();
    public DbSet<ItemType> ItemTypes => Set<ItemType>();
    public DbSet<LevelExp> LevelExps => Set<LevelExp>();
    public DbSet<Magic> Magics => Set<Magic>();
    public DbSet<MagicType> MagicTypes => Set<MagicType>();
    public DbSet<Map> Maps => Set<Map>();
    public DbSet<MonsterType> MonsterTypes => Set<MonsterType>();
    public DbSet<Npc> Npcs => Set<Npc>();
    public DbSet<Passway> Passways => Set<Passway>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<PointAllot> PointAllots => Set<PointAllot>();
    public DbSet<Portal> Portals => Set<Portal>();
    public DbSet<ShopItem> ShopItems => Set<ShopItem>();
    public DbSet<WeaponSkill> WeaponSkills => Set<WeaponSkill>();
    public DbSet<WeaponSkillExp> WeaponSkillExps => Set<WeaponSkillExp>();
    public DbSet<WeaponSkillName> WeaponSkillNames => Set<WeaponSkillName>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LevelExp>()
            .HasKey(e => e.Level);

        modelBuilder.Entity<Player>()
            .Property(e => e.Id)
            .UseIdentityColumn(1000001);

        modelBuilder.Entity<WeaponSkillExp>()
            .HasKey(e => e.Level);

        modelBuilder.Entity<WeaponSkillName>()
            .HasKey(e => e.Type);
    }
}