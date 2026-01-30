namespace TeamSuneat
{
    public class CharacterAnimatorLog
    {
        private Character _ownerCharacter;

        public CharacterAnimatorLog(Character owner)
        {
            _ownerCharacter = owner;
        }

        private string LogFormat(string content)
        {
            return string.Format("{0}, {1}", _ownerCharacter.Name.ToLogString(), content);
        }

        public void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Animation, LogFormat(content));
            }
        }

        public void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = LogFormat(string.Format(format, args));
                Log.Progress(LogTags.Animation, formattedContent);
            }
        }

        public void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Animation, LogFormat(content));
            }
        }

        public void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = LogFormat(string.Format(format, args));
                Log.Info(LogTags.Animation, formattedContent);
            }
        }

        public void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Animation, LogFormat(content));
            }
        }

        public void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = LogFormat(string.Format(format, args));
                Log.Warning(LogTags.Animation, formattedContent);
            }
        }

        public void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.Animation, LogFormat(content));
            }
        }

        public void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = LogFormat(string.Format(format, args));
                Log.Error(LogTags.Animation, formattedContent);
            }
        }
    }
}