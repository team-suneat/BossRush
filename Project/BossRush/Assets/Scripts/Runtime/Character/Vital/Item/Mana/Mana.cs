using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class Mana : VitalResource
    {
        public override VitalResourceTypes Type => VitalResourceTypes.Mana;

        public override void LoadCurrentValue()
        {
            Current = Max;

            LogInfo("캐릭터의 마나를 초기화합니다. {0}/{1}", Current, Max);
        }

        public override bool AddCurrentValue(int value)
        {
            if (base.AddCurrentValue(value))
            {
                Vital.RefreshResourceGauge();
                return true;
            }
            return false;
        }

        public override bool UseCurrentValue(int value)
        {
            if (base.UseCurrentValue(value))
            {
                Vital.RefreshResourceGauge();
                return true;
            }
            return false;
        }

        protected override void OnAddCurrentValue(int value)
        {
            base.OnAddCurrentValue(value);
            Vital.RefreshResourceGauge();
        }

        public override void RefreshMaxValue(bool shouldAddExcessToCurrent = false)
        {
            if (Vital == null || Vital.Owner == null || Vital.Owner.Stat == null)
            {
                LogWarning("최대 마나을 불러올 수 없습니다. 바이탈, 소유 캐릭터, 능력치 시스템 중 최소 하나가 없습니다.");
                return;
            }

            float statValue = Vital.Owner.Stat.FindValueOrDefault(StatNames.Mana);
            int maxManaByStat = Mathf.RoundToInt(statValue);
            if (maxManaByStat > 0)
            {
                int previousMax = Max;
                Max = Mathf.RoundToInt(maxManaByStat);

                LogInfo("캐릭터의 능력치에 따라 최대 마나을 갱신합니다. {0}/{1}", Current, Max);

                if (shouldAddExcessToCurrent && Max > previousMax)
                {
                    Current += Max - previousMax;
                }
                if (Current > Max)
                {
                    Current = Max;

                    LogInfo("캐릭터의 남은 마나이 최대 마나보다 크다면, 최대 마나으로 설정합니다. {0}/{1}", Current, Max);
                }
            }
        }
    }
}