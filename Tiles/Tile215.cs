using Terraria;

namespace WireShark.Tiles;

public class Tile215 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num58 = tile.TileFrameX % 54 / 18;
            var num59 = tile.TileFrameY % 36 / 18;
            var num60 = i - num58;
            var num61 = j - num59;
            var num62 = 36;
            if (Main.tile[num60, num61].TileFrameY >= 36)
            {
                num62 = -36;
            }

            for (var num63 = num60; num63 < num60 + 3; num63++)
            {
                for (var num64 = num61; num64 < num61 + 2; num64++)
                {
                    Main.tile[num63, num64].TileFrameY = (short) (Main.tile[num63, num64].TileFrameY + num62);
                }
            }
        }
    }
}