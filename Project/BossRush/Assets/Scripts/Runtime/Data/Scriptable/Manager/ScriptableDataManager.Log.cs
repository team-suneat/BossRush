using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 로그 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Log Get Methods

        /// <summary>
        /// 로그 설정 에셋을 가져옵니다.
        /// </summary>
        public LogSettingAsset GetLogSetting()
        {
            return _logSetting;
        }

        #endregion Log Get Methods

        #region Log Find Methods

        /// <summary>
        /// 로그 태그를 찾습니다.
        /// </summary>
        public bool FindLog(LogTags tag)
        {
            if (_logSetting != null)
            {
                return _logSetting.Find(tag);
            }

            return false;
        }

        #endregion Log Find Methods

        #region Log Load Methods

        /// <summary>
        /// 로그 설정 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadLogSettingSync(string filePath)
        {
            if (!filePath.Contains("LogSetting"))
            {
                return false;
            }

            _logSetting = ResourcesManager.LoadResource<LogSettingAsset>(filePath);
            if (_logSetting != null)
            {
#if !UNITY_EDITOR
                _logSetting.ExternSwitchOffAll();
#endif
                Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                return true;
            }

            return false;
        }

        #endregion Log Load Methods

        #region Log Utility Methods

        /// <summary>
        /// 로그 로드 문자열 체크를 확인합니다.
        /// </summary>
        public bool CheckLogWithLoadString()
        {
            if (_logSetting != null)
            {
                return _logSetting.LoadString;
            }

            return false;
        }

        #endregion Log Utility Methods
    }
}
