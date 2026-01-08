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

        private void CallParryAnimationEvent()
        {
            if (_character != null && _character.MyVital != null && _character.MyVital.Pulse != null)
            {
                _character.MyVital.Pulse.UseParry();
            }
        }
    }
}