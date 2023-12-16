using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using log4net.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Terraria.ModLoader;
using WireShark.Tiles;
using WireSharkRuntime;

namespace WireSharkRuntime
{
    public static class WiringWrapper
    {
        public static void HitSwitch(int i, int j)
        {
            if (!WorldGen.InWorld(i, j, 0))
            {
                return;
            }
            if (Main.tile[i, j] == null)
            {
                return;
            }
            if (Main.tile[i, j].TileType == 135 || Main.tile[i, j].TileType == 314 || Main.tile[i, j].TileType == 423 || Main.tile[i, j].TileType == 428 || Main.tile[i, j].TileType == 442)
            {
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                BigTripWire(i, j, 1, 1);
                return;
            }
            if (Main.tile[i, j].TileType == 440)
            {
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16 + 16, j * 16 + 16));
                BigTripWire(i, j, 3, 3);
                return;
            }
            if (Main.tile[i, j].TileType == 136)
            {
                if (Main.tile[i, j].TileFrameY == 0)
                {
                    Main.tile[i, j].TileFrameY = 18;
                }
                else
                {
                    Main.tile[i, j].TileFrameY = 0;
                }
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                BigTripWire(i, j, 1, 1);
                return;
            }
            if (Main.tile[i, j].TileType == 144)
            {
                if (Main.tile[i, j].TileFrameY == 0)
                {
                    Main.tile[i, j].TileFrameY = 18;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Wiring.CheckMech(i, j, 18000);
                    }
                }
                else
                {
                    Main.tile[i, j].TileFrameY = 0;
                }
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                return;
            }
            if (Main.tile[i, j].TileType == 441 || Main.tile[i, j].TileType == 468)
            {
                var num = Main.tile[i, j].TileFrameX / 18 * -1;
                var num2 = Main.tile[i, j].TileFrameY / 18 * -1;
                num %= 4;
                if (num < -1)
                {
                    num += 2;
                }
                num += i;
                num2 += j;
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                BigTripWire(num, num2, 2, 2);
                return;
            }
            if (Main.tile[i, j].TileType == 132 || Main.tile[i, j].TileType == 411)
            {
                short num3 = 36;
                var num4 = Main.tile[i, j].TileFrameX / 18 * -1;
                var num5 = Main.tile[i, j].TileFrameY / 18 * -1;
                num4 %= 4;
                if (num4 < -1)
                {
                    num4 += 2;
                    num3 = -36;
                }
                num4 += i;
                num5 += j;
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.tile[num4, num5].TileType == 411)
                {
                    Wiring.CheckMech(num4, num5, 60);
                }
                for (var k = num4; k < num4 + 2; k++)
                {
                    for (var l = num5; l < num5 + 2; l++)
                    {
                        if (Main.tile[k, l].TileType == 132 || Main.tile[k, l].TileType == 411)
                        {
                            var tile = Main.tile[k, l];
                            tile.TileFrameX += num3;
                        }
                    }
                }
                WorldGen.TileFrame(num4, num5, false, false);
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                BigTripWire(num4, num5, 2, 2);
            }
        }

        // entry point
        public static void BigTripWire(int l, int t, int w, int h)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            DLLManager.BigTripWire(l, t, w, h);
        }
    }
}