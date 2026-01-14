using UnityEngine;

namespace TeamSuneat
{
    public class CharacterParryAnimationEvent : MonoBehaviour
    {
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        private void CallReleaseParryAnimationEvent()
        {
            if (_character?.CharacterAnimator != null)
            {
                _character.CharacterAnimator.ReleaseParryState();
            }
        }
    }
}