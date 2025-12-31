using System.Collections.Generic;
using System.Text;
using TeamSuneat.Data;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 에디터 메뉴에서 고정된 구글 스프레드시트 CSV를 불러와 결과를 확인/로그하는 유틸리티.
    /// </summary>
    public static class GoogleSheetsMenu
    {
        [MenuItem("Tools/Google Sheets/모든 시트 불러오기 (GID 목록)")]
        public static async void LoadMultipleSheetsByGids()
        {
            // 중앙집중식 GID 상수 사용
            string[] gids = new string[]
            {
                GoogleSheetDatasetGids.Stat,
                GoogleSheetDatasetGids.String,
            };

            for (int idx = 0; idx < gids.Length; idx++)
            {
                string gid = gids[idx];
                string url = GameGoogleSheetLoader.BuildPublishedTSVUrlForGid(gid);
                Debug.Log($"[GoogleSheets] (gid:{gid}) 시작 - URL: {url}");

                IReadOnlyList<Dictionary<string, string>> rows = await GameGoogleSheetLoader.LoadAsync(url, $"google_sheet_cache_gid_{gid}.tsv");
                if (rows == null)
                {
                    Debug.LogError($"[GoogleSheets] (gid:{gid}) 결과가 null입니다.");
                    continue;
                }

                Debug.Log($"[GoogleSheets] (gid:{gid}) 로드 완료 - 행 수: {rows.Count}");
                for (int i = 0; i < rows.Count; i++)
                {
                    string preview = PreviewRow(rows[i]);
                    Debug.Log($"[GoogleSheets] (gid:{gid}) row[{i}]: {preview}");
                }
            }
        }

        #region Convert Sheet To Json

        [MenuItem("Tools/Google Sheets/Convert To Json/All")]
        public static void ConvertAllToJson()
        {
            ConvertStatToJson();
            ConvertStringToJson();
        }

        [MenuItem("Tools/Google Sheets/Convert To Json/Stat")]
        public static void ConvertStatToJson()
        {
            ConvertCacheToJson<StatData>("Stat", GoogleSheetDatasetGids.Stat);
        }

        [MenuItem("Tools/Google Sheets/Convert To Json/String")]
        public static void ConvertStringToJson()
        {
            ConvertCacheToJson<StringData>("String", GoogleSheetDatasetGids.String);
        }

        private static async void ConvertCacheToJson<IData>(string key, string gid)
        {
            string url = GameGoogleSheetLoader.BuildPublishedTSVUrlForGid(gid);
            Debug.Log($"[GoogleSheets] {key} 변환 시작 - url:{url}");

            IReadOnlyList<Dictionary<string, string>> rows = await GameGoogleSheetLoader.LoadAsync(url, $"{key.ToLowerString()}_cache.tsv");
            if (rows == null)
            {
                Debug.LogError($"[GoogleSheets] {key} 로드 실패");
                return;
            }

            bool ok = GoogleSheetConversionRunner.ConvertByGid(gid, rows, out List<IData> list);
            Debug.Log($"[GoogleSheets] {key} 변환 결과 - 입력:{rows.Count}, 성공:{list?.Count ?? 0}");

            if (!ok || list == null)
            {
                Debug.LogError($"[GoogleSheets] {key} 변환 실패");
                return;
            }

            bool saved = GoogleSheetJsonSaver.SaveListAsJson(list, $"{key}.json");
            Debug.Log(saved ? $"[GoogleSheets] {key} JSON 저장 완료" : $"[GoogleSheets] {key} JSON 저장 실패");
        }

        #endregion Convert Sheet To Json

        private static string PreviewRow(Dictionary<string, string> row)
        {
            if (row == null || row.Count == 0)
            {
                return "<empty>";
            }

            int printed = 0;
            StringBuilder sb = new();
            foreach (KeyValuePair<string, string> kv in row)
            {
                if (sb.Length > 0)
                {
                    _ = sb.Append(", ");
                }

                _ = sb.Append(kv.Key).Append(": ").Append(kv.Value);
                printed++;
            }
            return sb.ToString();
        }
    }
}