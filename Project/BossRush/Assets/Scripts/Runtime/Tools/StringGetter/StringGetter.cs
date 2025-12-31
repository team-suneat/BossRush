using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 다국어 문자열 캐싱 유틸리티.
    /// * 언어 ▶ (key ▶ value) 2‑계층 <see cref="ConcurrentDictionary{TKey,TValue}"/>
    /// * 스레드 안전 보장 & 폴백 로직 포함.
    /// </summary>
    public static partial class StringGetter
    {
        // ──────────────────────────────────────────────────────────────────
        //  Fields & Stats
        // ──────────────────────────────────────────────────────────────────

        /// <summary>캐시 정리·스왑 등 대규모 작업을 잠글 때만 사용하는 전역 락</summary>
        private static readonly object _lockObject = new();

        /// <summary>언어 ▶ (key ▶ value) 2‑계층 캐시</summary>
        private static readonly ConcurrentDictionary<LanguageNames, ConcurrentDictionary<string, string>>
            _cachePool = new();

        /// <summary>캐시 효율을 모니터링하기 위한 간단한 통계</summary>
        private static class CacheStats
        {
            public static int TotalHits { get; private set; }
            public static int TotalMisses { get; private set; }

            [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
            public static void RecordHit()
            {
                TotalHits++;
            }

            [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
            public static void RecordMiss()
            {
                TotalMisses++;
            }

            public static void Reset()
            { TotalHits = 0; TotalMisses = 0; }
        }

        // ──────────────────────────────────────────────────────────────────
        //  Helpers
        // ──────────────────────────────────────────────────────────────────

        /// <summary>현재 게임 설정에서 선택된 언어의 캐시를 반환합니다. 없으면 즉시 생성.</summary>
        private static ConcurrentDictionary<string, string> CurrentCache =>
            _cachePool.GetOrAdd(GameSetting.Instance.Language.Name,
                                _ => new ConcurrentDictionary<string, string>(Environment.ProcessorCount, 256));

        /// <summary>실제 플레이 환경에서만 캐싱을 동작시킵니다.</summary>
        private static bool IsValidOperation()
        {
            return Application.isPlaying;
        }

        // ──────────────────────────────────────────────────────────────────
        //  Healthcycle
        // ──────────────────────────────────────────────────────────────────

        /// <summary>모든 언어 캐시를 비우고 통계를 초기화합니다.</summary>
        public static void Clear()
        {
            lock (_lockObject)
            {
                foreach (KeyValuePair<LanguageNames, ConcurrentDictionary<string, string>> pair in _cachePool)
                {
                    Log.Info(LogTags.String, "스트링 캐시 초기화 – {0}: {1}개", pair.Key, pair.Value.Count);
                    pair.Value.Clear();
                }
                CacheStats.Reset();
            }
        }

        // ──────────────────────────────────────────────────────────────────
        //  Core Cache API (private)
        // ──────────────────────────────────────────────────────────────────

        private static void RegisterString(string key, string value)
        {
            if (!IsValidOperation())
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (CurrentCache.TryAdd(key, value))
            {
                Log.Info(LogTags.String, "캐시 등록 – [{0}] {1} (총 {2})", GameSetting.Instance.Language.Name, key, CurrentCache.Count);
            }
        }

        public static string ConcatStringWithComma(params string[] values)
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                stringBuilder.Append(values[i]);
                if (i < values.Length - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            return stringBuilder.ToString();
        }
    }
}