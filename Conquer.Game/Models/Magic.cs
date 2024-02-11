namespace Conquer.Game.Models;

public class Magic
{
    public uint Id { get; set; }
    public uint PlayerId { get; set; }
    public ushort Type { get; set; }
    public byte Level { get; set; }
    public uint Experience { get; set; }
}