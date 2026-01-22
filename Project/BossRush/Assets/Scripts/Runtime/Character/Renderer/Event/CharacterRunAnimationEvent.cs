using TeamSuneat.Audio;
using UnityEngine;

namespace TeamSuneat
{
    public class CharacterRunAnimationEvent : MonoBehaviour
    {
        [SerializeField]
        private GameObject _dustVFXPrefab;
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        // 애니메이션 이벤트로 호출됩니다.
        private void CallRunAnimationEvent()
        {
            if (_dustVFXPrefab != null)
            {
                VFXManager.Spawn(_dustVFXPrefab, _character.FootPoint, _character.IsFacingRight);
            }

            AudioManager.Instance.PlaySFXOneShotUnscaled(SoundNames.Movement_Run);
        }
    }
}