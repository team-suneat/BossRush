using System.Collections.Generic;
using Lean.Pool;
using TeamSuneat.Data;


namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        // 패시브의 쿨다운 감소 기능

        private void ReduceCooldown(PassiveEffectSettings effectSetting, PassiveTrigger triggerInfo)
        {
            if (effectSetting.ReduceCooldown.IsValid())
            {
                for (int i = 0; i < effectSetting.ReduceCooldown.Length; i++)
                {
                    var cooldownData = effectSetting.ReduceCooldown[i];
                    if (cooldownData.ReduceOnlyTriggerSkill)
                    {
                        SkillEntity skillEntity = Owner.Skill.Find(triggerInfo.SkillName);
                        if (skillEntity == null)
                        {
                            LogWarning("스킬({0})의 엔티티를 찾을 수 없습니다. 패시브의 쿨다운 감소를 적용할 수 없습니다.", triggerInfo.SkillName);
                        }
                        else
                        {
                            ReduceCooldown(skillEntity, cooldownData);
                            AppliedEffects |= AppliedEffects.Cooldown; // 쿨다운 감소 완료
                        }
                    }

                    if (cooldownData.ReduceCooldownSkill != SkillNames.None)
                    {
                        SkillEntity skillEntity = Owner.Skill.Find(cooldownData.ReduceCooldownSkill);
                        if (skillEntity == null)
                        {
                            LogWarning("스킬({0})의 엔티티를 찾을 수 없습니다. 패시브의 쿨다운 감소를 적용할 수 없습니다.", cooldownData.ReduceCooldownSkill);
                        }
                        else
                        {
                            ReduceCooldown(skillEntity, cooldownData);
                            AppliedEffects |= AppliedEffects.Cooldown; // 쿨다운 감소 완료
                        }
                    }

                    if (cooldownData.ReduceCooldownSkillCategory != SkillCategories.None)
                    {
                        List<SkillEntity> skillEntities = Owner.Skill.FindAll(cooldownData.ReduceCooldownSkillCategory);
                        if (skillEntities.IsValid())
                        {
                            for (int j = 0; j < skillEntities.Count; j++)
                            {
                                if (skillEntities[j] == null)
                                {
                                    LogWarning("카테고리 내 스킬 엔티티를 찾을 수 없습니다. 패시브의 쿨다운 감소를 적용할 수 없습니다.");
                                }
                                else
                                {
                                    ReduceCooldown(skillEntities[j], cooldownData);
                                    AppliedEffects |= AppliedEffects.Cooldown; // 쿨다운 감소 완료
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ReduceCooldown(SkillEntity skillEntity, PassiveReduceCooldown reduceCooldown)
        {
            if (skillEntity.Level == 0)
            {
                // LogProgress("스킬 레벨이 0이어서 쿨다운 감소를 적용하지 않습니다.");
                return;
            }

            if (!skillEntity.IsAssigned)
            {
                // LogProgress("할당되지 않은 스킬이어서 쿨다운 감소를 적용하지 않습니다.");
                return;
            }

            if (reduceCooldown.ReduceCooldownRate > 0)
            {
                float reduceCooldownRate = TSStatEx.GetValueByLevel(reduceCooldown.ReduceCooldownRate, reduceCooldown.ReduceCooldownRateByLevel, Level);
                skillEntity.ReduceCooldownTimeRate(reduceCooldownRate);

                if (Log.LevelProgress)
                {
                    LogProgress("스킬({0})의 쿨다운이 {1}만큼 감소합니다.", skillEntity.Name.ToLogString(), ValueStringEx.GetPercentString(reduceCooldownRate));
                }
            }

            if (reduceCooldown.ReduceCooldownTime > 0)
            {
                float reduceCooldownTime = TSStatEx.GetValueByLevel(reduceCooldown.ReduceCooldownTime, reduceCooldown.ReduceCooldownTimeByLevel, Level);
                skillEntity.ReduceCooldownTime(reduceCooldownTime);

                if (Log.LevelProgress)
                {
                    LogProgress("스킬({0})의 쿨다운이 {1}초 감소합니다.", skillEntity.Name.ToLogString(), ValueStringEx.GetValueString(reduceCooldownTime));
                }
            }
        }
    }
}