using UnityEngine;

namespace TeamSuneat
{
	[RequireComponent(typeof(CollisionController))]
	[RequireComponent(typeof(Rigidbody2D))]
	public partial class PhysicsController : XBehaviour
	{
		public void SetIgnorePlatform(bool isIgnorePlatform)
		{
			if (Controller != null)
			{
				Controller.SetIgnorePlatform(isIgnorePlatform);
			}
		}

		public void ResetIgnorePlatform()
		{
			if (Controller != null)
			{
				Controller.ResetIgnorePlatform();
			}
		}
	}
}