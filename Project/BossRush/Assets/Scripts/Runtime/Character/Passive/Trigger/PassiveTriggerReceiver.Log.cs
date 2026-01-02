

namespace TeamSuneat.Passive
{
    public abstract partial class PassiveTriggerReceiver
    {
        private void LogRegister()
        {
            if (Log.LevelInfo)
            {
                LogInfo("패시브 트리거가 등록되었습니다. {0}", Trigger.ToLogString());
            }
        }

        private void LogUnregister()
        {
            if (Log.LevelInfo)
            {
                LogInfo("패시브 트리거가 해제되었습니다. 트리거: {0}", Trigger.ToLogString());
            }
        }

        private void LogProgressNotAlive()
        {
            if (Log.LevelProgress)
            {
                LogProgress("소유한 패시브 엔티티가 캐릭터({0})에 존재하지 않습니다. 트리거: {1}, 패시브: {2}", Owner.GetHierarchyName(), Trigger.ToLogString(), Entity.GetHierarchyPath());
            }
        }

        private void LogProgressNoEntity()
        {
            if (Log.LevelError)
            {
                LogError("소유한 패시브 엔티티가 존재하지 않습니다. 트리거: {0}", Trigger.ToLogString());
            }
        }

        private void LogProgressNoChecker()
        {
            if (Log.LevelError)
            {
                LogError("소유한 패시브 엔티티 체커가 존재하지 않습니다. 트리거: {0}", Trigger.ToLogString());
            }
        }

        private void LogExecute()
        {
            if (Log.LevelInfo)
            {
                if (TriggerInfo.SkillName != SkillNames.None)
                {
                    LogInfo("패시브 트리거가 실행됩니다. 트리거: {0}, 스킬: {1}", Trigger.ToLogString(), TriggerInfo.SkillName);
                }
                else if (TriggerInfo.BuffName != BuffNames.None)
                {
                    LogInfo("패시브 트리거가 실행됩니다. 트리거: {0}, 버프: {1}", Trigger.ToLogString(), TriggerInfo.BuffName);
                }
            }
        }

        private void LogErrorConditionTargetNotSet()
        {
            if (Log.LevelError)
            {
                Log.Error($"패시브({_triggerSettings.Name.ToLogString()})의 조건 타겟({_conditionSettings.ConditionTarget})이 설정되지 않았습니다.");
            }
        }

        #region Log

        private string FormatEntityLog(string content)
        {
            return string.Format("{0}, {1}, {2}", Owner.Name.ToLogString(), Entity.Name.ToLogString(), content);
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

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.PassiveTrigger, FormatEntityLog(content));
            }
        }

        protected virtual void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.PassiveTrigger, formattedContent);
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.PassiveTrigger, FormatEntityLog(content));
            }
        }

        protected virtual void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.PassiveTrigger, formattedContent);
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.PassiveTrigger, FormatEntityLog(content));
            }
        }

        protected virtual void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Error(LogTags.PassiveTrigger, formattedContent);
            }
        }

        #endregion Log
    }
}