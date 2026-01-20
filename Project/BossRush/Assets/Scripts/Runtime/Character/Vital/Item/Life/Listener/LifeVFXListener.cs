using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// Life의 데미지/죽음 이벤트를 구독하여 VFX를 생성하는 Listener입니다.
    /// </summary>
    public sealed class LifeVFXListener : XBehaviour
    {
        private Life _life;

        private void Awake()
        {
            _life = GetComponentInParent<Life>();
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
            if (_life == null)
            {
                return;
            }

            _life.OnDamage += HandleDamageVFX;
            _life.OnDeath += HandleDeathVFX;
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            if (_life == null)
            {
                return;
            }

            _life.OnDamage -= HandleDamageVFX;
            _life.OnDeath -= HandleDeathVFX;
        }

        private void HandleDamageVFX(DamageResult damage)
        {
            if (damage == null)
            {
                return;
            }

            Vector3 damagePosition = damage.DamagePosition;

            SpawnDamageVFX(damage, damagePosition);
            SpawnElementalDamageVFX(damage, damagePosition);

            if (damage.DamageType.IsInstantDamage())
            {
                SpawnDamageBloodVFX(damagePosition);
            }

            SpawnHitFX(damage, damagePosition);
        }

        private void HandleDeathVFX(DamageResult damage)
        {
            // 죽음 시 전용 VFX가 필요하다면 이곳에서 처리
        }

        private void SpawnElementalDamageVFX(DamageResult damageResult, Vector3 damagePosition)
        {
            switch (damageResult.DamageType)
            {
                case DamageTypes.DamageOverTime:
                    _ = VFXManager.Spawn("fx_damage_blood", damagePosition, true);
                    break;
            }
        }

        private void SpawnDamageBloodVFX(Vector3 damagePosition)
        {
            if (_life == null || _life.Vital == null || _life.Vital.Owner == null)
            {
                return;
            }

            if (_life.Vital.Owner.IsPlayer)
            {
                _ = VFXManager.Spawn("fx_player_damage_blood", damagePosition, true);
            }
        }

        private void SpawnDamageVFX(DamageResult damageResult, Vector3 damagePosition)
        {
            if (!damageResult.DamageType.IsInstantDamage())
            {
                return;
            }

            if (_life == null || _life.Vital == null)
            {
                Log.Error("피격 이펙트를 생성할 수 없습니다. 바이탈 또는 캐릭터가 설정되지 않았습니다. {0}", this.GetHierarchyPath());
                return;
            }

            // Todo: 예리함/둔기류 피격 이펙트 추가
        }

        private void SpawnHitFX(DamageResult damageResult, Vector3 damagePosition)
        {
            if (damageResult.Asset == null)
            {
                return;
            }

            if (_life == null || _life.Vital == null || _life.Vital.Owner == null)
            {
                return;
            }

            string prefabName = damageResult.Asset.HitFXPrefabName;

            if (string.IsNullOrEmpty(prefabName))
            {
                return;
            }

            bool isFacingRight = true;
            if (damageResult.TargetCharacter != null)
            {
                isFacingRight = damageResult.TargetCharacter.IsFacingRight;
            }

            _ = VFXManager.Spawn(prefabName, damagePosition, isFacingRight);
        }
    }
}