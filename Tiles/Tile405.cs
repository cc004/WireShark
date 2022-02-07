﻿using Terraria;

namespace WireShark.Tiles;

public class Tile405 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num27 = tile.frameX % 54 / 18;
            var num28 = tile.frameY % 36 / 18;
            var num29 = i - num27;
            var num30 = j - num28;
            var num31 = 54;
            if (Main.tile[num29, num30].frameX >= 54)
            {
                num31 = -54;
            }

            for (var num32 = num29; num32 < num29 + 3; num32++)
            {
                for (var num33 = num30; num33 < num30 + 2; num33++)
                {
                    Main.tile[num32, num33].frameX = (short) (Main.tile[num32, num33].frameX + num31);
                }
            }
        }
    }
}