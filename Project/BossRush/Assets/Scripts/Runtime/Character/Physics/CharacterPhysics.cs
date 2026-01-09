using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CharacterPhysicsCore))]
    public class CharacterPhysics : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;

        private CharacterPhysicsCore _core;
        private CharacterJump _jump;
        private CharacterDash _dash;
        private CharacterDownJump _downJump;
        private CharacterKnockback _knockback;

        private void Awake()
        {
            _core = GetComponent<CharacterPhysicsCore>();
            _jump = GetComponent<CharacterJump>();
            _dash = GetComponent<CharacterDash>();
            _downJump = GetComponent<CharacterDownJump>();
            _knockback = GetComponent<CharacterKnockback>();
        }

        #region Core 프로퍼티 위임

        public bool IsGrounded => _core != null && _core.IsGrounded;
        public bool IsOnOneWayPlatform => _core != null && _core.IsOnOneWayPlatform;
        public bool IsLeftCollision => _core != null && _core.IsLeftCollision;
        public bool IsRightCollision => _core != null && _core.IsRightCollision;
        public bool IsCeiling => _core != null && _core.IsCeiling;
        public bool IsCollideX => _core != null && _core.IsCollideX;
        public bool IsCollideY => _core != null && _core.IsCollideY;
        public Vector2 RigidbodyVelocity => _core != null ? _core.RigidbodyVelocity : Vector2.zero;
        public int FacingDirection => _core != null ? _core.FacingDirection : 1;
        public float MoveSpeed => _moveSpeed;

        #endregion Core 프로퍼티 위임

        #region Ability 프로퍼티 위임

        public bool IsDashing => _dash != null && _dash.IsDashing;
        public bool CanDash => _dash != null && _dash.CanDash;
        public bool IsAirDashEnabled => _dash != null && _dash.IsAirDashEnabled;
        public float DashCooldownRemaining => _dash != null ? _dash.DashCooldownRemaining : 0f;
        public int RemainingJumps => _jump != null ? _jump.RemainingJumps : 0;
        public int ExtraJumps => _jump != null ? _jump.ExtraJumps : 0;
        public bool IsKnockback => _knockback != null && _knockback.IsKnockback;

        #endregion Ability 프로퍼티 위임

        #region Core 메서드 위임

        public void ApplyHorizontalInput(float axis)
        {
            if (_core != null)
            {
                _core.ApplyHorizontalInput(axis);
            }
        }

        public void ApplyHorizontalVelocity(float velocityX)
        {
            if (_core != null)
            {
                _core.ApplyHorizontalVelocity(velocityX);
            }
        }

        public void ApplyVerticalVelocity(float velocityY)
        {
            if (_core != null)
            {
                _core.ApplyVerticalVelocity(velocityY);
            }
        }

        public void ApplyVelocity(Vector2 velocity)
        {
            if (_core != null)
            {
                _core.ApplyVelocity(velocity);
            }
        }

        #endregion Core 메서드 위임

        #region Ability 메서드 위임

        public void RequestJump()
        {
            _jump?.RequestJump();
        }

        public void ExecuteJump()
        {
            _jump?.ExecuteJump();
        }

        public void ReleaseJump()
        {
            _jump?.ReleaseJump();
        }

        public void RequestDownJump()
        {
            _downJump?.RequestDownJump();
        }

        public void SetExtraJumps(int count)
        {
            _jump?.SetExtraJumps(count);
        }

        public void SetAirDashEnabled(bool enabled)
        {
            _dash?.SetAirDashEnabled(enabled);
        }

        public void ResetJumpCounterOnLanding()
        {
            _jump?.ResetJumpCounterOnLanding();
        }

        public void RequestDash()
        {
            _dash?.RequestDash();
        }

        public void ApplyKnockback(Vector2 direction)
        {
            _knockback?.ApplyKnockback(direction);
        }

        #endregion Ability 메서드 위임

        #region 통합 업데이트

        public void PhysicsTick()
        {
            _core?.PhysicsTick();
            _jump?.AbilityTick();
            _dash?.AbilityTick();
            _downJump?.AbilityTick();
            _knockback?.AbilityTick();
        }

        #endregion 통합 업데이트
    }
}