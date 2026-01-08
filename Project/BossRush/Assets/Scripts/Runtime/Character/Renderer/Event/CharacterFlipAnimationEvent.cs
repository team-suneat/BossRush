using UnityEngine;

namespace TeamSuneat
{
    public class CharacterFlipAnimationEvent : MonoBehaviour
    {
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        /// <summary>
        /// 애니메이션 이벤트로 호출됩니다. 공격 중 캐릭터의 방향 전환을 허용합니다.
        /// </summary>
        private void EnableFlipInput()
        {
            if (_character?.StateMachine?.CurrentState == CharacterState.Attack)
            {
                if (_character.StateMachine is PlayerStateMachine playerStateMachine)
                {
                    playerStateMachine.EnableFlipInput();
                }
            }
        }

        /// <summary>
        /// 애니메이션 이벤트로 호출됩니다. 공격 중 캐릭터의 방향 전환을 금지합니다.
        /// </summary>
        private void DisableFlipInput()
        {
            if (_character?.StateMachine?.CurrentState == CharacterState.Attack)
            {
                if (_character.StateMachine is PlayerStateMachine playerStateMachine)
                {
                    playerStateMachine.DisableFlipInput();
                }
            }
        }
    }
}

