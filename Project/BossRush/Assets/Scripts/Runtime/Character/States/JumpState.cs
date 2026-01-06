using UnityEngine;

namespace TeamSuneat
{
    public class JumpState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterPhysics _physics;
        private PlayerInput _input;

        public JumpState(CharacterStateMachine stateMachine, CharacterPhysics physics, PlayerInput input)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _input = input;
        }

        public void OnEnter()
        {
            // Jumping 상태 진입 시 점프 실행
            // 점프 가능 여부 확인 후 실행
            if (_physics != null && _physics.RemainingJumps > 0)
            {
                _physics.ExecuteJump();
            }
        }

        public void OnUpdate()
        {
            // 입력 기반 전환은 Update에서 처리
            // 물리 변수 기반 전환은 OnFixedUpdate로 이동
        }

        public void OnFixedUpdate()
        {
            // 물리가 없으면 업데이트 스킵
            if (_physics == null)
            {
                return;
            }

            // 상승 속도가 0 이하이면 Falling로 전환 (물리 변수 기반 - FixedUpdate에서 처리)
            // 바닥 착지는 FallingState에서 처리 (단방향 플랫폼 통과 시 문제 방지)
            if (_physics.RigidbodyVelocity.y <= 0f)
            {
                _stateMachine.TransitionToState(CharacterState.Falling);
                return;
            }
        }

        public void OnExit()
        {
            // Jumping 상태 종료 시 처리
        }

        public void OnJumpRequested()
        {
            // 공중 점프 처리 (더블 점프 등)
            if (_physics != null && _physics.RemainingJumps > 0)
            {
                _physics.ExecuteJump();
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
            // Jumping에서 전환 가능한 상태
            return targetState == CharacterState.Falling ||
                   targetState == CharacterState.Idle ||
                   targetState == CharacterState.Walk ||
                   targetState == CharacterState.Dash;
        }
    }
}