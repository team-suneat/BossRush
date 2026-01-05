using TeamSuneat.Data;

namespace TeamSuneat
{
    public static class EnumCompareEx
    {
        public static bool Compare(this StatNames[] values1, StatNames[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this StateEffects[] values1, StateEffects[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this HitmarkNames[] values1, HitmarkNames[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this DamageTypes[] values1, DamageTypes[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this GradeNames[] values1, GradeNames[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this MapObjectNames[] values1, MapObjectNames[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this MapTypes[] values1, MapTypes[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this CurrencyNames[] values1, CurrencyNames[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this CharacterNames[] values1, CharacterNames[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this MonsterGrades[] values1, MonsterGrades[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (values1[i] != values2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(this HitmarkAssetData[] values1, HitmarkAssetData[] values2)
        {
            if (values1 != null && values2 == null)
            {
                return false;
            }
            if (values1 == null && values2 != null)
            {
                return false;
            }
            if (values1.Length != values2.Length)
            {
                return false;
            }

            for (int i = 0; i < values1.Length; i++)
            {
                if (!values1[i].CompareDamage(values2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}