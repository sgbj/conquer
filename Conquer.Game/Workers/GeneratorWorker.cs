namespace Conquer.Game.Workers;

public class GeneratorWorker(MapService mapService, MonsterService monsterService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            foreach (var generator in monsterService.Generators.Values)
            {
                if (DateTime.UtcNow - generator.LastGen < TimeSpan.FromSeconds(generator.RestSecs))
                {
                    continue;
                }

                var monsterType = monsterService.MonsterTypes[generator.MonsterTypeId];

                var count = Math.Min(generator.MaxPerGen, generator.MaxNpc - generator.Monsters.Count);

                for (var i = 0; i < count; i++)
                {
                    var x = (ushort)(generator.BoundX + Random.Shared.Next(generator.BoundCx));
                    var y = (ushort)(generator.BoundY + Random.Shared.Next(generator.BoundCy));

                    var direction = (Direction)Random.Shared.Next(0, 8);

                    var monster = new Monster
                    {
                        Id = monsterService.NextMonsterId(),
                        Health = monsterType.Life,
                        MapId = generator.MapId,
                        X = x,
                        Y = y,
                        Direction = direction,
                        MonsterType = monsterType,
                        Generator = generator
                    };

                    generator.Monsters.TryAdd(monster.Id, monster);
                    monsterService.Monsters.TryAdd(monster.Id, monster);

                    if (mapService.GameMaps.TryGetValue(monster.MapId, out var gameMap))
                    {
                        await gameMap.AddAsync(monster);
                    }
                }

                generator.LastGen = DateTime.UtcNow;
            }
        }
    }
}