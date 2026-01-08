using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        private Coroutine _regenerateCoroutine;

        public float PulseRegenerateRate { get; set; }

        public void StartRegenerate()
        {
            if (Owner != null)
            {
                StartRegenerateCoroutine();
            }
        }

        private void StartRegenerateCoroutine()
        {
            if (_regenerateCoroutine == null)
            {
                _regenerateCoroutine = StartXCoroutine(ProcessRegenerate());
            }
            else
            {
                LogWarningRegenerate();
            }
        }

        private void StopRegenerateCoroutine()
        {
            StopXCoroutine(ref _regenerateCoroutine);
        }

        private IEnumerator ProcessRegenerate()
        {
            LogInfoStartRegeneration();

            while (true)
            {
                if (!IsAlive)
                {
                    LogProgressFailedToRegenerateByNotAlive();
                    break;
                }
                if (PulseRegenerateRate <= 0f)
                {
                    LogProgressFailedToRegenerateByZeroPoint();
                    break;
                }

                yield return null;

                RegeneratePulse();
            }

            _regenerateCoroutine = null;
        }

        void RegeneratePulse()
        {
            if (Pulse != null)
            {
                if (PulseRegenerateRate > 0f)
                {
                    // 1초당 값에 Time.deltaTime을 곱하여 프레임당 값으로 변환
                    // 펄스 재생: 게이지 진행도 증가 (0~1 범위)
                    float frameGainAmount = PulseRegenerateRate * Time.deltaTime;
                    float gainAmount = Mathf.Clamp01(frameGainAmount);
                    if (gainAmount > 0f)
                    {
                        Pulse.OnAttackSuccess(gainAmount);
                    }
                }
                else if (PulseRegenerateRate < 0f)
                {
                    // 1초당 값에 Time.deltaTime을 곱하여 프레임당 값으로 변환
                    // 펄스 소모: 온전한 펄스 사용
                    float frameConsumeRate = Mathf.Abs(PulseRegenerateRate * Time.deltaTime);
                    int consumeCount = Mathf.CeilToInt(frameConsumeRate);
                    for (int i = 0; i < consumeCount; i++)
                    {
                        Pulse.TryUseFullPulse();
                    }
                }
            }
        }

        private void Clear()
        {
            Log.Info(LogTags.Vital, "{0}, 생명력/마나 재생력을 비활성화합니다.", Owner.Name.ToLogString());

            PulseRegenerateRate = 0f;
        }
    }
}