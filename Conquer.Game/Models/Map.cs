namespace Conquer.Game.Models;

public class Map
{
    public uint Id { get; set; }
    public string Name { get; set; } = null!;
    public string DescribeText { get; set; } = null!;
    public ushort DocId { get; set; }
    public uint Type { get; set; }
    public uint Weather { get; set; }
    public uint BgMusic { get; set; }
    public uint BgMusicShow { get; set; }
    public ushort PortalX { get; set; }
    public ushort PortalY { get; set; }
    public uint RebornMap { get; set; }
    public uint RebornPortal { get; set; }
    public uint Light { get; set; }
}