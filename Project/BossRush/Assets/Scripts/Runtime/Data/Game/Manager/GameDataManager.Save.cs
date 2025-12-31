using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public partial class GameDataManager
    {
        public void Save()
        {
            float currentTime = Time.time;

            // 쿨다운 체크
            if (currentTime - _lastSaveTime < SAVE_COOLDOWN_TIME)
            {
                _pendingSave = true;
                return;
            }

            if (USE_ASYNC_SAVE) { SaveAsync(); }    // 비동기 저장 모드
            else { SaveSync(); }// 동기 저장 모드

            GlobalEvent.Send(GlobalEventType.SAVE_GAME_DATA);
        }

        public void ProcessPendingSave()
        {
            if (_pendingSave && Time.time - _lastSaveTime >= SAVE_COOLDOWN_TIME)
            {
                _pendingSave = false;
                Save();
            }
        }

        public string SaveForEditorOrDevelopment()
        {
            string chunk = SaveForEditor(0);
            if (!string.IsNullOrEmpty(chunk))
            {
                if (_saveCount >= GAME_DATA_SAVE_INTERVAL_COUNT)
                {
                    SaveForEditor(1);
                    _saveCount = 0;
                }
            }

            return chunk;
        }

        public string SaveForBuild()
        {
            string chunk = SaveForBuild(0);
            if (!string.IsNullOrEmpty(chunk))
            {
                if (_saveCount >= GAME_DATA_SAVE_INTERVAL_COUNT)
                {
                    _ = SaveForBuild(1);
                    _saveCount = 0;
                }
            }

            return chunk;
        }

        public string SaveForEditor(int index)
        {
            return PerformSave(index, false);
        }

        private string SaveForBuild(int index)
        {
            return PerformSave(index, false);
        }

        private string PerformSave(int index, bool isBackground = false, float? customTimestamp = null)
        {
            string chunk = ConvertToChunk(index, isBackground);
            if (string.IsNullOrEmpty(chunk))
            {
                return null; // 변경사항이 없음
            }

            string saveFilePath = GetSaveFilePath(index);
            string chunkAES = TryApplyAES() ? Encrypt(chunk) : chunk;

            if (Write(saveFilePath, chunkAES))
            {
                // 백그라운드에서는 customTimestamp 사용, 동기에서는 Time.time 사용
                if (isBackground)
                {
                    // 백그라운드에서는 미리 준비된 timestamp 사용
                    UpdateSaveState(chunk, index, customTimestamp ?? _lastSaveTime, isBackground);
                }
                else
                {
                    // 동기에서는 Time.time 사용
                    UpdateSaveState(chunk, index, Time.time, isBackground);
                }

                // 메인 저장 파일(index 0) 저장 성공 시 LastSaveTime 업데이트
                if (index == 0)
                {
                    Data?.GetSelectedProfile()?.Statistics?.RegisterLastSaveTime();
                }

                return chunk;
            }

            return null;
        }

        private string ConvertToChunk(int index, bool isBackground = false)
        {
            string chunk = null;

            if (Data != null)
            {
                if (string.IsNullOrEmpty(Data.Profile.Stage.CurrentAreaString))
                {
                    Log.Warning("현재 지역의 스트링이 유효하지 않습니다.");
                    return null;
                }
                chunk = SerializeObject(Data);
            }

            if (string.IsNullOrEmpty(chunk))
            {
                return null;
            }

            // 스레드 안전성 처리
            if (isBackground)
            {
                lock (_asyncSaveLock)
                {
                    if (_storedChunks[index] == chunk)
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (_storedChunks[index] == chunk)
                {
                    return null;
                }
            }

            return chunk;
        }

        private void UpdateSaveState(string chunk, int index, float timestamp, bool isBackground = false)
        {
            _storedChunks[index] = chunk;

            if (index == 0)
            {
                _saveCount++;
                _lastSaveTime = timestamp;

                if (CheckUnityEditor())
                {
                    Debug.Log($"유니티 에디터용 게임 데이터를 저장합니다. Index:{index}, SaveCount:{_saveCount}/{GAME_DATA_SAVE_INTERVAL_COUNT}");
                }

                // 백업 저장이 필요한 경우
                if (_saveCount >= GAME_DATA_SAVE_INTERVAL_COUNT)
                {
                    _ = PerformSave(1, isBackground, timestamp); // 백업 저장
                    _saveCount = 0;

                    if (CheckUnityEditor())
                    {
                        Debug.Log($"유니티 에디터용 게임 백업 데이터를 저장합니다. Index:1");
                    }
                }
            }
        }

        #region 비동기 저장

        /// <summary>
        /// 비동기적으로 게임 데이터를 저장합니다.
        /// </summary>
        private void SaveAsync()
        {
            lock (_asyncSaveLock)
            {
                if (_isAsyncSaving)
                {
                    Debug.LogWarning("이미 비동기 저장 중입니다. 저장 요청을 무시합니다.");
                    return;
                }
                _isAsyncSaving = true;
            }

            // 메인 스레드에서 최소한의 정보만 준비
            bool needsBackup = _saveCount >= GAME_DATA_SAVE_INTERVAL_COUNT;
            bool useEncryption = TryApplyAES();
            float currentTimestamp = Time.time; // 메인 스레드에서 미리 계산

            TeamSuneat.Log.Progress("게임 데이터를 저장합니다: {0}/{1}", _saveCount, GAME_DATA_SAVE_INTERVAL_COUNT);

            // 모든 무거운 작업을 백그라운드 스레드로 이동
            _ = System.Threading.Tasks.Task.Run(() => PerformFullAsyncSave(needsBackup, useEncryption, currentTimestamp));
        }

        /// <summary>
        /// 완전한 비동기 저장 작업을 수행합니다 (직렬화, 암호화, 파일 쓰기 모두 백그라운드에서).
        /// </summary>
        private void PerformFullAsyncSave(bool needsBackup, bool useEncryption, float timestamp)
        {
            bool saveSuccess = false;
            bool backupSuccess = true;
            string errorMessage = null;
            string originalChunk = null;

            try
            {
                // 백그라운드에서 저장 수행
                originalChunk = PerformSave(0, true, timestamp);
                if (string.IsNullOrEmpty(originalChunk))
                {
                    return; // 변경사항이 없으면 저장하지 않음
                }

                saveSuccess = true;

                // 백업 저장도 필요하면 처리
                if (needsBackup)
                {
                    string backupChunk = PerformSave(1, true, timestamp);
                    backupSuccess = !string.IsNullOrEmpty(backupChunk);
                }
            }
            catch (System.Exception ex)
            {
                errorMessage = ex.Message;
                Debug.LogError(errorMessage);
            }
            finally
            {
                // 스레드 안전한 상태 리셋
                lock (_asyncSaveLock)
                {
                    _isAsyncSaving = false;
                }

                // 메인 스레드로 결과 전달 (Unity API 호출)
                MonoBehaviour monoBehaviour = UnityEngine.Object.FindAnyObjectByType<MonoBehaviour>();
                if (monoBehaviour != null)
                {
                    _ = monoBehaviour.StartCoroutine(
                        OnAsyncSaveComplete(saveSuccess, backupSuccess, originalChunk, needsBackup, errorMessage));
                }
            }
        }

        /// <summary>
        /// 비동기 저장 완료 시 메인 스레드에서 호출됩니다.
        /// </summary>
        private System.Collections.IEnumerator OnAsyncSaveComplete(bool saveSuccess, bool backupSuccess, string originalChunk, bool needsBackup, string errorMessage)
        {
            yield return null; // 한 프레임 대기

            if (saveSuccess)
            {
                if (backupSuccess)
                {
                    Debug.Log("비동기 저장이 완료되었습니다.");
                }
                else
                {
                    Debug.LogWarning("메인 저장은 성공했지만 백업 저장에 실패했습니다.");
                }
            }
            else
            {
                Debug.LogError("비동기 저장 중 파일 쓰기 실패");
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Debug.LogError($"비동기 저장 중 오류 발생: {errorMessage}");
            }
        }

        #endregion 비동기 저장

        #region 동기 저장

        /// <summary>
        /// 동기적으로 게임 데이터를 저장합니다.
        /// </summary>
        private void SaveSync()
        {
            if (CheckUnityEditor())
            {
                _ = SaveForEditorOrDevelopment();
            }
            else
            {
                _ = SaveForBuild();
            }
        }

        #endregion 동기 저장

        #region 유틸리티

        /// <summary>
        /// Unity 에디터인지 확인합니다.
        /// </summary>
        /// <returns>에디터 여부</returns>
        private bool CheckUnityEditor()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        #endregion 유틸리티
    }
}