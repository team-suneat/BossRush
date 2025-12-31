using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace TeamSuneat
{
    public class GameAddressableAssetManager
    {
        private static readonly Dictionary<string, object> _resourcesCache = new();
        private static readonly Dictionary<string, AsyncOperationHandle> _asyncOperationHandles = new();
        private static readonly Dictionary<string, int> _referenceCounts = new();

        #region 리소스 불러오기

        public T LoadResource<T>(string assetGuid) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetGuid))
            {
                Log.Warning("리소스 키가 비어 있습니다.");
                return null;
            }

            // 이미 캐시된 리소스가 있는지 확인
            if (_resourcesCache.TryGetValue(assetGuid, out object cachedResource))
            {
                return cachedResource as T;
            }

            return null;
        }

        public async Task<T> LoadResourceAsync<T>(string assetGuidOrKey) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetGuidOrKey))
            {
                Log.Warning("리소스 키가 비어 있습니다.");
                return null;
            }

            // Addressables 초기화 확인
            if (!IsAddressableSystemReady())
            {
                Log.Error(LogTags.Resource, "Addressable 시스템이 초기화되지 않았거나 빌드되지 않았습니다. 리소스를 로드할 수 없습니다: {0}", assetGuidOrKey);
                Log.Error(LogTags.Resource, "Unity Editor에서 Play 모드로 들어가기 전에 'Window > Asset Management > Addressables > Groups'에서 'Build > New Build > Default Build Script'를 실행하세요.");
                return null;
            }

            AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(assetGuidOrKey, typeof(T));

            try
            {
                IList<IResourceLocation> locations = await locationsHandle.Task;
                if (locations == null || locations.Count == 0)
                {
                    Log.Warning(LogTags.Resource, "Addressable 리소스 위치를 찾을 수 없습니다: {0}", assetGuidOrKey);
                    return null;
                }

                IResourceLocation location = locations[0];
                string cacheKey = location.PrimaryKey;

                if (_resourcesCache.TryGetValue(cacheKey, out object cachedResource))
                {
                    // 캐시된 리소스 사용 시 참조 카운트 증가
                    if (_referenceCounts.ContainsKey(cacheKey))
                    {
                        _referenceCounts[cacheKey]++;
                    }
                    else
                    {
                        _referenceCounts[cacheKey] = 1;
                    }
                    return cachedResource as T;
                }

                AsyncOperationHandle<T> asyncOperation = Addressables.LoadAssetAsync<T>(location);
                _asyncOperationHandles[cacheKey] = asyncOperation;

                T resource = await asyncOperation.Task;
                if (resource != null)
                {
                    Log.Info(LogTags.Resource, "AssetGUID를 키로 리소스를 캐시합니다: {0}", cacheKey);
                    _resourcesCache[cacheKey] = resource;
                    
                    // 참조 카운트 증가
                    if (_referenceCounts.ContainsKey(cacheKey))
                    {
                        _referenceCounts[cacheKey]++;
                    }
                    else
                    {
                        _referenceCounts[cacheKey] = 1;
                    }
                    
                    return resource;
                }

                Log.Error("Addressable 리소스 불러오기 실패: {0}", cacheKey);
                
                // 실패 시 핸들 정리
                if (_asyncOperationHandles.ContainsKey(cacheKey))
                {
                    Addressables.Release(_asyncOperationHandles[cacheKey]);
                    _asyncOperationHandles.Remove(cacheKey);
                }
                
                return null;
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;
                
                // 특정 에러 메시지에 대한 더 명확한 안내
                if (errorMessage.Contains("Invalid path") || errorMessage.Contains("settings.json"))
                {
                    Log.Error(LogTags.Resource, "Addressable 리소스 불러오기 실패: {0}", assetGuidOrKey);
                    Log.Error(LogTags.Resource, "Addressables가 빌드되지 않았습니다. Unity Editor에서 다음을 수행하세요:");
                    Log.Error(LogTags.Resource, "1. Window > Asset Management > Addressables > Groups 메뉴 열기");
                    Log.Error(LogTags.Resource, "2. Build > New Build > Default Build Script 실행");
                    Log.Error(LogTags.Resource, "3. 또는 Play Mode Script를 'Use Asset Database'로 설정 (Window > Asset Management > Addressables > Settings > Play Mode Script)");
                }
                else
                {
                    Log.Error(LogTags.Resource, "Addressable 리소스 불러오기 중 오류 발생: {0}, 오류: {1}", assetGuidOrKey, errorMessage);
                }
                
                return null;
            }
            finally
            {
                Addressables.Release(locationsHandle);
            }
        }

        public async Task<T> LoadResourceAsync<T>(AssetReference assetReference) where T : UnityEngine.Object
        {
            if (assetReference == null)
            {
                Log.Warning(LogTags.Resource, "AssetReference가 null입니다.");
                return null;
            }

            string key = assetReference.AssetGUID;

            // 이미 캐시된 리소스가 있는지 확인
            if (_resourcesCache.TryGetValue(key, out object cachedResource))
            {
                // 캐시된 리소스 사용 시 참조 카운트 증가
                if (_referenceCounts.ContainsKey(key))
                {
                    _referenceCounts[key]++;
                }
                else
                {
                    _referenceCounts[key] = 1;
                }
                Log.Info(LogTags.Resource, "캐시된 리소스를 사용합니다: {0}", key);
                return cachedResource as T;
            }

            try
            {
                // 비동기 불러오기 시작
                AsyncOperationHandle<T> asyncOperation = assetReference.LoadAssetAsync<T>();
                _asyncOperationHandles[key] = asyncOperation;

                T resource = await asyncOperation.Task;

                if (resource != null)
                {
                    _resourcesCache[key] = resource;
                    
                    // 참조 카운트 증가
                    if (_referenceCounts.ContainsKey(key))
                    {
                        _referenceCounts[key]++;
                    }
                    else
                    {
                        _referenceCounts[key] = 1;
                    }
                    
                    Log.Info(LogTags.Resource, "AssetReference 리소스를 비동기로 불러오기했습니다: {0}", key);
                    return resource;
                }
                else
                {
                    Log.Error(LogTags.Resource, "AssetReference 리소스 불러오기 실패: {0}", key);
                    
                    // 실패 시 핸들 정리
                    if (_asyncOperationHandles.ContainsKey(key))
                    {
                        Addressables.Release(_asyncOperationHandles[key]);
                        _asyncOperationHandles.Remove(key);
                    }
                    
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Resource, "AssetReference 리소스 불러오기 중 오류 발생: {0}, 오류: {1}", key, ex.Message);
                
                // 예외 발생 시 핸들 정리
                if (_asyncOperationHandles.ContainsKey(key))
                {
                    Addressables.Release(_asyncOperationHandles[key]);
                    _asyncOperationHandles.Remove(key);
                }
                
                return null;
            }
        }

        public async Task<IList<T>> LoadResourcesByLabelAsync<T>(string label) where T : UnityEngine.Object
        {
            // Addressables 초기화 확인
            if (!IsAddressableSystemReady())
            {
                Log.Error(LogTags.Resource, "Addressable 시스템이 초기화되지 않았거나 빌드되지 않았습니다. 라벨 리소스를 로드할 수 없습니다: {0}", label);
                Log.Error(LogTags.Resource, "Unity Editor에서 Play 모드로 들어가기 전에 'Window > Asset Management > Addressables > Groups'에서 'Build > New Build > Default Build Script'를 실행하세요.");
                return new List<T>();
            }

            try
            {
                // 먼저 라벨에 해당하는 리소스 위치를 확인
                AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
                
                try
                {
                    IList<IResourceLocation> locations = await locationsHandle.Task;
                    
                    if (locations == null || locations.Count == 0)
                    {
                        Log.Warning(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 찾을 수 없습니다.", label, typeof(T).Name);
                        return new List<T>();
                    }

                    // 실제 리소스 로드
                    AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
                    _asyncOperationHandles[label] = handle;

                    IList<T> assets = await handle.Task;
                    
                    if (assets != null && assets.Count > 0)
                    {
                        Log.Info(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 {2}개 불러왔습니다.", label, typeof(T).Name, assets.Count);
                        await CacheResourcesByLocations(label, assets);
                        return assets;
                    }
                    else
                    {
                        Log.Warning(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 불러왔지만 비어있습니다.", label, typeof(T).Name);
                        return new List<T>();
                    }
                }
                finally
                {
                    Addressables.Release(locationsHandle);
                }
            }
            catch (UnityEngine.AddressableAssets.InvalidKeyException ex)
            {
                Log.Error(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 찾을 수 없습니다. 라벨이 존재하지 않거나 해당 타입의 에셋이 없습니다.", label, typeof(T).Name);
                Log.Error(LogTags.Resource, "에러 상세: {0}", ex.Message);
                Log.Error(LogTags.Resource, "해결 방법: Addressables Groups에서 '{0}' 라벨이 올바르게 설정되어 있는지 확인하세요.", label);
                return new List<T>();
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;
                
                if (errorMessage.Contains("not assignable") || errorMessage.Contains("InvalidKeyException"))
                {
                    Log.Error(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 로드할 수 없습니다.", label, typeof(T).Name);
                    Log.Error(LogTags.Resource, "원인: 라벨에 해당 타입의 에셋이 없거나 타입이 맞지 않습니다.");
                    Log.Error(LogTags.Resource, "해결 방법: Addressables Groups에서 '{0}' 라벨이 {1} 타입의 에셋에만 할당되어 있는지 확인하세요.", label, typeof(T).Name);
                }
                else
                {
                    Log.Error(LogTags.Resource, "{0} 라벨 리소스 불러오기 중 오류 발생: {1}", label, errorMessage);
                }
                
                return new List<T>();
            }
        }

        public IList<T> LoadResourcesByLabelSync<T>(string label) where T : UnityEngine.Object
        {
            // Addressables 초기화 확인
            if (!IsAddressableSystemReady())
            {
                Log.Error(LogTags.Resource, "Addressable 시스템이 초기화되지 않았거나 빌드되지 않았습니다. 라벨 리소스를 로드할 수 없습니다: {0}", label);
                Log.Error(LogTags.Resource, "Unity Editor에서 Play 모드로 들어가기 전에 'Window > Asset Management > Addressables > Groups'에서 'Build > New Build > Default Build Script'를 실행하세요.");
                return new List<T>();
            }

            try
            {
                // 먼저 라벨에 해당하는 리소스 위치를 확인
                AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
                IList<IResourceLocation> locations = locationsHandle.WaitForCompletion();
                
                try
                {
                    if (locations == null || locations.Count == 0)
                    {
                        Log.Warning(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 찾을 수 없습니다.", label, typeof(T).Name);
                        return new List<T>();
                    }

                    // 실제 리소스 로드 (동기)
                    AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
                    _asyncOperationHandles[label] = handle;

                    IList<T> assets = handle.WaitForCompletion();
                    
                    if (assets != null && assets.Count > 0)
                    {
                        Log.Info(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 {2}개 동기적으로 불러왔습니다.", label, typeof(T).Name, assets.Count);
                        CacheResourcesByLocationsSync(label, assets, locations);
                        return assets;
                    }
                    else
                    {
                        Log.Warning(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 불러왔지만 비어있습니다.", label, typeof(T).Name);
                        return new List<T>();
                    }
                }
                finally
                {
                    Addressables.Release(locationsHandle);
                }
            }
            catch (UnityEngine.AddressableAssets.InvalidKeyException ex)
            {
                Log.Error(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 찾을 수 없습니다. 라벨이 존재하지 않거나 해당 타입의 에셋이 없습니다.", label, typeof(T).Name);
                Log.Error(LogTags.Resource, "에러 상세: {0}", ex.Message);
                Log.Error(LogTags.Resource, "해결 방법: Addressables Groups에서 '{0}' 라벨이 올바르게 설정되어 있는지 확인하세요.", label);
                return new List<T>();
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message;
                
                if (errorMessage.Contains("not assignable") || errorMessage.Contains("InvalidKeyException"))
                {
                    Log.Error(LogTags.Resource, "{0} 라벨로 {1} 타입의 리소스를 로드할 수 없습니다.", label, typeof(T).Name);
                    Log.Error(LogTags.Resource, "원인: 라벨에 해당 타입의 에셋이 없거나 타입이 맞지 않습니다.");
                    Log.Error(LogTags.Resource, "해결 방법: Addressables Groups에서 '{0}' 라벨이 {1} 타입의 에셋에만 할당되어 있는지 확인하세요.", label, typeof(T).Name);
                }
                else
                {
                    Log.Error(LogTags.Resource, "{0} 라벨 리소스 불러오기 중 오류 발생: {1}", label, errorMessage);
                }
                
                return new List<T>();
            }
        }

        #endregion 리소스 불러오기

        #region 리소스 해제

        public void ReleaseResource<T>(T resource) where T : UnityEngine.Object
        {
            if (resource == null)
            {
                return;
            }

            // 리소스 이름으로 키 찾기 (캐시에서)
            string keyToRelease = null;
            foreach (var kvp in _resourcesCache)
            {
                if (kvp.Value == resource)
                {
                    keyToRelease = kvp.Key;
                    break;
                }
            }

            if (keyToRelease != null)
            {
                ReleaseResource(keyToRelease);
            }
            else
            {
                // 키를 찾을 수 없어도 Addressables.Release는 호출
                Addressables.Release(resource);
                Log.Info(LogTags.Resource, "리소스를 해제했습니다: {0} (키를 찾을 수 없음)", resource.name);
            }
        }

        public void ReleaseResource(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            // 참조 카운트 감소
            if (_referenceCounts.TryGetValue(key, out int count))
            {
                count--;
                if (count <= 0)
                {
                    // 참조 카운트가 0 이하가 되면 실제로 해제
                    _referenceCounts.Remove(key);

                    if (_asyncOperationHandles.TryGetValue(key, out AsyncOperationHandle handle))
                    {
                        Addressables.Release(handle);
                        _asyncOperationHandles.Remove(key);
                    }

                    if (_resourcesCache.ContainsKey(key))
                    {
                        _resourcesCache.Remove(key);
                    }

                    Log.Info(LogTags.Resource, "리소스를 해제했습니다: {0}", key);
                }
                else
                {
                    // 아직 참조가 남아있으면 카운트만 감소
                    _referenceCounts[key] = count;
                    Log.Info(LogTags.Resource, "리소스 참조 카운트 감소: {0} (남은 참조: {1})", key, count);
                }
            }
            else
            {
                // 참조 카운트가 없으면 바로 해제 (하위 호환성)
                if (_asyncOperationHandles.TryGetValue(key, out AsyncOperationHandle handle))
                {
                    Addressables.Release(handle);
                    _asyncOperationHandles.Remove(key);
                }

                if (_resourcesCache.ContainsKey(key))
                {
                    _resourcesCache.Remove(key);
                }

                Log.Info(LogTags.Resource, "리소스를 해제했습니다: {0} (참조 카운트 없음)", key);
            }
        }

        public void ReleaseAllResources()
        {
            foreach (AsyncOperationHandle handle in _asyncOperationHandles.Values)
            {
                Addressables.Release(handle);
            }

            _asyncOperationHandles.Clear();
            _resourcesCache.Clear();
            _referenceCounts.Clear();

            Log.Info(LogTags.Resource, "모든 Addressable 리소스를 해제했습니다.");
        }

        public void ReleaseResourcesByLabel(string label)
        {
            if (string.IsNullOrEmpty(label))
            {
                return;
            }

            List<string> keysToRelease = new List<string>();
            foreach (string key in _resourcesCache.Keys)
            {
                // 라벨로 로드된 리소스는 라벨을 키로 사용하거나, 라벨과 연관된 키를 찾아야 함
                // 실제 구현은 라벨과 키의 매핑이 필요할 수 있음
                // 여기서는 간단히 라벨이 키에 포함되어 있는지 확인
                if (key.Contains(label))
                {
                    keysToRelease.Add(key);
                }
            }

            foreach (string key in keysToRelease)
            {
                ReleaseResource(key);
            }

            Log.Info(LogTags.Resource, "{0} 라벨의 리소스 {1}개를 해제했습니다.", label, keysToRelease.Count);
        }

        #endregion 리소스 해제

        #region 메모리 모니터링

        public int GetCachedResourceCount()
        {
            return _resourcesCache.Count;
        }

        public int GetActiveHandleCount()
        {
            return _asyncOperationHandles.Count;
        }

        public int GetTotalReferenceCount()
        {
            int total = 0;
            foreach (int count in _referenceCounts.Values)
            {
                total += count;
            }
            return total;
        }

        public Dictionary<string, int> GetReferenceCounts()
        {
            return new Dictionary<string, int>(_referenceCounts);
        }

        #endregion 메모리 모니터링

        #region 유틸리티

        private bool IsAddressableSystemReady()
        {
            try
            {
                // ResourceManager가 null이 아니고, 초기화가 완료되었는지 확인
                if (Addressables.ResourceManager == null)
                {
                    return false;
                }

                // Editor 모드에서는 추가 확인
#if UNITY_EDITOR
                // Editor 모드에서 Addressables가 제대로 설정되었는지 확인
                var settings = Addressables.ResourceManager.ResourceProviders;
                if (settings == null)
                {
                    return false;
                }
#endif

                return true;
            }
            catch (System.Exception ex)
            {
                Log.Warning(LogTags.Resource, "Addressable 시스템 확인 중 오류: {0}", ex.Message);
                return false;
            }
        }

        public bool IsResourceLoaded(string key)
        {
            return _resourcesCache.ContainsKey(key);
        }

        public bool IsResourceLoaded(AssetReference assetReference)
        {
            if (assetReference == null)
            {
                return false;
            }

            return IsResourceLoaded(assetReference.AssetGUID);
        }

        private async Task CacheResourcesByLocations<T>(string label, IList<T> assets) where T : UnityEngine.Object
        {
            AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));

            try
            {
                IList<IResourceLocation> locations = await locationsHandle.Task;
                if (locations == null || locations.Count == 0)
                {
                    Log.Warning(LogTags.Resource, "{0} 라벨로 리소스 위치를 찾을 수 없습니다.", label);
                    return;
                }

                int count = Mathf.Min(assets.Count, locations.Count);
                for (int i = 0; i < count; i++)
                {
                    string cacheKey = locations[i].PrimaryKey;
                    T asset = assets[i];

                    if (_resourcesCache.ContainsKey(cacheKey))
                    {
                        continue;
                    }

                    _resourcesCache.Add(cacheKey, asset);
                    
                    // 참조 카운트 증가
                    if (_referenceCounts.ContainsKey(cacheKey))
                    {
                        _referenceCounts[cacheKey]++;
                    }
                    else
                    {
                        _referenceCounts[cacheKey] = 1;
                    }
                    
                    Log.Progress(LogTags.Resource, "{0} 라벨로 불러온 리소스를 AssetGUID로 캐시합니다. Key: {1}, Asset: {2}", label, cacheKey, asset.name);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Resource, "{0} 라벨 리소스 캐싱 중 오류 발생: {1}", label, ex.Message);
            }
            finally
            {
                Addressables.Release(locationsHandle);
            }
        }

        private void CacheResourcesByLocationsSync<T>(string label, IList<T> assets, IList<IResourceLocation> locations) where T : UnityEngine.Object
        {
            if (locations == null || locations.Count == 0)
            {
                Log.Warning(LogTags.Resource, "{0} 라벨로 리소스 위치를 찾을 수 없습니다.", label);
                return;
            }

            try
            {
                int count = Mathf.Min(assets.Count, locations.Count);
                for (int i = 0; i < count; i++)
                {
                    string cacheKey = locations[i].PrimaryKey;
                    T asset = assets[i];

                    if (_resourcesCache.ContainsKey(cacheKey))
                    {
                        continue;
                    }

                    _resourcesCache.Add(cacheKey, asset);
                    
                    // 참조 카운트 증가
                    if (_referenceCounts.ContainsKey(cacheKey))
                    {
                        _referenceCounts[cacheKey]++;
                    }
                    else
                    {
                        _referenceCounts[cacheKey] = 1;
                    }
                    
                    Log.Progress(LogTags.Resource, "{0} 라벨로 불러온 리소스를 AssetGUID로 캐시합니다. Key: {1}, Asset: {2}", label, cacheKey, asset.name);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.Resource, "{0} 라벨 리소스 캐싱 중 오류 발생: {1}", label, ex.Message);
            }
        }

        #endregion 유틸리티
    }
}