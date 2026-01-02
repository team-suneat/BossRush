using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public class AttackSystem : XBehaviour
    {
        private Character _ownerCharacter;

        private Dictionary<HitmarkNames, AttackEntity> _entities = new();

        private List<HitmarkNames> _hitmarks = new List<HitmarkNames>();

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
            _hitmarks.Clear();
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

                    if (!_hitmarks.Contains(hitmarkName))
                    {
                        _hitmarks.Add(hitmarkName);
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
            if (_hitmarks.IsValid())
            {
                HitmarkNames hitmarkName = _hitmarks[0];
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
            if (_hitmarks.IsValid())
            {
                for (int i = 0; i < _hitmarks.Count; i++)
                {
                    Deactivate(_hitmarks[i]);
                }
            }
        }

        //

        public void OnDeath()
        {
            for (int i = 0; i < _hitmarks.Count; i++)
            {
                if (_entities.ContainsKey(_hitmarks[i]))
                {
                    _entities[_hitmarks[i]].OnOwnerDeath();
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

        public AttackEntity CreateAndRegisterEntity(HitmarkAssetData assetData)
        {
            if (_entities.ContainsKey(assetData.Name))
            {
                return _entities[assetData.Name];
            }

            AttackEntity attackEntity = null;

            if (assetData.EntityType == AttackEntityTypes.Target)
            {
                attackEntity = GameObjectEx.CreateGameObject<AttackTargetEntity>(assetData.Name.ToString(), transform);
            }

            if (attackEntity != null)
            {
                attackEntity.Name = assetData.Name;
                attackEntity.AutoGetOwnerComponents();
                attackEntity.Initialization();

                _entities.Add(assetData.Name, attackEntity);
                _hitmarks.Add(assetData.Name);
            }
            else
            {
                Log.Error($"공격 독립체({assetData.Name.ToLogString()})를 찾을 수 없습니다. 새로운 공격 독립체를 생성할 수 없습니다.");
            }

            return attackEntity;
        }

        public AttackEntity SpawnAndRegisterEntity(HitmarkNames hitmarkName)
        {
            if (!_entities.ContainsKey(hitmarkName))
            {
                AttackEntity attackEntity = ResourcesManager.SpawnAttackEntity(hitmarkName, transform);
                if (attackEntity != null)
                {
                    attackEntity.Name = hitmarkName;
                    attackEntity.SetOwner(_ownerCharacter);
                    attackEntity.AutoGetOwnerComponents();
                    attackEntity.Initialization();

                    _entities.Add(hitmarkName, attackEntity);
                    _hitmarks.Add(hitmarkName);
                }

                return attackEntity;
            }

            return null;
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