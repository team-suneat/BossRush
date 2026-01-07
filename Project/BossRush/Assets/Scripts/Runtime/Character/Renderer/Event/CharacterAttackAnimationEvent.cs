using UnityEngine;

namespace TeamSuneat
{
    public class CharacterAttackAnimationEvent : MonoBehaviour
    {
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        /// <summary>
        /// 애니메이션 이벤트로 호출됩니다.
        /// </summary>
        private void StartBasicAttackAnimationEvent()
        {
            if (_character != null && _character.Attack != null)
            {
                _character.Attack.Activate();
            }
        }

        /// <summary>
        /// 애니메이션 이벤트로 호출됩니다.
        /// </summary>
        private void StartAttackAnimationEvent(string hitmarkNameString)
        {
            if (_character != null && _character.Attack != null)
            {
                _character.Attack.Activate(hitmarkNameString);
            }
        }

        /// <summary>
        /// 애니메이션 이벤트로 호출됩니다. 다음 콤보 입력 가능 시간을 활성화합니다.
        /// </summary>
        private void EnableComboInput()
        {
            if (_character?.StateMachine?.CurrentState == CharacterState.Attack)
            {
                if (_character.StateMachine is PlayerMovementStateMachine playerStateMachine)
                {
                    playerStateMachine.EnableComboInput();
                }
            }
        }

        /// <summary>
        /// 애니메이션 이벤트로 호출됩니다. 다음 콤보 입력 가능 시간을 비활성화합니다.
        /// </summary>
        private void DisableComboInput()
        {
            if (_character?.StateMachine?.CurrentState == CharacterState.Attack)
            {
                if (_character.StateMachine is PlayerMovementStateMachine playerStateMachine)
                {
                    playerStateMachine.DisableComboInput();
                }
            }
        }

        /// <summary>
        /// 애니메이션 이벤트로 호출됩니다. 공격 중 캐릭터의 방향 전환을 허용합니다.
        /// </summary>
        private void EnableFlipInput()
        {
            if (_character?.StateMachine?.CurrentState == CharacterState.Attack)
            {
                if (_character.StateMachine is PlayerMovementStateMachine playerStateMachine)
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
                if (_character.StateMachine is PlayerMovementStateMachine playerStateMachine)
                {
                    playerStateMachine.DisableFlipInput();
                }
            }
        }
    }
}