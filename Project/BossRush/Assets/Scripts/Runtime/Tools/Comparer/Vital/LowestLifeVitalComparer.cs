namespace TeamSuneat
{
    public class LowestLifeVitalComparer : VitalComparer
    {
        public override int Compare(Vital a, Vital b)
        {
            if (a != null && b != null)
            {
                float lifeA = a.GetCurrent(VitalResourceTypes.Life);
                float lifeB = b.GetCurrent(VitalResourceTypes.Life);

                if (lifeA < lifeB)
                {
                    return -1;
                }
                else if (lifeA > lifeB)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}