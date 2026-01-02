

namespace TeamSuneat
{
    public partial class AttackProjectileEntity : AttackEntity
    {
        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent<Character>.Register(GlobalEventType.MONSTER_CHARACTER_ATTACK_CAST, OnMonsterCharacterCastAttack);
            RegisterStatEventOfBaseProjectile();
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent<Character>.Unregister(GlobalEventType.MONSTER_CHARACTER_ATTACK_CAST, OnMonsterCharacterCastAttack);
            UnregisterStatEventOfBaseProjectile();
        }

        private void OnMonsterCharacterCastAttack(Character attacker)
        {
            AttackMonster = attacker;
        }

        private void OnRefreshStatProjectileCountExtraShots(StatNames statName, float addStatValue)
        {
            if (StatNameOfExtraShots == statName)
            {
                SetBaseProjectilePerShotAddedExtraShot();
            }
        }

        private void OnRefreshStatProjectileCountPerShot(StatNames statName, float addStatValue)
        {
            if (StatNameOfPerShot == statName)
            {
                SetBaseProjectilePerShotToStat();
            }
        }
    }
}