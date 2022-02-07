using Terraria;

namespace WireShark.Tiles;

public class Tile406 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num2 = tile.frameX % 54 / 18;
            var num3 = tile.frameY % 54 / 18;
            var num4 = i - num2;
            var num5 = j - num3;
            var num6 = 54;
            if (Main.tile[num4, num5].frameY >= 108)
            {
                num6 = -108;
            }

            for (var k = num4; k < num4 + 3; k++)
            {
                for (var l = num5; l < num5 + 3; l++)
                {
                    Main.tile[k, l].frameY = (short) (Main.tile[k, l].frameY + num6);
                }
            }
        }
    }
}