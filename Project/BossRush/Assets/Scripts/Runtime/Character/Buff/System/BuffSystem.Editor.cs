using System.Linq;
using System.Text;
using UnityEngine;

namespace TeamSuneat
{
    public partial class BuffSystem : XBehaviour
    {
        #region Editor

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
        }

        public override void AutoNaming()
        {
            SetGameObjectName(string.Format("#Buff({0})", Owner.Name));
        }

        #endregion Editor

        #region Gizmo

#if UNITY_EDITOR

        private BuffEntity[] entitiesForGizmo;
        private StringBuilder stringBuilder = new StringBuilder();

        private void OnDrawGizmosSelected()
        {
            entitiesForGizmo = _entities.Values.ToArray();
            stringBuilder.Clear();

            foreach (BuffEntity entity in entitiesForGizmo)
            {
                stringBuilder.AppendFormat("{0}(Lv.{1}, Stack.{2})", entity.Name.GetLocalizedString(), entity.Level, entity.Stack);
                stringBuilder.AppendLine();
            }

            GizmoEx.DrawText(stringBuilder.ToString(), position + Vector3.down);
        }

#endif

        #endregion Gizmo

        #region Log

        private string FormatEntityLog(string content)
        {
            return string.Format("[System] {0}, {1}", Owner.Name.ToLogString(), content);
        }

        protected void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Buff, FormatEntityLog(content));
            }
        }

        protected void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.Buff, formattedContent);
            }
        }

        protected void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Buff, FormatEntityLog(content));
            }
        }

        protected void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.Buff, formattedContent);
            }
        }

        protected void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Buff, FormatEntityLog(content));
            }
        }

        protected void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.Buff, formattedContent);
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

        #endregion Log
    }
}