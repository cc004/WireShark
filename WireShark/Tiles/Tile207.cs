using Terraria;

namespace WireShark.Tiles;

public class Tile207 : TileInfo
{
    protected override void HitWireInternal()
    {
        WorldGen.SwitchFountain(i, j);
    }
}