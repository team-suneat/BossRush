using System.Collections.Generic;

namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        #region OnLoadData

        private static void OnLoadJsonDataAll()
        {
            OnLoadJsonData(_statSheetData.Values);
            OnLoadJsonData(_stringSheetData.Values);
        }

        private static void OnLoadJsonData(IEnumerable<IData<int>> datas)
        {
            foreach (IData<int> data in datas)
            {
                data.OnLoadData();
            }
        }

        private static void OnLoadJsonData(IEnumerable<IData<string>> datas)
        {
            foreach (IData<string> data in datas)
            {
                data.OnLoadData();
            }
        }

        private static void OnLoadJsonData<T>(ListMultiMap<int, T> multiMap) where T : IData<int>
        {
            foreach (List<T> valueList in multiMap.Storage.Values)
            {
                foreach (T data in valueList)
                {
                    data.OnLoadData();
                }
            }
        }

        private static void OnLoadJsonData<T>(ListMultiMap<string, T> multiMap) where T : IData<string>
        {
            foreach (List<T> valueList in multiMap.Storage.Values)
            {
                foreach (T data in valueList)
                {
                    data.OnLoadData();
                }
            }
        }

        #endregion OnLoadData
    }
}