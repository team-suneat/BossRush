using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    /// <summary>
    /// 게임 타임스케일 관리를 담당하는 싱글톤 클래스
    /// 슬로우 모션 등 연출 효과를 위한 타임스케일 조절을 지원합니다.
    /// </summary>
    public class GameTimeManager : Singleton<GameTimeManager>
    {
        #region Private Fields

        private float _factor = 1.0f; // 100%

        #endregion Private Fields

        #region Public Methods

        public void SetFactor(float factor, bool useSetScale = true)
        {
            _factor = factor;

            if (useSetScale)
            {
                Time.timeScale = _factor;
                Log.Info(LogTags.Time, "시간 스케일을 {0}로 설정합니다.", ValueStringEx.GetPercentString(Time.timeScale));
            }
        }

        public IEnumerator ActivateSlowMotion(float duration, float factor, UnityAction onCompleted)
        {
            Time.timeScale = factor;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = _factor;

            onCompleted?.Invoke();
        }

        #endregion Public Methods
    }
}