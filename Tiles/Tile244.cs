using Terraria;

namespace WireShark.Tiles;

public class Tile244 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            int num74;
            for (num74 = tile.TileFrameX / 18; num74 >= 3; num74 -= 3)
            {
            }

            int num75;
            for (num75 = tile.TileFrameY / 18; num75 >= 3; num75 -= 3)
            {
            }

            var num76 = i - num74;
            var num77 = j - num75;
            var num78 = 54;
            if (Main.tile[num76, num77].TileFrameX >= 54)
            {
                num78 = -54;
            }

            for (var num79 = num76; num79 < num76 + 3; num79++)
            {
                for (var num80 = num77; num80 < num77 + 2; num80++)
                {
                    Main.tile[num79, num80].TileFrameX = (short) (Main.tile[num79, num80].TileFrameX + num78);
                }
            }
        }
    }
}