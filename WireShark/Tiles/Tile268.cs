using Terraria;

namespace WireShark.Tiles;

public class Tile268 : TileInfo
{
    protected override void HitWireInternal()
    {
        if (!tile.HasActuator)
        {
            if (type >= 262)
            {
                type -= 7;
            }
            else
            {
                type += 7;
            }

            WorldGen.SquareTileFrame(i, j, true);
        }
    }
}