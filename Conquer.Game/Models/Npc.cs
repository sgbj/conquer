namespace Conquer.Game.Models;

public class Npc : IEntity
{
    public string Name { get; set; } = null!;
    public byte Type { get; set; }
    public uint LookFace { get; set; }
    public byte Base { get; set; }
    public byte Sort { get; set; }
    public uint Id { get; set; }
    public uint MapId { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
}