using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        void Awake()
        {
            Owner = this.FindFirstParentComponent<Character>();
            Collider = GetComponent<BoxCollider2D>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            Life?.RegisterOnDeathEvent(OnDeath);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            Life?.UnregisterOnDeathEvent(OnDeath);
        }

        public virtual void OnBattleReady()
        {
            Generate();

            Life?.Initialize();
            Mana?.Initialize();
            Barrier?.Initialize();

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

        public void OnLevelUp(StatSystem statSystem, int previousLife)
        {
            if (Life != null)
            {
                Life.RefreshMaxValue();

                if (previousLife > CurrentLife)
                {
                    // 레벨 업을 통해 능력치가 재조정됨에 따라 현재 생명력이 더 낮아졌다면, 되돌립니다.
                    CurrentLife = previousLife;
                }
            }
        }

        public void OnLevelDown()
        {
            if (Life != null)
            {
                Life.RefreshMaxValue();
            }
        }

        public bool CheckDamageImmunity(DamageResult damageResult)
        {
            if (Life.CheckInvulnerable())
            {
                Life.UseZero();
                return true;
            }

            return false;
        }

        public bool TakeDamage(DamageResult damageResult)
        {
            if (CurrentLife <= 0)
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
                Life.TakeDamage(damageResult, damageResult.Attacker);
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
            Life?.Heal(value);
        }

        public void RestoreMana(int value)
        {
            Mana?.AddCurrentValue(value);
        }

        public void Charge(int value)
        {
            Barrier?.AddCurrentValue(value);
        }

        public void AddCurrentValue(VitalConsumeTypes consumeType, float value)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.FixedLife:
                    {
                        Heal((int)value);
                    }
                    break;

                case VitalConsumeTypes.MaxLifePercent:
                    {
                        Heal(Mathf.RoundToInt(MaxLife * value));
                    }
                    break;

                case VitalConsumeTypes.CurrentLifePercent:
                    {
                        Heal(Mathf.RoundToInt(CurrentLife * value));
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
                case VitalConsumeTypes.FixedLife:
                case VitalConsumeTypes.MaxLifePercent:
                case VitalConsumeTypes.CurrentLifePercent:
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
                case VitalConsumeTypes.FixedLife:
                case VitalConsumeTypes.MaxLifePercent:
                case VitalConsumeTypes.CurrentLifePercent:
                    {
                        if (Life != null)
                        {
                            if (value > 0)
                            {
                                Life.Use(value, Owner, hitmarkAssetData.IgnoreDeathByConsume);
                                return;
                            }
                        }
                    }
                    break;
            }

            LogErrorUseBattleResource(hitmarkAssetData, value);
        }

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

                case VitalResourceTypes.Life:
                    if (Life != null)
                    {
                        return Life.Current;
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

                case VitalConsumeTypes.CurrentLifePercent:
                case VitalConsumeTypes.MaxLifePercent:
                case VitalConsumeTypes.FixedLife:
                    return CurrentLife;
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

                case VitalConsumeTypes.CurrentLifePercent:
                case VitalConsumeTypes.MaxLifePercent:
                case VitalConsumeTypes.FixedLife:
                    return MaxLife;
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

                case VitalResourceTypes.Life:
                    if (Life != null)
                    {
                        return Life.Rate;
                    }
                    break;
            }

            LogWarningFindResourceRate(resourceType);
            return 0f;
        }

        #endregion Get Value
    }
}