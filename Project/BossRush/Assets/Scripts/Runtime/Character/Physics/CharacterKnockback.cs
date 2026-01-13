using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CharacterPhysicsCore))]
    public class CharacterKnockback : MonoBehaviour
    {
        [Header("Knockback")]
        [SerializeField] private float _knockbackForce = 10f;
        [SerializeField] private float _knockbackDuration = 0.3f;
        [SerializeField][Range(0f, 1f)] private float _bounceMultiplier = 0.5f;
        [SerializeField][Range(0f, 1f)] private float _verticalRatio = 0.3f;

        private CharacterPhysicsCore _physics;
        private float _knockbackTimer;
        private Vector2 _knockbackDirection;
        private float _currentHorizontalForce;
        private float _currentVerticalForce;

        public bool IsKnockback => _physics != null && _physics.IsKnockback;

        private void Awake()
        {
            _physics = GetComponent<CharacterPhysicsCore>();
        }

        public void ApplyKnockback(Vector2 direction)
        {
            if (_physics == null) return;

            if (direction.magnitude < 0.01f)
            {
                direction = new Vector2(-_physics.FacingDirection, 0f);
            }
            else
            {
                direction.Normalize();
            }

            _knockbackDirection = direction;
            _knockbackTimer = _knockbackDuration;

            _currentHorizontalForce = _knockbackForce;
            _currentVerticalForce = _knockbackForce * _verticalRatio;

            Vector2 knockbackVelocity = new Vector2(
                _knockbackDirection.x * _currentHorizontalForce,
                _knockbackDirection.y * _currentVerticalForce
            );

            _physics.ApplyVelocity(knockbackVelocity);
            _physics.SetKnockback(true);
        }

        public void OnWallCollision()
        {
            if (!IsKnockback) return;

            _knockbackDirection.x *= -1f;
            _currentHorizontalForce *= _bounceMultiplier;
            _currentVerticalForce *= _bounceMultiplier;

            Vector2 knockbackVelocity = new Vector2(
                _knockbackDirection.x * _currentHorizontalForce,
                _knockbackDirection.y * _currentVerticalForce
            );

            _physics.ApplyVelocity(knockbackVelocity);
        }

        public void AbilityTick()
        {
            if (_physics == null) return;

            if (_physics.IsKnockback)
            {
                _knockbackTimer -= Time.fixedDeltaTime;

                Vector2 knockbackVelocity = new Vector2(
                    _knockbackDirection.x * _currentHorizontalForce,
                    _knockbackDirection.y * _currentVerticalForce
                );

                _physics.ApplyVelocity(knockbackVelocity);

                if (_knockbackTimer <= 0f)
                {
                    _physics.SetKnockback(false);
                    _knockbackDirection = Vector2.zero;
                    _currentHorizontalForce = 0f;
                    _currentVerticalForce = 0f;
                }
            }
        }
    }
}