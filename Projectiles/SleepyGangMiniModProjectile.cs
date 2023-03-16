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
					if (Projectile.position.X < targetPlayer.position.X)
					{
						if (Math.Abs(targetPlayer.position.X - Projectile.position.X) < bufferThreshold)
						{
							Projectile.spriteDirection = -1;
						}
						Projectile.direction = 1;
						return;
					}
					else
					{
						if (Math.Abs(targetPlayer.position.X - Projectile.position.X) < bufferThreshold)
						{
							Projectile.spriteDirection = 1;
						}
						Projectile.direction = -1;
					}
					break;
				case true:
				default:
					if (Projectile.position.X < targetPlayer.position.X)
					{
						if (Math.Abs(targetPlayer.position.X - Projectile.position.X) < bufferThreshold)
						{
							Projectile.spriteDirection = 1;
						}
						Projectile.direction = 1;
						return;
					}
					else
					{
						if (Math.Abs(targetPlayer.position.X - Projectile.position.X) < bufferThreshold)
						{
							Projectile.spriteDirection = -1;
						}
						Projectile.direction = -1;
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
				Projectile.frame = firstAnimationFrameIndex;
				return;
			}

			Projectile.frameCounter++;

			if (Projectile.frame < firstAnimationFrameIndex || Projectile.frame > lastAnimationFrameIndex) //check bounds
			{
				Projectile.frame = firstAnimationFrameIndex;
				return;
			}

			if (Projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				Projectile.frameCounter = 0;
				return;
			}

			if (Projectile.frameCounter == ticksBetweenFrames) //increment frame
			{
				if (Projectile.frame != lastAnimationFrameIndex)
				{
						Projectile.frame += 1;
				}
				else if (transitionFrameCount >= (lastAnimationFrameIndex - firstAnimationFrameIndex))
				{
						return; //end looping animation
				}
				else
				{
						Projectile.frame = firstAnimationFrameIndex + transitionFrameCount;
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
				Projectile.frame = firstAnimationFrameIndex;
				return;
			}

			Projectile.frameCounter++;

			if (Projectile.frame < firstAnimationFrameIndex || Projectile.frame > lastAnimationFrameIndex) //check bounds
			{
				Projectile.frame = firstAnimationFrameIndex;
				return;
			}

			if (Projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				Projectile.frameCounter = 0;
				return;
			}

			if (!backAndForthMode) //standard looping animation
			{
				if (Projectile.frameCounter == ticksBetweenFrames) //increment frame
				{
					if (Projectile.frame != lastAnimationFrameIndex)
					{
						Projectile.frame += 1;
					}
					else if (transitionFrameCount >= (lastAnimationFrameIndex - firstAnimationFrameIndex))
					{
						return; //end looping animation
					}
					else
					{
						Projectile.frame = firstAnimationFrameIndex + transitionFrameCount;
					}
				}
				return;
			}
			else //back-and-forth looping animation
			{
				if (Projectile.frameCounter == ticksBetweenFrames)
				{
					if (Projectile.frame == firstAnimationFrameIndex + transitionFrameCount)
					{
							backAndForthVariable = false; //set to forward mode
							Projectile.frame += 1;
						return;
					}
					else if (Projectile.frame != lastAnimationFrameIndex)
					{
						if (backAndForthVariable == true) //reverse mode
						{
							Projectile.frame -= 1;
							return;
						}
						else //forward mode
						{
							Projectile.frame += 1;
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
						Projectile.frame -= 1;
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
				Projectile.frame = frameArray[0];
				return;
			}

			Projectile.frameCounter++;
			if (Projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				Projectile.frameCounter = 0;
			}

			for (int i = 0; i < frameArray.Length; i++) //find current frame in array, will default to last frame if not found
			{
				if (Projectile.frame == frameArray[i] || i == frameArray.Length)
				{
					currentFrameIndex = frameArray[i];
					break;
				}
			}

			if (Projectile.frameCounter == ticksBetweenFrames) //increment frame
			{
				if (Projectile.frame != frameArray[frameArray.Length - 1])
				{
					Projectile.frame = frameArray[currentFrameIndex + 1];
				}
				else if (transitionFrameCount >= frameArray.Length)
				{
					return; //end looping animation
				}
				else
				{
					Projectile.frame = frameArray[transitionFrameCount];
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
				Projectile.frame = frameArray[0];
				currentFrameIndex = 0;
				return;
			}

			Projectile.frameCounter++;
			if (Projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				Projectile.frameCounter = 0;
			}


			if (Projectile.frameCounter == ticksBetweenFrames) //increment frame
			{
				if (Projectile.frame != frameArray[frameArray.Length - 1])
				{
					Projectile.frame = frameArray[currentFrameIndex + 1];
					currentFrameIndex += 1;
				}
				else if (transitionFrameCount >= frameArray.Length)
				{
					return; //end looping animation
				}
				else
				{
					Projectile.frame = frameArray[transitionFrameCount];
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
				Projectile.frame = frameArray[0];
				return;
			}

			Projectile.frameCounter++;
			if (Projectile.frameCounter > ticksBetweenFrames) //reset counter
			{
				Projectile.frameCounter = 0;
			}

			for (int i=0; i < frameArray.Length; i++) //find current frame in array, will default to last frame if not found
			{
				if (Projectile.frame == frameArray[i] || i == frameArray.Length)
				{
					currentFrameIndex = frameArray[i];
					break;
				}
			}

			if (!backAndForthMode) //standard looping animation
			{
				if (Projectile.frameCounter == ticksBetweenFrames) //increment frame
				{
					if (Projectile.frame != frameArray[frameArray.Length - 1])
					{
						Projectile.frame = frameArray[currentFrameIndex + 1];
					}
					else if (transitionFrameCount >= frameArray.Length)
					{
						return; //end looping animation
					}
					else
					{
						Projectile.frame = frameArray[transitionFrameCount];
					}
				}
				return;
			}
			else //back-and-forth looping animation
			{
				if (Projectile.frameCounter == ticksBetweenFrames)
				{
					if (Projectile.frame != frameArray[frameArray.Length - 1])
					{
						if (backAndForthVariable == true) //reverse mode
						{
							Projectile.frame = frameArray[currentFrameIndex - 1];
							return;
						}
						else //forward mode
						{
							Projectile.frame = frameArray[currentFrameIndex + 1];
							return;
						}
					}
					else if (transitionFrameCount >= frameArray.Length)
					{
						return; //end looping animation
					}
					else if (Projectile.frame == frameArray[transitionFrameCount])
					{
						backAndForthVariable = false; //set to forward mode
						Projectile.frame = frameArray[currentFrameIndex + 1];
						return;
					}
					else
					{
						backAndForthVariable = true;
						Projectile.frame = frameArray[currentFrameIndex - 1];
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
			float slopeRise = targetPoint.Y - Projectile.position.Y;
			float slopeRun = targetPoint.X - Projectile.position.X;
			float distanceToTargetX = (float)Math.Abs(slopeRun);
			float distanceToTargetY = (float)Math.Abs(slopeRise);
			float distanceToTargetAbsolute = (float)Math.Sqrt(Math.Pow(distanceToTargetX, 2f) + Math.Pow(distanceToTargetY, 2f)); //pythagoras, yay!
			if (distanceToTargetAbsolute > (stopDistance + 0.5f)) //handles movement and acceleration, small buffer for stopDistance
			{
				if (maxVelocity == 0f) //unlimited max velocity
				{
					if (acceleration == 0f) //if both maxVelocity and acceleration are 0, this indicates teleport movement
					{
						Projectile.position.X = targetPoint.X;
						Projectile.position.Y = targetPoint.Y;
						Projectile.velocity.X = 0f;
						Projectile.velocity.Y = 0f;
						return;
					}
					else
					{
						if (distanceToTargetY <= 0.2f)
						{
							Projectile.velocity.Y = 0f;
						}
						else if (distanceToTargetY > 2f) //additional check to prevent odd behavior at distanceToTargetY ~= 0
						{
							Projectile.velocity.Y += (acceleration * 0.002f * Math.Abs(slopeRise) * (Projectile.position.Y >= targetPoint.Y ? -1 : 1));
						}
						if (distanceToTargetX <= 0.2f)
						{
							Projectile.velocity.X = 0f;
						}
						else if (distanceToTargetX > 2f) //additional check to prevent odd behavior at distanceToTargetX ~= 0
						{
							Projectile.velocity.X += (acceleration * 0.002f * Math.Abs(slopeRun) * Projectile.direction);
						}
					}
				}
				else //limited max velocity
				{
					if (Math.Abs(Projectile.velocity.X) > (maxVelocity + 0.02f)) //check for maxVelocity on X axis, slight buffer for floating point shenanigans
					{
						Projectile.velocity.X = Projectile.direction * maxVelocity; //this one's easy because of projectile.direction
					}
					else //not at maxVelocity on X axis
					{
						if (distanceToTargetX < 0.2f)
						{
							Projectile.velocity.X = 0f;
						}
						else if (distanceToTargetX > 2f) //additional check to prevent odd behavior at distanceToTargetX ~= 0
						{
							Projectile.velocity.X += (acceleration * 0.002f * Math.Abs(slopeRun) * Projectile.direction);
						}
					}
					if (Math.Abs(Projectile.velocity.Y) > (maxVelocity + 0.02f)) //check for maxVelocity on Y axis, slight buffer for floating point shenanigans
					{
						if (Projectile.velocity.Y < 0)
						{
							Projectile.velocity.Y = -1f * maxVelocity;
						}
						else
						{
							Projectile.velocity.Y = maxVelocity;
						}
					}
					else //not at maxVelocity on Y axis
					{
						if (distanceToTargetY < 0.2f)
						{
							Projectile.velocity.Y = 0f;
						}
						else if (distanceToTargetY > 2f) //additional check to prevent odd behavior at distanceToTargetY ~= 0
						{
							Projectile.velocity.Y += (acceleration * 0.002f * Math.Abs(slopeRise) * (Projectile.position.Y >= targetPoint.Y ? -1 : 1));
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
						Projectile.position.X = targetPoint.X;
						Projectile.velocity.X = 0f;
						stoppedOnXAxis = true;
					}
					if (slopeRise <= 0.2f)
					{
						Projectile.position.Y = targetPoint.Y;
						Projectile.velocity.Y = 0f;
						if (stoppedOnXAxis)
						{
							Projectile.netUpdate = true;
							return;
						}
					}
				}
				else //handles all other types of stop behavior
				{
					if ((deceleration == 0f) || ((Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) <= 0.2f))
					{
						Projectile.velocity.X = 0f;
						Projectile.velocity.Y = 0f;
						Projectile.netUpdate = true;
						return; //this is important to prevent a potential division by zero
					}
					Projectile.velocity.X = Projectile.velocity.X / (1f + deceleration);
					Projectile.velocity.Y = Projectile.velocity.Y / (1f + deceleration);
				}
			}
			Projectile.netUpdate = true;
		}
	}

}
