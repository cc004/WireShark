using Terraria;

namespace WireShark.Tiles;

public class Tile209 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num34 = tile.frameX % 72 / 18;
            var num35 = tile.frameY % 54 / 18;
            var num36 = i - num34;
            var num37 = j - num35;
            var num38 = tile.frameY / 54;
            var num39 = tile.frameX / 72;
            var num40 = -1;
            if (num34 == 1 || num34 == 2)
            {
                num40 = num35;
            }

            var num41 = 0;
            if (num34 == 3)
            {
                num41 = -54;
            }

            if (num34 == 0)
            {
                num41 = 54;
            }

            if (num38 >= 8 && num41 > 0)
            {
                num41 = 0;
            }

            if (num38 == 0 && num41 < 0)
            {
                num41 = 0;
            }

            var flag = false;
            if (num41 != 0)
            {
                for (var num42 = num36; num42 < num36 + 4; num42++)
                {
                    for (var num43 = num37; num43 < num37 + 3; num43++)
                    {
                        Main.tile[num42, num43].frameY = (short) (Main.tile[num42, num43].frameY + num41);
                    }
                }

                flag = true;
            }

            if ((num39 == 3 || num39 == 4) && (num40 == 0 || num40 == 1))
            {
                num41 = ((num39 == 3) ? 72 : -72);
                for (var num44 = num36; num44 < num36 + 4; num44++)
                {
                    for (var num45 = num37; num45 < num37 + 3; num45++)
                    {
                        Main.tile[num44, num45].frameX = (short) (Main.tile[num44, num45].frameX + num41);
                    }
                }

                flag = true;
            }

            if (flag)
            {
            }

            if (num40 != -1)
            {
                bool flag2 = !((num39 == 3 || num39 == 4) && num40 < 2);

                if (WiringWrapper.CheckMech(num36, num37, 30) && flag2)
                {
                    WorldGen.ShootFromCannon(num36, num37, num38, num39 + 1, 0, 0f, WiringWrapper.CurrentUser, true);
                }
            }
        }
    }
}