using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class StatSystem : XBehaviour
    {
        /// <summary>
        /// 능력치를 검색합니다.
        /// </summary>
        /// <returns> 등록된 능력치가 없다면 기본값 또는 0을 반환합니다. </returns>
        public float FindValueOrDefault(StatNames statName)
        {
            if (statName != StatNames.None)
            {
                if (ContainsKey(statName))
                {
                    return _stats[statName].Value;
                }
                else
                {
                    StatData statData = JsonDataManager.FindStatData(statName);
                    if (statData.IsValid())
                    {
                        return statData.DefaultValue;
                    }
                }
            }
            return 0f;
        }

        public int FindValueOrDefaultToInt(StatNames statName)
        {
            return Mathf.RoundToInt(FindValueOrDefault(statName));
        }

        /// <summary>
        /// 특정 출처에 따른 능력치 값을 검색합니다.
        /// </summary>
        /// <returns> 능력치 값이 없을 때는 0을 반환합니다. </returns>
        public float FindValueBySource(StatNames statName, Component source)
        {
            if (statName != StatNames.None)
            {
                if (ContainsKey(statName))
                {
                    CharacterStat characterStat = _stats[statName];
                    if (characterStat != null)
                    {
                        return characterStat.FindValueBySource(source);
                    }
                }
            }

            return 0f;
        }

        /// <summary>
        /// 특정 출처에 따른 능력치 값을 검색합니다.
        /// </summary>
        /// <returns> 능력치 값이 없을 때는 0을 반환합니다. </returns>
        public float FindValueBySource(StatNames statName, string sourceType)
        {
            if (statName != StatNames.None)
            {
                if (ContainsKey(statName))
                {
                    CharacterStat characterStat = _stats[statName];
                    if (characterStat != null)
                    {
                        return characterStat.FindValueByType(sourceType);
                    }
                }
            }

            return 0f;
        }
    }
}