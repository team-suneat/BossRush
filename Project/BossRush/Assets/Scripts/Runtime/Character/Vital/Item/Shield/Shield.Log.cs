namespace TeamSuneat
{
    /// <summary> Shield 클래스의 로깅 기능을 담당합니다. </summary>
    public partial class Shield
    {
        /// <summary> 보호막 계산 데이터를 로깅합니다. </summary>
        private void LogShieldMaxValueRefreshed(ShieldCalculationData data)
        {
            LogShieldMaxValueRefreshed(Current, Max, data.FixedValue, Vital.MaxHealth, data.Multiplier);
        }

        #region Log

        protected void LogShieldLoaded(int current, int max)
        {
            if (Log.LevelInfo)
            {
                LogInfo("플레이어 캐릭터의 보호막을 불러옵니다. {0}/{1}", current, max);
            }
        }

        protected void LogShieldInitialized(int current, int max)
        {
            if (Log.LevelInfo)
            {
                LogInfo("캐릭터의 보호막을 초기화합니다. {0}/{1}", current, max);
            }
        }

        protected void LogShieldMaxValueRefreshed(int current, int max, float fixedValue, int maxHealth, float valueRate)
        {
            if (Log.LevelInfo)
            {
                LogInfo("캐릭터의 최대 보호막을 불러옵니다. {0}/{1}. 계산식: 고정 보호막({2}) * 보호막 배율({3})",
                    current.ToSelectString(max), max.ToSelectString(0), fixedValue, ValueStringEx.GetPercentString(valueRate, 1));
            }
        }

        protected void LogShieldMaxValueIncreasedFromZero(int current, int max, float fixedValue, int maxHealth, float valueRateBaseOnMaxHealth, float valueRate)
        {
            LogInfo("캐릭터의 최대 보호막을 불러옵니다. 최대 값이 0에서부터 설정되어 현재값도 함께 설정합니다. {0}/{1}. 계산식: [고정 보호막({2}) + 최대 생명력 비례 고정 보호막({3}*{4})] * 보호막 배율({5})",
                current.ToSelectString(), max.ToSelectString(0), fixedValue, maxHealth, valueRateBaseOnMaxHealth, valueRate);
        }

        protected void LogShieldMaxValueError()
        {
            if (Log.LevelError)
            {
                LogError("캐릭터의 최대 보호막을 불러올 수 없습니다. Vital의 캐릭터 또는 능력치 정보를 불러올 수 없습니다.");
            }
        }

        protected void LogShieldUsageFailure(int value)
        {
            if (Log.LevelError)
            {
                if (Current <= 0)
                {
                    LogError("보호막을 사용할 수 없습니다. 현재 보호막 값이 부족합니다: {0}/{1}", Current, Max);
                }
                else if (value <= 0)
                {
                    LogError("보호막을 사용할 수 없습니다. 잘못된 값이 입력되었습니다: {0}", value);
                }
            }
        }

        #endregion Log
    }
}