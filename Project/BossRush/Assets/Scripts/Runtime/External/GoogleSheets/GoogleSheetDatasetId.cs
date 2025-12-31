using System.Collections.Generic;

namespace TeamSuneat
{
    /// <summary>
    /// 구글 스프레드시트 데이터셋 식별자.
    /// - 주의: 실제 GID 값은 정수 enum이 아닌 문자열입니다. 아래 레지스트리에서 관리합니다.
    /// </summary>
    public enum GoogleSheetDatasetId
    {
        None = 0,
        String,
        Stat,
    }

    /// <summary>
    /// 데이터셋 ↔ GID 매핑 레지스트리
    /// - 시트 탭의 gid 값을 문자열로 관리합니다.
    /// - 프로젝트 전역에서 동일한 참조를 사용하도록 고정 상수와 조회 API를 제공합니다.
    /// </summary>
    public static class GoogleSheetDatasetGids
    {
        // 고정 GID 상수
        public const string String = "0";
        public const string Stat = "1316031185";

        private static readonly Dictionary<GoogleSheetDatasetId, string> DatasetIdToGid = new()
        {
            { GoogleSheetDatasetId.String, String },
            { GoogleSheetDatasetId.Stat, Stat},
        };

        public static string GetGid(GoogleSheetDatasetId datasetId)
        {
            return DatasetIdToGid.TryGetValue(datasetId, out string gid) ? gid : null;
        }

        public static IEnumerable<(GoogleSheetDatasetId datasetId, string gid)> EnumerateMappings()
        {
            foreach (KeyValuePair<GoogleSheetDatasetId, string> kv in DatasetIdToGid)
            {
                yield return (kv.Key, kv.Value);
            }
        }
    }
}