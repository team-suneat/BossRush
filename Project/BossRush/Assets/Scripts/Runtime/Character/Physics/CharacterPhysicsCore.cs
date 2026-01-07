using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class CharacterPhysicsCore : MonoBehaviour
    {
        [Header("Collider Setup")]
        [SerializeField] private Vector2 _colliderSize = new Vector2(0.5f, 1f);
        [SerializeField] private bool _autoSetupCollider = true;

        [Header("Collision Detection")]
        [SerializeField] private float _groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask _groundLayerMask = 1;
        [SerializeField] private float _coyoteTime = 0.2f;
        [SerializeField] private float _skinWidth = 0.03f;
        [SerializeField][Range(2, 8)] private int _horizontalRayCount = 4;
        [SerializeField][Range(2, 8)] private int _verticalRayCount = 4;

        protected Rigidbody2D _rb;
        protected BoxCollider2D _boxCollider;
        private CharacterKnockback _knockback;

        public bool IsGrounded { get; private set; }
        public bool IsOnOneWayPlatform { get; private set; }
        public bool IsJumping { get; internal set; }
        public bool IsDashing { get; internal set; }
        public bool IsKnockback { get; internal set; }
        public bool IsLeftCollision { get; private set; }
        public bool IsRightCollision { get; private set; }
        public bool IsCeiling { get; private set; }
        public bool IsCollideX { get; private set; }
        public bool IsCollideY { get; private set; }

        public Vector2 RigidbodyVelocity => _rb != null ? _rb.linearVelocity : Vector2.zero;
        public int FacingDirection { get; private set; } = 1;

        public float CoyoteTimeRemaining => _coyoteTimeCounter;
        public bool IsIgnoringPlatforms { get; private set; }
        public LayerMask GroundLayerMask => _groundLayerMask;
        public BoxCollider2D BoxCollider => _boxCollider;

        private RaycastOrigins _raycastOrigins;
        private PlayerCollisionInfo _collisionInfo;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;
        private Bounds _originalBounds;
        private float _coyoteTimeCounter;

        private void Awake()
        {
            SetupRigidbody2D();

            if (_autoSetupCollider)
            {
                SetupCollider();
            }

            InitializeCollisionSystem();
            _knockback = GetComponent<CharacterKnockback>();
        }

        private void SetupRigidbody2D()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (_rb == null)
            {
                _rb = gameObject.AddComponent<Rigidbody2D>();
            }

            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.freezeRotation = true;
            _rb.gravityScale = 3f;
        }

        private void SetupCollider()
        {
            _boxCollider = GetComponent<BoxCollider2D>();

            if (_boxCollider == null)
            {
                _boxCollider = gameObject.AddComponent<BoxCollider2D>();
            }

            _boxCollider.size = _colliderSize;
            _boxCollider.isTrigger = false;
        }

        private void InitializeCollisionSystem()
        {
            _collisionInfo.Reset();
            CalculateRaySpacing();
        }

        public void ApplyHorizontalInput(float axis)
        {
            if (_rb == null) return;
            if (IsKnockback) return;

            if (Mathf.Abs(axis) > 0.01f)
            {
                FacingDirection = axis > 0 ? 1 : -1;
            }

            _rb.linearVelocity = new Vector2(axis, _rb.linearVelocity.y);
        }

        public void ApplyHorizontalVelocity(float velocityX)
        {
            if (_rb == null) return;
            _rb.linearVelocity = new Vector2(velocityX, _rb.linearVelocity.y);
        }

        public void ApplyVerticalVelocity(float velocityY)
        {
            if (_rb == null) return;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, velocityY);
        }

        public void ApplyVelocity(Vector2 velocity)
        {
            if (_rb == null) return;
            _rb.linearVelocity = velocity;
        }

        public void PhysicsTick()
        {
            UpdateCollisionDetection();
        }

        public void SetJumping(bool value)
        {
            IsJumping = value;
        }

        public void SetDashing(bool value)
        {
            IsDashing = value;
        }

        public void SetKnockback(bool value)
        {
            IsKnockback = value;
        }

        public void SetIgnoringPlatforms(bool value)
        {
            IsIgnoringPlatforms = value;
        }

        public void ResetCoyoteTime()
        {
            _coyoteTimeCounter = 0f;
        }

        public void CheckCollisions()
        {
            if (_boxCollider == null) return;

            UpdateRaycastOrigins();
            _collisionInfo.Reset();

            CheckVerticalCollisions();
            CheckHorizontalCollisions();
        }

        private void UpdateCollisionDetection()
        {
            CheckCollisions();
            UpdateGroundedState();
        }

        private void UpdateGroundedState()
        {
            if (_collisionInfo.below)
            {
                IsGrounded = true;
                _coyoteTimeCounter = _coyoteTime;
                IsJumping = false;
            }
            else
            {
                _coyoteTimeCounter -= Time.fixedDeltaTime;
                IsGrounded = _coyoteTimeCounter > 0f;
            }

            IsLeftCollision = _collisionInfo.left;
            IsRightCollision = _collisionInfo.right;
            IsCeiling = _collisionInfo.above;
            IsCollideX = _collisionInfo.left || _collisionInfo.right;
            IsCollideY = _collisionInfo.above || _collisionInfo.below;
        }

        private void UpdateRaycastOrigins()
        {
            if (_boxCollider == null) return;

            _originalBounds = _boxCollider.bounds;

            Bounds innerBounds = _boxCollider.bounds;
            innerBounds.Expand(_skinWidth * -2);

            _raycastOrigins.bottomLeft = new Vector2(_originalBounds.min.x, _originalBounds.min.y);
            _raycastOrigins.bottomRight = new Vector2(_originalBounds.max.x, _originalBounds.min.y);
            _raycastOrigins.topLeft = new Vector2(_originalBounds.min.x, _originalBounds.max.y);
            _raycastOrigins.topRight = new Vector2(_originalBounds.max.x, _originalBounds.max.y);

            _raycastOrigins.top = new Vector2(innerBounds.center.x, _originalBounds.max.y);
            _raycastOrigins.bottom = new Vector2(innerBounds.center.x, _originalBounds.min.y);
            _raycastOrigins.left = new Vector2(_originalBounds.min.x, innerBounds.center.y);
            _raycastOrigins.right = new Vector2(_originalBounds.max.x, innerBounds.center.y);
        }

        private void CalculateRaySpacing()
        {
            if (_boxCollider == null) return;

            Bounds innerBounds = _boxCollider.bounds;
            innerBounds.Expand(_skinWidth * -2);

            float innerBoundsWidth = innerBounds.size.x;
            float innerBoundsHeight = innerBounds.size.y;

            _horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 1, int.MaxValue);
            _verticalRayCount = Mathf.Clamp(_verticalRayCount, 1, int.MaxValue);

            if (_horizontalRayCount > 2)
            {
                _horizontalRaySpacing = innerBoundsHeight / (_horizontalRayCount - 1);
            }
            else
            {
                _horizontalRaySpacing = 0f;
            }

            if (_verticalRayCount > 2)
            {
                _verticalRaySpacing = innerBoundsWidth / (_verticalRayCount - 1);
            }
            else
            {
                _verticalRaySpacing = 0f;
            }
        }

        private Vector2 GetVerticalRayOrigin(int index, bool isTop)
        {
            if (_boxCollider == null) return Vector2.zero;

            Bounds innerBounds = _boxCollider.bounds;
            innerBounds.Expand(_skinWidth * -2);

            float contactOffset = Physics2D.defaultContactOffset;
            float yPos = isTop ? _originalBounds.max.y : _originalBounds.min.y + _skinWidth;

            float leftEdge = _originalBounds.min.x - contactOffset;
            float rightEdge = _originalBounds.max.x + contactOffset;

            if (_verticalRayCount == 1)
            {
                return new Vector2(innerBounds.center.x, yPos);
            }
            else if (_verticalRayCount == 2)
            {
                if (index == 0)
                {
                    return new Vector2(leftEdge, yPos);
                }
                else
                {
                    return new Vector2(rightEdge, yPos);
                }
            }
            else
            {
                if (index == 0)
                {
                    return new Vector2(leftEdge, yPos);
                }
                else if (index == _verticalRayCount - 1)
                {
                    return new Vector2(rightEdge, yPos);
                }
                else
                {
                    float innerWidth = innerBounds.size.x;
                    float spacing = innerWidth / (_verticalRayCount - 1);
                    return new Vector2(innerBounds.min.x + spacing * index, yPos);
                }
            }
        }

        private Vector2 GetHorizontalRayOrigin(int index, bool isLeft)
        {
            if (_boxCollider == null) return Vector2.zero;

            Bounds innerBounds = _boxCollider.bounds;
            innerBounds.Expand(_skinWidth * -2);

            float contactOffset = Physics2D.defaultContactOffset;
            float xPos = isLeft ? _originalBounds.min.x - contactOffset : _originalBounds.max.x + contactOffset;

            float bottomEdge = _originalBounds.min.y - contactOffset;
            float topEdge = _originalBounds.max.y + contactOffset;

            if (_horizontalRayCount == 1)
            {
                return new Vector2(xPos, innerBounds.center.y);
            }
            else if (_horizontalRayCount == 2)
            {
                if (index == 0)
                {
                    return new Vector2(xPos, bottomEdge);
                }
                else
                {
                    return new Vector2(xPos, topEdge);
                }
            }
            else
            {
                if (index == 0)
                {
                    return new Vector2(xPos, bottomEdge);
                }
                else if (index == _horizontalRayCount - 1)
                {
                    return new Vector2(xPos, topEdge);
                }
                else
                {
                    float innerHeight = innerBounds.size.y;
                    float spacing = innerHeight / (_horizontalRayCount - 1);
                    return new Vector2(xPos, innerBounds.min.y + spacing * index);
                }
            }
        }

        private void CheckVerticalCollisions()
        {
            if (_boxCollider == null || _rb == null) return;

            float velocityY = _rb.linearVelocity.y;

            float checkDistanceDown = _groundCheckDistance;
            if (velocityY <= 0f)
            {
                checkDistanceDown = Mathf.Max(_groundCheckDistance, Mathf.Abs(velocityY * Time.fixedDeltaTime) + _skinWidth);
            }

            checkDistanceDown = Mathf.Max(checkDistanceDown, _skinWidth * 2f);

            float checkDistanceUp = 0f;
            if (velocityY > 0f)
            {
                checkDistanceUp = Mathf.Abs(velocityY * Time.fixedDeltaTime) + _skinWidth;
            }

            _collisionInfo.below = false;
            _collisionInfo.above = false;
            IsOnOneWayPlatform = false;

            for (int i = 0; i < _verticalRayCount; i++)
            {
                Vector2 rayOrigin = GetVerticalRayOrigin(i, false);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, checkDistanceDown, _groundLayerMask);

                Debug.DrawRay(rayOrigin, Vector2.down * checkDistanceDown, hit.collider != null ? Color.red : Color.yellow);

                if (hit.collider != null)
                {
                    if (hit.collider == _boxCollider || hit.collider.gameObject == gameObject)
                    {
                        continue;
                    }

                    OneWayPlatform oneWayPlatform = hit.collider.GetComponent<OneWayPlatform>();
                    bool isOneWayPlatform = oneWayPlatform != null;
                    float currentVelocityY = _rb.linearVelocity.y;

                    if (isOneWayPlatform)
                    {
                        if (hit.distance == 0f)
                        {
                            continue;
                        }

                        if (IsIgnoringPlatforms)
                        {
                            continue;
                        }

                        if (currentVelocityY < 0f)
                        {
                            continue;
                        }
                    }

                    bool isActuallyGrounded = !isOneWayPlatform || currentVelocityY >= 0f;

                    if (isActuallyGrounded)
                    {
                        _collisionInfo.below = true;
                        IsOnOneWayPlatform = isOneWayPlatform;
                        break;
                    }
                }
            }

            if (checkDistanceUp > 0f)
            {
                for (int i = 0; i < _verticalRayCount; i++)
                {
                    Vector2 rayOrigin = GetVerticalRayOrigin(i, true);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, checkDistanceUp, _groundLayerMask);

                    Debug.DrawRay(rayOrigin, Vector2.up * checkDistanceUp, hit.collider != null ? Color.red : Color.yellow);

                    if (hit.collider != null)
                    {
                        if (hit.collider == _boxCollider || hit.collider.gameObject == gameObject)
                        {
                            continue;
                        }

                        OneWayPlatform oneWayPlatform = hit.collider.GetComponent<OneWayPlatform>();
                        if (oneWayPlatform != null)
                        {
                            continue;
                        }

                        _collisionInfo.above = true;
                        break;
                    }
                }
            }
        }

        private void CheckHorizontalCollisions()
        {
            if (_boxCollider == null || _rb == null) return;

            float velocityX = _rb.linearVelocity.x;

            if (Mathf.Abs(velocityX) < 0.01f)
            {
                _collisionInfo.left = false;
                _collisionInfo.right = false;
                _collisionInfo.faceDir = 1;
                return;
            }

            float directionX = Mathf.Sign(velocityX);
            _collisionInfo.faceDir = (int)directionX;

            float rayLength = Mathf.Abs(velocityX * Time.fixedDeltaTime) + _skinWidth;

            if (rayLength < _skinWidth * 2f)
            {
                rayLength = _skinWidth * 2f;
            }

            _collisionInfo.left = false;
            _collisionInfo.right = false;

            for (int i = 0; i < _horizontalRayCount; i++)
            {
                Vector2 rayOrigin = GetHorizontalRayOrigin(i, directionX == -1);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _groundLayerMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, hit.collider != null ? Color.red : Color.yellow);

                if (hit.collider != null)
                {
                    if (hit.collider == _boxCollider || hit.collider.gameObject == gameObject)
                    {
                        continue;
                    }

                    OneWayPlatform oneWayPlatform = hit.collider.GetComponent<OneWayPlatform>();
                    if (oneWayPlatform != null)
                    {
                        if (hit.distance == 0f)
                        {
                            continue;
                        }
                        continue;
                    }

                    if (directionX == -1)
                    {
                        _collisionInfo.left = true;
                    }
                    else
                    {
                        _collisionInfo.right = true;
                    }

                    if (IsKnockback && _knockback != null)
                    {
                        _knockback.OnWallCollision();
                    }

                    break;
                }
            }
        }
    }
}

