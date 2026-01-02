using TeamSuneat.Data;
using TeamSuneat.Stage;


namespace TeamSuneat.Passive
{
    public class PassiveTriggerAttackMonster : PassiveTriggerReceiver
    {
        private DamageResult damageResult = new DamageResult();

        protected override PassiveTriggers Trigger => PassiveTriggers.AttackMonster;

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
            // 값싼 조건부터 빠르게 반환하여 불필요한 검사 최소화
            if (damageResult == null)
            {
                LogProgress("패시브 발동 실패: DamageResult가 null입니다.");
                return;
            }

            this.damageResult = damageResult;

            if (TryExecute(damageResult))
            {
                ExecuteWithDamage(damageResult);
            }
        }
    }
}