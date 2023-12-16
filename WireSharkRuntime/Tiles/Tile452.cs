using Terraria;

namespace WireShark.Tiles;

public class Tile452 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num7 = tile.TileFrameX % 54 / 18;
            var num8 = tile.TileFrameY % 54 / 18;
            var num9 = i - num7;
            var num10 = j - num8;
            var num11 = 54;
            if (Main.tile[num9, num10].TileFrameX >= 54)
            {
                num11 = -54;
            }

            for (var m = num9; m < num9 + 3; m++)
            {
                for (var n = num10; n < num10 + 3; n++)
                {
                    Main.tile[m, n].TileFrameX = (short) (Main.tile[m, n].TileFrameX + num11);
                }
            }
        }
    }
}