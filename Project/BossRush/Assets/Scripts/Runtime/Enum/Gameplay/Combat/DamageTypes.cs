namespace TeamSuneat
{
    public enum DamageTypes
    {
        None,

        Normal,  // 일반 피해
        Thorns,  // 가시 피해
        DamageOverTime, // 일반 지속 피해

        #region 처형 (Execution)

        Execution, // 처형

        #endregion 처형 (Execution)

        #region 회복

        Heal, // 생명력 회복
        HealOverTime, // 생명력 지속 회복

        RestoreMana, // 마나 회복
        RestoreManaOverTime, // 마나 지속 회복

        ChargeShield, // 보호막 충전

        #endregion 회복
    }

    public static class DamageTypeChecker
    {
        public static bool IsInstantDamage(this DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.Normal:
                case DamageTypes.Thorns:
                case DamageTypes.Execution:

                    return true;
            }

            return false;
        }

        public static bool IsDamageOverTime(this DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.DamageOverTime:
                    {
                        return true;
                    }
            }

            return false;
        }

        public static bool IsDamage(this DamageTypes key)
        {
            switch (key)
            {
                case DamageTypes.Normal:
                case DamageTypes.Thorns:
                case DamageTypes.DamageOverTime:
                    {
                        return true;
                    }
            }
            return false;
        }

        public static bool IsHeal(this DamageTypes key)
        {
            switch (key)
            {
                case DamageTypes.Heal:
                case DamageTypes.HealOverTime:
                    {
                        return true;
                    }
            }
            return false;
        }

        public static bool IsRestoreMana(this DamageTypes key)
        {
            switch (key)
            {
                case DamageTypes.RestoreMana:
                case DamageTypes.RestoreManaOverTime:
                    {
                        return true;
                    }
            }
            return false;
        }

        public static bool IsCharge(this DamageTypes key)
        {
            switch (key)
            {
                case DamageTypes.ChargeShield:
                    {
                        return true;
                    }
            }
            return false;
        }
    }
}