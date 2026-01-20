using System;

namespace TeamSuneat
{
    /// <summary>
    /// ForceVelocity의 충돌 시 정지 방식을 나타내는 enum
    /// Flags를 사용하여 여러 방식을 조합할 수 있습니다.
    /// </summary>
    [Flags]
    public enum FVStopOnCollisionType
    {
        None = 0,

        /// <summary> 벽 충돌 시 X축 정지 </summary>
        StopXOnHitWall = 1 << 0,  // 1

        /// <summary> 지면 충돌 시 X축 정지 </summary>
        StopXOnHitGround = 1 << 1,  // 2

        /// <summary> 지면 충돌 시 Y축 정지 </summary>
        StopYOnHitGround = 1 << 2,  // 4
    }
}
