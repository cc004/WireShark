namespace WireShark.Tiles;

public class Tile4 : TileInfo
{
    protected override void HitWireInternal()
    {
        if (tile.frameX < 66)
        {
            tile.frameX += 66;
        }
        else
        {
            tile.frameX -= 66;
        }
    }
}