using UnityEngine;

namespace TeamSuneat.Data
{
    public partial class BuffAssetData : ScriptableData<int>
    {
        private void CustomLog()
        {
#if UNITY_EDITOR

            if (MaxStack > 0)
            {
                if (Name is not BuffNames.Chilled and not BuffNames.Jolted)
                {
                    if (StatApplicationByStack == StatApplicationsByStack.None)
                    {
                        Log.Error("버프({0})의 스택별 적용 옵션이 설정되어 있지 않습니다.", Name);
                    }

                    if (Stats.IsValidArray())
                    {
                        if (!StatValuesByLevel.IsValidArray())
                        {
                            if (!StatValuesIncreaseInRange.IsValidArray())
                            {
                                Log.Error("능력치 데이터({0})의 레벨별 값이 설정되어 있지 않습니다.", Name);
                            }
                        }
                    }
                }
            }

            LogStat();
#endif
        }

        private void LogStat()
        {
#if UNITY_EDITOR

            string type = "BuffAsset".ToColorString(GameColors.Buff);
            EnumExplorer.LogStat(type, Name.ToString(), MaxStackByStat);
            EnumExplorer.LogStat(type, Name.ToString(), DurationByStat);

            if (MinStatValues.IsValidArray())
            {
                EnumExplorer.LogStat(type, Name.ToString(), Name.GetLocalizedString(), Stats, MinStatValues);
            }
            else if (StatValues.IsValidArray())
            {
                EnumExplorer.LogStat(type, Name.ToString(), Name.GetLocalizedString(), Stats, StatValues);
            }
            else if (Stats.IsValidArray())
            {
                EnumExplorer.LogStat(type, Name.ToString(), Name.GetLocalizedString(), Stats, StatValues);
            }
#endif
        }

#if UNITY_EDITOR

        protected Color GetBuffTargetTypeColor(BuffTargetTypes buffTarget)
        {
            if (buffTarget == 0)
            {
                return GameColors.CreamIvory;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetBuffApplicationColor(BuffApplications buffApplication)
        {
            if (buffApplication == 0)
            {
                return GameColors.CreamIvory;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetStatApplicationsByStackColor(StatApplicationsByStack statApplicationsByStack)
        {
            if (MaxStack <= 0 && statApplicationsByStack != 0)
            {
                return GameColors.CherryRed;
            }
            else if (MaxStack > 0 && statApplicationsByStack == 0)
            {
                return GameColors.CherryRed;
            }
            else if (MaxStack > 0 && statApplicationsByStack > 0)
            {
                return GameColors.GreenYellow;
            }

            return GameColors.CreamIvory;
        }

        protected Color GetStatNamesColor()
        {
            if (Stats.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.CreamIvory;
        }

        protected Color GetStatValuesColor()
        {
            if (!StatValues.IsValidArray())
            {
                return GameColors.CreamIvory;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetStatValuesByLevelColor()
        {
            if (!StatValuesByLevel.IsValidArray())
            {
                return GameColors.CreamIvory;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetLinkedBuffStatDivisorsColor()
        {
            if (!LinkedBuffStatDivisors.IsValidArray())
            {
                return GameColors.CreamIvory;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetMinStatValuesColor()
        {
            if (!MinStatValues.IsValidArray())
            {
                return GameColors.CreamIvory;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetMaxStatValuesColor()
        {
            if (!MaxStatValues.IsValidArray())
            {
                return GameColors.CreamIvory;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetStatValuesIncreaseInRangeColor()
        {
            if (!StatValuesIncreaseInRange.IsValidArray())
            {
                return GameColors.CreamIvory;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

#endif
    }
}