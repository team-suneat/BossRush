using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CharacterPhysicsCore))]
    public class CharacterDash : MonoBehaviour
    {
        private const float DASH_VELOCITY_Y = 0f;

        [Header("Dash")]
        [SerializeField] private float _dashDistance = 2f;
        [SerializeField] private float _dashDuration = 0.2f;
        [SerializeField] private float _dashCooldown = 0.5f;
        [SerializeField] private bool _airDashEnabled = true;

        private CharacterPhysicsCore _physics;
        private Vital _vital;
        private float _dashDurationCounter;
        private float _dashCooldownRemaining;
        private Vector2 _dashDirection;
        private float? _originalGravityScale;

        public bool IsDashing => _physics != null && _physics.IsDashing;
        public bool CanDash => _dashCooldownRemaining <= 0f && !IsDashing && HasPulse();
        public bool IsAirDashEnabled => _airDashEnabled;
        public float DashCooldownRemaining => _dashCooldownRemaining;

        private void Awake()
        {
            _physics = GetComponent<CharacterPhysicsCore>();
            _vital = GetComponentInChildren<Vital>();
        }

        // 방향 없이 대시 요청 (캐릭터가 바라보는 방향으로 대시)
        public void RequestDash()
        {
            Vector2 direction = new Vector2(_physics != null ? _physics.FacingDirection : 1f, 0f);
            RequestDash(direction);
        }

        private void RequestDash(Vector2 direction)
        {
            if (!CanDash) return;
            if (_physics == null) return;
            if (_physics.IsKnockback) return;
            if (!_airDashEnabled && !_physics.IsGrounded) return;
            ExecuteDash(direction);
        }

        private void ExecuteDash(Vector2 direction)
        {
            if (direction.magnitude < 0.01f)
            {
                direction = new Vector2(_physics.FacingDirection, 0f);
            }
            else
            {
                direction.Normalize();
            }

            // 펄스 소모
            if (!ConsumePulse())
            {
                return;
            }

            float dashSpeed = _dashDistance.SafeDivide(_dashDuration);
            _dashDirection = direction;

            Vector2 dashVelocity = new Vector2(_dashDirection.x * dashSpeed, DASH_VELOCITY_Y);
            _physics.ApplyVelocity(dashVelocity);

            // 대시 중 중력 비활성화
            if (_physics.Rigidbody != null)
            {
                _originalGravityScale = _physics.Rigidbody.gravityScale;
                _physics.Rigidbody.gravityScale = 0f;
            }

            _physics.SetDashing(true);
            _dashDurationCounter = _dashDuration;
            _dashCooldownRemaining = _dashCooldown;
        }

        public void SetAirDashEnabled(bool enabled)
        {
            _airDashEnabled = enabled;
        }

        public void AbilityTick()
        {
            if (_physics == null) return;

            // 캐릭터가 살아있지 않으면 업데이트 스킵
            if (_vital != null && !_vital.IsAlive)
            {
                if (_physics.IsDashing)
                {
                    RestoreGravity();
                    _physics.SetDashing(false);
                    _dashDirection = Vector2.zero;
                    _dashDurationCounter = 0f;
                }
                return;
            }

            if (_physics.IsDashing)
            {
                _dashDurationCounter -= Time.fixedDeltaTime;

                float dashSpeed = _dashDistance / _dashDuration;
                _physics.ApplyVelocity(new Vector2(_dashDirection.x * dashSpeed, DASH_VELOCITY_Y));

                if (_dashDurationCounter <= 0f)
                {
                    RestoreGravity();
                    _physics.SetDashing(false);
                    _dashDirection = Vector2.zero;
                }
            }

            if (_dashCooldownRemaining > 0f)
            {
                _dashCooldownRemaining -= Time.fixedDeltaTime;
            }
        }

        private bool HasPulse()
        {
            if (_vital == null)
            {
                return false;
            }

            return _vital.CanUsePulse;
        }

        private bool ConsumePulse()
        {
            if (_vital == null)
            {
                return false;
            }
            return _vital.UseDash();
        }

        private void RestoreGravity()
        {
            if (_physics.Rigidbody != null && _originalGravityScale.HasValue)
            {
                _physics.Rigidbody.gravityScale = _originalGravityScale.Value;
                _originalGravityScale = null;
            }
        }
    }
}