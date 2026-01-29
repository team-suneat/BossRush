using UnityEngine;

namespace TeamSuneat
{
    public class CharacterJumpAnimationEvent : MonoBehaviour
    {
        [SerializeField]
        private GameObject _dustVFXPrefab;
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        // 애니메이션 이벤트로 호출됩니다.
        private void CallJumpAnimationEvent()
        {
            if (_dustVFXPrefab != null)
            {
                VFXManager.Spawn(_dustVFXPrefab, _character.FootPoint, _character.IsFacingRight);
            }
        }
        // 애니메이션 이벤트로 호출됩니다.
        private void CallTargetJumpAnimationEvent()
        {
            if (_character is MonsterCharacter monsterCharacter && monsterCharacter.TargetJump != null)
            {
                monsterCharacter.TargetJump.ExecuteJump();
            }
        }
    }
}