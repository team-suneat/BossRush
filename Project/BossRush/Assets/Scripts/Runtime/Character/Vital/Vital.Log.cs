using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        private string LogFormat(string content)
        {
            if (Owner != null)
            {
                return string.Format("{0}({1}), {2}", Owner.Name.ToLogString(), SID, content);
            }
            else
            {
                return string.Format("{0}, {1}", this.GetHierarchyName(), content);
            }
        }

        protected void LogProgress(string format)
        {
            if (!Log.LevelProgress)
            {
                return;
            }

            Log.Progress(LogTags.Vital, LogFormat(format));
        }

        protected void LogInfo(string format)
        {
            if (!Log.LevelInfo)
            {
                return;
            }

            Log.Info(LogTags.Vital, LogFormat(format));
        }

        protected void LogWarning(string format)
        {
            if (!Log.LevelWarning)
            {
                return;
            }

            Log.Warning(LogTags.Vital, LogFormat(format));
        }

        protected void LogError(string format)
        {
            if (!Log.LevelError)
            {
                return;
            }

            Log.Error(LogFormat(format));
        }

        protected void LogProgress(string format, params object[] args)
        {
            if (!Log.LevelProgress)
            {
                return;
            }

            Log.Progress(LogTags.Vital, LogFormat(string.Format(format, args)));
        }

        protected void LogInfo(string format, params object[] args)
        {
            if (!Log.LevelInfo)
            {
                return;
            }

            Log.Info(LogTags.Vital, LogFormat(string.Format(format, args)));
        }

        protected void LogWarning(string format, params object[] args)
        {
            if (!Log.LevelWarning)
            {
                return;
            }

            Log.Warning(LogTags.Vital, LogFormat(string.Format(format, args)));
        }

        protected void LogError(string format, params object[] args)
        {
            if (!Log.LevelError)
            {
                return;
            }

            Log.Error(LogFormat(string.Format(format, args)));
        }

        //────────────────────────────────────────────────────────────────────────────────────────────────

        private void LogErrorUseBattleResource(HitmarkAssetData hitmarkAssetData, int value)
        {
            if (!Log.LevelError)
            {
                return;
            }

            if (Owner != null)
            {
                LogError("이 공격({0})으로 캐릭터({1})의 전투 자원({2})을 소모할 수 없습니다. Value:{3}"
                    , hitmarkAssetData.Name.ToLogString(), Owner.Name, hitmarkAssetData.ResourceConsumeType, value);
            }
            else
            {
                LogError("이 공격({0})으로 캐릭터의 전투 자원({1})을 소모할 수 없습니다. Value:{2}"
                    , hitmarkAssetData.Name.ToLogString(), hitmarkAssetData.ResourceConsumeType, value);
            }
        }

        private void LogErrorAddResource(VitalConsumeTypes consumeType, float rate)
        {
            if (!Log.LevelError)
            {
                return;
            }

            LogError("전투 자원({0})을 회복할 수 없습니다. Value Rate:{1}", consumeType, rate);
        }

        private void LogErrorTakeDamageZero(HitmarkNames hitmarkName)
        {
            if (!Log.LevelError)
            {
                return;
            }

            LogError("{0}의 설정된 피해량이 0입니다. 피해를 받지 못합니다.", hitmarkName.ToLogString());
        }

        private void LogErrorFindCurrentResource(VitalResourceTypes resourceType)
        {
            if (!Log.LevelError)
            {
                return;
            }

            LogError("Vital에서 전투 자원({0})의 현재 값을 찾을 수 없습니다. 경로: {1}", resourceType, this.GetHierarchyPath());
        }

        private void LogErrorFindCurrentResource(VitalConsumeTypes consumeTypes)
        {
            if (!Log.LevelError)
            {
                return;
            }

            LogError("Vital에서 전투 자원({0})의 현재 값을 찾을 수 없습니다. 경로: {1}", consumeTypes, this.GetHierarchyPath());
        }

        private void LogErrorFindMaxResource(VitalConsumeTypes consumeTypes)
        {
            if (!Log.LevelError)
            {
                return;
            }

            LogError("Vital에서 전투 자원({0})의 최대 값을 찾을 수 없습니다. 경로: {1}", consumeTypes, this.GetHierarchyPath());
        }

        private void LogWarningFindResourceRate(VitalResourceTypes resourceType)
        {
            if (!Log.LevelWarning)
            {
                return;
            }

            LogWarning("Vital에서 전투 자원({0})의 비율을 찾을 수 없습니다. 경로: {1}", resourceType, this.GetHierarchyPath());
        }
    }
}