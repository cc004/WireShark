using Terraria;

namespace WireShark.Tiles;

public class Tile388 : TileInfo
{
    protected override void HitWireInternal()
    {
        var flag4 = type == 389;
        WorldGen.ShiftTallGate(i, j, flag4);
    }
}