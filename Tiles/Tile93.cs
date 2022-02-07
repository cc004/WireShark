using Terraria;

namespace WireShark.Tiles;

public class Tile93 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            int num84;
            for (num84 = tile.frameY / 18; num84 >= 3; num84 -= 3)
            {
            }

            num84 = j - num84;
            short num85 = 18;
            if (tile.frameX > 0)
            {
                num85 = -18;
            }

            var tile12 = Main.tile[i, num84];
            tile12.frameX += num85;
            var tile13 = Main.tile[i, num84 + 1];
            tile13.frameX += num85;
            var tile14 = Main.tile[i, num84 + 2];
            tile14.frameX += num85;
        }
    }
}