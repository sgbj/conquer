using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>()
            .Property(e => e.Id)
            .UseIdentityColumn(1000001);
    }
}