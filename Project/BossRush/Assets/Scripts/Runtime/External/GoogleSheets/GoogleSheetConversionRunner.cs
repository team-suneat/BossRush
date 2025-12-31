using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// GID에 따라 등록된 컨버터로 행 리스트를 강타입 데이터로 변환하는 실행기.
    /// </summary>
    public static class GoogleSheetConversionRunner
    {
        private static readonly Dictionary<string, object> gidToConverter = new()
        {
            { GoogleSheetDatasetGids.String, new StringRowConverter() },
            { GoogleSheetDatasetGids.Stat, new StatRowConverter() },
        };

        public static bool HasConverterForGid(string gid)
        {
            return gidToConverter.ContainsKey(gid);
        }

        /// <summary>
        /// 특정 gid에 맞는 컨버터로 변환을 시도합니다.
        /// 변환에 성공하면 result에 결과 리스트가 담기고 true를 반환합니다.
        /// </summary>
        public static bool ConvertByGid<TModel>(string gid, IReadOnlyList<Dictionary<string, string>> rows, out List<TModel> result)
        {
            result = new List<TModel>();

            if (!gidToConverter.TryGetValue(gid, out object converterObj))
            {
                Log.Warning($"컨버터 미등록 GID: {gid}");
                return false;
            }

            if (converterObj is not IGoogleSheetRowConverter<TModel> converter)
            {
                Log.Warning($"타입 불일치 컨버터 GID: {gid} (요청 타입: {typeof(TModel).Name})");
                return false;
            }

            int success = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] is not Dictionary<string, string> row)
                {
                    continue;
                }

                if (converter.TryConvert(row, out TModel model))
                {
                    result.Add(model);
                    success++;
                }
            }

            Debug.Log($"[GoogleSheetConversionRunner] {typeof(TModel)}(GID:{gid}) 변환 완료: 입력 {rows.Count} → 성공 {success} / 스킵 {rows.Count - success}");
            return success > 0;
        }
    }
}