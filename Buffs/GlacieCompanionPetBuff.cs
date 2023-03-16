using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SleepyGangMiniMod.Buffs
{
	public class GlacieCompanionPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
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
			SleepyGangMiniModPlayer modPlayer = player.GetModPlayer<SleepyGangMiniModPlayer>();
			modPlayer.glacieCompanionPet = true;
			bool petProjectileNotSpawned = true;
			if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("GlacieCompanionPet").Type] > 0)
			{
				petProjectileNotSpawned = false;
			}
			if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(spawnSource: player.GetSource_Buff(buffIndex),
										 position: new Vector2(player.position.X + player.width / 2, player.position.Y + player.height / 2),
										 velocity: new Vector2(0f, 0f),
										 Type: Mod.Find<ModProjectile>("GlacieCompanionPet").Type,
										 Damage: 0,
										 KnockBack: 0f,
										 Owner: player.whoAmI,
										 ai0: 0f,
										 ai1: 0f
				);
			}
		}
	}
}