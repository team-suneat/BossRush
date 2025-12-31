using System;

namespace TeamSuneat
{
    public class StatLogHandler
    {
        public void LogLevelUp()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Stat, "(System) 레벨업으로 인한 능력치 변경");
            }
        }

        public void LogClearAllStats()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Stat, "(System) 모든 능력치를 초기화합니다.");
            }
        }

        public void LogReplaceStat(StatNames statName, StatModifier modifier)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Stat, "(System) 능력치({0})를 교체합니다. 값({1})",
                    statName.ToLogString(),
                    modifier.GetValueString());
            }
        }

        public void LogAllStatChange(string action, float value)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Stat, "(System) 모든 능력치({0}) {1}",
                    ValueStringEx.GetValueString(value, true),
                    action);
            }
        }

        public void LogAdd(StatNames statName, StatModifier modifier, float totalValue)
        {
            if (Log.LevelInfo)
            {
                float modifierValue = modifier.Value;
                if (modifierValue.IsZero() && totalValue.IsZero())
                {
                    return;
                }

                Log.Info(LogTags.Stat, "(System) {0} {1}, 능력치를 추가합니다. Add:{2}, Total:{3}, Source: {4}",
                        "Owner", // Owner.Name.ToLogString() 대신 임시로 "Owner" 사용
                        statName.ToLogString(),
                        statName.GetStatValueString(modifierValue, true),
                        statName.GetStatValueString(totalValue, true),
                        modifier.GetSourceString());
            }
        }

        public void LogStatDataError(StatNames statName)
        {
            if (Log.LevelError)
            {
                Log.Error("능력치 데이터가 올바르지 않아 능력치({0})를 추가할 수 없습니다.", statName.ToLogString());
            }
        }

        public void LogStatDataError()
        {
            if (Log.LevelError)
            {
                Log.Error("능력치 데이터가 올바르지 않아 능력치를 추가할 수 없습니다.");
            }
        }

        public void LogComponentRemoval(UnityEngine.Component source, StatNames statName)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Stat, "(System) {0} 컴포넌트({1})에서 추가한 능력치({2})를 삭제합니다.",
                    "Owner", // Owner.Name.ToLogString() 대신 임시로 "Owner" 사용
                    source.GetHierarchyName(),
                    statName.ToLogString());
            }
        }

        public void LogItemRemoval(StatNames statName)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Stat, "(System) {0} 아이템에서 추가한 능력치({1})를 삭제합니다.",
                    "Owner", // Owner.Name.ToLogString() 대신 임시로 "Owner" 사용
                    statName.ToLogString());
            }
        }

        public void LogModifierRemoval(StatNames statName, StatModifier statModifier, string totalValue = "0")
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Stat, "(System) {0} {1}, 능력치 Modifier를 삭제합니다. {2}, Total: {3}",
                    "Owner", // Owner.Name.ToLogString() 대신 임시로 "Owner" 사용
                    statName.ToLogString(),
                    statName.GetStatValueString(statModifier.Value).ToErrorString(),
                    totalValue);
            }
        }

        public void LogAttackSpeedCalculation(float result)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Stat, "(System) {1} 공격 속도를 계산합니다: {0}", ValueStringEx.GetValueString(result), "Owner");
            }
        }
    }
}