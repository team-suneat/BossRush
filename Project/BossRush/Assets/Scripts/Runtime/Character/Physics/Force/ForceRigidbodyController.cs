using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class ForceRigidbodyController : XBehaviour
	{
		public Collider2D Collider2D;

		public Rigidbody2D RigidBody;

		public bool UseAddForceOnEnabled;

		public bool RandomVelocity;

		[HideIf("RandomVelocity")]
		public Vector2 FixedVelocity;

		[ShowIf("RandomVelocity")]
		public Vector2 MinVelcity;

		[ShowIf("RandomVelocity")]
		public Vector2 MaxVelcity;

		public float ForcePower;

		public override void AutoGetComponents()
		{
			base.AutoGetComponents();

			Collider2D = GetComponent<Collider2D>();

			RigidBody = GetComponent<Rigidbody2D>();
		}

		protected virtual void Awake()
		{
			if (Collider2D == null)
			{
				Collider2D = GetComponent<Collider2D>();
			}

			if (RigidBody == null)
			{
				RigidBody = GetComponent<Rigidbody2D>();
			}
		}

		protected override void OnStart()
		{
			base.OnStart();

			if (UseAddForceOnEnabled)
			{
				AddForce();
			}
		}

		protected override void OnEnabled()
		{
			base.OnEnabled();

			if (UseAddForceOnEnabled)
			{
				AddForce();
			}
		}

		public void AddForce()
		{
			if (Collider2D != null)
			{
				Collider2D.isTrigger = false;
			}

			if (RigidBody != null)
			{
				RigidBody.gravityScale = GameDefine.DEFAULT_PHYSICS_DEFAULT_GRAVITY_IN_RIGIDBODY;
				RigidBody.simulated = true;
				RigidBody.linearVelocity = Vector2.zero;

				Vector3 velocity = Vector3.zero;

				if (RandomVelocity)
				{
					float velocityX = RandomEx.Range(MinVelcity.x, MaxVelcity.x);
					float velocityY = RandomEx.Range(MinVelcity.y, MaxVelcity.y);
					velocity = new Vector3(velocityX, velocityY, 0);
				}
				else
				{
					velocity = FixedVelocity;
				}

				RigidBody.AddForce(velocity * ForcePower);
			}
		}
	}
}