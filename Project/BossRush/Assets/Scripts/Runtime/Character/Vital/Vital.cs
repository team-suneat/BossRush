using TeamSuneat.Data;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        protected override void OnStart()
        {
            base.OnStart();
            Health?.RegisterOnDeathEvent(OnDeath);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            Health?.UnregisterOnDeathEvent(OnDeath);
            UIManager.Instance?.GaugeManager?.UnregisterCharacter(this);
        }

        public virtual void OnBattleReady()
        {
            Generate();

            Health?.Initialize();
            Mana?.Initialize();
            Shield?.Initialize();

            if (UseSpawnGaugeOnInit)
            {
                SpawnCharacterGauge();
            }

            RegisterVital();
            StartRegenerate();
        }

        public void RegisterVital()
        {
            VitalManager.Instance?.Add(this);
        }

        public void UnregisterVital()
        {
            VitalManager.Instance?.Remove(this);
        }

        public void Despawn()
        {
            if (Owner != null)
            {
                Owner.Despawn();
            }
        }

        public void OnLevelUp(StatSystem statSystem, int previousHealth)
        {
            if (Health != null)
            {
                Health.RefreshMaxValue();

                if (previousHealth > CurrentHealth)
                {
                    // 레벨 업을 통해 능력치가 재조정됨에 따라 현재 생명력이 더 낮아졌다면, 되돌립니다.
                    CurrentHealth = previousHealth;
                }

                RefreshHealthGauge();
            }
        }

        public void OnLevelDown()
        {
            if (Health != null)
            {
                Health.RefreshMaxValue();
                RefreshHealthGauge();
            }
        }

        public bool CheckDamageImmunity(DamageResult damageResult)
        {
            if (Health.CheckInvulnerable())
            {
                Health.UseZero();
                return true;
            }
            else if (damageResult.IsEvasion)
            {
                Health.UseZero();
                return true;
            }

            return false;
        }

        public bool TakeDamage(DamageResult damageResult)
        {
            if (CurrentHealth <= 0)
            {
                LogWarning("캐릭터의 현재 체력이 0입니다. 피해를 받지 않습니다.");
                return false;
            }
            else if (damageResult.DamageValue <= 0)
            {
                LogWarning("설정된 피해가 0 또는 음수입니다. 피해를 받지 않습니다.");
                return false;
            }
            else if (damageResult.DamageValue > 0)
            {
                Health.TakeDamage(damageResult, damageResult.Attacker);
                SendGlobalEventOfDamaged(damageResult);

                return true;
            }
            else
            {
                LogErrorTakeDamageZero(damageResult.HitmarkName);
            }

            return false;
        }

        private void SendGlobalEventOfDamaged(DamageResult damageResult)
        {
            if (Owner == null)
            {
                return;
            }

            if (Owner.IsPlayer)
            {
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_DAMAGED, damageResult);
            }
            else if (Owner.IsBoss)
            {
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.BOSS_CHARACTER_DAMAGED, damageResult);
            }
            else
            {
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.MONSTER_CHARACTER_DAMAGED, damageResult);
            }
        }

        // Damage Hitmark On Hit

        private void ApplyDamageInfo(DamageResult damageResult, Vital targetVital)
        {
            switch (damageResult.DamageType)
            {
                case DamageTypes.Heal:
                    {
                        int healValue = Mathf.CeilToInt(damageResult.DamageValue);
                        targetVital.Heal(healValue);
                    }
                    break;

                case DamageTypes.RestoreMana:
                    {
                        int manaValue = Mathf.CeilToInt(damageResult.DamageValue);
                        targetVital.RestoreMana(manaValue);
                    }
                    break;

                case DamageTypes.ChargeShield:
                    {
                        int chargeValue = Mathf.CeilToInt(damageResult.DamageValue);
                        targetVital.Charge(chargeValue);
                    }
                    break;

                default:
                    {
                        if (!targetVital.CheckDamageImmunity(damageResult))
                        {
                            _ = targetVital.TakeDamage(damageResult);
                        }
                    }
                    break;
            }
        }

        // Event

        protected virtual void OnDeath(DamageResult damageResult)
        {
            DieEvent?.Invoke();
        }

        public void Heal(int value)
        {
            Health?.Heal(value);
        }

        public void RestoreMana(int value)
        {
            Mana?.AddCurrentValue(value);
        }

        public void Charge(int value)
        {
            Shield?.AddCurrentValue(value);
        }

        public void AddCurrentValue(VitalConsumeTypes consumeType, float value)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.FixedHealth:
                    {
                        Heal((int)value);
                    }
                    break;

                case VitalConsumeTypes.MaxHealthPercent:
                    {
                        Heal(Mathf.RoundToInt(MaxHealth * value));
                    }
                    break;

                case VitalConsumeTypes.CurrentHealthPercent:
                    {
                        Heal(Mathf.RoundToInt(CurrentHealth * value));
                    }
                    break;

                default:
                    {
                        LogErrorAddResource(consumeType, value);
                    }
                    break;
            }
        }

        public void AddCurrentValue(VitalConsumeTypes consumeType, int value)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.FixedHealth:
                case VitalConsumeTypes.MaxHealthPercent:
                case VitalConsumeTypes.CurrentHealthPercent:
                    {
                        Heal(value);
                    }
                    break;

                default:
                    {
                        LogErrorAddResource(consumeType, value);
                    }
                    break;
            }
        }

        public void UseCurrentValue(HitmarkAssetData hitmarkAssetData, int value)
        {
            switch (hitmarkAssetData.ResourceConsumeType)
            {
                case VitalConsumeTypes.FixedHealth:
                case VitalConsumeTypes.MaxHealthPercent:
                case VitalConsumeTypes.CurrentHealthPercent:
                    {
                        if (Health != null)
                        {
                            if (value > 0)
                            {
                                Health.Use(value, Owner, hitmarkAssetData.IgnoreDeathByConsume);
                                return;
                            }
                        }
                    }
                    break;
            }

            LogErrorUseBattleResource(hitmarkAssetData, value);
        }

        public virtual void ProcessAbility()
        { }

        public virtual bool CheckConsumingPotion()
        {
            return true;
        }

        #region Get Value

        public float GetCurrent(VitalResourceTypes resourceType)
        {
            switch (resourceType)
            {
                case VitalResourceTypes.None:
                    return 0;

                case VitalResourceTypes.Health:
                    if (Health != null)
                    {
                        return Health.Current;
                    }
                    break;
            }

            LogErrorFindCurrentResource(resourceType);
            return 0f;
        }

        public int GetCurrent(VitalConsumeTypes consumeType)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.None:
                    return 0;

                case VitalConsumeTypes.CurrentHealthPercent:
                case VitalConsumeTypes.MaxHealthPercent:
                case VitalConsumeTypes.FixedHealth:
                    return CurrentHealth;
            }

            LogErrorFindCurrentResource(consumeType);

            return 0;
        }

        public int GetMax(VitalConsumeTypes consumeType)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.None:
                    return 0;

                case VitalConsumeTypes.CurrentHealthPercent:
                case VitalConsumeTypes.MaxHealthPercent:
                case VitalConsumeTypes.FixedHealth:
                    return MaxHealth;
            }

            LogErrorFindMaxResource(consumeType);

            return 0;
        }

        public float GetRate(VitalResourceTypes resourceType)
        {
            switch (resourceType)
            {
                case VitalResourceTypes.None:
                    return 0;

                case VitalResourceTypes.Health:
                    if (Health != null)
                    {
                        return Health.Rate;
                    }
                    break;
            }

            LogWarningFindResourceRate(resourceType);
            return 0f;
        }

        #endregion Get Value
    }
}
