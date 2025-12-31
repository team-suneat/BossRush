using UnityEngine;

namespace TeamSuneat
{
	[RequireComponent(typeof(CollisionController))]
	[RequireComponent(typeof(Rigidbody2D))]
	public partial class PhysicsController : XBehaviour
	{
		public override void AutoSetting()
		{
			base.AutoSetting();

			if (Rigidbody != null)
			{
				Rigidbody.bodyType = RigidbodyType2D.Kinematic;
				Rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			}
		}

		public override void AutoGetComponents()
		{
			base.AutoGetComponents();

			Owner ??= GetComponent<Character>();
			Rigidbody ??= GetComponent<Rigidbody2D>();
			Controller ??= GetComponent<CollisionController>();
			LandingPoint ??= this.FindComponent<Transform>("Points/LandingPoint");
		}

		protected virtual void Awake()
		{
			Owner ??= GetComponent<Character>();
			Rigidbody ??= GetComponent<Rigidbody2D>();
			Controller ??= GetComponent<CollisionController>();
			LandingPoint ??= this.FindComponent<Transform>("Points/LandingPoint");
		}
	}
}