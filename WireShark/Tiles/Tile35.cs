using Terraria;

namespace WireShark.Tiles;

public class Tile35 : TileInfo
{
    protected override void HitWireInternal()
    {
        WorldGen.SwitchMB(i, j);
    }
}