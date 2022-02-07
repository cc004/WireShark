namespace WireShark.Tiles;

public class Tile174 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            short num95 = 18;
            if (tile.frameX > 0)
            {
                num95 = -18;
            }

            var tile20 = tile;
            tile20.frameX += num95;
        }
    }
}