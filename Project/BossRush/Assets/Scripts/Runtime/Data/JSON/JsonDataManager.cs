using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        #region Field

        /// <summary>
        /// 비동기 모드 사용 여부를 나타내는 상수
        /// </summary>
        public const bool IS_ASYNC_MODE = true;

        private static readonly JsonSerializerSettings _deserializeSettings;

        private static readonly Dictionary<string, StringData> _stringSheetData = new();
        private static readonly Dictionary<int, StatData> _statSheetData = new();

        #endregion Field

        public static void ClearAll()
        {
            _stringSheetData.Clear();
            _statSheetData.Clear();
        }

        public static bool CheckLoaded()
        {
            return _stringSheetData.Count > 0 && _statSheetData.Count > 0;
        }

        public static void SetStringData(IEnumerable<StringData> list)
        {
            _stringSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (StringData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                string key = item.GetKey();
                if (_stringSheetData.ContainsKey(key))
                {
                    LogWarning("StringData 키 중복: {0}", key);
                    continue;
                }
                _stringSheetData.Add(key, item);
            }
        }

        public static void SetStatData(IEnumerable<StatData> list)
        {
            _statSheetData.Clear();
            if (list == null)
            {
                return;
            }

            foreach (StatData item in list)
            {
                if (item == null)
                {
                    continue;
                }

                int key = item.GetKey();
                if (_statSheetData.ContainsKey(key))
                {
                    LogWarning("StatData 키 중복: {0}", key);
                    continue;
                }
                _statSheetData.Add(key, item);
            }
        }
    }
}