using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class EnemyAttackAnimationEvent : CharacterAttackAnimationEvent
    {
        [FoldoutGroup("#Attack Ready VFX")]
        [SerializeField]
        [SuffixLabel("패링 가능한 공격 준비 VFX")]
        private GameObject _parryableAttackReadyVFX;

        [FoldoutGroup("#Attack Ready VFX")]
        [SerializeField]
        [SuffixLabel("패링 불가능한 공격 준비 VFX")]
        private GameObject _unparryableAttackReadyVFX;

        [FoldoutGroup("#Attack Ready VFX")]
        [SerializeField]
        [SuffixLabel("VFX 생성 위치 (null이면 캐릭터 위치)")]
        private Transform _vfxSpawnPoint;

        // 애니메이션 이벤트로 호출됩니다. hitmarkName을 통해 패링 가능 여부를 자동 판단하여 VFX를 생성합니다.
        private void SpawnAttackReadyVFX(string hitmarkNameString = "")
        {
            if (_character == null || _character.Attack == null) return;

            bool isParryable = DetermineParryable(hitmarkNameString);
            SpawnAttackReadyVFXInternal(isParryable);
        }

        // 애니메이션 이벤트로 호출됩니다. 패링 가능한 공격 준비 VFX를 생성합니다.
        private void SpawnParryableAttackReadyVFX()
        {
            SpawnAttackReadyVFXInternal(true);
        }

        // 애니메이션 이벤트로 호출됩니다. 패링 불가능한 공격 준비 VFX를 생성합니다.
        private void SpawnUnparryableAttackReadyVFX()
        {
            SpawnAttackReadyVFXInternal(false);
        }

        private bool DetermineParryable(string hitmarkNameString)
        {
            if (string.IsNullOrEmpty(hitmarkNameString)) return true;

            HitmarkNames hitmarkName = DataConverter.ToEnum<HitmarkNames>(hitmarkNameString);
            if (hitmarkName == HitmarkNames.None) return true;

            AttackEntity attackEntity = _character.Attack.FindEntity(hitmarkName);
            if (attackEntity == null || attackEntity.AssetData == null) return true;

            return !attackEntity.AssetData.IgnoreParry;
        }

        private void SpawnAttackReadyVFXInternal(bool isParryable)
        {
            if (_character == null) return;

            GameObject vfxPrefab = isParryable ? _parryableAttackReadyVFX : _unparryableAttackReadyVFX;
            if (vfxPrefab == null) return;

            VFXManager.Spawn(vfxPrefab, _vfxSpawnPoint.position, true);
        }
    }
}