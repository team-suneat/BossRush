using System;
using System.Collections.Generic;

namespace TeamSuneat
{
    /// <summary>
    /// 게임 내 제한 시간(타이머) 관리를 담당하는 싱글톤 클래스
    /// 보스 모드, 던전, 이벤트 등 다양한 제한 시간을 중앙에서 관리합니다.
    /// </summary>
    public class GameTimerManager : SingletonMonoBehaviour<GameTimerManager>
    {
        #region Private Fields

        private Dictionary<string, GameTimer> _timers = new Dictionary<string, GameTimer>();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// 타이머를 시작합니다.
        /// </summary>
        /// <param name="timerId">타이머 고유 ID</param>
        /// <param name="duration">지속 시간 (초)</param>
        /// <param name="onExpired">만료 시 콜백</param>
        /// <param name="onTick">틱마다 호출되는 콜백 (선택)</param>
        /// <param name="tickInterval">틱 간격 (초, 기본값: 0.1초)</param>
        public void StartTimer(string timerId, float duration,
            Action onExpired,
            Action<float> onTick = null,
            float tickInterval = 0.1f)
        {
            if (_timers.ContainsKey(timerId))
            {
                Log.Warning(LogTags.Time, "타이머가 이미 실행 중입니다: {0}", timerId);
                StopTimer(timerId);
            }

            var timer = new GameTimer(timerId, duration, onExpired, onTick, tickInterval);
            _timers[timerId] = timer;
            timer.Start(this);

            Log.Info(LogTags.Time, "타이머 시작: {0}, 지속 시간: {1}초", timerId, duration);
        }

        /// <summary>
        /// 타이머를 중지합니다.
        /// </summary>
        public void StopTimer(string timerId)
        {
            if (_timers.TryGetValue(timerId, out GameTimer timer))
            {
                timer.Stop(this);
                _timers.Remove(timerId);
                Log.Info(LogTags.Time, "타이머 중지: {0}", timerId);
            }
        }

        /// <summary>
        /// 타이머를 일시정지합니다.
        /// </summary>
        public void PauseTimer(string timerId)
        {
            if (_timers.TryGetValue(timerId, out GameTimer timer))
            {
                timer.Pause();
                Log.Info(LogTags.Time, "타이머 일시정지: {0}", timerId);
            }
        }

        /// <summary>
        /// 타이머를 재개합니다.
        /// </summary>
        public void ResumeTimer(string timerId)
        {
            if (_timers.TryGetValue(timerId, out GameTimer timer))
            {
                timer.Resume();
                Log.Info(LogTags.Time, "타이머 재개: {0}", timerId);
            }
        }

        /// <summary>
        /// 남은 시간을 반환합니다.
        /// </summary>
        public float GetRemainingTime(string timerId)
        {
            if (_timers.TryGetValue(timerId, out GameTimer timer))
            {
                return timer.RemainingTime;
            }
            return 0f;
        }

        /// <summary>
        /// 경과 시간을 반환합니다.
        /// </summary>
        public float GetElapsedTime(string timerId)
        {
            if (_timers.TryGetValue(timerId, out GameTimer timer))
            {
                return timer.ElapsedTime;
            }
            return 0f;
        }

        /// <summary>
        /// 타이머가 실행 중인지 확인합니다.
        /// </summary>
        public bool IsTimerActive(string timerId)
        {
            return _timers.ContainsKey(timerId) && _timers[timerId].IsActive;
        }

        /// <summary>
        /// 모든 타이머를 중지합니다.
        /// </summary>
        public void StopAllTimers()
        {
            foreach (var timer in _timers.Values)
            {
                timer.Stop(this);
            }
            _timers.Clear();
            Log.Info(LogTags.Time, "모든 타이머 중지");
        }

        #endregion Public Methods
    }
}