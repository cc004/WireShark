using Terraria.GameContent.Events;

namespace WireShark.Tiles;

public class Tile455 : TileInfo
{
    protected override void HitWireInternal()
    {
        BirthdayParty.ToggleManualParty();
    }
}