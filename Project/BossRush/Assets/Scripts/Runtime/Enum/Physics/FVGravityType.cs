namespace TeamSuneat
{
    /// <summary>
    /// ForceVelocity의 중력 사용 방식을 나타내는 enum
    /// </summary>
    public enum FVGravityType
    {
        None = 0,

        /// <summary> 일반 중력 사용 </summary>
        UseGravity = 1,

        /// <summary> 커스텀 중력 사용 </summary>
        UseCustomGravity = 2,
    }
}
