using Lean.Pool;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        public bool TryAttack()
        {
            LoadAttackEntity();

            if (Attack == null)
            {
                return false;
            }

            Vital ownerVital = null;
            if (Owner != null)
            {
                LogInfo("버프를 가진 캐릭터를 목표로 설정합니다. 시전자:{0}, 버프 상태이상:{1}",
                     Caster.Name.ToLogString(), AssetData.StateEffect.ToLogString());

                ownerVital = Owner.MyVital;

                GlobalEvent<BuffNames, SID>.Send(GlobalEventType.MONSTER_CHARACTER_APPLY_ATTACK_BUFF, Name, Owner.SID);
            }

            Attack.SetOwner(Caster);
            Attack.SetTarget(ownerVital);
            Attack.Initialize();
            Attack.Activate();
            return true;
        }

        private void LoadAttackEntity()
        {
            if (Attack != null) { return; }
            if (AssetData.Hitmark == HitmarkNames.None) { return; }

            LogWarning("버프의 공격 독립체가 설정되지 않았습니다. Hitmark:{0}", AssetData.Hitmark.ToLogString());

            if (Owner == null || Owner.Attack == null)
            {
                return;
            }

            Attack = Owner.Attack.FindEntity(AssetData.Hitmark);
            if (Attack == null)
            {
                Attack = Owner.Attack.SpawnAndRegisterEntity(AssetData.Hitmark);
                if (Attack == null)
                {
                    HitmarkAssetData assetData = ScriptableDataManager.Instance.FindHitmarkClone(AssetData.Hitmark);
                    Attack = Owner.Attack.CreateAndRegisterEntity(assetData);
                }
            }
        }

        private void ApplyHealHealth()
        {
            if (Caster == null || Owner == null) return;

            _damageInfo.HitmarkAssetData = ScriptableDataManager.Instance.FindHitmarkClone(AssetData.Hitmark);
            if (!_damageInfo.HitmarkAssetData.IsValid())
            {
                LogError("버프의 피해 정보에서 히트마크 정보를 읽어올 수 없습니다:{0}, Hitmark:{1}", Name.ToLogString(), AssetData.Hitmark.ToLogString());
            }

            _damageInfo.SetAttacker(Caster);
            _damageInfo.SetTargetVital(Owner.MyVital);
            _damageInfo.SetStack(Stack);
            _damageInfo.SetLevel(Level);
            _damageInfo.Execute();

            DamageResult damageResult;
            for (int i = 0; i < _damageInfo.DamageResults.Count; i++)
            {
                damageResult = _damageInfo.DamageResults[i];
                if (damageResult.DamageType.IsHeal())
                {
                    Owner.MyVital.Heal(damageResult.DamageValueToInt);
                }
                else if (damageResult.DamageType.IsRestoreMana())
                {
                    Owner.MyVital.RestoreMana(damageResult.DamageValueToInt);
                }
                else if (damageResult.DamageType.IsCharge())
                {
                    Owner.MyVital.Charge(damageResult.DamageValueToInt);
                }
            }
        }

        public void ApplyDamage()
        {
            if (AssetData.Hitmark == HitmarkNames.None)
            {
                Log.Error("버프의 피해량 계산에 실패했습니다. Buff:{0}, Hitmark:{1}", Name.ToLogString(), AssetData.Hitmark.ToLogString());
                return;
            }

            _damageInfo.HitmarkAssetData = ScriptableDataManager.Instance.FindHitmarkClone(AssetData.Hitmark);
            if (!_damageInfo.HitmarkAssetData.IsValid())
            {
                LogError("버프의 피해 정보에서 히트마크 정보를 읽어올 수 없습니다:{0}, Hitmark:{1}", Name.ToLogString(), AssetData.Hitmark.ToLogString());
            }

            _damageInfo.SetAttacker(Caster);

            _damageInfo.SetTargetVital(Owner.MyVital);
            _damageInfo.SetStack(Stack);
            _damageInfo.Tick = Tick;
            _damageInfo.SetLevel(Level);
            _damageInfo.Execute();

            if (_damageInfo.DamageResults.IsValid())
            {
                DamageResult damageResult;
                for (int i = 0; i < _damageInfo.DamageResults.Count; i++)
                {
                    damageResult = _damageInfo.DamageResults[i];
                    if (damageResult.DamageType.IsHeal())
                    {
                        Owner.MyVital.Heal(damageResult.DamageValueToInt);
                        continue;
                    }

                    if (damageResult.DamageType.IsRestoreMana())
                    {
                        Owner.MyVital.RestoreMana(damageResult.DamageValueToInt);
                        continue;
                    }

                    if (damageResult.DamageType.IsCharge())
                    {
                        Owner.MyVital.Charge(damageResult.DamageValueToInt);
                        continue;
                    }

                    if (!Owner.MyVital.CheckDamageImmunity(damageResult))
                    {
                        Owner.MyVital.TakeDamage(damageResult);
                    }
                }
            }
            else
            {
                Log.Error("버프의 피해량 계산에 실패했습니다. Buff:{0}, Hitmark:{1}", Name.ToLogString(), AssetData.Hitmark.ToLogString());
            }
        }

        //────────────────────────────────────────────────────────────────────────────────

        public void ApplyInvulnerable()
        {
            Owner.MyVital.Health.SetTemporarilyInvulnerable(this);
        }

        public void ReleaseInvulnerable()
        {
            Owner.MyVital.Health.ResetTemporarilyInvulnerable(this);
        }

        //────────────────────────────────────────────────────────────────────────────────

        private void DeactivateAttack()
        {
            if (Attack != null)
            {
                Attack.Deactivate();
            }
        }

        private void DeactivateTick()
        {
            Tick = 0;
        }
    }
}