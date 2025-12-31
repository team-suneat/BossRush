using UnityEngine;

namespace TeamSuneat
{
	/// <summary>
	/// 낙하 이후 충돌을 비활성화
	/// </summary>

	public class CollisionDescent : XBehaviour
	{
		public Rigidbody2D RigidBody;

		public Collider2D Collider2d;

		public LayerMask CollisionLayer;

#if UNITY_EDITOR

		public override void AutoGetComponents()
		{
			base.AutoGetComponents();

			RigidBody = GetComponent<Rigidbody2D>();

			Collider2d = GetComponent<Collider2D>();
		}

		public override void AutoSetting()
		{
			base.AutoSetting();

			CollisionLayer = GameLayers.Mask.Collision;
		}

#endif

		public void OnCollisionEnter2D(Collision2D collision)
		{
			if (false == LayerEx.IsInMask(collision.gameObject.layer, CollisionLayer))
			{
				return;
			}

			if (RigidBody != null)
			{
				RigidBody.bodyType = RigidbodyType2D.Kinematic;
				RigidBody.linearVelocity = Vector2.zero;
			}

			if (Collider2d != null)
			{
				Collider2d.isTrigger = true;
			}

			gameObject.layer = GameLayers.Default;
		}
	}
}