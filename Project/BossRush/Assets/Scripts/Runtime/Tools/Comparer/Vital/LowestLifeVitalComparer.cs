namespace TeamSuneat
{
    public class LowestLifeVitalComparer : VitalComparer
    {
        public override int Compare(Vital a, Vital b)
        {
            if (a != null && b != null)
            {
                if (a.CurrentLife < b.CurrentLife)
                {
                    return -1;
                }
                else if (a.CurrentLife > b.CurrentLife)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}