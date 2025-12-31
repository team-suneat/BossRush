using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class GameData
    {
        /// <summary>
        /// 세이브 파일 버전 정보 (마이그레이션용)
        /// </summary>
        public int SaveVersion = 1;
        public VProfile Profile;

        public GameData()
        {
        }

        public void CreateProfile()
        {
            if (Profile == null)
            {
                Profile = VProfile.CreateDefault();
                Profile.OnLoadGameData();
                Profile.Statistics.RegisterGameStartTime();
                Debug.Log("프로필을 생성합니다.");
            }
        }

        public void DeleteCurrentProfile()
        {
            if (Profile != null)
            {
                Profile = null;
                Debug.Log("프로필을 삭제합니다.");
            }
        }

        public void DeleteProfile(int index)
        {
            Profile = null;

            Debug.Log($"{index + 1}번째 프로필을 삭제합니다.");
        }

        public static GameData CreateDefault()
        {
            Log.Info(LogTags.GameData, "새로운 게임 데이터를 생성합니다.");

            GameData defaultGameData = new GameData();
            defaultGameData.CreateProfile();

            return defaultGameData;
        }

        public VProfile GetSelectedProfile()
        {
            if (Profile != null)
            {
                return Profile;
            }
            return null;
        }

        public void OnLoadGameData()
        {
            Profile.OnLoadGameData();
        }
    }
}