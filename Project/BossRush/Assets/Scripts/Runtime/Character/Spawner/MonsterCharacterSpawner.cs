using Sirenix.OdinInspector;
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

        [SerializeField]
        [InfoBox("몬스터 스폰 시 플레이어 타겟을 자동 설정")]
        private bool _isAutoSetTargetOnSpawn;

        [Title("#Respawn")]
        [SuffixLabel("리스폰 사용")]
        [SerializeField] private bool _useRespawn;

        [SuffixLabel("리스폰 지연 시간")]
        [SerializeField] private float _respawnDelayTime;

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

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent<Character>.Register(GlobalEventType.MONSTER_CHARACTER_DEATH, OnMonsterCharacterDeath);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent<Character>.Unregister(GlobalEventType.MONSTER_CHARACTER_DEATH, OnMonsterCharacterDeath);
        }

        private void OnMonsterCharacterDeath(Character character)
        {
            if (character is not MonsterCharacter monster || SpawnedMonsters == null)
            {
                return;
            }

            if (!SpawnedMonsters.Remove(monster))
            {
                return;
            }

            if (_useRespawn)
            {
                _ = CoroutineNextTimer(_respawnDelayTime, () => SpawnMonster());
            }
        }

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

            AutoSetTargetOnSpawn(monster);

            return monster;
        }

        private void AutoSetTargetOnSpawn(MonsterCharacter monster)
        {
            if (monster == null || !_isAutoSetTargetOnSpawn)
            {
                return;
            }

            PlayerCharacter player = CharacterManager.Instance.Player;
            if (player != null)
            {
                Log.Info(LogTags.CharacterSpawn, "몬스터 스폰 시 플레이어 타겟을 자동 설정합니다. 몬스터: {0}, 플레이어: {1}", monster.Name.ToLogString(), player.Name.ToLogString());
                monster.SetTarget(player);
            }
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