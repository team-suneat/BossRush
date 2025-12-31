using Newtonsoft.Json;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// 게임 데이터 관리의 핵심 클래스
    /// </summary>
    public partial class GameDataManager
    {
        // 상수
        private const int GAME_DATA_COUNT = 3;

        private const int GAME_DATA_SAVE_INTERVAL_COUNT = 3;
        private const bool USE_ASYNC_SAVE = false; // true: 비동기 저장, false: 동기 저장
        private const float SAVE_COOLDOWN_TIME = 1.0f; // 저장 쿨다운 시간 (초)

        // 필드
        private string[] _storedChunks;

        private bool _isAsyncSaving = false;
        private readonly object _asyncSaveLock = new object();
        private float _lastSaveTime = 0f;
        private bool _pendingSave = false;

        // 프로퍼티
        public bool IsSaving => _isAsyncSaving;

        /// <summary>
        /// 로컬 세이브 파일 암호화에 사용하는 대칭키를 구성한다.
        /// 고정된 값으로 사용하여 어떤 기기이든 사용할 수 있게 한다.
        /// </summary>
        private readonly string _symmetricKey;

        private int _saveCount;

        private readonly JsonSerializerSettings _serializeSettings;
        private readonly JsonSerializerSettings _deserializeSettings;

        // 프로퍼티
        public GameData Data;

        /// <summary>
        /// GameDataManager 인스턴스를 생성합니다.
        /// </summary>
        public GameDataManager()
        {
            _storedChunks = new string[2];
            _saveCount = 0;

            // 직렬화는 알파벳 순서 정렬로 만든다. (스트링 비교를 위해)
            _serializeSettings = new JsonSerializerSettings { ContractResolver = new OrderedContractResolver() };

            // 역직렬화는 private set을 허용해야 한다.
            _deserializeSettings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };

            _symmetricKey = AES.Encrypt(GameSymmetricIdentifier(), "pub");

            // 파일 경로 포맷 초기화
            SetSaveFilePath();

            Debug.Log("게임 데이터 매니저를 생성합니다.");
        }
    }
}