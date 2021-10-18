using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WireShark.Items {
    public class Test : ModItem {
        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Test"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("This is a basic modded sword.");
        }

        public override void SetDefaults() {
            item.damage = 50;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }
        public override bool UseItem(Player player) {
            if (player.itemAnimation == player.itemAnimationMax - 2) {
                Point p = Main.MouseWorld.ToTileCoordinates();
                WiringWarpper.GetWireAccelerator().ActiviateAll(p.X, p.Y, new System.Collections.Generic.HashSet<int>());
                // Main.NewText("Activate");
            }
            return base.UseItem(player);
        }
        public override void AddRecipes() {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
