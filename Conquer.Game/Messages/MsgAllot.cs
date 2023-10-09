namespace Conquer.Game.Messages;

public record MsgAllot : IMessage
{
    public byte Strength { get; set; }
    public byte Dexterity { get; set; }
    public byte Vitality { get; set; }
    public byte Mana { get; set; }
    public ushort Size => 8;
    public MessageType Type => MessageType.MsgAllot;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Strength = buffer[4];
        Dexterity = buffer[5];
        Vitality = buffer[6];
        Mana = buffer[7];
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        buffer[4] = Strength;
        buffer[5] = Dexterity;
        buffer[6] = Vitality;
        buffer[7] = Mana;
    }

    public Task HandleAsync(GameClient client)
    {
        client.Player.Strength += Strength;
        client.Player.Dexterity += Dexterity;
        client.Player.Vitality += Vitality;
        client.Player.Mana += Mana;
        client.Player.AttributePoints -= (ushort)(Strength + Dexterity + Vitality + Mana);
        return Task.CompletedTask;
    }
}