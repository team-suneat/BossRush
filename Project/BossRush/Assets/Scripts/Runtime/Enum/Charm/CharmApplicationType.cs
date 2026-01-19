using System;

namespace TeamSuneat
{
    /// <summary>
    /// 부적의 적용 방식을 나타내는 enum
    /// Flags를 사용하여 여러 방식을 조합할 수 있습니다.
    /// </summary>
    [Flags]
    public enum CharmApplicationType
    {
        None = 0,

        /// <summary>
        /// 버프 방식: 능력치만 영구로 얻는 방식 (StatBuff 등 설정)
        /// </summary>
        Buff = 1 << 0,  // 1

        /// <summary>
        /// 스킬 방식: 입력된 시전 키에 따라 적용되는 기술을 바꾸는 방식
        /// </summary>
        Skill = 1 << 1,  // 2

        /// <summary>
        /// 패시브 방식: 조건부로 특정 시점에 작동되는 방식 (패링 시 공격, 패링 시 버프 등)
        /// </summary>
        Passive = 1 << 2,  // 4

        // 조합 타입들
        /// <summary>
        /// 버프 + 패시브 조합
        /// </summary>
        BuffAndPassive = Buff | Passive,  // 5
    }
}
