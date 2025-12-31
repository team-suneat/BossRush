namespace TeamSuneat
{
    public partial class AttackEntity
    {
        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                if (Owner != null)
                {
                    Log.Progress(LogTags.Attack, StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                if (Owner != null)
                {
                    Log.Info(LogTags.Attack, StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                if (Owner != null)
                {
                    Log.Warning(LogTags.Attack, StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                if (Owner != null)
                {
                    Log.Error(StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format(format, args);
                LogProgress(content);
            }
        }

        protected void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string content = string.Format(format, args);
                LogInfo(content);
            }
        }

        protected void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string content = string.Format(format, args);
                LogWarning(content);
            }
        }

        protected void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string content = string.Format(format, args);
                LogError(content);
            }
        }
    }
}