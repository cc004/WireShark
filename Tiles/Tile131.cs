using Terraria;

namespace WireShark.Tiles;

public class Tile131 : TileInfo
{
    protected override void HitWireInternal()
    {

        tile.type = 130;
        WorldGen.SquareTileFrame(i, j, true);
    }
}