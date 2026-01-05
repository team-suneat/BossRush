using System.Collections;
using System.Collections.Generic;
using TeamSuneat;
using TeamSuneat.Data;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat.Stage
{
    public class StageSystem : XBehaviour
    {
        public StageNames Name;
        public string NameString;

        [SerializeField]
        private MonsterCharacterSpawner _monsterSpawner;

        private StageAsset _currentStageAsset;

        private Coroutine _stageFlowCoroutine;

        public override void AutoSetting()
        {
            base.AutoSetting();
            NameString = Name.ToString();
        }

        public override void AutoNaming()
        {
            SetGameObjectName(NameString);
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public void Initialize(StageLoader stageLoader, PlayerCharacterSpawner playerCharacterSpawner)
        {
            LoadStageData();
            InitializeMonster();
            RegisterCurrentStage();
            RegisterGlobalEvents();

            Log.Info(LogTags.Stage, "스테이지 초기화 완료: {0}", Name);
            _stageFlowCoroutine = StartCoroutine(StartStageFlow());
        }

        public void CleanupStage()
        {
            StopStageFlow();
            UnregisterGlobalEvents();
            _monsterSpawner?.CleanupAllMonsters();
        }

        private void LoadStageData()
        {
            _currentStageAsset = ScriptableDataManager.Instance.FindStage(Name);
            if (_currentStageAsset == null)
            {
                Log.Warning(LogTags.Stage, "스테이지 에셋을 찾을 수 없습니다: {0}", Name);
                return;
            }
        }

        private void InitializeMonster()
        {
            if (_monsterSpawner == null)
            {
                Log.Warning(LogTags.Stage, "MonsterCharacterSpawner가 설정되지 않았습니다.");
                return;
            }

            _monsterSpawner.Initialize(_currentStageAsset);
        }

        private void RegisterCurrentStage()
        {
            if (GameApp.Instance?.gameManager == null)
            {
                return;
            }

            // GameApp.Instance.gameManager.CurrentStageSystem = this;
        }

        private IEnumerator StartStageFlow()
        {
            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null)
            {
                GlobalEvent<StageNames>.Send(GlobalEventType.STAGE_SPAWNED, Name);
                yield break;
            }

            StartMonsters(profile);
            GlobalEvent<StageNames>.Send(GlobalEventType.STAGE_SPAWNED, Name);
        }

        private void StartMonsters(Data.Game.VProfile profile)
        {
            if (_currentStageAsset == null)
            {
                Log.Warning(LogTags.Stage, "스테이지 에셋이 없어 몬스터를 시작할 수 없습니다.");
                return;
            }

            if (_monsterSpawner != null)
            {
                _monsterSpawner.SpawnMonsters();
                SetPlayerTargetToFirstMonster();
            }
        }

        private void RegisterGlobalEvents()
        {
            GlobalEvent<Character>.Register(GlobalEventType.BOSS_CHARACTER_DEATH, OnBossDeath);
            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerDespawn);
        }

        private void UnregisterGlobalEvents()
        {
            GlobalEvent<Character>.Unregister(GlobalEventType.BOSS_CHARACTER_DEATH, OnBossDeath);
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerDespawn);
        }

        // OnBossDeath() 수정 - 보스만 확인하도록
        private void OnBossDeath(Character character)
        {
            // 보스만 확인 (일반 몬스터는 무시)
            if (character == null || !character.IsBoss)
            {
                return;
            }

            // 보스가 죽었는지 확인
            if (_monsterSpawner?.SpawnedMonsters == null)
            {
                return;
            }

            // 스폰된 몬스터 중 보스만 확인
            bool isBossDefeated = true;
            for (int i = 0; i < _monsterSpawner.SpawnedMonsters.Count; i++)
            {
                MonsterCharacter monster = _monsterSpawner.SpawnedMonsters[i];
                if (monster != null && monster.IsAlive && monster.IsBoss)
                {
                    isBossDefeated = false;
                    break;
                }
            }

            if (!isBossDefeated)
            {
                return;
            }

            // 보스 체력 게이지 정리
            UIManager.Instance?.HUDManager?.OnBossDied();

            Log.Info(LogTags.Stage, "보스 처치 완료. 다음 스테이지로 진행합니다.");
        }

        private void OnPlayerDespawn()
        {
        }

        private void SetPlayerTargetToFirstMonster()
        {
            if (_monsterSpawner?.SpawnedMonsters == null || _monsterSpawner.SpawnedMonsters.Count == 0)
            {
                Log.Warning(LogTags.Stage, "스폰된 몬스터가 없어 플레이어 타겟을 설정할 수 없습니다.");
                return;
            }

            PlayerCharacter player = CharacterManager.Instance.Player;
            if (player == null)
            {
                Log.Warning(LogTags.Stage, "플레이어가 없어 타겟을 설정할 수 없습니다.");
                return;
            }

            MonsterCharacter firstMonster = _monsterSpawner.SpawnedMonsters[0];
            if (firstMonster == null || !firstMonster.IsAlive)
            {
                Log.Warning(LogTags.Stage, "첫 번째 몬스터가 유효하지 않아 타겟을 설정할 수 없습니다.");
                return;
            }

            player.SetTarget(firstMonster);
            Log.Info(LogTags.Stage, "플레이어 타겟을 첫 번째 몬스터로 설정했습니다: {0}", firstMonster.Name.ToLogString());
        }

        private Data.Game.VProfile GetSelectedProfile()
        {
            return GameApp.GetSelectedProfile();
        }

        public void EnterBossMode()
        {
            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null)
            {
                Log.Warning(LogTags.Stage, "프로필이 없어 보스 모드에 진입할 수 없습니다.");
                return;
            }
        }

        private void StopStageFlow()
        {
            if (_stageFlowCoroutine != null)
            {
                StopCoroutine(_stageFlowCoroutine);
                _stageFlowCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "스테이지 흐름 종료");
        }
    }
}