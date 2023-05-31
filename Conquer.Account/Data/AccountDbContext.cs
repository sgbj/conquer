using System.Net;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Conquer.Account.Data;

public class AccountDbContext : IdentityDbContext<User>
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    public DbSet<Server> Servers => Set<Server>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Server>().HasData(
            new Server
            {
                Id = 1,
                Name = "Classic",
                IpAddress = IPAddress.Loopback,
                Port = 5816,
                Url = "http://localhost:5817"
            });

        builder.Entity<User>().HasData(
            // Test1 Password1
            new User
            {
                Id = "9ab1c52c-1511-4054-a581-ab4a5769d54b",
                UserName = "Test1",
                NormalizedUserName = "TEST1",
                PasswordHash = "AQAAAAIAAYagAAAAEF2uDyrJ56FZZm1csPUAgTs+E4lTrYpk7qCR8LSqYaXdPYLzb7kZtY7pLyCNUrTKLw==",
                SecurityStamp = "A2LPBW54USMGVGFJH4MR6ILQIUXJW7RR",
                ConcurrencyStamp = "186fe289-f01e-4185-8a96-4bd6e6429da5",
                LockoutEnabled = true
            },
            // Test2 Password2
            new User
            {
                Id = "13697d95-892d-42a6-8967-72fd15db40fe",
                UserName = "Test2",
                NormalizedUserName = "TEST2",
                PasswordHash = "AQAAAAIAAYagAAAAEA9S2sIiqoOQxcdfEi0Wj6cSFTMJVMOvt/xodYD/WTc3mMMyzJ/tEjYAJQ2teEalZg==",
                SecurityStamp = "K4A2HOYGT7EX7DSHBVVCYRPDAZGEDXAW",
                ConcurrencyStamp = "cbc58ec3-51ec-467c-a5c1-dc1a2e41a4f3",
                LockoutEnabled = true
            },
            // Test3 Password3
            new User
            {
                Id = "d04ffa4a-0ca1-48b6-ad54-7f8c5be9cbe1",
                UserName = "Test3",
                NormalizedUserName = "TEST3",
                PasswordHash = "AQAAAAIAAYagAAAAEHcKWZN9uxFYqcP6fHsAFPq+Apmq0FLKVLm4HOA4jii7iHM05xp0wfw/UIMgXyMCkQ==",
                SecurityStamp = "CFBNCGM6NZFBL7CEOHZWUYSOTLIAL7GN",
                ConcurrencyStamp = "b6ad90b7-1079-4303-8ccb-622d893141d7",
                LockoutEnabled = true
            });
    }
}