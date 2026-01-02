using Lean.Pool;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        private bool _isRegisterRefreshStat;

        private void RegisterRefreshStatEvent()
        {
            if (_isRegisterRefreshStat)
            {
                LogWarning("능력치 갱신 이벤트를 등록하지 않습니다. 이미 등록되어있습니다.");
                return;
            }

            if (Owner != null)
            {
                Owner.Stat.RegisterRefreshedEvent(OnRefreshedStat);

                _isRegisterRefreshStat = true;
            }
        }

        private void UnregisterRefreshStatEvent()
        {
            if (Owner != null)
            {
                Owner.Stat.UnregisterRefreshedEvent(OnRefreshedStat);

                _isRegisterRefreshStat = false;
            }
        }

        // 능력치 갱신 이벤트 (Refresh Stat Event)

        private void OnRefreshedStat(StatNames statName, float addStatValue)
        {
            switch (statName)
            {
                case StatNames.AddAllSkillLevel:            // 모든 기술 레벨
                case StatNames.AddAssignSkillLevel:         // 할당된 기술 레벨
                case StatNames.AddAwardedSkillLevel:        // 포인트를 부여한 기술 레벨
                case StatNames.MaxBasicSkillLevel:          // 기본 기술의 최대 레벨
                case StatNames.MaxCoreSkillLevel:           // 핵심 기술의 최대 레벨
                case StatNames.MaxAssistantSkillLevel:      // 보조 기술의 최대 레벨
                case StatNames.MaxPowerSkillLevel:          // 숙련 기술의 최대 레벨
                case StatNames.MaxUltimateSkillLevel:       // 궁극 기술의 최대 레벨
                case StatNames.AddBasicSkillLevel:          // 기본 기술의 추가 레벨
                case StatNames.AddCoreSkillLevel:           // 핵심 기술의 추가 레벨
                case StatNames.AddAssistantSkillLevel:      // 보조 기술의 추가 레벨
                case StatNames.AddPowerSkillLevel:          // 숙련 기술의 추가 레벨
                case StatNames.AddUltimateSkillLevel:       // 궁극 기술의 추가 레벨
                    {
                        VSkill skillInfo = CharacterInfo.Skill.Find(Name);
                        if (skillInfo != null)
                        {
                            int totalLevel = skillInfo.GetTotalLevel();
                            ApplyAdditionalLevelByStat(totalLevel);
                        }
                        else
                        {
                            Log.Warning("기술의 세이브데이터를 찾을 수 없습니다. 능력치 갱신에 따라 기술의 추가 레벨을 획득하지 못합니다. 기술:{0}, 능력치:{1}", Name, statName);
                        }
                    }
                    break;

                // 기술 재사용 대기시간 설정

                case StatNames.CooldownTimeReductionRate:
                case StatNames.DashCooldownTimeReductionRate:
                case StatNames.DashCooldownTime:
                case StatNames.PowerSkillCooldownTimeReductionRate:
                case StatNames.UltimateSkillCooldownTimeReductionRate:
                    {
                        SetupCooldownTime();
                    }
                    break;

                // 기술 횟수

                case StatNames.DashSkillCount:
                    {
                        if (_skillData.Category == SkillCategories.Dash)
                        {
                            RefreshSkillMaxCount();
                            RefillSkillCount();
                        }
                    }
                    break;

                case StatNames.FrostArrowSkillCount:
                    {
                        if (Name == SkillNames.FrostArrow)
                        {
                            if (addStatValue > 0)
                            {
                                AddSkillCount();
                            }                            
                        }
                    }
                    break;

                case StatNames.FrostArrowSkillMaxCount:
                    {
                        if (Name == SkillNames.FrostArrow)
                        {
                            int previousCount = _skillCount;
                            RefreshSkillMaxCount();

                            int count = UnityEngine.Mathf.Min(previousCount, _skillMaxCount);
                            SetSkillCount(count);
                        }
                    }
                    break;

                // 시전자 자원

                case StatNames.Resource:
                case StatNames.ResourceRate:
                case StatNames.SkillResourceCostRate: // 기술 자원 소모량
                case StatNames.FixedSkillResourceCost: // 고정 기술 자원 소모량
                case StatNames.FixedBasicSkillResourceCost: // 기본 기술 자원 소모량
                case StatNames.CoreSkillResourceCostRate: // 핵심 기술 자원 소모량
                case StatNames.NoResourceCostForCoreSkill: // 핵심 기술 자원 소모하지 않음
                case StatNames.PowerSkillResourceCostPerCooldown: // 재사용 대기시간당 숙련기술 자원 소모량
                case StatNames.FixedLeapSlashResourceCost:  // 마검사 기술 [도약 베기] 자원 소모량
                case StatNames.ResourceCostForWolfBrawl: // 사냥꾼 기술 [늑대 난투] 자원 소모량
                    {
                        RefreshResourceCost();
                    }
                    break;
            }

            if (_useReplaceAnimation)
            {
                OnRefreshReplacedAnimationStat(statName, addStatValue);
            }
        }
    }
}