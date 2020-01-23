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
		/// Flips projectile (and sprite) to face a target player
		/// </summary>
		public void SGProjectileFacePlayer(Player targetPlayer, bool spriteIsFacingRight = true, float bufferThreshold = 0f)
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
		/// Animates a loop between the two specified frames at a specified speed, with optional parameter for number of non-looping transition frames.
		/// <para>
		public void SGProjectileAnimateBetweenFrames(int firstAnimationFrameIndex, int lastAnimationFrameIndex, int ticksBetweenFrames, int transitionFrameCount = 0)
		{
			if (firstAnimationFrameIndex >= lastAnimationFrameIndex) //input validation
			{
				projectile.frame = firstAnimationFrameIndex;
				return;
			}

			projectile.frameCounter++;

			if (projectile.frame < firstAnimationFrameIndex || projectile.frame > lastAnimationFrameIndex) //check bounds
			{
				projectile.frame = firstAnimationFrameIndex;
				return;
			}

			if (projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				projectile.frameCounter = 0;
				return;
			}

			if (projectile.frameCounter == ticksBetweenFrames) //increment frame
			{
				if (projectile.frame != lastAnimationFrameIndex)
				{
						projectile.frame += 1;
				}
				else if (transitionFrameCount >= (lastAnimationFrameIndex - firstAnimationFrameIndex))
				{
						return; //end looping animation
				}
				else
				{
						projectile.frame = firstAnimationFrameIndex + transitionFrameCount;
				}
			}
		}
		/// <summary>
		/// Animates a loop between the two specified frames at a specified speed, with optional parameters for number of non-looping transition frames and back-and-forth style.
		/// <para>
		/// Back-and-forth mode requires that an additional flag be passed by reference as the backAndForthVariable argument.</para>
		/// </summary>
		public void SGProjectileAnimateBetweenFrames(int firstAnimationFrameIndex, int lastAnimationFrameIndex, ref bool backAndForthVariable, int ticksBetweenFrames, int transitionFrameCount = 0, bool backAndForthMode = true)
		{
			if (firstAnimationFrameIndex >= lastAnimationFrameIndex) //input validation
			{
				projectile.frame = firstAnimationFrameIndex;
				return;
			}

			projectile.frameCounter++;

			if (projectile.frame < firstAnimationFrameIndex || projectile.frame > lastAnimationFrameIndex) //check bounds
			{
				projectile.frame = firstAnimationFrameIndex;
				return;
			}

			if (projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				projectile.frameCounter = 0;
				return;
			}

			if (!backAndForthMode) //standard looping animation
			{
				if (projectile.frameCounter == ticksBetweenFrames) //increment frame
				{
					if (projectile.frame != lastAnimationFrameIndex)
					{
						projectile.frame += 1;
					}
					else if (transitionFrameCount >= (lastAnimationFrameIndex - firstAnimationFrameIndex))
					{
						return; //end looping animation
					}
					else
					{
						projectile.frame = firstAnimationFrameIndex + transitionFrameCount;
					}
				}
				return;
			}
			else //back-and-forth looping animation
			{
				if (projectile.frameCounter == ticksBetweenFrames)
				{
					if (projectile.frame == firstAnimationFrameIndex + transitionFrameCount)
					{
							backAndForthVariable = false; //set to forward mode
							projectile.frame += 1;
						return;
					}
					else if (projectile.frame != lastAnimationFrameIndex)
					{
						if (backAndForthVariable == true) //reverse mode
						{
							projectile.frame -= 1;
							return;
						}
						else //forward mode
						{
							projectile.frame += 1;
							return;
						}
					}
					else if (transitionFrameCount >= (lastAnimationFrameIndex - firstAnimationFrameIndex))
					{
						return; //end looping animation
					}
					else
					{
						backAndForthVariable = true; //set to reverse mode
						projectile.frame -= 1;
					}
				}


			}
		}

		/// <summary>
		/// Animates a loop from a one-dimensional integer array of frame numbers.  Accepts an additional argument for transition frames.
		///<para></para>
		/// This overload does not require a reference integer, but isn't as efficient.  Use it sparingly, with a small frameArray.
		/// </summary>
		public void SGProjectileAnimateFromArray(int[] frameArray, int ticksBetweenFrames, int transitionFrameCount = 0)
		{
			int currentFrameIndex = frameArray[0];
			if (frameArray.Length < 2) //input validation
			{
				projectile.frame = frameArray[0];
				return;
			}

			projectile.frameCounter++;
			if (projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				projectile.frameCounter = 0;
			}

			for (int i = 0; i < frameArray.Length; i++) //find current frame in array, will default to last frame if not found
			{
				if (projectile.frame == frameArray[i] || i == frameArray.Length)
				{
					currentFrameIndex = frameArray[i];
					break;
				}
			}

			if (projectile.frameCounter == ticksBetweenFrames) //increment frame
			{
				if (projectile.frame != frameArray[frameArray.Length - 1])
				{
					projectile.frame = frameArray[currentFrameIndex + 1];
				}
				else if (transitionFrameCount >= frameArray.Length)
				{
					return; //end looping animation
				}
				else
				{
					projectile.frame = frameArray[transitionFrameCount];
				}
			}
		}

		/// <summary>
		/// Animates a loop from a one-dimensional integer array of frame numbers.  Accepts an additional argument for transition frames.
		/// </summary>
		public void SGProjectileAnimateFromArray(int[] frameArray, int ticksBetweenFrames, ref int currentFrameIndex, int transitionFrameCount = 0)
		{
			if (frameArray.Length < 2) //input validation
			{
				projectile.frame = frameArray[0];
				currentFrameIndex = 0;
				return;
			}

			projectile.frameCounter++;
			if (projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				projectile.frameCounter = 0;
			}


			if (projectile.frameCounter == ticksBetweenFrames) //increment frame
			{
				if (projectile.frame != frameArray[frameArray.Length - 1])
				{
					projectile.frame = frameArray[currentFrameIndex + 1];
					currentFrameIndex += 1;
				}
				else if (transitionFrameCount >= frameArray.Length)
				{
					return; //end looping animation
				}
				else
				{
					projectile.frame = frameArray[transitionFrameCount];
					currentFrameIndex = 0;
				}
			}
		}

		/// <summary>
		/// Animates a loop from a one-dimensional integer array of frame numbers.  Accepts additional arguments for transition frames and "back-and-forth" style animation loops.
		/// <para>
		/// Back-and-forth mode requires that an additional flag be passed as the backAndForthVariable argument, or else it will not function properly.</para>
		/// </summary>
		public void SGProjectileAnimateFromArray(int[] frameArray, ref bool backAndForthVariable, int ticksBetweenFrames, int transitionFrameCount = 0, bool backAndForthMode = true)
		{
			int currentFrameIndex = frameArray[0];
			if (frameArray.Length < 2) //input validation
			{
				projectile.frame = frameArray[0];
				return;
			}

			projectile.frameCounter++;
			if (projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				projectile.frameCounter = 0;
			}

			for (int i=0; i < frameArray.Length; i++) //find current frame in array, will default to last frame if not found
			{
				if (projectile.frame == frameArray[i] || i == frameArray.Length)
				{
					currentFrameIndex = frameArray[i];
					break;
				}
			}

			if (!backAndForthMode) //standard looping animation
			{
				if (projectile.frameCounter == ticksBetweenFrames) //increment frame
				{
					if (projectile.frame != frameArray[frameArray.Length - 1])
					{
						projectile.frame = frameArray[currentFrameIndex + 1];
					}
					else if (transitionFrameCount >= frameArray.Length)
					{
						return; //end looping animation
					}
					else
					{
						projectile.frame = frameArray[transitionFrameCount];
					}
				}
				return;
			}
			else //back-and-forth looping animation
			{
				if (projectile.frameCounter == ticksBetweenFrames)
				{
					if (projectile.frame != frameArray[frameArray.Length - 1])
					{
						if (backAndForthVariable == true) //reverse mode
						{
							projectile.frame = frameArray[currentFrameIndex - 1];
							return;
						}
						else //forward mode
						{
							projectile.frame = frameArray[currentFrameIndex + 1];
							return;
						}
					}
					else if (transitionFrameCount >= frameArray.Length)
					{
						return; //end looping animation
					}
					else if (projectile.frame == frameArray[transitionFrameCount])
					{
						backAndForthVariable = false; //set to forward mode
						projectile.frame = frameArray[currentFrameIndex + 1];
						return;
					}
					else
					{
						backAndForthVariable = true;
						projectile.frame = frameArray[currentFrameIndex - 1];
					}
				}


			}


		}


		/// <summary>
		/// Moves the projectile towards a point, accepts additional (float) parameters for maximum velocity, acceleration, deceleration, and stop distance.  
		/// <para>
		/// Set other parameters to zero for unlimited velocity, instant acceleration/deceleration, or precise stop-on-point.  Setting all to zero will cause teleport movement.
		/// </para><para>
		/// Negative values may cause weird, unintended behavior.
		/// </para>
		/// </summary>
		public void SGProjectileMoveTowardsPoint(Vector2 targetPoint, float maxVelocity = 0, float acceleration = 1f, float deceleration = 3f, float stopDistance = 0f)
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
