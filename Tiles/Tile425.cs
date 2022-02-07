using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace WireShark.Tiles;

public class Tile425 : TileInfo
{
    protected override void HitWireInternal()
    {
        {
            var num19 = tile.frameX % 36 / 18;
            var num20 = tile.frameY % 36 / 18;
            var num21 = i - num19;
            var num22 = j - num20;
            for (var num23 = num21; num23 < num21 + 2; num23++)
            {
                for (var num24 = num22; num24 < num22 + 2; num24++)
                {
                }
            }

            if (!Main.AnnouncementBoxDisabled)
            {
                var pink = Color.Pink;
                var num25 = Sign.ReadSign(num21, num22, false);
                if (num25 != -1 && Main.sign[num25] != null &&
                    !string.IsNullOrWhiteSpace(Main.sign[num25].text))
                {
                    if (Main.AnnouncementBoxRange == -1)
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewTextMultiline(Main.sign[num25].text, false, pink, 460);
                            return;
                        }

                        if (Main.netMode == NetmodeID.Server)
                        {
                        }
                    }
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        if (Main.player[Main.myPlayer]
                                .Distance(new Vector2(num21 * 16 + 16, num22 * 16 + 16)) <=
                            Main.AnnouncementBoxRange)
                        {
                            Main.NewTextMultiline(Main.sign[num25].text, false, pink, 460);
                        }
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        for (var num26 = 0; num26 < 255; num26++)
                        {
                            if (Main.player[num26].active &&
                                Main.player[num26]
                                    .Distance(new Vector2(num21 * 16 + 16, num22 * 16 + 16)) <=
                                Main.AnnouncementBoxRange)
                            {
                            }
                        }
                    }
                }
            }
        }
    }
}