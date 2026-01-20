using System;

namespace TeamSuneat
{
    /// <summary>
    /// ForceVelocity의 중력 사용 방식을 나타내는 enum
    /// Flags를 사용하여 여러 방식을 조합할 수 있습니다.
    /// </summary>
    [Flags]
    public enum FVGravityType
    {
        None = 0,

        /// <summary> 일반 중력 사용 </summary>
        UseGravity = 1 << 0,  // 1

        /// <summary> 커스텀 중력 사용 </summary>
        UseCustomGravity = 1 << 1,  // 2
    }
}
