using TeamSuneat.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 피드백 처리를 담당하는 핸들러입니다.
    /// </summary>
    public class UIPopupFeedbackHandler : MonoBehaviour, IUIPopupFeedbackHandler
    {
        [FoldoutGroup("#Feedback Settings")]
        [SerializeField] private GameFeedbacks _openFeedback;

        [FoldoutGroup("#Feedback Settings")]
        [SerializeField] private GameFeedbacks _closeFeedback;

        public void Initialize()
        {
            // 피드백 컴포넌트 자동 찾기
            if (_openFeedback == null)
            {
                _openFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/Open");
            }

            if (_closeFeedback == null)
            {
                _closeFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/Close");
            }
        }

        public void Cleanup()
        {
            // 피드백 정리 작업이 필요한 경우 여기에 구현
        }

        public void TriggerOpenFeedback()
        {
            if (_openFeedback != null)
            {
                _openFeedback.PlayFeedbacks();
                Log.Info(LogTags.UI_Popup, "팝업 열기 피드백을 트리거했습니다.");
            }
            else
            {
                Log.Warning(LogTags.UI_Popup, "열기 피드백이 설정되지 않았습니다.");
            }
        }

        public void TriggerCloseFeedback()
        {
            if (_closeFeedback != null)
            {
                _closeFeedback.PlayFeedbacks();
                Log.Info(LogTags.UI_Popup, "팝업 닫기 피드백을 트리거했습니다.");
            }
            else
            {
                Log.Warning(LogTags.UI_Popup, "닫기 피드백이 설정되지 않았습니다.");
            }
        }

        public void SetOpenFeedback(GameFeedbacks feedback)
        {
            _openFeedback = feedback;
            Log.Info(LogTags.UI_Popup, "열기 피드백을 설정했습니다.");
        }

        public void SetCloseFeedback(GameFeedbacks feedback)
        {
            _closeFeedback = feedback;
            Log.Info(LogTags.UI_Popup, "닫기 피드백을 설정했습니다.");
        }
    }
}