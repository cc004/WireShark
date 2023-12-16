using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace WireShark.Tiles;

public class Tile235 : TileInfo
{
    public static Vector2[] _teleport = new Vector2[2];

    static Tile235()
    {
        _teleport[0] = new Vector2(-1, -1);
        _teleport[1] = new Vector2(-1, -1);
    }
    protected override unsafe void HitWireInternal()
    {
        int num153 = i - tile.TileFrameX / 18;
        if (_teleport[0].X == -1f)
        {
            _teleport[0].X = num153;
            _teleport[0].Y = j;
            if (tile.IsHalfBlock)
            {
                _teleport[0].Y += 0.5f;
            }
        }
        else if (_teleport[0].X != num153 || _teleport[0].Y != j)
        {
            _teleport[1].X = num153;
            _teleport[1].Y = j;
            if (tile.IsHalfBlock)
            {
                _teleport[1].Y += 0.5f;
            }

            Teleport();
            _teleport[0] = new Vector2(-1, -1);
            _teleport[1] = new Vector2(-1, -1);
        }
    }
    private static bool TeleporterHitboxIntersects(Rectangle teleporter, Rectangle entity)
    {
        Rectangle rectangle = Rectangle.Union(teleporter, entity);
        return rectangle.Width <= teleporter.Width + entity.Width && rectangle.Height <= teleporter.Height + entity.Height;
    }

    private static void Teleport()
    {
        if (_teleport[0].X < _teleport[1].X + 3f && _teleport[0].X > _teleport[1].X - 3f && _teleport[0].Y > _teleport[1].Y - 3f && _teleport[0].Y < _teleport[1].Y)
        {
            return;
        }
        Rectangle[] array = new Rectangle[2];
        array[0].X = (int)(_teleport[0].X * 16f);
        array[0].Width = 48;
        array[0].Height = 48;
        array[0].Y = (int)(_teleport[0].Y * 16f - (float)array[0].Height);
        array[1].X = (int)(_teleport[1].X * 16f);
        array[1].Width = 48;
        array[1].Height = 48;
        array[1].Y = (int)(_teleport[1].Y * 16f - (float)array[1].Height);
        for (int i = 0; i < 2; i++)
        {
            Vector2 vector = new Vector2((float)(array[1].X - array[0].X), (float)(array[1].Y - array[0].Y));
            if (i == 1)
            {
                vector = new Vector2((float)(array[0].X - array[1].X), (float)(array[0].Y - array[1].Y));
            }
            if (!Wiring.blockPlayerTeleportationForOneIteration)
            {
                for (int j = 0; j < 255; j++)
                {
                    if (Main.player[j].active && !Main.player[j].dead && !Main.player[j].teleporting && TeleporterHitboxIntersects(array[i], Main.player[j].Hitbox))
                    {
                        Vector2 vector2 = Main.player[j].position + vector;
                        Main.player[j].teleporting = true;
                        if (Main.netMode == 2)
                        {
                            RemoteClient.CheckSection(j, vector2, 1);
                        }
                        Main.player[j].Teleport(vector2, 0, 0);
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(65, -1, -1, null, 0, (float)j, vector2.X, vector2.Y, 0, 0, 0);
                        }
                    }
                }
            }
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].teleporting && Main.npc[k].lifeMax > 5 && !Main.npc[k].boss && !Main.npc[k].noTileCollide)
                {
                    int type = Main.npc[k].type;
                    if (!NPCID.Sets.TeleportationImmune[type] && TeleporterHitboxIntersects(array[i], Main.npc[k].Hitbox))
                    {
                        Main.npc[k].teleporting = true;
                        Main.npc[k].Teleport(Main.npc[k].position + vector, 0, 0);
                    }
                }
            }
        }
        for (int l = 0; l < 255; l++)
        {
            Main.player[l].teleporting = false;
        }
        for (int m = 0; m < 200; m++)
        {
            Main.npc[m].teleporting = false;
        }
    }
}