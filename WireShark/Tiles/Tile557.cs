using Terraria;

namespace WireShark.Tiles;

public class Tile557 : TileInfo
{
    protected override void HitWireInternal()
    {

        tile.TileType = 546;
        WorldGen.SquareTileFrame(i, j, true);
    }
}