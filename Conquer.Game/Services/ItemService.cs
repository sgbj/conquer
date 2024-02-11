using System.Collections.Concurrent;
using System.Collections.Frozen;
using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Services;

public class ItemService
{
    private const uint FloorItemIdFirst = 2000001;
    private const uint FloorItemIdLast = 2999999;

    private readonly IDbContextFactory<GameDbContext> _dbContextFactory;

    private readonly object _lockObj = new();

    private uint _nextFloorItemId = FloorItemIdFirst;

    public ItemService(IDbContextFactory<GameDbContext> dbContextFactory) => _dbContextFactory = dbContextFactory;

    public ConcurrentDictionary<uint, ItemType> ItemTypes { get; private set; } = null!;
    public ConcurrentDictionary<uint, ItemAddition> ItemAdditions { get; private set; } = null!;
    public FrozenSet<ShopItem> ShopItems { get; private set; } = null!;

    public uint NextMapItemId()
    {
        lock (_lockObj)
        {
            var id = _nextFloorItemId++;

            if (_nextFloorItemId > FloorItemIdLast)
            {
                _nextFloorItemId = FloorItemIdFirst;
            }

            return id;
        }
    }

    // public async Task AddItemAsync(GameClient client, Item item)
    // {
    //     await using var db = await Server.CreateDbContextAsync();
    //     db.Add(item);
    //     await db.SaveChangesAsync();
    //     
    //     Player.Items.Add(item);
    //     
    //     await WriteAsync(new MsgItemInfo(item, ItemInfoAction.AddItem));
    // }
    //
    // public async Task RemoveItemAsync(Item item)
    // {
    //     await using var db = await Server.CreateDbContextAsync();
    //     db.Remove(item);
    //     await db.SaveChangesAsync();
    //     
    //     Player.Items.Remove(item);
    //     
    //     await WriteAsync(new MsgItemInfo(item, ItemInfoAction.));
    // }
    //
    // public async Task DropItemAsync(uint itemId, ushort x, ushort y)
    // {
    //     var item = Player.GetItemById(itemId);
    //
    //     if (item is null)
    //     {
    //         return;
    //     }
    //
    //     await RemoveItemAsync(item);
    //         
    //     var mapItem = new MapItem
    //     {
    //         Id = itemService.NextMapItemId(),
    //         MapId = client.Player.MapId,
    //         X = X,
    //         Y = Y,
    //         Item = item
    //     };
    //     client.Map.QuadTree.Insert(mapItem, new(mapItem.X, mapItem.Y, 1, 1));
    //     await playerService.UpdateScreenAsync(client);
    //     await client.WriteScreenAsync(new MsgMapItem(mapItem));
    // }

    public async Task InitializeAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        ItemTypes = new(await db.ItemTypes.ToDictionaryAsync(itemType => itemType.Id));
        ItemAdditions = new(await db.ItemAdditions.ToDictionaryAsync(itemAddition => itemAddition.Id));
        ShopItems = db.ShopItems.ToFrozenSet();
    }
}