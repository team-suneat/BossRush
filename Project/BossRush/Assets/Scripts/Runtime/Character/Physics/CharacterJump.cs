using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CharacterPhysicsCore))]
    public class CharacterJump : MonoBehaviour
    {
        [Header("Jump")]
        [SerializeField] private float _jumpForce = 15f;
        [SerializeField] private float _jumpBufferTime = 0.2f;
        [SerializeField] private float _jumpCutMultiplier = 0.5f;
        [SerializeField] private int _extraJumps = 0;

        private CharacterPhysicsCore _physics;
        private float _jumpBufferCounter;
        private int _jumpCounter;

        public int RemainingJumps => _jumpCounter;
        public int ExtraJumps { get; private set; }

        private void Awake()
        {
            _physics = GetComponent<CharacterPhysicsCore>();
            InitializeJumpSystem();
        }

        private void InitializeJumpSystem()
        {
            _jumpCounter = 1 + _extraJumps;
            ExtraJumps = _extraJumps;
        }

        public void RequestJump()
        {
            _jumpBufferCounter = _jumpBufferTime;
        }

        public void ExecuteJump()
        {
            if (_physics == null) return;

            _physics.ApplyVerticalVelocity(_jumpForce);
            _physics.ResetCoyoteTime();
            _jumpBufferCounter = 0f;
            _physics.SetJumping(true);

            if (_physics.IsGrounded)
            {
                _jumpCounter = _extraJumps;
            }
            else
            {
                if (_jumpCounter > 0)
                {
                    _jumpCounter--;
                }
            }
        }

        public void ReleaseJump()
        {
            if (_physics == null) return;

            Vector2 velocity = _physics.RigidbodyVelocity;
            if (velocity.y > 0f)
            {
                _physics.ApplyVerticalVelocity(velocity.y * _jumpCutMultiplier);
            }
        }

        public void ResetJumpCounterOnLanding()
        {
            _jumpCounter = 1 + _extraJumps;
        }

        public void SetExtraJumps(int count)
        {
            _extraJumps = Mathf.Max(0, count);
            ExtraJumps = _extraJumps;

            if (_physics.IsGrounded)
            {
                _jumpCounter = 1 + _extraJumps;
            }
        }

        public void AbilityTick()
        {
            if (_jumpBufferCounter > 0f)
            {
                _jumpBufferCounter -= Time.fixedDeltaTime;
            }
        }
    }
}

