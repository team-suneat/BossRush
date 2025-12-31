using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Stage;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterCharacterSpawner : XBehaviour
    {
        #region Private Fields

        [SerializeField]
        private Transform _spawnParentPoint;

        private StageAsset _currentStageAsset;
        private AreaAsset _currentAreaAsset;
        private Transform _scrollContainer;
        private StageScrollController _scrollController;

        #endregion Private Fields

        #region Properties

        public List<MonsterCharacter> SpawnedMonsters { get; private set; }

        public bool IsAllMonstersDefeated
        {
            get
            {
                if (SpawnedMonsters == null || SpawnedMonsters.Count == 0)
                {
                    return true;
                }

                for (int i = 0; i < SpawnedMonsters.Count; i++)
                {
                    if (SpawnedMonsters[i] != null && SpawnedMonsters[i].IsAlive)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        #endregion Properties

        #region Public Methods

        public void Initialize(StageAsset stageAsset, AreaAsset areaAsset, Transform scrollContainer = null, StageScrollController scrollController = null)
        {
            _currentStageAsset = stageAsset;
            _currentAreaAsset = areaAsset;
            _scrollContainer = scrollContainer;
            _scrollController = scrollController;
            SpawnedMonsters = new List<MonsterCharacter>();
        }

        public void SpawnWave(int waveIndex)
        {
            if (_currentStageAsset == null || _currentAreaAsset == null)
            {
                Log.Error(LogTags.CharacterSpawn, "스테이지 또는 지역 에셋이 설정되지 않았습니다.");
                return;
            }

            // 이전 웨이브의 죽은 몬스터들을 리스트에서 제거
            CleanupDeadMonsters();

            List<CharacterNames> monstersToSpawn = DetermineMonstersForWave(waveIndex);

            if (monstersToSpawn.Count == 0)
            {
                Log.Warning(LogTags.CharacterSpawn, "웨이브 {0}에 스폰할 몬스터가 없습니다.", waveIndex);
                return;
            }

            for (int i = 0; i < monstersToSpawn.Count; i++)
            {
                Vector3 spawnPos;
                if (_scrollController != null)
                {
                    spawnPos = _scrollController.GetSpawnPosition(i);
                }
                else
                {
                    Vector3 originPosition = transform.position;
                    spawnPos = new Vector3(
                        originPosition.x + (i * GameDefine.MONSTER_SPAWN_POSITION_PADDING),
                        originPosition.y,
                        originPosition.z
                    );
                }
                SpawnMonster(monstersToSpawn[i], spawnPos);
            }

            Log.Info(LogTags.CharacterSpawn, "웨이브 {0} 몬스터 스폰 완료: {1}마리", waveIndex, monstersToSpawn.Count);
        }

        public MonsterCharacter SpawnMonster(CharacterNames characterName, Vector3 spawnPosition)
        {
            if (characterName == CharacterNames.None)
            {
                Log.Error(LogTags.CharacterSpawn, "유효하지 않은 몬스터 이름입니다.");
                return null;
            }

            Transform parentTransform = _spawnParentPoint != null ? _spawnParentPoint : transform;
            MonsterCharacter monster = ResourcesManager.SpawnMonsterCharacter(characterName, parentTransform);

            if (monster == null)
            {
                Log.Error(LogTags.CharacterSpawn, "몬스터 스폰 실패: {0}", characterName);
                return null;
            }

            monster.transform.position = spawnPosition;
            monster.Initialize();

            SpawnedMonsters.Add(monster);

            Log.Info(LogTags.CharacterSpawn, "몬스터 스폰: {0} 위치: {1}", characterName, spawnPosition);

            return monster;
        }

        public void CleanupAllMonsters()
        {
            if (SpawnedMonsters == null)
            {
                return;
            }

            for (int i = SpawnedMonsters.Count - 1; i >= 0; i--)
            {
                if (SpawnedMonsters[i] != null)
                {
                    CleanupMonster(SpawnedMonsters[i]);
                }
            }

            SpawnedMonsters.Clear();
        }

        #endregion Public Methods

        #region Private Methods

        private List<CharacterNames> DetermineMonstersForWave(int waveIndex)
        {
            List<CharacterNames> result = new List<CharacterNames>();

            // 마지막 웨이브인지 확인
            bool isLastWave = _currentStageAsset != null && waveIndex == _currentStageAsset.WaveCount - 1;

            if (isLastWave)
            {
                // 마지막 웨이브면 모든 몬스터를 보물 상자로 교체
                for (int i = 0; i < _currentStageAsset.MonsterCountPerWave; i++)
                {
                    result.Add(CharacterNames.TreasureChest);
                }
            }
            else
            {
                // 일반 웨이브는 기존 로직 유지
                for (int i = 0; i < _currentStageAsset.MonsterCountPerWave; i++)
                {
                    CharacterNames normalMonster = GetRandomNormalMonster();
                    if (normalMonster != CharacterNames.None)
                    {
                        result.Add(normalMonster);
                    }
                }
            }

            return result;
        }

        private CharacterNames GetRandomNormalMonster()
        {
            if (_currentStageAsset.MonsterCandidates == null ||
                _currentStageAsset.MonsterCandidates.Count == 0)
            {
                Log.Error(LogTags.CharacterSpawn, "일반 몬스터 후보가 없습니다.");
                return CharacterNames.None;
            }

            int randomIndex = Random.Range(0, _currentStageAsset.MonsterCandidates.Count);
            int candidateIndex = _currentStageAsset.MonsterCandidates[randomIndex];

            if (_currentAreaAsset.NormalMonsters != null &&
                candidateIndex >= 0 &&
                candidateIndex < _currentAreaAsset.NormalMonsters.Length)
            {
                return _currentAreaAsset.NormalMonsters[candidateIndex];
            }

            Log.Error(LogTags.CharacterSpawn, "일반 몬스터 인덱스가 유효하지 않습니다: {0}", candidateIndex);
            return CharacterNames.None;
        }

        private void CleanupMonster(MonsterCharacter monster)
        {
            if (monster == null)
            {
                return;
            }
            monster.Despawn();
        }

        private void CleanupDeadMonsters()
        {
            if (SpawnedMonsters == null || SpawnedMonsters.Count == 0)
            {
                return;
            }

            for (int i = SpawnedMonsters.Count - 1; i >= 0; i--)
            {
                MonsterCharacter monster = SpawnedMonsters[i];
                if (monster == null || !monster.IsAlive)
                {
                    SpawnedMonsters.RemoveAt(i);
                }
            }
        }

        #endregion Private Methods
    }
}