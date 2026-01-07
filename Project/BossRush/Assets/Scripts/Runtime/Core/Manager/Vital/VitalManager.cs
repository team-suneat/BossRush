using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class VitalManager : Singleton<VitalManager>
    {
        private readonly List<Vital> _vitals = new List<Vital>();

        private readonly Dictionary<Collider2D, Vital> _colliders = new Dictionary<Collider2D, Vital>();

        public int Count => _vitals.Count;

        public void Add(Vital vital)
        {
            if (vital == null)
            {
                return;
            }

            if (!_vitals.Contains(vital))
            {
                _vitals.Add(vital);
                AddCollider(vital);

                Log.Info(LogTags.Vital, "[Manager] {0}(SID: {1}) 인게임 바이탈를 등록합니다.", vital.GetHierarchyName(), vital.SID.ToSelectString());
            }
            else
            {
                Log.Warning(LogTags.Vital, "[Manager] 이미 등록된 바이탈을 등록할 수 없습니다. {0}", vital.GetHierarchyPath());
            }
        }

        private void AddCollider(Vital vital)
        {
            if (vital.Collider == null)
            {
                Log.Error("[VitalManager] Vital의 Collider가 설정되지 않았습니다. {0}", vital.GetHierarchyPath());
                return;
            }

            if (!_colliders.ContainsKey(vital.Collider))
            {
                _colliders.Add(vital.Collider, vital);

                Log.Info(LogTags.Vital, "[Manager] 인게임 바이탈 충돌체를 등록합니다. {0}, Collider: {1}",
                    vital.GetHierarchyName(), vital.Collider.GetHierarchyPath());
            }
            else
            {
                Log.Warning(LogTags.Vital, "[Manager] 이미 등록된 바이탈 충돌체를 등록할 수 없습니다. {0}, Collider: {1}",
                    vital.GetHierarchyPath(), vital.Collider.GetHierarchyPath());
            }
        }

        public void Remove(Vital vital)
        {
            if (vital == null)
            {
                return;
            }

            if (_vitals.Contains(vital))
            {
                _vitals.Remove(vital);
                RemoveCollider(vital);

                Log.Info(LogTags.Vital, "[Manager] {0}(SID: {1}) 인게임 바이탈를 해제합니다.", vital.GetHierarchyName(), vital.SID.ToSelectString());
            }
            else
            {
                Log.Warning(LogTags.Vital, "[Manager] 등록되지않은 바이탈을 등록 해제할 수 없습니다. {0}", vital.GetHierarchyPath());
            }
        }

        private void RemoveCollider(Vital vital)
        {
            if (vital.Collider == null)
            {
                return;
            }

            if (_colliders.ContainsKey(vital.Collider))
            {
                _colliders.Remove(vital.Collider);

                Log.Info(LogTags.Vital, "[Manager] 인게임 바이탈 충돌체를 등록 해제합니다. {0}, Collider: {1}",
                    vital.GetHierarchyName(), vital.Collider.GetHierarchyPath());
            }
            else
            {
                Log.Warning(LogTags.Vital, "[Manager] 등록되지않은 바이탈 충돌체를 등록 해제할 수 없습니다. {0}",
                    vital.GetHierarchyPath(), vital.Collider?.GetHierarchyPath() ?? "null");
            }
        }

        public void Clear()
        {
            _vitals.Clear();
            _colliders.Clear();

            Log.Info(LogTags.Vital, "[Manager] 모든 인게임 바이탈를 삭제/해제합니다.");
        }

        public Vital Find(Collider2D collider)
        {
            if (collider == null)
            {
                return null;
            }

            if (_colliders.TryGetValue(collider, out Vital vital))
            {
                return vital;
            }

            return null;
        }

        public Vital FindDamageable(Collider2D collider)
        {
            if (collider == null)
            {
                return null;
            }

            if (_colliders.TryGetValue(collider, out Vital vital))
            {
                if (vital.Life != null && vital.Life.CheckInvulnerable())
                {
                    return null;
                }

                return vital;
            }

            return null;
        }

        public List<Vital> FindInBox(Vector3 position, Vector2 boxSize, LayerMask layerMask)
        {
            List<Vital> results = new List<Vital>();

            for (int i = 0; i < _vitals.Count; i++)
            {
                Vital vital = _vitals[i];
                if (!IsValidVitalForDetection(vital, layerMask))
                {
                    continue;
                }

                if (vital.CheckColliderInBox(position, boxSize))
                {
                    results.Add(vital);
                    Log.Info(LogTags.Detect, "후보 바이탈을 타겟에 추가합니다. {0}", vital.GetHierarchyPath());
                }
            }

            return results;
        }

        public List<Vital> FindInCircle(Vector3 position, float radius, LayerMask layerMask)
        {
            List<Vital> results = new List<Vital>();

            for (int i = 0; i < _vitals.Count; i++)
            {
                Vital vital = _vitals[i];
                if (!IsValidVitalForDetection(vital, layerMask))
                {
                    continue;
                }

                if (vital.CheckColliderInCircle(position, radius))
                {
                    results.Add(vital);
                    Log.Info(LogTags.Detect, "후보 바이탈을 타겟에 추가합니다. {0}", vital.GetHierarchyPath());
                }
            }

            return results;
        }

        public List<Vital> FindInArc(Vector3 position, float radius, float arcAngle, bool isFacingRight, LayerMask layerMask)
        {
            List<Vital> results = new List<Vital>();

            for (int i = 0; i < _vitals.Count; i++)
            {
                Vital vital = _vitals[i];
                if (!IsValidVitalForDetection(vital, layerMask))
                {
                    continue;
                }

                if (vital.CheckColliderInArc(position, radius, arcAngle, isFacingRight))
                {
                    results.Add(vital);
                    Log.Info(LogTags.Detect, "후보 바이탈을 타겟에 추가합니다. {0}", vital.GetHierarchyPath());
                }
            }

            return results;
        }

        public List<Collider2D> FindColliderInBox(Vector3 position, Vector2 boxSize, LayerMask layerMask)
        {
            List<Collider2D> results = new List<Collider2D>();

            for (int i = 0; i < _vitals.Count; i++)
            {
                Vital vital = _vitals[i];
                if (vital == null || !vital.IsAlive)
                {
                    continue;
                }

                if (!LayerEx.IsInMask(vital.gameObject.layer, layerMask))
                {
                    continue;
                }

                if (vital.CheckColliderInBox(position, boxSize, out Collider2D vitalCollider))
                {
                    results.Add(vitalCollider);
                    Log.Info(LogTags.Detect, "후보 바이탈 충돌체를 타겟에 추가합니다. {0}", vitalCollider.GetHierarchyPath());
                }
            }

            return results;
        }

        public List<Collider2D> FindColliderInCircle(Vector3 position, float radius, LayerMask layerMask)
        {
            List<Collider2D> results = new List<Collider2D>();

            for (int i = 0; i < _vitals.Count; i++)
            {
                Vital vital = _vitals[i];
                if (vital == null || !vital.IsAlive)
                {
                    continue;
                }

                if (!LayerEx.IsInMask(vital.gameObject.layer, layerMask))
                {
                    continue;
                }

                if (vital.CheckColliderInCircle(position, radius, out Collider2D vitalCollider))
                {
                    results.Add(vitalCollider);
                    Log.Info(LogTags.Detect, "후보 바이탈을 타겟에 추가합니다. {0}", vital.GetHierarchyPath());
                }
            }

            return results;
        }

        private bool IsValidVitalForDetection(Vital vital, LayerMask layerMask)
        {
            if (vital == null)
            {
                return false;
            }

            if (vital.Life == null)
            {
                return false;
            }

            if (!vital.IsAlive)
            {
                return false;
            }

            if (vital.Life.CheckInvulnerable())
            {
                return false;
            }

            if (!LayerEx.IsInMask(vital.gameObject.layer, layerMask))
            {
                return false;
            }

            return true;
        }
    }
}