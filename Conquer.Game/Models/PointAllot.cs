namespace Conquer.Game.Models;

public class PointAllot
{
    public uint Id { get; set; }
    public byte Profession { get; set; }
    public byte Level { get; set; }
    public ushort Force { get; set; }
    public ushort Speed { get; set; }
    public ushort Health { get; set; }
    public ushort Soul { get; set; }
}