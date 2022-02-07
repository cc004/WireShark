using Terraria;

namespace WireShark.Tiles;

public class Tile429 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num151 = (short) (Main.tile[i, j].frameX / 18);
            var flag6 = num151 % 2 >= 1;
            var flag7 = num151 % 4 >= 2;
            var flag8 = num151 % 8 >= 4;
            var flag9 = num151 % 16 >= 8;
            var flag10 = false;
            short num73 = 0;
            switch (WiringWrapper._currentWireColor)
            {
                case 1:
                    num73 = 18;
                    flag10 = !flag6;
                    break;
                case 2:
                    num73 = 72;
                    flag10 = !flag8;
                    break;
                case 3:
                    num73 = 36;
                    flag10 = !flag7;
                    break;
                case 4:
                    num73 = 144;
                    flag10 = !flag9;
                    break;
            }

            if (flag10)
            {
                var tile6 = tile;
                tile6.frameX += num73;
            }
            else
            {
                var tile7 = tile;
                tile7.frameX -= num73;
            }
        }
    }
}