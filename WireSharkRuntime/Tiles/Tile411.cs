using Terraria;

namespace WireShark.Tiles;

public class Tile411 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num12 = tile.TileFrameX % 36 / 18;
            var num13 = tile.TileFrameY % 36 / 18;
            var num14 = i - num12;
            var num15 = j - num13;
            var num16 = 36;
            if (Main.tile[num14, num15].TileFrameX >= 36)
            {
                num16 = -36;
            }

            for (var num17 = num14; num17 < num14 + 2; num17++)
            {
                for (var num18 = num15; num18 < num15 + 2; num18++)
                {
                    Main.tile[num17, num18].TileFrameX = (short) (Main.tile[num17, num18].TileFrameX + num16);
                }
            }
        }
    }
}