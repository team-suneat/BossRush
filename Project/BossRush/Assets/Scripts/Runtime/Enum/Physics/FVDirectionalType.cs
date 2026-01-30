namespace TeamSuneat
{
    /// <summary>
    /// ForceVelocity의 방향성 타입을 나타내는 enum
    /// </summary>
    public enum FVDirectionalType
    {
        None,

        /// <summary> 바라보는 방향으로 </summary>
        Facing,

        /// <summary> 반대 방향으로 </summary>
        Reverse,

        /// <summary> 넉백: 공격자가 바라보는 방향으로 밀기 </summary>
        AttackerFacing,

        /// <summary> 넉백: 공격자 반대 방향으로 밀기 </summary>
        AttackerReverse,

        /// <summary> 넉백: 공격자 기준 피격자 위치(오른쪽/왼쪽)로 밀기 </summary>
        RelativeToAttacker,
    }
}
