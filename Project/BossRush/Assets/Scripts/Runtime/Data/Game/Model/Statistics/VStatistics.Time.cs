using System;
using System.Globalization;

namespace TeamSuneat.Data
{
    public partial class VStatistics
    {
        #region Get

        public TimeSpan GetTotalPlayTime()
        {
            TimeSpan totalPlayTime = LastSaveTime - GameStartTime;
            return totalPlayTime;
        }

        public string GetLastSaveTimeString()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            return LastSaveTime.ToString("yyyy/MM/dd HH:mm:ss", culture);
        }

        public string GetTotalPlayTimeString()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            return GetTotalPlayTime().ToString(@"hh\:mm", culture);
        }

        #endregion Get

        public void RegisterGameStartTime()
        {
            GameStartTime = DateTime.Now;
        }

        /// <summary>
        /// 마지막 저장 시간을 등록합니다.
        /// </summary>
        public void RegisterLastSaveTime()
        {
            LastSaveTime = DateTime.Now;
        }
    }
}