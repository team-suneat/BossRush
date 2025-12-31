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
                _character.Attack.ActivateBasic();
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
    }
}