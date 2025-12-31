namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 입력 처리를 담당하는 핸들러 인터페이스입니다.
    /// </summary>
    public interface IUIPopupInputHandler : IUIPopupHandler
    {
        /// <summary>
        /// 입력 시스템을 설정합니다.
        /// </summary>
        /// <param name="maxIndex">최대 타겟 인덱스</param>
        void SetupInput(int maxIndex);

        /// <summary>
        /// 지정된 인덱스의 타겟을 활성화합니다.
        /// </summary>
        /// <param name="index">활성화할 타겟의 인덱스</param>
        void ActivateTarget(int index);

        /// <summary>
        /// 다음 타겟으로 이동합니다.
        /// </summary>
        void NextTarget();

        /// <summary>
        /// 이전 타겟으로 이동합니다.
        /// </summary>
        void PrevTarget();

        /// <summary>
        /// 지정된 인덱스의 타겟을 선택합니다.
        /// </summary>
        /// <param name="index">선택할 타겟의 인덱스</param>
        void SelectTarget(int index);

        /// <summary>
        /// 현재 타겟 인덱스를 가져옵니다.
        /// </summary>
        int CurrentTargetIndex { get; }

        /// <summary>
        /// 최대 타겟 인덱스를 가져옵니다.
        /// </summary>
        int MaxTargetIndex { get; }
    }
}