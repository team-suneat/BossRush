using UnityEngine;

namespace TeamSuneat
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class XRigidbody : XBehaviour
	{
		public Rigidbody2D Body;

		public RigidbodyType2D Type => Body.bodyType;

		public override void AutoGetComponents()
		{
			base.AutoGetComponents();

			Body = GetComponent<Rigidbody2D>();
		}

		protected virtual void Awake()
		{
			if (Body == null)
			{
				Body = GetComponent<Rigidbody2D>();
			}
		}

		public void SetBodyType(RigidbodyType2D bodyType)
		{
			if (Body != null)
			{
				Body.bodyType = bodyType;
			}
		}

		public void SetGravity(float gravityScale)
		{
			if (Body != null)
			{
				Body.gravityScale = gravityScale;
			}
		}

		public void ResetGravity()
		{
			if (Body != null)
			{
				Body.gravityScale = 0;
			}
		}

		public void SetVelocity(Vector2 velocity)
		{
			if (Body != null)
			{
				Body.linearVelocity= velocity;
			}
		}

		public void ResetVelocity()
		{
			if (Body != null)
			{
				Body.linearVelocity = Vector2.zero;
			}
		}

		public void SetAngularVelocity(float angularVelocity)
		{
			if (Body != null)
			{
				Body.angularVelocity = angularVelocity;
			}
		}

		public void SetFreezeRotation(bool isFreeze)
		{
			if (Body != null)
			{
				Body.freezeRotation = isFreeze;
			}
		}

		public void AddForce(Vector2 force)
		{
			if (Body != null)
			{
				Body.AddForce(force);
			}
		}
	}
}