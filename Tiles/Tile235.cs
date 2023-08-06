using Terraria;

namespace WireShark.Tiles;

public class Tile235 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num72 = i - tile.TileFrameX / 18;
            if (tile.WallType == 87 && j > Main.worldSurface && !NPC.downedPlantBoss)
            {
                return;
            }

            if (WiringWrapper._teleport[0].X == -1f)
            {
                WiringWrapper._teleport[0].X = num72;
                WiringWrapper._teleport[0].Y = j;
                if (tile.IsHalfBlock)
                {
                    var expr_EFC_cp_0 = WiringWrapper._teleport;
                    var expr_EFC_cp_ = 0;
                    expr_EFC_cp_0[expr_EFC_cp_].Y = expr_EFC_cp_0[expr_EFC_cp_].Y + 0.5f;
                }
            }
            else if (WiringWrapper._teleport[0].X != num72 || WiringWrapper._teleport[0].Y != j)
            {
                WiringWrapper._teleport[1].X = num72;
                WiringWrapper._teleport[1].Y = j;
                if (tile.IsHalfBlock)
                {
                    var expr_F75_cp_0 = WiringWrapper._teleport;
                    var expr_F75_cp_ = 1;
                    expr_F75_cp_0[expr_F75_cp_].Y = expr_F75_cp_0[expr_F75_cp_].Y + 0.5f;
                }
            }
        }
    }
}