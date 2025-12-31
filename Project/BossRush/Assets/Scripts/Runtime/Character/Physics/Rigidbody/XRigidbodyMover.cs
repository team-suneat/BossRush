using UnityEngine;

namespace TeamSuneat
{
	public class XRigidbodyMover : XBehaviour
	{
		public Rigidbody2D Body;

		public Vector2 direction;

		public float speed;

		public override void AutoGetComponents()
		{
			base.AutoGetComponents();

			Body = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			if (Body != null)
			{
				Body.MovePosition(Body.position + direction * speed * Time.fixedDeltaTime);
			}
		}
	}
}