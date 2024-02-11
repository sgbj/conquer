namespace Conquer.Game.Messages;

public enum InteractAction
{
    Steal = 1, //Works?
    Attack = 2, //Attack Miss -> Param = 0
    Heal = 3, //Works?
    Poison = 4, //Works?
    Assassinate = 5, //Works?
    Freeze = 6, //Works?
    Unfreeze = 7, //Works?
    Court = 8,
    Marry = 9,
    Divorce = 10, //Works?
    PresentMoney = 11, //Works?
    PresentItem = 12, //Works?
    SendFlowers = 13,
    Kill = 14,
    JoinSyn = 15, //Works?
    AcceptSynMember = 16, //Works?
    KickOutSynMember = 17, //Works?
    PresentPower = 18, //Works?
    QueryInfo = 19, //Works?
    RushAtk = 20, //Works?
    MagicAttack = 21,
    AbortMagic = 22,
    ReflectWeapon = 23,
    Bump = 24, //Dash
    Shoot = 25,
    ReflectMagic = 26
}

public record MsgInteract : IMessage
{
    public uint Timestamp { get; set; } = (uint)Environment.TickCount;
    public uint Sender { get; set; }
    public uint Target { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public InteractAction Action { get; set; }
    public uint Data { get; set; }
    public ushort Size => 28;
    public MessageType Type => MessageType.MsgInteract;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Timestamp = ReadUInt32LittleEndian(buffer[4..]);
        Sender = ReadUInt32LittleEndian(buffer[8..]);
        Target = ReadUInt32LittleEndian(buffer[12..]);
        X = ReadUInt16LittleEndian(buffer[16..]);
        Y = ReadUInt16LittleEndian(buffer[18..]);
        Action = (InteractAction)ReadUInt32LittleEndian(buffer[20..]);
        Data = ReadUInt32LittleEndian(buffer[24..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Timestamp);
        WriteUInt32LittleEndian(buffer[8..], Sender);
        WriteUInt32LittleEndian(buffer[12..], Target);
        WriteUInt16LittleEndian(buffer[16..], X);
        WriteUInt16LittleEndian(buffer[18..], Y);
        WriteUInt32LittleEndian(buffer[20..], (uint)Action);
        WriteUInt32LittleEndian(buffer[24..], Data);
    }

    public Task HandleAsync(GameClient client, ILogger<MsgInteract> logger)
    {
        return Action switch
        {
            InteractAction.Attack => Attack(),
            InteractAction.MagicAttack => MagicAttack(),
            _ => Default()
        };

        async Task Attack()
        {
            Data = 1;
            await client.WriteScreenAsync(this);
        }

        async Task MagicAttack()
        {
            Target = ((((Target & 0xFFFFE000) >> 13) | ((Target & 0x1FFF) << 19)) ^ 0x5F2D2463 ^ Sender) - 0x746F4AE6;
            X = (ushort)(X ^ (Sender & 0xFFFF) ^ 0x2ED6);
            X = (ushort)(((X << 1) | ((X & 0x8000) >> 15)) - 0x22EE);
            Y = (ushort)(Y ^ (Sender & 0xFFFF) ^ 0xB99B);
            Y = (ushort)(((Y << 5) | ((Y & 0xF800) >> 11)) - 0x8922);
            Data = (ushort)(Data ^ 0x915D ^ Sender);
            Data = (ushort)(((Data << 3) | (Data >> 13)) - 0xEB42);

            logger.LogInformation("Decrypted message {Message}", this);

            var magic = client.Player.Magics.FirstOrDefault(magic => magic.Type == Data);

            if (magic is null)
            {
                return;
            }

            await client.WriteScreenAsync(new MsgMagicEffect
            {
                Sender = Sender,
                Target = Target,
                MagicType = magic.Type,
                Level = magic.Level,
                Targets = { (Target, 1) }
            });
        }

        async Task Default()
        {
            logger.LogWarning("Unhandled action {Action}.", Action);
            await client.WriteAsync(this);
        }
    }
}