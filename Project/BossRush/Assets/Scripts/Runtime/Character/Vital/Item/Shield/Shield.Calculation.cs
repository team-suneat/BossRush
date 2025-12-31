using UnityEngine;

namespace TeamSuneat
{
    /// <summary> Shield 클래스의 계산 관련 기능을 담당합니다. </summary>
    public partial class Shield
    {
        /// <summary> VitalResource의 RefreshMaxValue를 오버라이드합니다. </summary>
        public override void RefreshMaxValue(bool shouldAddExcessToCurrent = false)
        {
            RefreshMaxValue(shouldAddExcessToCurrent, false);
        }

        /// <summary> 보호막 최대값을 갱신합니다. </summary>
        /// <param name="shouldAddExcessToCurrent">최대값 증가 시 현재값도 함께 증가할지 여부</param>
        /// <param name="shouldLoadCurrentValueToMax">능력치 변경 시 현재값을 최대값으로 설정할지 여부</param>
        public void RefreshMaxValue(bool shouldAddExcessToCurrent = false, bool shouldLoadCurrentValueToMax = false)
        {
            if (!ValidateVitalAndOwner())
            {
                LogShieldMaxValueError();
                return;
            }

            int previousMax = Max;
            ShieldCalculationData calculationData = CalculateShieldValues();

            SetNewMaxValue(calculationData);
            AdjustCurrentValue(previousMax, shouldAddExcessToCurrent, shouldLoadCurrentValueToMax);
            RefreshUIAndHandleEvents(previousMax);
            LogShieldMaxValueRefreshed(calculationData);
        }

        /// <summary> Vital과 Owner의 유효성을 검증합니다. </summary>
        private bool ValidateVitalAndOwner()
        {
            return Vital.Owner != null && Vital.Owner.Stat != null;
        }

        /// <summary> 보호막 계산에 필요한 모든 값을 조회합니다. </summary>
        private ShieldCalculationData CalculateShieldValues()
        {
            float multiplier = FindStatValueByOwner(StatNames.ShieldMulti);
            float fixedValue = FindStatValueByOwner(StatNames.Shield);

            return new ShieldCalculationData
            {
                Multiplier = multiplier,
                FixedValue = fixedValue,
            };
        }

        /// <summary> 새로운 최대값을 설정합니다. </summary>
        private void SetNewMaxValue(ShieldCalculationData data)
        {
            float totalValue = data.FixedValue;

            if (totalValue > 0)
            {
                Max = Mathf.RoundToInt(totalValue * data.Multiplier);
            }
            else
            {
                Max = 0;
            }
        }

        /// <summary> 현재값을 조정합니다. </summary>
        private void AdjustCurrentValue(int previousMax, bool shouldAddExcessToCurrent, bool shouldLoadCurrentValueToMax)
        {
            // 보호막 배율 변경 시 현재값을 비례적으로 조정
            if (previousMax > 0 && Max > 0)
            {
                float ratio = Current.SafeDivide01(previousMax);
                Current = Mathf.RoundToInt(Max * ratio);
            }
            else if (shouldAddExcessToCurrent && Max > previousMax)
            {
                Current += Max - previousMax;
            }

            // 능력치 변경 시 현재값을 최대값으로 설정
            if (shouldLoadCurrentValueToMax)
            {
                Current = Max;
            }
            else if (Current > Max)
            {
                Current = Max;
            }
        }

        /// <summary> UI를 갱신하고 이벤트를 처리합니다. </summary>
        private void RefreshUIAndHandleEvents(int previousMax)
        {
            Vital.RefreshShieldGauge();

            if (Max == 0 && previousMax != 0)
            {
                OnDestroyShield();
            }
            else if (Max > 0 && previousMax == 0)
            {
                OnGainShield();
            }
        }
    }
}