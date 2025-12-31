using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        // 공통 헬퍼 (DamageCalculator)
        private bool TryApplyReferenceDamage(HitmarkAssetData damageAssetData, ref DamageResult damageResult)
        {
            if (damageAssetData.LinkedDamageType == LinkedDamageTypes.Attack && ReferenceValue > 0f)
            {
                damageResult.DamageValue = ReferenceValue;
                damageResult.ReducedDamageValue = 0f;
                return true;
            }
            return false;
        }

        private int GetHitmarkLevel()
        {
            return Mathf.Max(Level, 1); // 히트마크 최소 레벨 설정
        }

        private float GetFixedDamageValue(HitmarkAssetData damageAsset, int hitmarkLevel)
        {
            if (damageAsset.UseWeaponDamage)
            {
                if (WeaponDamageOverride.HasValue)
                {
                    return WeaponDamageOverride.Value;
                }

                WarnWeaponDamageNotProvided(damageAsset);
            }

            return StatEx.GetValueByLevel(damageAsset.FixedDamage, damageAsset.FixedDamageByLevel, hitmarkLevel);
        }

        //

        private void ComputeByType(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryCheatDamage(damageAsset, ref damageResult))
            {
                return;
            }

            switch (damageAsset.DamageType)
            {
                case DamageTypes.Heal:
                case DamageTypes.HealOverTime: { ComputeHealValue(damageAsset, ReferenceValue, ref damageResult); } break;
                case DamageTypes.RestoreMana:
                case DamageTypes.RestoreManaOverTime: { ComputeRestoreManaValue(damageAsset, ReferenceValue, ref damageResult); } break;
                case DamageTypes.ChargeShield: { ComputeChargeValue(damageAsset, ReferenceValue, ref damageResult); } break;
                case DamageTypes.Normal: { HandleComputeDamage(damageAsset, ref damageResult); } break;
                case DamageTypes.Thorns: { HandleComputeThorns(damageAsset, ref damageResult); } break;
                case DamageTypes.DamageOverTime: { HandleComputeDamageOverTime(damageAsset, ref damageResult); } break;
                case DamageTypes.Execution: { damageResult.DamageValue = float.MaxValue; } break;
                default:
                    Log.Warning("데미지 에셋에서 피해 종류(DamageType) 설정이 올바르지 않을 가능성이 있습니다: {0}, {1}", damageAsset.Name.ToLogString(), damageAsset.DamageType);
                    break;
            }
        }

        private void HandleComputeDamage(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            if (DetermineEvasion(damageResult)) { damageResult.IsEvasion = true; return; }
            if (DetermineExecute(damageResult)) { damageResult.DamageValue = float.MaxValue; return; }
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputeDamage(damageAsset, GetHitmarkLevel(), ref damageResult);
        }

        private void HandleComputeDamageOverTime(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputeDamageOverTime(damageAsset, GetHitmarkLevel(), Stack, ref damageResult);
        }

        private void HandleComputeThorns(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputeThornsDamage(damageAsset, GetHitmarkLevel(), ref damageResult);
        }

        // Compute

        // 피해 계산
        // 문서 공식 (5.4 캐릭터 실제 피해량 계산식):
        // 1단계: 총 공격력 = 기본 공격력 + 장비 공격력 + 강화 공격력
        // 2단계: 치명타 피해 = 기본 데미지 × (1 + 치명타 피해%)
        // 3단계: 총 데미지 = (평타/스킬 계수) × 총 공격력 × ... + 회심의 일격 보너스
        private void ComputeDamage(HitmarkAssetData damageAsset, int hitmarkLevel, ref DamageResult damageResult)
        {
            ClearLogBuilder();
            AppendLineToLog();

            // 1단계: 총 공격력 계산 (기본 공격력 + 장비 공격력 + 강화 공격력은 이미 능력치 시스템에서 합산됨)
            float totalAttackPower = FindAttackerStatValue(StatNames.Attack);
            float fixedDamage = GetFixedDamageValue(damageAsset, hitmarkLevel);
            float baseDamage = totalAttackPower + fixedDamage;

            LogDamageCalculation("피해량", baseDamage, totalAttackPower, fixedDamage);

            // 피해량 배율 계산 (평타/스킬 계수)
            float damageMultiplier = CalculateDamageMultiplier(damageResult, hitmarkLevel, 0);

            // 피해 감소 계산
            float damageReduction = CalculateDamageReduction(damageResult);

            // 피해 증폭 계산 (치명타 + 회심의 일격)
            // 2단계: 치명타 피해 = 기본 데미지 × (1 + 치명타 피해%)
            // 회심의 일격 피해 = 기본 데미지 × (1 + 회심의 일격%)
            float damageAmplification = CalculateDamageAmplification(damageAsset, damageResult);

            /*
             * 계산 공식:
             * 총 피해량 = 기본 피해량 × 스킬 계수
             * 최종 피해량 = 총 피해량 × 피해 감소 × 피해 증폭(치명타/회심의 일격)
             * 감소된 피해량 = 총 피해량 × (1 - 피해 감소 배율)
             */

            float totalDamage = baseDamage * damageMultiplier;
            float finalDamageValue = totalDamage * damageReduction * damageAmplification;
            float reducedDamageValue = totalDamage * (1f - damageReduction);

            LogDamageCalculationStart(
                Attacker,
                TargetVital,
                baseDamage,
                damageMultiplier,
                damageReduction,
                damageAmplification,
                finalDamageValue);

            LogInfo(GetLogString());

            finalDamageValue = Mathf.Max(damageAsset.MinDamageValue, finalDamageValue);

            damageResult.DamageValue = finalDamageValue;
            damageResult.ReducedDamageValue = reducedDamageValue;
        }

        private void ComputeDamageOverTime(HitmarkAssetData damageAsset, int hitmarkLevel, int stack, ref DamageResult damageResult)
        {
            ClearLogBuilder();
            AppendLineToLog();

            // 피해량 계산
            float totalAttackPower = FindAttackerStatValue(StatNames.Attack);
            float fixedDamage = GetFixedDamageValue(damageAsset, hitmarkLevel);
            float baseDamage = totalAttackPower + fixedDamage;

            LogDamageCalculation("지속 피해량", baseDamage, totalAttackPower, fixedDamage);

            // 피해량 배율 계산
            float damageMultiplier = CalculateDamageMultiplier(damageResult, hitmarkLevel, stack);

            // 피해 감소 계산
            float damageReduction = CalculateDamageReduction(damageResult);
            LogDamageReduction("지속", damageReduction);

            // 최종 피해 계산
            float damageAmplification = CalculateDamageAmplification(damageAsset, damageResult);

            /*
             * 최종 지속 피해량 = (기본 피해량 × 피해 배율) × 피해 감소 × 피해 증폭
             * 감소된 지속 피해량 = (기본 피해량 × 피해 배율) × (1 - 피해 감소)
             */

            float totalDamage = baseDamage * damageMultiplier;
            float finalDamageValue = totalDamage * damageReduction * damageAmplification;
            float reducedDamageValue = totalDamage * (1f - damageReduction);

            LogDamageCalculationStart(
                Attacker,
                TargetVital,
                baseDamage,
                damageMultiplier,
                damageReduction,
                damageAmplification,
                finalDamageValue);

            LogInfo(GetLogString());

            finalDamageValue = Mathf.Max(damageAsset.MinDamageValue, finalDamageValue);

            damageResult.DamageValue = finalDamageValue;
            damageResult.ReducedDamageValue = reducedDamageValue;
        }

        private void ComputeThornsDamage(HitmarkAssetData damageAsset, int hitmarkLevel, ref DamageResult damageResult)
        {
            ClearLogBuilder();
            AppendLineToLog();

            // 피해량 계산
            float totalAttackPower = FindAttackerStatValue(StatNames.Attack);
            float fixedDamage = GetFixedDamageValue(damageAsset, hitmarkLevel);
            float thornsDamage = totalAttackPower + fixedDamage;

            LogDamageCalculation("가시 피해량", thornsDamage, totalAttackPower, fixedDamage);

            // 피해량 배율 계산
            float thornsDamageMultiplier = CalculateThornsDamageMultiplier(damageResult, hitmarkLevel, 0);

            // 피해 감소 계산
            float damageReduction = CalculateDamageReduction(damageResult);
            LogDamageReduction("가시", damageReduction);

            // 피해 증폭 계산
            float damageAmplification = CalculateDamageAmplification(damageAsset, damageResult);

            /*
             * 최종 가시 피해량 = (기본 피해 × 배율) × 피해 감소 × 증폭
             * 감소된 가시 피해량 = 총 피해량 × (1 - 피해 감소 배율)
             */

            float totalThornsDamage = (thornsDamage * thornsDamageMultiplier);
            float finalDamageValue = totalThornsDamage * damageReduction * damageAmplification;
            float reducedDamageValue = totalThornsDamage * (1f - damageReduction);

            LogDamageCalculationStart(
                Attacker,
                TargetVital,
                thornsDamage,
                thornsDamageMultiplier,
                damageReduction,
                damageAmplification,
                finalDamageValue);

            LogInfo(GetLogString());

            finalDamageValue = Mathf.Max(damageAsset.MinDamageValue, finalDamageValue);

            damageResult.DamageValue = finalDamageValue;
            damageResult.ReducedDamageValue = reducedDamageValue;
        }

        private void ComputeHealValue(HitmarkAssetData damageAsset, float referenceValue, ref DamageResult damageResult)
        {
            int hitmarkLevel = GetHitmarkLevel();
            float fixedValue = GetFixedDamageValue(damageAsset, hitmarkLevel);

            float magnification;

            magnification = damageAsset.LinkedHitmarkMagnification;

            if (Level > 1)
            {
                magnification += (Level - 1) * damageAsset.LinkedValueMagnificationByLevel;
            }

            if (Stack > 1)
            {
                magnification += (Stack - 1) * damageAsset.LinkedValueMagnificationByStack;
            }

            float result = fixedValue + (referenceValue * magnification);
            result = Mathf.Max(result, damageAsset.MinDamageValue);

            damageResult.DamageValue = result;
            damageResult.ReducedDamageValue = 0;

            if (result.IsZero())
            {
                return;
            }

            LogHealingOrResourceRestoration(damageAsset, fixedValue, referenceValue, magnification, result);
        }

        private void ComputeRestoreManaValue(HitmarkAssetData damageAsset, float referenceValue, ref DamageResult damageResult)
        {
            int hitmarkLevel = GetHitmarkLevel();
            float fixedValue = GetFixedDamageValue(damageAsset, hitmarkLevel);

            float magnification = damageAsset.LinkedHitmarkMagnification;

            if (Level > 1)
            {
                magnification += (Level - 1) * damageAsset.LinkedValueMagnificationByLevel;
            }

            if (Stack > 1)
            {
                magnification += (Stack - 1) * damageAsset.LinkedValueMagnificationByStack;
            }

            float result = fixedValue + (referenceValue * magnification);
            result = Mathf.Max(result, damageAsset.MinDamageValue);

            damageResult.DamageValue = result;
            damageResult.ReducedDamageValue = 0;

            if (result.IsZero())
            {
                return;
            }

            LogHealingOrResourceRestoration(damageAsset, fixedValue, referenceValue, magnification, result);
        }

        private void ComputeChargeValue(HitmarkAssetData damageAsset, float referenceValue, ref DamageResult damageResult)
        {
            int hitmarkLevel = GetHitmarkLevel();
            float fixedValue = GetFixedDamageValue(damageAsset, hitmarkLevel);

            float magnification = damageAsset.LinkedHitmarkMagnification;

            if (Level > 1)
            {
                magnification += (Level - 1) * damageAsset.LinkedValueMagnificationByLevel;
            }

            if (Stack > 1)
            {
                magnification += (Stack - 1) * damageAsset.LinkedValueMagnificationByStack;
            }

            float result = fixedValue + (referenceValue * magnification);
            result = Mathf.Max(result, damageAsset.MinDamageValue);

            damageResult.DamageValue = result;
            damageResult.ReducedDamageValue = 0;

            if (result.IsZero())
            {
                return;
            }

            LogHealingOrResourceRestoration(damageAsset, fixedValue, referenceValue, magnification, result);
        }
    }
}