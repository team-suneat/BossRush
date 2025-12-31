using Lean.Pool;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        #region Fields

        private static readonly GameAddressableAssetManager _addressableManager = new();

        #endregion Fields

        #region Initialization

        public static void InitializeAddressables()
        {
            Log.Warning("비동기 모드에서는 InitializeAddressablesAsync()를 사용하세요.");
        }

        #endregion Initialization

        #region Resource Management

        public static bool IsResourceLoaded<T>(string path) where T : UnityEngine.Object
        {
            return _addressableManager.IsResourceLoaded(path);
        }

        public static void ReleaseResource<T>(T resource) where T : UnityEngine.Object
        {
            if (resource == null)
            {
                return;
            }

            _addressableManager.ReleaseResource(resource);
        }

        internal static T LoadAddressableResource<T>(string targetAddressablePath)
        {
            return default;
        }

        #endregion Resource Management

        #region Object Instantiation

        public static GameObject Instantiate(GameObject prefab, Transform parent = null, bool usePool = true)
        {
            return InstantiateInternal(prefab, null, parent, usePool);
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position, bool usePool = true)
        {
            return InstantiateInternal(prefab, position, null, usePool);
        }

        public static T Instantiate<T>(GameObject prefab, Transform parent = null, bool usePool = true)
        {
            return InstantiateInternal<T>(prefab, null, parent, usePool);
        }

        public static T Instantiate<T>(GameObject prefab, Vector3 position, bool usePool = true)
        {
            return InstantiateInternal<T>(prefab, position, null, usePool);
        }

        #endregion Object Instantiation

        #region Prefab Spawning

        public static GameObject SpawnPrefab(string prefabName, Vector3 position)
        {
            return SpawnPrefabInternal(prefabName, position);
        }

        public static GameObject SpawnPrefab(string prefabName, Transform parent)
        {
            return SpawnPrefabInternal(prefabName, null, parent);
        }

        public static T SpawnPrefab<T>(string prefabName, Vector3 position) where T : Component
        {
            return SpawnPrefabInternal<T>(prefabName, position);
        }

        public static T SpawnPrefab<T>(string prefabName, Transform parent) where T : Component
        {
            return SpawnPrefabInternal<T>(prefabName, null, parent);
        }

        #endregion Prefab Spawning

        #region Async Prefab Spawning

        public static async Task<T> SpawnPrefabAsync<T>(string prefabName, Vector3 position) where T : Component
        {
            return await SpawnPrefabInternalAsync<T>(prefabName, position);
        }

        public static async Task<T> SpawnPrefabAsync<T>(string prefabName, Transform parent) where T : Component
        {
            return await SpawnPrefabInternalAsync<T>(prefabName, null, parent);
        }

        public static async Task<GameObject> SpawnPrefabAsync(string prefabName, Vector3 position)
        {
            return await SpawnPrefabInternalAsync(prefabName, position);
        }

        public static async Task<GameObject> SpawnPrefabAsync(string prefabName, Transform parent)
        {
            return await SpawnPrefabInternalAsync(prefabName, null, parent);
        }

        #endregion Async Prefab Spawning

        #region Object Pooling

        public static void Despawn(GameObject clone)
        {
            if (clone.activeInHierarchy && clone.activeSelf)
            {
                if (false == Scenes.XScene.IsChangeScene)
                {
                    LeanPool.Despawn(clone);
                }
                else
                {
                    Log.Info(LogTags.Resource, "씬 전환 중에는 LeadPool을 사용하여 제거할 수 없습니다. {0}", clone.GetHierarchyName());
                }
            }
        }

        public static void Despawn(GameObject clone, float delay)
        {
            if (clone.activeInHierarchy && clone.activeSelf)
            {
                if (false == Scenes.XScene.IsChangeScene)
                {
                    LeanPool.Despawn(clone, delay);
                }
                else
                {
                    GameObject.Destroy(clone);
                    Log.Warning("씬 전환 중에는 LeadPool을 사용하여 제거할 수 없습니다. 게임오브젝트를 삭제합니다. {0}", clone.GetHierarchyName());
                }
            }
        }

        #endregion Object Pooling

        #region Addressable System

        public static bool IsAddressableSystemInitialized()
        {
            try
            {
                return UnityEngine.AddressableAssets.Addressables.ResourceManager != null;
            }
            catch
            {
                return false;
            }
        }

        public static void OnSceneUnload()
        {
            // 씬 전환 시 사용하지 않는 리소스 정리
            // 참조 카운트가 0인 리소스만 해제하거나, 특정 라벨의 리소스 해제
            Log.Info(LogTags.Resource, "씬 전환 시 리소스 정리 시작");
        }

        public static void ReleaseResourcesByLabel(string label)
        {
            _addressableManager.ReleaseResourcesByLabel(label);
        }

        public static int GetCachedResourceCount()
        {
            return _addressableManager.GetCachedResourceCount();
        }

        public static int GetActiveHandleCount()
        {
            return _addressableManager.GetActiveHandleCount();
        }

        public static int GetTotalReferenceCount()
        {
            return _addressableManager.GetTotalReferenceCount();
        }

        #endregion Addressable System

        #region Resource Loading

        public static T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            if (!IsValidPath(path, "리소스 경로"))
            {
                return null;
            }

            return _addressableManager.LoadResource<T>(path);
        }

        public static async Task<T> LoadResourceAsync<T>(string path) where T : UnityEngine.Object
        {
            if (!IsValidPath(path, "리소스 경로"))
            {
                return null;
            }

            try
            {
                T resource = await LoadAddressableResourceAsync<T>(path);
                if (resource != null)
                {
                    return resource;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(LogTags.Resource, $"Addressable 로드 실패. 경로:{path}, 메세지:{ex.Message}");
            }

            return null;
        }

        public static async Task<T> LoadAddressableResourceAsync<T>(string path) where T : UnityEngine.Object
        {
            if (!IsValidPath(path, "리소스 경로"))
            {
                return null;
            }

            return await _addressableManager.LoadResourceAsync<T>(path);
        }

        public static async Task<IList<T>> LoadResourcesByLabelAsync<T>(string label) where T : UnityEngine.Object
        {
            if (!IsValidPath(label, "라벨"))
            {
                return new List<T>();
            }

            try
            {
                return await _addressableManager.LoadResourcesByLabelAsync<T>(label);
            }
            catch (Exception ex)
            {
                Log.Error(LogTags.Resource, "라벨 리소스 로드 실패: {0}, 오류: {1}", label, ex.Message);
                return new List<T>();
            }
        }

        public static IList<T> LoadResourcesByLabelSync<T>(string label) where T : UnityEngine.Object
        {
            if (!IsValidPath(label, "라벨"))
            {
                return new List<T>();
            }

            try
            {
                return _addressableManager.LoadResourcesByLabelSync<T>(label);
            }
            catch (Exception ex)
            {
                Log.Error(LogTags.Resource, "라벨 리소스 동기 로드 실패: {0}, 오류: {1}", label, ex.Message);
                return new List<T>();
            }
        }

        #endregion Resource Loading

        #region Specialized Resource Loading

        public static SpriteAtlas LoadSpriteAtlas(string atlasPath)
        {
            return LoadResource<SpriteAtlas>(atlasPath);
        }

        public static Sprite LoadSprite(string spriteName, string atlasName)
        {
            string atlasPath = PathManager.FindAtlasPath(atlasName);
            if (!string.IsNullOrEmpty(atlasPath))
            {
                SpriteAtlas spriteAtlas = LoadResource<SpriteAtlas>(atlasPath);
                return spriteAtlas?.GetSprite(spriteName);
            }

            return null;
        }

        public static Texture2D LoadTexture(string texturePath)
        {
            return LoadResource<Texture2D>(texturePath);
        }

        public static AudioClip LoadAudioClip(string audioPath)
        {
            return LoadResource<AudioClip>(audioPath);
        }

        public static Material LoadMaterial(string materialPath)
        {
            return LoadResource<Material>(materialPath);
        }

        public static Font LoadFont(string fontPath)
        {
            return LoadResource<Font>(fontPath);
        }

        public static async Task<SpriteAtlas> LoadSpriteAtlasAsync(string atlasPath)
        {
            return await LoadResourceAsync<SpriteAtlas>(atlasPath);
        }

        public static async Task<Sprite> LoadSpriteAsync(string spritePath)
        {
            return await LoadResourceAsync<Sprite>(spritePath);
        }

        public static async Task<Texture2D> LoadTextureAsync(string texturePath)
        {
            return await LoadResourceAsync<Texture2D>(texturePath);
        }

        public static async Task<AudioClip> LoadAudioClipAsync(string audioPath)
        {
            return await LoadResourceAsync<AudioClip>(audioPath);
        }

        public static async Task<Material> LoadMaterialAsync(string materialPath)
        {
            return await LoadResourceAsync<Material>(materialPath);
        }

        public static async Task<Font> LoadFontAsync(string fontPath)
        {
            return await LoadResourceAsync<Font>(fontPath);
        }

        #endregion Specialized Resource Loading

        #region Advanced Resource Loading

        private static async Task<SpriteAtlas> LoadSpriteAtlasBySpriteNameAsync(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            string atlasName = GetAtlasNameBySpriteName(spriteName);
            string atlasPath = PathManager.FindAtlasPath(atlasName);
            return await LoadResourceAsync<SpriteAtlas>(atlasPath);
        }

        private static string GetAtlasNameBySpriteName(string spriteName)
        {
            if (spriteName.Contains("ui_item_icon_"))
            {
                return "atlas_resources_item";
            }
            else if (spriteName.Contains("ui_skill_icon_"))
            {
                return "atlas_resources_skill";
            }
            else if (spriteName.Contains("portrait"))
            {
                return "atlas_resources_npc";
            }
            else
            {
                return "atlas_resources";
            }
        }

        #endregion Advanced Resource Loading

        #region Private Helper Methods

        private static GameObject SpawnPrefabInternal(string prefabName, Vector3? position = null, Transform parent = null)
        {
            string prefabPath = PathManager.FindPrefabPath(prefabName);
            GameObject prefab = LoadResource<GameObject>(prefabPath);

            if (prefab == null)
            {
                LogPrefabSpawnFailure(prefabName, "캐시에서 찾기");
                return null;
            }

            return position.HasValue
                ? Instantiate(prefab, position.Value)
                : Instantiate(prefab, parent);
        }

        private static async Task<GameObject> SpawnPrefabInternalAsync(string prefabName, Vector3? position = null, Transform parent = null)
        {
            string prefabPath = PathManager.FindPrefabPath(prefabName);
            GameObject prefab = await LoadResourceAsync<GameObject>(prefabPath);

            if (prefab == null)
            {
                LogPrefabSpawnFailure(prefabName, "비동기 로드");
                return null;
            }

            return position.HasValue
                ? Instantiate(prefab, position.Value)
                : Instantiate(prefab, parent);
        }

        private static T SpawnPrefabInternal<T>(string prefabName, Vector3? position = null, Transform parent = null) where T : Component
        {
            GameObject instance = SpawnPrefabInternal(prefabName, position, parent);
            return instance?.GetComponent<T>();
        }

        private static async Task<T> SpawnPrefabInternalAsync<T>(string prefabName, Vector3? position = null, Transform parent = null) where T : Component
        {
            GameObject instance = await SpawnPrefabInternalAsync(prefabName, position, parent);
            return instance?.GetComponent<T>();
        }

        private static GameObject InstantiateInternal(GameObject prefab, Vector3? position = null, Transform parent = null, bool usePool = true)
        {
            if (prefab == null)
            {
                return null;
            }

            Vector3 spawnPosition = position ?? prefab.transform.position;
            Quaternion spawnRotation = prefab.transform.rotation;

            return usePool
                ? LeanPool.Spawn(prefab, spawnPosition, spawnRotation, parent)
                : UnityEngine.Object.Instantiate(prefab, spawnPosition, spawnRotation, parent);
        }

        private static T InstantiateInternal<T>(GameObject prefab, Vector3? position = null, Transform parent = null, bool usePool = true)
        {
            if (prefab == null)
            {
                return default;
            }

            GameObject clone = InstantiateInternal(prefab, position, parent, usePool);
            if (clone == null)
            {
                LogPrefabInstantiateFailure(prefab.name.ToDisableString());
                return default;
            }

            return clone.GetComponent<T>();
        }

        private static void LogResourceLoadFailure(string resourceName, string operation = "로드")
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Resource, "리소스를 {0}할 수 없습니다: {1}", operation, resourceName);
            }
        }

        private static void LogPrefabSpawnFailure(string prefabName, string operation = "스폰")
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Resource, "프리팹을 {0}할 수 없습니다: {1}", operation, prefabName);
            }
        }

        private static void LogPrefabInstantiateFailure(string prefabName)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Resource, "프리팹을 인스턴스화할 수 없습니다: {0}", prefabName);
            }
        }

        private static bool IsValidPath(string path, string pathType = "경로")
        {
            if (string.IsNullOrEmpty(path))
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Resource, "{0}이 비어 있습니다.", pathType);
                }
                return false;
            }
            return true;
        }

        #endregion Private Helper Methods
    }
}