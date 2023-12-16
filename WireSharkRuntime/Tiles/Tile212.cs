using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace WireShark.Tiles;

public class Tile212 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num46 = tile.TileFrameX % 54 / 18;
            var num47 = tile.TileFrameY % 54 / 18;
            var num48 = i - num46;
            var num49 = j - num47;
            var num148 = (short) (tile.TileFrameX / 54);
            var num50 = -1;
            if (num46 == 1)
            {
                num50 = num47;
            }

            var num51 = 0;
            if (num46 == 0)
            {
                num51 = -54;
            }

            if (num46 == 2)
            {
                num51 = 54;
            }

            if (num148 >= 1 && num51 > 0)
            {
                num51 = 0;
            }

            if (num148 == 0 && num51 < 0)
            {
                num51 = 0;
            }

            var flag3 = false;
            if (num51 != 0)
            {
                for (var num52 = num48; num52 < num48 + 3; num52++)
                {
                    for (var num53 = num49; num53 < num49 + 3; num53++)
                    {
                        Main.tile[num52, num53].TileFrameX = (short) (Main.tile[num52, num53].TileFrameX + num51);
                    }
                }

                flag3 = true;
            }

            if (flag3)
            {
            }

            if (num50 != -1 && Wiring.CheckMech(num48, num49, 10))
            {
                var num149 = 12f + Main.rand.Next(450) * 0.01f;
                float num54 = Main.rand.Next(85, 105);
                float num150 = Main.rand.Next(-35, 11);
                var type2 = 166;
                var damage = 0;
                var knockBack = 0f;
                var vector = new Vector2((num48 + 2) * 16 - 8, (num49 + 2) * 16 - 8);
                if (tile.TileFrameX / 54 == 0)
                {
                    num54 *= -1f;
                    vector.X -= 12f;
                }
                else
                {
                    vector.X += 12f;
                }

                var num55 = num54;
                var num56 = num150;
                var num57 = (float) Math.Sqrt(num55 * num55 + num56 * num56);
                num57 = num149 / num57;
                num55 *= num57;
                num56 *= num57;
                Projectile.NewProjectile(null, vector.X, vector.Y, num55, num56, type2, damage, knockBack, Main.myPlayer, 0f, 0f);
            }
        }
    }
}