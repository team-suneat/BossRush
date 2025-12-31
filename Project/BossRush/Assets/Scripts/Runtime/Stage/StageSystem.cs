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

        [SerializeField]
        private BossModeHandler _bossModeHandler;

        [SerializeField]
        private StageScrollController _scrollController;

        [SerializeField]
        private float _nextWaveDelay = 3f;

        private StageAsset _currentStageAsset;
        private AreaAsset _currentAreaAsset;

        private Coroutine _stageFlowCoroutine;
        private Coroutine _nextWaveDelayCoroutine;
        private Coroutine _waveResetFadeCoroutine;

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
            InitializeBossModeHandler(stageLoader, playerCharacterSpawner);
            RegisterCurrentStage();
            RegisterGlobalEvents();

            if (_scrollController != null)
            {
                _scrollController.SetMaxPointIndex(_currentStageAsset.GetStageMonsterCount());
                _scrollController.StopScrolling();
            }

            Log.Info(LogTags.Stage, "스테이지 초기화 완료: {0}", Name);
            _stageFlowCoroutine = StartCoroutine(StartStageFlow());
        }

        public void CleanupStage()
        {
            _bossModeHandler?.Cleanup();
            StopStageFlow();
            StopNextWaveDelay();
            StopWaveResetFade();
            UnregisterGlobalEvents();
            _monsterSpawner?.CleanupAllMonsters();
            _scrollController?.StopScrolling();
        }

        private void LoadStageData()
        {
            _currentStageAsset = ScriptableDataManager.Instance.FindStage(Name);
            if (_currentStageAsset == null)
            {
                Log.Warning(LogTags.Stage, "스테이지 에셋을 찾을 수 없습니다: {0}", Name);
                return;
            }

            _currentAreaAsset = ScriptableDataManager.Instance.FindArea(_currentStageAsset.AreaName);
            if (_currentAreaAsset == null)
            {
                Log.Warning(LogTags.Stage, "지역 에셋을 찾을 수 없습니다: {0}", _currentStageAsset.AreaName);
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

            Transform scrollContainer = _scrollController != null ? _scrollController.ScrollContainer : null;
            _monsterSpawner.Initialize(_currentStageAsset, _currentAreaAsset, scrollContainer, _scrollController);
        }

        private void InitializeBossModeHandler(StageLoader stageLoader, PlayerCharacterSpawner playerCharacterSpawner)
        {
            if (_bossModeHandler == null)
            {
                Log.Warning(LogTags.Stage, "BossModeHandler가 설정되지 않았습니다.");
                return;
            }

            _bossModeHandler.Initialize(
                _monsterSpawner,
                playerCharacterSpawner,
                _currentStageAsset,
                _currentAreaAsset,
                transform,
                StopNextWaveDelay,
                StopWaveResetFade,
                SetPlayerTargetToFirstMonster,
                OnWaveRestore,
                OnStageProgressUpdate,
                stageLoader
            );
        }

        private void OnWaveRestore(int wave)
        {
            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage != null)
            {
                profile.Stage.CurrentWave = wave;
            }
        }

        private void OnStageProgressUpdate(int currentWave, int totalWaves)
        {
            UIManager.Instance.HUDManager.SetStageProgress(currentWave, totalWaves);
        }

        private void RegisterCurrentStage()
        {
            if (GameApp.Instance?.gameManager == null)
            {
                return;
            }

            GameApp.Instance.gameManager.CurrentStageSystem = this;
        }

        private IEnumerator StartStageFlow()
        {
            yield return WaitForStageTitleNotice();

            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null)
            {
                GlobalEvent<StageNames>.Send(GlobalEventType.STAGE_SPAWNED, Name);
                yield break;
            }

            StartFirstWave(profile);
            GlobalEvent<StageNames>.Send(GlobalEventType.STAGE_SPAWNED, Name);
        }

        private IEnumerator WaitForStageTitleNotice()
        {
            UIStageTitleNotice stageNotice = SpawnStageTitleNotice();
            if (stageNotice == null)
            {
                yield break;
            }

            bool isCompleted = false;
            stageNotice.OnCompleted += () => isCompleted = true;

            while (!isCompleted)
            {
                yield return null;
            }
        }

        private void StartFirstWave(Data.Game.VProfile profile)
        {
            if (_currentStageAsset == null)
            {
                Log.Warning(LogTags.Stage, "스테이지 에셋이 없어 첫 웨이브를 시작할 수 없습니다.");
                return;
            }

            int currentWave = profile.Stage.CurrentWave;
            if (currentWave < 0 || currentWave >= _currentStageAsset.WaveCount)
            {
                currentWave = 0;
                profile.Stage.ResetCurrentWave();
            }

            // 첫 웨이브 시작 시 스크롤 리셋
            _scrollController?.ResetToFirstPoint();

            if (_monsterSpawner != null)
            {
                _monsterSpawner.SpawnWave(currentWave);
                SetPlayerTargetToFirstMonster();
            }

            UIManager.Instance.HUDManager.SetStageProgress(currentWave + 1, _currentStageAsset.WaveCount);
        }

        private void RegisterGlobalEvents()
        {
            GlobalEvent<Character>.Register(GlobalEventType.MONSTER_CHARACTER_DEATH, OnMonsterDeath);
            GlobalEvent<Character>.Register(GlobalEventType.BOSS_CHARACTER_DEATH, OnBossDeath);
            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerDespawn);
        }

        private void UnregisterGlobalEvents()
        {
            GlobalEvent<Character>.Unregister(GlobalEventType.MONSTER_CHARACTER_DEATH, OnMonsterDeath);
            GlobalEvent<Character>.Unregister(GlobalEventType.BOSS_CHARACTER_DEATH, OnBossDeath);
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerDespawn);
        }

        private void OnMonsterDeath(Character character)
        {
            if (_bossModeHandler != null && _bossModeHandler.IsBossMode)
            {
                return;
            }

            // 개별 몬스터 사망 시 스크롤 이동
            _scrollController?.MoveToNextPoint();

            // 모든 몬스터 사망 체크 (기존 로직 유지)
            if (_monsterSpawner == null || !_monsterSpawner.IsAllMonstersDefeated)
            {
                return;
            }

            StopNextWaveDelay();
            _nextWaveDelayCoroutine = StartCoroutine(StartNextWaveWithDelay());
        }

        // OnBossDeath() 수정 - 보스만 확인하도록
        private void OnBossDeath(Character character)
        {
            if (_bossModeHandler == null || !_bossModeHandler.IsBossMode)
            {
                return;
            }

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
            _bossModeHandler.ExitBossMode(true);
        }

        private void OnPlayerDespawn()
        {
            if (_bossModeHandler == null || !_bossModeHandler.IsBossMode)
            {
                return;
            }

            Log.Info(LogTags.Stage, "플레이어 사망. 보스 모드 종료.");
            _bossModeHandler.ExitBossMode();
        }

        private IEnumerator StartNextWaveWithDelay()
        {
            yield return new WaitForSeconds(_nextWaveDelay);

            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null || _currentStageAsset == null)
            {
                _nextWaveDelayCoroutine = null;
                yield break;
            }

            int nextWaveIndex = profile.Stage.CurrentWave + 1;
            if (nextWaveIndex >= _currentStageAsset.WaveCount)
            {
                StopWaveResetFade();
                _waveResetFadeCoroutine = StartCoroutine(StartWaveResetWithFade());
            }
            else
            {
                StartNextWave();
            }

            _nextWaveDelayCoroutine = null;
        }

        private IEnumerator StartWaveResetWithFade()
        {
            Log.Info(LogTags.Stage, "모든 웨이브 완료: {0}. 첫 웨이브부터 반복합니다.", Name);

            if (UIManager.Instance?.ScreenFader != null)
            {
                // 페이드 인/아웃 효과 적용
                UIManager.Instance.ScreenFader.FadeInOut(Color.black, 0.5f, 0, 0.3f);

                // 페이드 인 완료 대기 (페이드 인 시간 + 유지 시간)
                yield return new WaitForSeconds(0.5f + 0.3f);

                // 웨이브 리셋 및 첫 웨이브 시작
                Data.Game.VProfile profile = GetSelectedProfile();
                if (profile?.Stage != null)
                {
                    profile.Stage.CurrentWave = 0;
                    UIManager.Instance.HUDManager.SetStageProgress(1, _currentStageAsset.WaveCount);

                    // 웨이브 리셋 시 스크롤 리셋
                    _scrollController?.ResetToFirstPoint();

                    if (_monsterSpawner != null)
                    {
                        _monsterSpawner.SpawnWave(0);
                        SetPlayerTargetToFirstMonster();
                    }
                }

                // 페이드 아웃 완료 대기
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                // ScreenFader가 없을 경우 일반 처리
                Data.Game.VProfile profile = GetSelectedProfile();
                if (profile?.Stage != null)
                {
                    profile.Stage.CurrentWave = 0;
                    UIManager.Instance.HUDManager.SetStageProgress(1, _currentStageAsset.WaveCount);

                    // 웨이브 리셋 시 스크롤 리셋
                    _scrollController?.ResetToFirstPoint();

                    if (_monsterSpawner != null)
                    {
                        _monsterSpawner.SpawnWave(0);
                        SetPlayerTargetToFirstMonster();
                    }
                }
            }

            _waveResetFadeCoroutine = null;
        }

        private void StartNextWave()
        {
            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null || _currentStageAsset == null)
            {
                return;
            }

            int nextWaveIndex = profile.Stage.CurrentWave + 1;
            profile.Stage.CurrentWave = nextWaveIndex;

            UIManager.Instance.HUDManager.SetStageProgress(nextWaveIndex + 1, _currentStageAsset.WaveCount);

            if (_monsterSpawner != null)
            {
                _monsterSpawner.SpawnWave(profile.Stage.CurrentWave);
                SetPlayerTargetToFirstMonster();
            }
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

        private UIStageTitleNotice SpawnStageTitleNotice()
        {
            return ResourcesManager.SpawnStageTitleNotice(Name);
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

            _bossModeHandler?.EnterBossMode(profile.Stage.CurrentWave);
        }

        public float BossModeRemainingTime
        {
            get => _bossModeHandler != null ? _bossModeHandler.BossModeRemainingTime : 0f;
        }

        public float BossModeElapsedTime
        {
            get => _bossModeHandler != null ? _bossModeHandler.BossModeElapsedTime : 0f;
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

        private void StopNextWaveDelay()
        {
            if (_nextWaveDelayCoroutine != null)
            {
                StopCoroutine(_nextWaveDelayCoroutine);
                _nextWaveDelayCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "웨이브 딜레이 종료");
        }

        private void StopWaveResetFade()
        {
            if (_waveResetFadeCoroutine != null)
            {
                StopCoroutine(_waveResetFadeCoroutine);
                _waveResetFadeCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "웨이브 리셋 페이드 종료");
        }
    }
}