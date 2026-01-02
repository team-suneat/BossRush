using System.Linq;
using Lean.Pool;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        private void RegisterReplacedAnimationSkill()
        {
            if (HasSkillCondition())
            {
                GlobalEvent<SkillNames, int>.Register(GlobalEventType.GAME_DATA_ADD_CHARACTER_SKILL, OnPlayerSkillAdded);
                GlobalEvent<SkillNames>.Register(GlobalEventType.GAME_DATA_REMOVE_CHARACTER_SKILL, OnPlayerSkillRemoved);
            }
        }

        private void UnregisterReplacedAnimationSkill()
        {
            if (HasSkillCondition())
            {
                GlobalEvent<SkillNames, int>.Unregister(GlobalEventType.GAME_DATA_ADD_CHARACTER_SKILL, OnPlayerSkillAdded);
                GlobalEvent<SkillNames>.Unregister(GlobalEventType.GAME_DATA_REMOVE_CHARACTER_SKILL, OnPlayerSkillRemoved);
            }
        }

        private void OnPlayerSkillAdded(SkillNames skillName, int skillLevel)
        {
            ProcessSkillEvent(skillName);
        }

        private void OnPlayerSkillRemoved(SkillNames skillName)
        {
            ProcessSkillEvent(skillName);
        }

        private void ProcessSkillEvent(SkillNames skillName)
        {
            if (HasSkillCondition(skillName))
            {
                SetupUsableAnimationAssets();
                SetOrderRange();
            }
        }

        private void OnRefreshReplacedAnimationStat(StatNames statName, float statValue)
        {
            if (HasStatCondition(statName))
            {
                SetupUsableAnimationAssets();
                SetOrderRange();
            }
        }

        private bool HasSkillCondition()
        {
            if (_useReplaceAnimation)
            {
                return _animationAssets.Any(asset => asset.ConditionSkillName != SkillNames.None);
            }

            return false;
        }

        private bool HasSkillCondition(SkillNames skillName)
        {
            if (_useReplaceAnimation)
            {
                return _animationAssets.Any(asset => asset.ConditionSkillName == skillName);
            }

            return false;
        }

        private bool HasStatCondition(StatNames statName)
        {
            return _animationAssets.Any(asset => asset.ConditionStatNames.Contains(statName));
        }
    }
}