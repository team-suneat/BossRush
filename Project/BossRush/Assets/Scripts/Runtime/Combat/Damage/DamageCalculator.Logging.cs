using System.Text;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
#if UNITY_EDITOR
        private readonly StringBuilder _stringBuilder = new();
#endif

        #region Logging

        // StringBuilder 헬퍼 메서드들

#if UNITY_EDITOR
        private void AppendLineToLog(string value = "") => _stringBuilder.AppendLine(value);
        private void ClearLogBuilder() => _stringBuilder.Clear();
        private void InsertToLog(int index, string value) => _stringBuilder.Insert(index, value);
        private string GetLogString() => _stringBuilder.ToString();
#else
        private void AppendLineToLog(string value = "") { }
        private void ClearLogBuilder() { }
        private void InsertToLog(int index, string value) { }
        private string GetLogString() => string.Empty;
#endif

        // 기본 로깅 메서드

        private string FormatEntityLog(string content)
        {
            if (HitmarkAssetData == null)
            {
                return content;
            }

            return string.Format("{0}, {1}", HitmarkAssetData.Name.ToLogString(), content);
        }

        protected virtual void LogProgress(string format, params object[] args)
        {
            if (!Log.LevelProgress)
            {
                return;
            }

            string content = args.Length > 0 ? string.Format(format, args) : format;
            Log.Progress(LogTags.Damage, FormatEntityLog(content));
        }

        protected virtual void LogInfo(string format, params object[] args)
        {
            if (!Log.LevelInfo)
            {
                return;
            }

            string content = args.Length > 0 ? string.Format(format, args) : format;
            Log.Info(LogTags.Damage, FormatEntityLog(content));
        }

        protected virtual void LogWarning(string format, params object[] args)
        {
            if (!Log.LevelWarning)
            {
                return;
            }

            string content = args.Length > 0 ? string.Format(format, args) : format;
            Log.Warning(LogTags.Damage, FormatEntityLog(content));
        }

        protected virtual void LogError(string format, params object[] args)
        {
            if (!Log.LevelError)
            {
                return;
            }

            string content = args.Length > 0 ? string.Format(format, args) : format;
            Log.Error(LogTags.Damage, FormatEntityLog(content));
        }

        // DamageCalculator 로깅

        private void LogHealingOrResourceRestoration(HitmarkAssetData damageAsset, float fixedValue, float referenceValue, float magnification, float result)
        {
            if (!Log.LevelInfo)
            {
                return;
            }

            string format = damageAsset.DamageType.IsHeal()
                ? "생명력 회복량을 계산합니다. 고정 회복량({0}) + [참조 회복량({1}) * 참조 회복 계수({2})] = {3}"
                : "{4} 회복량을 계산합니다. 고정 회복량({0}) + [참조 회복량({1}) * 참조 회복 계수({2})] = {3}";

            LogInfo(format,
                ValueStringEx.GetValueString(fixedValue),
                ValueStringEx.GetValueString(referenceValue),
                ValueStringEx.GetPercentString(magnification, 0f),
                ValueStringEx.GetValueString(result), damageAsset.DamageType);
        }

        private void LogErrorHitmarkNotSet()
        {
            LogError("설정된 히트마크가 없습니다.");
        }

        private void LogProgressResetDamageCalculators()
        {
            LogProgress("이전 계산된 피해값을 초기화합니다.");
        }

        private void LogProgressAttacker(string path)
        {
            LogProgress("공격자를 설정합니다. {0}", path);
        }

        private void LogProgressTargetVital(string path)
        {
            LogProgress("목표 바이탈을 설정합니다. {0}", path);
        }

        private void LogWarningTargetVital()
        {
            LogWarning("목표 바이탈을 설정할 수 없습니다.");
        }

        private void LogManaCostReferenceValue(string value)
        {
            LogProgress("마나 소모 참조값을 설정합니다. Value: {0}", value);
        }

        private void LogDamageReferenceValue(string value)
        {
            LogProgress("피해 참조값을 설정합니다. Value: {0}", value);
        }

        private void LogCooldownReferenceValue(string value)
        {
            LogProgress("재사용 대기시간 참조값을 설정합니다. Value: {0}", value);
        }

        private void LogDamageCalculation(string damageType, float damageValue, float totalAttackPower, float fixedDamage)
        {
            if (!Log.LevelInfo)
            {
                return;
            }

            string format = GetDamageCalculationFormat(damageType);
            LogProgress(format,
                damageValue.ToColorString(GameColors.Physical),
                totalAttackPower,
                ValueStringEx.GetValueString(fixedDamage, 0));
        }

        private void LogDamageCalculationStart(Character attacker, Vital targetVital, float damageValue, float finalDamageValue)
        {
            if (!Log.LevelInfo)
            {
                return;
            }

            string format = "공격자({0}) ▶ 피격자({1}), 최종 피해량:{2}.\n계산식: <b>[</b>공격력({3})<b>]</b>\n";
            string attackerName = attacker != null ? attacker.Name.ToLogString() : "None";
            string targetString = string.Empty;

            if (targetVital != null)
            {
                targetString = targetVital.Owner != null ? targetVital.Owner.GetHierarchyName() : targetVital.GetHierarchyName();
            }

            InsertToLog(0, string.Format(format,
                attackerName,
                targetString,
                finalDamageValue.ToColorString(GameColors.Physical),
                damageValue.ToColorString(GameColors.Physical)));
        }

        private string GetDamageCalculationFormat(string damageType)
        {
            return $"{damageType} 피해량을 계산합니다. {{0}} 계산식: [{{1}}(총 공격력) + {{2}}(고정 피해)]";
        }

        private void LogProgressReferenceValue(string description, float value)
        {
            LogProgress("{0}을 참조값을 설정합니다. Value: {1}", description, value.ToSelectString(0));
        }

        private void LogErrorReferenceValue(LinkedDamageTypes linkedDamageType, StateEffects linkedStateEffect, float value)
        {
            LogWarning("참조값을 설정할 수 없습니다. 참조 피해 종류: {0}, 참조 상태 이상: {1}, 참조 값: {2}", linkedDamageType, linkedStateEffect, value.ToSelectString(0));
        }

        #endregion Logging
    }
}

