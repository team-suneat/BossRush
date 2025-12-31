using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        // 최소 피해 증폭 배율 (피해가 0이 되는 것을 방지)
        private const float MIN_DAMAGE_AMPLIFICATION = 0.01f;

        // 피해 증폭 계산: 치명타 + 회심의 일격
        private float CalculateDamageAmplification(HitmarkAssetData damageAssetData, DamageResult damageResult)
        {
            float damageAmplification = 1f;

            // 치명타 피해 배율: 기본 데미지 × (1 + 치명타 피해%)
            float criticalDamageMultiplier = GetCriticalDamageMultiplier(damageResult);
            if (!criticalDamageMultiplier.IsEqual(1))
            {
                damageAmplification *= criticalDamageMultiplier;
            }

            // 회심의 일격 피해 배율: 기본 데미지 × (1 + 회심의 일격%)
            float devastatingStrikeMultiplier = GetDevastatingStrikeMultiplier(damageResult);
            if (!devastatingStrikeMultiplier.IsEqual(1))
            {
                damageAmplification *= devastatingStrikeMultiplier;
            }

            return Mathf.Max(MIN_DAMAGE_AMPLIFICATION, damageAmplification);
        }

        // 치명타 피해 배율 계산: 기본 데미지 × (1 + 치명타 피해%)
        private float GetCriticalDamageMultiplier(DamageResult damageResult)
        {
            if (damageResult.DamageType.IsInstantDamage())
            {
                damageResult.IsCritical = DetermineCritical(HitmarkAssetData);
                if (damageResult.IsCritical)
                {
                    return CalculateCriticalDamageMultiplier(damageResult);
                }
            }

            return 1f;
        }

        // 회심의 일격 피해 배율 계산: 기본 데미지 × (1 + 회심의 일격%)
        private float GetDevastatingStrikeMultiplier(DamageResult damageResult)
        {
            if (damageResult.DamageType.IsInstantDamage())
            {
                damageResult.IsDevastatingStrike = DetermineDevastatingStrike(damageResult);
                if (damageResult.IsDevastatingStrike)
                {
                    return CalculateDevastatingStrikeMultiplier(damageResult);
                }
            }

            return 1f;
        }

        // 회심의 일격 피해량 배율 계산
        private float CalculateDevastatingStrikeMultiplier(DamageResult damageResult)
        {
            float devastatingStrikeMultiplier = 0f;
            _ = TryAddAttackerStatValue(StatNames.DevastatingStrike, ref devastatingStrikeMultiplier, LogDevastatingStrikeRate);

            // 기본 데미지 × (1 + 회심의 일격%)
            return 1f + devastatingStrikeMultiplier;
        }
    }
}