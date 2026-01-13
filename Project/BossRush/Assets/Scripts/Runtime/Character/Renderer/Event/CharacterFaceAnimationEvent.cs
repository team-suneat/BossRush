using UnityEngine;

namespace TeamSuneat
{
    public class CharacterFaceAnimationEvent : MonoBehaviour
    {
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        private void z()
        {
            if (_character == null) return;
            if (_character.TargetCharacter == null) return;

            _character.FaceToTarget();
        }
    }
}