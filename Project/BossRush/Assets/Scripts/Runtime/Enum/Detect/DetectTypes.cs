namespace TeamSuneat
{
    public enum DetectAreaTypes
    {
        None,
        Arc,
        Circle,
        Box,
    }

    public enum DetectPriorityTypes
    {
        None,

        /// <summary>
        /// 거리
        /// </summary>
        Distance,

        /// <summary>
        /// 캐릭터 종류 (Boss > Elite > Monster)
        /// </summary>
        CharacterType,

        /// <summary>
        /// 남은 체력과 보호막
        /// </summary>
        RemainingVital,

        /// <summary>
        /// 남은 체력과 보호막 + 거리
        /// </summary>
        RemainingLifeAndDistance,
    }

    public enum TargetSelections
    {
        None,
    }
}