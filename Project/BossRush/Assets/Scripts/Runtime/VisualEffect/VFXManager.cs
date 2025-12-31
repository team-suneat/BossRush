using System.Collections.Generic;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat
{
    public static class VFXManager
    {
        private static ListMultiMap<string, VFXObject> _effects = new ListMultiMap<string, VFXObject>();
        private static List<VFXObject> _parts = new List<VFXObject>();

        private const int PARTS_MAX_COUNT = 10;
        private const int VFX_MAX_COUNT = 50;

        //

        private static bool CheckMoreVFXThanMaxCount(string prefabName)
        {
            if (_effects.Count > VFX_MAX_COUNT)
            {
                ClearNull();

                if (_effects.Count > VFX_MAX_COUNT)
                {
                    Log.Warning(LogTags.Effect, "[Manager] VFXObject의 최대 개수를 초과합니다. VFXObject를 생성하지 않습니다: {0}", prefabName);
                    return true;
                }
            }

            return false;
        }

        public static bool CheckMorePartsThanMaxCount()
        {
            if (_parts.Count > PARTS_MAX_COUNT)
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Effect, "[Manager] Parts VFXObject 최대 개수를 초과합니다. Parts VFXObject를 생성하지 않습니다.");
                }
                return true;
            }

            return false;
        }

        //

        public static VFXObject Spawn(GameObject prefab, Character character, Transform parent = null)
        {
            if (prefab == null)
            {
                return null;
            }

            if (CheckMoreVFXThanMaxCount(prefab.name))
            {
                return null;
            }

            VFXObject visualEffect = ResourcesManager.Instantiate<VFXObject>(prefab, Vector3.zero);
            if (visualEffect != null)
            {
                visualEffect.SetParent(character, parent);
                visualEffect.SetPosition(character.transform.position, Vector2.zero);
                visualEffect.SetOwner(character);
                visualEffect.Initialize();
            }

            return visualEffect;
        }

        public static VFXObject Spawn(string prefabName, Character character, Transform parent = null)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return null;
            }

            if (CheckMoreVFXThanMaxCount(prefabName))
            {
                return null;
            }

            VFXObject visualEffect = ResourcesManager.SpawnPrefab<VFXObject>(prefabName, Vector3.zero);
            if (visualEffect != null)
            {
                visualEffect.SetParent(character, parent);
                visualEffect.SetPosition(character.transform.position, Vector2.zero);
                visualEffect.SetOwner(character);
                visualEffect.Initialize();
            }

            return visualEffect;
        }

        //

        public static VFXObject Spawn(GameObject prefab, Transform parent, bool isFacingRight)
        {
            if (prefab == null)
            {
                return null;
            }

            if (CheckMoreVFXThanMaxCount(prefab.name))
            {
                return null;
            }

            VFXObject visualEffect = ResourcesManager.Instantiate<VFXObject>(prefab, Vector3.zero);
            if (visualEffect != null)
            {
                visualEffect.SetParent(parent);
                visualEffect.SetDirection(isFacingRight);
                visualEffect.Initialize();
            }

            return visualEffect;
        }

        public static VFXObject Spawn(string prefabName, Transform parent, bool isFacingRight)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return null;
            }

            if (CheckMoreVFXThanMaxCount(prefabName))
            {
                return null;
            }

            VFXObject visualEffect = ResourcesManager.SpawnPrefab<VFXObject>(prefabName, Vector3.zero);
            if (visualEffect != null)
            {
                visualEffect.SetParent(parent);
                visualEffect.SetDirection(isFacingRight);
                visualEffect.Initialize();
            }

            return visualEffect;
        }

        //

        public static VFXObject Spawn(GameObject prefab, Vector3 spawnPosition, bool isFacingRight)
        {
            if (prefab == null)
            {
                return null;
            }

            if (CheckMoreVFXThanMaxCount(prefab.name))
            {
                return null;
            }

            VFXObject visualEffect = ResourcesManager.Instantiate<VFXObject>(prefab, spawnPosition);
            if (visualEffect != null)
            {
                visualEffect.SetDirection(isFacingRight);
                visualEffect.Initialize();
            }

            return visualEffect;
        }

        public static VFXObject Spawn(string prefabName, Vector3 spawnPosition, bool isFacingRight)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return null;
            }

            if (CheckMoreVFXThanMaxCount(prefabName))
            {
                return null;
            }

            VFXObject visualEffect = ResourcesManager.SpawnPrefab<VFXObject>(prefabName, spawnPosition);
            if (visualEffect != null)
            {
                visualEffect.SetDirection(isFacingRight);
                visualEffect.Initialize();
            }

            return visualEffect;
        }

        //

        public static void Register(VFXObject effect)
        {
            if (_effects.ContainsKey(effect.name))
            {
                _effects.Add(effect.name, effect);
            }
            else
            {
                _effects.Add(effect.name, effect);
            }

            Log.Progress(LogTags.Effect, "[Manager] VFXObject를 등록합니다: {0}, VFXObject 수: {1}", effect.GetHierarchyPath(), _effects.Count);
        }

        public static void Unregister(VFXObject effect)
        {
            if (_effects.ContainsKey(effect.name))
            {
                Log.Progress(LogTags.Effect, "[Manager] VFXObject를 등록 해제합니다: {0}, VFXObject 수: {1}", effect.GetHierarchyPath(), _effects.Count);
                _effects.Remove(effect.name, effect);
            }
            else
            {
                Log.Warning(LogTags.Effect, "[Manager] 등록되지 않은 VFXObject를 해제할 수 없습니다: {0}, VFXObject 수: {1}", effect.GetHierarchyPath(), _effects.Count);
            }
        }

        public static void RegisterParts(VFXObject effect)
        {
            _parts.Add(effect);

            Log.Progress(LogTags.Effect, "[Manager] PARTS VFXObject를 등록합니다: {0}, VFXObject 수: {1}", effect.GetHierarchyPath(), _effects.Count);
        }

        public static void DespawnFirstParts()
        {
            if (_parts != null && _parts.Count > 0)
            {
                VFXObject firstPartVFX = _parts[0];
                Log.Progress(LogTags.Effect, "[Manager] 첫 번째 PARTS VFXObject를 강제 삭제 후 등록 해제합니다: {0}, VFXObject 수: {1}", firstPartVFX.GetHierarchyPath(), _effects.Count);
                firstPartVFX.ForceDespawn();
                _parts.RemoveAt(0);
            }
        }

        public static void ClearNull()
        {
            int removeCount = _effects.ClearNull();
            Log.Progress(LogTags.Effect, "[Manager] 연결이 끊긴 VFXObject를 모두 등록 해제합니다: {0}, VFXObject 수: {1}", removeCount.ToColorString(GameColors.CreamIvory), _effects.Count);
        }
    }
}