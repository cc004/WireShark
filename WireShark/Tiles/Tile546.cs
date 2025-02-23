using Terraria;
using Terraria.ID;

namespace WireShark.Tiles;

public class Tile546 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].HasTile)
            {
                if (TileID.Sets.BasicChest[Main.tile[i, j - 1].TileType] ||
                    TileID.Sets.BasicChestFake[Main.tile[i, j - 1].TileType])
                {
                    return;
                }

                if (Main.tile[i, j - 1].TileType == 88)
                {
                    return;
                }
            }

            tile.TileType = 557;
            WorldGen.SquareTileFrame(i, j, true);
        }
    }
}