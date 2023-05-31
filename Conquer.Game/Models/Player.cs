namespace Conquer.Game.Models;

public enum PkMode
{
    Pk,
    Peace,
    Team,
    Capture
}

public enum Profession
{
    InternTrojan = 10,
    Trojan,
    VeteranTrojan,
    TigerTrojan,
    DragonTrojan,
    TrojanMaster,

    InternWarrior = 20,
    Warrior,
    BrassWarrior,
    SilverWarrior,
    GoldWarrior,
    WarriorMaster,

    InternArcher = 40,
    Archer,
    EagleArcher,
    TigerArcher,
    DragonArcher,
    ArcherMaster,

    InternTaoist = 100,
    Taoist,

    WaterTaoist = 132,
    WaterWizard,
    WaterMaster,
    WaterSaint,

    FireTaoist = 142,
    FireWizard,
    FireMaster,
    FireSaint
}

public enum Direction
{
    South,
    SouthWest,
    West,
    NorthWest,
    North,
    NorthEast,
    East,
    SouthEast
}

public class Player
{
    public uint Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public uint Model { get; set; }
    public ushort Avatar { get; set; }
    public ushort Hair { get; set; }
    public uint Money { get; set; }
    public byte Level { get; set; }
    public ulong Experience { get; set; }
    public ushort Strength { get; set; }
    public ushort Dexterity { get; set; }
    public ushort Vitality { get; set; }
    public ushort Mana { get; set; }
    public ushort AttributePoints { get; set; }
    public ushort Health { get; set; }
    public ushort Magic { get; set; }
    public PkMode PkMode { get; set; }
    public short PkPoints { get; set; }
    public Profession Profession { get; set; }
    public uint MapId { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public Direction Direction { get; set; }
    public byte Rebirths { get; set; }
    public uint? SpouseId { get; set; }
    public Player? Spouse { get; set; }
}