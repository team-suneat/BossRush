using UnityEngine;

namespace TeamSuneat
{
    public class CharacterCastAnimationEvent : MonoBehaviour
    {
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        private void CallCastStartAnimationEvent()
        {
            if (_character?.CharacterAnimator is PlayerCharacterAnimator playerAnimator)
            {
                playerAnimator.SetCanCounterParryWhileCasting(true);
            }
        }

        private void CallCastEndAnimationEvent()
        {
            if (_character?.CharacterAnimator is PlayerCharacterAnimator playerAnimator)
            {
                playerAnimator.SetCanCounterParryWhileCasting(false);
            }
        }
    }
}