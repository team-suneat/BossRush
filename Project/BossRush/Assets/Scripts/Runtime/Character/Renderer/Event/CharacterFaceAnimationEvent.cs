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

        // 애니메이션 이벤트로 호출됩니다.
        private void CallFaceAnimationEvent()
        {
            if (_character == null)
            {
                return;
            }

            if (_character.TargetCharacter == null)
            {
                return;
            }

            _character.FaceToTarget();
        }
    }
}