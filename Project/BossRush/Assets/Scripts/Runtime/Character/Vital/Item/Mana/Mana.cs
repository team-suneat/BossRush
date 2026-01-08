using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 게이지 기반 마나 시스템입니다. </summary>
    [System.Serializable]
    public class Mana : VitalResource
    {
        [Title("#Mana")]
        [ReadOnly]
        [FoldoutGroup("#ManaGauge")]
        [SuffixLabel("현재 게이지 진행도 (0~1)")]
        public float GaugeProgress = 0f;

        public event Action<float> OnGaugeProgressChanged;

        public override VitalResourceTypes Type => VitalResourceTypes.Mana;

        /// <summary> 현재 온전한 마나 개수. </summary>
        public int FullManaCount => Current;

        /// <summary> 최대 온전한 마나 개수. </summary>
        public int MaxFullManaCount => Max;

        public override void LoadCurrentValue()
        {
            Current = Max;
            GaugeProgress = 0f;
            NotifyGaugeProgressChanged();
            SendGlobalEventOfChange();

            LogInfo("캐릭터의 마나를 초기화합니다. {0}/{1}", Current, Max);
        }

        public override void Initialize()
        {
            base.Initialize();
            GaugeProgress = 0f;
            NotifyGaugeProgressChanged();
        }

        public override bool AddCurrentValue(int value)
        {
            if (base.AddCurrentValue(value))
            {
                return true;
            }
            return false;
        }

        public override bool UseCurrentValue(int value)
        {
            if (base.UseCurrentValue(value))
            {
                return true;
            }
            return false;
        }

        protected override void OnAddCurrentValue(int value)
        {
            base.OnAddCurrentValue(value);
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
                if (AddFullMana())
                {
                    GaugeProgress = 0f;
                }
            }
            else
            {
                GaugeProgress = newProgress;
            }

            NotifyGaugeProgressChanged();
            LogInfo("공격 성공으로 마나 게이지를 증가합니다. 진행도: {0:F1}%", GaugeProgress * 100f);
        }

        /// <summary> 온전한 마나 추가. </summary>
        private bool AddFullMana()
        {
            if (Current >= Max)
            {
                LogWarning("온전한 마나가 최대 개수에 도달했습니다. {0}/{1}", Current, Max);
                return false;
            }

            Current++;
            SendGlobalEventOfChange();
            LogInfo("온전한 마나를 획득합니다. {0}/{1}", Current, Max);
            return true;
        }

        /// <summary> 온전한 마나 사용 시도. </summary>
        public bool TryUseFullMana()
        {
            if (Current <= 0)
            {
                LogWarning("온전한 마나가 부족합니다. 현재: {0}", Current);
                return false;
            }

            Current--;
            SendGlobalEventOfChange();
            LogInfo("온전한 마나를 사용합니다. 남은 개수: {0}/{1}", Current, Max);
            return true;
        }

        private void NotifyGaugeProgressChanged()
        {
            OnGaugeProgressChanged?.Invoke(GaugeProgress);
        }
    }
}