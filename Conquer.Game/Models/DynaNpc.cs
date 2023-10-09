namespace Conquer.Game.Models;

public class DynaNpc : IEntity
{
    public uint OwnerId { get; set; }
    public byte OwnerType { get; set; }
    public string Name { get; set; } = null!;
    public byte Type { get; set; }
    public uint LookFace { get; set; }
    public uint Life { get; set; }
    public byte Base { get; set; }
    public byte Sort { get; set; }
    public byte Level { get; set; }
    public ushort Defence { get; set; }
    public byte MagicDef { get; set; }
    public uint Id { get; set; }
    public uint MapId { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
}