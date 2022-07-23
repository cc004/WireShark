﻿using Terraria;

namespace WireShark.Tiles;

public class Tile406 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num2 = tile.TileFrameX % 54 / 18;
            var num3 = tile.TileFrameY % 54 / 18;
            var num4 = i - num2;
            var num5 = j - num3;
            var num6 = 54;
            if (Main.tile[num4, num5].TileFrameY >= 108)
            {
                num6 = -108;
            }

            for (var k = num4; k < num4 + 3; k++)
            {
                for (var l = num5; l < num5 + 3; l++)
                {
                    Main.tile[k, l].TileFrameY = (short) (Main.tile[k, l].TileFrameY + num6);
                }
            }
        }
    }
}