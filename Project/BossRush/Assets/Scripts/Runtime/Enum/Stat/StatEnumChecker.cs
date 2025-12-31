using TeamSuneat.Data;

namespace TeamSuneat
{
    public static class StatEnumChecker
    {
        public static bool IsReduceStat(this StatNames key)
        {
            switch (key)
            {
                case StatNames.DamageReduction:
                    return true;
            }

            return false;
        }

        public static bool IsPercent(this StatNames key)
        {
            StatData statData = JsonDataManager.FindStatData(key);

            if (statData.IsValid())
            {
                switch (statData.Mod)
                {
                    case StatModType.PercentAdd:
                    case StatModType.PercentMulti:
                        {
                            return true;
                        }
                }
            }

            return false;
        }
    }
}