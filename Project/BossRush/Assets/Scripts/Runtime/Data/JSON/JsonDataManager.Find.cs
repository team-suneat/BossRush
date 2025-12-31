namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        public static string FindStringClone(string key)
        {
            if (_stringSheetData.ContainsKey(key))
            {
                return _stringSheetData[key].GetString();
            }

            return string.Empty;
        }

        public static string FindStringClone(string key, LanguageNames languageName)
        {
            if (_stringSheetData.ContainsKey(key))
            {
                return _stringSheetData[key].GetString(languageName);
            }

            Log.Warning(LogTags.JsonData, "스트링을 찾을 수 없습니다: {0}", key);
            return string.Empty;
        }

        public static StringData FindStringData(string key)
        {
            if (_stringSheetData.ContainsKey(key))
            {
                return _stringSheetData[key];
            }

            return new StringData();
        }

        public static StatData FindStatData(StatNames key)
        {
            return FindStatData(key.ToInt());
        }

        public static StatData FindStatData(int TID)
        {
            if (_statSheetData.ContainsKey(TID))
            {
                return _statSheetData[TID];
            }

            return new StatData();
        }
    }
}