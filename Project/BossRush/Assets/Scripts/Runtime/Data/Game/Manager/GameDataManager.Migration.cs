using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public partial class GameDataManager
    {
        private const int CURRENT_SAVE_VERSION = 1;
        private const int MIN_SUPPORTED_VERSION = 1;

        private GameData MigrateAndLoad(string jsonString)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonString))
                {
                    Debug.LogError("마이그레이션할 JSON 문자열이 비어있습니다.");
                    return null;
                }

                // 현재 버전으로 불러오기 시도
                GameData currentData = LoadCurrentVersion(jsonString);
                if (currentData != null)
                {
                    Debug.Log($"현재 버전({CURRENT_SAVE_VERSION})으로 세이브 데이터를 불러오는 데 성공했습니다.");
                    return currentData;
                }

                // 버전 추출 및 마이그레이션 시도
                int saveVersion = ExtractSaveVersion(jsonString);
                Debug.Log($"세이브 파일 버전: {saveVersion}, 현재 버전: {CURRENT_SAVE_VERSION}");

                if (saveVersion < MIN_SUPPORTED_VERSION)
                {
                    Debug.LogError($"지원하지 않는 세이브 버전입니다: {saveVersion} (최소 지원: {MIN_SUPPORTED_VERSION})");
                    return null;
                }

                if (saveVersion > CURRENT_SAVE_VERSION)
                {
                    Debug.LogError($"현재 게임이 지원하지 않는 세이브 버전입니다: {saveVersion} (현재 버전: {CURRENT_SAVE_VERSION})");
                    return null;
                }

                if (saveVersion == CURRENT_SAVE_VERSION)
                {
                    Debug.LogError("현재 버전이지만 세이브 데이터를 불러오는 데 실패했습니다.");
                    return null;
                }

                // 마이그레이션 수행
                GameData migratedData = MigrateToCurrentVersion(jsonString, saveVersion);
                if (migratedData != null)
                {
                    Debug.Log($"버전 {saveVersion}에서 {CURRENT_SAVE_VERSION}로 마이그레이션 성공");
                    return migratedData;
                }
                else
                {
                    Debug.LogError($"버전 {saveVersion}에서 {CURRENT_SAVE_VERSION}로 마이그레이션 실패");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"마이그레이션 중 오류: {ex.Message}");
                return null;
            }
        }

        private int ExtractSaveVersion(string jsonString)
        {
            try
            {
                // 간단한 JSON 파싱으로 SaveVersion 필드 추출
                if (jsonString.Contains("\"SaveVersion\":"))
                {
                    int startIndex = jsonString.IndexOf("\"SaveVersion\":") + "\"SaveVersion\":".Length;
                    int endIndex = jsonString.IndexOf(",", startIndex);
                    if (endIndex == -1)
                    {
                        endIndex = jsonString.IndexOf("}", startIndex);
                    }

                    if (startIndex > 0 && endIndex > startIndex)
                    {
                        string versionStr = jsonString.Substring(startIndex, endIndex - startIndex).Trim();
                        if (int.TryParse(versionStr, out int version))
                        {
                            return version;
                        }
                    }
                }

                // SaveVersion 필드가 없으면 기본값 1 반환
                Debug.Log("SaveVersion 필드를 찾을 수 없어 기본값 1을 사용합니다.");
                return 1;
            }
            catch (Exception ex)
            {
                Debug.LogError($"세이브 버전 추출 중 오류: {ex.Message}");
                return 1;
            }
        }

        private GameData LoadCurrentVersion(string jsonString)
        {
            try
            {
                // 먼저 세이브 버전을 확인
                int saveVersion = ExtractSaveVersion(jsonString);

                // 현재 버전이 아니면 null 반환 (마이그레이션 필요)
                if (saveVersion != CURRENT_SAVE_VERSION)
                {
                    Debug.Log($"세이브 버전({saveVersion})이 현재 버전({CURRENT_SAVE_VERSION})과 다릅니다. 마이그레이션이 필요합니다.");
                    return null;
                }

                // 현재 버전인 경우에만 역직렬화 수행
                return JsonConvert.DeserializeObject<GameData>(jsonString, _deserializeSettings);
            }
            catch (Exception ex)
            {
                Debug.LogError($"현재 버전의 세이브 데이터를 불러오는 데 실패했습니다: {ex.Message}");
                return null;
            }
        }

        private GameData MigrateToCurrentVersion(string jsonString, int fromVersion)
        {
            try
            {
                string currentJson = jsonString;

                // 단계별 마이그레이션 수행
                for (int version = fromVersion; version < CURRENT_SAVE_VERSION; version++)
                {
                    Debug.Log($"버전 {version}에서 {version + 1}로 마이그레이션 수행");
                    currentJson = MigrateVersion(version, currentJson);

                    if (string.IsNullOrEmpty(currentJson))
                    {
                        Debug.LogError($"버전 {version}에서 {version + 1}로 마이그레이션 실패");
                        return null;
                    }
                }

                // 최종 버전으로 역직렬화
                return LoadCurrentVersion(currentJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"마이그레이션 중 오류: {ex.Message}");
                return null;
            }
        }

        private string MigrateVersion(int fromVersion, string jsonString)
        {
            switch (fromVersion)
            {
                // 추가 버전 마이그레이션은 여기에 추가
                default:
                    Debug.LogError($"지원하지 않는 마이그레이션: {fromVersion} → {fromVersion + 1}");
                    return null;
            }
        }

        #region 마이그레이션 공통 로직

        private string PerformMigration(GameData gameData, int fromVersion, int toVersion)
        {
            try
            {
                // 버전 확인 및 마이그레이션 액션 수행
                if (gameData.SaveVersion == fromVersion)
                {
                    gameData.SaveVersion = toVersion;
                    Debug.Log($"버전 {fromVersion}에서 버전 {toVersion}로 마이그레이션: SaveVersion 필드 업데이트");
                }

                // JSON 직렬화하여 반환
                return JsonConvert.SerializeObject(gameData, Formatting.Indented, _serializeSettings);
            }
            catch (Exception ex)
            {
                Debug.LogError($"버전 {fromVersion} 마이그레이션 중 오류: {ex.Message}");
                return null;
            }
        }

        #endregion 마이그레이션 공통 로직

        private bool CanMigrate(string jsonString)
        {
            try
            {
                int saveVersion = ExtractSaveVersion(jsonString);
                return saveVersion >= MIN_SUPPORTED_VERSION && saveVersion <= CURRENT_SAVE_VERSION;
            }
            catch
            {
                return false;
            }
        }

        private void LogMigrationInfo(string jsonString)
        {
            try
            {
                int saveVersion = ExtractSaveVersion(jsonString);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"마이그레이션 정보:");
                stringBuilder.AppendLine($"  - 세이브 버전: {saveVersion}");
                stringBuilder.AppendLine($"  - 현재 버전: {CURRENT_SAVE_VERSION}");
                stringBuilder.AppendLine($"  - 최소 지원 버전: {MIN_SUPPORTED_VERSION}");
                stringBuilder.AppendLine($"  - 마이그레이션 필요: {saveVersion != CURRENT_SAVE_VERSION}");
                stringBuilder.AppendLine($"  - 마이그레이션 가능: {CanMigrate(jsonString)}");

                Debug.Log(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogError($"마이그레이션 정보 출력 중 오류: {ex.Message}");
            }
        }
    }
}