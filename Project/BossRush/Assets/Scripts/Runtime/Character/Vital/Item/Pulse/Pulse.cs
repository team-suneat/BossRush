using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 캐릭터의 패링 게이지를 관리하는 클래스입니다. </summary>
    public class Pulse : VitalResource
    {
        #region Field

        [Title("#Pulse")]
        [ReadOnly]
        [FoldoutGroup("#PulseGauge")]
        [SuffixLabel("현재 게이지 진행도 (0~1)")]
        public float GaugeProgress = 0f;

        public event Action<float> OnGaugeProgressChanged;

        #endregion Field

        #region Parameter

        public override VitalResourceTypes Type => VitalResourceTypes.Pulse;

        /// <summary> 패링 사용 가능 여부 (Current 1 이상). </summary>
        public bool CanParry => Current >= 1;

        #endregion Parameter

        public override void Initialize()
        {
            LogInfo("패링 게이지를 초기화합니다.");

            base.Initialize();

            // 전투 시작 시 항상 풀 게이지
            LoadCurrentValue();

            GaugeProgress = 0f;
            NotifyGaugeProgressChanged();
        }

        public override void LoadCurrentValue()
        {
            Current = Max;
            GaugeProgress = 0f;
            NotifyGaugeProgressChanged();
            SendGlobalEventOfChange();

            LogInfo("현재 온전한 펄스를 최대값으로 불러옵니다. {0}/{1}", Current, Max);
        }

        public override void RefreshMaxValue(bool shouldAddExcessToCurrent = false)
        {
            if (Vital == null || Vital.Owner == null || Vital.Owner.Stat == null)
            {
                LogWarning("최대 펄스를 불러올 수 없습니다. 바이탈, 소유 캐릭터, 능력치 시스템 중 최소 하나가 없습니다.");
                return;
            }

            float statValue = Vital.Owner.Stat.FindValueOrDefault(StatNames.Pulse);
            int maxPulseByStat = Mathf.RoundToInt(statValue);
            if (maxPulseByStat > 0)
            {
                int previousMax = Max;
                Max = maxPulseByStat;

                LogInfo("캐릭터의 능력치에 따라 최대 펄스를 갱신합니다. {0}/{1}", Current, Max);

                if (shouldAddExcessToCurrent && Max > previousMax)
                {
                    Current += Max - previousMax;
                }
                if (Current > Max)
                {
                    Current = Max;
                    LogInfo("캐릭터의 남은 펄스가 최대 펄스보다 크다면, 최대 펄스로 설정합니다. {0}/{1}", Current, Max);
                }
            }
        }

        /// <summary> 공격 성공 시 게이지 증가. </summary>
        public void OnAttackSuccess(float gainAmount = 0f)
        {
            if (gainAmount <= 0f)
            {
                return;
            }

            float newProgress = GaugeProgress + gainAmount;

            if (newProgress >= 1f)
            {
                if (AddFullPulse())
                {
                    GaugeProgress = 0f;
                }
            }
            else
            {
                GaugeProgress = newProgress;
            }

            NotifyGaugeProgressChanged();
            LogInfo("공격 성공으로 펄스 게이지를 증가합니다. 진행도: {0:F1}%", GaugeProgress * 100f);
        }

        /// <summary> 온전한 펄스 추가. </summary>
        private bool AddFullPulse()
        {
            if (Current >= Max)
            {
                LogWarning("온전한 펄스가 최대 개수에 도달했습니다. {0}/{1}", Current, Max);
                return false;
            }

            Current++;
            SendGlobalEventOfChange();
            LogInfo("온전한 펄스를 획득합니다. {0}/{1}", Current, Max);
            return true;
        }

        /// <summary> 온전한 펄스 사용 시도. </summary>
        public bool TryUseFullPulse()
        {
            if (Current <= 0)
            {
                LogWarning("온전한 펄스가 부족합니다. 현재: {0}", Current);
                return false;
            }

            Current--;
            SendGlobalEventOfChange();
            LogInfo("온전한 펄스를 사용합니다. 남은 개수: {0}/{1}", Current, Max);

            return true;
        }

        /// <summary> 패링 시전 시 게이지 소모. (검사 없이 소모만 수행) </summary>
        public void UseParry()
        {
            // Current 값을 1 감소
            Current = Mathf.Max(0, Current - 1);
            SendGlobalEventOfChange();
            LogInfo("패링 게이지를 소모합니다. 남은 Current: {0}/{1}", Current, Max);
        }

        /// <summary> 패링 성공 시 게이지 회복. </summary>
        public void OnParrySuccess()
        {
            OnAttackSuccess(0.5f);
            LogInfo("패링 성공! 게이지를 회복합니다.");
        }

        private void NotifyGaugeProgressChanged()
        {
            OnGaugeProgressChanged?.Invoke(GaugeProgress);
        }
    }
}