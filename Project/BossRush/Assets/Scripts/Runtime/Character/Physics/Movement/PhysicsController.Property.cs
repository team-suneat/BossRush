using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
	[RequireComponent(typeof(CollisionController))]
	[RequireComponent(typeof(Rigidbody2D))]
	public partial class PhysicsController : XBehaviour
	{
		public bool FlyingOwner
		{
			get
			{
				if (Owner == null)
				{
					return false;
				}

				if (Owner.DataCharacter == null)
				{
					return false;
				}

				return Owner.IsFlying;
			}
		}

		public bool IsInputDirection
		{
			get
			{
				if (FVMover != null)
				{
					return FVMover.Data.Direction == FVDirections.DirectionalInput || FVMover.Data.Direction == FVDirections.DirectionalX;
				}

				return false;
			}
		}

		public Vector2 ForceVelocity
		{
			get
			{
				if (FVMover != null)
				{
					return FVMover.Velocity;
				}

				return Vector2.zero;
			}
		}

		public float AccelerationTimeAirborne => JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_ACCELERATION_TIME_AIRBORNE);

		public float AccelerationTimeGrounded => JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_ACCELERATION_TIME_GROUNDED);

		public float WallSlideSpeedMax => JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_WALL_SLIDE_MAX_SPEED);

		public float WallStickTime => JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_WALL_STICK_TIME);

		private float m_maxVelocityY;

		public float MaxVelocityY
		{
			get
			{
				if (Mathf.Abs(m_maxVelocityY) <= 0f)
				{
					m_maxVelocityY = JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_PHYSICS_MAX_VELOCITY_Y);
				}

				return m_maxVelocityY;
			}
		}

		public float MoveSpeed
		{
			get
			{
				MoveSpeedResult = DefaultMoveSpeed * MoveSpeedMultiplier;

				return DefaultMoveSpeed * MoveSpeedMultiplier;
			}
		}
	}
}