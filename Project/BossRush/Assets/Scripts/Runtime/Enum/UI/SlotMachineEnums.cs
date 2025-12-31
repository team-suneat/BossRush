namespace TeamSuneat
{
    /// <summary>
    /// 슬롯 머신 상태
    /// </summary>
    public enum SlotMachineState
    {
        None,
        Spinning,       // 슬롯이 돌아가는 중
        Stopping,       // 슬롯이 멈추는 중
        Stopped,        // 모든 슬롯이 멈춤
        Result          // 결과 표시 중
    }

    /// <summary>
    /// 개별 슬롯 상태
    /// </summary>
    public enum SlotState
    {
        None,
        Idle,           // 대기
        Spinning,       // 돌아가는 중
        Stopping,       // 멈추는 중
        Stopped         // 멈춤
    }

    /// <summary>
    /// 슬롯 결과 타입
    /// </summary>
    public enum SlotResultType
    {
        None,
        Character,      // 캐릭터
        Item,          // 아이템
        Gold,          // 골드
        Experience     // 경험치
    }
}
