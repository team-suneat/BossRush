using UnityEngine;

namespace TeamSuneat
{
    public class ParryState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterPhysics _physics;
        private readonly Character _character;
        private CharacterAnimator _animator;

        public ParryState(CharacterStateMachine stateMachine, CharacterPhysics physics, CharacterAnimator animator, Character character)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _animator = animator;
            _character = character;
        }

        public void OnEnter()
        {
            // 펄스 소모
            if (_character != null && _character.MyVital != null && _character.MyVital.Pulse != null)
            {
                _character.MyVital.UseParry();
            }

            // 이동 및 방향 전환 잠금
            _animator?.LockMovement();
            _animator?.LockFlip();

            // 패리 애니메이션 재생
            _animator?.PlayParryAnimation();
        }

        public void OnUpdate()
        {
            // 패리 애니메이션 중에는 입력 처리 없음
        }

        public void OnFixedUpdate()
        {
            if (_physics == null || _animator == null || _character == null)
            {
                return;
            }

            // 애니메이션 종료 시 자동으로 Idle/Walk로 전환
            if (!_animator.IsParrying)
            {
                CharacterCommand cmd = _character.Command;
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
            // 이동 및 방향 전환 잠금 해제
            _animator?.UnlockMovement();
            _animator?.UnlockFlip();
        }

        public void OnJumpRequested()
        {
            // 패리 중에는 점프 무시
        }

        public void OnDashRequested(Vector2 direction)
        {
            // 패리 중에는 대시 무시
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // 패리에서 전환 가능한 상태
            return targetState == CharacterState.Idle ||
                   targetState == CharacterState.Walk ||
                   targetState == CharacterState.Jumping ||
                   targetState == CharacterState.Falling;
        }
    }
}