using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        private float CalculateDamageReduction(DamageResult damageResult)
        {
#if UNITY_EDITOR
            int index = _stringBuilder.Length;
#endif
            GameDefineAsset defineAsset = ScriptableDataManager.Instance.GetGameDefine();
            GameDefineAssetData defineAssetData = defineAsset.Data;
            float damageReduction = 1f;

            // 공통 피해 감소율 : 0.3이면 30% 감소
            float commonRate = GetDamageReductionRate(damageResult);

            // 피해 감소율
            float combinedMultiplier = 1 - commonRate; // 예: 1 - 0.3 = 0.7(70%)
            damageReduction *= combinedMultiplier;
            AddLogDamageReduction(commonRate);

            // 피해 증폭 (쇠퇴율) 적용
            if (!DiminishingReturns.IsZero())
            {
                float DiminishingRate = 1 + DiminishingReturns;
                damageReduction *= DiminishingRate;
                AddLogDiminishingRate(DiminishingRate);
            }

            // 최종 피해 계수 로그
#if UNITY_EDITOR
            if (!damageReduction.Compare(1))
            {
                AddLogTotalDamageReduction(index, damageReduction);
            }
#endif

            return damageReduction;
        }

        private float GetDamageReductionRate(DamageResult damageResult)
        {
            float damageReduction = 0f;
            if (TryAddTargetStatValue(StatNames.DamageReduction, ref damageReduction))
            {
                LogCommonDamageReductionFromStat(damageReduction);
            }

            return damageReduction;
        }
    }
}