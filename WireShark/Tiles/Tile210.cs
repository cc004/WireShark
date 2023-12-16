using Terraria;

namespace WireShark.Tiles;

public class Tile210 : TileInfo
{
    protected override void HitWireInternal()
    {
        WorldGen.ExplodeMine(i, j, true);
    }
}