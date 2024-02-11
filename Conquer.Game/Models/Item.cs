namespace Conquer.Game.Models;

public enum ItemPosition : byte
{
    Inventory = 0,
    Head = 1,
    Necklace = 2,
    Armor = 3,
    Right = 4,
    Left = 5,
    Ring = 6,
    Bottle = 7,
    Boots = 8,
    Garment = 9,
    Remove = 255
}

public enum Gem : byte
{
    None = 0,

    NormalPhoenixGem = 1,
    RefinedPhoenixGem = 2,
    SuperPhoenixGem = 3,

    NormalDragonGem = 11,
    RefinedDragonGem = 12,
    SuperDragonGem = 13,

    NormalFuryGem = 21,
    RefinedFuryGem = 22,
    SuperFuryGem = 23,

    NormalRainbowGem = 31,
    RefinedRainbowGem = 32,
    SuperRainbowGem = 33,

    NormalKylinGem = 41,
    RefinedKylinGem = 42,
    SuperKylinGem = 43,

    NormalVioletGem = 51,
    RefinedVioletGem = 52,
    SuperVioletGem = 53,

    NormalMoonGem = 61,
    RefinedMoonGem = 62,
    SuperMoonGem = 63,

    NormalTortoiseGem = 71,
    RefinedTortoiseGem = 72,
    SuperTortoiseGem = 73,

    Empty = 255
}

public class Item
{
    public uint Id { get; set; }
    public uint ItemTypeId { get; set; }
    public uint PlayerId { get; set; }
    public ushort Amount { get; set; }
    public ushort AmountLimit { get; set; }
    public byte Ident { get; set; }
    public ItemPosition Position { get; set; }
    public Gem Gem1 { get; set; }
    public Gem Gem2 { get; set; }
    public byte Magic1 { get; set; }
    public byte Magic2 { get; set; }
    public byte Magic3 { get; set; }
    public uint Restrain { get; set; }
    public byte Bless { get; set; }
    public byte Enchant { get; set; }
}