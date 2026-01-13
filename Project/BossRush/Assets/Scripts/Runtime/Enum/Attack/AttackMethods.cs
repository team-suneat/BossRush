namespace TeamSuneat
{
    public enum AttackMethods
    {
        None = 0,

        All,

        Suicide,

        Execution,

        Reflect, /// 피해 반사

        Operate, /// 맵 오브젝트 작동

        SpawnCharacter, /// 캐릭터 생성

        //

        NormalAttack = 10,

        ChargeAttack,

        SkillAttack,

        SubWeaponAttack,

        Dash,

        DashAttack,

        ParryingAttack,

        DiveAttack,

        //

        Heal, /// 체력 회복

        HealByMaxLife,

        ReCharge, /// 보호막 재충전

        //

        BurnMana,   /// 마나 연소

        RecoveryMana, /// 마나 회복

        //

        TrueDamage, /// 어떤 스탯에도 영향을 받지 않음

        BreakBarrier,
    }

    public enum AttackEntityTypes
    {
        None,

        Target,

        Area,

        Operate,

        Spawn,

        Max,
    }

    public enum AttackHitTypes
    {
        None,

        Single, // 활성화시 최초 1회

        Continuous, // 활성화시 다단히트

        Collision, // 충돌시 최초 1회
    }

    public enum ChangeAttackOrderType
    {
        None = -1,

        First,

        Second,

        Third,

        Fourth,

        Fifth,

        Sixth,

        Seventh,

        Eighth,

        Ninth,

        Tenth,

        NotLast = 100,

        Last,
    }

    public enum CharacterAttackTypes
    {
        None = 0,

        AirAttack,

        Attack,

        Charge,

        ChargeAttack,

        ChargeSwapAttack,

        ChargeMinAttack,

        ChargeMaxAttack,

        Skill,

        ChargeSkill,

        SubWeapon,

        Dash,

        DashAttack,

        ForceJumpAttack,

        DiveAttackStart,

        Parrying,

        HyperCore,

        BossAttack,

        Dying,
    }
}