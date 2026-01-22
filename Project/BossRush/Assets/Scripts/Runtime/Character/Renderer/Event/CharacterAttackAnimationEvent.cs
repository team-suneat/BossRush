using UnityEngine;

namespace TeamSuneat
{
    public class CharacterAttackAnimationEvent : MonoBehaviour
    {
        protected Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
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
            if (_character == null) { return; }
            if (_character.StateMachine == null) { return; }
            if (_character.StateMachine is not PlayerStateMachine playerStateMachine) { return; }
            if (_character.StateMachine.CurrentState != CharacterState.Attack) { return; }

            playerStateMachine.EnableComboInput();
        }

        /// <summary>
        /// 애니메이션 이벤트로 호출됩니다. 다음 콤보 입력 가능 시간을 비활성화합니다.
        /// </summary>
        private void DisableComboInput()
        {
            if (_character == null) { return; }
            if (_character.StateMachine == null) { return; }
            if (_character.StateMachine is not PlayerStateMachine playerStateMachine) { return; }
            if (_character.StateMachine.CurrentState != CharacterState.Attack) { return; }

            playerStateMachine.DisableComboInput();
        }
    }
}