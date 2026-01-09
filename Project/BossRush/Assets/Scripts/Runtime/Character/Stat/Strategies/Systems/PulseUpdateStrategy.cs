namespace TeamSuneat
{
    public class PulseUpdateStrategy : BaseStatUpdateStrategy
    {
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshPulse(System);
        }

        public override void OnRemove(StatNames statName, float value)
        {
            RefreshPulse(System);
        }

        private void RefreshPulse(StatSystem StatSystem)
        {
            if (StatSystem.Owner.MyVital.Pulse != null)
            {
                LogRefresh("Pulse");
                StatSystem.Owner.MyVital.Pulse.RefreshMaxValue(true);
            }
        }
    }
}