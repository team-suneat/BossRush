using UnityEngine;

namespace TeamSuneat
{
    public class DeadState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private Character _character;

        public DeadState(CharacterStateMachine stateMachine, Character character)
        {
            _stateMachine = stateMachine;
            _character = character;
        }

        public void OnEnter()
        {
            // Dead 상태 진입 시 처리
        }

        public void OnUpdate()
        {
            // Dead 상태에서는 아무것도 하지 않음
        }

        public void OnFixedUpdate()
        {
            // Dead 상태 FixedUpdate
        }

        public void OnExit()
        {
            // Dead 상태 종료 시 처리 (부활 등)
        }

        public void OnJumpRequested()
        {
            // Dead 상태에서는 이동 불가
        }

        public void OnDashRequested(Vector2 direction)
        {
            // Dead 상태에서는 이동 불가
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // Dead에서 전환 가능한 상태 (부활 시 Idle로 전환 가능)
            return targetState == CharacterState.Idle;
        }
    }
}