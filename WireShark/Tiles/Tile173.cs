using Terraria;

namespace WireShark.Tiles;

public class Tile173 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            int num86;
            for (num86 = tile.TileFrameY / 18; num86 >= 2; num86 -= 2)
            {
            }

            num86 = j - num86;
            var num87 = tile.TileFrameX / 18;
            if (num87 > 1)
            {
                num87 -= 2;
            }

            num87 = i - num87;
            short num88 = 36;
            if (Main.tile[num87, num86].TileFrameX > 0)
            {
                num88 = -36;
            }

            var tile15 = Main.tile[num87, num86];
            tile15.TileFrameX += num88;
            var tile16 = Main.tile[num87, num86 + 1];
            tile16.TileFrameX += num88;
            var tile17 = Main.tile[num87 + 1, num86];
            tile17.TileFrameX += num88;
            var tile18 = Main.tile[num87 + 1, num86 + 1];
            tile18.TileFrameX += num88;
        }
    }
}