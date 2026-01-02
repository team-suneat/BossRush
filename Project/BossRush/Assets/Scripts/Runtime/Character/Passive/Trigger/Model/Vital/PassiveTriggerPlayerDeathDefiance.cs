

namespace TeamSuneat.Passive
{
    public class PassiveTriggerPlayerDeathDefiance : PassiveTriggerReceiver
    {
        private DamageResult damageResult = new DamageResult();
        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerDeathDefiance;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<DamageResult>.Register(GlobalEventType.PLAYER_CHARACTER_DEATH_DEFIANCE, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<DamageResult>.Unregister(GlobalEventType.PLAYER_CHARACTER_DEATH_DEFIANCE, OnGlobalEvent);
        }

        private void OnGlobalEvent(DamageResult damageResult)
        {
            this.damageResult = damageResult;

            if (TryExecute(damageResult))
            {
                ExecuteWithDamage(damageResult);
            }
        }

        public override bool TryExecute(DamageResult damageResult)
        {
            if (damageResult != null)
            {
                if (!_effectSettings.UseApplySelf && damageResult.Attacker == damageResult.TargetCharacter)
                {
                    LogFailedToExecute();
                    return false;
                }
            }

            return base.TryExecute(damageResult);
        }

        private void LogFailedToExecute()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 발동을 실패했습니다. 공격자와 피격자가 같아서 패시브를 발동할 수 없습니다. {0}", damageResult.Attacker.GetHierarchyName());
            }
        }
    }
}