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
        /// 패링 가능 (패링시 스턴)
        /// </summary>
        ParryableWithStun,

        /// <summary>
        /// 패링 불가
        /// </summary>
        Unparryable,
    }
}
