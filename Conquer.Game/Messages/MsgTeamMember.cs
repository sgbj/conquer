namespace Conquer.Game.Messages;

public enum TeamMemberAction : byte
{
    AddMember = 0,
    DelMember = 1
}

public record MsgTeamMember : IMessage
{
    public TeamMemberAction Action { get; set; }
    public List<(string Name, uint Id, uint LookFace, ushort MaxHealth, ushort Health)> Members { get; set; } = new();
    public ushort Size => (ushort)(8 + Members.Count * 28);
    public MessageType Type => MessageType.MsgTeamMember;

    public void Read(ReadOnlySpan<byte> buffer)
    {
        Action = (TeamMemberAction)buffer[4];
        var count = buffer[5];
        var index = 8;
        for (var i = 0; i < count; i++)
        {
            var name = ReadString(buffer.Slice(index, 16));
            index += 16;
            var id = ReadUInt32LittleEndian(buffer[index..]);
            index += sizeof(uint);
            var lookface = ReadUInt32LittleEndian(buffer[index..]);
            index += sizeof(uint);
            var maxHealth = ReadUInt16LittleEndian(buffer[index..]);
            index += sizeof(ushort);
            var health = ReadUInt16LittleEndian(buffer[index..]);
            index += sizeof(ushort);
            Members.Add((name, id, lookface, maxHealth, health));
        }
    }

    public void Write(Span<byte> buffer)
    {
        WriteUInt16LittleEndian(buffer, Size);
        WriteUInt16LittleEndian(buffer[2..], (ushort)Type);
        buffer[4] = (byte)Action;
        var index = 8;
        foreach (var (name, id, lookFace, maxHealth, health) in Members)
        {
            WriteString(buffer.Slice(index, 16), name);
            index += 16;
            WriteUInt32LittleEndian(buffer[index..], id);
            index += sizeof(uint);
            WriteUInt32LittleEndian(buffer[index..], lookFace);
            index += sizeof(uint);
            WriteUInt16LittleEndian(buffer[index..], maxHealth);
            index += sizeof(ushort);
            WriteUInt16LittleEndian(buffer[index..], health);
            index += sizeof(ushort);
        }
    }
}