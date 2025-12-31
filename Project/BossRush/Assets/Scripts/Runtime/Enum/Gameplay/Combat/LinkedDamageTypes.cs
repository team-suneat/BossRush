namespace TeamSuneat
{
    public enum LinkedDamageTypes
    {
        None,

        /// <summary>
        /// 공격자의 무기 공격력
        /// </summary>
        Attack,

        // 자원

        /// <summary>
        /// 공격자의 현재 생명력
        /// </summary>
        CurrentHealthOfAttacker,

        /// <summary>
        /// 공격자의 최대 생명력
        /// </summary>
        MaxHealthOfAttacker,

        /// <summary>
        /// 공격자의 현재 보호막
        /// </summary>
        CurrentShieldOfAttacker,

        /// <summary>
        /// 공격자의 최대 보호막
        /// </summary>
        MaxShieldOfAttacker,

        /// <summary>
        /// 피격자의 현재 생명력
        /// </summary>
        CurrentHealthOfTarget,

        /// <summary>
        /// 피격자의 최대 생명력
        /// </summary>
        MaxHealthOfTarget,

        // 능력치

        /// <summary>
        /// 공격자의 기술 재사용 대기시간
        /// </summary>
        SkillCooldownTime,
    }
}