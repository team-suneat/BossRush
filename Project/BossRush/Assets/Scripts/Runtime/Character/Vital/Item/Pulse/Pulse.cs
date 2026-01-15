using Sirenix.OdinInspector;
using System;
using System.Collections;
using TeamSuneat.Setting;
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

        [Title("#Burnout")]
        [ReadOnly]
        [FoldoutGroup("#PulseGauge")]
        [SuffixLabel("번아웃 상태 여부")]
        private bool _isBurnout = false;

        public event Action<bool> OnBurnoutStateChanged;

        [Title("#Regenerate")]
        [ReadOnly]
        [FoldoutGroup("#PulseGauge")]
        [SuffixLabel("재생 비율")]
        public float RegenerateRate { get; set; }

        private Coroutine _regenerateCoroutine;

        #endregion Field

        #region Parameter

        public override VitalResourceTypes Type => VitalResourceTypes.Pulse;

        public bool IsBurnout => _isBurnout;

        public bool CanUsePulse => Current >= 1 && !_isBurnout;

        #endregion Parameter

        public override void Initialize()
        {
            LogInfo("패링 게이지를 초기화합니다.");

            base.Initialize();

            // 전투 시작 시 항상 풀 게이지
            LoadCurrentValue();

            GaugeProgress = 0f;
            _isBurnout = false;
            NotifyGaugeProgressChanged();
        }

        public override void LoadCurrentValue()
        {
            Current = Max;
            GaugeProgress = 0f;
            _isBurnout = false;
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

        //

        public void OnAttackSuccess(float gainAmount = 0f)
        {
            if (gainAmount <= 0f)
            {
                return;
            }

            float newProgress = GaugeProgress + gainAmount;
            if (newProgress >= 1f)
            {
                if (AddCurrentValue(1))
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

        public void OnRegenerate(float gainAmount)
        {
            if (gainAmount <= 0f)
            {
                return;
            }

            float newProgress = GaugeProgress + gainAmount;
            if (newProgress >= 1f)
            {
                if (AddCurrentValue(1))
                {
                    GaugeProgress = 0f;
                }
            }
            else if (newProgress < 0f)
            {
                GaugeProgress = 0f;
            }
            else
            {
                GaugeProgress = newProgress;
            }

            NotifyGaugeProgressChanged();
        }

        public void OnParrySuccess()
        {
            LogInfo("패링 성공! 게이지를 회복합니다.");
            AddCurrentValue(1);
        }

        public override bool UseCurrentValue(int value, DamageResult damageResult)
        {
            if (GameSetting.Instance.Cheat.IsNotCostPulse)
            {
                return true;
            }

            bool result = base.UseCurrentValue(value, damageResult);

            if (result && Current == 0 && !_isBurnout)
            {
                _isBurnout = true;
                NotifyBurnoutStateChanged();
                LogInfo("펄스가 0이 되어 번아웃 상태로 진입합니다.");
                StartRegenerate();
            }

            return result;
        }

        public override bool AddCurrentValue(int value)
        {
            bool wasBurnout = _isBurnout;
            bool result = base.AddCurrentValue(value);

            if (result && wasBurnout && Current >= 3)
            {
                _isBurnout = false;
                NotifyBurnoutStateChanged();
                LogInfo("펄스가 3이 되어 번아웃 상태가 해제됩니다.");
                StopRegenerate();
            }

            return result;
        }

        private void NotifyGaugeProgressChanged()
        {
            OnGaugeProgressChanged?.Invoke(GaugeProgress);
        }

        private void NotifyBurnoutStateChanged()
        {
            OnBurnoutStateChanged?.Invoke(_isBurnout);
        }

        //

        public void StartRegenerate()
        {
            if (Vital?.Owner == null)
            {
                return;
            }

            if (_regenerateCoroutine != null)
            {
                LogWarning("펄스 재생 코루틴이 이미 실행 중입니다.");
                return;
            }

            if (RegenerateRate <= 0f)
            {
                LogInfo("재생 비율이 0 이하이므로 재생을 시작하지 않습니다.");
                return;
            }

            _regenerateCoroutine = StartXCoroutine(ProcessRegenerate());
            LogInfo("펄스 재생을 시작합니다. 재생 비율: {0}", RegenerateRate);
        }

        public void StopRegenerate()
        {
            StopXCoroutine(ref _regenerateCoroutine);
        }

        private IEnumerator ProcessRegenerate()
        {
            while (true)
            {
                if (Vital?.Owner == null)
                {
                    LogInfo("바이탈 또는 소유자가 없어 재생을 중지합니다.");
                    break;
                }

                if (!Vital.Owner.IsAlive)
                {
                    LogInfo("캐릭터가 생존하지 않아 재생을 중지합니다.");
                    break;
                }

                if (RegenerateRate <= 0f)
                {
                    LogInfo("재생 비율이 0 이하로 변경되어 재생을 중지합니다.");
                    break;
                }

                if (!_isBurnout)
                {
                    LogInfo("번아웃 상태가 해제되어 재생을 중지합니다.");
                    break;
                }

                yield return null;

                float frameGainAmount = Mathf.Clamp01(RegenerateRate * Time.deltaTime);
                OnRegenerate(frameGainAmount);
            }

            _regenerateCoroutine = null;
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            StopRegenerate();
        }
    }
}