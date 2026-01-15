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
        [SuffixLabel("패링 가능한 공격 준비 VFX (패링시 스턴)")]
        private GameObject _parryableWithStunAttackReadyVFX;

        [FoldoutGroup("#Attack Ready VFX")]
        [SerializeField]
        [SuffixLabel("패링 불가능한 공격 준비 VFX")]
        private GameObject _unparryableAttackReadyVFX;

        [FoldoutGroup("#Attack Ready VFX")]
        [SerializeField]
        [SuffixLabel("VFX 생성 위치 (null이면 캐릭터 위치)")]
        private Transform _vfxSpawnPoint;

        // 애니메이션 이벤트로 호출됩니다. hitmarkName을 통해 패링 타입을 자동 판단하여 VFX를 생성합니다.
        private void SpawnAttackReadyVFX(string hitmarkNameString = "")
        {
            if (_character == null || _character.Attack == null) return;

            ParryTypes parryType = DetermineParryType(hitmarkNameString);
            SpawnAttackReadyVFXInternal(parryType);
        }

        // 애니메이션 이벤트로 호출됩니다. 패링 가능한 공격 준비 VFX를 생성합니다.
        private void SpawnParryableAttackReadyVFX()
        {
            SpawnAttackReadyVFXInternal(ParryTypes.Parryable);
        }

        // 애니메이션 이벤트로 호출됩니다. 패링 가능한 공격 준비 VFX를 생성합니다 (패링시 스턴).
        private void SpawnParryableWithStunAttackReadyVFX()
        {
            SpawnAttackReadyVFXInternal(ParryTypes.ParryableWithStun);
        }

        // 애니메이션 이벤트로 호출됩니다. 패링 불가능한 공격 준비 VFX를 생성합니다.
        private void SpawnUnparryableAttackReadyVFX()
        {
            SpawnAttackReadyVFXInternal(ParryTypes.Unparryable);
        }

        private ParryTypes DetermineParryType(string hitmarkNameString)
        {
            if (string.IsNullOrEmpty(hitmarkNameString)) return ParryTypes.Parryable;

            HitmarkNames hitmarkName = DataConverter.ToEnum<HitmarkNames>(hitmarkNameString);
            if (hitmarkName == HitmarkNames.None) return ParryTypes.Parryable;

            AttackEntity attackEntity = _character.Attack.FindEntity(hitmarkName);
            if (attackEntity == null || attackEntity.AssetData == null) return ParryTypes.Parryable;

            return attackEntity.AssetData.ParryType;
        }

        private void SpawnAttackReadyVFXInternal(ParryTypes parryType)
        {
            if (_character == null) return;

            GameObject vfxPrefab = GetVFXPrefabByParryType(parryType);
            if (vfxPrefab == null) return;

            Vector3 spawnPosition = _vfxSpawnPoint != null ? _vfxSpawnPoint.position : _character.transform.position;
            VFXManager.Spawn(vfxPrefab, spawnPosition, true);
        }

        private GameObject GetVFXPrefabByParryType(ParryTypes parryType)
        {
            switch (parryType)
            {
                case ParryTypes.Parryable:
                    return _parryableAttackReadyVFX;

                case ParryTypes.ParryableWithStun:
                    return _parryableWithStunAttackReadyVFX;

                case ParryTypes.Unparryable:
                    return _unparryableAttackReadyVFX;

                default:
                    return _parryableAttackReadyVFX;
            }
        }
    }
}