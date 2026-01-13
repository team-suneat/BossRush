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
        private float _factor = 1.0f; // 100%

        public void LogicUpdate()
        {
            if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                return;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    SetFactor(0.1f);
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SetFactor(1f);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SetFactor(2f);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    SetFactor(3f);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    SetFactor(4f);
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    SetFactor(5f);
                }
            }
        }

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
    }
}