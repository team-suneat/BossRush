

namespace TeamSuneat.Passive
{
    public class PassiveTriggerAttackMonsterCritical : PassiveTriggerReceiver
    {
        private DamageResult damageResult = new DamageResult();

        protected override PassiveTriggers Trigger => PassiveTriggers.AttackMonsterCritical;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<DamageResult>.Register(GlobalEventType.PLAYER_CHARACTER_ATTACK_MONSTER_CRITICAL, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<DamageResult>.Unregister(GlobalEventType.PLAYER_CHARACTER_ATTACK_MONSTER_CRITICAL, OnGlobalEvent);
        }

        private void OnGlobalEvent(DamageResult damageResult)
        {
            if (damageResult != null)
            {
                this.damageResult = damageResult;

                if (TryExecute(damageResult))
                {
                    ExecuteWithDamage(damageResult);
                }
            }
        }

        public override bool TryExecute(DamageResult damageResult)
        {
            if (!damageResult.IsCritical)
            {
                return false;
            }

            return base.TryExecute(damageResult);
        }
    }
}