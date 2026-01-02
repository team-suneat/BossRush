

namespace TeamSuneat.Passive
{
    public class PassiveTriggerMonsterKill : PassiveTriggerReceiver
    {
        private DamageResult damageResult = new DamageResult();
        protected override PassiveTriggers Trigger => PassiveTriggers.MonsterKill;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<DamageResult>.Register(GlobalEventType.PLAYER_CHARACTER_KILL_MONSTER, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<DamageResult>.Unregister(GlobalEventType.PLAYER_CHARACTER_KILL_MONSTER, OnGlobalEvent);
        }

        private void OnGlobalEvent(DamageResult damageResult)
        {
            this.damageResult = damageResult;

            if (TryExecute(damageResult))
            {
                ExecuteWithDamage(damageResult);
            }
        }
    }
}