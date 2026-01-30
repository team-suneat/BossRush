namespace TeamSuneat
{
    public partial class Character
    {
        private string LogFormat(string content)
        {
            return string.Format("{0}, {1}", Name.ToLogString(), content);
        }

        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTag, LogFormat(content));
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTag, LogFormat(content));
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTag, LogFormat(content));
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTag, LogFormat(content));
            }
        }

        protected virtual void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format(format, args);
                Log.Progress(LogTag, LogFormat(content));
            }
        }

        protected virtual void LogInfo(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format(format, args);
                Log.Progress(LogTag, LogFormat(content));
            }
        }

        protected virtual void LogWarning(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format(format, args);
                Log.Progress(LogTag, LogFormat(content));
            }
        }

        protected virtual void LogError(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format(format, args);
                Log.Progress(LogTag, LogFormat(content));
            }
        }
    }
}