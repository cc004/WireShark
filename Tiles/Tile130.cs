using Terraria;
using Terraria.ID;

namespace WireShark.Tiles;

public class Tile130 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].IsActive)
            {
                if (TileID.Sets.BasicChest[Main.tile[i, j - 1].type] ||
                    TileID.Sets.BasicChestFake[Main.tile[i, j - 1].type])
                {
                    return;
                }

                if (Main.tile[i, j - 1].type == 88)
                {
                    return;
                }
            }

            tile.type = 131;
            WorldGen.SquareTileFrame(i, j, true);
        }
    }
}