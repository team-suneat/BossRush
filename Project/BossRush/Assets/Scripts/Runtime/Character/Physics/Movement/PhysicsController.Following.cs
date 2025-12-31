using System;
using TeamSuneat.Data;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
	[RequireComponent(typeof(CollisionController))]
	[RequireComponent(typeof(Rigidbody2D))]
	public partial class PhysicsController : XBehaviour
	{
		public void ResetTargetToFollow()
		{
			if (UseFollowing || UseOnceFollowing)
			{
				Log.Info(LogTags.Physics, "Reset Target To Follow");

				FollowPosition = Vector3.zero;
			}
		}

		public void SetTargetToFollow(Vector3 targetPosition)
		{
			if (UseFollowing || UseOnceFollowing)
			{
				Log.Info(LogTags.Physics, "Set Target To Follow. targetPosition : {0}", targetPosition);

				FollowPosition = targetPosition;
			}
			else
			{
				Log.Warning(LogTags.Physics, " Failed To Set Target To Follow. targetPosition : {0}", targetPosition);
			}
		}

		public void SetTargetToFollow(Transform target)
		{
			if (UseFollowing || UseOnceFollowing)
			{
				Log.Info(LogTags.Physics, "Set Target To Follow. target : {0}", target.GetHierarchyPath());
				Following = target;
			}
			else
			{
				Log.Warning(LogTags.Physics, " Failed To Set Target To Follow. target : {0}", target.GetHierarchyPath());
			}
		}

		public void SetFollowCompletedEvent(UnityAction callback)
		{
			if (UseOnceFollowing)
			{
				Log.Info(LogTags.Physics, "Set Follow Completed Event");

				OnceFollowCallback = callback;
			}
			else
			{
				Log.Warning(LogTags.Physics, " Failed To Set Follow Completed Event. Not use once follow.");
			}
		}

		public void CallOnceFollowingCompltedEvent()
		{
			if (UseOnceFollowing)
			{
				if (Following != null)
				{
					position = new Vector3(Following.position.x, position.y);
				}

				UseOnceFollowing = false;

				Following = null;

				FollowPosition = Vector3.zero;

				if (OnceFollowCallback != null)
				{
					OnceFollowCallback();
				}
			}
		}

		private void CalculateFollowVelocityInGround(Vector3 targetPosition)
		{
			if (false == Controller.IsGrounded)
			{
				// 지상 유닛은 땅에서만 이동한다.
				return;
			}

			// 지상
			m_smoothTime = AccelerationTimeGrounded;

			Vector2 target = new Vector2(targetPosition.x, 0);

			Vector2 origin = new Vector2(transform.position.x, 0);

			m_distanceBetweenFollowing = Vector2.Distance(origin, target);

			if (m_distanceBetweenFollowing > MinDistanceToOwner)
			{
				Vector2 directional = VectorEx.Normalize(target - origin);

				int directionalXToInt = System.Math.Sign(directional.x);

				float targetVelocityX = directionalXToInt * MoveSpeed;

				Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref m_velocityXSmoothing, m_smoothTime);
			}
			else
			{
				CallOnceFollowingCompltedEvent();

				Velocity.x = 0;
			}

			Velocity.y = Mathf.Clamp(gravity * Time.fixedDeltaTime, -MaxVelocityY, MaxVelocityY);
		}

		private void CalculateFollowVelocityInAir(Vector3 targetPosition)
		{
			// 공중
			m_smoothTime = AccelerationTimeAirborne;

			m_distanceBetweenFollowing = Vector2.Distance(targetPosition, transform.position);

			DebugEx.DrawLine(transform.position, targetPosition, Color.yellow);

			DebugEx.DrawCross(targetPosition, Color.yellow);

			if (m_distanceBetweenFollowing > MinDistanceToOwner)
			{
				// 일정거리 멀어졌을 때 따라간다.

				Vector3 direction = VectorEx.Normalize(targetPosition - transform.position);

				float acceleration = JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_ACCELERATION_FOLLOWING);

				float targetVelocityX = 0;

				if (Mathf.Abs(direction.x) > 0.1f)
				{
					targetVelocityX = direction.x * MoveSpeed;
				}

				float targetVelocityY = 0;

				if (Mathf.Abs(direction.y) > 0.1f)
				{
					targetVelocityY = direction.y * MoveSpeed;
				}

				if (acceleration > 0)
				{
					targetVelocityX *= acceleration;

					targetVelocityY *= acceleration;
				}

				Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref m_velocityXSmoothing, m_smoothTime);

				Velocity.y = Mathf.SmoothDamp(Velocity.y, targetVelocityY, ref m_velocityYSmoothing, m_smoothTime);

				if (m_distanceBetweenFollowing < Math.Abs(Velocity.y))
				{
					if (Velocity.y > 1)
					{
						Velocity.y = m_distanceBetweenFollowing;
					}
					else if (Velocity.y < 1)
					{
						Velocity.y = -m_distanceBetweenFollowing;
					}
				}
				else
				{
					Velocity.y = Mathf.Clamp(Velocity.y, -MaxVelocityY, MaxVelocityY);
				}
			}
			else
			{
				CallOnceFollowingCompltedEvent();

				Velocity = Vector3.zero;
			}
		}
	}
}