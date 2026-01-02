

namespace TeamSuneat.Passive
{
    public class PassiveTriggerMonsterDamaged : PassiveTriggerReceiver
    {
        private DamageResult damageResult = new DamageResult();
        protected override PassiveTriggers Trigger => PassiveTriggers.MonsterDamaged;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<DamageResult>.Register(GlobalEventType.MONSTER_CHARACTER_DAMAGED, OnGlobalEvent);
            GlobalEvent<DamageResult>.Register(GlobalEventType.BOSS_CHARACTER_DAMAGED, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<DamageResult>.Unregister(GlobalEventType.MONSTER_CHARACTER_DAMAGED, OnGlobalEvent);
            GlobalEvent<DamageResult>.Unregister(GlobalEventType.BOSS_CHARACTER_DAMAGED, OnGlobalEvent);
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