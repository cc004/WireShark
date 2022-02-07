namespace WireShark.Tiles;

public class Tile419 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var lgate = WiringWrapper.onLogicLampChange[i, j];
            switch (tile.frameX)
            {
                case 0:
                {
                    if (lgate != null)
                    {
                        ++lgate.lampon;
                        if (!lgate.erroronly)
                            WiringWrapper._LampsToCheck.Enqueue(lgate);
                    }

                    tile.frameX = 18;
                    break;
                }
                case 18:
                {
                    if (lgate != null)
                    {
                        --lgate.lampon;
                        if (!lgate.erroronly)
                            WiringWrapper._LampsToCheck.Enqueue(lgate);
                    }

                    tile.frameX = 0;
                    break;
                }
                default:
                {
                    if (lgate != null)
                        WiringWrapper._LampsToCheck.Enqueue(lgate);
                    break;
                }
            }
        }
    }
}