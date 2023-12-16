using Terraria;

namespace WireShark.Tiles;

public class Tile92 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num96 = j - tile.TileFrameY / 18;
            short num97 = 18;
            if (tile.TileFrameX > 0)
            {
                num97 = -18;
            }

            for (var num98 = num96; num98 < num96 + 6; num98++)
            {
                var tile21 = Main.tile[i, num98];
                tile21.TileFrameX += num97;
            }
        }
    }
}