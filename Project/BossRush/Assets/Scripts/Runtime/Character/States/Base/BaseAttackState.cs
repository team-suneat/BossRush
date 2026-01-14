using UnityEngine;

namespace TeamSuneat
{
    public abstract class BaseAttackState : ICharacterState
    {
        protected CharacterStateMachine _stateMachine;
        protected CharacterPhysics _physics;
        protected readonly Character _character;
        protected CharacterAnimator _animator;

        protected BaseAttackState(CharacterStateMachine stateMachine, CharacterPhysics physics, CharacterAnimator animator, Character character)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _animator = animator;
            _character = character;
        }

        public virtual void OnEnter()
        {
        }

        public abstract void OnUpdate();

        public abstract void OnFixedUpdate();

        public virtual void OnExit()
        {
            // 이동 해제
            _animator?.UnlockMovement();

            // flip 해제
            _animator?.UnlockFlip();
        }

        public void OnJumpRequested()
        {
            // 공격 중에는 점프 무시
        }

        public void OnDashRequested(Vector2 direction)
        {
            // 공격 중에는 대시 무시
        }

        public virtual bool CanTransitionTo(CharacterState targetState)
        {
            // 공격에서 전환 가능한 상태
            return targetState == CharacterState.Idle ||
                   targetState == CharacterState.Walk ||
                   targetState == CharacterState.Jumping ||
                   targetState == CharacterState.Falling;
        }

        public void EnableFlip()
        {
            _animator?.UnlockFlip();
        }

        public void DisableFlip()
        {
            _animator?.LockFlip();
        }

        protected void PlayAttackAnimation(int attackIndex)
        {
            // 공격 방향 처리
            Character.FacingDirections? targetDirection = GetAttackDirection();
            if (targetDirection.HasValue)
            {
                if (_stateMachine != null && _stateMachine.Character != null)
                {
                    _stateMachine.Character.ForceFace(targetDirection.Value);
                }
            }

            string animationName = $"Attack{attackIndex + 1}";
            _animator?.PlayAttackAnimation(animationName);
        }

        protected void PlayAirAttackAnimation()
        {
            // 공격 방향 처리
            Character.FacingDirections? targetDirection = GetAttackDirection();
            if (targetDirection.HasValue)
            {
                if (_stateMachine != null && _stateMachine.Character != null)
                {
                    _stateMachine.Character.ForceFace(targetDirection.Value);
                }
            }

            _animator?.PlayAttackAnimation("AirAttack");
        }

        protected virtual Character.FacingDirections? GetAttackDirection()
        {
            // 기본 구현: null 반환 (방향 변경 없음)
            // 플레이어/몬스터별로 오버라이드
            return null;
        }

        protected void TransitionToNextState()
        {
            if (_physics == null || _animator == null || _character == null)
            {
                return;
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
            }
        }
    }
}
