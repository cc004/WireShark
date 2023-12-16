namespace WireShark.Tiles;

public class Tile4 : TileInfo
{
    protected override void HitWireInternal()
    {
        if (tile.TileFrameX < 66)
        {
            tile.TileFrameX += 66;
        }
        else
        {
            tile.TileFrameX -= 66;
        }
    }
}