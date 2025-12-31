namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public int CurrentHealth
        {
            get => Health != null ? Health.Current : 0;
            set
            {
                if (Health != null)
                {
                    Health.Current = value;
                }
            }
        }

        public float HealthRate => Health != null ? Health.Rate : 0f;

        public int MaxHealth
        {
            get => Health != null ? Health.Max : 0;
            set
            {
                if (Health != null)
                {
                    Health.Max = value;
                }
            }
        }

        public int CurrentShield
        {
            get => Shield != null ? Shield.Current : 0;
            set
            {
                if (Shield != null)
                {
                    Shield.Current = value;
                }
            }
        }

        public float ShieldRate
        {
            get
            {
                if (Shield != null)
                {
                    return CurrentShield.SafeDivide(MaxShield);
                }

                return 0f;
            }
        }

        public int MaxShield
        {
            get => Shield != null ? Shield.Max : 0;
            set
            {
                if (Shield != null)
                {
                    Shield.Max = value;
                }
            }
        }

        public bool IsAlive => CurrentHealth > 0;

        public bool IsInvulnerable
        {
            get
            {
                if (Health != null)
                {
                    return Health.CheckInvulnerable();
                }
                return false;
            }
        }
    }
}