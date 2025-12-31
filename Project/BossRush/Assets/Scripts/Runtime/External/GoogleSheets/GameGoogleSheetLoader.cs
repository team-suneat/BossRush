using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace TeamSuneat
{
    /// <summary>
    /// 구글 스프레드시트를 TSV로 공개(Publish to web)한 URL에서 런타임으로 데이터를 가져오고,
    /// 파싱 후 캐시(persistentDataPath)에 저장/로드하는 최소 유틸리티.
    ///
    /// - 주의: 민감 데이터는 포함하지 말 것.
    /// - 보안: 프로덕션 빌드에서는 캐시를 사용하지 않아 사용자가 캐시 파일을 수정해 치트하는 것을 방지합니다.
    /// - 개발/에디터 빌드에서만 캐시를 사용하며, 네트워크 실패 시 캐시에서 폴백합니다.
    /// - 프로덕션 빌드에서 네트워크 실패 시 빈 결과를 반환합니다.
    /// </summary>
    public static class GameGoogleSheetLoader
    {
        // 고정 공개 TSV URL 상수
        private const string DEFAULT_TSV_URL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQvl-vArisb1itvV7oFmSwnU5_V3DHHO58LiAs9btdlc6xN2eZmRb4ETTmMucDKzyWcZPGy6GcHxbf-/pub?output=tsv";

        /// <summary>
        /// 게시된 TSV 기본 URL(…/pub) 반환. (gid/query 미포함)
        /// </summary>
        public static string BasePublishedUrl => GetBasePublishedUrl(DEFAULT_TSV_URL);

        /// <summary>
        /// 특정 gid 탭을 TSV로 가져오는 게시 URL을 생성합니다.
        /// </summary>
        public static string BuildPublishedTSVUrlForGid(string gid)
        {
            string baseUrl = BasePublishedUrl;
            if (string.IsNullOrEmpty(baseUrl))
            {
                return null;
            }

            return string.IsNullOrEmpty(gid)
                ? baseUrl + "?single=true&output=tsv"
                : baseUrl + "?gid=" + gid + "&single=true&output=tsv";
        }

        /// <summary>
        /// 특정 데이터셋 식별자에 해당하는 탭의 게시 TSV URL을 생성합니다.
        /// </summary>
        public static string BuildPublishedCsvUrlForDataset(GoogleSheetDatasetId datasetId)
        {
            string gid = GoogleSheetDatasetGids.GetGid(datasetId);
            return BuildPublishedTSVUrlForGid(gid);
        }

        private static string GetBasePublishedUrl(string publishedCsvUrl)
        {
            if (string.IsNullOrEmpty(publishedCsvUrl))
            {
                return null;
            }

            int idx = publishedCsvUrl.IndexOf("/pub");
            if (idx < 0)
            {
                return null;
            }

            return publishedCsvUrl.Substring(0, idx) + "/pub";
        }

        /// <summary>
        /// TSV URL에서 데이터를 가져와 파싱한 뒤, 캐시에 저장합니다. 실패 시 캐시에서 불러옵니다.
        /// 보안상 프로덕션 빌드에서는 캐시를 사용하지 않고, 개발/에디터 빌드에서만 캐시를 사용합니다.
        /// </summary>
        /// <param name="csvUrl">구글 시트 TSV 공개 URL (export?format=csv&gid=...)</param>
        /// <param name="cacheFileName">persistentDataPath 하위에 저장할 캐시 파일명 (개발/에디터 빌드에서만 사용)</param>
        /// <returns>헤더를 키로 갖는 행 딕셔너리 리스트</returns>
        public static async Task<IReadOnlyList<Dictionary<string, string>>> LoadAsync(string csvUrl, string cacheFileName)
        {
            if (string.IsNullOrWhiteSpace(csvUrl))
            {
                throw new ArgumentException("csvUrl 비어있음", nameof(csvUrl));
            }

            if (string.IsNullOrWhiteSpace(cacheFileName))
            {
                throw new ArgumentException("cacheFileName 비어있음", nameof(cacheFileName));
            }

            bool allowCache = GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD;
            string cachePath = allowCache ? Path.Combine(Application.persistentDataPath, cacheFileName) : null;
            string csvText = await DownloadTextAsync(csvUrl);

            if (string.IsNullOrEmpty(csvText))
            {
                // 네트워크 실패 → 개발/에디터 빌드에서만 캐시 폴백 사용
                // 프로덕션 빌드에서는 캐시를 사용하지 않아 치트 방지
                if (allowCache && !string.IsNullOrEmpty(cachePath) && File.Exists(cachePath))
                {
                    csvText = File.ReadAllText(cachePath);
                    Debug.LogWarning($"[GameGoogleSheetCsvLoader] 네트워크 실패로 캐시에서 로드: {cacheFileName}");
                }
                else
                {
                    Debug.LogError($"[GameGoogleSheetCsvLoader] 네트워크 실패 및 캐시 사용 불가 (프로덕션 빌드): {cacheFileName}");
                    return Array.Empty<Dictionary<string, string>>();
                }
            }
            else
            {
                // 성공 시 개발/에디터 빌드에서만 캐시 갱신
                if (allowCache && !string.IsNullOrEmpty(cachePath))
                {
                    SafeWriteAllText(cachePath, csvText);
                }
            }

            return ParseCsvToDictList(csvText);
        }

        /// <summary>
        /// TSV 문자열을 헤더 기반 딕셔너리 리스트로 변환합니다.
        /// 따옴표로 감싼 셀 내 콤마/개행, 이중 따옴표 이스케이프를 처리합니다.
        /// </summary>
        public static List<Dictionary<string, string>> ParseCsvToDictList(string csv)
        {
            List<Dictionary<string, string>> result = new();
            if (string.IsNullOrEmpty(csv))
            {
                return result;
            }

            // 통일된 개행 처리
            string normalized = csv.Replace("\r\n", "\n").Replace("\r", "\n");
            List<string> rows = SplitCsvRows(normalized);
            if (rows.Count == 0)
            {
                return result;
            }

            // 구분자 자동 감지(콤마/탭). 다중 선택 드롭다운 등으로 인한 콤마 포함 셀 이슈 완화
            char delimiter = DetectDelimiter(rows[0]);
            List<string> headers = SplitCsvLine(rows[0], delimiter);
            if (headers.Count == 0)
            {
                return result;
            }

            // 헤더 전처리 분리: 공백 제거 및 '#' 시작 컬럼 제외
            BuildIncludedHeaderMaps(headers, out List<int> includeColumnIndices, out List<string> includeHeaders);
            if (includeColumnIndices.Count == 0)
            {
                return result;
            }

            for (int i = 1; i < rows.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(rows[i]))
                {
                    continue;
                }

                List<string> cols = SplitCsvLine(rows[i], delimiter);
                Dictionary<string, string> rowDict = new(includeColumnIndices.Count);
                for (int idx = 0; idx < includeColumnIndices.Count; idx++)
                {
                    int colIndex = includeColumnIndices[idx];
                    string key = includeHeaders[idx];
                    string val = colIndex < cols.Count ? cols[colIndex] : string.Empty;
                    rowDict[key] = val;
                }
                result.Add(rowDict);
            }

            return result;
        }

        private static async Task<string> DownloadTextAsync(string url)
        {
            using UnityWebRequest req = UnityWebRequest.Get(url);
            UnityWebRequestAsyncOperation op = req.SendWebRequest();
            while (!op.isDone)
            {
                await Task.Yield();
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                return null;
            }

            return req.downloadHandler.text;
        }

        private static void SafeWriteAllText(string path, string contents)
        {
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    _ = Directory.CreateDirectory(dir);
                }

                File.WriteAllText(path, contents, new UTF8Encoding(false));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameGoogleSheetCsvLoader] 캐시 저장 실패: {e.Message}");
            }
        }

        // TSV 파싱 로직들
        private static List<string> SplitCsvRows(string text)
        {
            List<string> rows = new();
            StringBuilder sb = new();
            bool inQuotes = false;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == '"')
                {
                    // 이중 따옴표 이스케이프 처리
                    bool isEscaped = i + 1 < text.Length && text[i + 1] == '"';
                    if (isEscaped)
                    {
                        _ = sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (ch == '\n' && !inQuotes)
                {
                    rows.Add(sb.ToString());
                    sb.Length = 0;
                }
                else
                {
                    _ = sb.Append(ch);
                }
            }
            if (sb.Length > 0)
            {
                rows.Add(sb.ToString());
            }

            return rows;
        }

        private static List<string> SplitCsvLine(string line, char delimiter)
        {
            List<string> cols = new();
            StringBuilder sb = new();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (ch == '"')
                {
                    bool isEscaped = i + 1 < line.Length && line[i + 1] == '"';
                    if (isEscaped)
                    {
                        _ = sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (ch == delimiter && !inQuotes)
                {
                    cols.Add(sb.ToString());
                    sb.Length = 0;
                }
                else
                {
                    _ = sb.Append(ch);
                }
            }
            cols.Add(sb.ToString());
            return cols;
        }

        // 헤더 한 줄을 기준으로 필드 구분자 감지: 탭(\t) 우선, 아니면 콤마
        private static char DetectDelimiter(string headerLine)
        {
            if (string.IsNullOrEmpty(headerLine)) return ',';
            int tabCount = 0;
            int commaCount = 0;
            bool inQuotes = false;
            for (int i = 0; i < headerLine.Length; i++)
            {
                char ch = headerLine[i];
                if (ch == '"')
                {
                    bool isEscaped = i + 1 < headerLine.Length && headerLine[i + 1] == '"';
                    if (isEscaped) { i++; continue; }
                    inQuotes = !inQuotes;
                    continue;
                }
                if (inQuotes) continue;
                if (ch == '\t') tabCount++;
                else if (ch == ',') commaCount++;
            }
            if (tabCount > 0 && tabCount >= commaCount) return '\t';
            return ',';
        }

        /// <summary>
        /// 헤더 전처리: 공백을 트림하고, '#'로 시작하는 컬럼을 제외한 포함 컬럼 인덱스/이름 목록을 생성합니다.
        /// </summary>
        private static void BuildIncludedHeaderMaps(List<string> headers, out List<int> includeColumnIndices, out List<string> includeHeaders)
        {
            includeColumnIndices = new List<int>(headers.Count);
            includeHeaders = new List<string>(headers.Count);
            for (int h = 0; h < headers.Count; h++)
            {
                string header = headers[h] != null ? headers[h].Trim() : string.Empty;
                if (string.IsNullOrEmpty(header))
                {
                    continue;
                }
                if (header.StartsWith("#"))
                {
                    continue;
                }
                includeColumnIndices.Add(h);
                includeHeaders.Add(header);
            }
        }
    }
}