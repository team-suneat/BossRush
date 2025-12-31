namespace TeamSuneat
{
    public enum ItemOptionTypes
    {
        None = 0,
        // 기본 아이템 옵션 타입들
        Basic,
        Advanced,
        Expert,
        // 기타
        Custom,

        /// <summary> 기본 옵션 </summary>
        Base,

        /// <summary> 타입별 추가 옵션 </summary>
        ByType,

        /// <summary> 보조 옵션 </summary>
        Sub,

        /// <summary> 유니크 옵션 </summary>
        Unique,

        /// <summary> 강화 및 각성 특수 옵션 </summary>
        Enhance,

        /// <summary> 균열 보석 옵션 </summary>
        LegendaryRiftGem
    }
}
