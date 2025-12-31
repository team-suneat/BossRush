using Sirenix.OdinInspector;
using UnityEngine;

public class PhysicsTester : MonoBehaviour
{
	public enum MoveTypes
	{
		None,

		Move,

		SmoothDamp,

		Lerp,

		Slerp,
	}

	[Title("RigidBody")]
	public Rigidbody2D rigidBody2D;

	[Title("Vector")]
	public Vector3 end = new Vector3(3, 0);

	public MoveTypes moveType;

	public float maxDistanceDelta = 2f;

	public float smoothTime = 0.05f;

	public float lerpTime = 0.05f;

	public float slerpTime = 0.05f;

	private void FixedUpdate()
	{
		switch (moveType)
		{
			case MoveTypes.Move:
				{
					transform.position = Vector3.MoveTowards(transform.position, end, maxDistanceDelta);
				}
				break;

			case MoveTypes.SmoothDamp:
				{
					Vector3 currentVelocity = Vector3.zero;

					transform.position = Vector3.SmoothDamp(transform.position, end, ref currentVelocity, smoothTime);
				}
				break;

			case MoveTypes.Lerp:
				{
					transform.position = Vector3.Lerp(transform.position, end, lerpTime);
				}
				break;

			case MoveTypes.Slerp:
				{
					transform.position = Vector3.Slerp(transform.position, end, slerpTime);
				}
				break;
		}
	}

	[Button("AddForce", ButtonSizes.Medium)]
	public void AddForce(Vector2 forceVelocity)
	{
		rigidBody2D.AddForce(forceVelocity, ForceMode2D.Force);
	}

	[Button("AddImpulse", ButtonSizes.Medium)]
	public void AddImpulse(Vector2 forceVelocity)
	{
		rigidBody2D.AddForce(forceVelocity, ForceMode2D.Impulse);
	}

	[Button("SetVelocity", ButtonSizes.Medium)]
	public void SetVelocity(Vector2 velocity)
	{
		rigidBody2D.linearVelocity = velocity;
	}
}