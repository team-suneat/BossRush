using UnityEngine;

namespace TeamSuneat
{
    public class FallingState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterPhysics _physics;
        private readonly Character _character;

        public FallingState(CharacterStateMachine stateMachine, CharacterPhysics physics, Character character)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _character = character;
        }

        public void OnEnter()
        {
            // Falling 상태 진입 시 처리
        }

        public void OnUpdate()
        {
            // 입력 기반 전환은 Update에서 처리
            // 물리 변수 기반 전환은 OnFixedUpdate로 이동
        }

        public void OnFixedUpdate()
        {
            // 물리가 없으면 업데이트 스킵
            if (_physics == null || _character == null)
            {
                return;
            }

            // 바닥에 닿았고, 아래로 내려가는 중이 아니면 착지 처리
            if (_physics.IsGrounded && _physics.RigidbodyVelocity.y >= 0f)
            {
                // 착지 시 버퍼된 점프 실행 시도
                if (_physics.TryExecuteBufferedJump())
                {
                    // 버퍼된 점프가 성공적으로 실행되면 Jumping 상태로 전환
                    _stateMachine.TransitionToState(CharacterState.Jumping);
                    return;
                }

                // 점프가 실행되지 않았으면 점프 카운터 리셋 및 Idle/Walk로 전환
                _physics.ResetJumpCounterOnLanding();

                var cmd = _character.Command;
                if (Mathf.Abs(cmd.HorizontalInput) > 0.01f)
                {
                    _stateMachine.TransitionToState(CharacterState.Walk);
                }
                else
                {
                    _stateMachine.TransitionToState(CharacterState.Idle);
                }
                return;
            }
        }

        public void OnExit()
        {
            // Falling 상태 종료 시 처리
        }

        public void OnJumpRequested()
        {
            // 공중 점프 처리
            if (_physics != null && _physics.RemainingJumps > 0)
            {
                _stateMachine.ChangeState(CharacterState.Jumping);
            }
        }

        public void OnDashRequested(Vector2 direction)
        {
            // 공중 대시 처리
            if (_physics != null && _physics.CanDash && _physics.IsAirDashEnabled)
            {
                _stateMachine.ChangeState(CharacterState.Dash);
            }
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // Falling에서 전환 가능한 상태
            return targetState == CharacterState.Idle ||
                   targetState == CharacterState.Walk ||
                   targetState == CharacterState.Jumping ||
                   targetState == CharacterState.Dash;
        }
    }
}