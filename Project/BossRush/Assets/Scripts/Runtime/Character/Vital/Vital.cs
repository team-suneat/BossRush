using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        private void Awake()
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
            Pulse?.StopRegenerate();
        }

        public virtual void OnBattleReady()
        {
            Generate();

            Life?.Initialize();
            Mana?.Initialize();
            Barrier?.Initialize();

            RegisterVital();
        }

        public void Despawn()
        {
            if (Owner != null)
            {
                Owner.Despawn();
            }
        }

        //

        public void RegisterVital()
        {
            VitalManager.Instance?.Add(this);
        }

        public void UnregisterVital()
        {
            VitalManager.Instance?.Remove(this);
        }

        //

        public bool CheckDamageImmunity(DamageResult damageResult)
        {
            if (Life.CheckInvulnerable())
            {
                Life.HandleDamageZero();
                return true;
            }

            return false;
        }

        public bool TakeDamage(DamageResult damageResult)
        {
            if (GetCurrent(VitalResourceTypes.Life) <= 0)
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

        //

        // Event

        protected virtual void OnDeath(DamageResult damageResult)
        {
            DieEvent?.Invoke();
        }

        public void Heal(int value)
        {
            if (Life != null)
            {
                Life.Heal(value);
            }
        }

        public void RestoreMana(int value)
        {
            if (Mana != null)
            {
                _ = Mana.AddCurrentValue(value);
            }
        }

        public void Charge(int value)
        {
            if (Barrier != null)
            {
                _ = Barrier.AddCurrentValue(value);
            }
        }

        public bool UseParry()
        {
            if (Pulse == null)
            {
                return false;
            }

            return Pulse.UseCurrentValue();
        }

        public bool UseDash()
        {
            if (Pulse == null)
            {
                return false;
            }

            return Pulse.UseCurrentValue();
        }

        //

        public void AddCurrentValue(VitalConsumeTypes consumeType, int value)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.FixedLife:
                    {
                        Heal(value);
                    }
                    break;

                case VitalConsumeTypes.FixedBarrier:
                    {
                        Charge(value);
                    }
                    break;

                case VitalConsumeTypes.FixedResource:
                    {
                        if (Mana != null)
                        {
                            float gainAmount = Mathf.Clamp01(value);
                            Mana.OnAttackSuccess(gainAmount);
                        }
                    }
                    break;

                case VitalConsumeTypes.FixedPulse:
                    {
                        if (Pulse != null)
                        {
                            float gainAmount = Mathf.Clamp01(value);
                            Pulse.OnAttackSuccess(gainAmount);
                        }
                    }
                    break;

                case VitalConsumeTypes.FixedResourceAndPulse:
                    {
                        float gainAmount = Mathf.Clamp01(value);
                        if (Mana != null)
                        {
                            Mana.OnAttackSuccess(gainAmount);
                        }
                        if (Pulse != null)
                        {
                            Pulse.OnAttackSuccess(gainAmount);
                        }
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

                case VitalConsumeTypes.FixedBarrier:
                    {
                        if (Barrier != null)
                        {
                            if (value > 0)
                            {
                                _ = Barrier.UseCurrentValue(value);
                                return;
                            }
                        }
                    }
                    break;

                case VitalConsumeTypes.FixedResource:
                    {
                        if (Mana != null)
                        {
                            if (value > 0)
                            {
                                // 온전한 마나를 value 개수만큼 사용
                                for (int i = 0; i < value; i++)
                                {
                                    if (!Mana.TryUseFullMana())
                                    {
                                        LogErrorUseBattleResource(hitmarkAssetData, value);
                                        return;
                                    }
                                }
                                return;
                            }
                        }
                    }
                    break;

                case VitalConsumeTypes.FixedPulse:
                    {
                        if (Pulse != null)
                        {
                            if (value > 0)
                            {
                                // 온전한 펄스를 value 개수만큼 사용
                                for (int i = 0; i < value; i++)
                                {
                                    if (!Pulse.UseCurrentValue())
                                    {
                                        LogErrorUseBattleResource(hitmarkAssetData, value);
                                        return;
                                    }
                                }
                                return;
                            }
                        }
                    }
                    break;

                case VitalConsumeTypes.FixedResourceAndPulse:
                    {
                        // 마나와 펄스를 각각 value 개수만큼 사용
                        if (Mana != null)
                        {
                            if (value > 0)
                            {
                                for (int i = 0; i < value; i++)
                                {
                                    if (!Mana.TryUseFullMana())
                                    {
                                        LogErrorUseBattleResource(hitmarkAssetData, value);
                                        return;
                                    }
                                }
                            }
                        }
                        if (Pulse != null)
                        {
                            if (value > 0)
                            {
                                for (int i = 0; i < value; i++)
                                {
                                    if (!Pulse.UseCurrentValue())
                                    {
                                        LogErrorUseBattleResource(hitmarkAssetData, value);
                                        return;
                                    }
                                }
                            }
                        }
                        return;
                    }
            }

            LogErrorUseBattleResource(hitmarkAssetData, value);
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

                case VitalResourceTypes.Barrier:
                    if (Barrier != null)
                    {
                        return Barrier.Current;
                    }
                    break;

                case VitalResourceTypes.Mana:
                    if (Mana != null)
                    {
                        return Mana.Current;
                    }
                    break;

                case VitalResourceTypes.Pulse:
                    if (Pulse != null)
                    {
                        return Pulse.Current;
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

                case VitalConsumeTypes.FixedLife:
                    return Life != null ? Life.Current : 0;

                case VitalConsumeTypes.FixedBarrier:
                    return Barrier != null ? Barrier.Current : 0;

                case VitalConsumeTypes.FixedResource:
                    return Mana != null ? Mana.Current : 0;

                case VitalConsumeTypes.FixedPulse:
                    return Pulse != null ? Pulse.Current : 0;

                case VitalConsumeTypes.FixedResourceAndPulse:
                    // 마나와 펄스 중 작은 값을 반환 (둘 다 사용해야 하므로)
                    int manaCurrent = Mana != null ? Mana.Current : int.MaxValue;
                    int pulseCurrent = Pulse != null ? Pulse.Current : int.MaxValue;
                    return Mathf.Min(manaCurrent, pulseCurrent);
            }

            LogErrorFindCurrentResource(consumeType);

            return 0;
        }

        public float GetMax(VitalResourceTypes resourceType)
        {
            switch (resourceType)
            {
                case VitalResourceTypes.None:
                    return 0;

                case VitalResourceTypes.Life:
                    if (Life != null)
                    {
                        return Life.Max;
                    }
                    break;

                case VitalResourceTypes.Barrier:
                    if (Barrier != null)
                    {
                        return Barrier.Max;
                    }
                    break;

                case VitalResourceTypes.Mana:
                    if (Mana != null)
                    {
                        return Mana.Max;
                    }
                    break;

                case VitalResourceTypes.Pulse:
                    if (Pulse != null)
                    {
                        return Pulse.Max;
                    }
                    break;
            }

            return 0f;
        }

        public int GetMax(VitalConsumeTypes consumeType)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.None:
                    return 0;

                case VitalConsumeTypes.FixedLife:
                    return Life != null ? Life.Max : 0;

                case VitalConsumeTypes.FixedBarrier:
                    return Barrier != null ? Barrier.Max : 0;

                case VitalConsumeTypes.FixedResource:
                    return Mana != null ? Mana.Max : 0;

                case VitalConsumeTypes.FixedPulse:
                    return Pulse != null ? Pulse.Max : 0;

                case VitalConsumeTypes.FixedResourceAndPulse:
                    // 마나와 펄스 중 작은 값을 반환
                    int manaMax = Mana != null ? Mana.Max : int.MaxValue;
                    int pulseMax = Pulse != null ? Pulse.Max : int.MaxValue;
                    return Mathf.Min(manaMax, pulseMax);
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

                case VitalResourceTypes.Barrier:
                    if (Barrier != null)
                    {
                        return Barrier.Rate;
                    }
                    break;

                case VitalResourceTypes.Mana:
                    if (Mana != null)
                    {
                        return Mana.Rate;
                    }
                    break;

                case VitalResourceTypes.Pulse:
                    if (Pulse != null)
                    {
                        return Pulse.Rate;
                    }
                    break;
            }

            LogWarningFindResourceRate(resourceType);
            return 0f;
        }

        #endregion Get Value
    }
}