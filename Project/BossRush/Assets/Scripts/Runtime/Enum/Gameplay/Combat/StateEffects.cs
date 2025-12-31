namespace TeamSuneat
{
    public enum StateEffects
    {
        None,

        // 공통

        Slow, // 둔화
        Vulnerable, // 취약
        EnhancedVulnerable, // 강화된 취약
        Weak, // 약화
        Dazed, // 멍함
        Paralysis, // 마비
        Stun, // 기절
        Snare, // 속박

        // 속성

        Burning, // 연소
        Chilled, // 오한
        Freeze, // 빙결
        Jolted, // 충격
        ElectricShock, // 감전
        Bleeding, // 출혈
        Poisoning, // 중독

        // 직업

        Rampage, // 광폭화 : 광전사 전용

        // 표기 상태

        DamageOverTime, // 지속 피해
        HealingEffect, // 치유 효과
        Shield, // 보호막
        Incapacitated, // 행동 불가
        Elite, // 정예 적
        Ferocity, // 흉포
        Carapace, // 껍질
        Overwhelm, // 압도
    }

    public static class StateEffectChecker
    {
        public static bool IsDamage(this StateEffects stateEffect)
        {
            switch (stateEffect)
            {
                case StateEffects.Burning:
                case StateEffects.Jolted:
                case StateEffects.Poisoning:
                case StateEffects.Bleeding:
                    return true;
            }

            return false;
        }

        public static bool IsCrowdControl(this StateEffects stateEffect)
        {
            switch (stateEffect)
            {
                case StateEffects.Freeze:
                case StateEffects.ElectricShock:
                case StateEffects.Paralysis:
                case StateEffects.Stun:                
                case StateEffects.Snare:
                    return true;
            }

            return false;
        }
    }
}
