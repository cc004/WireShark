using Terraria;

namespace WireShark.Tiles;

public class Tile386 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var value = type == 387;
            var num65 = WorldGen.ShiftTrapdoor(i, j, true, -1).ToInt();
            if (num65 == 0)
            {
                num65 = -WorldGen.ShiftTrapdoor(i, j, false, -1).ToInt();
            }

            if (num65 != 0)
            {
            }
        }
    }
}