using Newtonsoft.Json;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public partial class GameDataManager
    {
        private const int GAME_DATA_COUNT = 3;
        private const int GAME_DATA_SAVE_INTERVAL_COUNT = 3;
        private const bool USE_ASYNC_SAVE = false;
        private const float SAVE_COOLDOWN_TIME = 1.0f;

        public GameData Data;
        public bool IsSaving => _isAsyncSaving;

        private string _storedChunk;
        private bool _isAsyncSaving = false;
        private readonly object _asyncSaveLock = new object();
        private float _lastSaveTime = 0f;
        private bool _pendingSave = false;
        private int _saveCount;
        private readonly JsonSerializerSettings _serializeSettings;
        private readonly JsonSerializerSettings _deserializeSettings;

        public GameDataManager()
        {
            _saveCount = 0;

            // 직렬화는 알파벳 순서 정렬로 만든다. (스트링 비교를 위해)
            _serializeSettings = new JsonSerializerSettings { ContractResolver = new OrderedContractResolver() };

            // 역직렬화는 private set을 허용해야 한다.
            _deserializeSettings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };

            SetSaveFilePath();

            Debug.Log("게임 데이터 매니저를 생성합니다.");
        }
    }
}