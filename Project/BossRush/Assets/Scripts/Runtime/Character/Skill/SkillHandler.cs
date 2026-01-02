using System.Text;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public class SkillHandler
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        public float GetCooldownTime(SkillAssetData skillData, int level)
        {
            Character playerCharacter = CharacterManager.Instance.Player;
            if (playerCharacter == null)
            {
                return 0f;
            }

            float baseCooldownTime = skillData.Cooldown;
            if (baseCooldownTime.IsZero())
            {
                // 기본 재사용 대기 시간이 0이라면 재사용 대기 시간 감소를 적용하지 않습니다.
                return 0f;
            }
            else
            {
                _stringBuilder.AppendLine($"기술의 레벨에 따른 재사용 대기시간({baseCooldownTime})을 계산합니다.");
            }

            // 재사용 대기 시간 감소를 적용합니다.

            _stringBuilder.Clear();

            float multiplier = 0f;
            float fixedValue = 0;
            float statValue = 0;

            StatData cooldownReductionData = JsonDataManager.FindStatDataClone(StatNames.CooldownTimeReductionRate);
            statValue = playerCharacter.Stat.FindValueOrDefault(StatNames.CooldownTimeReductionRate);

            if (!statValue.IsZero())
            {
                multiplier += statValue;
                _stringBuilder.AppendLine($"재사용 대기 시간 감소 능력치를 적용합니다. {ValueStringEx.GetPercentString(statValue, true)}");
            }
            if (skillData.Tag == SkillTags.Dash)
            {
                statValue = playerCharacter.Stat.FindValueOrDefault(StatNames.DashCooldownTimeReductionRate);
                if (!statValue.IsZero())
                {
                    multiplier += statValue;
                    _stringBuilder.AppendLine($"대시 기술의 재사용 대기 시간 감소 능력치를 적용합니다. {ValueStringEx.GetPercentString(statValue, true)}");
                }

                fixedValue = playerCharacter.Stat.FindValueOrDefault(StatNames.DashCooldownTime);
                if (!statValue.IsZero())
                {
                    fixedValue += statValue;
                    _stringBuilder.AppendLine($"대시 기술의 고정 재사용 대기 시간 감소 능력치를 적용합니다. {ValueStringEx.GetValueString(statValue, true)}");
                }
            }
            if (skillData.Category == SkillCategories.Power)
            {
                statValue = playerCharacter.Stat.FindValueOrDefault(StatNames.PowerSkillCooldownTimeReductionRate);
                if (!statValue.IsZero())
                {
                    multiplier += statValue;
                    _stringBuilder.AppendLine($"강한 기술의 재사용 대기 시간 감소 능력치를 적용합니다. {ValueStringEx.GetPercentString(statValue, true)}");
                }
            }
            else if (skillData.Category == SkillCategories.Ultimate)
            {
                statValue = playerCharacter.Stat.FindValueOrDefault(StatNames.UltimateSkillCooldownTimeReductionRate);
                if (!statValue.IsZero())
                {
                    multiplier += statValue;
                    _stringBuilder.AppendLine($"궁극 기술의 재사용 대기 시간 감소 능력치를 적용합니다. {ValueStringEx.GetPercentString(statValue, true)}");
                }
            }

            multiplier = Mathf.Clamp(multiplier, cooldownReductionData.MinValue, cooldownReductionData.MaxValue);
            float result = baseCooldownTime - (baseCooldownTime * multiplier) + fixedValue;

            if (_stringBuilder.Length == 0)
            {
                _stringBuilder.Insert(0, $"최종 재사용 대기시간({ValueStringEx.GetValueString(result, true)})을 설정합니다.");
            }
            else
            {
                _stringBuilder.Insert(0, $"최종 재사용 대기시간({ValueStringEx.GetValueString(result, true)})을 설정합니다.\n");
            }

            LogProgress(skillData.Name, _stringBuilder.ToString());

            return result;
        }

        protected void LogProgress(SkillNames skillName, string content)
        {
            if (Log.LevelProgress)
            {
                string addString = $"{skillName.ToLogString()}, ";
                Log.Progress(LogTags.Skill, addString + content);
            }
        }

        protected void LogProgress(SkillNames skillName, string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string addString = $"{skillName.ToLogString()}, ";
                Log.Progress(LogTags.Skill, addString + format, args);
            }
        }
    }
}