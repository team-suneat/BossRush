using TeamSuneat.Data;


namespace TeamSuneat.Passive
{
    public partial class PassiveSystem : XBehaviour
    {
        #region Log

        private string FormatEntityLog(string content)
        {
            if (Owner != null)
            {
                return string.Format("[System] {0}, {1}", Owner.Name.ToLogString(), content);
            }
            else
            {
                return string.Format("[System] {0}", content);
            }
        }

        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Passive, FormatEntityLog(content));
            }
        }

        protected virtual void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.Passive, formattedContent);
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Passive, FormatEntityLog(content));
            }
        }

        protected virtual void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.Passive, formattedContent);
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Passive, FormatEntityLog(content));
            }
        }

        protected virtual void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.Passive, formattedContent);
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.Passive, FormatEntityLog(content));
            }
        }

        protected virtual void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Error(LogTags.Passive, formattedContent);
            }
        }

        #endregion Log

        /// <summary>
        /// 패시브 독립체 생성 및 등록 로그
        /// </summary>
        private void LogCreateOrRegister(PassiveAsset passiveAsset, int currentLevel, int newLevel)
        {
            if (Log.LevelInfo)
            {
                LogInfo("{0}({1}) 패시브 독립체를 생성하고 등록합니다. (레벨: {2})",
                passiveAsset.Name.ToLogString(), passiveAsset.Skill.ToLogString(), currentLevel, newLevel);
            }
        }

        /// <summary>
        /// 이미 등록된 패시브 독립체 로그
        /// </summary>
        private void LogAlreadyRegistered(PassiveAsset passiveAsset, int currentLevel, int newLevel)
        {
            if (Log.LevelProgress)
            {
                LogProgress("{0}({1}) 패시브 독립체는 이미 등록되었습니다. (기존 레벨: {2}, 신규 레벨: {3})",
                passiveAsset.Name.ToLogString(), passiveAsset.Skill.ToLogString(), currentLevel, newLevel);
            }
        }

        /// <summary>
        /// RestTime 유지 중 로그
        /// </summary>
        private void LogRestTimeInfo(PassiveAsset passiveAsset, float remainingTime)
        {
            if (Log.LevelInfo)
            {
                LogInfo("{0}({1}) 패시브 추가 - 기존 RestTime 유지 중 (남은시간: {2:F1}초)",
                passiveAsset.Name.ToLogString(), passiveAsset.Skill.ToLogString(), remainingTime);
            }
        }

        private void LogFailedAdd(PassiveNames passiveName)
        {
            if (Log.LevelWarning)
            {
                LogWarning("{0}, 패시브를 추가할 수 없습니다. 패시브의 정보를 찾을 수 없습니다.", passiveName.ToLogString());
            }
        }

        private void LogAdd(PassiveAsset passiveAsset, int level)
        {
            if (Log.LevelInfo)
            {
                LogInfo("{0}({1}), 패시브를 추가했습니다. 패시브 레벨: {2}", passiveAsset.Name.ToLogString(), passiveAsset.Skill.ToLogString(), level);
            }
        }

        private void LogRemove(PassiveNames passiveName)
        {
            if (Log.LevelInfo)
            {
                LogInfo("{0}, 패시브를 삭제했습니다.", passiveName.ToLogString());
            }
        }

        private void LogFailedRemove(PassiveNames passiveName)
        {
            if (Log.LevelWarning)
            {
                LogWarning("{0}, 등록되지 않은 패시브를 삭제할 수 없습니다. ", passiveName.ToLogString());
            }
        }
    }
}