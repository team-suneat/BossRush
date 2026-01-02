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
        RemainingHealthAndDistance,
    }

    public enum TargetSelections
    {
        None,

        SingleTarget, /// 모든 발사체가 하나의 타겟을 설정한다.

        MultipleTarget, /// 발사체마다 각각 다른 타겟을 설정한다.

                        /// 발사체의 수보다 타겟의 수가 적어 할당받지 못한 발사체는 생성되지 않습니다.

        PriorityMultipleTarget, /// 발사체는 우선순위에 따라 타겟을 설정합니다.

                                /// 발사체의 수보다 타겟의 수가 적어 할당받지 못한 발사체는 다시 우선순위에 맞추어 설정됩니다.
    }
}