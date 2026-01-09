using System.Collections.Generic;

namespace TeamSuneat.Assets.Scripts.Runtime.Combat.Attack
{
    public class AttackSystem : XBehaviour
    {
        private Character _ownerCharacter;

        private readonly Dictionary<HitmarkNames, AttackEntity> _entities = new();

        private readonly List<HitmarkNames> _hitmarkList = new();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            AttackEntity[] entities = GetComponentsInChildren<AttackEntity>();
            if (entities.IsValid())
            {
                _entities.Clear();

                for (int i = 0; i < entities.Length; i++)
                {
                    if (_entities.ContainsKey(entities[i].Name))
                    {
                        Log.Error("{0}, 같은 히트마크를 가진 공격 독립체가 있습니다. 등록에 실패했습니다.", entities[i].Name.ToLogString());
                    }
                    else
                    {
                        _entities.Add(entities[i].Name, entities[i]);
                    }
                }
            }
        }

        public override void AutoNaming()
        {
            SetGameObjectName("#Attack");
        }

        //---------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            _ownerCharacter = this.FindFirstParentComponent<Character>();
        }

        public void Initialize()
        {
            _hitmarkList.Clear();
            _entities.Clear();

            RegisterAll();
        }

        private void RegisterAll()
        {
            AttackEntity[] entities = GetComponentsInChildren<AttackEntity>();
            if (entities.IsValid())
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    AttackEntity attackEntity = entities[i];
                    HitmarkNames hitmarkName = entities[i].Name;

                    if (!_hitmarkList.Contains(hitmarkName))
                    {
                        _hitmarkList.Add(hitmarkName);
                    }

                    if (!_entities.ContainsKey(hitmarkName))
                    {
                        entities[i].Initialization();
                        _entities.Add(hitmarkName, attackEntity);
                    }
                    else
                    {
                        Log.Warning(LogTags.Attack, "{0}, 같은 히트마크를 가진 공격 독립체가 있습니다. 등록에 실패했습니다.", entities[i].Name.ToLogString());
                    }
                }
            }
        }

        public void OnBattleReady()
        {
            if (_entities.IsValid())
            {
                foreach (KeyValuePair<HitmarkNames, AttackEntity> item in _entities)
                {
                    item.Value.OnBattleReady();
                }
            }
        }

        //---------------------------------------------------------------------------------------------------------------

        public void Activate()
        {
            if (_hitmarkList.IsValid())
            {
                HitmarkNames hitmarkName = _hitmarkList[0];
                AttackEntity entity = FindEntity(hitmarkName);
                if (entity != null)
                {
                    entity.Activate();
                }
                else
                {
                    LogFailedToFindEntity(hitmarkName);
                }
            }
        }

        public void Activate(string hitmarkNameString)
        {
            HitmarkNames hitmarkName = DataConverter.ToEnum<HitmarkNames>(hitmarkNameString);
            if (hitmarkName != HitmarkNames.None)
            {
                Activate(hitmarkName);
            }
            else
            {
                LogFailedToFindEntity(hitmarkName, hitmarkNameString);
            }
        }

        public void Activate(HitmarkNames hitmarkName)
        {
            if (_entities.ContainsKey(hitmarkName))
            {
                _entities[hitmarkName].Activate();
            }
            else
            {
                LogFailedToFindEntity(hitmarkName);
            }
        }

        public void Deactivate(string hitmarkNameString)
        {
            HitmarkNames hitmarkName = DataConverter.ToEnum<HitmarkNames>(hitmarkNameString);
            if (hitmarkName != HitmarkNames.None)
            {
                Deactivate(hitmarkName);
            }
            else
            {
                LogFailedToFindEntity(hitmarkName, hitmarkNameString);
            }
        }

        public void Deactivate(HitmarkNames hitmarkName)
        {
            if (!_entities.IsValid() || !_entities.ContainsKey(hitmarkName))
            {
                return;
            }

            AttackEntity attackEntity = _entities[hitmarkName];
            if (attackEntity.Level == 0)
            {
                LogFailedToDeactivate(attackEntity);
                return;
            }

            attackEntity.Deactivate();
        }

        public void DeactivateAll()
        {
            if (_hitmarkList.IsValid())
            {
                for (int i = 0; i < _hitmarkList.Count; i++)
                {
                    Deactivate(_hitmarkList[i]);
                }
            }
        }

        //

        public void OnDeath()
        {
            for (int i = 0; i < _hitmarkList.Count; i++)
            {
                if (_entities.ContainsKey(_hitmarkList[i]))
                {
                    _entities[_hitmarkList[i]].OnOwnerDeath();
                }
            }
        }

        public bool ContainEntity(HitmarkNames hitmarkName)
        {
            if (_entities.ContainsKey(hitmarkName))
            {
                return true;
            }

            return false;
        }

        public AttackEntity FindEntity(HitmarkNames hitmarkName)
        {
            if (_entities.ContainsKey(hitmarkName))
            {
                return _entities[hitmarkName];
            }

            return null;
        }

        public virtual bool CheckTargetInAttackableArea(HitmarkNames hitmarkName)
        {
            if (!ContainEntity(hitmarkName))
            {
                return false;
            }

            AttackEntity entity = FindEntity(hitmarkName);
            if (entity != null)
            {
                if (entity.EntityType == AttackEntityTypes.Area)
                {
                    AttackAreaEntity areaEntity = entity as AttackAreaEntity;
                    return areaEntity.CheckTargetInArea();
                }
            }

            return false;
        }

        // Log Methods

        private void LogFailedToFindEntity(HitmarkNames hitmarkName)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Attack, "{0}, 설정된 히트마크 이름을 가진 Attack Entity로 찾을 수 없습니다.", hitmarkName.ToLogString());
            }
        }

        private void LogFailedToFindEntity(HitmarkNames hitmarkName, string hitmarkNameString)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Attack, "{0} ({1}), 설정된 히트마크 이름을 가진 Attack Entity로 찾을 수 없습니다.", hitmarkName.ToLogString(), hitmarkNameString);
            }
        }

        private void LogFailedToDeactivate(AttackEntity attackEntity)
        {
            if (Log.LevelProgress)
            {
                Log.Progress("공격 독립체({0}의 레벨이 설정되지 않았습니다. 비활성화할 수 없습니다.", attackEntity.Name.ToLogString());
            }
        }
    }
}