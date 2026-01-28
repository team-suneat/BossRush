namespace TeamSuneat
{
    public enum CharmName
    {
        None = 0,

        DefaultCharm = 1,

        #region Attack (공격) - 100~199
        TechEcho = 100,              // 기술 피해 +20%, 성공 시 5초 1초마다 1피해
        WideArcBlade = 101,          // 공격 범위 50% 증가
        RiskReward = 102,            // 받는 피해 +1, 주는 피해 +1
        #endregion

        #region Skill (기술) - 200~299
        LinearPulseShot = 200,       // 바라보는 방향 일직선 발사체
        OverheadNova = 201,          // 머리 위 넓은 범위 공격
        SlamDownStrike = 202,        // 내려찍기 기술
        #endregion

        #region Support (보조) - 300~399
        ManaFeedback = 300,          // 피해 입을 때 마나/스태미너 25% 획득
        ManaOvercharge = 301,        // 마나 +2
        StaminaBoost = 302,          // 스태미너 +2
        FullHealthParry = 303,       // 체력 full 시 패링 소모 50% ↓
        VelocitySurge = 304,         // 이동속도 +50%
        #endregion

        #region Counter (반격) - 400~499
        AntiParryShield = 400,       // 패링 불가 공격 피해 -1
        ParryStrike = 401,           // 패링 성공 시 피해
        DamagePulse = 402,           // 피해 입으면 주변 피해
        ReflectDrone = 403,          // 발사체 패링 시 되돌려 1피해
        NoKnockParry = 404,          // 패링 시 넉백 없음
        LastStandFury = 405,         // 체력 1 시 공격력 75% ↑
        #endregion
    }
}
