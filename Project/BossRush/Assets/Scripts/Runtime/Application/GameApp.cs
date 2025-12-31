using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TeamSuneat.Setting;
using UnityEngine;
using UnityEngine.U2D;

namespace TeamSuneat
{
    public sealed class GameApp : XGameApp
    {
        public static GameApp Instance;
        public float prevChangeSceneTime;

        public GameData data
        {
            get
            {
                if (dataManager != null)
                {
                    return dataManager.Data;
                }

                return null;
            }
            set => dataManager.Data = value;
        }

        public GameDataManager dataManager;
        public GameManager gameManager;

        public bool IsInitialized { get; private set; }

        private bool _isApplicationPaused = false;

        //

        public void ForceApplicationStart()
        {
            OnApplicationStart();
        }

        //

        protected override void OnApplicationStart()
        {
            Instance = this;

            // LOG
            Log.LoadLevel();
            Log.Initialize();

            // SETTING
            GameSetting.Instance.Initialize();

            // PREF DATA
            GamePrefs.ClearOnEntryPoint();

            // FIXED DATA
            LoadAsync();
        }

        private async void LoadAsync()
        {
            await PathManager.LoadAllAsync();
            await ScriptableDataManager.Instance.LoadScriptableAssetsAsync();

            // 구글 시트에서 동기화
            bool googleSheetSynced = await GoogleSheetRuntimeSync.FetchConvertAndApplyAllAsync();
            if (!googleSheetSynced)
            {
                // 미리 생성된 JSON 파일을 로드
                await JsonDataManager.LoadJsonSheetsAsync();
            }

            await ResourcesManager.LoadResourcesByLabelAsync<SpriteAtlas>(AddressableLabels.Ingame);
            await ResourcesManager.LoadResourcesByLabelAsync<GameObject>(AddressableLabels.Ingame);

            // LOAD SAVED DATA
            LoadGameData();

            IsInitialized = true;
        }

        protected override void OnApplicationStarted()
        {
            gameManager = gameObject.AddComponent<GameManager>();
        }

        private void Update()
        {
            // 대기 중인 저장 처리
            dataManager?.ProcessPendingSave();
        }

        public static GameApp Create()
        {
            GameObject gameApp = new("@GameApp");

            return gameApp.AddComponent<GameApp>();
        }

        public void LoadGameData()
        {
            dataManager ??= new GameDataManager();
            dataManager.LoadGameDataWithRecovery();

            // 오프라인 시간 계산 (데이터 로드 후)
            if (data?.GetSelectedProfile() != null)
            {
                OfflineTimeManager.Instance.CalculateOfflineTime();
            }
        }

        public void SaveGameData()
        {
            // data.Statistics.RegisterLastSaveTime();
            dataManager?.Save();
        }

        public static VProfile GetSelectedProfile()
        {
            return Instance?.data?.GetSelectedProfile();
        }

        protected override void OnApplicationPaused()
        {
            if (_isApplicationPaused) return; // 중복 방지

            _isApplicationPaused = true;

            // 저장 (LastSaveTime 자동 업데이트)
            SaveGameData();

            Log.Info(LogTags.Time, "앱이 백그라운드로 이동했습니다. 데이터를 저장했습니다.");
        }

        protected override void OnApplicationResume()
        {
            if (!_isApplicationPaused) return; // 중복 방지

            _isApplicationPaused = false;

            // 오프라인 시간 계산
            if (data?.GetSelectedProfile() != null)
            {
                OfflineTimeManager.Instance.CalculateOfflineTime();

                double offlineSeconds = OfflineTimeManager.Instance.RewardableOfflineTimeSeconds;
                if (offlineSeconds > 0)
                {
                    Log.Info(LogTags.Time, $"앱이 포그라운드로 복귀했습니다. 오프라인 시간: {OfflineTimeManager.Instance.GetOfflineTimeString()}");
                    // 향후: 오프라인 보상 처리
                }
                else
                {
                    Log.Info(LogTags.Time, "앱이 포그라운드로 복귀했습니다.");
                }
            }
        }
    }
}