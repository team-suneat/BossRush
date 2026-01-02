using TeamSuneat.Data;

namespace TeamSuneat.Passive
{
    public abstract partial class PassiveTriggerReceiver
    {
        protected Character Owner { get; set; }

        protected PassiveEntity Entity { get; set; }

        protected PassiveTriggerChecker Checker { get; set; }

        protected PassiveTrigger TriggerInfo { get; set; }

        protected abstract PassiveTriggers Trigger { get; }

        protected bool _isRegistered;

        protected PassiveTriggerSettings _triggerSettings;
        protected PassiveConditionSettings _conditionSettings;
        protected PassiveEffectSettings _effectSettings;

        public virtual void Initialize()
        {
            CreateChecker();
        }

        public void SetEntity(PassiveEntity passiveEntity)
        {
            Entity = passiveEntity;
            if (passiveEntity != null)
            {
                TriggerInfo = PassiveTrigger.Create(Entity.Name, Entity.Level);
                Owner = Entity.Owner;
                if (Owner == null)
                {
                    LogError("패시브 독립체의 캐릭터가 설정되지 않았습니다.");
                }
            }
            else
            {
                LogError("패시브 독립체가 설정되지 않았습니다.");
            }
        }

        public void SetPassiveAssetData(PassiveTriggerSettings triggerSettings, PassiveConditionSettings conditionSettings, PassiveEffectSettings effectSettings)
        {
            _triggerSettings = triggerSettings;
            _conditionSettings = conditionSettings;
            _effectSettings = effectSettings;

            if (triggerSettings != null)
            {
                LogProgress("패시브 독립체의 발동(Trigger) 에셋 데이터를 설정합니다.");
            }
            else
            {
                LogError("패시브 독립체의 발동(Trigger) 에셋 데이터가 설정되지 않았습니다.");
            }

            if (conditionSettings != null)
            {
                LogProgress("패시브 독립체의 조건(Condition) 에셋 데이터를 설정합니다.");
            }

            if (effectSettings != null)
            {
                LogProgress("패시브 독립체의 효과(Effect) 에셋 데이터가 설정합니다.");
            }
            else
            {
                LogError("패시브 독립체의 효과(Effect) 에셋 데이터가 설정되지 않았습니다.");
            }
        }

        public virtual void Activate()
        {
            if (!_isRegistered)
            {
                Register();
            }

            ActivateTriggerSetting();
        }

        private void ActivateTriggerSetting()
        {
            if (_triggerSettings != null)
            {
                if (_triggerSettings.TriggerCountDontSave)
                {
                    if (Entity != null)
                    {
                        Entity.ResetTriggerCount();
                    }
                }

                switch (_triggerSettings.InitTriggerCondition)
                {
                    case PassiveTriggerCondition.AttemptTrigger:
                        {
                            if (TryExecute())
                            {
                                Execute();
                            }
                        }
                        break;

                    case PassiveTriggerCondition.ForceTrigger:
                        {
                            Execute();
                        }
                        break;
                }
            }
        }

        public void Deactivate()
        {
            if (_isRegistered)
            {
                Unregister();
            }
        }

        private void CreateChecker()
        {
            Checker = new PassiveTriggerChecker();
            Checker.Setup(Owner, _triggerSettings, _conditionSettings);
        }

        protected virtual void Register()
        {
            LogRegister();

            OnRegister();

            _isRegistered = true;
        }

        protected virtual void OnRegister()
        {
        }

        protected virtual void Unregister()
        {
            LogUnregister();

            _isRegistered = false;
        }

        public virtual bool TryExecute()
        {
            if (!Owner.IsAlive)
            {
                LogProgressNotAlive();
                return false;
            }
            else if (Entity == null)
            {
                LogProgressNoEntity();
                return false;
            }
            else if (Checker == null)
            {
                LogProgressNoChecker();
                return false;
            }
            else if (!Entity.TryExecute(TriggerInfo))
            {
                return false;
            }
            else if (!Checker.CheckTriggerChance(Entity.Level))
            {
                return false;
            }

            return true;
        }

        public virtual bool TryExecute(DamageResult damageResult)
        {
            if (!TryExecute()) { return false; }
            if (!ValidateTriggerConditions(damageResult)) { return false; }
            if (!ValidateCharacterConditions(damageResult)) { return false; }
            if (!ValidateTriggerCount()) { return false; }

            return true;
        }

        private bool ValidateTriggerConditions(DamageResult damageResult)
        {
            if (_triggerSettings == null)
            {
                return true;
            }

            // 비용이 낮은 순서로 검사
            if (!Checker.CheckTriggerPercent()) { return false; }
            if (!Checker.CheckTriggerOwner(damageResult)) { return false; }
            if (!Checker.CheckTriggerPotion()) { return false; }
            if (!ValidateHitmarkConditions(damageResult)) { return false; }
            if (!ValidateSkillAndDamage(damageResult)) { return false; }

            return true;
        }

        private bool ValidateSkillAndDamage(DamageResult damageResult)
        {
            if (!Checker.CheckTriggerDamageType(damageResult.DamageType)) { return false; }
            if (!Checker.CheckTriggerEntityType(damageResult.AttackEntity)) { return false; }
            if (!Checker.CheckTriggerIgnoreDamageType(damageResult)) { return false; }
            if (!Checker.CheckTriggerSkillCategory(damageResult.Skill)) { return false; }
            if (!Checker.CheckTriggerSkillElement(damageResult.Skill)) { return false; }

            return true;
        }

        private bool ValidateHitmarkConditions(DamageResult damageResult)
        {
            if (!Checker.CheckTriggerHitmark(damageResult.HitmarkName)) { return false; }
            if (!Checker.CheckTriggerIgnoreHitmarks(damageResult.HitmarkName)) { return false; }
            if (!Checker.CheckTriggerIgnoreBuffOnHit(damageResult.TargetCharacter, damageResult.BuffAssetOnHit)) { return false; }

            return true;
        }

        private bool ValidateCharacterConditions(DamageResult damageResult)
        {
            if (_conditionSettings == null)
            {
                return true;
            }

            // 메인 캐릭터 조건 체크
            Character conditionCharacter = GetConditionCharacter(_conditionSettings.ConditionTarget, damageResult);
            if (!ValidateMainCharacterConditions(conditionCharacter))
            {
                return false;
            }

            // 무시 대상 조건 체크
            if (_conditionSettings.ConditionIgnoreTarget != PassiveTargetTypes.None)
            {
                Character ignoreCharacter = GetConditionCharacter(_conditionSettings.ConditionIgnoreTarget, damageResult);
                if (!ValidateIgnoreCharacterConditions(ignoreCharacter))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateMainCharacterConditions(Character character)
        {
            if (character == null)
            {
                return false;
            }

            return Checker.CheckConditionSkill(character)
                && Checker.CheckConditionIgnoreSkill(character)
                && ValidateCharacterStateAndResources(character);
        }

        private bool ValidateCharacterStateAndResources(Character character)
        {
            return Checker.CheckConditionSkillCooldown(character)
                && Checker.CheckConditionBuff(character)
                && Checker.CheckConditionBuffStack(character)
                && Checker.CheckConditionStateEffect(character)
                && Checker.CheckConditionVitalResource(character)
                && Checker.CheckConditionMonsterGrade(character)
                && Checker.CheckConditionRelic(character);
        }

        private bool ValidateIgnoreCharacterConditions(Character character)
        {
            if (character == null)
            {
                return false;
            }

            return Checker.CheckConditionIgnoreBuff(character)
                && Checker.CheckConditionIgnoreStateEffect(character);
        }

        private bool ValidateTriggerCount()
        {
            if (!Checker.CheckTriggerCount(Entity.TriggerCount))
            {
                Entity.AddTriggerCount();
                return false;
            }
            return true;
        }

        public bool CheckConditions(Character targetCharacter = null)
        {
            if (_conditionSettings != null)
            {
                Character conditionCharacter = GetConditionCharacter(_conditionSettings.ConditionTarget, targetCharacter);
                if (conditionCharacter != null)
                {
                    if (!Checker.CheckConditionSkill(conditionCharacter)) { return false; }
                    else if (!Checker.CheckConditionIgnoreSkill(conditionCharacter)) { return false; }
                    else if (!Checker.CheckConditionSkillCooldown(conditionCharacter)) { return false; }
                    else if (!Checker.CheckConditionBuff(conditionCharacter)) { return false; }
                    else if (!Checker.CheckConditionBuffStack(conditionCharacter)) { return false; }
                    else if (!Checker.CheckConditionStateEffect(conditionCharacter)) { return false; }
                    else if (!Checker.CheckConditionVitalResource(conditionCharacter)) { return false; }
                    else if (!Checker.CheckConditionMonsterGrade(conditionCharacter)) { return false; }
                    else if (!Checker.CheckConditionRelic(conditionCharacter)) { return false; }
                }

                if (_conditionSettings.ConditionIgnoreTarget != PassiveTargetTypes.None)
                {
                    Character ignoreCharacter = GetConditionCharacter(_conditionSettings.ConditionIgnoreTarget, targetCharacter);
                    if (ignoreCharacter != null)
                    {
                        if (!Checker.CheckConditionIgnoreBuff(ignoreCharacter)) { return false; }
                        if (!Checker.CheckConditionIgnoreStateEffect(ignoreCharacter)) { return false; }
                    }
                }
            }

            if (!Checker.CheckTriggerCount(Entity.TriggerCount))
            {
                Entity.AddTriggerCount();
                return false;
            }

            return true;
        }

        public void ExecuteWithDamage(DamageResult damageResult)
        {
            TriggerInfo.UpdateFromDamageResult(damageResult);
            Execute();
        }

        public virtual void Execute()
        {
            LogExecute();

            Entity.Execute(_effectSettings.ApplyDelayTime, TriggerInfo);
        }

        private Character GetConditionCharacter(PassiveTargetTypes targetType, DamageResult damageResult)
        {
            switch (targetType)
            {
                case PassiveTargetTypes.Owner:
                    return Owner;

                case PassiveTargetTypes.Attacker:
                    return damageResult.Attacker;

                case PassiveTargetTypes.Target:
                    return damageResult.TargetCharacter;

                default:
                    LogErrorConditionTargetNotSet();
                    return null;
            }
        }

        protected Character GetConditionCharacter(PassiveTargetTypes targetType, Character targetCharacter = null)
        {
            switch (targetType)
            {
                case PassiveTargetTypes.Owner:
                    return Owner;

                case PassiveTargetTypes.Target:
                    return targetCharacter;

                default:
                    LogErrorConditionTargetNotSet();
                    return null;
            }
        }
    }
}