using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public class SkillSystem : XBehaviour
    {
        private Character _ownerCharacter;

        [ShowInInspector]
        private Dictionary<SkillName, SkillEntity> _entities = new();

        [ShowInInspector]
        private List<SkillName> _skillList = new();

        public override void AutoNaming()
        {
            SetGameObjectName("#Skill");
        }

        //---------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            _ownerCharacter = this.FindFirstParentComponent<Character>();
            RegisterSkillEntitiesAll();
        }

        private void RegisterSkillEntitiesAll()
        {
            SkillEntity[] entities = GetComponentsInChildren<SkillEntity>();
            if (!entities.IsValid())
            {
                return;
            }

            _entities.Clear();
            for (int i = 0; i < entities.Length; i++)
            {
                SkillEntity entity = entities[i];
                if (entity.Name == SkillName.None)
                {
                    continue;
                }

                _entities[entity.Name] = entity;
                _skillList.Add(entity.Name);
            }
        }

        public virtual void Initialize()
        {
            if (_entities.IsValid())
            {
                foreach (KeyValuePair<SkillName, SkillEntity> item in _entities)
                {
                    item.Value.Initialization();
                }
            }
        }

        public void OnBattleReady()
        {
            if (_entities.IsValid())
            {
                foreach (KeyValuePair<SkillName, SkillEntity> item in _entities)
                {
                    item.Value.OnBattleReady();
                }
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        // 스킬 추가/제거 (BuffSystem 패턴)

        public void AddSkill(SkillName skillName, int level = 1)
        {
            if (skillName == SkillName.None || _ownerCharacter == null)
            {
                return;
            }

            if (_entities.ContainsKey(skillName))
            {
                Log.Warning(LogTags.Skill, "{0}, 스킬이 이미 등록되어 있습니다. {1}", _ownerCharacter.Name.ToLogString(), skillName.ToLogString());
                return;
            }

            SkillAssetData skillData = ScriptableDataManager.Instance.FindSkillClone(skillName);
            if (!skillData.IsValid())
            {
                Log.Warning(LogTags.Skill, "{0}, 스킬 데이터를 찾을 수 없습니다. {1}", _ownerCharacter.Name.ToLogString(), skillName.ToLogString());
                return;
            }

            SkillEntity entity = ResourcesManager.SpawnSkillEntity(skillName, transform);
            if (entity != null)
            {
                entity.SetOwner(_ownerCharacter);
                entity.SetLevel(level);
                entity.Initialization();

                // 획득 즉시 적용되는 스킬은 등록 직후 효과를 적용합니다.
                if (entity.AssetData != null && entity.AssetData.TriggerType == SkillTriggerType.OnAcquire)
                {
                    entity.OnAcquire();
                }

                _entities[skillName] = entity;
                if (!_skillList.Contains(skillName))
                {
                    _skillList.Add(skillName);
                }

                Log.Info(LogTags.Skill, "{0}에게 스킬 추가: {1} (레벨: {2})", _ownerCharacter.Name.ToLogString(), skillName.ToLogString(), level);
            }
            else
            {
                Log.Warning(LogTags.Skill, "{0}, 스킬 엔티티를 생성할 수 없습니다. {1}", _ownerCharacter.Name.ToLogString(), skillName.ToLogString());
            }
        }

        public void RemoveSkill(SkillName skillName)
        {
            if (skillName == SkillName.None || !_entities.TryGetValue(skillName, out SkillEntity entity))
            {
                return;
            }

            if (entity.IsActive)
            {
                entity.Deactivate();
            }

            _entities.Remove(skillName);
            _skillList.Remove(skillName);

            if (entity != null)
            {
                Log.Info(LogTags.Skill, "{0}에서 스킬 제거: {1}", _ownerCharacter.Name.ToLogString(), skillName.ToLogString());
                entity.Despawn();
            }
        }

        public void ClearAll()
        {
            int count = _skillList.Count;
            for (int i = 0; i < _skillList.Count; i++)
            {
                SkillName skillName = _skillList[i];
                if (_entities.TryGetValue(skillName, out SkillEntity entity))
                {
                    if (entity.IsActive)
                    {
                        entity.Deactivate();
                    }
                    entity.Despawn();
                }
            }

            _entities.Clear();
            _skillList.Clear();

            if (_ownerCharacter != null && count > 0)
            {
                Log.Info(LogTags.Skill, "{0}의 모든 스킬 제거: {1}개", _ownerCharacter.Name.ToLogString(), count);
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        // 스킬 활성화/비활성화 (AttackSystem 패턴)

        public bool TryActivate(SkillName skillName)
        {
            if (!_entities.ContainsKey(skillName))
            {
                LogFailedToFindEntity(skillName);
                return false;
            }

            return _entities[skillName].TryActivate();
        }

        public void Activate(SkillName skillName)
        {
            _entities[skillName].Activate();
        }

        public void Deactivate(SkillName skillName)
        {
            if (!_entities.IsValid() || !_entities.ContainsKey(skillName))
            {
                return;
            }

            SkillEntity entity = _entities[skillName];
            if (entity.IsActive)
            {
                entity.Deactivate();
            }
        }

        public void DeactivateAll()
        {
            if (_skillList.IsValid())
            {
                for (int i = 0; i < _skillList.Count; i++)
                {
                    Deactivate(_skillList[i]);
                }
            }
        }

        //---------------------------------------------------------------------------------------------------------------

        public void LogicUpdate()
        {
            if (_entities.IsValid())
            {
                foreach (KeyValuePair<SkillName, SkillEntity> item in _entities)
                {
                    item.Value.LogicUpdate();
                }
            }
        }

        public void OnDeath()
        {
            for (int i = 0; i < _skillList.Count; i++)
            {
                if (_entities.ContainsKey(_skillList[i]))
                {
                    _entities[_skillList[i]].OnOwnerDeath();
                }
            }
        }

        public bool HasSkill(SkillName skillName)
        {
            return _entities.ContainsKey(skillName);
        }

        public SkillEntity FindEntity(SkillName skillName)
        {
            if (_entities.ContainsKey(skillName))
            {
                return _entities[skillName];
            }

            return null;
        }

        //---------------------------------------------------------------------------------------------------------------
        // Log Methods

        private void LogFailedToFindEntity(SkillName skillName)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill, "{0}, 설정된 스킬 이름을 가진 Skill Entity를 찾을 수 없습니다. {1}", _ownerCharacter?.Name.ToLogString() ?? "Unknown", skillName.ToLogString());
            }
        }
    }
}