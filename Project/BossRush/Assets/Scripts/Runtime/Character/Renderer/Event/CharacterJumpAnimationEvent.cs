using UnityEngine;

namespace TeamSuneat
{
    public class CharacterJumpAnimationEvent : MonoBehaviour
    {
        [SerializeField]
        private GameObject _dustVFXPrefab;
        private Character _character;

        [SerializeField]
        private FVNames[] _jumpForceVelocityNames;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        // 애니메이션 이벤트로 호출됩니다.
        private void CallJumpDustVFXAnimationEvent()
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

        // 애니메이션 이벤트로 호출됩니다.
        private void CallJumpAnimationEvent(int index)
        {
            if (_jumpForceVelocityNames.IsValid(index))
            {
                Data.ForceVelocityAssetData assetData = Data.ScriptableDataManager.Instance?.FindForceVelocityClone(_jumpForceVelocityNames[index]);
                if (!assetData.IsValid())
                {
                    Log.Warning("JumpForceVelocity 데이터를 찾을 수 없습니다. {0}", _jumpForceVelocityNames[index].ToLogString());
                    return;
                }
                _character.Physics.StartForceVelocity(assetData, _character.IsFacingRight);
            }
        }
    }
}