namespace TeamSuneat
{
    public enum StatModType
    {
        None,

        /// <summary> 값을 더합니다 </summary>
        Flat,

        /// <summary> %를 더합니다. </summary>
        PercentAdd,

        /// <summary> %를 곱합니다. </summary>
        PercentMulti,

        /// <summary> 0 또는 1로 사용 여부를 설정합니다. </summary>
        Use,

        /// <summary> 추가되지 않고 적용합니다. </summary>
        Once,

        DiminishingReturns, // 점감
    }
}