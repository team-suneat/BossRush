using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CharacterPhysicsCore))]
    public class CharacterDownJump : MonoBehaviour
    {
        [Header("Down Jump")]
        [SerializeField] private float _downJumpForce = 0f;
        [SerializeField] private float _downJumpPlatformIgnoreTime = 0.3f;

        private CharacterPhysicsCore _physics;
        private float _platformIgnoreTimer;

        private void Awake()
        {
            _physics = GetComponent<CharacterPhysicsCore>();
        }

        public void RequestDownJump()
        {
            if (_physics == null) return;
            if (!_physics.IsGrounded) return;
            ExecuteDownJump();
        }

        private void ExecuteDownJump()
        {
            if (_physics == null) return;

            Vector2 velocity = _physics.RigidbodyVelocity;
            float newVelocityY = Mathf.Min(velocity.y, _downJumpForce);
            _physics.ApplyVerticalVelocity(newVelocityY);

            _physics.SetIgnoringPlatforms(true);
            _platformIgnoreTimer = _downJumpPlatformIgnoreTime;
            IgnoreOneWayPlatformsBelow();
        }

        private void IgnoreOneWayPlatformsBelow()
        {
            if (_physics == null || _physics.BoxCollider == null) return;

            Vector2 boxSize = _physics.BoxCollider.size;
            Vector2 boxCenter = _physics.BoxCollider.bounds.center;
            Vector2 rayOrigin = new Vector2(boxCenter.x, boxCenter.y - boxSize.y * 0.5f - 0.05f);
            float checkDistance = 2f;

            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, checkDistance, _physics.GroundLayerMask);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null || hit.collider == _physics.BoxCollider) continue;

                OneWayPlatform oneWayPlatform = hit.collider.GetComponent<OneWayPlatform>();
                if (oneWayPlatform != null)
                {
                    oneWayPlatform.DisableCollisionTemporarily(_physics.BoxCollider, _downJumpPlatformIgnoreTime);
                }
            }
        }

        public void AbilityTick()
        {
            if (_physics == null) return;

            if (_physics.IsIgnoringPlatforms)
            {
                _platformIgnoreTimer -= Time.fixedDeltaTime;
                if (_platformIgnoreTimer <= 0f)
                {
                    _physics.SetIgnoringPlatforms(false);
                }
            }
        }
    }
}

