using System;
using System.Threading;
using UnityEngine;

namespace TeamSuneat
{
    public static class RandomEx
    {
        private sealed class RandomHolder
        {
            public int SeedVersion;
            public System.Random Rng;
        }

        // 결정적 모드: SetSeed 호출 시 활성화, 고정 시드로 재현성 보장
        // 비결정적 모드: 기본값, 매 실행마다 다른 시퀀스
        private static int _baseSeed;
        private static bool _hasBaseSeed;

        // 재시드 요청을 모든 스레드에 전파하기 위한 버전 카운터
        private static int _seedVersion;

        // 스레드별 독립적인 RNG 인스턴스 보유 (스레드 안전성 확보)
        private static readonly ThreadLocal<RandomHolder> ThreadRandom = new(() => new RandomHolder());

        public static void SetSeed(int seed)
        {
            _baseSeed = seed;
            _hasBaseSeed = true;

            // 모든 스레드가 다음 접근 시 RNG를 재생성하도록 버전 증가
            Interlocked.Increment(ref _seedVersion);
        }

        public static void ClearSeed()
        {
            _hasBaseSeed = false;
            Interlocked.Increment(ref _seedVersion);
        }

        public static void Reseed(int? customSeed = null)
        {
            if (customSeed.HasValue)
            {
                SetSeed(customSeed.Value);
                return;
            }

            // 시드 모드는 유지하고 시퀀스만 리셋
            Interlocked.Increment(ref _seedVersion);
        }

        public static bool GetBoolValue()
        {
            return GetRng().Next(0, 2) == 1;
        }

        public static float GetFloatValue()
        {
            // 0 이상 1 미만
            return (float)GetRng().NextDouble();
        }

        #region Vector

        public static Vector2 GetVector2Value(Vector2 size)
        {
            float x = size.x.IsZero() ? 0f : Range(-size.x, size.x);
            float y = size.y.IsZero() ? 0f : Range(-size.y, size.y);

            return new Vector2(x, y);
        }

        public static Vector3 GetVector3Value(Vector2 size)
        {
            float x = size.x.IsZero() ? 0f : Range(-size.x, size.x);
            float y = size.y.IsZero() ? 0f : Range(-size.y, size.y);

            return new Vector3(x, y, 0f);
        }

        #endregion Vector

        public static int Range(int min, int max)
        {
            if (min >= max)
            {
                return min;
            }

            return GetRng().Next(min, max);
        }

        public static float Range(float min, float max)
        {
            // 범위가 유효하지 않거나 0인 경우 최소값만 반환합니다.
            // 위의 정수 오버로드의 동작을 반영합니다.
            if (min >= max || Mathf.Approximately(min, max))
            {
                return min;
            }

            float randomValue = GetFloatValue();
            return (randomValue * (max - min)) + min;
        }

        private static System.Random GetRng()
        {
            RandomHolder holder = ThreadRandom.Value;

            // 원자적으로 현재 시드 버전 읽기
            int currentVersion = Volatile.Read(ref _seedVersion);

            // 버전이 일치하면 기존 RNG 재사용
            if (holder.Rng != null && holder.SeedVersion == currentVersion)
            {
                return holder.Rng;
            }

            // 버전이 변경되었거나 초기 생성이면 새 RNG 생성
            int seed = CreateSeedForCurrentThread(currentVersion);
            holder.Rng = new System.Random(seed);
            holder.SeedVersion = currentVersion;

            return holder.Rng;
        }

        private static int CreateSeedForCurrentThread(int seedVersion)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            if (_hasBaseSeed)
            {
                // 결정적 모드: 같은 baseSeed + seedVersion이면 같은 시드 보장
                unchecked
                {
                    int hash = 17;
                    hash = (hash * 31) ^ _baseSeed;
                    hash = (hash * 31) ^ seedVersion;
                    hash = (hash * 31) ^ threadId;
                    return hash;
                }
            }

            // 비결정적 모드: 매 실행마다 다른 시드
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) ^ Environment.TickCount;
                hash = (hash * 31) ^ threadId;
                hash = (hash * 31) ^ Application.version.GetHashCode();
                return hash;
            }
        }
    }
}
