using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public partial class DamageCalculator
    {
        private Vital _targetVital;

        public Vital TargetVital => _targetVital;
        public Character TargetCharacter => _targetVital?.Owner;
        public HitmarkAssetData HitmarkAssetData { get; set; }
        public Character Attacker { get; private set; }
        public AttackEntity AttackEntity { get; set; }
        public List<DamageResult> DamageResults { get; private set; } = new();
        public float ReferenceValue { get; set; }
        public float DamageReferenceValue { get; private set; }
        public float ManaCostReferenceValue { get; private set; }
        public float CooldownReferenceValue { get; private set; }
        public int Level { get; private set; } = 1;
        public int Stack { get; set; }

        //─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        #region Public Methods

        public void Execute()
        {
            if (!IsComputeValid())
            {
                return;
            }

            ResetDamageCalculators();

            DamageResult damageResult = CreateDamageResult(HitmarkAssetData);

            RefreshReferenceValue(HitmarkAssetData);
            ComputeByType(HitmarkAssetData, ref damageResult);

            DamageResults.Add(damageResult);
        }

        public void SetAttacker(Character attacker)
        {
            Attacker = attacker;
            if (attacker != null)
            {
                LogProgressAttacker(attacker.GetHierarchyName());
            }
        }

        public void SetTargetVital(Vital targetVital)
        {
            if (targetVital != null)
            {
                _targetVital = targetVital;
                LogProgressTargetVital(_targetVital.GetHierarchyPath());
            }
            else
            {
                _targetVital = null;
                LogWarningTargetVital();
            }
        }

        public void SetResourceCostReferenceValue(int manaCost)
        {
            ManaCostReferenceValue = manaCost;
            LogManaCostReferenceValue(ManaCostReferenceValue.ToSelectString(0));
        }

        public void SetDamageReferenceValue(int damageValue)
        {
            DamageReferenceValue = damageValue;
            LogDamageReferenceValue(DamageReferenceValue.ToSelectString(0));
        }

        public void SetCooldownReferenceValue(float cooldownTime)
        {
            CooldownReferenceValue = cooldownTime;
            LogCooldownReferenceValue(CooldownReferenceValue.ToSelectString(0));
        }

        public void SetStack(int stack)
        {
            Stack = stack;
        }

        public void SetLevel(int level)
        {
            Level = level;
        }

        #endregion Public Methods

        #region Compute

        private bool IsComputeValid()
        {
            if (!HitmarkAssetData.IsValid())
            {
                LogErrorHitmarkNotSet();
                return false;
            }

            return true;
        }

        private DamageResult CreateDamageResult(HitmarkAssetData damageAsset)
        {
            return new DamageResult
            {
                Asset = damageAsset,
                HitmarkLevel = Level,
                Attacker = Attacker,
                AttackEntity = AttackEntity,
                TargetVital = TargetVital
            };
        }

        private void ResetDamageCalculators()
        {
            DamageResults.Clear();
            LogProgressResetDamageCalculators();
        }

        private bool TryCheatDamage(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                return false;
            }

            if (GameSetting.Instance == null || GameSetting.Instance.Cheat == null)
            {
                return false;
            }

            // 플레이어가 공격자일 때만 치트 적용
            if (Attacker == null || !Attacker.IsPlayer)
            {
                return false;
            }

            // 무한 피해 치트
            if (GameSetting.Instance.Cheat.IsInfinityDamage)
            {
                damageResult.DamageValue = float.MaxValue;
                LogInfo("치트: 무한 피해를 적용합니다.");
                return true;
            }

            // 1 피해 치트 (공격 타입에만 적용)
            if (GameSetting.Instance.Cheat.IsOneDamageAttack)
            {
                if (damageAsset.DamageType == DamageTypes.Normal)
                {
                    damageResult.DamageValue = 1f;
                    LogInfo("치트: 1 피해를 적용합니다.");
                    return true;
                }
            }

            // 퍼센트 피해 치트 (공격 타입에만 적용)
            if (GameSetting.Instance.Cheat.IsPercentDamage)
            {
                if (damageAsset.DamageType == DamageTypes.Normal)
                {
                    if (TargetVital != null && TargetVital.MaxLife > 0f)
                    {
                        // 최대 체력의 1% 피해
                        damageResult.DamageValue = TargetVital.MaxLife * 0.01f;
                        LogInfo("치트: 퍼센트 피해를 적용합니다. (최대 체력의 1%)");
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryApplyReferenceDamage(HitmarkAssetData damageAssetData, ref DamageResult damageResult)
        {
            if (damageAssetData.LinkedDamageType == LinkedDamageTypes.Attack && ReferenceValue > 0f)
            {
                damageResult.DamageValue = ReferenceValue;
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
            return StatEx.GetValueByLevel(damageAsset.FixedDamage, damageAsset.FixedDamageByLevel, hitmarkLevel);
        }

        public void RefreshReferenceValue(HitmarkAssetData damageAsset)
        {
            switch (damageAsset.LinkedDamageType)
            {
                case LinkedDamageTypes.None:
                    return;

                case LinkedDamageTypes.Attack:
                    ReferenceValue = DamageReferenceValue;
                    LogProgressReferenceValue("피해량", ReferenceValue);
                    break;

                case LinkedDamageTypes.CurrentLifeOfAttacker:
                    if (Attacker?.MyVital != null)
                    {
                        ReferenceValue = Attacker.MyVital.CurrentLife;
                        LogProgressReferenceValue("공격자의 현재 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxLifeOfAttacker:
                    if (Attacker?.MyVital != null)
                    {
                        ReferenceValue = Attacker.MyVital.MaxLife;
                        LogProgressReferenceValue("공격자의 최대 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.CurrentShieldOfAttacker:
                    if (Attacker?.MyVital != null)
                    {
                        ReferenceValue = Attacker.MyVital.CurrentShield;
                        LogProgressReferenceValue("공격자의 현재 보호막", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxShieldOfAttacker:
                    if (Attacker?.MyVital != null)
                    {
                        ReferenceValue = Attacker.MyVital.MaxShield;
                        LogProgressReferenceValue("공격자의 최대 보호막", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.CurrentLifeOfTarget:
                    if (TargetVital != null)
                    {
                        ReferenceValue = TargetVital.CurrentLife;
                        LogProgressReferenceValue("피격자의 현재 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxLifeOfTarget:
                    if (TargetVital != null)
                    {
                        ReferenceValue = TargetVital.MaxLife;
                        LogProgressReferenceValue("피격자의 최대 생명력", ReferenceValue);
                    }
                    break;
            }

            if (ReferenceValue == 0)
            {
                LogErrorReferenceValue(damageAsset.LinkedDamageType, damageAsset.LinkedStateEffect, ReferenceValue);
            }
        }

        private void ComputeByType(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryCheatDamage(damageAsset, ref damageResult))
            {
                return;
            }

            switch (damageAsset.DamageType)
            {
                case DamageTypes.Heal:
                case DamageTypes.HealOverTime:
                    ComputeHealValue(damageAsset, ReferenceValue, ref damageResult);
                    break;

                case DamageTypes.RestoreMana:
                case DamageTypes.RestoreManaOverTime:
                    ComputeRestoreManaValue(damageAsset, ReferenceValue, ref damageResult);
                    break;

                case DamageTypes.ChargeShield:
                    ComputeChargeValue(damageAsset, ReferenceValue, ref damageResult);
                    break;

                case DamageTypes.Normal:
                    HandleComputeDamage(damageAsset, ref damageResult);
                    break;

                case DamageTypes.DamageOverTime:
                    HandleComputeDamage(damageAsset, ref damageResult);
                    break;

                case DamageTypes.Execution:
                    damageResult.DamageValue = float.MaxValue;
                    break;

                default:
                    Log.Warning("데미지 에셋에서 피해 종류(DamageType) 설정이 올바르지 않을 가능성이 있습니다: {0}, {1}", damageAsset.Name.ToLogString(), damageAsset.DamageType);
                    break;
            }
        }

        private void HandleComputeDamage(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            if (TryApplyReferenceDamage(damageAsset, ref damageResult))
            {
                return;
            }

            ComputeDamage(damageAsset, GetHitmarkLevel(), ref damageResult);
        }

        // 피해 계산
        // 문서 공식 (5.4 캐릭터 실제 피해량 계산식):
        // 1단계: 총 공격력 = 기본 공격력 + 장비 공격력 + 강화 공격력
        // 2단계: 총 데미지 = 총 공격력
        private void ComputeDamage(HitmarkAssetData damageAsset, int hitmarkLevel, ref DamageResult damageResult)
        {
            ClearLogBuilder();
            AppendLineToLog();

            // 1단계: 총 공격력 계산 (기본 공격력 + 장비 공격력 + 강화 공격력은 이미 능력치 시스템에서 합산됨)
            float totalAttackPower = FindAttackerStatValue(StatNames.Attack);
            float fixedDamage = GetFixedDamageValue(damageAsset, hitmarkLevel);
            float baseDamage = totalAttackPower + fixedDamage;

            LogDamageCalculation("피해량", baseDamage, totalAttackPower, fixedDamage);

            // 계산 공식: 최종 피해량 = 기본 피해량
            float finalDamageValue = baseDamage;

            LogDamageCalculationStart(Attacker, TargetVital, baseDamage, finalDamageValue);
            LogInfo(GetLogString());

            damageResult.DamageValue = finalDamageValue;
        }

        private void ComputeHealValue(HitmarkAssetData damageAsset, float referenceValue, ref DamageResult damageResult)
        {
            ComputeValueWithMagnification(damageAsset, referenceValue, ref damageResult);
        }

        private void ComputeRestoreManaValue(HitmarkAssetData damageAsset, float referenceValue, ref DamageResult damageResult)
        {
            ComputeValueWithMagnification(damageAsset, referenceValue, ref damageResult);
        }

        private void ComputeChargeValue(HitmarkAssetData damageAsset, float referenceValue, ref DamageResult damageResult)
        {
            ComputeValueWithMagnification(damageAsset, referenceValue, ref damageResult);
        }

        private void ComputeValueWithMagnification(HitmarkAssetData damageAsset, float referenceValue, ref DamageResult damageResult)
        {
            int hitmarkLevel = GetHitmarkLevel();
            float fixedValue = GetFixedDamageValue(damageAsset, hitmarkLevel);
            float magnification = CalculateMagnification(damageAsset);

            float result = fixedValue + (referenceValue * magnification);
            damageResult.DamageValue = result;

            if (result.IsZero())
            {
                return;
            }

            LogHealingOrResourceRestoration(damageAsset, fixedValue, referenceValue, magnification, result);
        }

        private float CalculateMagnification(HitmarkAssetData damageAsset)
        {
            float magnification = damageAsset.LinkedHitmarkMagnification;

            if (Level > 1)
            {
                magnification += (Level - 1) * damageAsset.LinkedValueMagnificationByLevel;
            }

            if (Stack > 1)
            {
                magnification += (Stack - 1) * damageAsset.LinkedValueMagnificationByStack;
            }

            return magnification;
        }

        #endregion Compute

        #region Stat Helper

        private float FindAttackerStatValue(StatNames statName)
        {
            if (Attacker != null)
            {
                return Attacker.Stat.FindValueOrDefault(statName);
            }

            return 0f;
        }

        #endregion Stat Helper
    }
}
