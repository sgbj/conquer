using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Conquer.Game.Services;

public class DMapService
{
    private readonly ILogger<DMapService> _logger;

    public DMapService(ILogger<DMapService> logger) => _logger = logger;

    public ConcurrentDictionary<uint, DMap> DMaps { get; } = new();

    public Task InitializeAsync()
    {
        _logger.LogInformation("Loading DMaps.");
        var stopwatchTimestamp = Stopwatch.GetTimestamp();

        using var gameMapReader = new BinaryReader(File.OpenRead("../../Conquer 2.0/ini/GameMap.dat"));

        var gameMapCount = gameMapReader.ReadUInt32();

        for (var i = 0; i < gameMapCount; i++)
        {
            var dMap = new DMap();
            var mapDocId = gameMapReader.ReadUInt32();
            var dMapPathLength = gameMapReader.ReadInt32();
            var dMapPath = Encoding.Latin1.GetString(gameMapReader.ReadBytes(dMapPathLength));
            gameMapReader.BaseStream.Seek(4, SeekOrigin.Current);

            using var dMapReader = new BinaryReader(File.OpenRead(Path.Combine("../../Conquer 2.0", dMapPath)));

            dMapReader.BaseStream.Seek(268, SeekOrigin.Current);
            dMap.Width = dMapReader.ReadUInt32();
            dMap.Height = dMapReader.ReadUInt32();

            dMap.Cells = new DMapCell[dMap.Width, dMap.Height];

            for (var y = 0; y < dMap.Height; y++)
            {
                for (var x = 0; x < dMap.Width; x++)
                {
                    dMap.Cells[x, y] = new()
                    {
                        Mask = dMapReader.ReadUInt16(),
                        Terrain = dMapReader.ReadUInt16(),
                        Altitude = dMapReader.ReadInt16()
                    };
                }

                dMapReader.BaseStream.Seek(4, SeekOrigin.Current);
            }

            var portalCount = dMapReader.ReadUInt32();
            dMap.Portals = new Portal[portalCount];

            for (var j = 0; j < dMap.Portals.Length; j++)
            {
                dMap.Portals[j] = new()
                {
                    PortalX = (ushort)dMapReader.ReadUInt32(),
                    PortalY = (ushort)dMapReader.ReadUInt32(),
                    Idx = dMapReader.ReadUInt32()
                };
            }

            var layerCount = dMapReader.ReadUInt32();

            for (var j = 0; j < layerCount; j++)
            {
                var type = dMapReader.ReadUInt32();

                switch (type)
                {
                    case 4: // MAP_COVER
                        dMapReader.BaseStream.Seek(416, SeekOrigin.Current);
                        break;

                    case 1: // MAP_TERRAIN
                        var scenePath = Encoding.Latin1.GetString(dMapReader.ReadBytes(260)).Split('\0')[0];
                        var startX = dMapReader.ReadUInt32();
                        var startY = dMapReader.ReadUInt32();

                        using (var sceneReader =
                               new BinaryReader(File.OpenRead(Path.Combine("../../Conquer 2.0", scenePath))))
                        {
                            var sceneCount = sceneReader.ReadUInt32();

                            for (var k = 0; k < sceneCount; k++)
                            {
                                sceneReader.BaseStream.Seek(332, SeekOrigin.Current);
                                var sceneWidth = sceneReader.ReadUInt32();
                                var sceneHeight = sceneReader.ReadUInt32();
                                sceneReader.BaseStream.Seek(4, SeekOrigin.Current);
                                var offsetX = sceneReader.ReadUInt32();
                                var offsetY = sceneReader.ReadUInt32();
                                sceneReader.BaseStream.Seek(4, SeekOrigin.Current);

                                for (var y = 0; y < sceneHeight; y++)
                                {
                                    for (var x = 0; x < sceneWidth; x++)
                                    {
                                        var dMapX = startX + offsetX + x - sceneWidth;
                                        var dMapY = startY + offsetY + y - sceneHeight;
                                        dMap.Cells[dMapX, dMapY] = new()
                                        {
                                            Mask = (ushort)sceneReader.ReadUInt32(),
                                            Terrain = (ushort)sceneReader.ReadUInt32(),
                                            Altitude = (short)sceneReader.ReadInt32()
                                        };
                                    }
                                }
                            }
                        }

                        break;

                    case 15: // MAP_SOUND
                        dMapReader.BaseStream.Seek(276, SeekOrigin.Current);
                        break;

                    case 10: // MAP_3DEFFECT
                        dMapReader.BaseStream.Seek(72, SeekOrigin.Current);
                        break;

                    case 19: // MAP_3DEFFECTNEW
                        dMapReader.BaseStream.Seek(96, SeekOrigin.Current);
                        break;
                }
            }

            DMaps[mapDocId] = dMap;
        }

        _logger.LogInformation("Loaded DMaps ({Elapsed}ms).",
            Stopwatch.GetElapsedTime(stopwatchTimestamp).TotalMilliseconds);

        return Task.CompletedTask;
    }
}