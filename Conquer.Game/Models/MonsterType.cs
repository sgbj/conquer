namespace Conquer.Game.Models;

public class MonsterType
{
    public uint Id { get; set; }
    public string Name { get; set; } = null!;
    public byte Type { get; set; }
    public byte AiType { get; set; }
    public uint Look { get; set; }
    public byte Level { get; set; }
    public ushort Life { get; set; }
    public ushort EscapeLife { get; set; }
    public byte AttackUser { get; set; }
    public uint AttackMin { get; set; }
    public uint AttackMax { get; set; }
    public uint Defense { get; set; }
    public byte Dexterity { get; set; }
    public byte Dodge { get; set; }
    public uint MagicType { get; set; }
    public uint MagicDef { get; set; }
    public uint MagicHitRate { get; set; }
    public byte ViewRange { get; set; }
    public byte AttackRange { get; set; }
    public ushort AttackSpeed { get; set; }
    public ushort MoveSpeed { get; set; }
    public ushort RunSpeed { get; set; }
    public byte DropArmet { get; set; }
    public byte DropNecklace { get; set; }
    public byte DropArmor { get; set; }
    public byte DropRing { get; set; }
    public byte DropWeapon { get; set; }
    public byte DropShield { get; set; }
    public byte DropShoes { get; set; }
    public uint DropMoney { get; set; }
    public uint DropHp { get; set; }
    public uint DropMp { get; set; }
    public ushort ExtraExp { get; set; }
    public ushort ExtraDamage { get; set; }
}