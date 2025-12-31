namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        // 피해량 배율 (Damage Multiplier)
        private float CalculateDamageMultiplier(DamageResult damageResult, int hitmarkLevel, int stack)
        {
#if UNITY_EDITOR
            int index = _stringBuilder.Length;
#endif

            float damageMultiplier = 0f;
            float damageMultiplierFromDamageType = CalculateDamageMultiplierByDamageType(damageResult.Asset.DamageType);

            damageMultiplier += damageMultiplierFromDamageType;
            if (stack > 1)
            {
                damageMultiplier += stack - 1;
                AddStackMultiplier(stack - 1);
            }

#if UNITY_EDITOR
            string content = GenerateContent(index, damageMultiplierFromDamageType, "피해량 배율을 계산합니다. [능력치 피해량 배율: ", "Calculates the Damage Multiplier. [Stat Damage Multiplier: ");
            _stringBuilder.Insert(index, content);
            _stringBuilder.AppendFormat("] => {0}", ValueStringEx.GetPercentString(damageMultiplier, 1));
            _stringBuilder.AppendLine();
#endif

            return damageMultiplier;
        }

        // 가시 피해량 배율 (Thorns Damage Multiplier)
        private float CalculateThornsDamageMultiplier(DamageResult damageResult, int hitmarkLevel, int stack)
        {
#if UNITY_EDITOR
            int index = _stringBuilder.Length;
#endif

            float damageMultiplier = 0f;
            float damageMultiplierFromDamageType = CalculateDamageMultiplierByDamageType(damageResult.Asset.DamageType);

            damageMultiplier += damageMultiplierFromDamageType;

            if (stack > 1)
            {
                damageMultiplier += stack - 1;
                AddStackMultiplier(stack - 1);
            }

#if UNITY_EDITOR
            string content = GenerateContent(index, damageMultiplierFromDamageType, "가시 피해량 배율을 계산합니다. [능력치 가시 피해량 배율: ", "Calculates the Thorns Damage Multiplier. [Stat Thorns Damage Multiplier: ");
            _stringBuilder.Insert(index, content);
            _stringBuilder.AppendFormat("] => {0}", ValueStringEx.GetPercentString(damageMultiplier, 1));
            _stringBuilder.AppendLine();
#endif

            return damageMultiplier;
        }

        // 피해 종류에 따른 피해량 배율을 계산합니다.
        private float CalculateDamageMultiplierByDamageType(DamageTypes damageType)
        {
            float damageRate = 1f;
            float elementDamageRate = 0;
            float magicDamageRateByStates = 0;
            float damageOverTimeRate = 0;

            // TODO: 특정 조건 피해량 적용

            float result = damageRate + damageOverTimeRate + elementDamageRate + magicDamageRateByStates;
            LogProgressDamageTypeMultiplier(damageRate, elementDamageRate, magicDamageRateByStates, damageOverTimeRate, result);

            return result;
        }

        // 치명타 피해량 배율을 계산합니다.
        // 공식: 기본 데미지 × (1 + 치명타 피해%)
        private float CalculateCriticalDamageMultiplier(DamageResult damageResult)
        {
            float criticalDamageMultiplier = 0f;
            _ = TryAddAttackerStatValue(StatNames.CriticalDamage, ref criticalDamageMultiplier, LogCriticalDamageRate);

            // 기본 데미지 × (1 + 치명타 피해%)
            return 1f + criticalDamageMultiplier;
        }
    }
}