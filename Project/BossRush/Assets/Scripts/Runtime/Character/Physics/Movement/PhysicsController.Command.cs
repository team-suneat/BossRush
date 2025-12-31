using UnityEngine;

namespace TeamSuneat
{
	[RequireComponent(typeof(CollisionController))]
	[RequireComponent(typeof(Rigidbody2D))]
	public partial class PhysicsController : XBehaviour
	{
		public bool SetDirectionalInput(float x, float y)
		{
			if (false == m_directionalInput.Equals(new Vector2(x, y)))
			{
				m_directionalInput.Set(x, y);
				return true;
			}

			return false;
		}

		public virtual void OnAttack(bool facingRight)
		{
		}

		public void OnJumpInputDown()
		{
			float jumpVelocity = customJumpVelocity == 0 ? maxJumpVelocity : customJumpVelocity;

			if (false == Controller.IsGrounded)
			{
				Velocity.y = jumpVelocity;

				return;
			}

			if (m_directionalInput.y < -0.5f)
			{
				if (Controller.collisions.standingThroughPlatform)
				{
					Controller.collisions.actionFalling = true;
				}
				else
				{
					Velocity.y = jumpVelocity;
				}
			}
			else
			{
				if (Controller.collisions.slidingDownMaxSlope)
				{
					float slopeNormalX = -Mathf.Sign(Controller.collisions.slopeNormal.x);
					if (slopeNormalX != System.Math.Sign(m_directionalInput.x))
					{
						// not jumping against max slope
						Velocity = new Vector3(jumpVelocity * Controller.collisions.slopeNormal.x, jumpVelocity * Controller.collisions.slopeNormal.y);
					}
				}
				else
				{
					Velocity.y = jumpVelocity;
				}
			}
		}

		public void OnJumpInputUp()
		{
			if (Velocity.y > minJumpVelocity)
			{
				Velocity.y = minJumpVelocity;
			}
		}

		public void OnJumpInputDownDuringFV()
		{
			float jumpVelocity = customJumpVelocity == 0 ? maxJumpVelocity : customJumpVelocity;

			if (false == Controller.IsGrounded)
			{
				FVMover.SetVelocityY(jumpVelocity);

				return;
			}

			if (Controller.collisions.slidingDownMaxSlope)
			{
				float slopeNormalX = -Mathf.Sign(Controller.collisions.slopeNormal.x);

				if (slopeNormalX != System.Math.Sign(m_directionalInput.x))
				{
					// not jumping against max slope
					FVMover.SetVelocityX(jumpVelocity * Controller.collisions.slopeNormal.x);
					FVMover.SetVelocityY(jumpVelocity * Controller.collisions.slopeNormal.y);
				}
			}
			else
			{
				FVMover.SetVelocityY(jumpVelocity);
			}
		}
	}
}