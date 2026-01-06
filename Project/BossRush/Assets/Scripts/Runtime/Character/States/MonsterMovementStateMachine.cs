using System.Collections.Generic;

namespace TeamSuneat
{
    public class MonsterMovementStateMachine : CharacterStateMachine
    {
        protected override void InitializeStates()
        {
            _states = new Dictionary<CharacterState, ICharacterState>
            {
                // 이동 상태 (기본만 구현, 나중에 확장 가능)
                { CharacterState.Idle, new IdleState(this, null, null) },

                // 조건 상태
                { CharacterState.Dead, new DeadState(this, _character) },
                { CharacterState.Stunned, new StunnedState(this, _character) },
                { CharacterState.ControlledMovement, new ControlledMovementState(this, _character) }
            };

            // 초기 상태 설정
            ChangeState(CharacterState.Idle);
        }

        protected override void HandleInput()
        {
            // AI 입력 처리 (나중에 구현)
            // 현재는 입력 없음
        }
    }
}