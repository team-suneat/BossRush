using TeamSuneat.Feedbacks;
using Lean.Pool;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
            Attack = GetComponentInChildren<AttackEntity>();

            ActivateFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Activate");
            ApplyFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Apply");
            OverlapFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Overlap");
            DeactivateFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Deactivate");
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Name != 0)
            {
                NameString = Name.ToString();
            }
        }

        public override void AutoNaming()
        {
#if UNITY_EDITOR
            SetGameObjectName("BuffEntity(" + Name.ToString() + ")");
#endif
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        #region Log

        private string FormatEntityLog(string content)
        {
            return string.Format("{0}({1}), {2}, {3}", Owner.Name.ToLogString(), Owner.name, Name.ToLogString(), content);
        }

        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Buff, FormatEntityLog(content));
            }
        }

        protected virtual void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.Buff, formattedContent);
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Buff, FormatEntityLog(content));
            }
        }

        protected virtual void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.Buff, formattedContent);
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Buff, FormatEntityLog(content));
            }
        }

        protected virtual void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.Buff, formattedContent);
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.Buff, FormatEntityLog(content));
            }
        }

        protected virtual void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Error(LogTags.Buff, formattedContent);
            }
        }

        #endregion Log
    }
}