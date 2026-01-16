namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public bool IsAlive => GetCurrent(VitalResourceTypes.Life) > 0;

        public bool IsInvulnerable
        {
            get
            {
                if (Life != null)
                {
                    return Life.CheckInvulnerable();
                }
                return false;
            }
        }

        //
    }
}