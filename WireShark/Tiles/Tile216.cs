using Terraria;

namespace WireShark.Tiles;

public class Tile216 : TileInfo
{
    protected override void HitWireInternal()
    {
        WorldGen.LaunchRocket(i, j, true);
    }
}