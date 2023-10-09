namespace Conquer.Game.Models;

// TODO
// Separate data models? Reorg? Guilds, Teams, etc.
//
// public class Screen : IEnumerable<IEntity>
// {
//     private readonly GameClient _client;
//     private readonly ConcurrentDictionary<uint, IEntity> _entities = new();
//
//     public Screen(GameClient client) => _client = client;
//
//     public async Task AddAsync(IEntity entity)
//     {
//         if (!_entities.TryAdd(entity.Id, entity))
//         {
//             return;
//         }
//
//         if (entity is Player player)
//         {
//             await player.Client.Screen.AddAsync(_client.Player);
//         }
//     }
//
//     public async Task RemoveAsync(IEntity entity)
//     {
//         if (!_entities.TryRemove(entity.Id, out _))
//         {
//             return;
//         }
//
//         if (entity is Player player)
//         {
//             await player.Client.Screen.RemoveAsync(_client.Player);
//         }
//     }
//
//     public async Task ClearAsync()
//     {
//         foreach (var entity in _entities.Values)
//         {
//             if (entity is Player player)
//             {
//                 await player.Client.Screen.RemoveAsync(_client.Player);
//             }
//         }
//
//         _entities.Clear();
//     }
//     
//     public IEnumerator<IEntity> GetEnumerator() => _entities.Values.GetEnumerator();
//
//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
// }

public class MapItem : IEntity
{
    public Item Item { get; set; } = null!;
    public uint Id { get; set; }
    public uint MapId { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
}

public class MessageBoard
{
}

public class Trade
{
    public Player Player1 { get; set; } = null!;
    public Player Player2 { get; set; } = null!;
    public uint Money1 { get; set; }
    public uint Money2 { get; set; }
    public List<Item> Items1 { get; set; } = null!;
    public List<Item> Items2 { get; set; } = null!;
}

public class Team
{
    public Player Leader { get; set; } = null!;
    public List<Player> Members { get; set; } = null!;
}

public class Friend
{
    public uint Id { get; set; }
    public uint PlayerId { get; set; }
    public uint FriendPlayerId { get; set; }
    public Player FriendPlayer { get; set; } = null!;
}

public class Enemy
{
    public uint Id { get; set; }
    public uint PlayerId { get; set; }
    public uint EnemyPlayerId { get; set; }
    public Player EnemyPlayer { get; set; } = null!;
}

[Flags]
public enum PlayerStatus : uint
{
    None = 0x00000000,
    Crime = 0x00000001, // Blue name
    Poison = 0x00000002,

    //Invisible = 0x04,
    XpFull = 0x00000010,
    Freeze = 0x00000020,
    TeamLeader = 0x00000040,
    Accuracy = 0x00000080, // Accuracy
    MagicDefense = 0x00000100, // MagicShield
    MagicAttack = 0x00000200, // Stigma
    Die = 0x00000400,
    FadeOut = 0x00000800,
    RedName = 0x00004000,
    BlackName = 0x00008000,
    SuperAtk = 0x00040000, // Superman
    MagicDodge = 0x00200000, // Dodge
    Hidden = 0x00400000, // Invisibility
    SuperSpeed = 0x00800000, // Cyclone
    Flying = 0x08000000, // Flying
    Intensify = 0x10000000, // Intensify
    CastingPray = 0x40000000, // Casting Pray
    Praying = 0x80000000 // Praying
}