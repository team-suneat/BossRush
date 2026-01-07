using UnityEngine;

namespace TeamSuneat
{
    public class AttackState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterPhysics _physics;
        private PlayerInput _input;
        private CharacterAnimator _animator;

        // 콤보 관련 필드
        private int _currentComboIndex = 0;
        private int _maxComboCount = 3;
        private bool _canQueueCombo = false;
        private bool _hasQueuedCombo = false;

        public AttackState(CharacterStateMachine stateMachine, CharacterPhysics physics, CharacterAnimator animator, PlayerInput input)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _animator = animator;
            _input = input;
        }

        public void SetMaxComboCount(int maxComboCount)
        {
            _maxComboCount = Mathf.Max(1, maxComboCount);
        }

        public void EnableComboQueue()
        {
            _canQueueCombo = true;
        }

        public void DisableComboQueue()
        {
            _canQueueCombo = false;

            if (_hasQueuedCombo)
            {
                _hasQueuedCombo = false;
            }
        }

        public void OnEnter()
        {
            _currentComboIndex = 0;
            _canQueueCombo = false;
            _hasQueuedCombo = false;

            // 이동 잠금
            _animator?.LockMovement();

            PlayComboAnimation(_currentComboIndex);
        }

        public void OnUpdate()
        {
            if (_input != null && _input.IsAttackPressed)
            {
                if (_canQueueCombo)
                {
                    if (_currentComboIndex < _maxComboCount - 1)
                    {
                        _hasQueuedCombo = true;
                    }
                }
            }
        }

        public void OnFixedUpdate()
        {
            if (_input == null || _physics == null || _animator == null)
            {
                return;
            }

            if (_hasQueuedCombo && _canQueueCombo)
            {
                _currentComboIndex++;
                _hasQueuedCombo = false;
                _canQueueCombo = false;
                PlayComboAnimation(_currentComboIndex);
            }

            if (!_animator.IsAttacking)
            {
                if (_physics.IsGrounded)
                {
                    if (Mathf.Abs(_input.HorizontalInput) > 0.01f)
                    {
                        _stateMachine.TransitionToState(CharacterState.Walk);
                    }
                    else
                    {
                        _stateMachine.TransitionToState(CharacterState.Idle);
                    }
                }
                else
                {
                    _stateMachine.TransitionToState(CharacterState.Falling);
                }
                return;
            }
        }

        public void OnExit()
        {
            _currentComboIndex = 0;
            _canQueueCombo = false;
            _hasQueuedCombo = false;

            // 이동 해제
            _animator?.UnlockMovement();
        }

        private void PlayComboAnimation(int comboIndex)
        {
            string animationName = $"Attack{comboIndex + 1}";
            _animator?.PlayAttackAnimation(animationName);
        }

        public void OnJumpRequested()
        {
            // 공격 중에는 점프 무시
        }

        public void OnDashRequested(Vector2 direction)
        {
            // 공격 중에는 대시 무시
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // 공격에서 전환 가능한 상태
            return targetState == CharacterState.Idle ||
                   targetState == CharacterState.Walk ||
                   targetState == CharacterState.Falling;
        }
    }
}

