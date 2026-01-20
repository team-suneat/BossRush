using System;

namespace TeamSuneat
{
    /// <summary>
    /// ForceVelocity의 무시 옵션을 나타내는 enum
    /// Flags를 사용하여 여러 옵션을 조합할 수 있습니다.
    /// </summary>
    [Flags]
    public enum FVIgnoreType
    {
        None = 0,

        /// <summary> 플랫폼 무시 </summary>
        IgnorePlatform = 1 << 0,  // 1

        /// <summary> 공중 캐릭터 무시 </summary>
        IgnoreCharacterInAir = 1 << 1,  // 2

        /// <summary> 플랫폼 + 공중 캐릭터 무시 </summary>
        IgnoreBoth = IgnorePlatform | IgnoreCharacterInAir,  // 3
    }
}
