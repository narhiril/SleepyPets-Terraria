using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SleepyGangMiniMod.Dusts
{
    class SleepyParticles : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            //dust.color = new Color(5, 180, 200); //deprecated, now handled by spawn code
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 10, 10);
            dust.alpha = 1;
            dust.scale = 1f;
            dust.velocity = new Vector2(10f / Main.rand.Next(100, 200), -10f / Main.rand.Next(100, 200));
            dust.noLight = true;
            dust.rotation = 0f;
        }

        public override bool Update(Dust dust)
        {
            int previousAlpha = dust.alpha;
            dust.alpha = (int)(1.05f * dust.alpha); //fade
            if (previousAlpha == dust.alpha)
            {
                dust.alpha++;
            }
            if ((previousAlpha % 5) == 0) //color shift
            {
                dust.color.G += 1;
                dust.color.B += 1;
            }
            dust.rotation = 0.3f * (float)Math.Sin(dust.color.B); //wobble
            dust.position += dust.velocity; //move
            dust.velocity *= 1.025f; //accelerate
            if (dust.alpha > 255)
            {
                dust.alpha = 255;
                dust.active = false; //cleanup
            }
            return false;
        }

    }
}
