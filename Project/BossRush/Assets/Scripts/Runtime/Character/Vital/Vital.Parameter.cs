namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public int CurrentLife
        {
            get => Life != null ? Life.Current : 0;
            set
            {
                if (Life != null)
                {
                    Life.Current = value;
                }
            }
        }

        public float LifeRate => Life != null ? Life.Rate : 0f;

        public int MaxLife
        {
            get => Life != null ? Life.Max : 0;
            set
            {
                if (Life != null)
                {
                    Life.Max = value;
                }
            }
        }

        public int CurrentShield
        {
            get => Barrier != null ? Barrier.Current : 0;
            set
            {
                if (Barrier != null)
                {
                    Barrier.Current = value;
                }
            }
        }

        public float ShieldRate
        {
            get
            {
                if (Barrier != null)
                {
                    return CurrentShield.SafeDivide(MaxShield);
                }

                return 0f;
            }
        }

        public int MaxShield
        {
            get => Barrier != null ? Barrier.Max : 0;
            set
            {
                if (Barrier != null)
                {
                    Barrier.Max = value;
                }
            }
        }

        public bool IsAlive => CurrentLife > 0;

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
    }
}