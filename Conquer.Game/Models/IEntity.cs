namespace Conquer.Game.Models;

public interface IEntity
{
    public uint Id { get; }
    public uint MapId { get; }
    public ushort X { get; }
    public ushort Y { get; }
}