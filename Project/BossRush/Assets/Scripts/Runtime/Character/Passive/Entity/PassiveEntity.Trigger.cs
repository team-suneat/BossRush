using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using TeamSuneat.Data;


namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        private Dictionary<PassiveTriggers, PassiveTriggerReceiver> _triggerReceivers = new();

        public int TriggerCount => ProfileInfo.Statistics.GetTriggerCount(Name);

        private IEnumerator RegisterTriggers()
        {
            yield return null;

            if (TriggerSettings.IsValid())
            {
                RegisterTrigger(ref _triggerReceivers);
            }
        }

        private void RegisterTrigger(ref Dictionary<PassiveTriggers, PassiveTriggerReceiver> dictionary)
        {
            if (TriggerSettings == null || TriggerSettings.Trigger == PassiveTriggers.None)
            {
                return;
            }

            if (!dictionary.ContainsKey(TriggerSettings.Trigger))
            {
                PassiveTriggerReceiver receiver = CreatePassiveTriggerReceiver(TriggerSettings.Trigger);
                if (receiver != null)
                {
                    receiver.SetEntity(this);
                    receiver.SetPassiveAssetData(TriggerSettings, ConditionSettings, EffectSettings);
                    receiver.Initialize();
                    receiver.Activate();

                    dictionary.Add(TriggerSettings.Trigger, receiver);

                    LogInfo("Receiver를 새로 생성하고 등록합니다. 트리거: {0}", TriggerSettings.Trigger.ToLogString());
                }
                else
                {
                    LogError("receiver를 등록하지 못했습니다. 트리거: {0}", TriggerSettings.Trigger.ToLogString());
                }
            }
            else
            {
                PassiveTriggerReceiver receiver = dictionary[TriggerSettings.Trigger];
                receiver.SetEntity(this);
                receiver.SetPassiveAssetData(TriggerSettings, ConditionSettings, EffectSettings);
                receiver.Initialize();
                receiver.Activate();

                LogInfo("등록된 Receiver를 재설정합니다. 트리거: {0}", TriggerSettings.Trigger.ToLogString());
            }
        }

        private void UnregisterTriggers()
        {
            if (TriggerSettings.IsValid())
            {
                UnregisterTrigger(ref _triggerReceivers, TriggerSettings);
            }
        }

        private void UnregisterTrigger(ref Dictionary<PassiveTriggers, PassiveTriggerReceiver> dictionary, PassiveTriggerSettings triggerSettings)
        {
            if (!triggerSettings.IsValid() || triggerSettings.Trigger == PassiveTriggers.None)
            {
                return;
            }

            if (dictionary.ContainsKey(triggerSettings.Trigger))
            {
                dictionary[triggerSettings.Trigger].Deactivate();

                LogInfo("receiver를 등록해제합니다. 트리거: {0}", triggerSettings.Trigger.ToLogString());
            }
        }

        private PassiveTriggerReceiver CreatePassiveTriggerReceiver(PassiveTriggers passiveTrigger)
        {
            switch (passiveTrigger)
            {
                case PassiveTriggers.Activate: { return new PassiveTriggerActivate(); }
                case PassiveTriggers.AttackMonster: { return new PassiveTriggerAttackMonster(); }
                case PassiveTriggers.AttackMonsterCritical: { return new PassiveTriggerAttackMonsterCritical(); }
                case PassiveTriggers.AttackMonsterNonCritical: { return new PassiveTriggerAttackMonsterNonCritical(); }
                case PassiveTriggers.MonsterKill: { return new PassiveTriggerMonsterKill(); }
                case PassiveTriggers.CastSkill: { return new PassiveTriggerCastSkill(); }
                case PassiveTriggers.StartSkillCooldown: { return new PassiveTriggerStartSkillCooldown(); }
                case PassiveTriggers.StopSkillCooldown: { return new PassiveTriggerStopSkillCooldown(); }
                case PassiveTriggers.RefreshCooldownSkill: { return new PassiveTriggerRefreshCooldownSkill(); }
                case PassiveTriggers.AssignSkill: { return new PassiveTriggerAssignSkill(); }
                case PassiveTriggers.UnassignSkill: { return new PassiveTriggerUnassignSkill(); }
                                    
                case PassiveTriggers.ExecuteAttackTarget: { return new PassiveTriggerExecuteAttackTarget(); }
                case PassiveTriggers.ExecuteAttackArea: { return new PassiveTriggerExecuteAttackArea(); }
                case PassiveTriggers.ExecuteAttackProjectile: { return new PassiveTriggerExecuteAttackProjectile(); }

                case PassiveTriggers.PlayerHealed: { return new PassiveTriggerPlayerHealed(); }
                case PassiveTriggers.PlayerChangeLife: { return new PassiveTriggerPlayerChangeLife(); }
                case PassiveTriggers.PlayerChangeVitalResource: { return new PassiveTriggerPlayerVitalResourceChange(); }
                case PassiveTriggers.PlayerUseVitalResource: { return new PassiveTriggerPlayerUseVitalResource(); }
                case PassiveTriggers.PlayerBarrierCharge: { return new PassiveTriggerPlayerBarrierCharge(); }
                case PassiveTriggers.PlayerBarrierDestroy: { return new PassiveTriggerPlayerBarrierDestroy(); }

                case PassiveTriggers.PlayerChangeStat: { return new PassiveTriggerPlayerStatChange(); }
                case PassiveTriggers.PlayerAddStat: { return new PassiveTriggerPlayerAddStat(); }
                case PassiveTriggers.PlayerRemoveStat: { return new PassiveTriggerPlayerRemoveStat(); }
                case PassiveTriggers.RestoreMonsterVitalResource: { return new PassiveTriggerRestoreMonsterVitalResource(); }
                case PassiveTriggers.PlayerLevelUp: { return new PassiveTriggerLevelUpPlayer(); }
                case PassiveTriggers.PlayerOperateMapObject: { return new PassiveTriggerOperateObeject(); }
                case PassiveTriggers.PlayerMoveToStage: { return new PassiveTriggerMoveToStage(); }
                case PassiveTriggers.PlayerDodge: { return new PassiveTriggerPlayerDodge(); }
                case PassiveTriggers.PlayerDamaged: { return new PassiveTriggerPlayerDamaged(); }
                case PassiveTriggers.PlayerDamagedCritical: { return new PassiveTriggerPlayerDamagedCritical(); }
                case PassiveTriggers.PlayerDeathDefiance: { return new PassiveTriggerPlayerDeathDefiance(); }
                case PassiveTriggers.MonsterDamaged: { return new PassiveTriggerMonsterDamaged(); }
                case PassiveTriggers.DestroyRelic: { return new PassiveTriggerDestroyRelic(); }
                case PassiveTriggers.AddBuffToPlayer: { return new PassiveTriggerAddBuffToPlayer(); }
                case PassiveTriggers.RemoveBuffToPlayer: { return new PassiveTriggerRemoveBuffToPlayer(); }
                case PassiveTriggers.AddBuffToMonster: { return new PassiveTriggerAddBuffToMonster(); }
                case PassiveTriggers.RemoveBuffToMonster: { return new PassiveTriggerRemoveBuffToMonster(); }
                case PassiveTriggers.AddStateEffectToPlayer: { return new PassiveTriggerAddStateEffectToPlayer(); }
                case PassiveTriggers.RemoveStateEffectToPlayer: { return new PassiveTriggerRemoveStateEffectToPlayer(); }
                case PassiveTriggers.AddStateEffectToMonster: { return new PassiveTriggerAddStateEffectToMonster(); }
                case PassiveTriggers.RemoveStateEffectToMonster: { return new PassiveTriggerRemoveStateEffectToMonster(); }
                case PassiveTriggers.AddBuffStackToPlayer: { return new PassiveTriggerAddBuffStackToPlayer(); }
                case PassiveTriggers.AddBuffStackToMonster: { return new PassiveTriggerAddBuffStackToMonster(); }
                case PassiveTriggers.TakePotion: { return new PassiveTriggerTakePotion(); }
                case PassiveTriggers.UsePotion: { return new PassiveTriggerUsePotion(); }
                case PassiveTriggers.EatFood: { return new PassiveTriggerEatFood(); }
                case PassiveTriggers.RerollBuyItem: { return new PassiveTriggerRerollBuyItem(); }
                case PassiveTriggers.ThrowRelic: { return new PassiveTriggerThrowRelic(); }
                case PassiveTriggers.TakeRelicOperate: { return new PassiveTriggerTakeRelicOperate(); }
                case PassiveTriggers.UseCurrency: { return new PassiveTriggerUseCurrency(); }
                case PassiveTriggers.CurrencyChanged: { return new PassiveTriggerCurrencyChanged(); }

                default:
                    {
                        Log.Error("패시브 트리거를 등록하지 못했습니다. {0}", passiveTrigger.ToLogString());
                    }
                    break;
            }

            return null;
        }

        public void AddTriggerCount()
        {
            if (TriggerSettings.TriggerCount > 0)
            {
                if (ProfileInfo != null)
                {
                    ProfileInfo.Statistics.AddTrrigerCount(Name);

                    LogInfo("패시브 발동 횟수를 추가합니다. 트리거 횟수: {0}", TriggerCount.ToString());
                }
            }
        }

        private bool TryTrrigerCount(TriggerCheckerTypes type)
        {
            switch (type)
            {
                case TriggerCheckerTypes.FullPotion:
                    {
                        if (!ProfileInfo.Potion.CheckFullPotion1())
                        {
                            return false;
                        }
                    }
                    break;

                case TriggerCheckerTypes.EmptyPotion:
                    {
                        if (ProfileInfo.Potion.Potion1Count != 0)
                        {
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }

        public void ResetTriggerCount()
        {
            if (TriggerSettings.TriggerCount > 0)
            {
                if (ProfileInfo != null)
                {
                    ProfileInfo.Statistics.UnregisterTrriger(Name);
                }

                LogInfo("패시브 발동 횟수를 초기화합니다. 트리거 횟수: 0");
            }
        }
    }
}