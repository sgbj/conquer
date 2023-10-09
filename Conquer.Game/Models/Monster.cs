namespace Conquer.Game.Models;

public class Monster : IEntity
{
    public ushort Health { get; set; }
    public Direction Direction { get; set; }
    public MonsterType MonsterType { get; set; } = null!;
    public Generator? Generator { get; set; }
    public uint Id { get; set; }
    public uint MapId { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
}