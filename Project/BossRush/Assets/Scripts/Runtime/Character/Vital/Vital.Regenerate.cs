using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        private Coroutine _regenerateCoroutine;

        protected int LifeRegeneratePoint { get; set; }

        private const float DEFAULT_REGENERATE_INTERVAL_TIME = 60f;

        public void StartRegenerate()
        {
            if (Owner != null)
            {
                Clear();

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
                if (LifeRegeneratePoint == 0)
                {
                    LogProgressFailedToRegenerateByZeroPoint();
                    break;
                }

                yield return new WaitForSeconds(DEFAULT_REGENERATE_INTERVAL_TIME);

                Regenerate();
            }

            _regenerateCoroutine = null;
        }

        private void Regenerate()
        {
            if (Life != null)
            {
                if (LifeRegeneratePoint > 0)
                {
                    Life.Regenerate(LifeRegeneratePoint);
                    Life.SpawnHealFloatyText(LifeRegeneratePoint);
                }
                else if (LifeRegeneratePoint < 0)
                {
                    // 소모량을 양수로 넘깁니다.
                    Life.Use(LifeRegeneratePoint * -1, Owner, true);
                }
            }
        }

        private void Clear()
        {
            Log.Info(LogTags.Vital, "{0}, 생명력/마나 재생력을 비활성화합니다.", Owner.Name.ToLogString());

            LifeRegeneratePoint = 0;
        }
    }
}