using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conquer.Game.Models;

public class Generator
{
    public uint Id { get; set; }
    public uint MapId { get; set; }
    public ushort BoundX { get; set; }
    public ushort BoundY { get; set; }
    public ushort BoundCx { get; set; }
    public ushort BoundCy { get; set; }
    public ushort MaxNpc { get; set; }
    public ushort RestSecs { get; set; }
    public ushort MaxPerGen { get; set; }
    public uint MonsterTypeId { get; set; }

    [NotMapped] public DateTime LastGen { get; set; }

    [NotMapped] public ConcurrentDictionary<uint, Monster> Monsters { get; } = new();
}