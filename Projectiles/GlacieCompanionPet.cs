using System;
using Microsoft.Xna.Framework;
using Terraria;
//using Terraria.ID;
using Terraria.ModLoader;

namespace SleepyGangMiniMod.Projectiles
{
	public class GlacieCompanionPet : SleepyGangMiniModProjectile
	{
		public int aiState;
		public float maxSpeed;
		protected bool isMovingTowardsPlayer = false;
		protected bool isSpriteRotated = false;
		protected bool doCollideFlag = false;
		protected bool useGroundMovement = false;
		protected bool isAsleep = false;
		protected bool isStuck = false;
		protected bool animationReversed = false;
		protected int aiStatePrevious;
		protected int firstAnimationFrameIndex;
		protected int lastAnimationFrameIndex;

		public override void SetDefaults()
		{
			projectile.netImportant = true;
			projectile.aiStyle = 0; //no vanilla ai, use custom
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.timeLeft *= 5;
			projectile.tileCollide = true;
			projectile.width = 32;
			projectile.height = 25;
			drawOffsetX = -12;
			drawOriginOffsetX = -9;
			drawOriginOffsetY = -9;
			projectile.ai[0] = 0f;
			projectile.ai[1] = 0f;
			projectile.frameCounter = 0;
			firstAnimationFrameIndex = 0;
			lastAnimationFrameIndex = (Main.projFrames[projectile.type] - 1);
			aiState = 1;
			maxSpeed = 8f;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sleepy Ice Buddy");
			Main.projPet[projectile.type] = true;
			Main.projFrames[projectile.type] = 16;
			//projectile.width = 50;
			//projectile.height = 40;
		}


		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			SleepyGangMiniModPlayer modPlayer = player.GetModPlayer<SleepyGangMiniModPlayer>();


			/*
			 *This 'if' tree checks distance thresholds to determine whether or not to move towards the player
			*/

			if ((Math.Abs(player.position.X - projectile.position.X) > 2550) || (Math.Abs(player.position.Y - projectile.position.Y) > 2550)) //if too far, teleport to owner
			{
				SGProjectileMoveTowardsPoint(player.position, 0f, 0f, 0f, 0f); //teleport
				//projectile.velocity.X = player.velocity.X; //decided to cut this, for now anyway
				//projectile.velocity.Y = player.velocity.Y;
				aiState = 1;
				isAsleep = false;
				isMovingTowardsPlayer = false;
				projectile.frameCounter = 0;
				return;
			}
			else if (aiState == 5 || aiState == 6)
			{
				goto AIStateSwitch; //skip the usual movement checks for special, stationary ai states
			}
			else if ((Math.Abs(player.position.X - projectile.position.X) + Math.Abs(player.position.Y - projectile.position.Y)) > 750) //seek out owner (no-collide)
			{
				isAsleep = false;
				projectile.tileCollide = false;
				doCollideFlag = false;
				isMovingTowardsPlayer = true;
				aiState = 1;
			}
			else if (!isAsleep && ((Math.Abs(player.position.X - projectile.position.X) > 200) || (Math.Abs(player.position.Y - projectile.position.Y) > 300))) //seek out owner
			{
				projectile.tileCollide = doCollideFlag; //this prevents an abrupt switch from no-collide to collide when crossing the distance threshold
				isMovingTowardsPlayer = true;
				aiState = 2; //if this doesn't get overwritten by a movement check, it will end up setting doCollideFlag to true;
			}
			else
			{
				projectile.tileCollide = doCollideFlag;
				isMovingTowardsPlayer = false;
				isStuck = false;
			}

			/*
			 * This 'if' tree performs movement checks, updating aiState if necessary
			*/

			if (isStuck || Math.Abs(projectile.velocity.Y) > 2f || (aiState != 2 && Math.Abs(projectile.velocity.Y) > 0.25f)) //if flying or falling
			{
				aiState = 1;
			}
			else if (Math.Abs(projectile.velocity.X) >= 0.1f) //if moving on X axis, but not flying or falling
			{
				aiState = 2;
			}
			else //not moving
			{
				if (isAsleep)
				{
					aiState = 4;
				}
				else if (!isMovingTowardsPlayer && (aiState < 3)) //if not seeking player and not in special animation
				{
					aiState = 0;
				}
				projectile.velocity.X = 0f;
				projectile.velocity.Y = 0f;
			}

		/*
		 * 
		 * The following 'switch' statement handles the pet's ai state.
		 * 
		 * 0 = idle, standard animation
		 * 1 = flying or falling
		 * 2 = running
		 * 3 = idle, open eyes animation
		 * 4 = idle, sleeping animation
		 * 5 = special animation state, waking up (unfinished)
		 * 6 = special animation state #2 (planned, not yet implemented)
		 * 
		 * 
		*/
	
		AIStateSwitch:
			switch (aiState)
			{
				case 0: //idle
					isMovingTowardsPlayer = false;
					projectile.tileCollide = true;
					doCollideFlag = true;
					/*
					 * This next bit handles special idle animations
					*/
					projectile.ai[0] += 0.03f;
					if (projectile.ai[0] > 3f) //sway back and forth after ~3s
					{
						isSpriteRotated = true;
						projectile.rotation = 0.125f * (float)Math.Sin(projectile.ai[0] - 2f);
					}
					else
					{
						isSpriteRotated = false;
					}
					if (projectile.ai[0] > 15f) // after ~15 seconds of idle
					{
						if (projectile.frameCounter == 0)
						{
							var idleAnimationRandomizer = Main.rand.Next(100);
							if (idleAnimationRandomizer >= 99)
							{
								aiState = 3; //switch to blinking idle animation
								projectile.ai[0] = 0f;
								goto case 3;
							}
							else if ((idleAnimationRandomizer <= 2) || projectile.ai[0] >= 30f)
							{
								aiState = 4; //switch to sleeping idle animation
								projectile.ai[0] = 0f;
								goto case 4;
							}
						}
					}
					//animation stuff
					firstAnimationFrameIndex = 1; //idle animation
					lastAnimationFrameIndex = 3;
					SGProjectileAnimateBetweenFrames(firstAnimationFrameIndex, lastAnimationFrameIndex, ref animationReversed, 14, 0, true);
					SGProjectileFacePlayer(player, false, 20f);
					break;
				case 1: //flying or falling
					useGroundMovement = false;
					if (isMovingTowardsPlayer) //flying
					{
						projectile.rotation = Math.Abs(projectile.velocity.X) * -0.05f * projectile.direction;
						isSpriteRotated = true;
					}
					projectile.ai[0] = 0f;
					//animation stuff
					firstAnimationFrameIndex = 12; //falling-flying animation
					lastAnimationFrameIndex = 13;
					SGProjectileAnimateBetweenFrames(firstAnimationFrameIndex, lastAnimationFrameIndex, 6);
					SGProjectileFacePlayer(player, false, 40f);
					break;
				case 2: //running
					isStuck = false;
					isMovingTowardsPlayer = true; //this supercedes earlier movement checks, it prevents some odd behavior
					projectile.tileCollide = true;
					doCollideFlag = true;
					isSpriteRotated = false;
					if (Math.Abs(player.position.Y - projectile.position.Y) < 75f)
					{
						useGroundMovement = true;
					}
					projectile.ai[0] = 0f;
					//animation stuff
					firstAnimationFrameIndex = 9; //running animation
					lastAnimationFrameIndex = 12;
					SGProjectileAnimateBetweenFrames(firstAnimationFrameIndex, lastAnimationFrameIndex, 6);
					SGProjectileFacePlayer(player, false, 40f);
					break;
				case 3: //idle, blinking animation
					isMovingTowardsPlayer = false;
					projectile.tileCollide = true;
					doCollideFlag = true;
					isSpriteRotated = false;
					//animation stuff
					firstAnimationFrameIndex = 1; //blinking idle animation
					lastAnimationFrameIndex = 8;
					SGProjectileAnimateBetweenFrames(firstAnimationFrameIndex, lastAnimationFrameIndex, 9, 3);
					SGProjectileFacePlayer(player, false, 40f);
					projectile.ai[0] += 0.03f;
					if (projectile.ai[0] > 4f) //revert to regular idle state on timer
					{
						projectile.ai[0] = 0f;
						aiState = 0;
						projectile.frameCounter = 0;
						goto case 0;
					}
					break;
				case 4: //idle, sleeping
				default:
					isAsleep = true;
					isMovingTowardsPlayer = false;
					projectile.tileCollide = true;
					doCollideFlag = true;
					isSpriteRotated = false;
					projectile.ai[0] += 0.03f;
					if (projectile.ai[0] > 3f)
					{
						projectile.ai[1] += 1f;
						projectile.ai[0] -= 0.60f;
						_ = Dust.NewDust(new Vector2(projectile.position.X + 5f + (3*projectile.ai[1]), projectile.position.Y - 10f - (2*projectile.ai[1])), 10, 10, mod.DustType("SleepyParticles"), 0f, 0f, 0, new Color(5, 180, 200), 1f);
						if (projectile.ai[1] > 2f)
						{
							projectile.ai[1] = 0f;
							projectile.ai[0] = 0f;
						}
					}
					if (Math.Abs(Main.MouseWorld.X - projectile.position.X) < projectile.width && Math.Abs(Main.MouseWorld.Y - projectile.position.Y) < projectile.height) //wake up with mouse
					{
						isAsleep = false;
						aiState = 5;
						projectile.ai[0] = 0f;
						projectile.ai[1] = 0f;
						projectile.frameCounter = 0;
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/GlacieNoises"), projectile.position);
						_ = Dust.NewDust(new Vector2(projectile.position.X + (-10f * projectile.spriteDirection), projectile.position.Y - 10f), 10, 10, mod.DustType("WakeupParticles"));
						break;
					}
					//animation stuff
					firstAnimationFrameIndex = 0;
					lastAnimationFrameIndex = 0;
					SGProjectileAnimateBetweenFrames(firstAnimationFrameIndex, lastAnimationFrameIndex, 7);
					break;
				case 5: //wakeup sequence, not yet fully implemented
					isAsleep = false;
					isMovingTowardsPlayer = false;
					projectile.tileCollide = true;
					doCollideFlag = true;
					isSpriteRotated = true;
					//animation stuff
					firstAnimationFrameIndex = 14;
					lastAnimationFrameIndex = 15;
					SGProjectileAnimateBetweenFrames(firstAnimationFrameIndex, lastAnimationFrameIndex, 6);
					SGProjectileFacePlayer(player, false, 20f);
					projectile.ai[0] += 0.03f;
					projectile.rotation = 0.15f * (float)Math.Sin(projectile.ai[0] * 5f);
					if (projectile.ai[0] < 3f)
					{
						goto MiscAIChecks; //skip movement checks
					}
					else
					{
						aiState = 0;
						projectile.ai[0] = 0f;
					}
					break;
				case 6: //not yet fully implemented
					isAsleep = false;
					isMovingTowardsPlayer = false;
					projectile.tileCollide = true;
					doCollideFlag = true;
					//animation stuff
					firstAnimationFrameIndex = 0; //todo, needs new sprite(s) on sprite sheet
					lastAnimationFrameIndex = 0;
					SGProjectileAnimateBetweenFrames(firstAnimationFrameIndex, lastAnimationFrameIndex, 6);
					SGProjectileFacePlayer(player, false, 40f);
					break;


			}

			/*
			 * The next 'if' tree handles the pet's actual movement 
			 * 
			*/
			if (isMovingTowardsPlayer)
			{
				isAsleep = false; //no sleepwalking here
				if (doCollideFlag)
				{
					if (useGroundMovement) //ground movement (duh lol)
					{
						if ((Math.Abs(projectile.velocity.X) < 0.1f) && (Math.Abs(projectile.velocity.Y) < 0.1f)) //detect if stuck on terrain
						{
							projectile.velocity.Y -= 0.3f * (2f + projectile.ai[1]); //bounce up
							projectile.ai[1] += 1f; //make next bounce stronger
							if (projectile.ai[1] > 7f)
							{
								projectile.ai[1] = 7f;
								projectile.position.Y -= 4f;
								projectile.tileCollide = false;
								doCollideFlag = false;
								projectile.velocity.Y -= (maxSpeed / 3f); //rocket up
								aiState = 1;
								useGroundMovement = false;
								isStuck = true;
								SGProjectileMoveTowardsPoint(new Vector2(player.position.X, player.position.Y - (player.height + 5f)), maxSpeed, 1f, 3f, 60f);
							}
						}
						if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height) && !isStuck) //if airborne
						{
							projectile.velocity.Y += 0.3f; //gravity
						}
						if (projectile.wet)
						{
							SGProjectileMoveTowardsPoint(new Vector2(player.position.X, (player.position.Y + (player.height / 2f))), (maxSpeed * 0.75f), 0.8f, 4f, 100f); //adjusted maxSpeed and acceleration if in water
						}
						else
						{
							SGProjectileMoveTowardsPoint(new Vector2(player.position.X, (player.position.Y + (player.height / 2f))), maxSpeed, 1f, 3f, 100f);
						}
					}
					else //aerial movement
					{
						projectile.ai[1] = 0f; //reset bounce strength modifier
						if (projectile.wet)
						{
							SGProjectileMoveTowardsPoint(new Vector2(player.position.X, (player.position.Y + (player.height / 2f))), (maxSpeed * 0.75f), 0.8f, 4f, 100f);
						}
						else
						{
							SGProjectileMoveTowardsPoint(new Vector2(player.position.X, (player.position.Y + (player.height / 2f))), maxSpeed, 1f, 3f, 100f);
						}
					}
				}
				else // no-collide movement
				{
					SGProjectileMoveTowardsPoint(new Vector2(player.position.X, player.position.Y - (player.height + 10f)), maxSpeed, 1f, 3f, 50f); //seek out point above player, prevents some instances of getting stuck in ground
				}
			}
			else if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height)) //if in the air, but not moving towards player
			{
				isStuck = false;
				if (projectile.velocity.Y < 10f)
				{
					projectile.velocity.Y += 0.3f; //gravity
				}
			}
			else //if not moving towards player and also not flying/falling
			{
				isStuck = false;
				if (doCollideFlag && Collision.SolidCollision(projectile.position, projectile.width, projectile.height) && Collision.SolidCollision(new Vector2(projectile.position.X, projectile.position.Y - (projectile.height / 6f)), projectile.width, projectile.height))
				{
					if (projectile.position.Y < (player.position.Y - 50f)) //this check is to prevent getting stuck in the floor or ceiling when coming out of no-collide mode
					{
						projectile.position.Y += projectile.height / 6f;
					}
					else
					{
						projectile.position.Y -= projectile.height / 6f; 
					}
				}
				if (Math.Abs(projectile.velocity.X) > 0.01f) //these next two if statements fix an uncommon bug where the pet would go soaring off into the sky during an incomplete no-collide movement
				{
					projectile.velocity.X = projectile.velocity.X / 2f;
				}
				if (Math.Abs(projectile.velocity.Y) > 0.01f) 
				{
					projectile.velocity.Y = projectile.velocity.Y / 2f;
				}
			}

			/*
			 * 
			 * Final, misc AI checks
			 * 
			*/

			MiscAIChecks:

			if (aiStatePrevious != aiState) //on ai state change
			{
				projectile.frameCounter = 0;
				projectile.ai[0] = 0f;
				projectile.ai[1] = 0f;
			}
			
			aiStatePrevious = aiState; //update this after it's been checked

			if (!isSpriteRotated) //reset rotation
			{
				projectile.rotation = 0f;
			}

			if (player.dead) //cleanup on player death
			{
				modPlayer.glacieCompanionPet = false;
			}
			if (modPlayer.glacieCompanionPet) //makes pet buff persistent until dismissed
			{
				projectile.timeLeft = 2;
			}

		}

	}
}