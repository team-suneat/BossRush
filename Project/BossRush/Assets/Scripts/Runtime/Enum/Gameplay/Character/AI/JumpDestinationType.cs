namespace TeamSuneat
{
    /// <summary>
    /// TargetJump 목적지 해석 방식.
    /// </summary>
    public enum JumpDestinationType
    {
        None,

        /// <summary>PositionGroup에서 정렬 전략에 따라 선택한 지점.</summary>
        PositionGroup,

        /// <summary>Owner의 현재 타겟 캐릭터 위치.</summary>
        OwnerTarget,
    }
}
