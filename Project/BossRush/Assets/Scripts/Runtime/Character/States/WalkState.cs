using UnityEngine;

namespace TeamSuneat
{
    public class WalkState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterPhysics _physics;
        private PlayerInput _input;

        public WalkState(CharacterStateMachine stateMachine, CharacterPhysics physics, PlayerInput input)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _input = input;
        }

        public void OnEnter()
        {
            // Walk 상태 진입 시 처리
        }

        public void OnUpdate()
        {
            // 입력이나 물리가 없으면 업데이트 스킵
            if (_input == null || _physics == null)
            {
                return;
            }

            // 수평 입력이 없으면 Idle로 전환 (입력 기반 - Update에서 처리)
            if (Mathf.Abs(_input.HorizontalInput) < 0.01f)
            {
                _stateMachine.TransitionToState(CharacterState.Idle);
                return;
            }
        }

        public void OnFixedUpdate()
        {
            // 입력이나 물리가 없으면 업데이트 스킵
            if (_input == null || _physics == null)
            {
                return;
            }

            // 공중에 떠있으면 Falling로 전환 (물리 변수 기반 - FixedUpdate에서 처리)
            if (!_physics.IsGrounded)
            {
                _stateMachine.TransitionToState(CharacterState.Falling);
                return;
            }
        }

        public void OnExit()
        {
            // Walk 상태 종료 시 처리
        }

        public void OnJumpRequested()
        {
            // 점프 요청 시 Jumping 상태로 전환
            if (_physics != null && _physics.IsGrounded)
            {
                _stateMachine.ChangeState(CharacterState.Jumping);
            }
        }

        public void OnDashRequested(Vector2 direction)
        {
            // 대시 요청 시 Dash 상태로 전환
            if (_physics != null && _physics.CanDash)
            {
                _stateMachine.ChangeState(CharacterState.Dash);
            }
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // Walk에서 전환 가능한 상태
            return targetState == CharacterState.Idle ||
                   targetState == CharacterState.Jumping ||
                   targetState == CharacterState.Dash ||
                   targetState == CharacterState.Falling;
        }
    }
}