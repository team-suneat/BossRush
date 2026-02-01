using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 캐릭터의 포이즈 게이지를 관리하는 클래스입니다. </summary>
    public class Poise : VitalResource
    {
        #region Field

        [Title("#Poise")]
        [ReadOnly]
        [FoldoutGroup("#PoiseGauge")]
        [SuffixLabel("브레이크 상태 여부")]
        private bool _isBroken = false;

        private Coroutine _recoveryCoroutine;
        private Coroutine _breakRecoveryCoroutine;

        #endregion Field

        #region Parameter

        public override VitalResourceTypes Type => VitalResourceTypes.Poise;

        public bool IsBroken => _isBroken;

        public new float Rate => Current.SafeDivide(100f);

        #endregion Parameter

        protected override void Awake()
        {
            base.Awake();

            if (Max == 0)
            {
                Max = 100;
            }
        }

        public override void Initialize()
        {
            LogInfo("포이즈 게이지를 초기화합니다.");

            base.Initialize();

            Current = 0;
            _isBroken = false;
            StopRecovery();
            StopBreakRecovery();

            LogInfo("포이즈 게이지를 0으로 초기화합니다. {0}/{1}", Current, Max);
        }

        public override void LoadCurrentValue()
        {
            Current = 0;
            _isBroken = false;
            StopRecovery();
            StopBreakRecovery();
            SendGlobalEventOfChange();

            LogInfo("포이즈 게이지를 0으로 불러옵니다. {0}/{1}", Current, Max);
        }

        public override void RefreshMaxValue(bool shouldAddExcessToCurrent = false)
        {
            Max = 100;
            LogInfo("포이즈 최대값을 100으로 설정합니다.");
        }

        public override bool AddCurrentValue(int value)
        {
            if (_isBroken)
            {
                LogInfo("브레이크 상태에서는 포이즈가 증가하지 않습니다.");
                return false;
            }

            if (value > 0)
            {
                int previousCurrent = Current;
                Current = Mathf.Clamp(Current + value, 0, Max);

                LogCurrentValueAdded(Type, Current - previousCurrent, Current, Max);
                NotifyValueChanged();
                SendGlobalEventOfChange();

                CheckBreak();

                if (Rate > 0f && Rate < 1f && _recoveryCoroutine == null)
                {
                    StartRecovery();
                }

                return true;
            }

            return false;
        }

        public void OnAttackSuccess()
        {
            AddPoise(0.1f);
        }

        public void OnParrySuccess()
        {
            AddPoise(0.2f);
        }

        public void OnCounterParrySuccess()
        {
            AddPoise(0.3f);
        }

        private void AddPoise(float amount)
        {
            if (_isBroken)
            {
                return;
            }

            int valueToAdd = Mathf.RoundToInt(amount * 100f);
            AddCurrentValue(valueToAdd);
        }

        private void CheckBreak()
        {
            if (Rate >= 1f && !_isBroken)
            {
                TriggerBreak();
            }
        }

        private void TriggerBreak()
        {
            _isBroken = true;
            StopRecovery();

            LogInfo("포이즈 브레이크! 3초간 기절 상태로 전환합니다.");

            if (Vital?.Owner != null)
            {
                Vital.Owner.ApplyStun(3f);
            }

            StartBreakRecovery();
        }

        private void StartRecovery()
        {
            if (Vital?.Owner == null)
            {
                return;
            }

            if (_recoveryCoroutine != null)
            {
                return;
            }

            _recoveryCoroutine = StartXCoroutine(ProcessRecovery());
            LogInfo("포이즈 자동 회복을 시작합니다.");
        }

        private void StopRecovery()
        {
            StopXCoroutine(ref _recoveryCoroutine);
        }

        private IEnumerator ProcessRecovery()
        {
            while (true)
            {
                if (Vital?.Owner == null)
                {
                    LogInfo("바이탈 또는 소유자가 없어 회복을 중지합니다.");
                    break;
                }

                if (!Vital.Owner.IsAlive)
                {
                    LogInfo("캐릭터가 생존하지 않아 회복을 중지합니다.");
                    break;
                }

                if (_isBroken)
                {
                    LogInfo("브레이크 상태로 회복을 중지합니다.");
                    break;
                }

                if (Rate <= 0f || Rate >= 1f)
                {
                    LogInfo("포이즈가 0 이하이거나 1 이상이어서 회복을 중지합니다.");
                    break;
                }

                yield return null;

                float decreaseAmount = 0.1f * Time.deltaTime;
                int valueToDecrease = Mathf.RoundToInt(decreaseAmount * 100f);

                if (valueToDecrease > 0)
                {
                    Current = Mathf.Max(0, Current - valueToDecrease);
                    NotifyValueChanged();
                    SendGlobalEventOfChange();
                }
            }

            _recoveryCoroutine = null;
        }

        private void StartBreakRecovery()
        {
            if (Vital?.Owner == null)
            {
                return;
            }

            if (_breakRecoveryCoroutine != null)
            {
                return;
            }

            _breakRecoveryCoroutine = StartXCoroutine(ProcessBreakRecovery());
            LogInfo("브레이크 포이즈 감소를 시작합니다.");
        }

        private void StopBreakRecovery()
        {
            StopXCoroutine(ref _breakRecoveryCoroutine);
        }

        private IEnumerator ProcessBreakRecovery()
        {
            float duration = 3f;
            float elapsed = 0f;
            int startValue = Current;

            while (elapsed < duration)
            {
                if (Vital?.Owner == null)
                {
                    break;
                }

                yield return null;

                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                Current = Mathf.RoundToInt(Mathf.Lerp(startValue, 0, progress));
                NotifyValueChanged();
                SendGlobalEventOfChange();
            }

            Current = 0;
            _isBroken = false;
            NotifyValueChanged();
            SendGlobalEventOfChange();

            LogInfo("브레이크 포이즈 감소 완료. 브레이크 상태 해제.");

            _breakRecoveryCoroutine = null;
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            StopRecovery();
            StopBreakRecovery();
        }
    }
}

