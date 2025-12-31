namespace TeamSuneat
{
    public enum EffectSpawnTypes
    {
        /// <summary>
        /// 순차적인
        /// </summary>
        Sequentially,

        /// <summary>
        /// 무작위의
        /// </summary>
        Randomly,

        /// <summary>
        /// 지정된
        /// </summary>
        Designated,
    }

    public enum EffectFacingTypes
    {
        None, Random, Owner, ReverseOwner
    }
}