using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        #region 능력치 검색 (Stat Find)

        /// <summary>
        /// 공격자가 해당 능력치를 가지고 있는지 확인합니다.
        /// </summary>
        /// <param name="statName">확인할 능력치 이름</param>
        /// <returns>능력치를 가지고 있으면 true</returns>
        private bool ContainsAttackerStatValue(StatNames statName)
        {
            if (Attacker != null)
            {
                if (Attacker.Stat.ContainsKey(statName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 공격자의 능력치 값을 검색합니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <returns>능력치 값 (없으면 기본값)</returns>
        private float FindAttackerStatValue(StatNames statName)
        {
            if (Attacker != null)
            {
                return Attacker.Stat.FindValueOrDefault(statName);
            }
            else
            {
                StatData statData = JsonDataManager.FindStatData(statName);
                if (statData.IsValid())
                {
                    return statData.DefaultValue;
                }
            }

            return 0f;
        }

        /// <summary>
        /// 피격자의 능력치 값을 검색합니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <returns>능력치 값 (없으면 기본값)</returns>
        private float FindTargetStatValue(StatNames statName)
        {
            if (TargetCharacter != null)
            {
                return TargetCharacter.Stat.FindValueOrDefault(statName);
            }
            else
            {
                StatData statData = JsonDataManager.FindStatData(statName);
                if (statData.IsValid())
                {
                    return statData.DefaultValue;
                }
            }

            return 0f;
        }

        #endregion 능력치 검색 (Stat Find)

        #region 상태이상 검색 (State Effect Find)

        /// <summary>
        /// 피격자가 해당 상태이상을 가지고 있는지 확인합니다.
        /// </summary>
        /// <param name="stateEffect">확인할 상태이상</param>
        /// <returns>상태이상을 가지고 있으면 true</returns>
        private bool ContainsTargetCharacterStateEffect(StateEffects stateEffect)
        {
            return TargetCharacter != null && TargetCharacter.Buff.ContainsStateEffect(stateEffect);
        }

        #endregion 상태이상 검색 (State Effect Find)

        #region 능력치 헬퍼 (Stat Helper)

        /// <summary>
        /// 공격자의 능력치를 검색하고 0이 아닐 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 0이 아닌 경우 true</returns>
        private bool TryAddAttackerStatValue(StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            float statValue = FindAttackerStatValue(statName);
            if (!statValue.IsZero())
            {
                result += statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 피격자의 능력치를 검색하고 0이 아닐 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 0이 아닌 경우 true</returns>
        private bool TryAddTargetStatValue(StatNames statName, ref float result, System.Action<float> logMethod = null)
        {
            float statValue = FindTargetStatValue(statName);
            if (!statValue.IsZero())
            {
                result += statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 공격자의 능력치를 검색하고 특정 값이 아닐 때만 결과에 추가하고 로그를 남깁니다.
        /// </summary>
        /// <param name="statName">검색할 능력치 이름</param>
        /// <param name="result">결과에 추가할 변수</param>
        /// <param name="notEqualValue">비교할 값</param>
        /// <param name="logMethod">로그 메서드 (선택적)</param>
        /// <returns>능력치 값이 비교값과 다른 경우 true</returns>
        private bool TryAddAttackerStatValueIfNotEqual(StatNames statName, ref float result, float notEqualValue, System.Action<float> logMethod = null)
        {
            float statValue = FindAttackerStatValue(statName);
            if (!statValue.Compare(notEqualValue))
            {
                result *= statValue;
                logMethod?.Invoke(statValue);
                return true;
            }
            return false;
        }

        #endregion 능력치 헬퍼 (Stat Helper)
    }
}