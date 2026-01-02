namespace TeamSuneat
{
    public enum SkillUseTypes
    {
        None,

        /// <summary>
        /// 액티브 : 기술 사용 명령을 입력한 시점에 기술이 사용됩니다.
        /// </summary>
        Active,

        /// <summary>
        /// 패시브 : 상시 적용 또는 조건부로 기술의 효과가 적용됩니다.
        /// </summary>
        Passive,

        /// <summary>
        /// 강화형 패시브 : 액티브를 습득했을 때 습득할 수 있는 패시브입니다.
        /// </summary>
        BoostPassive,

        /// <summary>
        /// 선택형 패시브 : 강화형 패시브를 습득했을 때 둘 중 하나만 습득할 수 있는 패시브입니다.
        /// </summary>
        SelectPassive,
    }
}