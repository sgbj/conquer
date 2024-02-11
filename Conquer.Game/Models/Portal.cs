namespace Conquer.Game.Models;

public class Portal
{
    public uint Id { get; set; }
    public uint MapId { get; set; }
    public uint Idx { get; set; }
    public ushort PortalX { get; set; }
    public ushort PortalY { get; set; }
}