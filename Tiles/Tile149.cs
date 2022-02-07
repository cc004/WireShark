namespace WireShark.Tiles;

public class Tile149 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            if (tile.frameX < 54)
            {
                var tile8 = tile;
                tile8.frameX += 54;
            }
            else
            {
                var tile9 = tile;
                tile9.frameX -= 54;
            }
        }
    }
}