using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 콜백 처리를 담당하는 핸들러입니다.
    /// </summary>
    public class UIPopupCallbackHandler : MonoBehaviour, IUIPopupCallbackHandler
    {
        private UnityAction<bool> _closeCallback;

        public void Initialize()
        {
            _closeCallback = null;
        }

        public void Cleanup()
        {
            ClearCallbacks();
        }

        public void RegisterCloseCallback(UnityAction<bool> action)
        {
            if (action != null)
            {
                _closeCallback += action;
                Log.Info(LogTags.UI_Popup, $"팝업 닫기 콜백을 등록했습니다: {action.Target}.{action.Method.Name}");
            }
        }

        public void UnregisterCloseCallback(UnityAction<bool> action)
        {
            if (action != null)
            {
                _closeCallback -= action;
                Log.Info(LogTags.UI_Popup, $"팝업 닫기 콜백을 해제했습니다: {action.Target}.{action.Method.Name}");
            }
        }

        public void InvokeCloseCallback(bool result)
        {
            if (Log.LevelInfo)
            {
                if (_closeCallback != null)
                {
                    System.Delegate[] delegates = _closeCallback.GetInvocationList();
                    Log.Info(LogTags.UI_Popup, $"팝업 닫기 콜백 {delegates.Length}개를 호출합니다.");
                    foreach (System.Delegate d in delegates)
                    {
                        Log.Info(LogTags.UI_Popup, $"- 콜백 호출: {d.Target}.{d.Method}");
                    }
                }
            }

            _closeCallback?.Invoke(result);
        }

        public void ClearCallbacks()
        {
            Log.Info(LogTags.UI_Popup, "팝업 닫기 콜백을 초기화합니다.");
            _closeCallback = null;
        }
    }
}