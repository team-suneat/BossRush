using System;

namespace TeamSuneat
{
    /// <summary>
    /// ForceVelocity의 마찰 및 저항 사용 방식을 나타내는 enum
    /// Flags를 사용하여 여러 방식을 조합할 수 있습니다.
    /// </summary>
    [Flags]
    public enum FVFrictionType
    {
        None = 0,

        /// <summary> 마찰 사용 </summary>
        UseForceFriction = 1 << 0,  // 1

        /// <summary> 공기 저항 사용 </summary>
        UseAirResist = 1 << 1,  // 2

        /// <summary> 마찰 + 공기 저항 사용 </summary>
        UseBoth = UseForceFriction | UseAirResist,  // 3
    }
}
