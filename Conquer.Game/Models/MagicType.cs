namespace Conquer.Game.Models;

public class MagicType
{
    public uint Id { get; set; }
    public ushort Type { get; set; }
    public byte Sort { get; set; }
    public string Name { get; set; } = null!;
    public byte Crime { get; set; }
    public byte Ground { get; set; }
    public byte Multi { get; set; }
    public uint Target { get; set; }
    public ushort Level { get; set; }
    public ushort UseMp { get; set; }
    public int Power { get; set; }
    public ushort IntoneDuration { get; set; }
    public byte Success { get; set; }
    public uint StepSecs { get; set; }
    public byte Range { get; set; }
    public byte Distance { get; set; }
    public uint Status { get; set; }
    public ushort RequiredProf { get; set; }
    public uint RequiredExp { get; set; }
    public byte RequiredLevel { get; set; }
    public byte UseXp { get; set; }
    public ushort WeaponSubtype { get; set; }
    public uint ActiveTimes { get; set; }
    public byte AutoActive { get; set; }
    public uint FloorAttr { get; set; }
    public byte AutoLearn { get; set; }
    public ushort LearnLevel { get; set; }
    public byte DropWeapon { get; set; }
    public byte UseEp { get; set; }
    public byte WeaponHit { get; set; }
    public uint UseItem { get; set; }
    public ushort NextMagic { get; set; }
    public ushort NextMagicDelay { get; set; }
    public byte UseItemNum { get; set; }
    public ushort SenderAction { get; set; }
    public string UpgradeDesc { get; set; } = null!;
    public string Desc { get; set; } = null!;
    public string IntoneEffect { get; set; } = null!;
    public string IntoneSound { get; set; } = null!;
    public string SenderEffect { get; set; } = null!;
    public string SenderSound { get; set; } = null!;
    public uint TargetDelay { get; set; }
    public string TargetEffect { get; set; } = null!;
    public string TargetSound { get; set; } = null!;
    public string GroundEffect { get; set; } = null!;
    public string TraceEffect { get; set; } = null!;
    public uint ScreenRepresent { get; set; }
    public byte UsableInMarket { get; set; }
    public uint TargetWoundDelay { get; set; }
}