using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterCharacterSpawner : XBehaviour
    {
        #region Private Fields

        [SerializeField]
        private Transform _spawnParentPoint;

        private StageAsset _currentStageAsset;

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

        public void Initialize(StageAsset stageAsset)
        {
            _currentStageAsset = stageAsset;
            SpawnedMonsters = new List<MonsterCharacter>();
        }

        public void SpawnMonsters()
        {
            if (_currentStageAsset == null)
            {
                Log.Error(LogTags.CharacterSpawn, "스테이지 에셋이 설정되지 않았습니다.");
                return;
            }

            // 죽은 몬스터들을 리스트에서 제거
            CleanupDeadMonsters();

            CharacterNames monsterToSpawn = DetermineMonsterToSpawn();

            if (monsterToSpawn == CharacterNames.None)
            {
                Log.Warning(LogTags.CharacterSpawn, "스폰할 몬스터가 없습니다.");
                return;
            }

            SpawnMonster(monsterToSpawn, transform.position);

            Log.Info(LogTags.CharacterSpawn, "몬스터 스폰 완료: {0}", monsterToSpawn);
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

        private CharacterNames DetermineMonsterToSpawn()
        {
            // 설정된 몬스터 이름이 있으면 우선 사용
            if (_currentStageAsset.MonsterCharacterName != CharacterNames.None)
            {
                return _currentStageAsset.MonsterCharacterName;
            }

            // 기존 시스템 사용 (하나만 선택)
            if (_currentStageAsset.MonsterCandidates != null &&
                _currentStageAsset.MonsterCandidates.Count > 0)
            {
                int randomIndex = Random.Range(0, _currentStageAsset.MonsterCandidates.Count);
                int candidateIndex = _currentStageAsset.MonsterCandidates[randomIndex];

                Log.Warning(LogTags.CharacterSpawn, "MonsterCandidates 인덱스 변환 로직이 구현되지 않았습니다. CharacterNames를 직접 설정해주세요.");
                return CharacterNames.None;
            }

            Log.Error(LogTags.CharacterSpawn, "스폰할 몬스터가 설정되지 않았습니다.");
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