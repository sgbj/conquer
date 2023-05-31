using Conquer.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Conquer.Account.Messages;

public record MsgAccount : IMessage
{
    public string? UserName { get; set; }
    public byte[]? EncryptedPassword { get; set; }
    public string? ServerName { get; set; }
    public ushort Size => 52;
    public MessageType Type => MessageType.MsgAccount;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        UserName = ReadString(buffer[4..20]);
        EncryptedPassword = buffer[20..36].ToArray();
        ServerName = ReadString(buffer[36..52]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteString(buffer[4..20], UserName);
        EncryptedPassword.CopyTo(buffer[20..36]);
        WriteString(buffer[36..52], ServerName);
    }

    public async Task HandleAsync(AccountClient client, AccountDbContext db, IPasswordCipher passwordCipher,
        UserManager<User> userManager, HttpClient httpClient)
    {
        var password = DecryptPassword();
        var user = await userManager.FindByNameAsync(UserName!);

        // User doesn't exist
        if (user is null)
        {
            await client.WriteAsync(new MsgConnectEx(RejectionCode.InvalidAccount));
            return;
        }

        // Locked out
        if (await userManager.IsLockedOutAsync(user))
        {
            await client.WriteAsync(new MsgConnectEx(RejectionCode.TryAgainLater));
            return;
        }

        // Invalid password
        if (!await userManager.CheckPasswordAsync(user, password))
        {
            await userManager.AccessFailedAsync(user);
            await client.WriteAsync(new MsgConnectEx(RejectionCode.InvalidAccount));
            return;
        }

        // Banned
        if (user.IsBanned)
        {
            await client.WriteAsync(new MsgConnectEx(RejectionCode.Banned));
            return;
        }

        var server = await db.Servers.FirstOrDefaultAsync(s => s.Name == ServerName);

        // Invalid server
        if (server is null)
        {
            await client.WriteAsync(new MsgConnectEx(RejectionCode.ServerDown));
            return;
        }

        // Success
        var token = await httpClient.GetFromJsonAsync<ulong>($"{server.Url}/transfer/{user.Id}");
        await client.WriteAsync(new MsgConnectEx(token, server.IpAddress, server.Port));

        string DecryptPassword()
        {
            Span<byte> decryptedPassword = stackalloc byte[EncryptedPassword!.Length];
            passwordCipher.Decrypt(EncryptedPassword, decryptedPassword);
            return ReadString(decryptedPassword);
        }
    }
}