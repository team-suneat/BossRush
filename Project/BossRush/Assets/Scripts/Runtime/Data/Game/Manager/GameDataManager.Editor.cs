using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// GameDataManager의 에디터 전용 복구 및 디버깅 기능들
    /// </summary>
    public partial class GameDataManager
    {
        /// <summary>
        /// [에디터 전용] 백업 파일에서 게임 데이터 복구를 시도합니다.
        /// </summary>
        public bool TryLoadFromBackup()
        {
#if UNITY_EDITOR
            string backupPath = GetBackupFilePath();
            Debug.Log($"[에디터 전용] 백업 파일에서 데이터 복구를 시도합니다. 경로: {backupPath}");

            if (File.Exists(backupPath))
            {
                return TryLoad(backupPath);
            }

            Debug.LogWarning("백업 파일이 존재하지 않습니다.");
#endif
            return false;
        }

        /// <summary>
        /// [에디터 전용] 모든 가능한 세이브 파일과 백업에서 로드를 시도합니다.
        /// </summary>
        public bool TryLoadFromAnyAvailableFile()
        {
#if UNITY_EDITOR
            // 1단계: 정상적인 세이브 파일들 시도
            for (int i = 0; i < GAME_DATA_COUNT; i++)
            {
                string saveFilePath = GetSaveFilePath(i);
                if (TryLoad(saveFilePath))
                {
                    Debug.Log($"[에디터 전용] 세이브 파일 {i + 1}에서 데이터를 성공적으로 불러왔습니다.");
                    return true;
                }
            }

            // 2단계: 백업 파일에서 시도
            if (TryLoadFromBackup())
            {
                Debug.Log("[에디터 전용] 백업 파일에서 데이터를 성공적으로 복구했습니다.");
                // 복구된 데이터를 메인 세이브 파일에 저장
                Save();
                return true;
            }

            Debug.LogError("[에디터 전용] 모든 세이브 파일과 백업에서 데이터 로드에 실패했습니다.");
#endif
            return false;
        }

        /// <summary>
        /// [에디터 전용] 세이브 파일의 마이그레이션 정보를 분석합니다.
        /// </summary>
        public void AnalyzeSaveFileMigration(string filePath)
        {
#if UNITY_EDITOR
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[에디터 전용] 파일이 존재하지 않습니다: {filePath}");
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(filePath);
                LogMigrationInfo(jsonContent);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 파일 분석 중 오류: {ex.Message}");
            }
#endif
        }

        /// <summary>
        /// [에디터 전용] 모든 세이브 파일의 마이그레이션 상태를 점검합니다.
        /// </summary>
        public void CheckAllSaveFilesMigrationStatus()
        {
#if UNITY_EDITOR
            Debug.Log("[에디터 전용] 모든 세이브 파일의 마이그레이션 상태를 점검합니다.");

            // 메인 세이브 파일들
            for (int i = 0; i < GAME_DATA_COUNT; i++)
            {
                string saveFilePath = GetSaveFilePath(i);
                if (File.Exists(saveFilePath))
                {
                    Debug.Log($"[에디터 전용] 세이브 파일 {i + 1} 분석:");
                    AnalyzeSaveFileMigration(saveFilePath);
                }
                else
                {
                    Debug.Log($"[에디터 전용] 세이브 파일 {i + 1}이 존재하지 않습니다.");
                }
            }

            // 백업 파일
            string backupFilePath = GetBackupFilePath();
            if (File.Exists(backupFilePath))
            {
                Debug.Log("[에디터 전용] 백업 파일 분석:");
                AnalyzeSaveFileMigration(backupFilePath);
            }
            else
            {
                Debug.Log("[에디터 전용] 백업 파일이 존재하지 않습니다.");
            }
#endif
        }

        /// <summary>
        /// [에디터 전용] 세이브 파일을 현재 버전으로 마이그레이션합니다.
        /// </summary>
        public bool MigrateSaveFile(string filePath)
        {
#if UNITY_EDITOR
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[에디터 전용] 파일이 존재하지 않습니다: {filePath}");
                return false;
            }

            try
            {
                string jsonContent = File.ReadAllText(filePath);

                // 마이그레이션 가능 여부 확인
                if (!CanMigrate(jsonContent))
                {
                    Debug.LogError($"[에디터 전용] 마이그레이션할 수 없는 파일입니다: {filePath}");
                    return false;
                }

                // 마이그레이션 수행
                GameData migratedData = MigrateAndLoad(jsonContent);
                if (migratedData != null)
                {
                    // 마이그레이션된 데이터를 다시 저장
                    Data = migratedData;
                    Save();

                    Debug.Log($"[에디터 전용] 마이그레이션 성공: {filePath}");
                    return true;
                }
                else
                {
                    Debug.LogError($"[에디터 전용] 마이그레이션 실패: {filePath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 마이그레이션 중 오류: {ex.Message}");
                return false;
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// [에디터 전용] 모든 세이브 파일 상태를 확인합니다.
        /// </summary>
        public void LogAllSaveFileStatus()
        {
#if UNITY_EDITOR
            Debug.Log("[에디터 전용] ─── 세이브 파일 상태 점검 ───");

            for (int i = 0; i < GAME_DATA_COUNT; i++)
            {
                string filePath = GetSaveFilePath(i);
                if (File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    Debug.Log($"[에디터 전용] 세이브 파일 {i + 1}: 존재함 ({fileInfo.Length} bytes, {fileInfo.LastWriteTime})");
                }
                else
                {
                    Debug.Log($"[에디터 전용] 세이브 파일 {i + 1}: 존재하지 않음 ({filePath})");
                }
            }

            // 백업 파일 정보 출력
            LogBackupFileInfo();
            Debug.Log("[에디터 전용] ─── 점검 완료 ───");
#endif
        }

        /// <summary>
        /// [에디터 전용] 백업 파일을 메인 세이브 파일로 복사합니다.
        /// </summary>
        public bool RestoreFromBackup()
        {
#if UNITY_EDITOR
            string backupPath = GetBackupFilePath();

            if (!File.Exists(backupPath))
            {
                Debug.LogError("[에디터 전용] 백업 파일이 존재하지 않습니다.");
                return false;
            }

            try
            {
                string mainSavePath = GetSaveFilePath(0);
                File.Copy(backupPath, mainSavePath, true);
                Debug.Log($"[에디터 전용] 백업 파일을 메인 세이브 파일로 복사했습니다: {backupPath} ▶ {mainSavePath}");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[에디터 전용] 백업 복구 실패: {ex.Message}");
                return false;
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// [에디터 전용] 세이브 파일의 상세 진단을 수행합니다.
        /// </summary>
        /// <param name="filePath">진단할 파일 경로</param>
        public void DiagnoseSaveFile(string filePath)
        {
#if UNITY_EDITOR
            Debug.Log($"[에디터 전용] ─── 세이브 파일 진단 시작: {Path.GetFileName(filePath)} ───");

            if (!File.Exists(filePath))
            {
                Debug.LogError($"[에디터 전용] 파일이 존재하지 않습니다: {filePath}");
                return;
            }

            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                Debug.Log($"[에디터 전용] 파일 크기: {fileInfo.Length:N0} bytes");
                Debug.Log($"[에디터 전용] 생성 시간: {fileInfo.CreationTime}");
                Debug.Log($"[에디터 전용] 수정 시간: {fileInfo.LastWriteTime}");

                // 파일 내용 읽기 시도
                string content = File.ReadAllText(filePath);
                Debug.Log($"[에디터 전용] 파일 내용 길이: {content.Length} characters");

                // JSON 형식 검증
                if (IsValidJson(content))
                {
                    Debug.Log("[에디터 전용] JSON 형식: 유효함");

                    // 마이그레이션 정보 분석
                    LogMigrationInfo(content);
                }
                else
                {
                    Debug.LogWarning("[에디터 전용] JSON 형식: 유효하지 않음");

                    // 암호화된 파일인지 확인
                    if (IsEncryptedFile(content))
                    {
                        Debug.Log("[에디터 전용] 파일 상태: 암호화됨");

                        // 복호화 시도
                        string decryptedContent = TryDecryptContent(content);
                        if (!string.IsNullOrEmpty(decryptedContent))
                        {
                            Debug.Log("[에디터 전용] 복호화: 성공");
                            if (IsValidJson(decryptedContent))
                            {
                                Debug.Log("[에디터 전용] 복호화된 JSON: 유효함");
                                LogMigrationInfo(decryptedContent);
                            }
                            else
                            {
                                Debug.LogError("[에디터 전용] 복호화된 JSON: 유효하지 않음");
                            }
                        }
                        else
                        {
                            Debug.LogError("[에디터 전용] 복호화: 실패");
                        }
                    }
                    else
                    {
                        Debug.LogError("[에디터 전용] 파일 상태: 손상됨 또는 알 수 없는 형식");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 진단 중 오류: {ex.Message}");
            }

            Debug.Log($"[에디터 전용] ─── 세이브 파일 진단 완료 ───");
#endif
        }

        /// <summary>
        /// [에디터 전용] 모든 세이브 파일에 대해 상세 진단을 수행합니다.
        /// </summary>
        public void DiagnoseAllSaveFiles()
        {
#if UNITY_EDITOR
            Debug.Log("[에디터 전용] ─── 모든 세이브 파일 진단 시작 ───");

            // 메인 세이브 파일들 진단
            for (int i = 0; i < GAME_DATA_COUNT; i++)
            {
                string saveFilePath = GetSaveFilePath(i);
                DiagnoseSaveFile(saveFilePath);
            }

            // 백업 파일들 진단
            string backupPath = GetBackupFilePath();
            if (File.Exists(backupPath))
            {
                DiagnoseSaveFile(backupPath);
            }
            else
            {
                Debug.Log("[에디터 전용] 백업 파일이 존재하지 않습니다.");
            }

            // 타임스탬프 백업 파일들 진단
            try
            {
                var allBackupFiles = GetAllBackupFilePaths()
                    .Take(3) // 최근 3개만 진단
                    .ToArray();

                foreach (string backupFile in allBackupFiles)
                {
                    DiagnoseSaveFile(backupFile);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 타임스탬프 백업 진단 중 오류: {ex.Message}");
            }

            Debug.Log("[에디터 전용] ─── 모든 세이브 파일 진단 완료 ───");
#endif
        }

        /// <summary>
        /// [에디터 전용] 손상된 세이브 파일을 복구 시도합니다.
        /// </summary>
        /// <param name="filePath">복구할 파일 경로</param>
        /// <returns>복구 성공 여부</returns>
        public bool AttemptFileRecovery(string filePath)
        {
#if UNITY_EDITOR
            Debug.Log($"[에디터 전용] ─── 파일 복구 시도: {Path.GetFileName(filePath)} ───");

            if (!File.Exists(filePath))
            {
                Debug.LogError($"[에디터 전용] 파일이 존재하지 않습니다: {filePath}");
                return false;
            }

            try
            {
                // 1. 백업 생성
                string backupPath = filePath + ".recovery_backup";
                File.Copy(filePath, backupPath, true);
                Debug.Log($"[에디터 전용] 복구 백업 생성: {backupPath}");

                // 2. 파일 내용 읽기
                string content = File.ReadAllText(filePath);

                // 3. 복구 시도
                string recoveredContent = AttemptContentRecovery(content);

                if (!string.IsNullOrEmpty(recoveredContent))
                {
                    // 4. 복구된 내용으로 파일 덮어쓰기
                    File.WriteAllText(filePath, recoveredContent);
                    Debug.Log($"[에디터 전용] 파일 복구 성공: {filePath}");
                    return true;
                }
                else
                {
                    Debug.LogError($"[에디터 전용] 파일 복구 실패: {filePath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 복구 중 오류: {ex.Message}");
                return false;
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// [에디터 전용] 세이브 파일을 안전하게 삭제합니다.
        /// </summary>
        /// <param name="filePath">삭제할 파일 경로</param>
        /// <returns>삭제 성공 여부</returns>
        public bool SafeDeleteSaveFile(string filePath)
        {
#if UNITY_EDITOR
            Debug.Log($"[에디터 전용] ─── 안전 삭제: {Path.GetFileName(filePath)} ───");

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[에디터 전용] 파일이 이미 존재하지 않습니다: {filePath}");
                return true;
            }

            try
            {
                // 1. 삭제 전 백업 생성
                string backupPath = filePath + ".deletion_backup";
                File.Copy(filePath, backupPath, true);
                Debug.Log($"[에디터 전용] 삭제 백업 생성: {backupPath}");

                // 2. 파일 삭제
                File.Delete(filePath);
                Debug.Log($"[에디터 전용] 파일 삭제 성공: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 삭제 중 오류: {ex.Message}");
                return false;
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// [에디터 전용] 세이브 파일 통계를 출력합니다.
        /// </summary>
        public void PrintSaveFileStatistics()
        {
#if UNITY_EDITOR
            Debug.Log("[에디터 전용] ─── 세이브 파일 통계 ───");

            try
            {
                string saveDirectory = Application.persistentDataPath;
                var allSaveFiles = Directory.GetFiles(saveDirectory, "*.dat")
                    .Where(f => Path.GetFileName(f).Contains(Application.productName))
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.LastWriteTime)
                    .ToArray();

                Debug.Log($"[에디터 전용] 총 세이브 파일 수: {allSaveFiles.Length}개");

                var mainSaves = allSaveFiles.Where(f => !Path.GetFileName(f.Name).Contains("Backup")).ToArray();
                var backupSaves = allSaveFiles.Where(f => Path.GetFileName(f.Name).Contains("Backup")).ToArray();

                Debug.Log($"[에디터 전용] 메인 세이브 파일: {mainSaves.Length}개");
                Debug.Log($"[에디터 전용] 백업 파일: {backupSaves.Length}개");

                if (allSaveFiles.Length > 0)
                {
                    var totalSize = allSaveFiles.Sum(f => f.Length);
                    Debug.Log($"[에디터 전용] 총 용량: {totalSize:N0} bytes ({totalSize / 1024.0 / 1024.0:F2} MB)");

                    var oldestFile = allSaveFiles.Last();
                    var newestFile = allSaveFiles.First();
                    Debug.Log($"[에디터 전용] 가장 오래된 파일: {oldestFile.Name} ({oldestFile.CreationTime:yyyy-MM-dd HH:mm:ss})");
                    Debug.Log($"[에디터 전용] 가장 최근 파일: {newestFile.Name} ({newestFile.LastWriteTime:yyyy-MM-dd HH:mm:ss})");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 통계 출력 중 오류: {ex.Message}");
            }

            Debug.Log("[에디터 전용] ─── 통계 완료 ───");
#endif
        }

        #region Private Helper Methods

        /// <summary>
        /// JSON 형식이 유효한지 확인합니다.
        /// </summary>
        private bool IsValidJson(string content)
        {
            try
            {
                JsonConvert.DeserializeObject(content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 파일이 암호화되었는지 확인합니다.
        /// </summary>
        private bool IsEncryptedFile(string content)
        {
            // 간단한 암호화 여부 판단 (실제로는 더 정교한 로직 필요)
            return !content.TrimStart().StartsWith("{") && !content.TrimStart().StartsWith("[");
        }

        /// <summary>
        /// 암호화된 내용을 복호화 시도합니다.
        /// </summary>
        private string TryDecryptContent(string content)
        {
            try
            {
                string symmetricKey = AES.Encrypt(GameSymmetricIdentifier(), "pub");
                return AES.Decrypt(content, symmetricKey);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 손상된 내용을 복구 시도합니다.
        /// </summary>
        private string AttemptContentRecovery(string content)
        {
            // 1. JSON 형식이면 그대로 반환
            if (IsValidJson(content))
            {
                return content;
            }

            // 2. 암호화된 파일이면 복호화 시도
            if (IsEncryptedFile(content))
            {
                string decrypted = TryDecryptContent(content);
                if (!string.IsNullOrEmpty(decrypted) && IsValidJson(decrypted))
                {
                    return decrypted;
                }
            }

            // 3. 부분적 복구 시도 (JSON 부분만 추출)
            var jsonMatch = System.Text.RegularExpressions.Regex.Match(content, @"\{.*\}", System.Text.RegularExpressions.RegexOptions.Singleline);
            if (jsonMatch.Success && IsValidJson(jsonMatch.Value))
            {
                Debug.Log("[에디터 전용] 부분적 JSON 복구 성공");
                return jsonMatch.Value;
            }

            return null;
        }

        #endregion Private Helper Methods
    }
}