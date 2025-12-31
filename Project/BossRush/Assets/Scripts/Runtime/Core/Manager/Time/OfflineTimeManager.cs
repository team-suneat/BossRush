using System;
using TeamSuneat.Data.Game;

namespace TeamSuneat
{
    /// <summary>
    /// 오프라인 시간 계산 및 보상을 관리하는 싱글톤 클래스
    /// </summary>
    public class OfflineTimeManager : Singleton<OfflineTimeManager>
    {
        #region Constants

        // 최대 오프라인 보상 시간 (24시간)
        private const double MAX_OFFLINE_HOURS = 24.0;

        // 최소 오프라인 시간 (1분 미만은 무시)
        private const double MIN_OFFLINE_MINUTES = 1.0;

        // 시간 조작 검증: 현재 시간이 마지막 저장 시간보다 이전이면 조작으로 간주
        private const double TIME_MANIPULATION_THRESHOLD_HOURS = -1.0;

        #endregion Constants

        #region Public Properties

        /// <summary>계산된 오프라인 시간 (초)</summary>
        public double OfflineTimeSeconds { get; private set; }

        /// <summary>보상 가능한 오프라인 시간 (초, 최대 시간 제한 적용)</summary>
        public double RewardableOfflineTimeSeconds { get; private set; }

        /// <summary>오프라인 시간이 유효한지 여부 (시간 조작 없음)</summary>
        public bool IsOfflineTimeValid { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void CalculateOfflineTime()
        {
            if (!TryGetLastSaveTime(out DateTime lastSaveTime))
            {
                ResetOfflineTime();
                return;
            }

            TimeSpan offlineTimeSpan = CalculateOfflineTimeSpan(lastSaveTime);
            if (!ValidateOfflineTime(offlineTimeSpan, lastSaveTime))
            {
                return;
            }

            double rewardableSeconds = CalculateRewardableTime(offlineTimeSpan);
            ApplyOfflineTimeResult(offlineTimeSpan, rewardableSeconds);
        }

        public void ResetOfflineTime()
        {
            OfflineTimeSeconds = 0;
            RewardableOfflineTimeSeconds = 0;
            IsOfflineTimeValid = false;
        }

        public TimeSpan GetOfflineTimeSpan()
        {
            return TimeSpan.FromSeconds(RewardableOfflineTimeSeconds);
        }

        public string GetOfflineTimeString()
        {
            if (RewardableOfflineTimeSeconds <= 0)
            {
                return "00:00:00";
            }

            TimeSpan timeSpan = GetOfflineTimeSpan();
            return FormatTimeSpan(timeSpan);
        }

        #endregion Public Methods

        #region Private Methods

        private bool TryGetLastSaveTime(out DateTime lastSaveTime)
        {
            lastSaveTime = default;

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null || profile.Statistics == null)
            {
                return false;
            }

            lastSaveTime = profile.Statistics.LastSaveTime;
            if (lastSaveTime == default(DateTime))
            {
                return false;
            }

            return true;
        }

        private TimeSpan CalculateOfflineTimeSpan(DateTime lastSaveTime)
        {
            DateTime currentTime = DateTime.Now;
            return currentTime - lastSaveTime;
        }

        private bool ValidateOfflineTime(TimeSpan offlineTimeSpan, DateTime lastSaveTime)
        {
            double offlineHours = offlineTimeSpan.TotalHours;

            // 시간 조작 검증: 현재 시간이 마지막 저장 시간보다 이전이면 조작으로 간주
            if (offlineHours < TIME_MANIPULATION_THRESHOLD_HOURS)
            {
                Log.Warning(LogTags.Time, $"시간 조작이 감지되었습니다. 마지막 저장: {lastSaveTime}, 현재: {DateTime.Now}");
                ResetOfflineTime();
                IsOfflineTimeValid = false;
                return false;
            }

            // 최소 시간 미만이면 오프라인 시간 없음으로 처리
            if (offlineHours < (MIN_OFFLINE_MINUTES / 60.0))
            {
                ResetOfflineTime();
                return false;
            }

            return true;
        }

        private double CalculateRewardableTime(TimeSpan offlineTimeSpan)
        {
            double offlineHours = offlineTimeSpan.TotalHours;

            // 비정상적으로 긴 오프라인 시간 감지
            if (offlineHours > MAX_OFFLINE_HOURS * 2)
            {
                string offlineTimeString = FormatTimeSpan(offlineTimeSpan);
                Log.Warning(LogTags.Time, $"비정상적으로 긴 오프라인 시간: {offlineTimeString}. 최대 보상 시간으로 제한합니다.");
            }

            // 최대 보상 시간 제한 적용
            double rewardableHours = Math.Min(offlineHours, MAX_OFFLINE_HOURS);
            return rewardableHours * 3600.0;
        }

        private void ApplyOfflineTimeResult(TimeSpan offlineTimeSpan, double rewardableSeconds)
        {
            OfflineTimeSeconds = offlineTimeSpan.TotalSeconds;
            RewardableOfflineTimeSeconds = rewardableSeconds;
            IsOfflineTimeValid = true;

            string actualTimeString = FormatTimeSpan(offlineTimeSpan);
            TimeSpan rewardableTimeSpan = TimeSpan.FromSeconds(rewardableSeconds);
            string rewardableTimeString = FormatTimeSpan(rewardableTimeSpan);
            Log.Info(LogTags.Time, $"오프라인 시간 계산 완료: 실제 {actualTimeString}, 보상 가능 {rewardableTimeString}");
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }

        #endregion Private Methods
    }
}