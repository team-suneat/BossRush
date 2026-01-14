using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public class GameTimeManager : SingletonMonoBehaviour<GameTimeManager>
    {
        private float _factor = 1.0f;
        private Coroutine _slowMotionCoroutine;
        private UnityAction _onCompletedSlowMotion;

        public void LogicUpdate()
        {
            if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                return;
            }

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SetFactor(0.1f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetFactor(1f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetFactor(2f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetFactor(3f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetFactor(4f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetFactor(5f);
            }
        }

        public void SetFactor(float factor, bool useSetScale = true)
        {
            _factor = factor;

            if (!useSetScale)
            {
                return;
            }

            Time.timeScale = _factor;
            Log.Info(LogTags.Time, "시간 스케일을 {0}로 설정합니다.", ValueStringEx.GetPercentString(Time.timeScale));
        }

        public void StartSlowMotion(float duration, float factor, UnityAction onCompleted = null)
        {
            StopSlowMotion();

            _onCompletedSlowMotion = onCompleted;
            _slowMotionCoroutine = StartXCoroutine(ActivateSlowMotionCoroutine(duration, factor));
        }

        public void StopSlowMotion(bool invokeOnCompleted = false)
        {
            if (_slowMotionCoroutine == null)
            {
                _onCompletedSlowMotion = null;
                return;
            }

            StopXCoroutine(ref _slowMotionCoroutine);
            Time.timeScale = _factor;
            Log.Info(LogTags.Time, "슬로우 모션을 중단하고 시간 스케일을 {0}로 복원합니다.", ValueStringEx.GetPercentString(Time.timeScale));

            if (invokeOnCompleted)
            {
                _onCompletedSlowMotion?.Invoke();
            }

            _onCompletedSlowMotion = null;
        }

        private IEnumerator ActivateSlowMotionCoroutine(float duration, float factor)
        {
            Time.timeScale = factor;
            Log.Info(LogTags.Time, "슬로우 모션을 시작합니다. 시간 스케일: {0}, 지속 시간: {1}초", ValueStringEx.GetPercentString(Time.timeScale), duration);

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = _factor;
            Log.Info(LogTags.Time, "슬로우 모션이 종료되었습니다. 시간 스케일을 {0}로 복원합니다.", ValueStringEx.GetPercentString(Time.timeScale));

            _slowMotionCoroutine = null;
            _onCompletedSlowMotion?.Invoke();
            _onCompletedSlowMotion = null;
        }
    }
}