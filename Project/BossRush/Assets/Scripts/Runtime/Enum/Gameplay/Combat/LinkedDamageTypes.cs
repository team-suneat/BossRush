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
        CurrentLifeOfAttacker,

        /// <summary>
        /// 공격자의 최대 생명력
        /// </summary>
        MaxLifeOfAttacker,

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
        CurrentLifeOfTarget,

        /// <summary>
        /// 피격자의 최대 생명력
        /// </summary>
        MaxLifeOfTarget,
    }
}