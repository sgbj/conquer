global using Conquer.Account.Data;
global using Conquer.Account.Models;
global using static Conquer.Protocols.SizeTypePrefixedProtocol;
global using static System.Buffers.Binary.BinaryPrimitives;
using Conquer;
using Conquer.Account;
using Conquer.Cryptography;
using Conquer.Middleware;
using Conquer.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<AccountDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddPooledDbContextFactory<AccountDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentityCore<User>(options => options.Password.RequireNonAlphanumeric = false)
    .AddEntityFrameworkStores<AccountDbContext>();

builder.Services.AddHttpClient();

builder.Services
    .AddTransient<IPasswordCipher, Rc5PasswordCipher>()
    .AddTransient<IConnectionCipher, TqConnectionCipher>()
    .AddTransient<IProtocol, SizeTypePrefixedProtocol>()
    .AddSingleton<MessageService>()
    .AddSingleton<AccountServer>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(9958, listenOptions =>
        listenOptions.UseConquer()
            .UseConnectionLogging()
            .UseConnectionHandler<AccountServer>());
});

var app = builder.Build();

app.Run();