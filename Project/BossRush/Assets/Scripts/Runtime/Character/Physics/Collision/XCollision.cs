using UnityEngine;

namespace TeamSuneat
{
	[System.Serializable]
	public struct XCollision
	{
		public bool above, below;

		public bool left, right;

		public bool climbingSlope;

		public bool descendingSlope;

		public bool slidingDownMaxSlope;

		public bool frontLeftGround;

		public bool frontRightGround;

		public float slopeAngle, slopeAngleOld;

		public Vector2 slopeNormal;

		public Vector2 moveAmountOld;

		public int faceDir;

		public bool standingThroughPlatform;

		public bool fallingThroughPlatform;

		public bool actionFalling; // ↓ && jump

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slidingDownMaxSlope = false;

			frontLeftGround = false;
			frontRightGround = false;

			slopeNormal = Vector2.zero;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
}