using Terraria;
using Terraria.ModLoader;

namespace SleepyGangMiniMod.Buffs
{
	public class GlacieCompanionPetBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Sleepy Ice Buddy");
			Description.SetDefault("You seem to have made a rather adorable friend");
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true;
			//Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 20000;
			SleepyGangMiniModPlayer modPlayer = (SleepyGangMiniModPlayer)player.GetModPlayer(mod, "SleepyGangMiniModPlayer");
			modPlayer.glacieCompanionPet = true;
			bool petProjectileNotSpawned = true;
			if (player.ownedProjectileCounts[mod.ProjectileType("GlacieCompanionPet")] > 0)
			{
				petProjectileNotSpawned = false;
			}
			if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.position.X + player.width / 2, player.position.Y + player.height / 2, 0f, 0f, mod.ProjectileType("GlacieCompanionPet"), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}