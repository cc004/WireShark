using Terraria;

namespace WireShark.Tiles;

public class Tile335 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num67 = j - tile.frameY / 18;
            var num68 = i - tile.frameX / 18;


            if (WiringWrapper.CheckMech(num68, num67, 30))
            {
                WorldGen.LaunchRocketSmall(num68, num67, true);
            }
        }
    }
}