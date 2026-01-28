using System;
using System.IO;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public partial class GameDataManager
    {
        private static string SaveFilePathFormat { get; set; }

        public static void SetSaveFilePath()
        {
            if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                SaveFilePathFormat = $"{Application.persistentDataPath}/{Application.productName}{"{0}"}_Dev.json";
            }
            else
            {
                SaveFilePathFormat = $"{Application.persistentDataPath}/{Application.productName}{"{0}"}.json";
            }
        }

        public static string GetSaveFilePath(int index)
        {
            if (string.IsNullOrEmpty(SaveFilePathFormat))
            {
                SetSaveFilePath();
            }

            return string.Format(SaveFilePathFormat, index + 1);
        }

        private string Read(string saveFilePath)
        {
            // 로드 전 임시 파일 처리
            ProcessOrphanedTempFiles(saveFilePath);
            return File.ReadAllText(saveFilePath);
        }

        private void ProcessOrphanedTempFiles(string saveFilePath)
        {
            try
            {
                string saveDirectory = Path.GetDirectoryName(saveFilePath);
                if (string.IsNullOrEmpty(saveDirectory) || !Directory.Exists(saveDirectory))
                {
                    return;
                }

                string fileName = Path.GetFileNameWithoutExtension(saveFilePath);
                string fileExtension = Path.GetExtension(saveFilePath);
                string searchPattern = $"{fileName}{fileExtension}.*.tmp";

                string[] tempFiles = Directory.GetFiles(saveDirectory, searchPattern);
                if (tempFiles.Length == 0)
                {
                    return;
                }

                Debug.LogWarning($"임시 파일 {tempFiles.Length}개 발견. 백업 파일로 변환합니다.");

                foreach (string tempFile in tempFiles)
                {
                    try
                    {
                        // 임시 파일 내용 검증
                        if (!File.Exists(tempFile))
                        {
                            continue;
                        }

                        FileInfo fileInfo = new FileInfo(tempFile);
                        if (fileInfo.Length == 0)
                        {
                            Debug.LogWarning($"빈 임시 파일 삭제: {Path.GetFileName(tempFile)}");
                            File.Delete(tempFile);
                            continue;
                        }

                        // 임시 파일 내용 읽기
                        string chunk = File.ReadAllText(tempFile);
                        if (string.IsNullOrEmpty(chunk))
                        {
                            Debug.LogWarning($"읽을 수 없는 임시 파일 삭제: {Path.GetFileName(tempFile)}");
                            File.Delete(tempFile);
                            continue;
                        }

                        // 타임스탬프 추출 (파일명에서)
                        string tempFileName = Path.GetFileName(tempFile);
                        string timestamp = ExtractTimestampFromTempFileName(tempFileName);
                        if (string.IsNullOrEmpty(timestamp))
                        {
                            // 타임스탬프를 추출할 수 없으면 파일 수정 시간 사용
                            timestamp = fileInfo.LastWriteTime.ToString("yyyyMMdd_HHmmss");
                        }

                        // 백업 파일로 변환
                        string backupPath = GetBackupFilePathWithTimestamp(timestamp);
                        File.Move(tempFile, backupPath);
                        Debug.Log($"임시 파일을 백업으로 변환: {Path.GetFileName(tempFile)} -> {Path.GetFileName(backupPath)}");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"임시 파일 처리 실패 ({Path.GetFileName(tempFile)}): {ex.Message}");
                        // 처리 실패한 임시 파일은 삭제 시도
                        try
                        {
                            if (File.Exists(tempFile))
                            {
                                File.Delete(tempFile);
                            }
                        }
                        catch
                        {
                            // 삭제 실패는 무시
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"임시 파일 처리 중 오류: {ex.Message}");
            }
        }

        private string ExtractTimestampFromTempFileName(string tempFileName)
        {
            // 파일명 형식: {원본파일명}.{타임스탬프}.tmp
            // 예: BossRush1_Dev.json.20250126_123456.tmp
            int lastDotIndex = tempFileName.LastIndexOf('.');
            if (lastDotIndex < 0)
            {
                return null;
            }

            int secondLastDotIndex = tempFileName.LastIndexOf('.', lastDotIndex - 1);
            if (secondLastDotIndex < 0)
            {
                return null;
            }

            string timestamp = tempFileName.Substring(secondLastDotIndex + 1, lastDotIndex - secondLastDotIndex - 1);
            // 타임스탬프 형식 검증 (yyyyMMdd_HHmmss)
            if (timestamp.Length == 15 && timestamp.Contains("_"))
            {
                return timestamp;
            }

            return null;
        }

        private bool Write(string saveFilePath, string chunk)
        {
            string tempFilePath = null;
            try
            {
                // 임시 파일 경로 생성
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                tempFilePath = $"{saveFilePath}.{timestamp}.tmp";

                // 임시 파일에 쓰기
                File.WriteAllText(tempFilePath, chunk ?? string.Empty);

                // 임시 파일 검증
                if (!ValidateTempFile(tempFilePath, chunk))
                {
                    // 검증 실패: 임시 파일을 에러 파일로 남기고, 본 파일은 건드리지 않음
                    RenameTempFileToErrorFile(tempFilePath, "validation_failed", "ValidateTempFile 실패");
                    return false;
                }

                // 검증 성공 시에만 본 파일 교체
                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                }

                File.Move(tempFilePath, saveFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("게임 데이터를 저장할 수 없습니다.\nException Message: {0}", ex.Message);

                try // 예외 발생 시에도 임시 파일은 에러 파일로 남김
                {
                    if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                    {
                        RenameTempFileToErrorFile(
                            tempFilePath,
                            "exception",
                            ex.ToString());
                    }
                }
                catch (Exception cleanupEx)
                {
                    Debug.LogWarning(
                        $"임시 파일 정리 중 추가 예외 발생: {cleanupEx.Message}");
                }

                return false;
            }
        }

        private bool ValidateTempFile(string tempFilePath, string originalChunk)
        {
            try
            {
                // 파일 존재 확인
                if (!File.Exists(tempFilePath))
                {
                    Debug.LogError("임시 파일이 생성되지 않았습니다.");
                    return false;
                }

                // 파일 크기 확인
                FileInfo fileInfo = new FileInfo(tempFilePath);
                if (fileInfo.Length == 0)
                {
                    Debug.LogError("임시 파일이 비어있습니다.");
                    return false;
                }

                // 파일 내용 읽어서 원본과 비교
                string writtenContent = File.ReadAllText(tempFilePath);
                if (string.IsNullOrEmpty(writtenContent))
                {
                    Debug.LogError("임시 파일 내용을 읽을 수 없습니다.");
                    return false;
                }
                else if (writtenContent != (originalChunk ?? string.Empty))
                {
                    Debug.LogError("임시 파일 내용이 원본과 일치하지 않습니다.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"임시 파일 검증 중 오류 발생: {ex.Message}");
                return false;
            }
        }

        private void RenameTempFileToErrorFile(string tempFilePath, string errorType, string errorMessage = null)
        {
            try
            {
                if (string.IsNullOrEmpty(tempFilePath) || !File.Exists(tempFilePath))
                {
                    return;
                }

                string errorFilePath = tempFilePath.Replace(".tmp", $".{errorType}.error");

                if (File.Exists(errorFilePath))
                {
                    File.Delete(errorFilePath);
                }

                File.Move(tempFilePath, errorFilePath);

                // 에러 정보 덧붙이기
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    File.AppendAllText(
                        errorFilePath,
                        $"\n\n--- ERROR INFO ---\n[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n{errorMessage}\n");
                }

                Debug.LogWarning($"임시 파일을 에러 파일로 전환했습니다. ErrorType={errorType}, File={Path.GetFileName(errorFilePath)}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"에러 파일로 전환 중 예외 발생: {ex.Message}");
            }
        }

        public static void DeleteSaveFileForEditor()
        {
            for (int i = 0; i < GAME_DATA_COUNT; i++)
            {
                string saveFilePath = GetSaveFilePath(i);

                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);

                    Debug.Log($"로컬 세이브 파일을 삭제합니다. SaveFilePath: {saveFilePath}");
                }
                else
                {
                    Debug.Log($"로컬 세이브 파일이 이미 삭제되었습니다. SaveFilePath: {saveFilePath}");
                }
            }
        }
    }
}