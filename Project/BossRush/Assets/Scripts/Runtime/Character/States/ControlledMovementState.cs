using UnityEngine;

namespace TeamSuneat
{
    public class ControlledMovementState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private Character _character;

        public ControlledMovementState(CharacterStateMachine stateMachine, Character character)
        {
            _stateMachine = stateMachine;
            _character = character;
        }

        public void OnEnter()
        {
            // ControlledMovement 상태 진입 시 처리
        }

        public void OnUpdate()
        {
            // ControlledMovement 상태에서는 조작된 움직임 처리
            // 조작이 끝나면 Idle로 전환 (추후 구현)
        }

        public void OnFixedUpdate()
        {
            // ControlledMovement 상태 FixedUpdate
        }

        public void OnExit()
        {
            // ControlledMovement 상태 종료 시 처리
        }

        public void OnJumpRequested()
        {
            // ControlledMovement 상태에서는 일반 이동 불가
        }

        public void OnDashRequested(Vector2 direction)
        {
            // ControlledMovement 상태에서는 일반 이동 불가
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // ControlledMovement에서 전환 가능한 상태
            return targetState == CharacterState.Idle ||
                   targetState == CharacterState.Dead ||
                   targetState == CharacterState.Stunned;
        }
    }
}