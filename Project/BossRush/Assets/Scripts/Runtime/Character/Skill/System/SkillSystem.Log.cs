

namespace TeamSuneat
{
    public partial class SkillSystem : XBehaviour
    {
        private string FormatEntityLog(string content)
        {
            return string.Format("[System] {0}, {1}", Owner.Name.ToLogString(), content);
        }

        protected void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Skill, FormatEntityLog(content));
            }
        }

        protected void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.Skill, formattedContent);
            }
        }

        protected void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill, FormatEntityLog(content));
            }
        }

        protected void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.Skill, formattedContent);
            }
        }

        protected void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill, FormatEntityLog(content));
            }
        }

        protected void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.Skill, formattedContent);
            }
        }

        protected void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(FormatEntityLog(content));
            }
        }

        protected void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Error(formattedContent);
            }
        }
    }
}