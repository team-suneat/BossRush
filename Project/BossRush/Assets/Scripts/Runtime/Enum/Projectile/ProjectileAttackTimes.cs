namespace TeamSuneat
{
    public enum ProjectileAttackTimes
    {
        None,

        /// <summary>
        /// 생성시
        /// </summary>
        OnSpawn,

        /// <summary>
        /// 충돌시
        /// </summary>
        OnCollision,

        /// <summary>
        /// 지속시간이 끝났을 시
        /// </summary>
        OnTimer,

        /// <summary>
        /// 특정 애니메이션 이벤트에서
        /// </summary>
        OnAnimationEvent,
    }
}