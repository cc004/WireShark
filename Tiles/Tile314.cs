using Terraria;

namespace WireShark.Tiles;

public class Tile314 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            if (WiringWrapper.CheckMech(i, j, 5))
            {
                Minecart.FlipSwitchTrack(i, j);
            }
        }
    }
}