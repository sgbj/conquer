namespace Conquer.Game.Models;

public class DMap
{
    public uint Width { get; set; }
    public uint Height { get; set; }
    public DMapCell[,] Cells { get; set; } = null!;
    public Portal[] Portals { get; set; } = null!;
}

public class DMapCell
{
    public ushort Mask { get; set; }
    public ushort Terrain { get; set; }
    public short Altitude { get; set; }
}