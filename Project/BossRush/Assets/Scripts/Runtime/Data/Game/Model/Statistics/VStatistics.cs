using System;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public partial class VStatistics
    {
        // 게임 시작 시간
        public DateTime GameStartTime;

        // 마지막 게임 저장 시간
        public DateTime LastSaveTime;

        public void OnLoadGameData()
        {
        }

        public static VStatistics CreateDefault()
        {
            return new VStatistics();
        }
    }
}