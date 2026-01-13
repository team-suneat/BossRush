using TeamSuneat.Scenes;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerCharacterSpawner : XBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private bool _useRespawn;
        [SerializeField] private float _respawnDelayTime;

        private XScene _parentScene;

        public PlayerCharacter SpawnedPlayer { get; private set; }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerCharacterDespawned);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerCharacterDespawned);
        }

        void OnPlayerCharacterDespawned()
        {
            if (_useRespawn)
            {
                CoroutineNextTimer(_respawnDelayTime, SpawnPlayer);
            }
        }

        public void Initialize(XScene parentScene)
        {
            _parentScene = parentScene;
        }

        public void SpawnPlayer()
        {
            PlayerCharacter cachedPlayer = CharacterManager.Instance.Player;
            if (cachedPlayer != null)
            {
                Log.Info(LogTags.CharacterSpawn, "이미 플레이어 캐릭터가 존재하여 새로 생성하지 않습니다.");
                SpawnedPlayer = cachedPlayer;
                return;
            }

            Vector3 spawnPosition = GetSpawnPosition();
            PlayerCharacter player = ResourcesManager.SpawnPlayerCharacter(_playerPrefab, spawnPosition, transform);
            if (player == null)
            {
                Log.Error(LogTags.CharacterSpawn, "플레이어 캐릭터 프리팹 스폰에 실패했습니다.");
                return;
            }

            player.Initialize();
            SpawnedPlayer = player;

            Log.Info(LogTags.CharacterSpawn, "플레이어 캐릭터를 생성했습니다. 위치: {0}", spawnPosition);
        }

        public void CleanupPlayer()
        {
            if (SpawnedPlayer != null)
            {
                CharacterManager.Instance.UnregisterPlayer(SpawnedPlayer);
                Destroy(SpawnedPlayer.gameObject);
                SpawnedPlayer = null;

                Log.Info(LogTags.CharacterSpawn, "플레이어 캐릭터를 정리했습니다.");
            }
        }

        private Vector3 GetSpawnPosition()
        {
            if (_spawnPoint != null)
            {
                return _spawnPoint.position;
            }

            return transform.position;
        }
    }
}