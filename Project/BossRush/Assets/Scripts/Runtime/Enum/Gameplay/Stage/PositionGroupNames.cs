namespace TeamSuneat
{
    /// <summary>
    /// 포지션 그룹을 식별하기 위한 키. PositionGroupManager에서 해당 키로 검색합니다.
    /// </summary>
    public enum PositionGroupNames
    {
        None,

        /// <summary>
        /// 첫 번째 보스의 네 번째 패턴 - 패링 불가 공격 (점프 + 공격) 착지 포지션 그룹
        /// </summary>
        Boss1Pattern4JumpLand,

        /// <summary>
        /// 첫 번째 보스의 네 번째 패턴 - 패링 불가 공격 (점프 + 공격) 바라보는 목표 포지션 그룹
        /// </summary>
        Boss1Pattern4FaceToPositionGroup,
    }
}
