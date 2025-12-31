namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 입력 차단 처리를 담당하는 핸들러 인터페이스입니다.
    /// </summary>
    public interface IUIPopupInputBlockHandler : IUIPopupHandler
    {
        /// <summary>
        /// 입력 차단을 시작합니다.
        /// </summary>
        void StartBlockInput();

        /// <summary>
        /// 입력 차단을 설정합니다.
        /// </summary>
        void SetBlockInput();

        /// <summary>
        /// 입력 차단을 해제합니다.
        /// </summary>
        void ResetBlockInput();

        /// <summary>
        /// 입력 차단 여부를 가져옵니다.
        /// </summary>
        bool IsBlockInput { get; }

        /// <summary>
        /// 팝업 스폰 차단 여부를 설정합니다.
        /// </summary>
        /// <param name="blockSpawnPopup">팝업 스폰 차단 여부</param>
        void SetBlockSpawnPopup(bool blockSpawnPopup);

        /// <summary>
        /// 팝업 설정을 구성합니다.
        /// </summary>
        /// <param name="isOpening">팝업이 열리는 중인지 여부</param>
        void ConfigurePopupSettings(bool isOpening);
    }
}