using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 콜백 처리를 담당하는 핸들러 인터페이스입니다.
    /// </summary>
    public interface IUIPopupCallbackHandler : IUIPopupHandler
    {
        /// <summary>
        /// 팝업 닫기 콜백을 등록합니다.
        /// </summary>
        /// <param name="action">등록할 콜백 액션</param>
        void RegisterCloseCallback(UnityAction<bool> action);

        /// <summary>
        /// 팝업 닫기 콜백을 해제합니다.
        /// </summary>
        /// <param name="action">해제할 콜백 액션</param>
        void UnregisterCloseCallback(UnityAction<bool> action);

        /// <summary>
        /// 닫기 콜백을 호출합니다.
        /// </summary>
        /// <param name="result">닫기 결과</param>
        void InvokeCloseCallback(bool result);

        /// <summary>
        /// 모든 콜백을 초기화합니다.
        /// </summary>
        void ClearCallbacks();
    }
}