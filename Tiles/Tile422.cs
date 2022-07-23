﻿using Terraria;

namespace WireShark.Tiles;

public class Tile422 : TileInfo
{
    protected override void HitWireInternal()
    {
        if (!tile.HasActuator)
        {
            tile.TileType = 421;
            WorldGen.SquareTileFrame(i, j, true);
        }
    }
}