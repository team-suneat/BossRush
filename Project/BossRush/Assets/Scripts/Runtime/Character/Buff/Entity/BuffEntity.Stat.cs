using Lean.Pool;
using UnityEngine;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        private int previousLevel = 0;  // 이전 레벨 저장
        private int previousStack = 0;  // 이전 스택 저장

        /// <summary>
        /// 이전 레벨과 스택을 초기화합니다.
        /// </summary>
        private void ResetPreviousLevelAndStack()
        {
            previousLevel = 0;  // 이전 레벨
            previousStack = 0;  // 이전 스택
        }

        /// <summary>
        /// 현재 레벨과 스택이 변경되었는지 확인하고, 변경되었으면 능력치를 갱신합니다.
        /// </summary>
        public void RefreshStats()
        {
            bool shouldRefresh = previousLevel != Level || previousStack != Stack;

            // 모든 능력치를 한 번에 갱신
            for (int i = 0; i < AssetData.Stats.Length; i++)
            {
                if (shouldRefresh)
                {
                    ApplyStatDifference();
                }
            }

            if (shouldRefresh)
            {
                previousLevel = Level;
                previousStack = Stack;
            }
        }

        /// <summary>
        /// 버프의 능력치 차이를 계산하여 적용합니다.
        /// </summary>
        private void ApplyStatDifference()
        {
            for (int i = 0; i < AssetData.Stats.Length; i++)
            {
                StatNames statName = AssetData.Stats[i];
                float newStatValue = CalculateStatValue(Level, Stack, i);

                Owner.Stat.RemoveBySource(statName, this);
                Owner.Stat.AddWithSourceInfo(statName, newStatValue, this, Name.ToString(), "Buff");

                LogStatUpdate(statName, newStatValue, previousLevel, Level, previousStack, Stack);
            }
        }

        /// 레벨과 스택에 따른 능력치 값을 계산합니다.
        /// </summary>
        /// <param name="level">레벨</param>
        /// <param name="stack">스택</param>
        /// <param name="index">능력치 인덱스</param>
        /// <returns>계산된 능력치 값</returns>
        private float CalculateStatValue(int level, int stack, int index)
        {
            if (AssetData.IsValid())
            {
                float statValue = 0;
                if (AssetData.StatValues.Length > index)
                {
                    statValue = AssetData.StatValues[index];
                }

                float statValueByLevel = 0;
                if (AssetData.StatValuesByLevel.Length > index)
                {
                    statValueByLevel = AssetData.StatValuesByLevel[index];
                }

                float resultStatValue = StatEx.GetValueByLevel(statValue, statValueByLevel, level);
                resultStatValue *= GetLinkedStatValue(index);

                StatApplicationsByStack statApplicationsByStack = AssetData.StatApplicationByStack;
                if (stack > 1)
                {
                    switch (statApplicationsByStack)
                    {
                        case StatApplicationsByStack.Multi: resultStatValue *= stack; break;
                        case StatApplicationsByStack.Pow: resultStatValue = Mathf.Pow(resultStatValue, stack); break;
                        case StatApplicationsByStack.NotUse: break;
                        default: LogStackWarning(stack); break;
                    }
                }

                return resultStatValue;
            }
            return 0;
        }

        #region Remove Stat

        private void RemoveStats()
        {
            StatNames statName;
            for (int i = 0; i < AssetData.Stats.Length; i++)
            {
                statName = AssetData.Stats[i];
                RemoveBuffStat(statName);
            }
        }

        private void RemoveBuffStat(StatNames statName)
        {
            if (statName == StatNames.None)
            {
                return;
            }

            if (!Owner.Stat.ContainsKey(statName, this))
            {
                return;
            }

            float statValue = Owner.Stat.FindValueBySource(statName, this);
            LogRemoveBuffStat(statName, statValue);
            Owner.Stat.RemoveBySource(statName, this);
        }

        #endregion Remove Stat

        #region Linked Stat

        private float GetLinkedStatValue(int index)
        {
            if (AssetData.LinkedBuffStatTypes == null || AssetData.LinkedBuffStatTypes.Length <= index)
            {
                return 1f;
            }
            else if (AssetData.LinkedBuffStatDivisors == null || AssetData.LinkedBuffStatDivisors.Length <= index)
            {
                return 1f;
            }
            else
            {
                float statDivisor = AssetData.LinkedBuffStatDivisors[index];
                switch (AssetData.LinkedBuffStatTypes[index])
                {
                    case LinkedBuffStatTypes.CurrentHealthOfAttacker: // 시전자의 현재 생명력
                        {
                            return GetCurrentHealthOfAttackerMultiplier(statDivisor);
                        }

                    case LinkedBuffStatTypes.MaxHealthOfAttacker: // 시전자의 최대 생명력
                        {
                            return GetMaxHealthOfAttackerMultiplier(statDivisor);
                        }

                    case LinkedBuffStatTypes.MissingHealthOfAttacker: // 시전자의 잃은 생명력
                        {
                            return GetMissingHealthOfAttackerMultiplier(statDivisor);
                        }

                    default:
                        {
                            return 1f;
                        }
                }
            }
        }

        private float GetCurrentHealthOfAttackerMultiplier(float statDivisor)
        {
            float multiplier = Mathf.FloorToInt(Owner.MyVital.CurrentHealth.SafeDivide(statDivisor));
            LogCurrentHealthMultiplier(Owner.MyVital.CurrentHealth, statDivisor, multiplier);

            return multiplier;
        }

        private float GetMaxHealthOfAttackerMultiplier(float statDivisor)
        {
            float multiplier = Mathf.FloorToInt(Owner.MyVital.MaxHealth.SafeDivide(statDivisor));
            LogMaxHealthMultiplier(Owner.MyVital.MaxHealth, statDivisor, multiplier);

            return multiplier;
        }

        private float GetMissingHealthOfAttackerMultiplier(float statDivisor)
        {
            float missingHealthRate = 1 - Owner.MyVital.HealthRate;
            float multiplier = Mathf.FloorToInt(missingHealthRate.SafeDivide(statDivisor));
            LogMissingHealthMultiplier(missingHealthRate, statDivisor, multiplier);

            return multiplier;
        }

        #endregion Linked Stat

        #region Log

        private void LogLegendaryStatUpdate(StatNames statName, float statValue)
        {
            if (Log.LevelInfo)
            {
                LogProgress("버프의 전설 장비 능력치를 갱신합니다. 능력치 이름: {0}, 능력치 값: {1}",
                statName.ToLogString(),
                statName.GetStatValueString(statValue, true));
            }
        }

        private void LogStatUpdate(StatNames statName, float statDifference, int previousLevel, int newLevel, int previousStack, int newStack)
        {
            if (Log.LevelProgress)
            {
                LogProgress("버프의 능력치를 갱신합니다. 능력치 이름: {0}, 변경된 능력치: {1}, 레벨 변화:{2} ▶ {3}, 스택 변화:{4} ▶ {5}",
                statName.ToLogString(),
                statName.GetStatValueString(statDifference, true),
                previousLevel, newLevel.ToSelectString(previousLevel),
                previousStack, newStack.ToSelectString(previousStack));
            }
        }

        private void LogRemoveBuffStat(StatNames statName, float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("버프의 능력치를 삭제합니다. 능력치 이름: {0}, 능력치 값: {1}", statName.ToLogString(), statName.GetStatValueString(statValue, true));
            }
        }

        private void LogStackWarning(int stack)
        {
            if (Log.LevelWarning)
            {
                LogWarning("버프의 스택({0})은 올랐으나 스택에 따른 능력치 변화에 실패했습니다.", stack.ToSelectString());
            }
        }

        private void LogCurrentHealthMultiplier(float currentHealth, float statDivisor, float multiplier)
        {
            if (Log.LevelProgress)
            {
                LogProgress("연결된 [시전자의 현재 생명력({0})]의 ({1})당 비례 능력치 배율({2})을 설정합니다.",
                currentHealth,
                statDivisor,
                ValueStringEx.GetPercentString(multiplier, 0f));
            }
        }

        private void LogMaxHealthMultiplier(float maxHealth, float statDivisor, float multiplier)
        {
            if (Log.LevelProgress)
            {
                LogProgress("연결된 [시전자의 최대 생명력({0})]의 ({1})당 비례 능력치 배율({2})을 설정합니다.",
                maxHealth,
                statDivisor,
                ValueStringEx.GetPercentString(multiplier, 0f));
            }
        }

        private void LogMissingHealthMultiplier(float missingHealthRate, float statDivisor, float multiplier)
        {
            if (Log.LevelProgress)
            {
                LogProgress("연결된 [시전자의 잃은 생명력 비율({0})]의 ({1})당 비례 능력치 배율({2})을 설정합니다.",
                ValueStringEx.GetPercentString(missingHealthRate),
                ValueStringEx.GetPercentString(statDivisor),
                ValueStringEx.GetValueString(multiplier, 0f));
            }
        }

        private void LogCurrentResourceMultiplier(float currentResource, float statDivisor, float multiplier)
        {
            if (Log.LevelProgress)
            {
                LogProgress("연결된 [시전자의 현재 전투 자원({0})]의 ({1})당 비례 능력치 배율({2})을 설정합니다.",
                currentResource,
                statDivisor,
                ValueStringEx.GetPercentString(multiplier, 0f));
            }
        }

        private void LogMaxResourceMultiplier(float maxResource, float statDivisor, float multiplier)
        {
            if (Log.LevelProgress)
            {
                LogProgress("연결된 [시전자의 최대 전투 자원({0})]의 ({1})당 비례 능력치 배율({2})을 설정합니다.",
                maxResource,
                statDivisor,
                ValueStringEx.GetPercentString(multiplier, 0f));
            }
        }

        private void LogMissingResourceMultiplier(float missingResourceRate, float statDivisor, float multiplier)
        {
            if (Log.LevelProgress)
            {
                LogProgress("연결된 [시전자의 잃은 자원 비율({0})]의 ({1})당 비례 능력치 배율({2})을 설정합니다.",
                ValueStringEx.GetPercentString(missingResourceRate),
                ValueStringEx.GetPercentString(statDivisor),
                ValueStringEx.GetValueString(multiplier, 0f));
            }
        }

        #endregion Log
    }
}