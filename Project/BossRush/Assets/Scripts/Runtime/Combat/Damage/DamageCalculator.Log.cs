using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        #region 기본 로깅 메서드

        private string FormatEntityLog(string content)
        {
            if (HitmarkAssetData != null)
            {
                return string.Format("{0}, {1}", HitmarkAssetData.Name.ToLogString(), content);
            }
            else
            {
                return content;
            }
        }

        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Damage, FormatEntityLog(content));
            }
        }

        protected virtual void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.Damage, formattedContent);
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Damage, FormatEntityLog(content));
            }
        }

        protected virtual void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.Damage, formattedContent);
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Damage, FormatEntityLog(content));
            }
        }

        protected virtual void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.Damage, formattedContent);
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.Damage, FormatEntityLog(content));
            }
        }

        protected virtual void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Error(LogTags.Damage, formattedContent);
            }
        }

        #endregion 기본 로깅 메서드

        #region DamageCalculator 로깅

        private void LogHealingOrResourceRestoration(HitmarkAssetData damageAsset, float fixedValue, float referenceValue, float magnification, float result)
        {
            if (Log.LevelInfo)
            {
                string format;
                if (damageAsset.DamageType.IsHeal())
                {
                    format = "생명력 회복량을 계산합니다. 고정 회복량({0}) + [참조 회복량({1}) * 참조 회복 계수({2})] = {3}";
                }
                else
                {
                    format = "{4} 회복량을 계산합니다. 고정 회복량({0}) + [참조 회복량({1}) * 참조 회복 계수({2})] = {3}";
                }

                LogInfo(format,
                    ValueStringEx.GetValueString(fixedValue),
                    ValueStringEx.GetValueString(referenceValue),
                    ValueStringEx.GetPercentString(magnification, 0f),
                    ValueStringEx.GetValueString(result), damageAsset.DamageType);
            }
        }

        private void LogErrorHitmarkNotSet()
        {
            if (Log.LevelError)
            {
                LogError("설정된 히트마크가 없습니다.");
            }
        }

        private void LogProgressResetDamageResults()
        {
            if (Log.LevelProgress)
            {
                LogProgress("이전 계산된 피해값을 초기화합니다.");
            }
        }

        private void LogProgressAttacker(string path)
        {
            if (Log.LevelProgress)
            {
                LogProgress("공격자를 설정합니다. {0}", path);
            }
        }

        private void LogProgressTargetVital(string path)
        {
            if (Log.LevelProgress)
            {
                LogProgress("목표 바이탈을 설정합니다. {0}", path);
            }
        }

        private void LogWarningTargetVital()
        {
            if (Log.LevelWarning)
            {
                LogWarning("목표 바이탈을 설정할 수 없습니다.");
            }
        }

        private void LogProgressManaCostReferenceValue(string value)
        {
            if (Log.LevelProgress)
            {
                LogProgress("마나 소모 참조값을 설정합니다. Value: {0}", value);
            }
        }

        private void LogProgressDamageReferenceValue(string value)
        {
            if (Log.LevelProgress)
            {
                LogProgress("피해 참조값을 설정합니다. Value: {0}", value);
            }
        }

        private void LogProgressCooldownReferenceValue(string value)
        {
            if (Log.LevelProgress)
            {
                LogProgress("재사용 대기시간 참조값을 설정합니다. Value: {0}", value);
            }
        }

        #endregion DamageCalculator 로깅

        #region 조건 판정 로깅

        private void LogInfoCriticalAppliedByCheat()
        {
            if (Log.LevelInfo)
            {
                LogInfo("치트를 통해 치명타를 적용합니다.");
            }
        }

        private void LogInfoNoCriticalAppliedByCheat()
        {
            if (Log.LevelInfo)
            {
                LogInfo("치트를 통해 치명타를 적용하지 않습니다.");
            }
        }

        private void LogCriticalHitApplied(float resultCriticalChance, float criticalChance, float targetCriticalChance)
        {
            if (Log.LevelInfo)
            {
                LogInfo("치명타가 적용되었습니다. 결과: {0}, 시전자: {1}, 피격자: {2}",
                    ValueStringEx.GetPercentString(resultCriticalChance, 0),
                    ValueStringEx.GetPercentString(criticalChance, 0),
                    ValueStringEx.GetPercentString(targetCriticalChance, 0));
            }
        }

        private void LogCriticalHitNotApplied(float resultCriticalChance, float criticalChance, float targetCriticalChance)
        {
            if (Log.LevelInfo)
            {
                LogInfo("치명타가 적용되지 않았습니다. 결과: {0}, 시전자: {1}, 피격자: {2}",
                    ValueStringEx.GetPercentString(resultCriticalChance, 0),
                    ValueStringEx.GetPercentString(criticalChance, 0),
                    ValueStringEx.GetPercentString(targetCriticalChance, 0));
            }
        }

        private void LogExecutionApplied(float targetHealthRate, float executionConditionalTargetHealthRate)
        {
            if (Log.LevelInfo)
            {
                LogInfo("처형이 적용되었습니다. 목표 생명력 비율: {0}, 처형 조건: {1}",
                    ValueStringEx.GetPercentString(targetHealthRate, 0),
                    ValueStringEx.GetPercentString(executionConditionalTargetHealthRate, 0));
            }
        }

        private void LogEvasionApplied(float hitChance, float attackerAccuracy, float targetEvasion)
        {
            if (Log.LevelInfo)
            {
                LogInfo("회피가 적용되었습니다. 명중 확률: {0}, 공격자 명중률: {1}, 피격자 회피율: {2}",
                    ValueStringEx.GetPercentString(hitChance, 0),
                    ValueStringEx.GetPercentString(attackerAccuracy, 0),
                    ValueStringEx.GetPercentString(targetEvasion, 0));
            }
        }

        private void LogEvasionNotApplied(float hitChance, float attackerAccuracy, float targetEvasion)
        {
            if (Log.LevelInfo)
            {
                LogInfo("명중이 적용되었습니다. 명중 확률: {0}, 공격자 명중률: {1}, 피격자 회피율: {2}",
                    ValueStringEx.GetPercentString(hitChance, 0),
                    ValueStringEx.GetPercentString(attackerAccuracy, 0),
                    ValueStringEx.GetPercentString(targetEvasion, 0));
            }
        }

        private void LogDevastatingStrikeApplied(float devastatingStrikeChance)
        {
            if (Log.LevelInfo)
            {
                LogInfo("회심의 일격이 적용되었습니다. 확률: {0}",
                    ValueStringEx.GetPercentString(devastatingStrikeChance, 0));
            }
        }

        private void LogDevastatingStrikeNotApplied(float devastatingStrikeChance)
        {
            if (Log.LevelInfo)
            {
                LogInfo("회심의 일격이 적용되지 않았습니다. 확률: {0}",
                    ValueStringEx.GetPercentString(devastatingStrikeChance, 0));
            }
        }

        #endregion 조건 판정 로깅

        #region 피해 계산 로깅

        private void LogDamageCalculation(string damageType, float damageValue, float totalAttackPower, float fixedDamage)
        {
            if (Log.LevelInfo)
            {
                string format = GetDamageCalculationFormat(damageType);
                LogProgress(format,
                    damageValue.ToColorString(GameColors.Physical),
                    totalAttackPower,
                    ValueStringEx.GetValueString(fixedDamage, 0));
            }
        }

        private void LogDamageReduction(string damageType, float damageReduction)
        {
            if (Log.LevelProgress)
            {
                string format = GetDamageReductionFormat(damageType);
                LogProgress(format, ValueStringEx.GetPercentString(-1 + damageReduction, 0));
            }
        }

        private void LogDamageCalculationStart(Character attacker, Vital targetVital, float damageValue, float damageMultiplier,
            float damageReduction, float damageAmplification, float finalDamageValue)
        {
            if (Log.LevelInfo)
            {
                string format = "공격자({0}) ▶ 피격자({1}), 최종 피해량:{6}.\n계산식: <b>[</b>공격력({2}) * 공격력 배율({3})<b>]</b> * 피해 감소 적용 배율({4}) * 피해 증폭({5})\n";
                string targetString = string.Empty;
                string attackerName = attacker != null ? attacker.Name.ToLogString() : "None";

                if (targetVital != null)
                {
                    targetString = targetVital.Owner != null ? targetVital.Owner.GetHierarchyName() : targetVital.GetHierarchyName();
                }

                InsertToLog(0, string.Format(format,
                    attackerName,
                    targetString,
                    damageValue.ToColorString(GameColors.Physical),
                    ValueStringEx.GetPercentString(damageMultiplier, 1),
                    ValueStringEx.GetPercentString(damageReduction, 0),
                    ValueStringEx.GetPercentString(damageAmplification, 1),
                    finalDamageValue.ToColorString(GameColors.Physical)));
            }
        }

        private string GetDamageCalculationFormat(string damageType)
        {
            return $"{damageType} 피해량을 계산합니다. {{0}} 계산식: [{{1}}(총 공격력) + {{2}}(고정 피해)]";
        }

        private string GetDamageReductionFormat(string damageType)
        {
            return $"{damageType} 피해 감소량을 계산합니다. {0}.";
        }

        private void WarnWeaponDamageNotProvided(HitmarkAssetData damageAsset)
        {
            if (Log.LevelWarning)
            {
                Log.Warning("무기 피해를 사용하도록 설정되었지만 주입된 무기 피해가 없습니다. Hitmark:{0}", damageAsset.Name.ToLogString());
            }
        }

        #endregion 피해 계산 로깅

        #region 피해 증폭 로깅

        private void LogDevastatingStrikeRate(float devastatingStrikeRate)
        {
            if (Log.LevelProgress)
            {
                LogProgress("공격자의 회심의 일격 피해 배율({0})을 적용합니다.", ValueStringEx.GetPercentString(devastatingStrikeRate, 1));
            }
        }

        private void LogDamageAmplificationByStat(StatNames statName, float statValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("능력치({0})에 의해 피해량 증폭이 적용됩니다. {1}", statName.ToLogString(), ValueStringEx.GetPercentString(statValue, 0));
            }
        }

        #endregion 피해 증폭 로깅

        #region 피해 감소 로깅

        private void AddLogDamageReduction(float reduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.Append(string.Format("{0}(능력치에 의한 피해 감소)", ValueStringEx.GetPercentString(reduction, 0)));
#endif
            }
        }

        private void AddLogTotalDamageReduction(int index, float result)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.Insert(index, "최종 피해 감소량을 계산합니다. [");
                _stringBuilder.AppendLine($"] => {ValueStringEx.GetPercentString(result, 0)}");
#endif
            }
        }

        private void AddLogDiminishingRate(float decrescenceRate)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                _stringBuilder.Append(string.Format(" * {0}(히트마크의 피해량 점감)", ValueStringEx.GetPercentString(decrescenceRate, 0)));
#endif
            }
        }

        private void LogCommonDamageReductionFromStat(float reduction)
        {
            if (Log.LevelInfo)
            {
#if UNITY_EDITOR
                LogInfo("공통 피해 감소량을 계산합니다. {0}(능력치에 의한 공통 피해 감소)", ValueStringEx.GetPercentString(reduction, 0));
#endif
            }
        }

        #endregion 피해 감소 로깅

        #region 피해 배율 로깅

        private void AddStackMultiplier(int stackMultiplier)
        {
#if UNITY_EDITOR
            _stringBuilder.Append(string.Format(" + {0}(스택에 따른 피해량 배율)", ValueStringEx.GetPercentString(stackMultiplier, 1)));
#endif
        }

        private string GenerateContent(int index, float damageMultiplierFromDamageType, string koreanText, string englishText)
        {
            string content = string.Format(koreanText + "{0}", ValueStringEx.GetPercentString(damageMultiplierFromDamageType, 1));
            return content;
        }

        private void LogCriticalDamageRate(float criticalDamageRate)
        {
            if (Log.LevelProgress)
            {
                LogProgress("공격자의 치명타 피해 배율({0})을 적용합니다.", ValueStringEx.GetPercentString(criticalDamageRate, 1));
            }
        }

        private void LogProgressDamageTypeMultiplier(float damageRate, float elementDamageRate, float magicDamageRateByStates, float damageOverTimeRate, float result)
        {
            if (Log.LevelProgress)
            {
                LogProgress(string.Format("피해량 배율({0}) + 속성 피해량 배율({1}) + 상태이상에 따른 피해량 배율({2}) + 지속 피해량 배율({3}) = {4}",
                    ValueStringEx.GetPercentString(damageRate, 1),
                    ValueStringEx.GetPercentString(elementDamageRate, 0),
                    ValueStringEx.GetPercentString(magicDamageRateByStates, 0),
                    ValueStringEx.GetPercentString(damageOverTimeRate, 0),
                    ValueStringEx.GetPercentString(result, 1)));
            }
        }

        #endregion 피해 배율 로깅

        #region 참조값 로깅

        private void LogProgressReferenceValue(string description, float value)
        {
            if (Log.LevelProgress)
            {
                LogProgress("{0}을 참조값을 설정합니다. Value: {1}", description, value.ToSelectString(0));
            }
        }

        private void LogErrorReferenceValue(LinkedDamageTypes linkedDamageType, StateEffects linkedStateEffect, float value)
        {
            if (Log.LevelWarning)
            {
                LogWarning("참조값을 설정할 수 없습니다. 참조 피해 종류: {0}, 참조 상태 이상: {1}, 참조 값: {2}", linkedDamageType, linkedStateEffect, value.ToSelectString(0));
            }
        }

        #endregion 참조값 로깅
    }
}
