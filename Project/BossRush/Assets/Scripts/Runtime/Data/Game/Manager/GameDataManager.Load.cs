using System.IO;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// GameDataManager의 로드 로직을 담당하는 partial 클래스
    /// </summary>
    public partial class GameDataManager
    {
        /// <summary>
        /// 세이브 파일에서 데이터를 로드합니다.
        /// </summary>
        /// <param name="saveFilePath">세이브 파일 경로</param>
        /// <returns>로드 성공 여부</returns>
        public bool TryLoad(string saveFilePath)
        {
            GameData gameData = LoadGameData(saveFilePath);
            if (gameData != null)
            {
                Data = gameData;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 게임 데이터를 로드합니다.
        /// </summary>
        /// <param name="filePath">파일 경로</param>
        /// <returns>로드된 GameData</returns>
        private GameData LoadGameData(string filePath)
        {
            GameData gameData;
            string chunk;

            if (File.Exists(filePath))
            {
                chunk = Read(filePath);
                if (string.IsNullOrEmpty(chunk))
                {
                    Debug.LogError($"해당 세이브 파일 경로({filePath})에서 읽어올 수 없습니다.");
                }
                else
                {
                    gameData = Deserialize(chunk);
                    if (gameData != null)
                    {
                        Debug.Log($"저장된 게임 데이터를 불러옵니다. File Path: {filePath}");
                        return gameData;
                    }
                    else
                    {
                        Debug.LogWarning($"저장된 게임 데이터를 불러오는데 실패했습니다. 비상 백업을 생성합니다. File Path: {filePath}");
                        SaveBackupWithTimestamp(chunk, filePath);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"세이브 파일 경로({filePath})에서 파일을 찾을 수 없습니다. 세이브 파일을 불러오지 못합니다.");
            }

            return null;
        }

        /// <summary>
        /// 로드된 게임 데이터에 대한 후처리를 수행합니다.
        /// </summary>
        public void OnLoadGameData()
        {
            if (Data != null)
            {
                Data.OnLoadGameData();
            }
        }

        /// <summary>
        /// 게임 데이터를 복구 시스템을 포함하여 로드합니다.
        /// </summary>
        public void LoadGameDataWithRecovery()
        {
            string saveFile1Path = GetSaveFilePath(0);
            string saveFile2Path = GetSaveFilePath(1);

            if (TryLoad(saveFile1Path))
            {
                OnLoadGameData();
            }
            else if (TryLoad(saveFile2Path))
            {
                OnLoadGameData();
                Save(); // 복구된 데이터를 메인 세이브에 저장
            }
            else
            {
                // 백업 파일에서 복구 시도
                GameData recoveredData = TryLoadFromBackupFiles(saveFile1Path);
                if (recoveredData != null)
                {
                    Data = recoveredData;
                    OnLoadGameData();
                    Save();
                }
                else
                {
                    // 모든 복구 시도 실패 시 새 데이터 생성
                    Data = GameData.CreateDefault();
                    Save();
                }
            }
        }
    }
}