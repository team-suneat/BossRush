using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// PulseRegen 관련 능력치 업데이트 전략
    /// PulseRegen을 처리합니다.
    /// </summary>
    public class PulseRegenUpdateStrategy : BaseStatUpdateStrategy
    {
        /// <summary>
        /// PulseRegen 관련 능력치가 추가될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">추가될 값</param>
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshPulseRegenerate(System);
        }

        /// <summary>
        /// PulseRegen 관련 능력치가 제거될 때 호출됩니다.
        /// </summary>

        /// <param name="statName">능력치 이름</param>
        /// <param name="value">제거될 값</param>
        public override void OnRemove(StatNames statName, float value)
        {
            RefreshPulseRegenerate(System);
        }

        /// <summary>
        /// Pulse 재생 비율을 새로고침합니다.
        /// </summary>

        private void RefreshPulseRegenerate(StatSystem StatSystem)
        {
            if (StatSystem.Owner.MyVital != null)
            {
                float pulseRegenValue = StatSystem.FindValueOrDefault(StatNames.PulseRegen);
                StatSystem.Owner.MyVital.PulseRegenerateRate = pulseRegenValue;

                if (Log.LevelInfo)
                {
                    string ownerName = GetOwnerName(StatSystem);
                    Log.Info(LogTags.Stat, "(System) {0}, 펄스 재생 비율을 설정합니다. 재생 비율: {1}",
                        ownerName,
                        ValueStringEx.GetValueString(pulseRegenValue, true));
                }
            }
        }
    }
}

