using System.Collections.Generic;
using System.Linq;
using TeamSuneat.Data;
using TeamSuneat.Passive;
using TeamSuneat.Setting;
using TeamSuneat.Stage;

using UnityEngine;

namespace TeamSuneat
{
    public partial class PassiveTriggerChecker
    {
        public bool CheckTriggerOwner(DamageResult damageResult)
        {
            if (_triggerSettings.TriggerOwner == PassiveTargetTypes.Owner)
            {
                if (_owner == null)
                {
                    LogProgressOwnerNotSet(damageResult.Attacker);
                    return false;
                }
            }
            else if (_triggerSettings.TriggerOwner == PassiveTargetTypes.Attacker)
            {
                if (damageResult.Attacker == null)
                {
                    LogProgressAttackerNotSet(damageResult.Attacker);
                    return false;
                }
                else if (_owner.SID != damageResult.Attacker.SID)
                {
                    LogProgressOwnerMismatch(damageResult.Attacker);
                    return false;
                }
            }
            else if (_triggerSettings.TriggerOwner == PassiveTargetTypes.Target)
            {
                if (damageResult.TargetCharacter == null)
                {
                    LogProgressTargetNotSet(damageResult.TargetCharacter);
                    return false;
                }
                else if (_owner.SID != damageResult.TargetCharacter.SID)
                {
                    LogProgressTargetMismatch(damageResult.TargetCharacter);
                    return false;
                }
            }

            return true;
        }

        public bool CheckTriggerChance(int passiveLevel)
        {
            if (GameSetting.Instance.Cheat.TriggerChanceType == GameCheat.TriggerChanceTypes.Pass)
            {
                return true;
            }
            else if (GameSetting.Instance.Cheat.TriggerChanceType == GameCheat.TriggerChanceTypes.Ignore)
            {
                return false;
            }

            float chance = CalculateTriggerChance(passiveLevel);
            if (chance > 0)
            {
                float randomValue = TSRandomEx.GetFloatValue();
                if (randomValue > chance)
                {
                    LogProgressFailedChance(_triggerSettings.Trigger, chance);
                    return false;
                }
            }

            return true;
        }

        private float CalculateTriggerChance(int passiveLevel)
        {
            switch (_triggerSettings.TriggerChanceCalcType)
            {
                case PassiveTriggerChanceCalcType.None:
                    {
                        // 발동 확률을 검사하지 않습니다. 반드시 발동합니다.
                        return 1f;
                    }

                case PassiveTriggerChanceCalcType.Fixed:
                    {
                        if (_triggerSettings.TriggerChance.IsZero())
                        {
                            Log.Error("발동 확률이 설정되지 않았습니다. 이 패시브는 발동하지 않습니다: {0}", _triggerSettings.Name);
                            return 0f;
                        }

                        if (_triggerSettings.TriggerChanceByLevel.IsZero())
                        {
                            return _triggerSettings.TriggerChance;
                        }
                        else
                        {
                            return TSStatEx.GetValueByLevel(_triggerSettings.TriggerChance, _triggerSettings.TriggerChanceByLevel, passiveLevel);
                        }
                    }

                case PassiveTriggerChanceCalcType.FromStat:
                    {
                        if (_triggerSettings.TriggerChanceByStat == StatNames.None)
                        {
                            Log.Error("발동 확률을 결정짓는 능력치가 설정되지 않았습니다. 이 패시브는 발동하지 않습니다: {0}", _triggerSettings.Name);
                            return 0f;
                        }

                        if (_owner == null || _owner.Stat == null)
                        {
                            Log.Error("패시브를 발동시킬 캐릭터 또는 캐릭터의 능력치 시스템이 설정되지 않았습니다. 이 패시브는 발동하지 않습니다: {0}", _triggerSettings.Name);
                            return 0f;
                        }

                        return _owner.Stat.FindValueOrDefault(_triggerSettings.TriggerChanceByStat);
                    }

                case PassiveTriggerChanceCalcType.FromOptionRange:
                    {
                        if (_triggerSettings.TriggerChanceOptionMinRange.IsZero() || _triggerSettings.TriggerChanceOptionMaxRange.IsZero())
                        {
                            Log.Error("발동 확률 범위가 설정되지 않았습니다. 이 패시브는 발동하지 않습니다: {0}", _triggerSettings.Name);
                            return 0f;
                        }

                        RunewordOption runewordOption = ProfileInfo.Inventory.GetEquippedRunewordOption(_triggerSettings.Name);
                        if (runewordOption != null)
                        {
                            if (runewordOption.TriggerChance > 0)
                            {
                                return runewordOption.TriggerChance;
                            }

                            return _triggerSettings.TriggerChanceOptionMinRange;
                        }

                        Log.Warning("전설 장비에 설정된 패시브({0})의 발동 확률을 불러올 수 없습니다. 최소 확률({1})을 반환합니다.",
                            _triggerSettings.Name.ToLogString(), ValueStringEx.GetPercentString(_triggerSettings.TriggerChanceOptionMinRange));

                        return _triggerSettings.TriggerChanceOptionMinRange;
                    }

                default:
                    Log.Error("정의되지 않은 발동 확률 타입이 사용되었습니다: {0}", _triggerSettings.TriggerChanceCalcType);

                    // 발동 확률을 검사하지 않습니다. 반드시 발동합니다. (기본값)
                    return 1f;
            }
        }

        public bool CheckTriggerCount(int triggerCount)
        {
            if (_triggerSettings.TriggerCount > 0)
            {
                if (_triggerSettings.TriggerCount > triggerCount + 1)
                {
                    LogProgressTriggerCountExceeded(triggerCount);
                    return false;
                }
            }

            return true;
        }

        public bool CheckTriggerVitalResource(Character conditionCharacter, int usedResource)
        {
            if (_owner == null)
            {
                LogProgressOwnerNotFound();
                return false;
            }
            else if (_triggerSettings.TriggerResourceType == VitalResourceTypes.None)
            {
                return true;
            }
            else if (_triggerSettings.TriggerResourceValue > 0)
            {
                if (!_triggerSettings.TriggerResourceOperator.Compare(usedResource, _triggerSettings.TriggerResourceValue))
                {
                    LogProgressVitalResourceValueNotMet(conditionCharacter, usedResource,
                    _triggerSettings.TriggerResourceType,
                    _triggerSettings.TriggerResourceOperator,
                    _triggerSettings.TriggerResourceValue);

                    return false;
                }
            }

            return true;
        }

        public bool CheckTriggerPotion()
        {
            switch (_triggerSettings.TriggerCheckType)
            {
                case TriggerCheckerTypes.FullPotion:
                    if (!ProfileInfo.Potion.CheckFullPotion1())
                    {
                        LogProgressFullPotionCheckFailed();
                        return false;
                    }
                    break;

                case TriggerCheckerTypes.EmptyPotion:
                    if (ProfileInfo.Potion.Potion1Count != 0)
                    {
                        LogProgressEmptyPotionCheckFailed();
                        return false;
                    }
                    break;
            }

            return true;
        }

        public bool CheckTriggerDamageType(DamageTypes damageType)
        {
            if (!_triggerSettings.TriggerDamageTypes.IsValidArray())
            {
                // 검사하지 않음
                return true;
            }

            for (int i = 0; i < _triggerSettings.TriggerDamageTypes.Length; i++)
            {
                if (_triggerSettings.TriggerDamageTypes[i] == DamageTypes.None)
                {
                    return true;
                }
                if (_triggerSettings.TriggerDamageTypes[i] == damageType)
                {
                    return true;
                }
            }

            LogProgressTriggerDamageTypeMismatch(damageType);
            return false;
        }

        public bool CheckTriggerEntityType(AttackEntity attackEntity)
        {
            if (_triggerSettings.TriggerEntityType == AttackEntityTypes.None)
            {
                // 검사하지 않음
                return true;
            }

            AttackEntityTypes entityType = AttackEntityTypes.None;
            if (attackEntity != null && attackEntity.AssetData.IsValid())
            {
                entityType = attackEntity.AssetData.EntityType;
            }

            if (_triggerSettings.TriggerEntityType == entityType)
            {
                return true;
            }

            LogProgressTriggerEntityTypeMismatch(entityType);
            return false;
        }

        public bool CheckTriggerSkillCategory(SkillNames skillName)
        {
            if (!_triggerSettings.TriggerSkillCategories.IsValidArray())
            {
                return true;
            }

            SkillAssetData skillData = ScriptableDataManager.Instance.FindSkillClone(skillName);
            if (!skillData.IsValid())
            {
                LogProgressInvalidTriggerSkill(skillName);
                return false;
            }

            if (_triggerSettings.TriggerSkillCategories.IsValidArray())
            {
                for (int i = 0; i < _triggerSettings.TriggerSkillCategories.Length; i++)
                {
                    if (_triggerSettings.TriggerSkillCategories[i] == skillData.Category)
                    {
                        return true;
                    }
                }
            }

            LogProgressTriggerSkillCategoryMismatch(skillName, skillData.Category);

            return false;
        }

        public bool CheckTriggerSkillElement(SkillNames skillName)
        {
            if (!_triggerSettings.TriggerSkillElements.IsValidArray())
            {
                return true;
            }

            SkillAssetData skillData = ScriptableDataManager.Instance.FindSkillClone(skillName);
            if (!skillData.IsValid())
            {
                LogProgressInvalidTriggerSkill(skillName);
                return false;
            }

            if (_triggerSettings.TriggerSkillElements.IsValidArray())
            {
                for (int i = 0; i < _triggerSettings.TriggerSkillElements.Length; i++)
                {
                    if (_triggerSettings.TriggerSkillElements[i] == skillData.Element)
                    {
                        return true;
                    }
                }
            }

            LogProgressTriggerSkillElementMismatch(skillName, skillData.Element);

            return false;
        }

        public bool CheckDifferentSkillCount()
        {
            // 서로 다른 속성 상태이상의 수를 검사합니다.
            if (_triggerSettings.DifferentElementalStateEffectCount > 0
                && _triggerSettings.DifferentElementalStateEffectOperator != PassiveOperator.None)
            {
                VCharacter characterInfo = GameApp.GetSelectedCharacter();
                int current = characterInfo.Skill.GetDifferentElementalAssignedSkillCount();
                int criteria = _triggerSettings.DifferentElementalStateEffectCount;

                if (_triggerSettings.DifferentElementalStateEffectOperator.Compare(current, criteria))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckTriggerHitmark(HitmarkNames hitmarkName)
        {
            if (!_triggerSettings.TriggerHitmarks.IsValidArray())
            {
                return true;
            }

            for (int i = 0; i < _triggerSettings.TriggerHitmarks.Length; i++)
            {
                if (_triggerSettings.TriggerHitmarks[i] == HitmarkNames.None)
                {
                    return true;
                }

                if (_triggerSettings.TriggerHitmarks[i] == hitmarkName)
                {
                    return true;
                }
            }

            LogProgressHitmarkMismatch(hitmarkName);
            return false;
        }

        public bool CheckTriggerIgnoreHitmarks(HitmarkNames hitmarkName)
        {
            if (!_triggerSettings.TriggerIgnoreHitmarks.IsValidArray())
            {
                return true;
            }

            for (int i = 0; i < _triggerSettings.TriggerIgnoreHitmarks.Length; i++)
            {
                if (_triggerSettings.TriggerIgnoreHitmarks[i] == HitmarkNames.None)
                {
                    continue;
                }

                if (_triggerSettings.TriggerIgnoreHitmarks[i] != hitmarkName)
                {
                    continue;
                }

                LogProgressIgnoreHitmarkMatch(hitmarkName);
                return false;
            }

            return true;
        }

        public bool CheckTriggerSkill(SkillNames skillName)
        {
            if (_triggerSettings.TriggerSkill == SkillNames.None)
            {
                return true;
            }

            if (_owner == null)
            {
                LogProgressOwnerNotFound();
                return false;
            }

            if (_triggerSettings.TriggerSkill != skillName)
            {
                LogProgressSkillMismatch(skillName);
                return false;
            }

            return true;
        }

        public bool CheckTriggerStat(StatNames statName)
        {
            if (!_triggerSettings.TriggerStats.IsValidArray())
            {
                return true;
            }

            for (int i = 0; i < _triggerSettings.TriggerStats.Length; i++)
            {
                if (_triggerSettings.TriggerStats[i] == statName)
                {
                    return true;
                }
            }

            LogProgressStatMismatch(statName);
            return false;
        }

        public bool CheckTriggerBuff(BuffNames buffName)
        {
            if (!_triggerSettings.TriggerBuffs.IsValidArray())
            {
                return true;
            }

            for (int i = 0; i < _triggerSettings.TriggerBuffs.Length; i++)
            {
                if (_triggerSettings.TriggerBuffs[i] == buffName)
                {
                    return true;
                }
            }

            LogProgressBuffMismatch(buffName);
            return false;
        }

        public bool CheckTriggerBuffType(BuffNames buffName)
        {
            if (_triggerSettings.TriggerBuffType == BuffTypes.None)
            {
                return true;
            }

            BuffAsset buffAsset = ScriptableDataManager.Instance.FindBuff(buffName);
            if (!buffAsset.IsValid())
            {
                LogProgressInvalidBuffAsset(buffName);
                return false;
            }

            if (_triggerSettings.TriggerBuffType != buffAsset.Data.Type)
            {
                LogProgressBuffTypeMismatch(buffName, buffAsset);
                return false;
            }

            return true;
        }

        public bool CheckContainsTriggerBuffType(Character targetCharacter, BuffNames buffName)
        {
            if (!_triggerSettings.TriggerStateEffects.IsValidArray())
            {
                return true;
            }

            if (targetCharacter == null)
            {
                LogProgressTargetCharacterNotFound();
                return false;
            }

            BuffAssetData buffData = ScriptableDataManager.Instance.FindBuffClone(buffName);
            if (buffData.IsValid())
            {
                if (!_triggerSettings.TriggerStateEffects.Contains(buffData.StateEffect))
                {
                    LogProgressBuffTypeNotContained(targetCharacter, buffName, buffData);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckContainsTriggerStateEffect(Character targetCharacter, StateEffects stateEffect)
        {
            if (!_triggerSettings.TriggerStateEffects.IsValidArray())
            {
                return true;
            }

            if (targetCharacter == null)
            {
                LogProgressTargetCharacterNotFound();
                return false;
            }
            if (!_triggerSettings.TriggerStateEffects.Contains(stateEffect))
            {
                LogProgressStateEffectNotContained(targetCharacter, stateEffect);
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckTriggerIgnoreBuffOnHit(Character targetCharacter, BuffAssetData buffAssetOnHit)
        {
            if (targetCharacter == null)
            {
                LogProgressTargetCharacterNotFound();
                return false;
            }

            if (_triggerSettings.TriggerIgnoreStateEffectOnHit != StateEffects.None)
            {
                if (buffAssetOnHit != null)
                {
                    if (_triggerSettings.TriggerIgnoreStateEffectOnHit == StateEffects.DamageOverTime)
                    {
                        switch (buffAssetOnHit.StateEffect)
                        {
                            case StateEffects.Burning:
                            case StateEffects.Jolted:
                            case StateEffects.Bleeding:
                            case StateEffects.Poisoning:
                                LogProgressIgnoreStateEffectOnHit(targetCharacter);
                                return false;
                        }
                    }
                    else if (_triggerSettings.TriggerIgnoreStateEffectOnHit == buffAssetOnHit.StateEffect)
                    {
                        LogProgressIgnoreStateEffectOnHit(targetCharacter);
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckTriggerIgnoreDamageType(DamageResult damageResult)
        {
            if (_triggerSettings.TriggerIgnoreDamageTypes.IsValidArray())
            {
                for (int i = 0; i < _triggerSettings.TriggerIgnoreDamageTypes.Length; i++)
                {
                    if (_triggerSettings.TriggerIgnoreDamageTypes[i] == damageResult.DamageType)
                    {
                        LogProgressIgnoreDamageType(damageResult);
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckTriggerPercent()
        {
            return CheckTriggerPercent(_triggerSettings.TriggerOperator, _triggerSettings.TriggerPercent, RandomEx.GetFloatValue());
        }

        public bool CheckTriggerPercent(PassiveOperator passiveOperator, float baseValue, float currentValue)
        {
            if (passiveOperator == PassiveOperator.None)
            {
                return true;
            }

            return passiveOperator.Compare(currentValue, baseValue);
        }

        public bool CheckTriggerMonsterRange()
        {
            if (_triggerSettings.TriggerMonsterRange > 0)
            {
                Vector3 ownerPosition = _owner.transform.position;

                Character[] monsters = CharacterManager.Instance.Monsters.ToArray();

                if (monsters.IsValid())
                {
                    for (int i = 0; i < monsters.Length; i++)
                    {
                        if (monsters[i].IsAlive)
                        {
                            if (Vector2.Distance(ownerPosition, monsters[i].position) < _triggerSettings.TriggerMonsterRange)
                            {
                                return true;
                            }
                        }
                    }
                }

                LogProgressMonsterRange();
                return false;
            }

            return true;
        }

        public bool CheckContainsTriggerMonsterCountStateEffects()
        {
            if (!_triggerSettings.TriggerMonsterCountStateEffects.IsValidArray())
            {
                return true;
            }
            if (_triggerSettings.TriggerMonsterCount <= 0)
            {
                return true;
            }

            Character[] monsters = CharacterManager.Instance.Monsters.ToArray();
            if (monsters.IsValid())
            {
                int containsCount = 0;
                for (int i = 0; i < monsters.Length; i++)
                {
                    if (!monsters[i].IsAlive) { continue; }
                    for (int j = 0; j < _triggerSettings.TriggerMonsterCountStateEffects.Length; j++)
                    {
                        StateEffects triggerMonsterCountStateEffect = _triggerSettings.TriggerMonsterCountStateEffects[j];
                        if (monsters[i].Buff.ContainsStateEffect(triggerMonsterCountStateEffect))
                        {
                            containsCount += 1;
                            break;
                        }
                    }
                }

                if (!CheckTriggerPercent(_triggerSettings.TriggerMonsterCountOperator, _triggerSettings.TriggerMonsterCount, containsCount))
                {
                    LogProgressContainsTriggerBuffTypeInMonsterCount(containsCount);
                    return false;
                }
            }

            return true;
        }

        public bool CheckTriggerStatOperator()
        {
            if (_triggerSettings.TriggerOperatorStats.IsValidArray())
            {
                for (int i = 0; i < _triggerSettings.TriggerOperatorStats.Length; i++)
                {
                    float value = _owner.Stat.FindValueOrDefault(_triggerSettings.TriggerOperatorStats[i]);

                    if (CheckTriggerPercent(_triggerSettings.TriggerStatOperator, _triggerSettings.TriggerStatValue, value))
                    {
                        return true;
                    }
                }

                LogProgressTriggerStatOperatorNotMet();
                return false;
            }

            return true;
        }

        public bool CheckTriggerMapObject(MapObjectNames name)
        {
            if (_triggerSettings.TriggerMapObjects.IsValidArray())
            {
                for (int i = 0; i < _triggerSettings.TriggerMapObjects.Length; i++)
                {
                    if (_triggerSettings.TriggerMapObjects[i] == name)
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        public bool CheckTriggerMapType(MapTypes mapType)
        {
            if (_triggerSettings.TriggerMapTypes.IsValidArray())
            {
                for (int i = 0; i < _triggerSettings.TriggerMapTypes.Length; i++)
                {
                    if (_triggerSettings.TriggerMapTypes[i] == mapType)
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        public bool CheckTriggerNPC(NPCNames name)
        {
            if (_triggerSettings.TriggerNPCs.IsValidArray())
            {
                for (int i = 0; i < _triggerSettings.TriggerNPCs.Length; i++)
                {
                    if (_triggerSettings.TriggerNPCs[i] == name)
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        public bool CheckTriggerCurrency(CurrencyNames currencyName)
        {
            if (_triggerSettings.TriggerCurrencies.IsValidArray())
            {
                for (int i = 0; i < _triggerSettings.TriggerCurrencies.Length; i++)
                {
                    if (_triggerSettings.TriggerCurrencies[i] == currencyName)
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        public bool CheckTriggerMaxStack(int stack, int maxStack)
        {
            if (_triggerSettings.TriggerMaxStack)
            {
                if (stack < maxStack)
                {
                    LogProgress("패시브를 발동할 수 없습니다. 버프의 최대 스택이 부족합니다. {0}/{1}", stack, maxStack);
                    return false;
                }
            }

            return true;
        }

        public bool CheckTriggerRestTime()
        {
            if (_triggerSettings.TriggerCheckPassiveRestTime)
            {
                // PassiveSystem의 RestTimeController를 통해 RestTime 상태 확인
                if (_owner.Passive.IsPassiveResting(_triggerSettings.Name))
                {
                    return false;
                }
            }

            return true;
        }
    }
}