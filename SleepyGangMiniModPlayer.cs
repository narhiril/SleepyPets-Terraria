using Terraria.ModLoader;

namespace SleepyGangMiniMod
{
	public class SleepyGangMiniModPlayer : ModPlayer
	{
		public bool glacieCompanionPet = false;

		public override void ResetEffects()
		{
			glacieCompanionPet = false;
		}
	}
}