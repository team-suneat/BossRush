using UnityEngine;

namespace TeamSuneat
{
    public class DashState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private PlayerPhysics _physics;
        private PlayerInput _input;

        public DashState(CharacterStateMachine stateMachine, PlayerPhysics physics, PlayerInput input)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _input = input;
        }

        public void OnEnter()
        {
            // Dash 상태 진입 시 대시 방향은 CharacterStateMachine에서 계산되어 전달됨
            // 여기서는 RequestDash만 호출하지 않음 (CharacterStateMachine에서 이미 호출됨)
        }

        public void OnUpdate()
        {
            // 입력이나 물리가 없으면 업데이트 스킵
            if (_input == null || _physics == null)
            {
                return;
            }

            // 대시가 끝나면 Idle/Walk/Falling로 전환
            if (!_physics.IsDashing)
            {
                if (_physics.IsGrounded)
                {
                    if (Mathf.Abs(_input.HorizontalInput) > 0.01f)
                    {
                        _stateMachine.ChangeState(CharacterState.Walk);
                    }
                    else
                    {
                        _stateMachine.ChangeState(CharacterState.Idle);
                    }
                }
                else
                {
                    _stateMachine.ChangeState(CharacterState.Falling);
                }
                return;
            }
        }

        public void OnFixedUpdate()
        {
            // Dash 상태 FixedUpdate
        }

        public void OnExit()
        {
            // Dash 상태 종료 시 처리
        }

        public void OnJumpRequested()
        {
            // 대시 중에는 점프 무시
        }

        public void OnDashRequested(Vector2 direction)
        {
            // 대시 요청 처리 (이미 Dash 상태이므로 무시)
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // Dash에서 전환 가능한 상태
            return targetState == CharacterState.Idle ||
                   targetState == CharacterState.Walk ||
                   targetState == CharacterState.Falling;
        }
    }
}