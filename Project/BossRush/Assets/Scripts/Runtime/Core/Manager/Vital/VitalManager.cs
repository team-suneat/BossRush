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
            if (vital != null)
            {
                if (!_vitals.Contains(vital))
                {
                    _vitals.Add(vital);

                    AddColliders(vital);

                    Log.Info(LogTags.Vital, "[Manager] {0}(SID: {1}) 인게임 바이탈를 등록합니다.", vital.GetHierarchyName(), vital.SID.ToSelectString());
                }
                else
                {
                    Log.Warning(LogTags.Vital, "[Manager] 이미 등록된 바이탈을 등록할 수 없습니다. {0}", vital.GetHierarchyPath());
                }
            }
        }

        private void AddColliders(Vital vital)
        {
            if (vital.Collider != null)
            {
                if (!_colliders.ContainsKey(vital.Collider))
                {
                    _colliders.Add(vital.Collider, vital);

                    Log.Info(LogTags.Vital, "[Manager] 인게임 바이탈 충돌체를 등록합니다. {0}, Collider: {1}",
                        vital.GetHierarchyName(), vital.Collider.GetHierarchyPath());
                }
                else
                {
                    Log.Warning(LogTags.Vital, "이미 등록된 바이탈 충돌체를 등록할 수 없습니다. {0}, Collider: {1}",
                        vital.GetHierarchyPath(), vital.Collider.GetHierarchyPath());
                }
            }
            else if (vital.Colliders.IsValid())
            {
                for (int i = 0; i < vital.Colliders.Length; i++)
                {
                    if (!_colliders.ContainsKey(vital.Colliders[i]))
                    {
                        _colliders.Add(vital.Colliders[i], vital);

                        Log.Info(LogTags.Vital, "[Manager] 인게임 바이탈 충돌체를 등록합니다. {0}, Collider: {1}",
                            vital.GetHierarchyName(), vital.Colliders[i].GetHierarchyPath());
                    }
                    else
                    {
                        Log.Warning(LogTags.Vital, "[Manager] 이미 등록된 바이탈 충돌체를 등록할 수 없습니다. {0}, Collider: {1}",
                            vital.GetHierarchyPath(), vital.Colliders[i].GetHierarchyPath());
                    }
                }
            }
            else
            {
                Log.Error("[VitalManager] Vital의 Collider가 설정되지 않았습니다. {0}", vital.GetHierarchyPath());
            }
        }

        public void Remove(Vital vital)
        {
            if (vital != null)
            {
                if (_vitals.Contains(vital))
                {
                    _vitals.Remove(vital);

                    RemoveColliders(vital);

                    Log.Info(LogTags.Vital, "[Manager] {0}(SID: {1}) 인게임 바이탈를 해제합니다.", vital.GetHierarchyName(), vital.SID.ToSelectString());
                }
                else
                {
                    Log.Warning(LogTags.Vital, "[Manager] 등록되지않은 바이탈을 등록 해제할 수 없습니다. {0}", vital.GetHierarchyPath());
                }
            }
        }

        private void RemoveColliders(Vital vital)
        {
            if (_colliders.ContainsKey(vital.Collider))
            {
                _colliders.Remove(vital.Collider);

                Log.Info(LogTags.Vital, "[Manager] 인게임 바이탈 충돌체를 등록 해제합니다. {0}, Collider: {1}",
                    vital.GetHierarchyName(), vital.Collider.GetHierarchyPath());
            }
            else
            {
                Log.Warning(LogTags.Vital, "[Manager] 등록되지않은 바이탈 충돌체를 등록 해제할 수 없습니다. {0}",
                    vital.GetHierarchyPath(), vital.Collider.GetHierarchyPath());
            }

            if (vital.Colliders.IsValid())
            {
                for (int i = 0; i < vital.Colliders.Length; i++)
                {
                    if (_colliders.ContainsKey(vital.Collider))
                    {
                        _colliders.Remove(vital.Colliders[i]);

                        Log.Info(LogTags.Vital, "[Manager] 인게임 바이탈 충돌체를 등록 해제합니다. {0}, Collider: {1}",
                            vital.GetHierarchyName(), vital.Colliders[i].GetHierarchyPath());
                    }
                    else
                    {
                        Log.Warning(LogTags.Vital, "[Manager] 등록되지않은 바이탈 충돌체를 등록 해제할 수 없습니다. {0}",
                            vital.GetHierarchyPath(), vital.Collider.GetHierarchyPath());
                    }
                }
            }
        }

        public void Clear()
        {
            _vitals.Clear();

            Log.Info(LogTags.Vital, "[Manager] 모든 인게임 바이탈를 삭제/해제합니다.");
        }

        public Vital Find(Collider2D collider)
        {
            if (collider != null)
            {
                if (_colliders.ContainsKey(collider))
                {
                    return _colliders[collider];
                }
            }

            return null;
        }

        public Vital FindDamagable(Collider2D collider)
        {
            if (collider != null)
            {
                if (_colliders.ContainsKey(collider))
                {
                    Vital vital = _colliders[collider];
                    if (vital.Life.CheckInvulnerable())
                    {
                        return null;
                    }

                    return vital;
                }
            }

            return null;
        }

        public List<Vital> FindInBox(Vector3 position, Vector2 boxSize, LayerMask layerMask)
        {
            List<Vital> results = new List<Vital>();

            if (_vitals != null)
            {
                for (int i = 0; i < _vitals.Count; i++)
                {
                    Vital vital = _vitals[i];
                    if (vital == null) { continue; }
                    if (vital.Life == null) { continue; }
                    if (!vital.IsAlive) { continue; }
                    if (vital.Life.CheckInvulnerable()) { continue; }
                    if (!LayerEx.IsInMask(vital.gameObject.layer, layerMask)) { continue; }

                    if (vital.CheckColliderInBox(position, boxSize))
                    {
                        results.Add(vital);
                        Log.Info(LogTags.Detect, "후보 바이탈을 타겟에 추가합니다. {0}", vital.GetHierarchyPath());
                    }
                }
            }

            return results;
        }

        public List<Vital> FindInCircle(Vector3 position, float radius, LayerMask layerMask)
        {
            List<Vital> results = new List<Vital>();

            if (_vitals != null)
            {
                for (int i = 0; i < _vitals.Count; i++)
                {
                    Vital vital = _vitals[i];
                    if (vital == null) { continue; }
                    if (vital.Life == null) { continue; }
                    if (!vital.IsAlive) { continue; }
                    if (vital.Life.CheckInvulnerable()) { continue; }
                    if (!LayerEx.IsInMask(vital.gameObject.layer, layerMask))
                    {
                        continue;
                    }

                    if (vital.CheckColliderInCircle(position, radius))
                    {
                        results.Add(_vitals[i]);
                        Log.Info(LogTags.Detect, "후보 바이탈을 타겟에 추가합니다. {0}", vital.GetHierarchyPath());
                    }
                }
            }

            return results;
        }

        public List<Vital> FindInArc(Vector3 position, float radius, float arcAngle, bool isFacingRight, LayerMask layerMask)
        {
            List<Vital> results = new List<Vital>();

            if (_vitals != null)
            {
                for (int i = 0; i < _vitals.Count; i++)
                {
                    Vital vital = _vitals[i];
                    if (vital == null) { continue; }
                    if (vital.Life == null) { continue; }
                    if (!vital.IsAlive) { continue; }
                    if (vital.Life.CheckInvulnerable()) { continue; }
                    if (!LayerEx.IsInMask(vital.gameObject.layer, layerMask)) { continue; }

                    if (vital.CheckColliderInArc(position, radius, arcAngle, isFacingRight))
                    {
                        results.Add(vital);
                        Log.Info(LogTags.Detect, "후보 바이탈을 타겟에 추가합니다. {0}", vital.GetHierarchyPath());
                    }
                }
            }

            return results;
        }

        public List<Collider2D> FindColliderInBox(Vector3 position, Vector2 boxSize, LayerMask layerMask)
        {
            List<Collider2D> results = new List<Collider2D>();

            if (_vitals != null)
            {
                for (int i = 0; i < _vitals.Count; i++)
                {
                    if (_vitals[i] == null)
                    {
                        continue;
                    }

                    if (!_vitals[i].IsAlive)
                    {
                        continue;
                    }

                    if (!LayerEx.IsInMask(_vitals[i].gameObject.layer, layerMask))
                    {
                        continue;
                    }

                    Collider2D vitalCollider;
                    if (_vitals[i].CheckColliderInBox(position, boxSize, out vitalCollider))
                    {
                        results.Add(vitalCollider);
                        Log.Info(LogTags.Detect, "후보 바이탈 충돌체를 타겟에 추가합니다. {0}", vitalCollider.GetHierarchyPath());
                    }
                }
            }

            return results;
        }

        public List<Collider2D> FindColliderInCircle(Vector3 position, float radius, LayerMask layerMask)
        {
            List<Collider2D> results = new List<Collider2D>();

            if (_vitals != null)
            {
                for (int i = 0; i < _vitals.Count; i++)
                {
                    if (_vitals[i] == null)
                    {
                        continue;
                    }

                    if (!_vitals[i].IsAlive)
                    {
                        continue;
                    }

                    if (!LayerEx.IsInMask(_vitals[i].gameObject.layer, layerMask))
                    {
                        continue;
                    }

                    Collider2D vitalCollider;
                    if (_vitals[i].CheckColliderInCircle(position, radius, out vitalCollider))
                    {
                        results.Add(vitalCollider);
                        Log.Info(LogTags.Detect, "후보 바이탈을 타겟에 추가합니다. {0}", _vitals[i].GetHierarchyPath());
                    }
                }
            }

            return results;
        }
    }
}