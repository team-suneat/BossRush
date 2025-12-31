using TeamSuneat.Feedbacks;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 피드백 처리를 담당하는 핸들러 인터페이스입니다.
    /// </summary>
    public interface IUIPopupFeedbackHandler : IUIPopupHandler
    {
        /// <summary>
        /// 팝업 열기 피드백을 트리거합니다.
        /// </summary>
        void TriggerOpenFeedback();

        /// <summary>
        /// 팝업 닫기 피드백을 트리거합니다.
        /// </summary>
        void TriggerCloseFeedback();

        /// <summary>
        /// 열기 피드백을 설정합니다.
        /// </summary>
        /// <param name="feedback">설정할 피드백</param>
        void SetOpenFeedback(GameFeedbacks feedback);

        /// <summary>
        /// 닫기 피드백을 설정합니다.
        /// </summary>
        /// <param name="feedback">설정할 피드백</param>
        void SetCloseFeedback(GameFeedbacks feedback);
    }
}