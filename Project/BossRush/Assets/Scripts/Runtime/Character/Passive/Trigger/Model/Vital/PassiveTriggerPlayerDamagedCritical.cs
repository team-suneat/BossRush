

namespace TeamSuneat.Passive
{
    public class PassiveTriggerPlayerDamagedCritical : PassiveTriggerReceiver
    {
        private DamageResult damageResult = new DamageResult();
        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerDamagedCritical;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<DamageResult>.Register(GlobalEventType.PLAYER_CHARACTER_DAMAGED, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<DamageResult>.Unregister(GlobalEventType.PLAYER_CHARACTER_DAMAGED, OnGlobalEvent);
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
            if (!damageResult.IsCritical)
            {
                return false;
            }

            if (damageResult.Attacker == damageResult.TargetCharacter)
            {
                LogFailedToExecute();
                return false;
            }

            return base.TryExecute(damageResult);
        }

        private void LogFailedToExecute()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동을 실패했습니다. 공격자와 피격자가 같을 때 패시브를 발동하지 않습니다. {0}", damageResult.Attacker.GetHierarchyName());
            }
        }
    }
}