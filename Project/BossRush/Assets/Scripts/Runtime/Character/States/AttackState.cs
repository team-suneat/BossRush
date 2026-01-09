using UnityEngine;

namespace TeamSuneat
{
    public class AttackState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterPhysics _physics;
        private readonly Character _character;
        private CharacterAnimator _animator;

        // 콤보 관련 필드
        private int _currentComboIndex = 0;
        private int _maxComboCount = 2;
        private bool _canQueueCombo = false;
        private bool _hasQueuedCombo = false;
        private bool _isAirAttack = false;

        public AttackState(CharacterStateMachine stateMachine, CharacterPhysics physics, CharacterAnimator animator, Character character)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _animator = animator;
            _character = character;
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

        public void EnableFlip()
        {
            _animator?.UnlockFlip();
        }

        public void DisableFlip()
        {
            _animator?.LockFlip();
        }

        public void OnEnter()
        {
            _currentComboIndex = 0;
            _canQueueCombo = false;
            _hasQueuedCombo = false;

            // flip 잠금 (애니메이션 이벤트로 해제)
            _animator?.LockFlip();

            // 공중/지상 상태 확인
            if (_physics != null && !_physics.IsGrounded)
            {
                // 공중 공격: 단일 콤보만 지원
                _isAirAttack = true;
                _maxComboCount = 1;
                PlayAirAttackAnimation();
            }
            else
            {

                // 이동 잠금
                _animator?.LockMovement();

                // 지상 공격: 기존 콤보 시스템 사용
                _isAirAttack = false;
                _maxComboCount = 3;  // 기본값으로 초기화
                PlayComboAnimation(_currentComboIndex);
            }
        }

        public void OnUpdate()
        {
            // 공중 공격 중에는 콤보 입력 무시 (단일 콤보만 지원)
            if (_isAirAttack)
            {
                return;
            }

            if (_character != null)
            {
                var cmd = _character.Command;
                if (cmd.IsAttackPressed)
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
        }

        public void OnFixedUpdate()
        {
            if (_physics == null || _animator == null || _character == null)
            {
                return;
            }

            if (_hasQueuedCombo)
            {
                _currentComboIndex++;
                _hasQueuedCombo = false;
                _canQueueCombo = false;
                PlayComboAnimation(_currentComboIndex);
            }

            if (!_animator.IsAttacking)
            {
                var cmd = _character.Command;
                if (_physics.IsGrounded)
                {
                    // 착지 시
                    if (Mathf.Abs(cmd.HorizontalInput) > 0.01f)
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
                    // 공중일 때: 속도에 따라 Jumping 또는 Falling로 전환
                    if (_physics.RigidbodyVelocity.y > 0f)
                    {
                        _stateMachine.TransitionToState(CharacterState.Jumping);
                    }
                    else
                    {
                        _stateMachine.TransitionToState(CharacterState.Falling);
                    }
                }
                return;
            }
        }

        public void OnExit()
        {
            _currentComboIndex = 0;
            _canQueueCombo = false;
            _hasQueuedCombo = false;
            _isAirAttack = false;

            // 이동 해제
            _animator?.UnlockMovement();

            // flip 해제
            _animator?.UnlockFlip();
        }

        private void PlayComboAnimation(int comboIndex)
        {
            // 콤보 전환 시 입력 방향에 맞게 캐릭터 반전 (flip 잠금 상태에서도 작동하도록 ForceFace 사용)
            if (_character != null && _stateMachine != null && _stateMachine.Character != null)
            {
                var cmd = _character.Command;
                if (Mathf.Abs(cmd.HorizontalInput) > 0.01f)
                {
                    Character.FacingDirections targetDirection = cmd.HorizontalInput > 0
                        ? Character.FacingDirections.Right
                        : Character.FacingDirections.Left;

                    _stateMachine.Character.ForceFace(targetDirection);
                }
            }

            string animationName = $"Attack{comboIndex + 1}";
            _animator?.PlayAttackAnimation(animationName);
        }

        private void PlayAirAttackAnimation()
        {
            // 공중 공격 시 입력 방향에 맞게 캐릭터 반전 (flip 잠금 상태에서도 작동하도록 ForceFace 사용)
            if (_character != null && _stateMachine != null && _stateMachine.Character != null)
            {
                var cmd = _character.Command;
                if (Mathf.Abs(cmd.HorizontalInput) > 0.01f)
                {
                    Character.FacingDirections targetDirection = cmd.HorizontalInput > 0
                        ? Character.FacingDirections.Right
                        : Character.FacingDirections.Left;

                    _stateMachine.Character.ForceFace(targetDirection);
                }
            }

            _animator?.PlayAttackAnimation("AirAttack");
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
                   targetState == CharacterState.Jumping ||
                   targetState == CharacterState.Falling;
        }
    }
}

