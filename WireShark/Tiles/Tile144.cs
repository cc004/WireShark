using Terraria;

namespace WireShark.Tiles;

public class Tile144 : TileInfo
{
    protected override void HitWireInternal()
    {
        WiringWrapper.HitSwitch(i, j);
        WorldGen.SquareTileFrame(i, j, true);
    }
}