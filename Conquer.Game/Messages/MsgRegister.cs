using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Messages;

public record MsgRegister : IMessage
{
    private static readonly ushort[] Hairs = { 10, 11, 13, 14, 15, 24, 30, 35, 37, 38, 39, 40 };

    public string? UserName { get; set; }
    public string? PlayerName { get; set; }
    public string? Password { get; set; }
    public ushort Model { get; set; }
    public Profession Profession { get; set; }
    public uint Id { get; set; }
    public ushort Size => 60;
    public MessageType Type => MessageType.MsgRegister;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        UserName = ReadString(buffer[4..20]);
        PlayerName = ReadString(buffer[20..36]);
        Password = ReadString(buffer[36..52]);
        Model = ReadUInt16LittleEndian(buffer[52..]);
        Profession = (Profession)ReadUInt16LittleEndian(buffer[54..]);
        Id = ReadUInt32LittleEndian(buffer[56..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteString(buffer[4..20], UserName);
        WriteString(buffer[20..36], PlayerName);
        WriteString(buffer[36..52], Password);
        WriteUInt16LittleEndian(buffer[52..], Model);
        WriteUInt16LittleEndian(buffer[54..], (ushort)Profession);
        WriteUInt32LittleEndian(buffer[56..], Id);
    }

    public async Task HandleAsync(GameClient client, GameDbContext db)
    {
        // Invalid user
        if (client.UserId is null)
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Register, "SYSTEM", "ALLUSERS", "", "Invalid user."));
            return;
        }

        // Invalid name
        if (PlayerName is null || Regex.IsMatch(PlayerName, @"\[|]"))
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Register, "SYSTEM", "ALLUSERS", "", "Invalid name."));
            return;
        }

        // Name taken
        if (await db.Players.AnyAsync(p => p.Name == PlayerName))
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Register, "SYSTEM", "ALLUSERS", "", "Name taken."));
            return;
        }

        // Success
        client.Player = new()
        {
            UserId = client.UserId,
            Name = PlayerName,
            Model = Model,
            Avatar = (ushort)(Model < 1005 ? Random.Shared.Next(1, 49) : Random.Shared.Next(201, 249)),
            Hair = (ushort)(Random.Shared.Next(3, 9) * 100 + Hairs[Random.Shared.Next(0, Hairs.Length)]),
            PkMode = PkMode.Peace,
            Money = 1000,
            Level = 1,
            Strength = 4,
            Dexterity = 6,
            Vitality = 12,
            Mana = 10,
            Profession = Profession,
            MapId = 1002,
            X = 438,
            Y = 377
        };
        client.Player.Health = (ushort)(client.Player.Strength * 3 + client.Player.Dexterity * 3 +
                                        client.Player.Mana * 3 + client.Player.Vitality * 24);
        client.Player.Magic = (ushort)(client.Player.Mana * 5);

        db.Add(client.Player);
        await db.SaveChangesAsync();

        await client.WriteAsync(new MsgTalk(TalkChannel.Register, "SYSTEM", "ALLUSERS", "", "ANSWER_OK"));
    }
}