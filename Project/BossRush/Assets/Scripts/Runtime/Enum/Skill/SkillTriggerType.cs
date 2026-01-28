namespace TeamSuneat
{
    public enum SkillTriggerType
    {
        None,

        // 입력키를 눌러 시전되는 스킬
        InputCast,

        // 획득 즉시 효과가 적용되는 스킬 (버프형)
        OnAcquire,

        // 조건을 만족할 때 발동하는 스킬
        Conditional,
    }
}
