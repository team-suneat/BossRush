namespace TeamSuneat
{
    public class LowestHealthVitalComparer : VitalComparer
    {
        public override int Compare(Vital a, Vital b)
        {
            if (a != null && b != null)
            {
                if (a.CurrentHealth < b.CurrentHealth)
                {
                    return -1;
                }
                else if (a.CurrentHealth > b.CurrentHealth)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}