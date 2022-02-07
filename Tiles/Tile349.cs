using Terraria;
using Terraria.ID;

namespace WireShark.Tiles;

public class Tile349 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num143 = j - tile.frameY / 18;
            int num144;
            for (num144 = tile.frameX / 18; num144 >= 2; num144 -= 2)
            {
            }

            num144 = i - num144;


            short num145;
            if (Main.tile[num144, num143].frameX == 0)
            {
                num145 = 216;
            }
            else
            {
                num145 = -216;
            }

            for (var num146 = 0; num146 < 2; num146++)
            {
                for (var num147 = 0; num147 < 3; num147++)
                {
                    var tile23 = Main.tile[num144 + num146, num143 + num147];
                    tile23.frameX += num145;
                }
            }

            if (Main.netMode == NetmodeID.Server)
            {
            }

            Animation.NewTemporaryAnimation((num145 > 0) ? 0 : 1, 349, num144, num143);
        }
    }
}