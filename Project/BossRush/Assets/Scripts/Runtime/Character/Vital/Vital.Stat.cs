namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (Owner != null && Owner.Stat != null)
            {
                Owner.Stat.RegisterOnRefresh(OnRefreshStat);
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            if (Owner != null && Owner.Stat != null)
            {
                Owner.Stat.UnregisterOnRefresh(OnRefreshStat);
            }
        }

        private void OnRefreshStat(StatNames statName, float addStatValue)
        {
        }
    }
}