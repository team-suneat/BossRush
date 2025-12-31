namespace TeamSuneat
{
    public enum StatNames
    {
        None, // 없음

        // 기본 능력치
        Attack,                  // 공격력
        Health,                  // 최대 체력
        HealthRegen,             // 체력 회복량
        AttackSpeed,             // 공격 속도(%)
        CriticalChance,          // 치명타 확률(%)
        CriticalDamage,          // 치명타 피해(%)
        Mana,                    // 마나
        ManaRegen,               // 마나 회복량
        GoldGain,                // 추가 골드 획득량(%)
        XPGain,                  // 추가 경험치(%)
        DamageReduction,         // 피해 감소(%)


        // 강화 시스템 능력치
        DevastatingStrike,       // 회심의 일격(%)
        DevastatingStrikeChance, // 회심의 일격 확률(%)

        // 성장 시스템 능력치
        Accuracy,                // 명중
        Dodge,                   // 회피

        // 보호막 능력치
        Shield,                  // 보호막
        ShieldMulti,             // 보호막 배율
    }
}