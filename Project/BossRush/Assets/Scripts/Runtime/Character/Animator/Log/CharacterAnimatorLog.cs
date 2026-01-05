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
    }
}