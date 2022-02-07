using Terraria;

namespace WireShark.Tiles;

public class Tile338 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num69 = j - tile.frameY / 18;
            var num70 = i - tile.frameX / 18;


            if (WiringWrapper.CheckMech(num70, num69, 30))
            {
                var flag5 = false;
                for (var num71 = 0; num71 < 1000; num71++)
                {
                    if (Main.projectile[num71].active && Main.projectile[num71].aiStyle == 73 &&
                        Main.projectile[num71].ai[0] == num70 && Main.projectile[num71].ai[1] == num69)
                    {
                        flag5 = true;
                        break;
                    }
                }

                if (!flag5)
                {
                    Projectile.NewProjectile(null, num70 * 16 + 8, num69 * 16 + 2, 0f, 0f,
                        419 + Main.rand.Next(4), 0, 0f, Main.myPlayer, num70, num69);
                }
            }
        }
    }
}