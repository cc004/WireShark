using Terraria;

namespace WireShark.Tiles;

public class Tile410 : TileInfo
{
    protected override void HitWireInternal()
    {
        WorldGen.SwitchMonolith(i, j);
    }
}