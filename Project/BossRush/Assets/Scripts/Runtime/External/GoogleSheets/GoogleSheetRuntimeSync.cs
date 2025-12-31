using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 런타임에서 시트 다운로드 → 변환 → JSON 저장까지 수행하는 진입점 유틸리티.
    /// 씬 어디서든 호출해 사용할 수 있습니다.
    /// </summary>
    public static class GoogleSheetRuntimeSync
    {
        /// <summary>
        /// 모든 등록된 GID를 순회하여 다운로드→변환 후, 결과를 JsonDataManager에 직접 주입합니다(파일 저장 없음).
        /// </summary>
        public static async Task<bool> FetchConvertAndApplyAllAsync()
        {
            // GameDefineAsset에서 구글 시트 동기화가 활성화되어 있는지 확인
            GameDefineAsset defineAsset = ScriptableDataManager.Instance?.GetGameDefine();
            if (defineAsset == null || !defineAsset.IsGoogleSheetSyncEnabled())
            {
                // 구글 시트 동기화가 비활성화되어 있으면 JSON 파일을 읽어오도록 합니다.
                return false;
            }

            bool anyApplied = false;

            // LINQ 대신 명시적인 루프 사용 (성능 규칙 준수)
            List<(GoogleSheetDatasetId datasetId, string gid, string cache)> targets = new();
            foreach ((GoogleSheetDatasetId datasetId, string gid) pair in GoogleSheetDatasetGids.EnumerateMappings())
            {
                if (pair.datasetId != GoogleSheetDatasetId.None)
                {
                    string cacheFileName = $"{pair.datasetId.ToLowerString()}_cache.tsv";
                    targets.Add((pair.datasetId, pair.gid, cacheFileName));
                }
            }

            for (int i = 0; i < targets.Count; i++)
            {
                (GoogleSheetDatasetId datasetId, string gid, string cache) = targets[i];
                if (string.IsNullOrEmpty(gid))
                {
                    continue;
                }

                string url = GameGoogleSheetLoader.BuildPublishedTSVUrlForGid(gid);
                IReadOnlyList<Dictionary<string, string>> rows = await GameGoogleSheetLoader.LoadAsync(url, cache);
                if (rows == null)
                {
                    Debug.LogError($"[GoogleSheetRuntimeSync] 로드 실패 gid:{gid} ({datasetId})");
                    continue;
                }

                bool success = ProcessDataset(datasetId, gid, rows);
                if (success)
                {
                    anyApplied = true;
                }
            }

            return anyApplied;
        }

        /// <summary>
        /// 데이터셋 타입에 따라 데이터를 변환하고 JsonDataManager에 주입합니다.
        /// </summary>
        private static bool ProcessDataset(GoogleSheetDatasetId datasetId, string gid, IReadOnlyList<Dictionary<string, string>> rows)
        {
            string datasetName = datasetId.ToString();

            switch (datasetId)
            {
                case GoogleSheetDatasetId.String:
                    return ProcessDatasetInternal<StringData>(datasetId, gid, rows, datasetName, JsonDataManager.SetStringData);

                case GoogleSheetDatasetId.Stat:
                    return ProcessDatasetInternal<StatData>(datasetId, gid, rows, datasetName, JsonDataManager.SetStatData);

                default:
                    Debug.LogWarning($"[GoogleSheetRuntimeSync] 미지원 데이터셋: {datasetId}");
                    return false;
            }
        }

        /// <summary>
        /// 제네릭 메서드로 데이터셋 변환 및 주입을 처리합니다.
        /// </summary>
        private static bool ProcessDatasetInternal<TModel>(
            GoogleSheetDatasetId datasetId,
            string gid,
            IReadOnlyList<Dictionary<string, string>> rows,
            string datasetName,
            Action<IEnumerable<TModel>> setDataAction) where TModel : class
        {
            if (!GoogleSheetConversionRunner.ConvertByGid<TModel>(gid, rows, out List<TModel> list))
            {
                Debug.LogError($"[GoogleSheetRuntimeSync] {datasetName} 변환 실패");
                return false;
            }

            setDataAction(list);
            return true;
        }
    }
}