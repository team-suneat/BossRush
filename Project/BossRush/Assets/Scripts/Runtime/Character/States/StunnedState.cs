using UnityEngine;

namespace TeamSuneat
{
    public class StunnedState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private Character _character;

        public StunnedState(CharacterStateMachine stateMachine, Character character)
        {
            _stateMachine = stateMachine;
            _character = character;
        }

        public void OnEnter()
        {
            // Stunned 상태 진입 시 처리
        }

        public void OnUpdate()
        {
            // Stunned 상태에서는 이동 불가
            // 기절 시간이 끝나면 Idle로 전환 (추후 구현)
        }

        public void OnFixedUpdate()
        {
            // Stunned 상태 FixedUpdate
        }

        public void OnExit()
        {
            // Stunned 상태 종료 시 처리
        }

        public void OnJumpRequested()
        {
            // Stunned 상태에서는 이동 불가
        }

        public void OnDashRequested(Vector2 direction)
        {
            // Stunned 상태에서는 이동 불가
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // Stunned에서 전환 가능한 상태
            return targetState == CharacterState.Idle ||
                   targetState == CharacterState.Dead;
        }
    }
}