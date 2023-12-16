using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WireShark.Items {
    public class Test : ModItem {
        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Test"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("This is a basic modded sword.");
        }
        public override void SetDefaults() {
            Item.damage = 50;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override bool? UseItem(Player player) {
            if (player.itemAnimation == player.itemAnimationMax - 2) {
                var p = Main.MouseWorld.ToTileCoordinates();
                Main.NewText($"point={p}");
            }
            return base.UseItem(player);
        }
    }
}
