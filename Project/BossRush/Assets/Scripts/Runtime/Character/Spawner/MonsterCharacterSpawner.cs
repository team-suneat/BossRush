using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterCharacterSpawner : XBehaviour
    {
        #region Private Fields

        [SerializeField]
        private Transform _spawnParentPoint;

        [SerializeField]
        private GameObject _monsterPrefab;

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

        public void Initialize()
        {
            SpawnedMonsters = new List<MonsterCharacter>();
        }

        public MonsterCharacter SpawnMonster()
        {
            if (_monsterPrefab == null)
            {
                return null;
            }

            Transform parentTransform = _spawnParentPoint != null ? _spawnParentPoint : transform;
            MonsterCharacter monster = ResourcesManager.SpawnMonsterCharacter(_monsterPrefab, parentTransform);
            if (monster == null)
            {
                if (_monsterPrefab != null)
                {
                    Log.Error(LogTags.CharacterSpawn, "몬스터 스폰 실패: {0}", _monsterPrefab.name);
                }
                else
                {
                    Log.Error(LogTags.CharacterSpawn, "몬스터 스폰 실패: {0}", this.GetHierarchyPath());
                }
                return null;
            }

            monster.Initialize();
            SpawnedMonsters.Add(monster);

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