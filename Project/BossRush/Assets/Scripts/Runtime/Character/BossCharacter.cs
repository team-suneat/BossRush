namespace TeamSuneat
{
    public class BossCharacter : MonsterCharacter
    {
        public override void BattleReady()
        {
            base.BattleReady();

            GlobalEvent<BossCharacter>.Send(GlobalEventType.BOSS_CHARACTER_BATTLE_READY, this);
        }

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            GlobalEvent<BossCharacter>.Send(GlobalEventType.BOSS_CHARACTER_DEATH, this);
        }
    }
}