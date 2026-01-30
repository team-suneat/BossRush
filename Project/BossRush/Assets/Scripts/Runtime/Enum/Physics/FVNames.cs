namespace TeamSuneat
{
    public enum FVNames
    {
        None,

        /// <summary> 플레이어 대시 </summary>
        PlayerDash,

        /// <summary> 플레이어 넉백 </summary>
        PlayerKnockback,

        // 첫 번째 보스
        Boss1Jump,


        /// <summary> 첫 번째 보스의 첫 번째 패턴 1 - 3연속 공격 1 </summary>
        Boss1Pattern1Attack1,
        /// <summary> 첫 번째 보스의 첫 번째 패턴 1 - 3연속 공격 2 </summary>
        Boss1Pattern1Attack2,
        /// <summary> 첫 번째 보스의 첫 번째 패턴 1 - 3연속 공격 3 </summary>
        Boss1Pattern1Attack3,

        /// <summary> 첫 번째 보스의 두 번째 패턴 2 - 점프 </summary>
        Boss1Pattern2Jump,
        /// <summary> 첫 번째 보스의 첫 번째 패턴 2 - 공격 </summary>
        Boss1Pattern2Attack,

        /// <summary> 첫 번째 보스의 세 번째 패턴 - 기술 패링 공격 </summary>
        Boss1Pattern3Attack,

        /// <summary> 첫 번째 보스의 네 번째 패턴 - 패링 불가 공격 (점프 + 공격)</summary>
        Boss1Pattern4Attack,

        /// <summary> 첫 번째 보스의 첫 번째 패턴 1 - 3연속 공격 1 넉백 </summary>
        Boss1Pattern1Knockback,
        /// <summary> 첫 번째 보스의 두 번째 패턴 2 - 점프 공격 넉백 </summary>
        Boss1Pattern2Knockback,
        /// <summary> 첫 번째 보스의 세 번째 패턴 - 기술 패링 공격 넉백 </summary>
        Boss1Pattern3Knockback,
        /// <summary> 첫 번째 보스의 네 번째 패턴 - 패링 불가 공격 (점프 + 공격) 넉백 </summary>
        Boss1Pattern4Knockback,

        //

        ContactAttackKnockback,
    }

    public enum FVSubjects
    {
        None,

        /// <summary> 소유자에게 적용 </summary>
        Owner,

        /// <summary> 타겟에게 적용 </summary>
        Target,
    }

    public enum FVDirections
    {
        None,

        /// <summary> 바라보는 방향 </summary>
        Face,

        /// <summary> 공격자 방향으로 </summary>
        ToAttacker,

        /// <summary> 타겟 방향으로 </summary>
        ToTarget,

        /// <summary> 타겟 위치로 </summary>
        ToTargetPosition,

        /// <summary> 지정된 방향으로 </summary>
        ToDirection,

        /// <summary> 방향 입력에 따라 </summary>
        DirectionalInput,

        /// <summary> X 방향으로 </summary>
        DirectionalX,

        /// <summary> 현재 속도 방향으로 </summary>
        Velocity,

        /// <summary> 반대 방향으로 </summary>
        Opposite,
    }
}