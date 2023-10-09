global using Conquer.Game.Data;
global using Conquer.Game.Messages;
global using Conquer.Game.Models;
global using Conquer.Game.Services;
global using static Conquer.Protocols.SizeTypePrefixedProtocol;
global using static System.Buffers.Binary.BinaryPrimitives;
using System.Security.Cryptography;
using Conquer;
using Conquer.Cryptography;
using Conquer.Game;
using Conquer.Game.Workers;
using Conquer.Middleware;
using Conquer.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddPooledDbContextFactory<GameDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContextPool<GameDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddMemoryCache();

builder.Services
    .AddTransient<IConnectionCipher, TqConnectionCipher>()
    .AddTransient<IProtocol, SizeTypePrefixedProtocol>()
    .AddSingleton<MessageService>()
    .AddSingleton<GameServer>()
    .AddSingleton<CommandService>()
    .AddSingleton<DMapService>()
    .AddSingleton<ItemService>()
    .AddSingleton<MapService>()
    .AddSingleton<MonsterService>()
    .AddSingleton<NpcService>()
    .AddSingleton<PlayerService>();

builder.Services
    .AddHostedService<GeneratorWorker>()
    .AddHostedService<MonsterWorker>();

builder.Services.AddOptions<GameServerOptions>().BindConfiguration("GameServer");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5816, listenOptions =>
        listenOptions.UseConquer()
            .UseConnectionLogging()
            .UseConnectionHandler<GameServer>());

    options.ListenAnyIP(5817);
});

var app = builder.Build();

app.MapGet("/transfer/{userId}", (string userId, IMemoryCache cache) =>
{
    Span<byte> buffer = stackalloc byte[sizeof(ulong)];
    RandomNumberGenerator.Fill(buffer);
    var token = ReadUInt64LittleEndian(buffer);
    cache.Set($"Transfer:{token}", userId, TimeSpan.FromMinutes(1));
    return token;
});

await app.Services.GetRequiredService<DMapService>().InitializeAsync();
await app.Services.GetRequiredService<ItemService>().InitializeAsync();
await app.Services.GetRequiredService<MapService>().InitializeAsync();
await app.Services.GetRequiredService<MonsterService>().InitializeAsync();
await app.Services.GetRequiredService<NpcService>().InitializeAsync();
await app.Services.GetRequiredService<PlayerService>().InitializeAsync();

app.Run();