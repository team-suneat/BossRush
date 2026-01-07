using UnityEngine;

namespace TeamSuneat
{
    public class AttackState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterPhysics _physics;
        private PlayerInput _input;
        private CharacterAnimator _animator;

        public AttackState(CharacterStateMachine stateMachine, CharacterPhysics physics, CharacterAnimator animator, PlayerInput input)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _animator = animator;
            _input = input;
        }

        public void OnEnter()
        {
            // 공격 애니메이션 트리거 활성화
            _animator?.PlayAttackAnimation("Attack");
        }

        public void OnUpdate()
        {
            // 입력 기반 전환은 Update에서 처리
        }

        public void OnFixedUpdate()
        {
            // 입력이나 물리가 없으면 업데이트 스킵
            if (_input == null || _physics == null)
            {
                return;
            }

            // 공격이 끝나면 (애니메이션이 끝나면) Idle/Walk/Falling로 전환
            if (_animator != null && !_animator.IsAttacking)
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
            // 공격 상태 종료 시 처리
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

