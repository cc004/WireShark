using Terraria;

namespace WireShark.Tiles;

public class Tile34 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            int num89;
            for (num89 = tile.TileFrameY / 18; num89 >= 3; num89 -= 3)
            {
            }

            var num90 = j - num89;
            var num91 = tile.TileFrameX % 108 / 18;
            if (num91 > 2)
            {
                num91 -= 3;
            }

            num91 = i - num91;
            short num92 = 54;
            if (Main.tile[num91, num90].TileFrameX % 108 > 0)
            {
                num92 = -54;
            }

            for (var num93 = num91; num93 < num91 + 3; num93++)
            {
                for (var num94 = num90; num94 < num90 + 3; num94++)
                {
                    var tile19 = Main.tile[num93, num94];
                    tile19.TileFrameX += num92;
                }
            }
        }
    }
}