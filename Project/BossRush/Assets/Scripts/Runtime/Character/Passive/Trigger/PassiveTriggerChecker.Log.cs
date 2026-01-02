using TeamSuneat.Data;
using TeamSuneat.Passive;
using TeamSuneat.Stage;


namespace TeamSuneat
{
    public partial class PassiveTriggerChecker
    {
        #region Log

        private string FormatEntityLog(string content)
        {
            return string.Format("{0}, {1}", PassiveName.ToLogString(), content);
        }

        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.PassiveTrigger, FormatEntityLog(content));
            }
        }

        protected virtual void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.PassiveTrigger, formattedContent);
            }
        }

        #endregion Log

        //-------------------------------------------------------------------------------------------------------------------------------------

        private void LogProgressOwnerNotSet(Character attacker)
        {
            if (Log.LevelProgress)
            {
                LogProgress("소유 캐릭터({0})가 설정되지 않았습니다. 패시브 발동 대상 캐릭터를 검사할 수 없습니다. ", attacker.GetHierarchyName());
            }
        }

        private void LogProgressAttackerNotSet(Character attacker)
        {
            if (Log.LevelProgress)
            {
                LogProgress("공격 캐릭터({0})가 설정되지 않았습니다. 패시브 발동 대상 캐릭터를 검사할 수 없습니다. ", attacker.GetHierarchyName());
            }
        }

        private void LogProgressOwnerMismatch(Character attacker)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 발동 대상 캐릭터({0})와 소유 캐릭터({1})가 다릅니다. ", _owner.GetHierarchyName(), attacker.GetHierarchyName());
            }
        }

        private void LogProgressTargetNotSet(Character targetCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("목표 캐릭터({0})가 설정되지 않았습니다. 패시브 발동 대상 캐릭터를 검사할 수 없습니다. ", targetCharacter.GetHierarchyName());
            }
        }

        private void LogProgressTargetMismatch(Character targetCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 발동 대상 캐릭터({0})와 목표 캐릭터({1})가 다릅니다. ", _owner.GetHierarchyName(), targetCharacter.GetHierarchyName());
            }
        }

        private void LogProgressFailedChance(PassiveTriggers trigger, float triggerChance)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브가 확률로 발동되지 못했습니다. 트리거: {0}, 확률: {1}", trigger.ToLogString(), ValueStringEx.GetPercentString(triggerChance));
            }
        }

        private void LogProgressTriggerCountExceeded(int triggerCount)
        {
            if (Log.LevelProgress)
            {
                LogProgress("발동 가능 횟수가 한계에 도달했습니다. 발동 가능 횟수: {0}/{1}", triggerCount.ToString(), _triggerSettings.TriggerCount.ToString());
            }
        }

        private void LogProgressFullPotionCheckFailed()
        {
            if (Log.LevelProgress)
            {
                LogProgress("포션 가득참 조건 검사(최대 포션 보유)가 실패했습니다. 포션의 수: {0}/{1}", ProfileInfo.Potion.Potion1Count, ProfileInfo.Potion.GetMaxCount());
            }
        }

        private void LogProgressEmptyPotionCheckFailed()
        {
            if (Log.LevelProgress)
            {
                LogProgress("포션 비어있음 조건 검사(포션을 하나도 보유하지 않음)가 실패했습니다. 포션의 수: {0}/{1}", ProfileInfo.Potion.Potion1Count, ProfileInfo.Potion.GetMaxCount());
            }
        }

        private void LogProgressTriggerDamageTypeMismatch(DamageTypes damageType)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 발동 데미지타입(DamageTypes)이 일치하지 않습니다. 발동 데미지타입: {0}, 실제 데미지타입: {1}", _triggerSettings.TriggerDamageTypes.JoinToString(), damageType.ToString());
            }
        }

        private void LogProgressTriggerEntityTypeMismatch(AttackEntityTypes entityType)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 발동 가능 공격 엔티티 타입(AttackEntityTypes)이 일치하지 않습니다. 발동: {0}, 실제: {1}", _triggerSettings.TriggerEntityType, entityType.ToString());
            }
        }

        private void LogProgressTriggerSkillCategoryMismatch(SkillNames skillName, SkillCategories skillCategory)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 발동 스킬 카테고리가 다릅니다. 발동 스킬 카테고리:({0}), 실제 스킬: ({1})",
                    _triggerSettings.TriggerSkillCategories.JoinToString(), skillCategory.ToLogString());
            }
        }

        private void LogProgressTriggerSkillElementMismatch(SkillNames skillName, GameElements skillElement)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 발동 가능한 속성이 다릅니다. 발동 스킬 속성:({0}), 실제 스킬 속성: ({1})",
                    _triggerSettings.TriggerSkillElements.JoinToString(), skillElement.GetLocalizedString(Setting.LanguageNames.Korean));
            }
        }

        private void LogProgressInvalidTriggerSkill(SkillNames skillName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 패시브를 발동시키는 스킬({0})이 올바르지 않습니다.", skillName);
            }
        }

        private void LogProgressHitmarkMismatch(HitmarkNames hitmarkName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 발동 히트마크가 일치하지 않습니다. 실제 히트마크: {0}, 발동 히트마크: {1}",
                    hitmarkName.ToLogString(), _triggerSettings.TriggerHitmarks.ToLogString());
            }
        }

        private void LogProgressIgnoreHitmarkMatch(HitmarkNames hitmarkName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 발동 무시 히트마크에 해당합니다. 발동 히트마크: {0}, 무시 히트마크: {1}", _triggerSettings.TriggerIgnoreHitmarks.JoinToString(), hitmarkName);
            }
        }

        private void LogProgressOwnerNotFound()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 발동 주체인 소유자 캐릭터가 없습니다.");
            }
        }

        private void LogProgressSkillMismatch(SkillNames skillName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 사용한 스킬이 발동 조건인 특정 스킬이 아닙니다. 발동 스킬: {0}, 실제 스킬: {1}", _triggerSettings.TriggerSkill, skillName);
            }
        }

        private void LogProgressStatMismatch(StatNames statName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 검사하는 능력치({0})가 발동 능력치({1})와 다릅니다.", statName.ToLogString(), _triggerSettings.TriggerStats.JoinToString());
            }
        }

        private void LogProgressBuffMismatch(BuffNames buffName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 검사하는 버프({0})가 발동 버프({1})와 다릅니다.", buffName.ToLogString(), _triggerSettings.TriggerBuffs.JoinToString());
            }
        }

        private void LogProgressInvalidBuffAsset(BuffNames buffName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 검사하는 버프 에셋({0})을 찾을 수 없습니다.", buffName.ToLogString());
            }
        }

        private void LogProgressBuffTypeMismatch(BuffNames buffName, BuffAsset buffAsset)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 검사하는 버프({0})의 타입({1})이 발동 버프 타입({2})과 다릅니다.", buffName.ToLogString(), buffAsset.Data.Type.ToLogString(), _triggerSettings.TriggerBuffType.ToLogString());
            }
        }

        private void LogProgressTargetCharacterNotFound()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 발동 대상이자 목표 캐릭터가 없습니다.");
            }
        }

        private void LogProgressBuffTypeNotContained(Character targetCharacter, BuffNames buffName, BuffAssetData buffData)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 목표가 발동 상태이펙트를 보유하고 있지 않습니다. 목표 캐릭터: {0}, 발동 상태이펙트: {1}, 보유 상태이펙트: {2}({3})",
                targetCharacter.GetHierarchyName(), _triggerSettings.TriggerStateEffects.JoinToString(), buffName.ToLogString(), buffData.StateEffect.ToLogString());
            }
        }

        private void LogProgressStateEffectNotContained(Character targetCharacter, StateEffects stateEffect)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 목표가 발동 상태이펙트를 보유하고 있지 않습니다. 목표 캐릭터: {0}, 발동 상태이펙트: {1}, 보유 상태이펙트: {2}",
                targetCharacter.GetHierarchyName(), _triggerSettings.TriggerStateEffects.JoinToString(), stateEffect.ToLogString());
            }
        }

        private void LogProgressIgnoreStateEffectOnHit(Character targetCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 트리거시 무시 해야 하는 상태이펙트를 보유합니다. 목표 캐릭터:{0}, 무시 해야 하는 상태이펙트: {1}",
                    targetCharacter.GetHierarchyName(), _triggerSettings.TriggerIgnoreStateEffectOnHit.ToLogString());
            }
        }

        private void LogProgressIgnoreDamageType(DamageResult damageResult)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 트리거시 무시 하는 데미지 타입에 해당합니다. 무시 데미지 타입: {0}", damageResult.DamageType.ToLowerString());
            }
        }

        private void LogProgressMonsterRange()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. {0} 거리 안에 몬스터가 없습니다.", _triggerSettings.TriggerMonsterRange.ToLowerString());
            }
        }

        private void LogProgressContainsTriggerBuffTypeInMonsterCount(int containsCount)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 트리거하는 몬스터 카운트 조건을 만족하지 못했습니다. {0}, {1}/{2}", _triggerSettings.TriggerMonsterCountOperator, containsCount, _triggerSettings.TriggerMonsterCount);
            }
        }

        private void LogProgressTriggerGradeNotMet(GradeNames gradeName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 트리거하는 무기 등급 조건을 만족하지 못했습니다. {0} != {1}", gradeName.ToLogString(), _triggerSettings.TriggerGrades.ToLogString());
            }
        }

        private void LogProgressTriggerStatOperatorNotMet()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 트리거하는 능력치 조건을 만족하지 못했습니다. {0}", _triggerSettings.TriggerOperator);
            }
        }

        private void LogProgressConditionSkillNotFound(Character conditionCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 조건 캐릭터가 해당 스킬을 보유하고 있지 않습니다. 목표 캐릭터: {0}, 조건 스킬: {1}", conditionCharacter.Name, _conditionSettings.ConditionSkill);
            }
        }

        private void LogProgressConditionSkillLevelZero(Character conditionCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 목표 캐릭터가 해당 스킬을 보유하고 있지만, 레벨이 0입니다. 목표 캐릭터: {0}, 조건 스킬: {1}", conditionCharacter.Name, _conditionSettings.ConditionSkill);
            }
        }

        private void LogProgressIgnoreSkillFound(Character conditionCharacter, SkillNames skillName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 목표 캐릭터가 무시해야 할 스킬을 보유하고 있습니다. 목표 캐릭터: {0}, 무시 스킬: {1}", conditionCharacter.Name, skillName);
            }
        }

        private void LogProgressSkillNotCooldowning(Character conditionCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 캐릭터({0})가 조건 스킬 쿨다운 카테고리({1})에 해당 스킬이 쿨다운 하고 있지 않습니다.", conditionCharacter.Name.ToLogString(), _conditionSettings.ConditionSkillCooldownCategories.JoinToString());
            }
        }

        private void LogProgressIgnoringSkillCooldownCategories(Character conditionCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 캐릭터({0})가 조건 스킬 쿨다운 카테고리({1})에 해당 스킬 쿨다운을 무시합니다.", conditionCharacter.Name.ToLogString(), _conditionSettings.ConditionSkillCooldownCategories.JoinToString());
            }
        }

        private void LogProgressSkillCooldownCategoriesNotMet(Character conditionCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 캐릭터({0})가 조건 스킬 쿨다운 카테고리({1})에 해당 스킬이 쿨다운 하고 있지 않습니다.", conditionCharacter.Name.ToLogString(), _conditionSettings.ConditionSkillCooldownCategories.JoinToString());
            }
        }

        private void LogProgressIgnoreBuffFound(BuffNames buffName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 조건 캐릭터가 무시해야 할 버프를 보유하고 있습니다. 발동 버프: {0}", buffName);
            }
        }

        private void LogProgressConditionBuffNotFound(Character conditionCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 조건 캐릭터가 해당 버프를 보유하고 있지 않습니다. 조건 캐릭터: {0}, 발동 버프: {1}, {2}", conditionCharacter.GetHierarchyName(), _conditionSettings.ConditionBuff, _conditionSettings.ConditionBuff2);
            }
        }

        private void LogProgressConditionBuffStackNotMet(object conditionBuff, int conditionBuffStack, int buffStack)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 조건 캐릭터가 해당 버프를 보유하고 있지 않습니다. 조건 버프: {0}, 조건 버프 스택: {1}, 실제 버프 스택: {2}", conditionBuff, conditionBuffStack.ToString(), buffStack.ToString());
            }
        }

        private void LogProgressStateEffectNotFound(Character conditionCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 조건 캐릭터({0})가 해당 상태이펙트({1})를 보유하고 있지 않습니다.", conditionCharacter.GetHierarchyName(), _conditionSettings.ConditionStateEffect.ToLogString());
            }
        }

        private void LogProgressStateEffectCountNotMet(Character conditionCharacter, int stateEffectCount)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 조건 캐릭터({0})가 해당 상태이펙트({1})의 개수 조건을 만족하지 않습니다. ({2}/{3})", conditionCharacter.GetHierarchyName(), _conditionSettings.ConditionStateEffect.ToLogString(), stateEffectCount, _conditionSettings.ConditionStateEffectCount);
            }
        }

        private void LogProgressIgnoreStateEffectFound(Character conditionCharacter, StateEffects stateEffect)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 조건 캐릭터가 무시하는 해당 상태이펙트를 보유하고 있습니다. 캐릭터: {0}, 보유 상태이펙트: {1}, 무시하는 해당 상태이펙트: {2}",
                    conditionCharacter.GetHierarchyName(), stateEffect.ToLogString(), _conditionSettings.ConditionIgnoreStateEffects.JoinToString());
            }
        }

        private void LogProgressIgnoreStateEffectCountNotMet(Character conditionCharacter, int stateEffectCount)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 조건 캐릭터({0})가 무시하는 해당 상태이펙트({1})의 개수 조건을 만족하지 않습니다. ({2}/{3})", conditionCharacter.GetHierarchyName(), _conditionSettings.ConditionStateEffect.ToLogString(), stateEffectCount, _conditionSettings.ConditionStateEffectCount);
            }
        }

        private void LogProgressVitalResourceRatioNotMet(Character conditionCharacter, float currentRate)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 조건 캐릭터({0})의 생명 자원({1}) 비율이 조건을 만족하지 않습니다. 연산자: {2}, 현재 비율: {3}, 조건 비율: {4}", conditionCharacter.GetHierarchyName(), _conditionSettings.ConditionVitalResource, _conditionSettings.ConditionVitalResourceOperator, ValueStringEx.GetPercentString(currentRate), ValueStringEx.GetPercentString(_conditionSettings.ConditionVitalResourceRatio));
            }
        }

        private void LogProgressVitalResourceValueNotMet(Character conditionCharacter, float currentValue, VitalResourceTypes vitalResource, PassiveOperator vitalResourceOperator, int vitalResourceValue)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동이 실패했습니다. 캐릭터({0})의 생명 자원({1}) 값이 조건을 만족하지 않습니다. 연산자: {2}, 현재 자원 값: {3}, 자원 값: {4}",
                    conditionCharacter.GetHierarchyName(), vitalResource, vitalResourceOperator,
                    ValueStringEx.GetValueString(currentValue), ValueStringEx.GetValueString(vitalResourceValue));
            }
        }

        private void LogProgressMonsterGradeNotMet(Character conditionCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동을 실패했습니다. 대상 캐릭터({0}, {1})가 설정 등급({2})에 해당하지 않습니다.", conditionCharacter.Name.ToLogString(), conditionCharacter.Grade, _conditionSettings.ConditionMonsterGrades.JoinToString());
            }
        }

        private void LogProgressConditionRelicNotPlayer()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동을 실패했습니다. 유물 조건은 플레이어 캐릭터에만 적용됩니다.");
            }
        }

        private void LogProgressConditionRelicNoRelicData()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동을 실패했습니다. 캐릭터의 유물 데이터를 찾을 수 없습니다.");
            }
        }

        private void LogProgressConditionRelicNotFound()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동을 실패했습니다. 캐릭터가 필요한 유물({0})을 보유하고 있지 않습니다.", _conditionSettings.ConditionRelicName.ToLogString());
            }
        }

        private void LogProgressConditionRelicStackNotMet(int currentStack, int requiredStack)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 작동을 실패했습니다. 유물({0})의 스택이 조건을 만족하지 않습니다. 현재 스택: {1}, 필요 조건: {2} {3}",
                    _conditionSettings.ConditionRelicName.ToLogString(),
                    currentStack,
                    _conditionSettings.ConditionRelicOperator.ToSelectString(),
                    requiredStack);
            }
        }
    }
}