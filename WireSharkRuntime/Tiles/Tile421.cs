using Terraria;

namespace WireShark.Tiles;

public class Tile421 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            if (!tile.HasActuator)
            {
                tile.TileType = 422;
                WorldGen.SquareTileFrame(i, j, true);

            }
        }
    }
}