namespace TeamSuneat
{
    public class AttackRangeUpdateStrategy : BaseStatUpdateStrategy
    {
        public override void OnAdd(StatNames statName, float value)
        {
            LogStatUpdate(StatNames.AttackRange, System.FindValueOrDefault(StatNames.AttackRange));
        }

        public override void OnRemove(StatNames statName, float value)
        {
            LogStatUpdate(StatNames.AttackRange, System.FindValueOrDefault(StatNames.AttackRange));
        }
    }
}
