using Lean.Pool;


namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        #region Log

        private string FormatEntityLog(string content)
        {
            return string.Format("[Entity] {0}, {1}", Name.ToLogString(), content);
        }

        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Skill, FormatEntityLog(content));
            }
        }

        protected virtual void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.Skill, formattedContent);
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill, FormatEntityLog(content));
            }
        }

        protected virtual void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.Skill, formattedContent);
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill, FormatEntityLog(content));
            }
        }

        protected virtual void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.Skill, formattedContent);
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.Skill, FormatEntityLog(content));
            }
        }

        protected virtual void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Error(LogTags.Skill, formattedContent);
            }
        }

        #endregion Log
    }
}