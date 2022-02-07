using Microsoft.Xna.Framework;
using Terraria;

namespace WireShark.Tiles;

public class Tile137 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num99 = tile.frameY / 18;
            var zero = Vector2.Zero;
            var speedX = 0f;
            var speedY = 0f;
            var num100 = 0;
            var damage2 = 0;
            switch (num99)
            {
                case 0:
                case 1:
                case 2:
                    if (WiringWrapper.CheckMech(i, j, 200))
                    {
                        var num101 = (tile.frameX == 0) ? -1 : ((tile.frameX == 18) ? 1 : 0);
                        var num102 = (tile.frameX < 36) ? 0 : ((tile.frameX < 72) ? -1 : 1);
                        zero = new Vector2(i * 16 + 8 + 10 * num101, j * 16 + 9 + num102 * 9);
                        var num103 = 3f;
                        if (num99 == 0)
                        {
                            num100 = 98;
                            damage2 = 20;
                            num103 = 12f;
                        }

                        if (num99 == 1)
                        {
                            num100 = 184;
                            damage2 = 40;
                            num103 = 12f;
                        }

                        if (num99 == 2)
                        {
                            num100 = 187;
                            damage2 = 40;
                            num103 = 5f;
                        }

                        speedX = num101 * num103;
                        speedY = num102 * num103;
                    }

                    break;
                case 3:
                    if (WiringWrapper.CheckMech(i, j, 300))
                    {
                        var num104 = 200;
                        for (var num105 = 0; num105 < 1000; num105++)
                        {
                            if (Main.projectile[num105].active && Main.projectile[num105].type == num100)
                            {
                                var num106 = (new Vector2(i * 16 + 8, j * 18 + 8) -
                                              Main.projectile[num105].Center).Length();
                                if (num106 < 50f)
                                {
                                    num104 -= 50;
                                }
                                else if (num106 < 100f)
                                {
                                    num104 -= 15;
                                }
                                else if (num106 < 200f)
                                {
                                    num104 -= 10;
                                }
                                else if (num106 < 300f)
                                {
                                    num104 -= 8;
                                }
                                else if (num106 < 400f)
                                {
                                    num104 -= 6;
                                }
                                else if (num106 < 500f)
                                {
                                    num104 -= 5;
                                }
                                else if (num106 < 700f)
                                {
                                    num104 -= 4;
                                }
                                else if (num106 < 900f)
                                {
                                    num104 -= 3;
                                }
                                else if (num106 < 1200f)
                                {
                                    num104 -= 2;
                                }
                                else
                                {
                                    num104--;
                                }
                            }
                        }

                        if (num104 > 0)
                        {
                            num100 = 185;
                            damage2 = 40;
                            var num107 = 0;
                            var num108 = 0;
                            switch (tile.frameX / 18)
                            {
                                case 0:
                                case 1:
                                    num107 = 0;
                                    num108 = 1;
                                    break;
                                case 2:
                                    num107 = 0;
                                    num108 = -1;
                                    break;
                                case 3:
                                    num107 = -1;
                                    num108 = 0;
                                    break;
                                case 4:
                                    num107 = 1;
                                    num108 = 0;
                                    break;
                            }

                            speedX = 4 * num107 + Main.rand.Next(-20 + ((num107 == 1) ? 20 : 0),
                                21 - ((num107 == -1) ? 20 : 0)) * 0.05f;
                            speedY = 4 * num108 + Main.rand.Next(-20 + ((num108 == 1) ? 20 : 0),
                                21 - ((num108 == -1) ? 20 : 0)) * 0.05f;
                            zero = new Vector2(i * 16 + 8 + 14 * num107, j * 16 + 8 + 14 * num108);
                        }
                    }

                    break;
                case 4:
                    if (WiringWrapper.CheckMech(i, j, 90))
                    {
                        var num109 = 0;
                        var num110 = 0;
                        switch (tile.frameX / 18)
                        {
                            case 0:
                            case 1:
                                num109 = 0;
                                num110 = 1;
                                break;
                            case 2:
                                num109 = 0;
                                num110 = -1;
                                break;
                            case 3:
                                num109 = -1;
                                num110 = 0;
                                break;
                            case 4:
                                num109 = 1;
                                num110 = 0;
                                break;
                        }

                        speedX = 8 * num109;
                        speedY = 8 * num110;
                        damage2 = 60;
                        num100 = 186;
                        zero = new Vector2(i * 16 + 8 + 18 * num109, j * 16 + 8 + 18 * num110);
                    }

                    break;
            }

            switch (num99 + 10)
            {
                case 0:
                    if (WiringWrapper.CheckMech(i, j, 200))
                    {
                        var num111 = -1;
                        if (tile.frameX != 0)
                        {
                            num111 = 1;
                        }

                        speedX = 12 * num111;
                        damage2 = 20;
                        num100 = 98;
                        zero = new Vector2(i * 16 + 8, j * 16 + 7);
                        zero.X += 10 * num111;
                        zero.Y += 2f;
                    }

                    break;
                case 1:
                    if (WiringWrapper.CheckMech(i, j, 200))
                    {
                        var num112 = -1;
                        if (tile.frameX != 0)
                        {
                            num112 = 1;
                        }

                        speedX = 12 * num112;
                        damage2 = 40;
                        num100 = 184;
                        zero = new Vector2(i * 16 + 8, j * 16 + 7);
                        zero.X += 10 * num112;
                        zero.Y += 2f;
                    }

                    break;
                case 2:
                    if (WiringWrapper.CheckMech(i, j, 200))
                    {
                        var num113 = -1;
                        if (tile.frameX != 0)
                        {
                            num113 = 1;
                        }

                        speedX = 5 * num113;
                        damage2 = 40;
                        num100 = 187;
                        zero = new Vector2(i * 16 + 8, j * 16 + 7);
                        zero.X += 10 * num113;
                        zero.Y += 2f;
                    }

                    break;
                case 3:
                    if (WiringWrapper.CheckMech(i, j, 300))
                    {
                        num100 = 185;
                        var num114 = 200;
                        for (var num115 = 0; num115 < 1000; num115++)
                        {
                            if (Main.projectile[num115].active && Main.projectile[num115].type == num100)
                            {
                                var num116 = (new Vector2(i * 16 + 8, j * 18 + 8) -
                                              Main.projectile[num115].Center).Length();
                                if (num116 < 50f)
                                {
                                    num114 -= 50;
                                }
                                else if (num116 < 100f)
                                {
                                    num114 -= 15;
                                }
                                else if (num116 < 200f)
                                {
                                    num114 -= 10;
                                }
                                else if (num116 < 300f)
                                {
                                    num114 -= 8;
                                }
                                else if (num116 < 400f)
                                {
                                    num114 -= 6;
                                }
                                else if (num116 < 500f)
                                {
                                    num114 -= 5;
                                }
                                else if (num116 < 700f)
                                {
                                    num114 -= 4;
                                }
                                else if (num116 < 900f)
                                {
                                    num114 -= 3;
                                }
                                else if (num116 < 1200f)
                                {
                                    num114 -= 2;
                                }
                                else
                                {
                                    num114--;
                                }
                            }
                        }

                        if (num114 > 0)
                        {
                            speedX = Main.rand.Next(-20, 21) * 0.05f;
                            speedY = 4f + Main.rand.Next(0, 21) * 0.05f;
                            damage2 = 40;
                            zero = new Vector2(i * 16 + 8, j * 16 + 16);
                            zero.Y += 6f;
                            Projectile.NewProjectile(null, (int) zero.X, (int) zero.Y, speedX, speedY, num100,
                                damage2, 2f, Main.myPlayer, 0f, 0f);
                        }
                    }

                    break;
                case 4:
                    if (WiringWrapper.CheckMech(i, j, 90))
                    {
                        speedX = 0f;
                        speedY = 8f;
                        damage2 = 60;
                        num100 = 186;
                        zero = new Vector2(i * 16 + 8, j * 16 + 16);
                        zero.Y += 10f;
                    }

                    break;
            }

            if (num100 != 0)
            {
                Projectile.NewProjectile(null, (int) zero.X, (int) zero.Y, speedX, speedY, num100, damage2, 2f,
                    Main.myPlayer, 0f, 0f);
            }
        }
    }
}