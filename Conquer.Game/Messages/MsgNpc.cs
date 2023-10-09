namespace Conquer.Game.Messages;

public enum NpcAction : ushort
{
    Activate = 0,
    AddNpc = 1,
    LeaveMap = 2,
    DelNpc = 3,
    ChangePos = 4,
    LayNpc = 5
}

public record MsgNpc : IMessage
{
    public uint Id { get; set; }
    public uint Data { get; set; }
    public NpcAction Action { get; set; }
    public ushort NpcSort { get; set; }
    public ushort Size => 16;
    public MessageType Type => MessageType.MsgNpc;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Id = ReadUInt32LittleEndian(buffer[4..]);
        Data = ReadUInt32LittleEndian(buffer[8..]);
        Action = (NpcAction)ReadUInt16LittleEndian(buffer[12..]);
        NpcSort = ReadUInt16LittleEndian(buffer[14..]);
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        WriteUInt32LittleEndian(buffer[4..], Id);
        WriteUInt32LittleEndian(buffer[8..], Data);
        WriteUInt16LittleEndian(buffer[12..], (ushort)Action);
        WriteUInt16LittleEndian(buffer[14..], NpcSort);
    }

    public async Task HandleAsync(GameClient client)
    {
        await client.WriteAsync(new MsgTaskDialog
        {
            Id = Id,
            DialogType = DialogType.Text,
            Strings = new() { $"ID:~{Id}" }
        });
        await client.WriteAsync(new MsgTaskDialog
        {
            Id = Id,
            DialogType = DialogType.Pic,
            Avatar = 1
        });
        await client.WriteAsync(new MsgTaskDialog
        {
            Id = Id,
            DialogType = DialogType.Link,
            Option = 255,
            Strings = new() { "Goodbye" }
        });
        await client.WriteAsync(new MsgTaskDialog
        {
            Id = Id,
            DialogType = DialogType.Create
        });
    }
}