

namespace TeamSuneat
{
    public partial class AttackEntity : XBehaviour
    {
        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                if (Owner != null)
                {
                    Log.Progress(LogTags.Attack, TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Progress(LogTags.Attack, TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                if (Owner != null)
                {
                    Log.Info(LogTags.Attack, TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Info(LogTags.Attack, TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                if (Owner != null)
                {
                    Log.Warning(LogTags.Attack, TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Warning(LogTags.Attack, TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                if (Owner != null)
                {
                    Log.Error(TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Error(TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
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