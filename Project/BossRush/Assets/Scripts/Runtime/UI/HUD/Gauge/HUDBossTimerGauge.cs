using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDBossTimerGauge : MonoBehaviour
    {
        [SerializeField] private UIGauge _timerGauge;

        private bool _isBossModeActive;

        private void Awake()
        {
            if (_timerGauge != null)
            {
                _timerGauge.SetActive(false);
            }
        }

        public void LogicUpdate()
        {
            if (!_isBossModeActive)
            {
                return;
            }

            UpdateTimer();
            _timerGauge?.LogicUpdate();
        }

        public void OnBossModeEntered()
        {
            if (_timerGauge == null)
            {
                Log.Warning(LogTags.UI_Gauge, "[BossTimerGauge] 타이머 게이지가 설정되지 않았습니다. {0}", this.GetHierarchyName());
                return;
            }

            _isBossModeActive = true;
            _timerGauge.SetActive(true);
            _timerGauge.ResetFrontValue();

            Log.Info(LogTags.UI_Gauge, "[BossTimerGauge] 보스 모드 진입. 타이머 게이지 활성화. 제한 시간: {0}초, {1}",
                GameDefine.BOSS_MODE_TIME_LIMIT, this.GetHierarchyName());
        }

        public void OnBossModeExited()
        {
            if (_timerGauge == null)
            {
                Log.Warning(LogTags.UI_Gauge, "[BossTimerGauge] 타이머 게이지가 설정되지 않았습니다. {0}", this.GetHierarchyName());
                return;
            }

            _isBossModeActive = false;
            _timerGauge.SetActive(false);
            _timerGauge.ResetFrontValue();
            _timerGauge.ResetValueText();

            Log.Info(LogTags.UI_Gauge, "[BossTimerGauge] 보스 모드 종료. 타이머 게이지 비활성화. {0}", this.GetHierarchyName());
        }

        private void UpdateTimer()
        {
            if (_timerGauge == null)
            {
                return;
            }

            float remainingTime = GameTimerManager.Instance.GetRemainingTime(GameTimerLabels.BossMode);
            float rate = remainingTime.SafeDivide01(GameDefine.BOSS_MODE_TIME_LIMIT);

            _timerGauge.SetFrontValue(rate);
            _timerGauge.SetValueText(FormatTime(remainingTime));
        }

        private string FormatTime(float seconds)
        {
            int totalSeconds = Mathf.CeilToInt(seconds);
            int minutes = totalSeconds.SafeDivideToInt(60);
            int secs = totalSeconds % 60;
            return $"{minutes:D2}:{secs:D2}";
        }
    }
}