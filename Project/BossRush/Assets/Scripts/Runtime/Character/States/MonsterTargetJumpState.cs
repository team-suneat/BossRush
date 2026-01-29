using UnityEngine;

namespace TeamSuneat
{
    public class MonsterTargetJumpState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterPhysics _physics;
        private readonly Character _character;

        public MonsterTargetJumpState(CharacterStateMachine stateMachine, CharacterPhysics physics, Character character)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _character = character;
        }

        public void OnEnter()
        {
            // 점프 상태 진입 시 특별한 처리 없음 (실제 점프는 TargetJumpSystem에서 애니메이션 이벤트로 실행)
        }

        public void OnUpdate()
        {
            // 입력 기반 전환은 Update에서 처리
        }

        public void OnFixedUpdate()
        {
            // 물리가 없으면 업데이트 스킵
            if (_physics == null)
            {
                return;
            }

            // 캐릭터가 살아있지 않으면 업데이트 스킵
            if (!_character.IsAlive)
            {
                return;
            }

            // 상승 속도가 0 이하이면 Falling로 전환
            // 착지는 TargetJumpSystem 코루틴에서 감지
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
            // 점프 중에는 무시
        }

        public void OnDashRequested(Vector2 direction)
        {
            // 점프 중에는 무시
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // Jumping에서 전환 가능한 상태
            return targetState == CharacterState.Falling ||
                   targetState == CharacterState.Idle ||
                   targetState == CharacterState.Walk;
        }
    }
}