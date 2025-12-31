using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// GameDataManager의 백업 시스템 전체를 담당하는 partial 클래스
    /// </summary>
    public partial class GameDataManager
    {
        #region 백업 시스템 상수

        private const int MAX_BACKUP_COUNT = 10;
        private const string BACKUP_FILE_PREFIX = "Backup_";

        #endregion 백업 시스템 상수

        #region 백업 생성

        /// <summary>
        /// 타임스탬프가 포함된 비상 백업을 생성합니다.
        /// </summary>
        /// <param name="chunk">백업할 데이터 chunk</param>
        /// <param name="originalFilePath">원본 파일 경로 (로그용)</param>
        private void SaveBackupWithTimestamp(string chunk, string originalFilePath)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFilePath = GetBackupFilePathWithTimestamp(timestamp);
                string chunkAES = Encrypt(chunk);

                if (Write(backupFilePath, chunkAES))
                {
                    Debug.Log($"비상 백업 생성: {backupFilePath} (원본: {Path.GetFileName(originalFilePath)})");
                    CleanupOldBackups();
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

        /// <summary>
        /// 타임스탬프가 포함된 백업 파일 경로를 반환합니다.
        /// </summary>
        /// <param name="timestamp">타임스탬프</param>
        /// <returns>백업 파일 경로</returns>
        private string GetBackupFilePathWithTimestamp(string timestamp)
        {
            return Path.Combine(Application.persistentDataPath, $"{BACKUP_FILE_PREFIX}{timestamp}.dat");
        }

        #endregion 백업 생성

        #region 백업 파일 검색

        /// <summary>
        /// 타임스탬프가 포함된 백업 파일 패턴을 검색합니다.
        /// </summary>
        /// <param name="saveDirectory">검색할 디렉토리</param>
        /// <returns>타임스탬프 백업 파일 경로 배열</returns>
        private string[] FindTimestampedBackupFiles(string saveDirectory)
        {
            return Directory.GetFiles(saveDirectory, $"{BACKUP_FILE_PREFIX}*.dat")
                .Where(f => Path.GetFileName(f).Contains("_"))
                .OrderByDescending(f => f)
                .ToArray();
        }

        /// <summary>
        /// 레거시 백업 파일 패턴을 검색합니다.
        /// </summary>
        /// <param name="saveDirectory">검색할 디렉토리</param>
        /// <returns>레거시 백업 파일 경로 배열</returns>
        private string[] FindLegacyBackupFiles(string saveDirectory)
        {
            return Directory.GetFiles(saveDirectory, $"{BACKUP_FILE_PREFIX}{Application.productName}.dat")
                .OrderByDescending(f => f)
                .ToArray();
        }

        /// <summary>
        /// 모든 백업 파일 경로를 반환합니다.
        /// </summary>
        /// <returns>백업 파일 경로 배열 (레거시 파일 우선, 타임스탬프 파일 후순)</returns>
        protected string[] GetAllBackupFilePaths()
        {
            try
            {
                string saveDirectory = Application.persistentDataPath;
                if (!Directory.Exists(saveDirectory))
                {
                    return Array.Empty<string>();
                }

                string[] legacyBackups = FindLegacyBackupFiles(saveDirectory);
                string[] timestampedBackups = FindTimestampedBackupFiles(saveDirectory);

                return legacyBackups.Concat(timestampedBackups).ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 검색 중 오류: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// 모든 백업 파일 정보를 FileInfo 배열로 반환합니다.
        /// </summary>
        /// <returns>백업 파일 정보 배열 (생성일 기준 내림차순)</returns>
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

        /// <summary>
        /// 백업 파일에서 GameData를 로드합니다.
        /// </summary>
        /// <param name="backupFilePath">백업 파일 경로</param>
        /// <returns>로드된 GameData, 실패 시 null</returns>
        private GameData LoadGameDataFromBackup(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    Debug.LogError($"백업 파일이 존재하지 않습니다: {backupFilePath}");
                    return null;
                }

                string encryptedChunk = File.ReadAllText(backupFilePath);
                if (string.IsNullOrEmpty(encryptedChunk))
                {
                    Debug.LogError($"백업 파일이 비어있습니다: {backupFilePath}");
                    return null;
                }

                string decryptedChunk = Decrypt(encryptedChunk);
                if (string.IsNullOrEmpty(decryptedChunk))
                {
                    Debug.LogError($"백업 파일 복호화 실패: {backupFilePath}");
                    return null;
                }

                return MigrateAndLoad(decryptedChunk);
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 로드 중 오류: {ex.Message}, 경로: {backupFilePath}");
                return null;
            }
        }

        /// <summary>
        /// 복구된 GameData를 메인 세이브 파일에 저장합니다.
        /// </summary>
        /// <param name="recoveredData">복구된 GameData</param>
        /// <returns>저장 성공 여부</returns>
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

                string encryptedData = TryApplyAES() ? Encrypt(serializedData) : serializedData;
                if (string.IsNullOrEmpty(encryptedData))
                {
                    Debug.LogError("백업 데이터 암호화 실패");
                    return false;
                }

                return Write(mainSavePath, encryptedData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"복구 데이터 저장 중 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 모든 백업 파일에서 데이터 복구를 시도합니다.
        /// </summary>
        /// <param name="originalFilePath">원본 파일 경로 (로그용)</param>
        /// <returns>복구된 GameData 객체</returns>
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

        /// <summary>
        /// 백업 파일 정보를 로그로 출력합니다.
        /// </summary>
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

        /// <summary>
        /// 특정 백업 파일에서 데이터를 복구합니다.
        /// </summary>
        /// <param name="backupFileName">백업 파일명</param>
        /// <returns>복구 성공 여부</returns>
        public bool RestoreFromBackup(string backupFileName)
        {
            try
            {
                string backupFilePath = Path.Combine(Application.persistentDataPath, backupFileName);

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

        #region 백업 정리

        /// <summary>
        /// 단일 백업 파일을 안전하게 삭제합니다.
        /// </summary>
        /// <param name="file">삭제할 파일 정보</param>
        private void DeleteBackupFile(FileInfo file)
        {
            try
            {
                File.Delete(file.FullName);
                Debug.Log($"오래된 백업 파일 삭제: {file.Name}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"백업 파일 삭제 실패: {file.Name}, 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 오래된 백업 파일들을 정리합니다.
        /// </summary>
        private void CleanupOldBackups()
        {
            try
            {
                FileInfo[] backupFiles = GetAllBackupFileInfos();

                if (backupFiles.Length > MAX_BACKUP_COUNT)
                {
                    IEnumerable<FileInfo> filesToDelete = backupFiles.Skip(MAX_BACKUP_COUNT);
                    foreach (FileInfo file in filesToDelete)
                    {
                        DeleteBackupFile(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"백업 파일 정리 중 오류: {ex.Message}");
            }
        }

        #endregion 백업 정리
    }
}
