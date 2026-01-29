namespace TeamSuneat
{
    /// <summary>
    /// 포지션 그룹을 식별하기 위한 키. PositionGroupManager에서 해당 키로 검색합니다.
    /// </summary>
    public enum PositionGroupNames
    {
        None,

        /// <summary>
        /// 보스 1 패턴 3 패링 불가 공격의 점프 착지 포지션 그룹
        /// </summary>
        Boss1Pattern3JumpLand,

        /// <summary>
        /// 보스 1 패턴 3 패링 불가 공격의 바라보는 목표 포지션 그룹
        /// </summary>
        Boss1Pattern3FaceToPositionGroup,
    }
}
