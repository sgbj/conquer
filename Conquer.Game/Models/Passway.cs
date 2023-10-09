namespace Conquer.Game.Models;

public class Passway
{
    public uint Id { get; set; }
    public uint MapId { get; set; }
    public uint PasswayIdx { get; set; }
    public ushort PortalMapId { get; set; }
    public uint PortalIdx { get; set; }
}