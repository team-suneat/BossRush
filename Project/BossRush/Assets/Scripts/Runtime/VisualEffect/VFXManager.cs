using UnityEngine;

namespace TeamSuneat
{
    public static class VFXManager
    {
        private static readonly ListMultiMap<string, VFXObject> _effects = new();

        private const int VFX_MAX_COUNT = 50;

        private static bool CheckMoreVFXThanMaxCount(string prefabName)
        {
            if (_effects.Count <= VFX_MAX_COUNT)
            {
                return false;
            }

            ClearNull();

            if (_effects.Count > VFX_MAX_COUNT)
            {
                Log.Warning(LogTags.Effect, "[Manager] VFXObject의 최대 개수를 초과합니다. VFXObject를 생성하지 않습니다: {0}", prefabName);
                return true;
            }

            return false;
        }

        //

        public static VFXObject Spawn(GameObject prefab, Transform parent, bool isFacingRight)
        {
            if (prefab == null)
            {
                return null;
            }

            try
            {
                VFXObject visualEffect = SpawnInternal(prefab, prefab.name, Vector3.zero);
                if (visualEffect != null)
                {
                    SetupForTransform(visualEffect, parent, isFacingRight);
                }

                return visualEffect;
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Effect, "[Manager] VFXObject 생성 중 오류 발생: {0}", ex.Message);
                return null;
            }
        }

        public static VFXObject Spawn(GameObject prefab, Vector3 spawnPosition, bool isFacingRight)
        {
            if (prefab == null)
            {
                return null;
            }

            try
            {
                VFXObject visualEffect = SpawnInternal(prefab, prefab.name, spawnPosition);
                if (visualEffect != null)
                {
                    SetupForPosition(visualEffect, isFacingRight);
                }

                return visualEffect;
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Effect, "[Manager] VFXObject 생성 중 오류 발생: {0}", ex.Message);
                return null;
            }
        }

        public static VFXObject Spawn(string prefabName, Transform parent, bool isFacingRight)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return null;
            }

            try
            {
                VFXObject visualEffect = SpawnInternal(null, prefabName, Vector3.zero);
                if (visualEffect != null)
                {
                    SetupForTransform(visualEffect, parent, isFacingRight);
                }

                return visualEffect;
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Effect, "[Manager] VFXObject 생성 중 오류 발생: {0}", ex.Message);
                return null;
            }
        }

        public static VFXObject Spawn(string prefabName, Vector3 spawnPosition, bool isFacingRight)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return null;
            }

            try
            {
                VFXObject visualEffect = SpawnInternal(null, prefabName, spawnPosition);
                if (visualEffect != null)
                {
                    SetupForPosition(visualEffect, isFacingRight);
                }

                return visualEffect;
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Effect, "[Manager] VFXObject 생성 중 오류 발생: {0}", ex.Message);
                return null;
            }
        }

        //

        private static VFXObject SpawnInternal(GameObject prefab, string prefabName, Vector3 spawnPosition)
        {
            if (CheckMoreVFXThanMaxCount(prefabName))
            {
                return null;
            }

            VFXObject visualEffect = prefab != null
                ? ResourcesManager.Instantiate<VFXObject>(prefab, spawnPosition)
                : ResourcesManager.SpawnPrefab<VFXObject>(prefabName, spawnPosition);

            if (visualEffect != null)
            {
                visualEffect.Initialize();
            }

            return visualEffect;
        }

        private static void SetupForTransform(VFXObject visualEffect, Transform parent, bool isFacingRight)
        {
            visualEffect.SetParent(parent);
            visualEffect.SetPosition(parent);
            visualEffect.SetDirection(isFacingRight);
        }

        private static void SetupForPosition(VFXObject visualEffect, bool isFacingRight)
        {
            visualEffect.SetDirection(isFacingRight);
        }

        //

        public static void Register(VFXObject effect)
        {
            if (effect == null)
            {
                Log.Warning(LogTags.Effect, "[Manager] null인 VFXObject를 등록할 수 없습니다.");
                return;
            }

            if (_effects.ContainsKey(effect.name) && _effects.Contains(effect.name, effect))
            {
                Log.Warning(LogTags.Effect, "[Manager] 이미 등록된 VFXObject입니다: {0}", effect.GetHierarchyPath());
                return;
            }

            _effects.Add(effect.name, effect);
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

        public static void ClearNull()
        {
            int removeCount = _effects.ClearNull();
            Log.Progress(LogTags.Effect, "[Manager] 연결이 끊긴 VFXObject를 모두 등록 해제합니다: {0}, VFXObject 수: {1}", removeCount.ToColorString(GameColors.CreamIvory), _effects.Count);
        }
    }
}