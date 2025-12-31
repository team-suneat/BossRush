namespace TeamSuneat
{
    public enum TriggerTypes
    {
        None = 0,
        OnAttack,
        OnHit,
        OnKill,
        OnDamage,
        OnHeal,
        OnSkillUse,
        OnBuffApply,
        OnBuffRemove,
        OnLevelUp,
        OnDeath,
        OnCritical,
        OnEvasion,
        OnBlock,
        // 기타
        Custom,
        HitmarkAttack,
    }
}
