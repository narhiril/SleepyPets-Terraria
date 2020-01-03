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
			item.width = 20;
			item.height = 30;
			item.useStyle = 1;
			item.useAnimation = 30;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.buffType = ModContent.BuffType<Buffs.GlacieCompanionPetBuff>();
			item.shoot = ModContent.ProjectileType<Projectiles.GlacieCompanionPet>();
			item.UseSound = new Terraria.Audio.LegacySoundStyle(SoundID.Pixie, 0);
			item.useTime = 30;
			item.rare = 9;
			item.damage = 0;
			item.noMelee = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}