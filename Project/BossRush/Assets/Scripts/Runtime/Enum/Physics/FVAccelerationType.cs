using System;

namespace TeamSuneat
{
    /// <summary>
    /// ForceVelocity의 가속도 사용 방식을 나타내는 enum
    /// Flags를 사용하여 여러 방식을 조합할 수 있습니다.
    /// </summary>
    [Flags]
    public enum FVAccelerationType
    {
        None = 0,

        /// <summary> X축 가속도 사용 </summary>
        UseAccelerationX = 1 << 0,  // 1

        /// <summary> Y축 가속도 사용 </summary>
        UseAccelerationY = 1 << 1,  // 2

        /// <summary> X축 + Y축 가속도 사용 </summary>
        UseAccelerationBoth = UseAccelerationX | UseAccelerationY,  // 3
    }
}
