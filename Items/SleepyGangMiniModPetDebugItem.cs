using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace SleepyGangMiniMod.Items
{
	public class SleepyGangMiniModPetDebugItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glacie Pet Debug Item");
			Tooltip.SetDefault("How did you get this, anyway?");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 30;
			Item.useStyle = 1;
			Item.useAnimation = 30;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.buffType = ModContent.BuffType<Buffs.GlacieCompanionPetBuff>();
			Item.shoot = ModContent.ProjectileType<Projectiles.GlacieCompanionPet>();
			Item.UseSound = SoundID.Pixie;
			Item.useTime = 30;
			Item.rare = 9;
			Item.damage = 0;
			Item.noMelee = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock);
			recipe.Register();
		}
	}
}