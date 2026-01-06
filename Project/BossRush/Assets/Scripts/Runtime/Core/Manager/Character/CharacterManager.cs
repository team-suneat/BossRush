using System.Collections.Generic;
using System.Linq;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        public int MonsterCount => Monsters != null ? Monsters.Count : 0;

        public PlayerCharacter Player { get; private set; }

        public List<MonsterCharacter> Monsters { get; private set; }

        public bool IsPause { get; set; }

        /// <summary> 소환된 스켈레톤의 최대 수 (엘리트 기술 : 해골 소환) </summary>
        public const int SUMMON_SKELETONS_MAX_COUNT = 20;

        private float _waitKillTime;

        public void Pause()
        {
            IsPause = true;
        }

        public void Resume()
        {
            IsPause = false;
        }

        public void Reset()
        {
            Monsters = new List<MonsterCharacter>();

            Player = null;
        }

        public void LogicUpdate()
        {
            if (Player != null)
            {
                Player.LogicUpdate();
            }

            if (Monsters.IsValid())
            {
                for (int i = 0; i < MonsterCount; i++)
                {
                    Monsters[i].LogicUpdate();
                }
            }

            _waitKillTime += Time.deltaTime;
        }

        public void PhysicsUpdate()
        {
            if (Scenes.XScene.IsChangeScene)
            {
                return;
            }

            if (Player != null)
            {
                Player.PhysicsUpdate();
            }

            if (!IsPause)
            {
                if (Monsters.IsValid())
                {
                    for (int i = 0; i < MonsterCount; i++)
                    {
                        Monsters[i].PhysicsUpdate();
                    }
                }
            }
        }

        #region Player

        public void RegisterPlayer(PlayerCharacter playerCharacter)
        {
            if (playerCharacter != null)
            {
                Player = playerCharacter;

                Log.Info(LogTags.CharacterSpawn, "[Manager] {0}(SID:{1}) 플레이어 캐릭터를 등록합니다.",
                    playerCharacter.Name.ToLogString(), playerCharacter.SID.ToSelectString());
            }
        }

        public void UnregisterPlayer(PlayerCharacter playerCharacter)
        {
            if (Player == null)
            {
                return;
            }

            if (playerCharacter != null)
            {
                if (Player == playerCharacter)
                {
                    Player = null;

                    Log.Info(LogTags.CharacterSpawn, "[Manager] {0}(SID:{1}) 인게임 몬스터 캐릭터를 등록 해제합니다.",
                        playerCharacter.Name.ToLogString(), playerCharacter.SID.ToSelectString());
                }
                else
                {
                    Log.Error("등록 해제하려는 플레이어가 같지 않습니다. 등록된:{0}(SID:{1}), 등록해제 시도:{2}(SID:{3})",
                        Player.Name.ToLogString(), Player.SID.ToSelectString(),
                        playerCharacter.Name.ToLogString(), playerCharacter.SID.ToSelectString());
                }
            }
        }

        public void UnregisterPlayer()
        {
            if (Player == null)
            {
                return;
            }

            Log.Info(LogTags.CharacterSpawn, "[Manager] {0}(SID:{1}) 플레이어 캐릭터를 등록 해제합니다.",
                    Player.Name.ToLogString(), Player.SID.ToSelectString());

            Player = null;
        }

        #endregion Player

        #region Monster

        public void Register(MonsterCharacter monsterCharacter)
        {
            if (monsterCharacter != null)
            {
                if (Monsters != null && !Monsters.Contains(monsterCharacter))
                {
                    Monsters.Add(monsterCharacter);
                    Monsters.RemoveNull();

                    Log.Info(LogTags.CharacterSpawn, "[Manager] {0}(SID:{1}) 인게임 몬스터 캐릭터를 등록합니다: {2}",
                        monsterCharacter.Name.ToLogString(), monsterCharacter.SID.ToSelectString(), Monsters.Count);
                }
            }
        }

        public void Unregister(MonsterCharacter monsterCharacter)
        {
            if (!Monsters.IsValid())
            {
                return;
            }

            if (monsterCharacter != null)
            {
                if (Monsters != null && Monsters.Contains(monsterCharacter))
                {
                    Monsters.Remove(monsterCharacter);
                    Monsters.RemoveNull();

                    Log.Info(LogTags.CharacterSpawn, "[Manager] {0}(SID:{1}) 인게임 몬스터 캐릭터를 등록 해제합니다: {2}",
                        monsterCharacter.Name.ToLogString(), monsterCharacter.SID.ToSelectString(), Monsters.Count);
                }
            }
        }

        /// <summary>
        /// 모든 몬스터를 자살시킵니다.
        /// </summary>
        public void SuicideAllMonsters(Character caster)
        {
            if (Monsters.IsValid())
            {
                for (int i = 0; i < Monsters.Count; i++)
                {
                    if (caster != null && caster == Monsters[i])
                    {
                        Log.Progress(LogTags.CharacterSpawn, "모든 몬스터 캐릭터를 자살시킬 때, 시전자를 제외합니다: {0}", caster.GetHierarchyName());
                        continue;
                    }

                    TakeInfinityDamage(Monsters[i]);
                }
            }
            else
            {
                Log.Warning(LogTags.CharacterSpawn, "등록된 몬스터 캐릭터가 없습니다. 모든 몬스터 캐릭터를 자살시킬 수 없습니다.");
            }
        }

        #endregion Monster

        public int GetMonsterCount(CharacterNames characterName)
        {
            return Monsters.FindAll(x => x.Name == characterName).Count();
        }

        #region 검색 (Find)

        public Character FindMonster(SID sid)
        {
            for (int i = 0; i < Monsters.Count; i++)
            {
                if (Monsters[i].SID == sid)
                {
                    return Monsters[i];
                }
            }

            return null;
        }

        public Character FindMonster(int index)
        {
            if (Monsters.IsValid())
            {
                if (Monsters.Count > index)
                {
                    return Monsters[index];
                }
            }

            return null;
        }

        public Character FindNearestMonster(Vector3 originPosition)
        {
            if (Monsters.IsValid())
            {
                Character result = null;
                Vector3 monsterPosition;
                float distance;
                float distanceMin = float.MaxValue;

                for (int i = 0; i < Monsters.Count; i++)
                {
                    Character monsterCharacter = Monsters[i];
                    if (!monsterCharacter.IsAlive)
                    {
                        continue;
                    }

                    monsterPosition = monsterCharacter.transform.position;
                    distance = Vector3.Distance(originPosition, monsterPosition);

                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        result = monsterCharacter;
                    }
                }

                return result;
            }

            return null;
        }

        public Character FindNearestMonsters(Vector3 originPosition)
        {
            if (Monsters.IsValid())
            {
                Character result = null;
                Vector3 monsterPosition;
                float distance;
                float distanceMin = float.MaxValue;

                for (int i = 0; i < Monsters.Count; i++)
                {
                    monsterPosition = Monsters[i].transform.position;
                    distance = Vector3.Distance(originPosition, monsterPosition);

                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        result = Monsters[i];
                    }
                }

                return result;
            }

            return null;
        }

        public Character FindFirstDamageableMonster()
        {
            if (Monsters.IsValid())
            {
                for (int i = 0; i < Monsters.Count; i++)
                {
                    if (!Monsters[i].IsAlive)
                    {
                        continue;
                    }

                    if (Monsters[i].MyVital.IsInvulnerable)
                    {
                        continue;
                    }

                    return Monsters[i];
                }
            }

            return null;
        }

        public Character FindBossMonster()
        {
            return Monsters.FirstOrDefault(x => x.IsBoss);
        }

        #endregion 검색 (Find)

        #region 강제 피해 (Force Damage)

        public void TakeInfinityDamage(CharacterNames characterName)
        {
            for (int i = 0; i < Monsters.Count; i++)
            {
                if (Monsters[i].Name == characterName)
                {
                    TakeInfinityDamage(Monsters[i]);
                }
            }
        }

        private void TakeInfinityDamage(Character targetCharacter)
        {
            DamageResult damageResult = new()
            {
                DamageValue = float.MaxValue,
                TargetVital = targetCharacter.MyVital,
                Attacker = targetCharacter
            };

            // 강제로 피해를 입힙니다.
            targetCharacter.MyVital.TakeDamage(damageResult);
        }

        /// <summary>
        /// 플레이어가 죽였다는 판정으로 모든 몬스터를 처치합니다.
        /// </summary>
        public void KillAllMonsters()
        {
            if (_waitKillTime < 1.5f)
            {
                UIManager.Instance.SpawnSoliloquyIngame(SoliloquyTypes.LockCheat);
                return;
            }
            _waitKillTime = 0;

            if (Monsters != null)
            {
                for (int i = 0; i < Monsters.Count; i++)
                {
                    DamageResult damageResult = new()
                    {
                        DamageValue = float.MaxValue,
                        TargetVital = Monsters[i].MyVital,
                        Attacker = Player,
                    };

                    // 강제로 피해를 입힙니다.
                    _ = Monsters[i].MyVital.TakeDamage(damageResult);
                }
            }
        }

        #endregion 강제 피해 (Force Damage)

        public void ClearMonsterAndAlliance()
        {
            Log.Info(LogTags.CharacterSpawn, "[Manager] 모든 인게임 캐릭터를 등록 해제합니다. 몬스터 수: {0}", Monsters.Count);

            Monsters.Clear();

            _waitKillTime = 0;
        }
    }
}