using Terraria;
using Terraria.ID;

namespace WireShark.Tiles;

public class Tile141 : TileInfo
{
    protected override void HitWireInternal()
    {
        WorldGen.KillTile(i, j, false, false, true);

        Projectile.NewProjectile(null, i * 16 + 8, j * 16 + 8, 0f, 0f, ProjectileID.Explosives, 500, 10f,
            Main.myPlayer, 0f, 0f);
    }
}