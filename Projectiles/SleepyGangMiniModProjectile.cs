using System;
using Microsoft.Xna.Framework;
using Terraria;
//using Terraria.ID;
using Terraria.ModLoader;

namespace SleepyGangMiniMod.Projectiles
{
    public class SleepyGangMiniModProjectile : ModProjectile // class contains shared methods for projectiles used by this mod
	{

		/// <summary>
		/// Flips projectile sprite to face a target player
		/// </summary>
		public void SGProjectileFacePlayer(Player targetPlayer, bool spriteIsFacingRight, float bufferThreshold)
		{
			switch (spriteIsFacingRight)
			{
				case false:
					if (projectile.position.X < targetPlayer.position.X)
					{
						if (Math.Abs(targetPlayer.position.X - projectile.position.X) < bufferThreshold)
						{
							projectile.spriteDirection = -1;
						}
						projectile.direction = 1;
						return;
					}
					else
					{
						if (Math.Abs(targetPlayer.position.X - projectile.position.X) < bufferThreshold)
						{
							projectile.spriteDirection = 1;
						}
						projectile.direction = -1;
					}
					break;
				case true:
				default:
					if (projectile.position.X < targetPlayer.position.X)
					{
						if (Math.Abs(targetPlayer.position.X - projectile.position.X) < bufferThreshold)
						{
							projectile.spriteDirection = 1;
						}
						projectile.direction = 1;
						return;
					}
					else
					{
						if (Math.Abs(targetPlayer.position.X - projectile.position.X) < bufferThreshold)
						{
							projectile.spriteDirection = -1;
						}
						projectile.direction = -1;
					}
					break;
			}
		}
		/// <summary>
		/// Animates a loop between the two specified frames at a specified speed, with an optional fourth parameter for number of non-looping transition frames (defaults to 0).
		/// </summary>
		public void SGProjectileAnimateBetweenFrames(int firstAnimationFrameIndex, int lastAnimationFrameIndex, int ticksBetweenFrames)
		{
			SGProjectileAnimateBetweenFrames(firstAnimationFrameIndex, lastAnimationFrameIndex, ticksBetweenFrames, 0);
		}

		/// <summary>
		/// Animates a loop between the two specified frames at a specified speed, with an optional fourth parameter for number of non-looping transition frames.
		/// </summary>
		public void SGProjectileAnimateBetweenFrames(int firstAnimationFrameIndex, int lastAnimationFrameIndex, int ticksBetweenFrames, int transitionFrameCount)
		{
			if (firstAnimationFrameIndex >= lastAnimationFrameIndex) //input validation
			{
				projectile.frame = firstAnimationFrameIndex;
				return;
			}

			projectile.frameCounter++;
			if (projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				projectile.frameCounter = 0;
			}
			if (projectile.frame < firstAnimationFrameIndex || projectile.frame > lastAnimationFrameIndex) //check bounds
			{
				projectile.frame = firstAnimationFrameIndex;
				return;
			}
			else if (projectile.frameCounter == ticksBetweenFrames) //increment frame
			{
				if (projectile.frame != lastAnimationFrameIndex)
				{
					projectile.frame += 1;
				}
				else
				{
					projectile.frame = firstAnimationFrameIndex + transitionFrameCount;
				}
			}
		}


		/// <summary>
		/// Moves the projectile towards a point using default parameters, accepts additional (float) parameters for maximum velocity, acceleration, deceleration, and stop distance.  
		/// <para>
		/// Default values are 0 (no limit) for max velocity, 2 for acceleration/deceleration, and 0 (stop-on-point) for stop distance.
		/// </para><para>
		/// Negative values may cause weird, unintended behavior.
		/// </para>
		/// </summary>
		public void SGProjectileMoveTowardsPoint(Vector2 targetPoint)
		{
			SGProjectileMoveTowardsPoint(targetPoint, 0f, 2f, 2f, 0f);
		}

		/// <summary>
		/// Moves the projectile towards a point, accepts additional (float) parameters for maximum velocity, acceleration, deceleration, and stop distance.  
		/// <para>
		/// Set other parameters to zero for unlimited velocity, instant acceleration/deceleration, or precise stop-on-point.  Setting all to zero will cause teleport movement.
		/// </para><para>
		/// Negative values may cause weird, unintended behavior.
		/// </para>
		/// </summary>
		public void SGProjectileMoveTowardsPoint(Vector2 targetPoint, float maxVelocity, float acceleration, float deceleration, float stopDistance)
		{
			float slopeRise = targetPoint.Y - projectile.position.Y;
			float slopeRun = targetPoint.X - projectile.position.X;
			float distanceToTargetX = (float)Math.Abs(slopeRun);
			float distanceToTargetY = (float)Math.Abs(slopeRise);
			float distanceToTargetAbsolute = (float)Math.Sqrt(Math.Pow(distanceToTargetX, 2f) + Math.Pow(distanceToTargetY, 2f)); //pythagoras, yay!
			if (distanceToTargetAbsolute > (stopDistance + 0.5f)) //handles movement and acceleration, small buffer for stopDistance
			{
				if (maxVelocity == 0f) //unlimited max velocity
				{
					if (acceleration == 0f) //if both maxVelocity and acceleration are 0, this indicates teleport movement
					{
						projectile.position.X = targetPoint.X;
						projectile.position.Y = targetPoint.Y;
						projectile.velocity.X = 0f;
						projectile.velocity.Y = 0f;
						return;
					}
					else
					{
						if (distanceToTargetY <= 0.2f)
						{
							projectile.velocity.Y = 0f;
						}
						else if (distanceToTargetY > 2f) //additional check to prevent odd behavior at distanceToTargetY ~= 0
						{
							projectile.velocity.Y += (acceleration * 0.002f * Math.Abs(slopeRise) * (projectile.position.Y >= targetPoint.Y ? -1 : 1));
						}
						if (distanceToTargetX <= 0.2f)
						{
							projectile.velocity.X = 0f;
						}
						else if (distanceToTargetX > 2f) //additional check to prevent odd behavior at distanceToTargetX ~= 0
						{
							projectile.velocity.X += (acceleration * 0.002f * Math.Abs(slopeRun) * projectile.direction);
						}
					}
				}
				else //limited max velocity
				{
					if (Math.Abs(projectile.velocity.X) > (maxVelocity + 0.02f)) //check for maxVelocity on X axis, slight buffer for floating point shenanigans
					{
						projectile.velocity.X = projectile.direction * maxVelocity; //this one's easy because of projectile.direction
					}
					else //not at maxVelocity on X axis
					{
						if (distanceToTargetX < 0.2f)
						{
							projectile.velocity.X = 0f;
						}
						else if (distanceToTargetX > 2f) //additional check to prevent odd behavior at distanceToTargetX ~= 0
						{
							projectile.velocity.X += (acceleration * 0.002f * Math.Abs(slopeRun) * projectile.direction);
						}
					}
					if (Math.Abs(projectile.velocity.Y) > (maxVelocity + 0.02f)) //check for maxVelocity on Y axis, slight buffer for floating point shenanigans
					{
						if (projectile.velocity.Y < 0)
						{
							projectile.velocity.Y = -1f * maxVelocity;
						}
						else
						{
							projectile.velocity.Y = maxVelocity;
						}
					}
					else //not at maxVelocity on Y axis
					{
						if (distanceToTargetY < 0.2f)
						{
							projectile.velocity.Y = 0f;
						}
						else if (distanceToTargetY > 2f) //additional check to prevent odd behavior at distanceToTargetY ~= 0
						{
							projectile.velocity.Y += (acceleration * 0.002f * Math.Abs(slopeRise) * (projectile.position.Y >= targetPoint.Y ? -1 : 1));
						}
					}
				}
			}
			else //decelerate and/or stop
			{
				if (stopDistance == 0f) //handles precise stop-at-target-point behavior
				{
					bool stoppedOnXAxis = false;
					if (slopeRun <= 0.2f)
					{
						projectile.position.X = targetPoint.X;
						projectile.velocity.X = 0f;
						stoppedOnXAxis = true;
					}
					if (slopeRise <= 0.2f)
					{
						projectile.position.Y = targetPoint.Y;
						projectile.velocity.Y = 0f;
						if (stoppedOnXAxis)
						{
							projectile.netUpdate = true;
							return;
						}
					}
				}
				else //handles all other types of stop behavior
				{
					if ((deceleration == 0f) || ((Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) <= 0.2f))
					{
						projectile.velocity.X = 0f;
						projectile.velocity.Y = 0f;
						projectile.netUpdate = true;
						return; //this is important to prevent a potential division by zero
					}
					projectile.velocity.X = projectile.velocity.X / (1f + deceleration);
					projectile.velocity.Y = projectile.velocity.Y / (1f + deceleration);
				}
			}
			projectile.netUpdate = true;
		}
	}

}
