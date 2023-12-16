using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using WireShark;

namespace WireSharkRuntime
{
    public static class WireAccelerator
    {
        static WireAccelerator()
        {
            var _vis = new int[Main.maxTilesX, Main.maxTilesY, 4, 3];
            for (var i = 0; i < Main.maxTilesX; i++)
            {
                for (var j = 0; j < Main.maxTilesY; j++)
                {
                    for (var k = 0; k < 4; k++)
                    {
                        _vis[i, j, k, 0] = -1;
                        _vis[i, j, k, 1] = -1;
                        _vis[i, j, k, 2] = -1;
                    }
                }
            }
        }

        public static TileInfo[,] _tileCache;

        public static void Preprocess()
        {
            _tileCache = new TileInfo[Main.maxTilesX, Main.maxTilesY];

            for (var i = 0; i < Main.maxTilesX; i++)
            {
                for (var j = 0; j < Main.maxTilesY; j++)
                {
                    if (IsAppliance(i, j))
                    {
                        _tileCache[i, j] = TileInfo.CreateTileInfo(i, j);
                    }
                }

                if (i % 100 == 0) Main.statusText = $"preprocess initializing {i * 1f / Main.maxTilesX:P1}";
            }
        }

        private static bool IsAppliance(int i, int j)
        {
            var tile = Main.tile[i, j];
            var type = (int) tile.TileType;
            if (ModContent.GetModTile(type) != null)
                return true;
            if (tile.HasActuator) return true;
            if (!tile.HasTile) return false;
            switch (type)
            {
                case 144:
                case 421 when !tile.HasActuator:
                case 422 when !tile.HasActuator:
                case >= 255 and <= 268 when !tile.HasActuator:
                case 419:
                case 406:
                case 452:
                case 411:
                case 425:
                case 405:
                case 209:
                case 212:
                case 215:
                case 130:
                case 131:
                case 387:
                case 386:
                case 389:
                case 388:
                case 11:
                case 10:
                case 216:
                case 497:
                case 15 when tile.TileFrameY / 40 == 1:
                case 15 when tile.TileFrameY / 40 == 20:
                case 335:
                case 338:
                case 235:
                case 4:
                case 429:
                case 149:
                case 244:
                case 565:
                case 42:
                case 93:
                case 126:
                case 95:
                case 100:
                case 173:
                case 564:
                case 593:
                case 594:
                case 34:
                case 314:
                case 33:
                case 174:
                case 49:
                case 372:
                case 92:
                case 137:
                case 443:
                case 531:
                case 139:
                case 35:
                case 207:
                case 410:
                case 480:
                case 509:
                case 455:
                case 141:
                case 210:
                case 142:
                case 143:
                case 105:
                case 349:
                case 506:
                case 546:
                case 557:
                    return true;
            }

            return false;
        }
    }
}
