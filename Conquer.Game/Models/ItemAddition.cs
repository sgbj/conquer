namespace Conquer.Game.Models;

public class ItemAddition
{
    public uint Id { get; set; }
    public uint ItemTypeId { get; set; }
    public byte Level { get; set; }
    public short Life { get; set; }
    public short AttackMax { get; set; }
    public short AttackMin { get; set; }
    public short Defense { get; set; }
    public short MagicAtk { get; set; }
    public short MagicDef { get; set; }
    public short Dexterity { get; set; }
    public short Dodge { get; set; }
}