namespace TeamSuneat
{
    public enum AdditionalDamageTypes
    {
        None,

        /// <summary>
        /// 공격자의 능력치
        /// </summary>
        StatOfAttacker,

        /// <summary>
        /// 공격자의 현재 생명력
        /// </summary>
        CurrentLifeOfAttacker,

        /// <summary>
        /// 공격자의 현재 생명력
        /// </summary>
        MaxLifeOfAttacker,

        /// <summary>
        /// 공격자의 현재 자원
        /// </summary>
        CurrentResourceOfAttacker,

        /// <summary>
        /// 공격자의 현재 자원
        /// </summary>
        MaxResourceOfAttacker,

        /// <summary>
        /// 피격자의 상태이상
        /// </summary>
        StateEffectOfTarget,
    }
}