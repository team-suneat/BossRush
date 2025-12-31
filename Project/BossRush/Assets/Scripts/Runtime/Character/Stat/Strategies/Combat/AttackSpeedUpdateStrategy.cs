namespace TeamSuneat
{
    public class AttackSpeedUpdateStrategy : BaseStatUpdateStrategy
    {
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshAttackSpeed(System);
        }

        private void RefreshAttackSpeed(StatSystem StatSystem)
        {
            float attackSpeed = StatSystem.FindValueOrDefault(StatNames.AttackSpeed);
            LogStatUpdate(StatNames.AttackSpeed, attackSpeed);

            if (System.Owner.CharacterAnimator != null)
            {
                System.Owner.CharacterAnimator.UpdateAttackSpeed(attackSpeed);
            }
        }
    }
}