using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public partial class GameDataManager
    {
        #region 백업 시스템 상수

        private const string BACKUP_FILE_PREFIX = "Backup_";
        private const string BACKUP_FOLDER_NAME = "Backup";

        #endregion 백업 시스템 상수

        #region 백업 생성

        private string GetBackupFolderPath()
        {
            string backupFolder = Path.Combine(Application.persistentDataPath, BACKUP_FOLDER_NAME);

            if (!Directory.Exists(backupFolder))
            {
                try
                {
                    Directory.CreateDirectory(backupFolder);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"백업 폴더 생성 실패: {ex.Message}");
                }
            }

            return backupFolder;
        }

        private void SaveBackupWithTimestamp(string chunk, string originalFilePath)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFilePath = GetBackupFilePathWithTimestamp(timestamp);

                if (Write(backupFilePath, chunk))
                {
                    Debug.Log($"비상 백업 생성: {backupFilePath} (원본: {Path.GetFileName(originalFilePath)})");
                }
                else
                {
                    Debug.LogError($"비상 백업 생성 실패: {backupFilePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"비상 백업 생성 중 오류: {ex.Message}");
            }
        }

        private string GetBackupFilePathWithTimestamp(string timestamp)
        {
            string backupFolder = GetBackupFolderPath();
            return Path.Combine(backupFolder, $"{BACKUP_FILE_PREFIX}{timestamp}.json");
        }

        #endregion 백업 생성

        #region 백업 파일 검색

        private string[] FindTimestampedBackupFiles(string saveDirectory)
        {
            return Directory.GetFiles(saveDirectory, $"{BACKUP_FILE_PREFIX}*.json")
                .Where(f => Path.GetFileName(f).Contains("_"))
                .OrderByDescending(f => f)
                .ToArray();
        }

        private string[] FindLegacyBackupFiles(string saveDirectory)
        {
            return Directory.GetFiles(saveDirectory, $"{BACKUP_FILE_PREFIX}{Application.productName}.json")
                .OrderByDescending(f => f)
                .ToArray();
        }

        protected string[] GetAllBackupFilePaths()
        {
            try
            {
                string backupFolder = GetBackupFolderPath();
                if (!Directory.Exists(backupFolder))
                {
                    return Array.Empty<string>();
                }

                string[] legacyBackups = FindLegacyBackupFiles(backupFolder);
                string[] timestampedBackups = FindTimestampedBackupFiles(backupFolder);

                return legacyBackups.Concat(timestampedBackups).ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 검색 중 오류: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        protected FileInfo[] GetAllBackupFileInfos()
        {
            try
            {
                // GetAllBackupFilePaths() 결과를 재사용하여 중복 검색 방지
                string[] backupPaths = GetAllBackupFilePaths();
                if (backupPaths.Length == 0)
                {
                    return Array.Empty<FileInfo>();
                }

                return backupPaths
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 정보 검색 중 오류: {ex.Message}");
                return Array.Empty<FileInfo>();
            }
        }

        #endregion 백업 파일 검색

        #region 백업 복구

        private GameData LoadGameDataFromBackup(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    Debug.LogError($"백업 파일이 존재하지 않습니다: {backupFilePath}");
                    return null;
                }

                string chunk = File.ReadAllText(backupFilePath);
                if (string.IsNullOrEmpty(chunk))
                {
                    Debug.LogError($"백업 파일이 비어있습니다: {backupFilePath}");
                    return null;
                }

                return MigrateAndLoad(chunk);
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 로드 중 오류: {ex.Message}, 경로: {backupFilePath}");
                return null;
            }
        }

        private bool SaveRecoveredData(GameData recoveredData)
        {
            try
            {
                string mainSavePath = GetSaveFilePath(0);
                string serializedData = JsonConvert.SerializeObject(recoveredData, Formatting.Indented, _serializeSettings);

                if (string.IsNullOrEmpty(serializedData))
                {
                    Debug.LogError("백업 데이터 직렬화 실패");
                    return false;
                }

                return Write(mainSavePath, serializedData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"복구 데이터 저장 중 오류: {ex.Message}");
                return false;
            }
        }

        public GameData TryLoadFromBackupFiles(string originalFilePath)
        {
            try
            {
                string[] backupFiles = GetAllBackupFilePaths();
                Debug.Log($"백업 파일 {backupFiles.Length}개 발견 (원본: {Path.GetFileName(originalFilePath)})");

                foreach (string backupFile in backupFiles)
                {
                    string fileName = Path.GetFileName(backupFile);
                    Debug.Log($"백업 파일에서 복구 시도: {fileName}");

                    GameData recoveredData = LoadGameDataFromBackup(backupFile);
                    if (recoveredData != null)
                    {
                        Debug.Log($"백업 파일에서 복구 성공: {fileName}");
                        return recoveredData;
                    }
                }

                Debug.LogWarning("모든 백업 파일에서 복구에 실패했습니다.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 복구 중 오류: {ex.Message}");
                return null;
            }
        }

        public void LogBackupFileInfo()
        {
            try
            {
                FileInfo[] backupFiles = GetAllBackupFileInfos();
                Debug.Log($"백업 파일 정보 (총 {backupFiles.Length}개):");

                foreach (FileInfo file in backupFiles)
                {
                    Debug.Log($"  - {file.Name}");
                    Debug.Log($"    크기: {file.Length:N0} bytes");
                    Debug.Log($"    생성일: {file.CreationTime:yyyy-MM-dd HH:mm:ss}");
                    Debug.Log($"    수정일: {file.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 정보 출력 중 오류: {ex.Message}");
            }
        }

        public bool RestoreFromBackup(string backupFileName)
        {
            try
            {
                string backupFolder = GetBackupFolderPath();
                string backupFilePath = Path.Combine(backupFolder, backupFileName);

                GameData recoveredData = LoadGameDataFromBackup(backupFilePath);
                if (recoveredData == null)
                {
                    Debug.LogError($"백업 파일에서 GameData 로드 실패: {backupFileName}");
                    return false;
                }

                if (!SaveRecoveredData(recoveredData))
                {
                    Debug.LogError($"백업 데이터 저장 실패: {backupFileName}");
                    return false;
                }

                Debug.Log($"백업에서 복구 성공: {backupFileName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 복구 중 오류: {ex.Message}");
                return false;
            }
        }

        #endregion 백업 복구

    }
}
