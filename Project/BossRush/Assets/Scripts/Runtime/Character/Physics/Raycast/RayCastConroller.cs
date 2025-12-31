using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class RaycastController : XBehaviour
	{
		[SuffixLabel("충돌하는 레이어")]
		public LayerMask collisionMask;

		[SuffixLabel("회피하는 레이어")]
		public LayerMask detectionMask;

		private float defaultSkinWidth = .03f;

		public float skinWidth = .03f;

		private const float dstBetweenRays = .25f;

		[ReadOnly] public int horizontalRayCount;

		[ReadOnly] public int verticalRayCount;

		[ReadOnly] public float horizontalRaySpacing;

		[ReadOnly] public float verticalRaySpacing;

		public BoxCollider2D Collider;

		[ReadOnly] public RaycastOrigins raycastOrigins;

#if UNITY_EDITOR

		public override void AutoSetting()
		{
			base.AutoSetting();

			Collider = GetComponent<BoxCollider2D>();

			skinWidth = defaultSkinWidth;
		}

#endif

		protected virtual void Awake()
		{
			if (Collider == null)
			{
				Collider = GetComponent<BoxCollider2D>();
			}

			skinWidth = defaultSkinWidth;
		}

		protected override void OnStart()
		{
			base.OnStart();

			CalculateRaySpacing();
		}

		public void UpdateRaycastOrigins()
		{
			Bounds bounds = Collider.bounds;
			bounds.Expand(skinWidth * -2);

			raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
			raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
			raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
			raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

			raycastOrigins.top = new Vector2(bounds.center.x, bounds.max.y);
			raycastOrigins.bottom = new Vector2(bounds.center.x, bounds.min.y);
			raycastOrigins.left = new Vector2(bounds.min.x, bounds.center.y);
			raycastOrigins.right = new Vector2(bounds.max.x, bounds.center.y);
		}

		public void CalculateRaySpacing()
		{
			Bounds bounds = Collider.bounds;
			bounds.Expand(skinWidth * -2);

			float boundsWidth = bounds.size.x;
			float boundsHeight = bounds.size.y;

			horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
			verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

			horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
			verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
		}

		public void SetColliderInfo(Vector2 size, Vector2 offset)
		{
			if (false == size.Equals(Vector2.zero))
			{
				Collider.size = size;
			}

			if (false == offset.Equals(Vector2.zero))
			{
				Collider.offset = offset;
			}

			if (size.Equals(Vector2.zero) && offset.Equals(Vector2.zero))
			{
				return;
			}
			else
			{
				CalculateRaySpacing();
			}
		}
	}
}