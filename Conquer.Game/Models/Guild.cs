namespace Conquer.Game.Models;

public class Guild
{
    public uint Id { get; set; }
    public string Name { get; set; } = null!;
    public uint Fund { get; set; }
    public List<Player> Members { get; set; } = null!;
    public List<GuildAlly> Allies { get; set; } = null!;
    public List<GuildEnemy> Enemies { get; set; } = null!;
}