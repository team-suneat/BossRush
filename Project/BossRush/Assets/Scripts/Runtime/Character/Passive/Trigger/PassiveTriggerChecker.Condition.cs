using TeamSuneat.Data;
using TeamSuneat.Passive;


namespace TeamSuneat
{
    public partial class PassiveTriggerChecker
    {
        public bool CheckConditionSkill(Character conditionCharacter)
        {
            if (_conditionSettings.ConditionSkill == SkillNames.None)
            {
                return true;
            }

            if (!conditionCharacter.Skill.ContainsKey(_conditionSettings.ConditionSkill))
            {
                LogProgressConditionSkillNotFound(conditionCharacter);
                return false;
            }
            else if (conditionCharacter.Skill.ContainsKey(_conditionSettings.ConditionSkill))
            {
                SkillEntity skillEntity = conditionCharacter.Skill.Find(_conditionSettings.ConditionSkill);
                if (skillEntity.Level == 0)
                {
                    LogProgressConditionSkillLevelZero(conditionCharacter);
                    return false;
                }
            }

            return true;
        }

        public bool CheckConditionIgnoreSkill(Character conditionCharacter)
        {
            if (!_conditionSettings.ConditionIgnoreSkills.IsValidArray())
            {
                return true;
            }

            if (_conditionSettings.ConditionIgnoreSkills.IsValidArray())
            {
                for (int i = 0; i < _conditionSettings.ConditionIgnoreSkills.Length; i++)
                {
                    if (conditionCharacter.Skill.ContainsKey(_conditionSettings.ConditionIgnoreSkills[i]))
                    {
                        SkillEntity skillEntity = conditionCharacter.Skill.Find(_conditionSettings.ConditionIgnoreSkills[i]);
                        if (skillEntity.Level > 0)
                        {
                            LogProgressIgnoreSkillFound(conditionCharacter, _conditionSettings.ConditionIgnoreSkills[i]);
                            return false;
                        }
                    }
                }
            }

            if (!conditionCharacter.Skill.ContainsKey(_conditionSettings.ConditionSkill))
            {
                LogProgressConditionSkillNotFound(conditionCharacter);
                return false;
            }
            else
            {
                SkillEntity skillEntity = conditionCharacter.Skill.Find(_conditionSettings.ConditionSkill);
                if (skillEntity.Level == 0)
                {
                    LogProgressConditionSkillLevelZero(conditionCharacter);
                    return false;
                }
            }

            return true;
        }

        public bool CheckConditionSkillCooldown(Character conditionCharacter)
        {
            if (_conditionSettings.ConditionSkillCooldownCategories.IsValidArray())
            {
                for (int i = 0; i < _conditionSettings.ConditionSkillCooldownCategories.Length; i++)
                {
                    if (conditionCharacter.Skill.CheckCooldowning(_conditionSettings.ConditionSkillCooldownCategories[i]))
                    {
                        LogProgressIgnoringSkillCooldownCategories(conditionCharacter);
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckConditionBuff(Character conditionCharacter)
        {
            if (_conditionSettings.ConditionBuff == BuffNames.None && _conditionSettings.ConditionBuff2 == BuffNames.None)
            {
                return true;
            }
            else if (!conditionCharacter.Buff.ContainsKey(_conditionSettings.ConditionBuff) && !conditionCharacter.Buff.ContainsKey(_conditionSettings.ConditionBuff2))
            {
                LogProgressConditionBuffNotFound(conditionCharacter);
                return false;
            }

            return true;
        }

        public bool CheckConditionBuffStack(Character conditionCharacter)
        {
            if (_conditionSettings.ConditionBuff == BuffNames.None && _conditionSettings.ConditionBuffType == BuffTypes.None)
            {
                return true;
            }

            if (_conditionSettings.ConditionBuff != BuffNames.None)
            {
                int buffStack = conditionCharacter.Buff.GetStack(_conditionSettings.ConditionBuff);
                if (buffStack < _conditionSettings.ConditionBuffStack)
                {
                    LogProgressConditionBuffStackNotMet(_conditionSettings.ConditionBuff, _conditionSettings.ConditionBuffStack, buffStack);
                    return false;
                }
            }

            if (_conditionSettings.ConditionBuffType != BuffTypes.None)
            {
                int buffStack = conditionCharacter.Buff.GetStack(_conditionSettings.ConditionStateEffect);
                if (buffStack < _conditionSettings.ConditionBuffStack)
                {
                    LogProgressConditionBuffStackNotMet(_conditionSettings.ConditionStateEffect, _conditionSettings.ConditionBuffStack, buffStack);
                    return false;
                }
            }

            return true;
        }

        public bool CheckConditionStateEffect(Character conditionCharacter)
        {
            if (_conditionSettings.ConditionBuffType == BuffTypes.StateEffect)
            {
                if (_conditionSettings.ConditionStateEffect != StateEffects.None)
                {
                    if (!conditionCharacter.Buff.ContainsStateEffect(_conditionSettings.ConditionStateEffect))
                    {
                        LogProgressStateEffectNotFound(conditionCharacter);
                        return false;
                    }
                }
                if (_conditionSettings.ConditionStateEffectOperator != PassiveOperator.None)
                {
                    int stateEffectCount = conditionCharacter.Buff.GetStateEffectCount();
                    if (!_conditionSettings.ConditionStateEffectOperator.Compare(stateEffectCount, _conditionSettings.ConditionStateEffectCount))
                    {
                        LogProgressStateEffectCountNotMet(conditionCharacter, stateEffectCount);
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckConditionIgnoreBuff(Character ignoreCharacter)
        {
            if (_conditionSettings.ConditionIgnoreBuff == BuffNames.None)
            {
                return true;
            }

            if (ignoreCharacter.Buff.ContainsKey(_conditionSettings.ConditionIgnoreBuff))
            {
                LogProgressIgnoreBuffFound(_conditionSettings.ConditionIgnoreBuff);
                return false;
            }

            return true;
        }

        public bool CheckConditionIgnoreStateEffect(Character conditionCharacter)
        {
            if (_conditionSettings.ConditionIgnoreBuffType == BuffTypes.StateEffect)
            {
                if (_conditionSettings.ConditionIgnoreStateEffects.IsValidArray())
                {
                    foreach (StateEffects stateEffect in _conditionSettings.ConditionIgnoreStateEffects)
                    {
                        if (conditionCharacter.Buff.ContainsStateEffect(stateEffect))
                        {
                            LogProgressIgnoreStateEffectFound(conditionCharacter, stateEffect);
                            return false;
                        }
                    }
                }

                if (_conditionSettings.ConditionIgnoreStateEffectOperator != PassiveOperator.None)
                {
                    int stateEffectCount = conditionCharacter.Buff.GetStateEffectCount();
                    if (!_conditionSettings.ConditionIgnoreStateEffectOperator.Compare(stateEffectCount, _conditionSettings.ConditionStateEffectCount))
                    {
                        LogProgressIgnoreStateEffectCountNotMet(conditionCharacter, stateEffectCount);
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckConditionVitalResource(Character conditionCharacter)
        {
            if (_owner == null)
            {
                LogProgressOwnerNotFound();
                return false;
            }

            if (_conditionSettings.ConditionVitalResource == VitalResourceTypes.None)
            {
                return true;
            }

            if (_conditionSettings.ConditionVitalResourceRatio > 0)
            {
                float currentRate = conditionCharacter.MyVital.GetRate(_conditionSettings.ConditionVitalResource);
                if (!_conditionSettings.ConditionVitalResourceOperator.Compare(currentRate, _conditionSettings.ConditionVitalResourceRatio))
                {
                    LogProgressVitalResourceRatioNotMet(conditionCharacter, currentRate);
                    return false;
                }
            }
            else if (_conditionSettings.ConditionVitalResourceValue > 0)
            {
                float currentValue = conditionCharacter.MyVital.GetCurrent(_conditionSettings.ConditionVitalResource);
                if (!_conditionSettings.ConditionVitalResourceOperator.Compare(currentValue, _conditionSettings.ConditionVitalResourceValue))
                {
                    LogProgressVitalResourceValueNotMet(conditionCharacter, currentValue,
                        _conditionSettings.ConditionVitalResource,
                        _conditionSettings.ConditionVitalResourceOperator,
                        _conditionSettings.ConditionVitalResourceValue);
                    return false;
                }
            }

            return true;
        }

        public bool CheckConditionMonsterGrade(Character conditionCharacter)
        {
            if (_conditionSettings.ConditionMonsterGrades.IsValidArray())
            {
                for (int i = 0; i < _conditionSettings.ConditionMonsterGrades.Length; i++)
                {
                    if (conditionCharacter.Grade == _conditionSettings.ConditionMonsterGrades[i])
                    {
                        return true;
                    }
                }

                LogProgressMonsterGradeNotMet(conditionCharacter);
                return false;
            }

            return true;
        }

        public bool CheckConditionRelic(Character conditionCharacter)
        {
            if (_conditionSettings.ConditionRelicName == RelicNames.None)
            {
                return true;
            }

            if (!conditionCharacter.IsPlayer)
            {
                LogProgressConditionRelicNotPlayer();
                return false;
            }

            VCharacter characterInfo = GameApp.GetSelectedCharacter();
            if (characterInfo?.Relic == null)
            {
                LogProgressConditionRelicNoRelicData();
                return false;
            }

            // 유물 보유 여부 확인
            if (!characterInfo.Relic.Contains(_conditionSettings.ConditionRelicName))
            {
                LogProgressConditionRelicNotFound();
                return false;
            }

            // 스택 조건이 설정된 경우 스택 확인
            if (_conditionSettings.ConditionRelicStack > 0 && _conditionSettings.ConditionRelicOperator != PassiveOperator.None)
            {
                VRelic relicInfo = characterInfo.Relic.Find(_conditionSettings.ConditionRelicName);
                if (relicInfo == null)
                {
                    LogProgressConditionRelicNotFound();
                    return false;
                }

                int currentStack = relicInfo.StackCount;
                int requiredStack = _conditionSettings.ConditionRelicStack;

                if (!_conditionSettings.ConditionRelicOperator.Compare(currentStack, requiredStack))
                {
                    LogProgressConditionRelicStackNotMet(currentStack, requiredStack);
                    return false;
                }
            }

            return true;
        }
    }
}