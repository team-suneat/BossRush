using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.Data
{
    public partial class JsonDataManager
    {
        /// <summary>
        /// JSON 파일이 {"items": [...]} 형태로 저장되어 있으므로 래퍼 클래스를 사용하여 역직렬화
        /// </summary>
        private sealed class ListWrapper<T>
        {
            public List<T> items;
        }

        public static List<T> DeserializeObject<T>(string jsonData)
        {
            try
            {
                // JSON이 {"items": [...]} 형태이므로 래퍼 클래스로 역직렬화
                ListWrapper<T> wrapper = JsonConvert.DeserializeObject<ListWrapper<T>>(jsonData);

                if (wrapper == null || wrapper.items == null)
                {
                    Debug.LogWarning(typeof(T).ToString() + ", Json 파일의 items 필드가 null입니다.");
                    return new List<T>();
                }

                return wrapper.items;
            }
            catch (Exception e)
            {
                Debug.LogError(typeof(T).ToString() + ", Json 파일의 역직렬화 작업 중 에러가 나타났습니다.\n" + e.ToString());

                return new List<T>();
            }
        }

        private static List<T> DeserializeJsonData<T>(string jsonData)
        {
            try
            {
                List<T> dataList = DeserializeObject<T>(jsonData);

                if (dataList == null || dataList.Count == 0)
                {
                    Debug.LogError(typeof(T).ToString() + ", Json 파일을 역직렬화할 수 없습니다.");
                }

                return dataList;
            }
            catch (Exception ex)
            {
                Debug.LogError(typeof(T).ToString() + ", Json 파일의 역직렬화 작업 중 에러가 나타났습니다.\n" + ex.ToString());
            }

            return null;
        }

        public static void ParseJsonData(_Sheet sheet, string jsonData)
        {
            switch (sheet)
            {
                case _Sheet.String:
                    {
                        ParseStringJsonData(sheet, jsonData);
                    }
                    break;
                case _Sheet.Stat:
                    {
                        ParseStatJsonData(sheet, jsonData);
                    }
                    break;
            }
        }

        private static void ParseStringJsonData(_Sheet sheet, string jsonData)
        {
            List<StringData> dataList = DeserializeJsonData<StringData>(jsonData);

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Refresh();

                if (!_stringSheetData.ContainsKey(dataList[i].GetKey()))
                {
                    _stringSheetData.Add(dataList[i].GetKey(), dataList[i]);
                }
                else
                {
                    LogSameKeyAlreadyExists(dataList[i].GetKey().ToString(), sheet.ToString());
                }
            }

            Log.Progress(LogTags.JsonData, $"({sheet.ToSelectString()}) Json 데이터를 읽어옵니다. 불러온 데이터의 수: {dataList.Count.ToSelectString()})");
        }

        private static void ParseStatJsonData(_Sheet sheet, string jsonData)
        {
            List<StatData> dataList = DeserializeJsonData<StatData>(jsonData);

            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Refresh();

                if (!_statSheetData.ContainsKey(dataList[i].GetKey()))
                {
                    _statSheetData.Add(dataList[i].GetKey(), dataList[i]);
                }
                else
                {
                    LogSameKeyAlreadyExists(dataList[i].GetKey().ToString(), sheet.ToString());
                }
            }

            Log.Progress(LogTags.JsonData, $"({sheet.ToSelectString()}) Json 데이터를 읽어옵니다. 불러온 데이터의 수: {dataList.Count.ToSelectString()})");
        }
    }
}