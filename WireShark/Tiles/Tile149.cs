namespace WireShark.Tiles;

public class Tile149 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            if (tile.TileFrameX < 54)
            {
                var tile8 = tile;
                tile8.TileFrameX += 54;
            }
            else
            {
                var tile9 = tile;
                tile9.TileFrameX -= 54;
            }
        }
    }
}