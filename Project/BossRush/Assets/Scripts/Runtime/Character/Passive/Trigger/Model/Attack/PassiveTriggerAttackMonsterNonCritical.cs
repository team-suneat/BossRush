

namespace TeamSuneat.Passive
{
    public class PassiveTriggerAttackMonsterNonCritical : PassiveTriggerReceiver
    {
        protected override PassiveTriggers Trigger => PassiveTriggers.AttackMonsterNonCritical;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<DamageResult>.Register(GlobalEventType.PLAYER_CHARACTER_ATTACK_MONSTER, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<DamageResult>.Unregister(GlobalEventType.PLAYER_CHARACTER_ATTACK_MONSTER, OnGlobalEvent);
        }

        private void OnGlobalEvent(DamageResult damageResult)
        {
            if (damageResult != null)
            {
                if (TryExecute(damageResult))
                {
                    ExecuteWithDamage(damageResult);
                }
            }
        }

        public override bool TryExecute(DamageResult damageResult)
        {
            if (damageResult.IsCritical)
            {
                LogFailedToExecute(damageResult);
                return false;
            }

            return base.TryExecute(damageResult);
        }

        private void LogFailedToExecute(DamageResult damageResult)
        {
            if (Log.LevelProgress)
            {
                LogProgress($"패시브 작동을 실패했습니다. 해당 공격({damageResult.HitmarkName.ToLogString()})이 치명타입니다.");
            }
        }
    }
}