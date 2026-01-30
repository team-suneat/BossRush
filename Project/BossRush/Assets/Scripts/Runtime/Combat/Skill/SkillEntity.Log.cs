namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour
    {
        private string LogFormat(string content)
        {
            if (Owner != null)
            {
                return StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content);
            }

            return StringGetter.ConcatStringWithComma(Name.ToLogString(), content);
        }

        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Skill, LogFormat(content));
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill, LogFormat(content));
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill, LogFormat(content));
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.Skill, LogFormat(content));
            }
        }

        protected void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = LogFormat(string.Format(format, args));
                Log.Progress(LogTags.Skill, formattedContent);
            }
        }

        protected void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = LogFormat(string.Format(format, args));
                Log.Info(LogTags.Skill, formattedContent);
            }
        }

        protected void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = LogFormat(string.Format(format, args));
                Log.Warning(LogTags.Skill, formattedContent);
            }
        }

        protected void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = LogFormat(string.Format(format, args));
                Log.Error(LogTags.Skill, formattedContent);
            }
        }
    }
}
