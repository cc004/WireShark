namespace WireShark.Tiles;

public class Tile143 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num120 = j - tile.frameY / 18;
            var num121 = tile.frameX / 18;
            if (num121 > 1)
            {
                num121 -= 2;
            }

            num121 = i - num121;


            if (type == 142)
            {
                for (var num122 = 0; num122 < 4; num122++)
                {
                    if (WiringWrapper._numInPump >= 19)
                    {
                        return;
                    }

                    int num123;
                    int num124;
                    if (num122 == 0)
                    {
                        num123 = num121;
                        num124 = num120 + 1;
                    }
                    else if (num122 == 1)
                    {
                        num123 = num121 + 1;
                        num124 = num120 + 1;
                    }
                    else if (num122 == 2)
                    {
                        num123 = num121;
                        num124 = num120;
                    }
                    else
                    {
                        num123 = num121 + 1;
                        num124 = num120;
                    }

                    WiringWrapper._inPumpX[WiringWrapper._numInPump] = num123;
                    WiringWrapper._inPumpY[WiringWrapper._numInPump] = num124;
                    WiringWrapper._numInPump++;
                }

                return;
            }

            for (var num125 = 0; num125 < 4; num125++)
            {
                if (WiringWrapper._numOutPump >= 19)
                {
                    return;
                }

                int num126;
                int num127;
                if (num125 == 0)
                {
                    num126 = num121;
                    num127 = num120 + 1;
                }
                else if (num125 == 1)
                {
                    num126 = num121 + 1;
                    num127 = num120 + 1;
                }
                else if (num125 == 2)
                {
                    num126 = num121;
                    num127 = num120;
                }
                else
                {
                    num126 = num121 + 1;
                    num127 = num120;
                }

                WiringWrapper._outPumpX[WiringWrapper._numOutPump] = num126;
                WiringWrapper._outPumpY[WiringWrapper._numOutPump] = num127;
                WiringWrapper._numOutPump++;
            }
        }
    }
}