namespace TeamSuneat
{
    public enum ParryTypes
    {
        None,

        /// <summary>
        /// 패링 가능
        /// </summary>
        Parryable,

        /// <summary>
        /// 반격 패링 가능 (스킬 패링 성공 시 공격자 기절)
        /// </summary>
        CounterParryable,

        /// <summary>
        /// 패링 불가
        /// </summary>
        Unparryable,
    }
}
