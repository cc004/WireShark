using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WireShark.Tiles;

public class Tile105 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num128 = j - tile.TileFrameY / 18;
            var num129 = tile.TileFrameX / 18;
            var num130 = 0;
            while (num129 >= 2)
            {
                num129 -= 2;
                num130++;
            }

            num129 = i - num129;
            num129 = i - tile.TileFrameX % 36 / 18;
            num128 = j - tile.TileFrameY % 54 / 18;
            num130 = tile.TileFrameX / 36 + tile.TileFrameY / 54 * 55;


            var num131 = num129 * 16 + 16;
            var num132 = (num128 + 3) * 16;
            var num133 = -1;
            var num134 = -1;
            var flag11 = true;
            var flag12 = false;
            switch (num130)
            {
                case 51:
                    num134 = Utils.SelectRandom<short>(Main.rand, new short[]
                    {
                        299,
                        538
                    });
                    break;
                case 52:
                    num134 = 356;
                    break;
                case 53:
                    num134 = 357;
                    break;
                case 54:
                    num134 = Utils.SelectRandom<short>(Main.rand, new short[]
                    {
                        355,
                        358
                    });
                    break;
                case 55:
                    num134 = Utils.SelectRandom<short>(Main.rand, new short[]
                    {
                        367,
                        366
                    });
                    break;
                case 56:
                    num134 = Utils.SelectRandom<short>(Main.rand, new short[]
                    {
                        359,
                        359,
                        359,
                        359,
                        360
                    });
                    break;
                case 57:
                    num134 = 377;
                    break;
                case 58:
                    num134 = 300;
                    break;
                case 59:
                    num134 = Utils.SelectRandom<short>(Main.rand, new short[]
                    {
                        364,
                        362
                    });
                    break;
                case 60:
                    num134 = 148;
                    break;
                case 61:
                    num134 = 361;
                    break;
                case 62:
                    num134 = Utils.SelectRandom<short>(Main.rand, new short[]
                    {
                        487,
                        486,
                        485
                    });
                    break;
                case 63:
                    num134 = 164;
                    flag11 &= NPC.MechSpawn(num131, num132, 165);
                    break;
                case 64:
                    num134 = 86;
                    flag12 = true;
                    break;
                case 65:
                    num134 = 490;
                    break;
                case 66:
                    num134 = 82;
                    break;
                case 67:
                    num134 = 449;
                    break;
                case 68:
                    num134 = 167;
                    break;
                case 69:
                    num134 = 480;
                    break;
                case 70:
                    num134 = 48;
                    break;
                case 71:
                    num134 = Utils.SelectRandom<short>(Main.rand, new short[]
                    {
                        170,
                        180,
                        171
                    });
                    flag12 = true;
                    break;
                case 72:
                    num134 = 481;
                    break;
                case 73:
                    num134 = 482;
                    break;
                case 74:
                    num134 = 430;
                    break;
                case 75:
                    num134 = 489;
                    break;
            }

            if (num134 != -1 && Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, num134) &&
                flag11)
            {
                if (!flag12 || !Collision.SolidTiles(num129 - 2, num129 + 3, num128, num128 + 2))
                {
                    num133 = NPC.NewNPC(null, num131, num132 - 12, num134, 0, 0f, 0f, 0f, 0f, 255);
                }
                else
                {
                    var position = new Vector2(num131 - 4, num132 - 22) - new Vector2(10f);
                    Utils.PoofOfSmoke(position);
                }
            }

            if (num133 <= -1)
            {
                switch (num130)
                {
                    case 4:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 1))
                        {
                            num133 = NPC.NewNPC(null, num131, num132 - 12, 1, 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 7:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 49))
                        {
                            num133 = NPC.NewNPC(null, num131 - 4, num132 - 6, 49, 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 8:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 55))
                        {
                            num133 = NPC.NewNPC(null, num131, num132 - 12, 55, 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 9:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 46))
                        {
                            num133 = NPC.NewNPC(null, num131, num132 - 12, 46, 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 10:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 21))
                        {
                            num133 = NPC.NewNPC(null, num131, num132, 21, 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 18:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 67))
                        {
                            num133 = NPC.NewNPC(null, num131, num132 - 12, 67, 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 23:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 63))
                        {
                            num133 = NPC.NewNPC(null, num131, num132 - 12, 63, 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 27:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 85))
                        {
                            num133 = NPC.NewNPC(null, num131 - 9, num132, 85, 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 28:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 74))
                        {
                            num133 = NPC.NewNPC(null, num131, num132 - 12, Utils.SelectRandom<short>(Main.rand,
                                new short[]
                                {
                                    74,
                                    297,
                                    298
                                }), 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 34:
                    {
                        for (var num135 = 0; num135 < 2; num135++)
                        {
                            for (var num136 = 0; num136 < 3; num136++)
                            {
                                var tile22 = Main.tile[num129 + num135, num128 + num136];
                                tile22.TileType = 349;
                                tile22.TileFrameX = (short) (num135 * 18 + 216);
                                tile22.TileFrameY = (short) (num136 * 18);
                            }
                        }

                        Animation.NewTemporaryAnimation(0, 349, num129, num128);
                        if (Main.netMode == NetmodeID.Server)
                        {
                        }

                        break;
                    }
                    case 42:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 58))
                        {
                            num133 = NPC.NewNPC(null, num131, num132 - 12, 58, 0, 0f, 0f, 0f, 0f, 255);
                        }

                        break;
                    }
                    case 37:
                    {
                        if (Wiring.CheckMech(num129, num128, 600) && Item.MechSpawn(num131, num132, 58) &&
                            Item.MechSpawn(num131, num132, 1734) && Item.MechSpawn(num131, num132, 1867))
                        {
                            Item.NewItem(null, num131, num132 - 16, 0, 0, 58, 1, false, 0, false, false);
                        }

                        break;
                    }
                    case 50:
                    {
                        if (Wiring.CheckMech(num129, num128, 30) && NPC.MechSpawn(num131, num132, 65))
                        {
                            if (!Collision.SolidTiles(num129 - 2, num129 + 3, num128, num128 + 2))
                            {
                                num133 = NPC.NewNPC(null, num131, num132 - 12, 65, 0, 0f, 0f, 0f, 0f, 255);
                            }
                            else
                            {
                                var position2 = new Vector2(num131 - 4, num132 - 22) - new Vector2(10f);
                                Utils.PoofOfSmoke(position2);
                            }
                        }

                        break;
                    }
                    case 2:
                    {
                        if (Wiring.CheckMech(num129, num128, 600) && Item.MechSpawn(num131, num132, 184) &&
                            Item.MechSpawn(num131, num132, 1735) && Item.MechSpawn(num131, num132, 1868))
                        {
                            Item.NewItem(null, num131, num132 - 16, 0, 0, 184, 1, false, 0, false, false);
                        }

                        break;
                    }
                    case 17:
                    {
                        if (Wiring.CheckMech(num129, num128, 600) && Item.MechSpawn(num131, num132, 166))
                        {
                            Item.NewItem(null, num131, num132 - 20, 0, 0, 166, 1, false, 0, false, false);
                        }

                        break;
                    }
                    case 40:
                    {
                        if (Wiring.CheckMech(num129, num128, 300))
                        {
                            var array = new List<int>();
                            var num137 = 0;
                            for (var num138 = 0; num138 < 200; num138++)
                            {
                                var vanillaCanGo = Main.npc[num138].type == NPCID.Merchant ||
                                                   Main.npc[num138].type == NPCID.ArmsDealer ||
                                                   Main.npc[num138].type == NPCID.Guide ||
                                                   Main.npc[num138].type == NPCID.Demolitionist ||
                                                   Main.npc[num138].type == NPCID.Clothier ||
                                                   Main.npc[num138].type == NPCID.GoblinTinkerer ||
                                                   Main.npc[num138].type == NPCID.Wizard ||
                                                   Main.npc[num138].type == NPCID.SantaClaus ||
                                                   Main.npc[num138].type == NPCID.Truffle ||
                                                   Main.npc[num138].type == NPCID.DyeTrader ||
                                                   Main.npc[num138].type == NPCID.Cyborg ||
                                                   Main.npc[num138].type == NPCID.Painter ||
                                                   Main.npc[num138].type == NPCID.WitchDoctor ||
                                                   Main.npc[num138].type == NPCID.Pirate ||
                                                   Main.npc[num138].type == NPCID.LightningBug ||
                                                   Main.npc[num138].type == NPCID.Angler ||
                                                   Main.npc[num138].type == NPCID.DD2Bartender;
                                if (Main.npc[num138].active &&
                                    NPCLoader.CanGoToStatue(Main.npc[num138], true).HasValue)
                                {
                                    array.Add(num138);
                                    num137++;
                                }
                            }

                            if (num137 > 0)
                            {
                                var num139 = array[Main.rand.Next(num137)];
                                Main.npc[num139].position.X = num131 - Main.npc[num139].width / 2;
                                Main.npc[num139].position.Y = num132 - Main.npc[num139].height - 1;

                                NPCLoader.OnGoToStatue(Main.npc[num139], true);
                            }
                        }

                        break;
                    }
                    case 41:
                    {
                        if (Wiring.CheckMech(num129, num128, 300))
                        {
                            var array2 = new List<int>();
                            var num140 = 0;
                            for (var num141 = 0; num141 < 200; num141++)
                            {
                                var vanillaCanGo2 = Main.npc[num141].type == NPCID.Nurse ||
                                                    Main.npc[num141].type == NPCID.Dryad ||
                                                    Main.npc[num141].type == NPCID.Mechanic ||
                                                    Main.npc[num141].type == NPCID.Steampunker ||
                                                    Main.npc[num141].type == NPCID.PartyGirl ||
                                                    Main.npc[num141].type == NPCID.Stylist;
                                    if (Main.npc[num141].active && NPCLoader.CanGoToStatue(Main.npc[num141], false).HasValue)
                                    {
                                        array2.Add(num141);
                                        num140++;
                                    }
                            }

                            if (num140 > 0)
                            {
                                var num142 = array2[Main.rand.Next(num140)];
                                Main.npc[num142].position.X = num131 - Main.npc[num142].width / 2;
                                Main.npc[num142].position.Y = num132 - Main.npc[num142].height - 1;

                                NPCLoader.OnGoToStatue(Main.npc[num142], false);
                            }

                        }

                        break;
                    }
                }
            }

            if (num133 >= 0)
            {
                Main.npc[num133].value = 0f;
                Main.npc[num133].npcSlots = 0f;
                Main.npc[num133].SpawnedFromStatue = true;
            }
        }
    }
}