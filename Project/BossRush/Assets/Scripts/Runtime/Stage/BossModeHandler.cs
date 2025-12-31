using System;
using System.Collections;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat.Stage
{
    public class BossModeHandler : XBehaviour
    {
        private MonsterCharacterSpawner _monsterSpawner;
        private PlayerCharacterSpawner _playerSpawner;

        private StageAsset _currentStageAsset;
        private AreaAsset _currentAreaAsset;
        private Transform _spawnTransform;
        private StageLoader _stageLoader;

        private bool _isBossMode = false;
        private int _originalWave = 0;
        private Coroutine _enterBossModeFadeCoroutine;
        private Coroutine _exitBossModeFadeCoroutine;

        private Action _stopNextWaveDelay;
        private Action _stopWaveResetFade;
        private Action _setPlayerTargetToFirstMonster;
        private Action<int> _onWaveRestore;
        private Action<int, int> _onStageProgressUpdate;

        public bool IsBossMode => _isBossMode;

        public float BossModeRemainingTime
        {
            get
            {
                if (!_isBossMode)
                {
                    return 0f;
                }
                return GameTimerManager.Instance.GetRemainingTime(GameTimerLabels.BossMode);
            }
        }

        public float BossModeElapsedTime
        {
            get
            {
                if (!_isBossMode)
                {
                    return 0f;
                }
                return GameTimerManager.Instance.GetElapsedTime(GameTimerLabels.BossMode);
            }
        }

        public void Initialize(
            MonsterCharacterSpawner monsterSpawner,
            PlayerCharacterSpawner playerSpawner,
            StageAsset stageAsset,
            AreaAsset areaAsset,
            Transform spawnTransform,
            System.Action stopNextWaveDelay,
            System.Action stopWaveResetFade,
            System.Action setPlayerTargetToFirstMonster,
            System.Action<int> onWaveRestore,
            System.Action<int, int> onStageProgressUpdate,
            StageLoader stageLoader)
        {
            _monsterSpawner = monsterSpawner;
            _playerSpawner = playerSpawner;
            _currentStageAsset = stageAsset;
            _currentAreaAsset = areaAsset;
            _spawnTransform = spawnTransform;
            _stopNextWaveDelay = stopNextWaveDelay;
            _stopWaveResetFade = stopWaveResetFade;
            _setPlayerTargetToFirstMonster = setPlayerTargetToFirstMonster;
            _onWaveRestore = onWaveRestore;
            _onStageProgressUpdate = onStageProgressUpdate;
            _stageLoader = stageLoader;
        }

        public void Cleanup()
        {
            StopBossModeTimer();
            StopEnterBossModeFade();
            StopExitBossModeFade();
            _isBossMode = false;
        }

        public void EnterBossMode(int originalWave)
        {
            if (!ValidateBossModeEntry())
            {
                return;
            }

            Data.Game.VProfile profile = GetSelectedProfile();
            PrepareBossModeEntry(profile, originalWave);
            StopEnterBossModeFade();
            _enterBossModeFadeCoroutine = StartCoroutine(EnterBossModeWithFade());
        }

        public void ExitBossMode(bool isBossDefeated = false)
        {
            if (!_isBossMode)
            {
                return;
            }

            StopBossModeTimer();
            _isBossMode = false;

            // HUD 그룹 전환 및 게이지 비활성화
            GlobalEvent.Send(GlobalEventType.BOSS_MODE_EXITED);

            CharacterManager.Instance.SuicideAllMonsters(null);

            StopExitBossModeFade();
            _exitBossModeFadeCoroutine = StartCoroutine(ExitBossModeWithFade(isBossDefeated));
        }

        private IEnumerator EnterBossModeWithFade()
        {
            if (UIManager.Instance?.ScreenFader != null)
            {
                UIManager.Instance.ScreenFader.FadeInOut(Color.black, 0.5f, 0, 0.3f);
                yield return new WaitForSeconds(0.5f + 0.3f);
            }

            // HUD 그룹 전환 및 게이지 활성화
            GlobalEvent.Send(GlobalEventType.BOSS_MODE_ENTERED);

            SpawnBoss();
            StartBossModeTimer();

            if (UIManager.Instance?.ScreenFader != null)
            {
                yield return new WaitForSeconds(0.5f);
            }

            Log.Info(LogTags.Stage, "보스 모드 진입. 원래 웨이브: {0}", _originalWave);
            _enterBossModeFadeCoroutine = null;
        }

        private IEnumerator ExitBossModeWithFade(bool isBossDefeated = false)
        {
            yield return WaitForFadeOut();

            PlayerCharacter player = CharacterManager.Instance.Player;
            if (player != null)
            {
                RestorePlayerHealthAndMana(player);
            }

            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage != null && _currentStageAsset != null)
            {
                if (isBossDefeated)
                {
                    HandleBossDefeated(profile);
                }
                else
                {
                    HandleBossDefeatFailed(profile, player);
                }
            }

            yield return WaitForFadeIn();
            LogExitBossMode(isBossDefeated);

            _exitBossModeFadeCoroutine = null;
        }

        private void SpawnBoss()
        {
            if (!ValidateBossSpawnData())
            {
                return;
            }

            CharacterNames bossName = GetBossName();
            if (bossName == CharacterNames.None)
            {
                return;
            }

            Vector3 spawnPosition = GetBossSpawnPosition();
            MonsterCharacter boss = _monsterSpawner?.SpawnMonster(bossName, spawnPosition);
            if (boss != null)
            {
                SetupBossAfterSpawn(boss, bossName);
            }
        }

        private void StartBossModeTimer()
        {
            StopBossModeTimer();
            GameTimerManager.Instance.StartTimer(
                GameTimerLabels.BossMode,
                GameDefine.BOSS_MODE_TIME_LIMIT,
                onExpired: () =>
                {
                    if (_isBossMode)
                    {
                        Log.Info(LogTags.Stage, "보스 모드 시간 제한 도달. 원래 스테이지로 복귀합니다.");
                        ExitBossMode();
                    }
                }
            );
        }

        private void StopBossModeTimer()
        {
            GameTimerManager.Instance.StopTimer(GameTimerLabels.BossMode);
            Log.Progress(LogTags.Stage, "보스 모드 타이머 종료");
        }

        private void StopEnterBossModeFade()
        {
            if (_enterBossModeFadeCoroutine != null)
            {
                StopCoroutine(_enterBossModeFadeCoroutine);
                _enterBossModeFadeCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "보스 모드 진입 페이드 종료");
        }

        private void StopExitBossModeFade()
        {
            if (_exitBossModeFadeCoroutine != null)
            {
                StopCoroutine(_exitBossModeFadeCoroutine);
                _exitBossModeFadeCoroutine = null;
            }

            Log.Progress(LogTags.Stage, "보스 모드 종료 페이드 종료");
        }

        private Data.Game.VProfile GetSelectedProfile()
        {
            return GameApp.GetSelectedProfile();
        }

        #region Boss Mode Entry

        private bool ValidateBossModeEntry()
        {
            if (_isBossMode)
            {
                Log.Warning(LogTags.Stage, "이미 보스 모드입니다.");
                return false;
            }

            Data.Game.VProfile profile = GetSelectedProfile();
            if (profile?.Stage == null || _currentStageAsset == null)
            {
                Log.Warning(LogTags.Stage, "프로필 또는 스테이지 에셋이 없어 보스 모드에 진입할 수 없습니다.");
                return false;
            }

            return true;
        }

        private void PrepareBossModeEntry(Data.Game.VProfile profile, int originalWave)
        {
            _stopNextWaveDelay?.Invoke();
            _stopWaveResetFade?.Invoke();

            _isBossMode = true;
            _originalWave = originalWave;
            profile.Stage.ResetCurrentWave();

            CharacterManager.Instance.SuicideAllMonsters(null);
        }

        #endregion Boss Mode Entry

        #region Boss Mode Exit

        private IEnumerator WaitForFadeOut()
        {
            if (UIManager.Instance?.ScreenFader != null)
            {
                UIManager.Instance.ScreenFader.FadeInOut(Color.black, 0.5f, 0, 0.3f);
                yield return new WaitForSeconds(0.5f + 0.3f);
            }
        }

        private IEnumerator WaitForFadeIn()
        {
            if (UIManager.Instance?.ScreenFader != null)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void RestorePlayerHealthAndMana(PlayerCharacter player)
        {
            if (player == null || !player.IsAlive || player.MyVital == null)
            {
                return;
            }

            int healthToRestore = player.MyVital.MaxHealth - player.MyVital.CurrentHealth;
            if (healthToRestore > 0)
            {
                player.MyVital.Heal(healthToRestore);
            }

            if (player.MyVital.Mana != null)
            {
                int manaToRestore = player.MyVital.Mana.Max - player.MyVital.Mana.Current;
                if (manaToRestore > 0)
                {
                    player.MyVital.RestoreMana(manaToRestore);
                }
            }

            Log.Info(LogTags.Stage, "보스 모드 종료. 플레이어 체력 및 마나 회복 완료");
        }

        private void HandleBossDefeated(Data.Game.VProfile profile)
        {
            StageNames nextStage = (StageNames)((int)profile.Stage.CurrentStage + 1);
            if (nextStage != StageNames.None)
            {
                profile.Stage.SelectStage(nextStage);
                profile.Stage.UpdateMaxReachedStage(nextStage);
            }

            profile.Stage.ResetCurrentWave();
            GameApp.Instance.SaveGameData();

            Log.Info(LogTags.Stage, "보스 처치 완료. 스테이지 증가: {0}, 웨이브 초기화", nextStage);

            if (_stageLoader != null)
            {
                _stageLoader.CleanupStage();
                _stageLoader.LoadStage();
            }
        }

        private void HandleBossDefeatFailed(Data.Game.VProfile profile, PlayerCharacter player)
        {
            profile.Stage.CurrentWave = _originalWave;

            // 플레이어가 죽었다면 재생성
            if (player == null)
            {
                _playerSpawner.SpawnPlayer();
            }

            if (_monsterSpawner != null)
            {
                _monsterSpawner.SpawnWave(_originalWave);

                if (player != null && player.IsAlive)
                {
                    _setPlayerTargetToFirstMonster?.Invoke();
                }

                _onStageProgressUpdate?.Invoke(_originalWave + 1, _currentStageAsset.WaveCount);
            }
        }

        private void LogExitBossMode(bool isBossDefeated)
        {
            if (isBossDefeated)
            {
                Log.Info(LogTags.Stage, "보스 모드 종료. 다음 스테이지로 진행");
            }
            else
            {
                Log.Info(LogTags.Stage, "보스 모드 종료. 웨이브 {0}로 복귀", _originalWave);
            }
        }

        #endregion Boss Mode Exit

        #region Boss Spawn

        private bool ValidateBossSpawnData()
        {
            if (_currentStageAsset == null || _currentAreaAsset == null)
            {
                Log.Error(LogTags.Stage, "스테이지 또는 지역 에셋이 설정되지 않았습니다.");
                return false;
            }

            int bossIndex = _currentStageAsset.BossMonsterIndex;
            if (bossIndex < 0 || bossIndex >= _currentAreaAsset.BossMonsters.Length)
            {
                Log.Error(LogTags.Stage, "보스 몬스터 인덱스가 유효하지 않습니다: {0}", bossIndex);
                return false;
            }

            return true;
        }

        private CharacterNames GetBossName()
        {
            int bossIndex = _currentStageAsset.BossMonsterIndex;
            CharacterNames bossName = _currentAreaAsset.BossMonsters[bossIndex];

            if (bossName == CharacterNames.None)
            {
                Log.Error(LogTags.Stage, "보스 몬스터 이름이 유효하지 않습니다.");
                return CharacterNames.None;
            }

            return bossName;
        }

        private Vector3 GetBossSpawnPosition()
        {
            Vector3 spawnPosition = _spawnTransform != null ? _spawnTransform.position : transform.position;
            return spawnPosition;
        }

        private void SetupBossAfterSpawn(MonsterCharacter boss, CharacterNames bossName)
        {
            _setPlayerTargetToFirstMonster?.Invoke();
            UIManager.Instance?.HUDManager?.OnBossSpawned(boss);
            Log.Info(LogTags.Stage, "보스 스폰 완료: {0}", bossName);
        }

        #endregion Boss Spawn
    }
}