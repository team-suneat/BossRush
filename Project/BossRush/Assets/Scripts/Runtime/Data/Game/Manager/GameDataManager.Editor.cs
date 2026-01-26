using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public partial class GameDataManager
    {
        public bool TryLoadFromBackup()
        {
#if UNITY_EDITOR
            string saveFilePath = GetSaveFilePath(0);
            GameData recoveredData = TryLoadFromBackupFiles(saveFilePath);

            if (recoveredData != null)
            {
                Data = recoveredData;
                Debug.Log("[에디터 전용] 타임스탬프 백업 파일에서 데이터 복구를 성공했습니다.");
                return true;
            }

            Debug.LogWarning("[에디터 전용] 백업 파일이 존재하지 않습니다.");
#endif
            return false;
        }

        public bool TryLoadFromAnyAvailableFile()
        {
#if UNITY_EDITOR
            // 1단계: 메인 세이브 파일 시도
            string saveFilePath = GetSaveFilePath(0);
            if (TryLoad(saveFilePath))
            {
                Debug.Log("[에디터 전용] 메인 세이브 파일에서 데이터를 성공적으로 불러왔습니다.");
                return true;
            }

            // 2단계: 타임스탬프 백업 파일에서 시도
            GameData recoveredData = TryLoadFromBackupFiles(saveFilePath);
            if (recoveredData != null)
            {
                Data = recoveredData;
                Debug.Log("[에디터 전용] 타임스탬프 백업 파일에서 데이터를 성공적으로 복구했습니다.");
                // 복구된 데이터를 메인 세이브 파일에 저장
                Save();
                return true;
            }

            Debug.LogError("[에디터 전용] 모든 세이브 파일과 백업에서 데이터 로드에 실패했습니다.");
#endif
            return false;
        }

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

        public void CheckAllSaveFilesMigrationStatus()
        {
#if UNITY_EDITOR
            Debug.Log("[에디터 전용] 모든 세이브 파일의 마이그레이션 상태를 점검합니다.");

            // 메인 세이브 파일
            string saveFilePath = GetSaveFilePath(0);
            if (File.Exists(saveFilePath))
            {
                Debug.Log("[에디터 전용] 메인 세이브 파일 분석:");
                AnalyzeSaveFileMigration(saveFilePath);
            }
            else
            {
                Debug.Log("[에디터 전용] 메인 세이브 파일이 존재하지 않습니다.");
            }

            // 타임스탬프 백업 파일들
            try
            {
                string[] backupFiles = GetAllBackupFilePaths();
                if (backupFiles.Length > 0)
                {
                    Debug.Log($"[에디터 전용] 타임스탬프 백업 파일 {backupFiles.Length}개 발견:");
                    foreach (string backupFile in backupFiles.Take(5)) // 최근 5개만 분석
                    {
                        Debug.Log($"[에디터 전용] 백업 파일 분석: {Path.GetFileName(backupFile)}");
                        AnalyzeSaveFileMigration(backupFile);
                    }
                }
                else
                {
                    Debug.Log("[에디터 전용] 타임스탬프 백업 파일이 존재하지 않습니다.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 백업 파일 검색 중 오류: {ex.Message}");
            }
#endif
        }

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

        public void LogAllSaveFileStatus()
        {
#if UNITY_EDITOR
            Debug.Log("[에디터 전용] ─── 세이브 파일 상태 점검 ───");

            // 메인 세이브 파일
            string filePath = GetSaveFilePath(0);
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                Debug.Log($"[에디터 전용] 메인 세이브 파일: 존재함 ({fileInfo.Length} bytes, {fileInfo.LastWriteTime})");
            }
            else
            {
                Debug.Log($"[에디터 전용] 메인 세이브 파일: 존재하지 않음 ({filePath})");
            }

            // 타임스탬프 백업 파일 정보 출력
            LogBackupFileInfo();
            Debug.Log("[에디터 전용] ─── 점검 완료 ───");
#endif
        }

        public bool RestoreFromBackup()
        {
#if UNITY_EDITOR
            string saveFilePath = GetSaveFilePath(0);
            GameData recoveredData = TryLoadFromBackupFiles(saveFilePath);

            if (recoveredData == null)
            {
                Debug.LogError("[에디터 전용] 복구할 백업 파일이 존재하지 않습니다.");
                return false;
            }

            try
            {
                Data = recoveredData;
                Save();
                Debug.Log("[에디터 전용] 가장 최근 백업 파일에서 메인 세이브 파일로 복구했습니다.");
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
                    Debug.LogError("[에디터 전용] JSON 형식: 유효하지 않음");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 진단 중 오류: {ex.Message}");
            }

            Debug.Log($"[에디터 전용] ─── 세이브 파일 진단 완료 ───");
#endif
        }

        public void DiagnoseAllSaveFiles()
        {
#if UNITY_EDITOR
            Debug.Log("[에디터 전용] ─── 모든 세이브 파일 진단 시작 ───");

            // 메인 세이브 파일 진단
            string saveFilePath = GetSaveFilePath(0);
            DiagnoseSaveFile(saveFilePath);

            // 타임스탬프 백업 파일들 진단
            try
            {
                var allBackupFiles = GetAllBackupFilePaths()
                    .Take(3) // 최근 3개만 진단
                    .ToArray();

                if (allBackupFiles.Length > 0)
                {
                    foreach (string backupFile in allBackupFiles)
                    {
                        DiagnoseSaveFile(backupFile);
                    }
                }
                else
                {
                    Debug.Log("[에디터 전용] 타임스탬프 백업 파일이 존재하지 않습니다.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[에디터 전용] 타임스탬프 백업 진단 중 오류: {ex.Message}");
            }

            Debug.Log("[에디터 전용] ─── 모든 세이브 파일 진단 완료 ───");
#endif
        }

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

        public void PrintSaveFileStatistics()
        {
#if UNITY_EDITOR
            Debug.Log("[에디터 전용] ─── 세이브 파일 통계 ───");

            try
            {
                string saveDirectory = Application.persistentDataPath;
                var allSaveFiles = Directory.GetFiles(saveDirectory, "*.json")
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

        private string AttemptContentRecovery(string content)
        {
            // 1. JSON 형식이면 그대로 반환
            if (IsValidJson(content))
            {
                return content;
            }

            // 2. 부분적 복구 시도 (JSON 부분만 추출)
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