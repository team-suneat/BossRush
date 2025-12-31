using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    [System.Serializable]
    public class OnCollisionCallback : UnityEvent
    {
    }

    [RequireComponent(typeof(Collider2D))]
    public class CollisionController : RaycastController
    {
        [ReadOnly] public XCollision collisions;

        public bool UseCheckFrontGround;

        public bool UseDetection;

        [ReadOnly] public bool DefaultIgnorePlatform;

        [ReadOnly] public bool m_isIgnorePlatform;

        [ReadOnly] public bool IsOnPlatform;

        private float m_maxSlopeAngle = 80f; // 사용하지 않음

        [Title("Collision")]
        public Collider2D HorizontalCollision;

        public Collider2D VerticalCollision;

        [Title("OnCollision")]
        public OnCollisionCallback OnCollision;

        public bool IsGrounded
        {
            get
            {
                return collisions.below;
            }

            private set
            {
                if (collisions.below != value)
                {
                    if (value)
                    {
                        if (OnCollision != null)
                        {
                            OnCollision.Invoke();
                        }
                    }

                    collisions.below = value;
                }
            }
        }

        public bool IsCeiling
        {
            get
            {
                return collisions.above;
            }

            private set
            {
                if (collisions.above != value)
                {
                    if (value)
                    {
                        if (OnCollision != null)
                        {
                            OnCollision.Invoke();
                        }
                    }
                    collisions.above = value;
                }
            }
        }

        public bool IsLeftCollision
        {
            get
            {
                return collisions.left;
            }

            private set
            {
                if (collisions.left != value)
                {
                    if (value)
                    {
                        if (OnCollision != null)
                        {
                            OnCollision.Invoke();
                        }
                    }
                    collisions.left = value;
                }
            }
        }

        public bool IsRightCollision
        {
            get
            {
                return collisions.right;
            }

            private set
            {
                if (collisions.right != value)
                {
                    if (value)
                    {
                        if (OnCollision != null)
                        {
                            OnCollision.Invoke();
                        }
                    }
                    collisions.right = value;
                }
            }
        }

        public bool IsCollideX
        {
            get
            {
                return collisions.left || collisions.right;
            }
        }

        public bool IsCollideY
        {
            get
            {
                return collisions.above || collisions.below;
            }
        }

        public bool IsFrontLeftGround
        {
            get
            {
                return collisions.frontLeftGround;
            }
        }

        public bool IsFrontRightGround
        {
            get
            {
                return collisions.frontRightGround;
            }
        }

        public bool IsLeftDetection
        {
            get; set;
        }

        public bool IsRightDetection
        {
            get; set;
        }

        public bool IsIgnorePlatform
        {
            get
            {
                return m_isIgnorePlatform;
            }

            set
            {
                if (value)
                {
                    IsGrounded = false;
                }

                m_isIgnorePlatform = value;
            }
        }

#if UNITY_EDITOR

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Collider != null)
            {
                Collider.isTrigger = true;
            }
        }

#endif

        protected override void Awake()
        {
            base.Awake();

            if (Collider != null)
            {
                Collider.offset = new Vector2(0, Collider.offset.y);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            collisions.faceDir = 1;
        }

        public void SetIgnorePlatform(bool isIgnorePlatform)
        {
            m_isIgnorePlatform = isIgnorePlatform;

            if (isIgnorePlatform)
            {
                IsGrounded = false;
            }
        }

        public void ResetIgnorePlatform()
        {
            SetIgnorePlatform(DefaultIgnorePlatform);
        }

        public void SetDefaultIgnorePlatform(bool isIgnorePlatform)
        {
            DefaultIgnorePlatform = isIgnorePlatform;

            ResetIgnorePlatform();
        }

        public void Move(Vector2 moveAmount)
        {
            Move(moveAmount, false);
        }

        public void Move(Vector2 moveAmount, bool standingOnPlatform)
        {
            UpdateRaycastOrigins();

            collisions.Reset();

            collisions.moveAmountOld = moveAmount;

            if (moveAmount.y < 0)
            {
                DescendSlope(ref moveAmount);
            }

            if (moveAmount.x != 0)
            {
                collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }

            HorizontalCollisions(ref moveAmount);

            if (UseCheckFrontGround)
            {
                FrontGroundCollisions(moveAmount);
            }

            if (UseDetection)
            {
                DetectionCollisions(moveAmount);
            }

            VerticalCollisions(ref moveAmount);

            if (transform != null)
            {
                if (false == moveAmount.IsNan())
                {
                    transform.Translate(moveAmount);
                }
            }

            if (standingOnPlatform || IsOnPlatform)
            {
                IsGrounded = true;
            }
        }

        private void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

            if (Mathf.Abs(moveAmount.x) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }

            Collider2D hitCollision = null;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                if (hit.collider != null)
                {
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

                    if (hit.collider.CompareTag(GameTags.Through))
                    {
                        continue;
                    }
                    if (hit.distance == 0)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        continue;
                    }

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && slopeAngle <= m_maxSlopeAngle)
                    {
                        if (collisions.descendingSlope)
                        {
                            collisions.descendingSlope = false;
                            moveAmount = collisions.moveAmountOld;
                        }
                        float distanceToSlopeStart = 0;
                        if (slopeAngle != collisions.slopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - skinWidth;
                            moveAmount.x -= distanceToSlopeStart * directionX;
                        }
                        ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                        moveAmount.x += distanceToSlopeStart * directionX;
                    }

                    if (false == collisions.climbingSlope || slopeAngle > m_maxSlopeAngle)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;

                        if (collisions.climbingSlope)
                        {
                            moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                        }

                        IsLeftCollision = directionX == -1;
                        IsRightCollision = directionX == 1;
                    }

                    hitCollision = hit.collider;
                }
                else
                {
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.yellow);
                }
            }

            HorizontalCollision = hitCollision;
        }

        private void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

            Collider2D hitCollision = null;
            bool isGrounded = false;
            bool isCeiling = false;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                if (hit.collider != null)
                {
                    // 엣지 플랫폼
                    if (hit.collider.CompareTag(GameTags.Through))
                    {
                        if (hit.distance == 0)
                        {
                            continue;
                        }

                        if (directionY == 1)
                        {
                            continue;
                        }

                        // 엣지를 무시하는 중
                        if (IsIgnorePlatform)
                        {
                            IsGrounded = false;
                            continue;
                        }

                        if (collisions.fallingThroughPlatform)
                        {
                            continue;
                        }

                        collisions.standingThroughPlatform = true;
                        if (collisions.actionFalling)
                        {
                            StartXCoroutine(FallingThroughPlatform());
                            continue;
                        }
                    }
                    else
                    {
                        collisions.standingThroughPlatform = false;
                    }

                    if (false == Mathf.Approximately(0f, hit.distance))
                    {
                        moveAmount.y = (hit.distance - skinWidth) * directionY;
                    }
                    else
                    {
                        if (false == IsCollideX)
                        {
                            moveAmount.y = (hit.distance - skinWidth) * directionY;
                        }
                    }

                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                    }

                    if (false == isGrounded)
                    {
                        IsGrounded = directionY == -1;
                    }

                    if (false == isCeiling)
                    {
                        IsCeiling = directionY == 1;
                    }

                    if (hitCollision == null)
                    {
                        hitCollision = hit.collider;
                    }

                    DebugEx.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
                }
                else
                {
                    DebugEx.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.yellow);
                }
            }

            // 갑자기 가만히 있는 렛모탄이 검사를 아래가 아닌 위를 검사하기 시작했다.

            if (hitCollision != null)
            {
                VerticalCollision = hitCollision;
                IsOnPlatform = GameTags.CompareTag(hitCollision, GameTags.MovingPlatform);
            }
            else
            {
                IsOnPlatform = false;
            }
            if (collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
                Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != collisions.slopeAngle)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        collisions.slopeAngle = slopeAngle;
                        collisions.slopeNormal = hit.normal;
                    }
                }
            }
        }

        private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
        {
            float moveDistance = Mathf.Abs(moveAmount.x);
            float climbMoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

            if (moveAmount.y <= climbMoveAmountY)
            {
                moveAmount.y = climbMoveAmountY;
                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                IsGrounded = true;
                collisions.climbingSlope = true;
                collisions.slopeAngle = slopeAngle;
                collisions.slopeNormal = slopeNormal;
            }
        }

        private void DescendSlope(ref Vector2 moveAmount)
        {
            RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
            RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
            if (maxSlopeHitLeft ^ maxSlopeHitRight)
            {
                SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
                SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
            }

            if (false == collisions.slidingDownMaxSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != 0 && slopeAngle <= m_maxSlopeAngle)
                    {
                        if (Mathf.Sign(hit.normal.x) == directionX)
                        {
                            if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                            {
                                float moveDistance = Mathf.Abs(moveAmount.x);
                                float descendMoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                                moveAmount.y -= descendMoveAmountY;

                                collisions.slopeAngle = slopeAngle;
                                collisions.descendingSlope = true;
                                IsGrounded = true;
                                collisions.slopeNormal = hit.normal;
                            }
                        }
                    }
                }
            }
        }

        private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
        {
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle > m_maxSlopeAngle)
                {
                    moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                    collisions.slopeAngle = slopeAngle;
                    collisions.slidingDownMaxSlope = true;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }

        private void FrontGroundCollisions(Vector2 moveAmount)
        {
            float rayLengthX = Mathf.Abs(moveAmount.x) + skinWidth;
            float rayLengthY = Mathf.Abs(moveAmount.y) + skinWidth;

            Vector2 rayOrigin = Vector3.zero;
            RaycastHit2D hit;

            // frontLeftGround
            rayOrigin = raycastOrigins.bottomLeft + (Vector2.left * rayLengthX);
            rayOrigin += Vector2.left * (verticalRaySpacing + moveAmount.x);
            hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLengthY, collisionMask);

            collisions.frontLeftGround = hit;

            // frontRightGround
            rayOrigin = raycastOrigins.bottomRight + (Vector2.right * rayLengthX);
            rayOrigin += Vector2.right * (verticalRaySpacing + moveAmount.x);
            hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLengthY, collisionMask);

            collisions.frontRightGround = hit;
        }

        private void DetectionCollisions(Vector2 moveAmount)
        {
            if (detectionMask == default(LayerMask))
            {
                return;
            }

            float directionX = collisions.faceDir;
            float rayLength = skinWidth + Mathf.Abs(moveAmount.x) * 10;

            IsLeftDetection = false;
            IsRightDetection = false;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);

                RaycastHit2D hitDetection = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, detectionMask);
                if (false == hitDetection || hitDetection.distance == 0)
                {
                    continue;
                }

                if (directionX == -1)
                {
                    IsLeftDetection = true;
                }
                if (directionX == 1)
                {
                    IsRightDetection = true;
                }
            }
        }

        private void ResetFallingThroughPlatform()
        {
            collisions.fallingThroughPlatform = false;
        }

        private IEnumerator FallingThroughPlatform()
        {
            collisions.fallingThroughPlatform = true;

            yield return new WaitForSeconds(0.1f);

            collisions.fallingThroughPlatform = false;

            collisions.actionFalling = false;
        }
    }
}