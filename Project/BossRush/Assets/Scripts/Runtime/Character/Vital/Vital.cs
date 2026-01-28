using TeamSuneat.Data;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
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
                GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_DAMAGED, damageResult);
            }
            else
            {
                GlobalEvent<DamageResult>.Send(GlobalEventType.MONSTER_CHARACTER_DAMAGED, damageResult);
            }
        }

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
                Mana.AddCurrentValue(value);
            }
        }

        public void Charge(int value)
        {
            if (Barrier != null)
            {
                Barrier.AddCurrentValue(value);
            }
        }

        public bool CanUse(VitalConsumeTypes consumeType, float cost)
        {
            // 비용이 0 이하이면 항상 사용 가능
            if (cost <= 0f)
            {
                return true;
            }

            switch (consumeType)
            {
                case VitalConsumeTypes.FixedResource:
                    {
                        if (Mana == null)
                        {
                            return false;
                        }
                        if (GameSetting.Instance.Cheat.IsNotCostResource)
                        {
                            return true;
                        }
                        if (Mana.Current >= cost)
                        {
                            return true;
                        }

                        return false;
                    }

                case VitalConsumeTypes.FixedPulse:
                    {
                        if (Pulse == null)
                        {
                            // ToDo: 펄스를 사용하게 된다면 false를 반환하도록
                            return true;
                        }
                        if (GameSetting.Instance.Cheat.IsNotCostPulse)
                        {
                            return true;
                        }
                        if (Pulse.Current >= cost && !Pulse.IsBurnout)
                        {
                            return true;
                        }
;
                        return false;
                    }

                default:
                    return true;
            }
        }

        public bool CanUseOrNotify(VitalConsumeTypes consumeType, float cost)
        {
            bool canUse = CanUse(consumeType, cost);
            switch (consumeType)
            {
                case VitalConsumeTypes.FixedResource:
                    {
                        if (!canUse)
                            ShowManaInsufficientToast();
                    }
                    break;

                case VitalConsumeTypes.FixedPulse:
                    {
                        if (!canUse)
                            ShowPulseInsufficientToast();
                    }
                    break;
            }

            return canUse;
        }

        private void ShowManaInsufficientToast()
        {
            UIManager.Instance?.NoticeManager?.ShowToast("마나가 부족합니다");
        }

        private void ShowPulseInsufficientToast()
        {
            UIManager.Instance?.NoticeManager?.ShowToast("펄스가 부족합니다");
        }

        //

        public void AddCurrentValue(VitalConsumeTypes consumeType, float value)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.FixedLife:
                    {
                        int valueToInt = Mathf.RoundToInt(value);
                        Heal(valueToInt);
                    }
                    break;

                case VitalConsumeTypes.FixedBarrier:
                    {
                        int valueToInt = Mathf.RoundToInt(value);
                        Charge(valueToInt);
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
                                Barrier.UseCurrentValue(value);
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
            }

            LogErrorUseBattleResource(hitmarkAssetData, value);
        }

        public void UseCurrentValue(VitalConsumeTypes resourceConsumeType, float value)
        {
            switch (resourceConsumeType)
            {
                case VitalConsumeTypes.FixedLife:
                    {
                        if (Life != null)
                        {
                            if (value > 0)
                            {
                                int valueToInt = Mathf.RoundToInt(value);
                                Life.Use(valueToInt, Owner, true);
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
                                int valueToInt = Mathf.RoundToInt(value);
                                Barrier.UseCurrentValue(valueToInt);
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
                                        return;
                                    }
                                }
                                return;
                            }
                        }
                    }
                    break;
            }
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