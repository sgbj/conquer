using System.Drawing;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Conquer.Game.Services;

public class CommandService(IDbContextFactory<GameDbContext> dbContextFactory, ItemService itemService,
    MapService mapService, MonsterService monsterService, NpcService npcService)
{
    public async Task HandleAsync(GameClient client, string command)
    {
        var args = command.Split();

        await (args[0] switch
        {
            "/avatar" => Avatar(),
            "/cyclone" => Cyclone(),
            "/dialog" => Dialog(),
            "/hair" => Hair(),
            "/item" => Item(),
            "/job" => Job(),
            "/level" => Level(),
            "/magic" => Magic(),
            "/map" => Map(),
            "/mapargb" => MapArgb(),
            "/model" => Model(),
            "/money" => Money(),
            "/npc" => Npc(),
            "/online" => Online(),
            "/quit" => Quit(),
            "/skill" => Skill(),
            "/status" => Status(),
            "/weather" => Weather(),
            _ => Default()
        });

        async Task Avatar()
        {
            if (args.Length != 2 || !ushort.TryParse(args[1], out var avatar))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            client.Player.Avatar = avatar;

            await client.WriteAsync(new MsgAction
            {
                Id = client.Player.Id,
                DataUInt1 = avatar,
                Action = ActionType.ChangeFace
            });
        }

        async Task Cyclone()
        {
            client.Player.Status ^= PlayerStatus.SuperSpeed;
            await client.WriteScreenAsync(
                new MsgUserAttrib(client.Player.Id, AttributeType.Flags, (ulong)client.Player.Status));
        }

        async Task Dialog()
        {
            if (args.Length != 2 || !uint.TryParse(args[1], out var dialog))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            await client.WriteAsync(new MsgAction
            {
                Id = client.Player.Id,
                Action = ActionType.OpenDialog,
                DataUInt1 = dialog
            });
        }

        async Task Hair()
        {
            if (args.Length != 2 || !ushort.TryParse(args[1], out var hair))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            client.Player.Hair = hair;

            await client.WriteAsync(new MsgUserAttrib(client.Player.Id, AttributeType.Hair, hair));
        }

        async Task Item()
        {
            if (args.Length != 6 ||
                !int.TryParse(args[2], out var quality) ||
                !byte.TryParse(args[3], out var ident) ||
                !byte.TryParse(args[4], out var gem1) ||
                !byte.TryParse(args[5], out var gem2))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            var name = args[1];

            var itemType = itemService.ItemTypes.Values
                .OrderByDescending(i => i.Id)
                .FirstOrDefault(i => i.Name == name && i.Id % 10 == quality);

            if (itemType is null)
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid item."));
                return;
            }

            var item = new Item
            {
                PlayerId = client.Player.Id,
                ItemTypeId = itemType.Id,
                Amount = itemType.Amount,
                AmountLimit = itemType.AmountLimit,
                Ident = ident,
                Position = ItemPosition.Inventory,
                Gem1 = (Gem)gem1,
                Gem2 = (Gem)gem2,
                Magic1 = itemType.Magic1,
                Magic2 = itemType.Magic2,
                Magic3 = itemType.Magic3,
                Bless = 0,
                Enchant = 0,
                Restrain = 0
            };

            await using var db = await dbContextFactory.CreateDbContextAsync();
            db.Add(item);
            await db.SaveChangesAsync();

            client.Player.Items.Add(item);

            await client.WriteAsync(new MsgItemInfo(item, ItemInfoAction.AddItem));
        }

        async Task Job()
        {
            if (args.Length != 2 || !byte.TryParse(args[1], out var profession))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            client.Player.Profession = (Profession)profession;

            await client.WriteAsync(new MsgUserAttrib(client.Player.Id, AttributeType.Profession, profession));
        }

        async Task Level()
        {
            if (args.Length != 2 || !byte.TryParse(args[1], out var level))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            client.Player.Level = level;

            await client.WriteAsync(new MsgUserAttrib(client.Player.Id, AttributeType.Level, level));
            await client.WriteAsync(new MsgAction
            {
                Id = client.Player.Id,
                Action = ActionType.UpLev
            });
        }

        async Task Magic()
        {
            if (args.Length != 3 || !ushort.TryParse(args[1], out var type) || !byte.TryParse(args[2], out var level))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            var magic = client.Player.Magics.FirstOrDefault(weaponSkill => weaponSkill.Type == type);

            if (magic is null)
            {
                magic = new() { Type = type };
                client.Player.Magics.Add(magic);
            }

            magic.Level = level;
            magic.Experience = 0;

            await client.WriteAsync(new MsgMagicInfo(magic));
        }

        async Task Map()
        {
            if (args.Length != 4 ||
                !uint.TryParse(args[1], out var mapId) ||
                !ushort.TryParse(args[2], out var x) ||
                !ushort.TryParse(args[3], out var y))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            if (mapService.GameMaps.TryGetValue(mapId, out var gameMap))
            {
                await client.GameMap.RemoveAsync(client.Player);

                client.Player.MapId = mapId;
                client.Player.X = x;
                client.Player.Y = y;

                await client.WriteAsync(new MsgAction
                {
                    Id = client.Player.Id,
                    DataUInt1 = mapId,
                    DataUShort3 = x,
                    DataUShort4 = y,
                    Action = ActionType.EnterMap
                });

                await gameMap.AddAsync(client.Player);
            }
        }

        async Task MapArgb()
        {
            if (args.Length != 2 ||
                !uint.TryParse(args[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var argb))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            await client.WriteAsync(new MsgAction
            {
                Id = client.Player.Id,
                Action = ActionType.MapArgb,
                DataUInt1 = argb
            });
        }

        async Task Model()
        {
            if (args.Length != 2 || !uint.TryParse(args[1], out var model))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            client.Player.Model = model;

            await client.WriteAsync(new MsgUserAttrib(client.Player.Id, AttributeType.Look, client.Player.LookFace));
        }

        async Task Money()
        {
            if (args.Length != 2 || !uint.TryParse(args[1], out var money))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            client.Player.Money = money;

            await client.WriteAsync(new MsgUserAttrib(client.Player.Id, AttributeType.Money, money));
        }

        async Task Npc()
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Not implemented."));
        }

        async Task Online()
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "",
                $"{client.Server.Clients.Count} players online."));
        }

        Task Quit()
        {
            client.Abort();
            return Task.CompletedTask;
        }

        async Task Skill()
        {
            if (args.Length != 3 || !ushort.TryParse(args[1], out var type) || !byte.TryParse(args[2], out var level))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            var weaponSkill = client.Player.WeaponSkills.FirstOrDefault(weaponSkill => weaponSkill.Type == type);

            if (weaponSkill is null)
            {
                weaponSkill = new() { Type = type };
                client.Player.WeaponSkills.Add(weaponSkill);
            }

            weaponSkill.Level = level;
            weaponSkill.Experience = 0;

            await client.WriteAsync(new MsgWeaponSkill(weaponSkill));
        }

        async Task Status()
        {
            if (args.Length != 2 || !Enum.TryParse(args[1], out PlayerStatus status))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            client.Player.Status ^= status;
            await client.WriteScreenAsync(new MsgUserAttrib(client.Player.Id, AttributeType.Flags,
                (ulong)client.Player.Status));
        }

        async Task Weather()
        {
            if (args.Length != 2 || !uint.TryParse(args[1], out var weatherType))
            {
                await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid arguments."));
                return;
            }

            await client.WriteAsync(new MsgWeather
            {
                WeatherType = (WeatherType)weatherType,
                Intensity = 1,
                Direction = 0,
                Color = Color.White
            });
        }

        async Task Default()
        {
            await client.WriteAsync(new MsgTalk(TalkChannel.Talk, "SYSTEM", "", "", "Invalid command."));
        }
    }
}