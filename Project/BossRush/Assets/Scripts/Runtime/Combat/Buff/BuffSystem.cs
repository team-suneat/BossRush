using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class BuffSystem : MonoBehaviour
    {
        private readonly Dictionary<BuffName, BuffEntity> _entities = new();
        private readonly List<BuffEntity> _entityList = new();

        public Character Owner { get; set; }

        private void Awake()
        {
            Owner = this.FindFirstParentComponent<Character>();
        }

        public bool Has(BuffName name)
        {
            return _entities.ContainsKey(name);
        }

        public void Add(BuffName buffName, int level, Character caster)
        {
            if (buffName == BuffName.None || Owner == null)
            {
                return;
            }

            BuffAssetData data = ScriptableDataManager.Instance.FindBuffClone(buffName);
            if (!data.IsValid())
            {
                Log.Warning(LogTags.Buff, "버프 데이터를 찾을 수 없습니다. {0}", buffName);
                return;
            }

            if (_entities.TryGetValue(buffName, out BuffEntity existing))
            {
                existing.AddStack(1);
                return;
            }

            BuffEntity entity = ResourcesManager.SpawnBuffEntity(buffName, transform);
            if (entity != null)
            {
                entity.Setup(data, Owner, caster, level);
                _entities[buffName] = entity;
                _entityList.Add(entity);
            }
            else
            {
                Log.Warning(LogTags.Buff, "버프 엔티티를 생성할 수 없습니다. {0}", buffName);
            }
        }

        public void Remove(BuffName name)
        {
            if (!_entities.TryGetValue(name, out BuffEntity entity))
            {
                return;
            }

            _ = _entities.Remove(name);
            _ = _entityList.Remove(entity);
            if (entity != null)
            {
                if (Owner != null)
                {
                    Log.Info(LogTags.Buff, "{0}에서 버프 제거: {1}", Owner.Name.ToLogString(), name.ToLogString());
                }
                entity.Despawn();
            }
        }

        public void Clear()
        {
            int count = _entityList.Count;
            for (int i = 0; i < _entityList.Count; i++)
            {
                if (_entityList[i] != null)
                {
                    _entityList[i].Despawn();
                }
            }
            _entities.Clear();
            _entityList.Clear();

            if (Owner != null && count > 0)
            {
                Log.Info(LogTags.Buff, "{0}의 모든 버프 제거: {1}개", Owner.Name.ToLogString(), count);
            }
        }

        public void LogicUpdate()
        {
            for (int i = 0; i < _entityList.Count; i++)
            {
                BuffEntity entity = _entityList[i];
                if (entity != null)
                {
                    entity.LogicUpdate();
                }
            }
        }
    }
}