using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 런타임 JSON 저장 유틸리티(리스트 직렬화 포함).
    /// JsonUtility는 리스트 루트 직렬화를 지원하지 않으므로 래퍼를 사용합니다.
    /// </summary>
    public static class GoogleSheetJsonSaver
    {
        [Serializable]
        private sealed class ListWrapper<T>
        {
            public List<T> items;
        }

        public static string GetDataDir()
        {
            // 에디터/프로젝트 자산에 포함될 수 있도록 Assets/Addressables/JSON으로 저장
            return Path.Combine(Application.dataPath, Path.Combine("Addressables", "JSON"));
        }

        public static string BuildPath(string fileName)
        {
            return Path.Combine(GetDataDir(), fileName);
        }

        public static bool SaveListAsJson<T>(IEnumerable<T> list, string fileName)
        {
            try
            {
                ListWrapper<T> wrapper = new()
                {
                    items = new List<T>(list)
                };

                string json = JsonUtility.ToJson(wrapper, prettyPrint: false);
                string dir = GetDataDir();
                if (!Directory.Exists(dir))
                {
                    _ = Directory.CreateDirectory(dir);
                }

                string path = BuildPath(fileName);
                File.WriteAllText(path, json, new UTF8Encoding(false));
                Debug.Log($"[GoogleSheetJsonSaver] 저장 완료: {path}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GoogleSheetJsonSaver] 저장 실패: {e.Message}");
                return false;
            }
        }
    }
}