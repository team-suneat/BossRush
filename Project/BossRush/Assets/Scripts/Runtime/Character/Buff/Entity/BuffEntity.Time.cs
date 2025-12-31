using System.Collections;
using Lean.Pool;
using UnityEngine;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        public void ResetElapsedTime()
        {
            if (ElapsedTime > 0)
            {
                LogProgress("버프의 지난 시간을 초기화합니다. {0} ▶ 0", ElapsedTime);

                ElapsedTime = 0;
            }

            if (_characterStun != null)
            {
                _characterStun.ResetElapsedTime();
            }

            if (AssetData.SetStackByElapsedTimeOnApply)
            {
                if (SetStackCount(Mathf.FloorToInt(ElapsedTime)))
                {
                    RefreshStats();
                }
            }
        }

        public void StartTimerExtern()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            if (Duration > 0)
            {
                if (AssetData.Interval > 0)
                {
                    StartRepetitiveTimer();
                }
                else
                {
                    StartDurationTimer();
                }
            }
        }

        // 지속 시간

        private void SetupDuration()
        {
            Duration = StatEx.GetValueByLevel(AssetData.Duration, AssetData.DurationByLevel, Level);
            Duration = StatEx.GetValueByStack(Duration, AssetData.DurationByStack, Stack);
            Duration += _additionalDuration;

            if (AssetData.DurationByStat != StatNames.None)
            {
                Duration += Owner.Stat.FindValueOrDefault(AssetData.DurationByStat);
            }

            if (Duration > 0)
            {
                LogInfo($"버프의 지속시간({Duration.ToSelectString()}초)을 설정합니다.");
            }
        }

        public void SetAdditionalDuration(float duration, float maxDuration)
        {
            if (_additionalDuration <= maxDuration && !duration.IsZero())
            {
                _additionalDuration += duration;
                LogProgress($"지속시간을 추가합니다. +{duration} ({_additionalDuration}/{maxDuration})");
            }
            else if (!_additionalDuration.IsEqual(maxDuration))
            {
                _additionalDuration = maxDuration;
                LogProgress($"최대 추가 지속시간에 도달하였습니다. {_additionalDuration}");
            }
            else
            {
                return;
            }

            SetupDuration();
        }

        private void ResetAdditionalDuration()
        {
            _additionalDuration = 0;
        }

        // 간격 시간

        private void SetupIntervalTime()
        {
            IntervalTime = AssetData.Interval;
        }

        // 지속 버프 타이머

        private void StartDurationTimer()
        {
            if (Duration <= 0 || Duration > 100)
            {
                // 지속시간이 설정되지 않았거나 너무 높은 지속시간을 가졌다면 타이머를 시작하지 않습니다.
                return;
            }

            if (_durationCoroutine != null)
            {
                return;
            }

            _durationCoroutine = StartXCoroutine(ProcessDurationTimer());
        }

        public void StopDurationTimer()
        {
            if (_durationCoroutine != null)
            {
                LogProgress("지속 버프 타이머를 중지합니다.");
                StopXCoroutine(ref _durationCoroutine);
                ResetAdditionalDuration();
            }
        }

        private IEnumerator ProcessDurationTimer()
        {
            if (AssetData.DelayTime > 0)
            {
                yield return new WaitForSeconds(AssetData.DelayTime);
            }

            WaitForFixedUpdate wait = new();

            LogProgress("버프의 타이머를 시작합니다. 지속시간: {0}", Duration);
            Apply();
            ResetElapsedTime();

            while (Duration > ElapsedTime)
            {
                yield return null;
                ElapsedTime += Time.deltaTime;
            }

            LogProgress("버프의 타이머를 종료합니다. 지속시간: {0}", Duration);
            OnCompleteTimer();
        }

        // 반복 버프 타이머

        private void StartRepetitiveTimer()
        {
            if (Mathf.Approximately(0f, Duration))
            {
                return;
            }

            if (_repetitiveCoroutine != null)
            {
                return;
            }

            _repetitiveCoroutine = StartXCoroutine(ProcessRepetitiveTimer());
        }

        public void StopRepetitiveTimer()
        {
            if (_repetitiveCoroutine != null)
            {
                LogProgress("반복 적용 버프 타이머를 중지합니다.");
                StopXCoroutine(ref _repetitiveCoroutine);
                ResetAdditionalDuration();
            }
        }

        private IEnumerator ProcessRepetitiveTimer()
        {
            // 딜레이가 있고 테스트 모드가 아니라면 초기 대기
            if (AssetData.DelayTime > 0)
            {
                yield return new WaitForSeconds(AssetData.DelayTime);
            }

            ElapsedTime = 0f;
            float intervalTime = IntervalTime;
            float elapsedTimeForRepeat = 0f;
            WaitForFixedUpdate wait = new();

            LogProgress("버프의 타이머를 시작합니다. 지속시간: {0}, 간격시간: {1}", Duration, intervalTime);

            // 최초 Apply() 호출 제거 ▶ 최초 간격 시간이 지난 후부터 적용
            while (ElapsedTime < Duration)
            {
                yield return wait;

                // FixedUpdate에서 증가하는 시간 사용
                float deltaTime = Time.fixedDeltaTime;
                ElapsedTime += deltaTime;
                elapsedTimeForRepeat += deltaTime;

                // 간격 시간이 도달하면 Apply() 호출
                if (elapsedTimeForRepeat >= intervalTime)
                {
                    LogProgress("버프의 간격 타이머에 따라 적용합니다. 현재 시간: {0}", ElapsedTime);
                    Apply();
                    elapsedTimeForRepeat -= intervalTime; // 남은 시간 보존
                }
            }

            LogProgress("버프의 타이머를 종료합니다. 지속시간: {0}, 간격시간: {1}", Duration, intervalTime);
            OnCompleteTimer();
        }

        // 타이머 종료

        private void OnCompleteTimer()
        {
            Owner.Buff.Remove(Name);

            _durationCoroutine = null;
            _repetitiveCoroutine = null;
        }

        // 버프 적용 후 유휴 타이머

        private void StartRestTimer()
        {
            if (AssetData.RestTime > 0)
            {
                float restTime = AssetData.RestTime;

                if (Level > 1)
                {
                    restTime += AssetData.RestTimeByLevel * (Level - 1);
                }

                restTime -= Duration;

                Owner.Buff.StartRestTimer(Name, restTime);
            }
        }
    }
}