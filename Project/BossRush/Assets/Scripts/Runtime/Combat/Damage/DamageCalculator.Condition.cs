using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        #region 치트

        private bool TryCheatDamage(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            // 치트 피해 관련 로직이 필요할 경우 여기에 구현
            return false;
        }

        #endregion 치트

        #region 명중률 계산

        // 명중률 계산: ACC 스탯 기반 포화 함수 (P = ACC / (ACC + K))
        private float CalcHitChance(float acc, float k = 100f, float min = 0.1f, float max = 0.95f)
        {
            if (acc <= 0f)
            {
                return min; // ACC가 0 이하면 최소값 반환
            }

            float pRaw = acc / (acc + k); // ACC=K에서 50%
            float p = Mathf.Clamp(pRaw, min, max);
            return p; // 0~1 사이
        }

        // 명중 판정: 랜덤 값과 명중률 비교
        private bool RollHit(float hitChance)
        {
            float r = RandomEx.GetFloatValue(); // 0~1
            return r <= hitChance;
        }

        #endregion 명중률 계산

        #region 조건 판정

        // 명중/회피 판정: ACC 스탯 기반 포화 함수 사용
        private bool DetermineEvasion(DamageResult damageResult)
        {
            if (damageResult.Asset.IgnoreEvasion)
            {
                return false;
            }

            if (Attacker == null || damageResult.TargetCharacter == null)
            {
                return false;
            }

            if (!damageResult.TargetCharacter.IsBoss)
            {
                // 보스 캐릭터만 회피 판정 가능
                return false;
            }

            float attackerAccuracy = 0f;
            float targetEvasion = 0f;
            float chance = 0f;
            bool isEvasion = false;

            if (Attacker.IsPlayer)
            {
                // 공격자가 플레이어: 명중만 계산 (회피 무시)
                attackerAccuracy = Attacker.Stat.FindValueOrDefault(StatNames.Accuracy);
                chance = CalcHitChance(attackerAccuracy);

                // 명중 실패 (회피 성공) 판정
                if (!RollHit(chance))
                {
                    isEvasion = true;
                }
            }
            else
            {
                // 공격자가 몬스터: 회피만 계산 (명중 무시)
                targetEvasion = damageResult.TargetCharacter.Stat.FindValueOrDefault(StatNames.Dodge);
                chance = CalcHitChance(targetEvasion); // 회피 성공 확률

                // 회피 성공 판정
                if (RollHit(chance))
                {
                    isEvasion = true;
                }
            }

            if (isEvasion)
            {
                SpawnFloatyText(StringDataLabels.FLOATY_MISS);
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_DODGE, damageResult);
                LogEvasionApplied(chance, attackerAccuracy, targetEvasion);
                return true;
            }

            LogEvasionNotApplied(chance, attackerAccuracy, targetEvasion);
            return false;
        }

        private bool DetermineImmuneCC(bool isCrowdControl)
        {
            if (!isCrowdControl)
            {
                return false;
            }

            if (TargetCharacter == null)
            {
                return false;
            }

            if (TargetCharacter.IsPlayer && GameSetting.Instance.Cheat.NotCrowdControl)
            {
                SpawnFloatyText(StringDataLabels.FLOATY_IMMUNE);
                return true;
            }

            if (TargetCharacter.IgnoreCrowdControl)
            {
                SpawnFloatyText(StringDataLabels.FLOATY_IMMUNE);
                return true;
            }

            return false;
        }

        private bool DetermineCritical(HitmarkAssetData hitmarkAssetData)
        {
            if (Attacker == null || TargetCharacter == null)
            {
                return false;
            }

            if (Attacker != TargetCharacter && Attacker.IsPlayer)
            {
                if (GameSetting.Instance.Cheat.CriticalType == GameCheat.CriticalTypes.Critical)
                {
                    LogInfoCriticalAppliedByCheat();
                    return true;
                }

                if (GameSetting.Instance.Cheat.CriticalType == GameCheat.CriticalTypes.NoCritical)
                {
                    LogInfoNoCriticalAppliedByCheat();
                    return false;
                }
            }

            float criticalChance = 0f;
            float targetCriticalChance = 0f;

            TryAddAttackerStatValue(StatNames.CriticalChance, ref criticalChance);

            GameDefineAssetData defineAssetData = ScriptableDataManager.Instance.GetGameDefine().Data;
            float resultCriticalChance = criticalChance + targetCriticalChance;

            // 치명타 확률이 0% 이하이면 치명타 발생하지 않음
            if (resultCriticalChance <= 0f)
            {
                LogCriticalHitNotApplied(resultCriticalChance, criticalChance, targetCriticalChance);
                return false;
            }

            float randomValue = RandomEx.GetFloatValue();
            if (randomValue < resultCriticalChance)
            {
                LogCriticalHitApplied(resultCriticalChance, criticalChance, targetCriticalChance);
                return true;
            }

            LogCriticalHitNotApplied(resultCriticalChance, criticalChance, targetCriticalChance);
            return false;
        }

        private bool DetermineExecute(DamageResult damageResult)
        {
            float conditionRate = damageResult.Asset.ExecutionConditionalTargetHealthRate;
            if (conditionRate <= 0f)
            {
                return false;
            }

            if (damageResult.TargetVital == null)
            {
                return false;
            }

            if (damageResult.TargetVital.HealthRate <= conditionRate)
            {
                LogExecutionApplied(damageResult.TargetVital.HealthRate, conditionRate);
                return true;
            }

            return false;
        }

        // 회심의 일격 판정: 회심의 일격 확률에 따른 랜덤 판정
        private bool DetermineDevastatingStrike(DamageResult damageResult)
        {
            if (Attacker == null || TargetCharacter == null)
            {
                return false;
            }

            // 플레이어만 회심의 일격 사용 가능
            if (!Attacker.IsPlayer)
            {
                return false;
            }

            // 회심의 일격 확률 가져오기
            float devastatingStrikeChance = Attacker.Stat.FindValueOrDefault(StatNames.DevastatingStrikeChance);

            // 회심의 일격 확률이 0% 이하이면 발생하지 않음
            if (devastatingStrikeChance <= 0f)
            {
                LogDevastatingStrikeNotApplied(devastatingStrikeChance);
                return false;
            }

            float randomValue = RandomEx.GetFloatValue();
            if (randomValue < devastatingStrikeChance)
            {
                LogDevastatingStrikeApplied(devastatingStrikeChance);
                return true;
            }

            LogDevastatingStrikeNotApplied(devastatingStrikeChance);
            return false;
        }

        #endregion 조건 판정
    }
}