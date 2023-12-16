using Microsoft.Xna.Framework;
using Terraria;

namespace WireShark.Tiles;

public class Tile443 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num117 = tile.TileFrameX / 36;
            var num118 = i - (tile.TileFrameX - num117 * 36) / 18;
            if (Wiring.CheckMech(num118, j, 200))
            {
                var vector2 = Vector2.Zero;
                var zero2 = Vector2.Zero;
                var num119 = 654;
                var damage3 = 20;
                if (num117 < 2)
                {
                    vector2 = new Vector2(num118 + 1, j) * 16f;
                    zero2 = new Vector2(0f, -8f);
                }
                else
                {
                    vector2 = new Vector2(num118 + 1, j + 1) * 16f;
                    zero2 = new Vector2(0f, 8f);
                }

                if (num119 != 0)
                {
                    Projectile.NewProjectile(null, (int) vector2.X, (int) vector2.Y, zero2.X, zero2.Y, num119,
                        damage3, 2f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}