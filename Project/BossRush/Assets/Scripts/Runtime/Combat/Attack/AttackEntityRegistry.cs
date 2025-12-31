using System.Collections.Generic;

namespace TeamSuneat
{
    public class AttackEntityRegistry
    {
        private readonly Dictionary<HitmarkNames, AttackEntity> _entities = new();
        private readonly List<HitmarkNames> _hitmarks = new();

        public bool Contains(HitmarkNames hitmarkName)
        {
            return _entities.ContainsKey(hitmarkName);
        }

        public AttackEntity Find(HitmarkNames hitmarkName)
        {
            return _entities.TryGetValue(hitmarkName, out AttackEntity entity) ? entity : null;
        }

        public void Register(AttackEntity attackEntity)
        {
            if (!ValidateEntityForRegistration(attackEntity))
            {
                return;
            }

            HitmarkNames hitmarkName = attackEntity.Name;

            if (!_hitmarks.Contains(hitmarkName))
            {
                _hitmarks.Add(hitmarkName);
            }

            if (!_entities.ContainsKey(hitmarkName))
            {
                _entities.Add(hitmarkName, attackEntity);
            }
            else
            {
                Log.Warning(LogTags.Attack, "{0}, 같은 히트마크를 가진 공격 독립체가 있습니다. 등록에 실패했습니다.", attackEntity.Name.ToLogString());
            }
        }

        public void Unregister(HitmarkNames hitmarkName)
        {
            if (_entities.ContainsKey(hitmarkName))
            {
                _ = _entities.Remove(hitmarkName);
                _ = _hitmarks.Remove(hitmarkName);
            }
        }

        public void Clear()
        {
            _entities.Clear();
            _hitmarks.Clear();
        }

        public bool IsValid()
        {
            return _entities.Count > 0;
        }

        public IReadOnlyList<HitmarkNames> GetAllHitmarks()
        {
            return _hitmarks.AsReadOnly();
        }

        public IReadOnlyDictionary<HitmarkNames, AttackEntity> GetAllEntities()
        {
            return _entities;
        }

        private bool ValidateEntityForRegistration(AttackEntity attackEntity)
        {
            if (attackEntity == null)
            {
                Log.Warning(LogTags.Attack, "등록할 AttackEntity가 null입니다.");
                return false;
            }

            if (attackEntity.Name == HitmarkNames.None)
            {
                Log.Warning(LogTags.Attack, "AttackEntity의 히트마크 이름이 설정되지 않았습니다.");
                return false;
            }

            return true;
        }
    }
}