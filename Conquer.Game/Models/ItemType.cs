namespace Conquer.Game.Models;

public class ItemType
{
    public uint Id { get; set; }
    public string Name { get; set; } = null!;
    public byte ReqProfession { get; set; }
    public byte ReqWeaponSkill { get; set; }
    public byte ReqLevel { get; set; }
    public byte ReqSex { get; set; }
    public ushort ReqForce { get; set; }
    public ushort ReqSpeed { get; set; }
    public ushort ReqHealth { get; set; }
    public ushort ReqSoul { get; set; }
    public byte Monopoly { get; set; }
    public ushort Weight { get; set; }
    public uint Price { get; set; }
    public int Task { get; set; }
    public ushort AttackMax { get; set; }
    public ushort AttackMin { get; set; }
    public short Defense { get; set; }
    public short Dexterity { get; set; }
    public short Dodge { get; set; }
    public short Life { get; set; }
    public short Mana { get; set; }
    public ushort Amount { get; set; }
    public ushort AmountLimit { get; set; }
    public byte Status { get; set; }
    public byte Gem1 { get; set; }
    public byte Gem2 { get; set; }
    public byte Magic1 { get; set; }
    public byte Magic2 { get; set; }
    public byte Magic3 { get; set; }
    public ushort MagicAtk { get; set; }
    public ushort MagicDef { get; set; }
    public ushort AtkRange { get; set; }
    public ushort AtkSpeed { get; set; }
}