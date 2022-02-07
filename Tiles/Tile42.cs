using Terraria;

namespace WireShark.Tiles;

public class Tile42 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            int num81;
            for (num81 = tile.frameY / 18; num81 >= 2; num81 -= 2)
            {
            }

            var num82 = j - num81;
            short num83 = 18;
            if (tile.frameX > 0)
            {
                num83 = -18;
            }

            var tile10 = Main.tile[i, num82];
            tile10.frameX += num83;
            var tile11 = Main.tile[i, num82 + 1];
            tile11.frameX += num83;
        }
    }
}