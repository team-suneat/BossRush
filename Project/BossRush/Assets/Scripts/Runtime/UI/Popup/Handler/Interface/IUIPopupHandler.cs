namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup 핸들러들의 기본 인터페이스입니다.
    /// </summary>
    public interface IUIPopupHandler
    {
        /// <summary>
        /// 핸들러를 초기화합니다.
        /// </summary>
        void Initialize();

        /// <summary>
        /// 핸들러를 정리합니다.
        /// </summary>
        void Cleanup();
    }
}