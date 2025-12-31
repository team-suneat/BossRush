namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        private static void LogWarningParseJsonData()
        {
#if UNITY_EDITOR
#endif
        }

        private static void LogSameKeyAlreadyExists(string dataName, string sheetName)
        {
#if UNITY_EDITOR
            LogWarning($"{dataName}, 같은 키를 가진 데이터가 이미 존재합니다. 시트: {sheetName}");
#endif
        }

        private static void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.JsonData, format, args);
            }
        }

        private static void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.JsonData, format, args);
            }
        }
    }
}