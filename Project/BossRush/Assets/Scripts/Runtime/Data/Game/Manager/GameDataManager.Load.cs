using System.IO;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public partial class GameDataManager
    {
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

        private GameData LoadGameData(string filePath)
        {
            // 로드 전 임시 파일 처리 (파일이 없어도 임시 파일은 처리)
            ProcessOrphanedTempFiles(filePath);

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

        public void OnLoadGameData()
        {
            if (Data != null)
            {
                Data.OnLoadGameData();
            }
        }

        public void LoadGameDataWithRecovery()
        {
            string saveFilePath = GetSaveFilePath(0);

            if (TryLoad(saveFilePath))
            {
                OnLoadGameData();
            }
            else
            {
                // 타임스탬프 백업 파일에서 복구 시도
                GameData recoveredData = TryLoadFromBackupFiles(saveFilePath);
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