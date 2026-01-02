using TeamSuneat.Data;

namespace TeamSuneat
{
    public class CharacterAnimatorLog
    {
        private Character _ownerCharacter;

        public CharacterAnimatorLog(Character owner)
        {
            _ownerCharacter = owner;
        }

        private string FormatEntityLog(string content)
        {
            return string.Format("{0}, {1}", _ownerCharacter.Name.ToLogString(), content);
        }

        public void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Animation, FormatEntityLog(content));
            }
        }

        public void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.Animation, formattedContent);
            }
        }

        public void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Animation, FormatEntityLog(content));
            }
        }

        public void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.Animation, formattedContent);
            }
        }

        public void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Animation, FormatEntityLog(content));
            }
        }

        public void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.Animation, formattedContent);
            }
        }

        public void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.Animation, FormatEntityLog(content));
            }
        }

        public void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Error(LogTags.Animation, formattedContent);
            }
        }

        //

        public void LogSkillInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Animation, FormatEntityLog(content));
            }
        }

        public void LogSkillInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.Skill_Animation, formattedContent);
            }
        }

        public void LogSkillWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill_Animation, FormatEntityLog(content));
            }
        }

        public void LogSkillWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.Skill_Animation, formattedContent);
            }
        }

        //

        public void LogSkillAnimationNameNotSet(SkillAnimationAsset skillAnimation)
        {
            if (Log.LevelWarning)
            {
                LogSkillWarning("기술 애니메이션의 이름이 설정되지 않았습니다. SkillName: {0}", skillAnimation.SkillName.ToLogString());
            }
        }

        public void LogSkillAnimationRegistered(SkillAnimationAsset skillAnimation)
        {
            if (Log.LevelInfo)
            {
                LogSkillInfo("기술의 애니메이션을 등록합니다. 애니메이션: {0}, 기술: {1}", skillAnimation.AnimationName, skillAnimation.SkillName.ToLogString());
            }
        }

        public void LogSkillAnimationAlreadyRegistered(SkillAnimationAsset skillAnimation)
        {
            if (Log.LevelWarning)
            {
                LogSkillWarning("이미 등록된 기술 애니메이션입니다. 애니메이션: {0}, 기술: {1}", skillAnimation.AnimationName, skillAnimation.SkillName.ToLogString());
            }
        }

        public void LogSkillAnimationUnregistered(SkillAnimationAsset skillAnimation)
        {
            if (Log.LevelInfo)
            {
                LogSkillInfo("기술의 애니메이션을 등록 해제합니다. 애니메이션: {0}, 기술: {1}", skillAnimation.AnimationName, skillAnimation.SkillName.ToLogString());
            }
        }

        // 재생 중인 기술 애니메이션

        public void LogPlaySkillAnimation(SkillAnimationAsset skillAnimation)
        {
            if (Log.LevelInfo)
            {
                LogSkillInfo("기술({0})의 애니메이션({1})을 재생합니다.", skillAnimation.SkillName.ToLogString(), skillAnimation.AnimationName);
            }
        }

        public void LogCannotPlaySequenceSkillAnimation(string parameterName)
        {
            if (Log.LevelWarning)
            {
                LogSkillWarning("순서를 가진 애니메이션을 재생할 수 없습니다. {0} 파라메터를 찾을 수 없습니다.", parameterName);
            }
        }

        public void LogStopSequenceSkillAnimation(string parameterName)
        {
            if (Log.LevelProgress)
            {
                Log.Progress("순서를 가진 애니메이션을 정지합니다. 파라메터: {0}.", parameterName);
            }
        }

        public void LogCannotStopSequenceSkillAnimation(string parameterName)
        {
            if (Log.LevelWarning)
            {
                LogSkillWarning("순서를 가진 애니메이션을 정지할 수 없습니다. ({0}) 파라메터를 찾을 수 없습니다.", parameterName);
            }
        }

        // 기술 애니메이션 재생 및 정지

        public void LogSetPlayingSkillAnimationName(string animationName)
        {
            if (Log.LevelInfo)
            {
                LogSkillInfo("재생 중인 기술 애니메이션 이름을 설정합니다. ({0})", animationName.ToSelectString());
            }
        }

        public void LogResetPlayingSkillAnimationName(string animationName)
        {
            if (Log.LevelInfo)
            {
                LogSkillInfo("재생 중인 기술 애니메이션 이름을 초기화합니다. ({0})", animationName.ToErrorString());
            }
        }

        // Animator State

        public void LogNotASkillAnimation(string playingAnimationName, string stopAnimationName)
        {
            if (Log.LevelInfo)
            {
                LogSkillInfo("이 애니메이션은 기술 애니메이션이 아닙니다. 재생중인 애니메이션: ({0}), 정지 애니메이션: ({1})", playingAnimationName, stopAnimationName);
            }
        }

        public void LogSkillAnimationStart(string animationName, float length)
        {
            if (Log.LevelInfo)
            {
                LogSkillInfo("시작된 기술 애니메이션:({0}), 애니메이션 길이: ({1})", animationName, length);
            }
        }

        public void LogCannotForceStopSequenceSkillAnimation(string stopAnimationName, int skillProgress)
        {
            if (Log.LevelWarning)
            {
                LogSkillWarning("순서를 가진 기술 애니메이션({0})을 시작 또는 종료 상태에서 강제 종료할 수 없습니다. 현재 순서: {1}", stopAnimationName, skillProgress);
            }
        }

        public void LogSkillAnimationExit(string animationName, string entityName)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Skill_Animation, "기술 애니메이션({0})이 종료되었습니다. 기술 독립체({1})의 OnExitSkillAnimationState를 호출합니다.", animationName, entityName);
            }
        }

        public void LogForceStopSkillAnimation(string playingAnimationName)
        {
            if (Log.LevelInfo)
            {
                LogSkillInfo("재생중인 기술 애니메이션({0})을 강제종료합니다.", playingAnimationName);
            }
        }

        //

        public void LogFailedToFindCurrentAnimationName(string currentAnimationName)
        {
            if (Log.LevelError)
            {
                Log.Error($"_skillAnimations에서 애니메이션 이름({currentAnimationName})을 찾을 수 없습니다.");
            }
        }

        // Player

        public void LogForceStopSkillAnimationByAttack(SkillAnimationAsset playingAnimation, HitmarkAssetData damageAssetData)
        {
            if (Log.LevelInfo)
            {
                LogSkillInfo("공격의 우선순위가 재생중인 기술 애니메이션보다 높을 때 기술 애니메이션을 강제로 피격 애니메이션을 재생합니다. 재생중인 기술({0})의 애니메이션 우선순위: {1}",
                    playingAnimation.AnimationName, playingAnimation.Priority);
            }
        }
    }
}