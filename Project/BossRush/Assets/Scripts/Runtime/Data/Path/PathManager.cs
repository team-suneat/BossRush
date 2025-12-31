using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace TeamSuneat
{
    public class PathManager
    {
        public static readonly string targetResourceDirectory = "Resources/Data";
        public static readonly string targetResourcePath = "Resources/Data/PathMetaData.json";
        public static readonly string targetAddressablePath = "Assets/Addressables/JSON/PathMetaData.json";
        public static readonly string writeAddressablePath = "Addressables/JSON/PathMetaData.json";

        public static readonly HashSet<string> ignoreExtensionSet = new() { ".meta", ".shader", ".cginc", ".text" };
        public static readonly HashSet<string> ignoreDirectorySet = new() { "VETASOFT 2DxFX", "ParadoxNotion" };
        private static Dictionary<string, string> database = new();

        public static string ToUnixPath(string path)
        {
            return path.Replace('\\', '/');
        }

        public static string ToLeafPath(string filePath)
        {
            string relPath = ToUnixPath(filePath);
            string leafPath = relPath.Trim('/');
            // 확장자를 포함하여 반환
            return leafPath;
        }

        public static string Lookup(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                return null;
            }

            if (!database.IsValid())
            {
                Log.Warning(LogTags.Path, "주소를 읽어올 수 없습니다. 주소 데이터 베이스를 불러오지 못했습니다.");
            }

            if (database.TryGetValue(fileNameWithExtension, out string path))
            {
                return path;
            }
            else
            {
                Log.Warning(LogTags.Path, "주소를 읽어올 수 없습니다. 파일이 등록되어있지 않습니다. 파일 이름: {0}", fileNameWithExtension);
            }

            return null;
        }

        /// <summary>
        /// 에셋 이름(확장자 없음)을 받아서 해당 경로를 반환합니다.
        /// </summary>
        /// <param name="assetNameWithoutExtension">확장자가 포함되지 않은 에셋 이름</param>
        /// <returns>에셋의 경로, 찾을 수 없으면 null</returns>
        public static string FindPath(string assetNameWithoutExtension)
        {
            if (string.IsNullOrEmpty(assetNameWithoutExtension))
            {
                Log.Warning(LogTags.Path, "에셋 이름이 null이거나 비어있습니다.");
                return null;
            }

            if (!database.IsValid())
            {
                Log.Warning(LogTags.Path, "주소를 읽어올 수 없습니다. 주소 데이터 베이스를 불러오지 못했습니다.");
                return null;
            }

            // 일반적인 확장자들로 시도해보기
            string[] commonExtensions = { ".prefab", ".asset", ".mat", ".png", ".jpg", ".jpeg", ".tga", ".psd", ".fbx", ".obj", ".mesh" };

            foreach (string extension in commonExtensions)
            {
                string assetNameWithExtension = assetNameWithoutExtension + extension;
                if (database.TryGetValue(assetNameWithExtension, out string path))
                {
                    Log.Info(LogTags.Path, "에셋 경로를 찾았습니다: {0} -> {1}", assetNameWithoutExtension, path);
                    return path;
                }
            }

            Log.Warning(LogTags.Path, "에셋을 찾을 수 없습니다. 에셋 이름: {0}", assetNameWithoutExtension);
            return null;
        }

        /// <summary>
        /// 에셋 타입에 따라 적절한 확장자를 선택하여 경로를 찾습니다.
        /// </summary>
        /// <typeparam name="T">에셋 타입</typeparam>
        /// <param name="assetNameWithoutExtension">확장자가 포함되지 않은 에셋 이름</param>
        /// <returns>에셋의 경로, 찾을 수 없으면 null</returns>
        public static string FindPathByType<T>(string assetNameWithoutExtension) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetNameWithoutExtension))
            {
                Log.Warning(LogTags.Path, "에셋 이름이 null이거나 비어있습니다.");
                return null;
            }

            if (!database.IsValid())
            {
                Log.Warning(LogTags.Path, "주소를 읽어올 수 없습니다. 주소 데이터 베이스를 불러오지 못했습니다.");
                return null;
            }

            // 타입별 확장자 매핑
            string[] extensions = GetExtensionsForType<T>();

            foreach (string extension in extensions)
            {
                string assetNameWithExtension = assetNameWithoutExtension + extension;
                if (database.TryGetValue(assetNameWithExtension, out string path))
                {
                    Log.Info(LogTags.Path, "에셋 경로를 찾았습니다: {0} -> {1}", assetNameWithoutExtension, path);
                    return path;
                }
            }

            Log.Warning(LogTags.Path, "에셋을 찾을 수 없습니다. 에셋 이름: {0}, 타입: {1}", assetNameWithoutExtension, typeof(T).Name);
            return null;
        }

        /// <summary>
        /// 타입에 따른 확장자 배열을 반환합니다.
        /// </summary>
        /// <typeparam name="T">에셋 타입</typeparam>
        /// <returns>확장자 배열</returns>
        private static string[] GetExtensionsForType<T>() where T : UnityEngine.Object
        {
            Type type = typeof(T);

            if (type == typeof(GameObject))
            {
                return new string[] { ".prefab" };
            }
            else if (type == typeof(ScriptableObject))
            {
                return new string[] { ".asset" };
            }
            else if (type == typeof(Material))
            {
                return new string[] { ".mat" };
            }
            else if (type == typeof(Texture2D) || type == typeof(Sprite))
            {
                return new string[] { ".png" };
            }
            else if (type == typeof(AudioClip))
            {
                return new string[] { ".wav", ".mp3" };
            }
            else if (type == typeof(AnimationClip))
            {
                return new string[] { ".anim" };
            }
            else if (type == typeof(Shader))
            {
                return new string[] { ".shader" };
            }
            else if (type == typeof(Font))
            {
                return new string[] { ".ttf", ".otf" };
            }
            else if (type == typeof(SpriteAtlas))
            {
                return new string[] { ".spriteatlasv2" };
            }
            else
            {
                Log.Warning(LogTags.Path, "({0}) 타입에 따른 확장명을 찾을 수 없습니다. 일반적인 확장자들을 시도합니다.", type.ToString());
                return new string[] { ".prefab", ".asset", ".mat", ".png", ".jpg", ".jpeg", ".tga", ".psd", ".fbx", ".obj", ".mesh" };
            }
        }

        public static async Task LoadAllAsync()
        {
            TextAsset textAsset = null;

            if (ResourcesManager.IsAddressableSystemInitialized())
            {
                try
                {
                    textAsset = await ResourcesManager.LoadAddressableResourceAsync<TextAsset>(targetAddressablePath);
                    if (textAsset != null)
                    {
                        Log.Info(LogTags.Path, "Addressable에서 PathMetaData 파일을 로드했습니다.");
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Warning(LogTags.Path, "Addressable에서 PathMetaData 파일을 로드할 수 없습니다.  오류: {0}", ex.Message);
                }
            }
            else
            {
                Log.Info(LogTags.Path, "Addressable 시스템이 초기화되지 않았습니다. ");
            }

            if (textAsset == null)
            {
                Log.Warning(LogTags.Path, "Addressable에서 PathMetaData 파일을 로드할 수 없습니다.");
                return;
            }

            try
            {
                database = JsonUtility.FromJson<XSerialization<string, string>>(textAsset.text).ToDictionary();
                if (database.Count > 0)
                {
                    Log.Info(LogTags.Path, "'PathMetaData' 파일을 불러옵니다. 등록된 파일 수: {0}", database.Count);
                }
            }
            catch (Exception e)
            {
                Log.Error("PathMetaData 파일 파싱 중 오류 발생: {0}", e.ToString());
            }
        }

        [MenuItem("Tools/Path/PathMetaData 파일 불러오기")]
        public static void LoadAllSync()
        {
            TextAsset textAsset = null;

            if (ResourcesManager.IsAddressableSystemInitialized())
            {
                try
                {
                    textAsset = ResourcesManager.LoadAddressableResource<TextAsset>(targetAddressablePath);
                    if (textAsset != null)
                    {
                        Log.Info(LogTags.Path, "Addressable에서 PathMetaData 파일을 로드했습니다.");
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Warning(LogTags.Path, "Addressable에서 PathMetaData 파일을 로드할 수 없습니다. 오류: {0}", ex.Message);
                }
            }
            else
            {
                Log.Info(LogTags.Path, "Addressable 시스템이 초기화되지 않았습니다.");
            }

            if (textAsset == null)
            {
                Log.Warning(LogTags.Path, "Addressable에서 PathMetaData 파일을 로드할 수 없습니다.");
                return;
            }

            try
            {
                database = JsonUtility.FromJson<XSerialization<string, string>>(textAsset.text).ToDictionary();
                if (database.Count > 0)
                {
                    Log.Info(LogTags.Path, "'PathMetaData' 파일을 불러옵니다. 등록된 파일 수: {0}", database.Count);
                }
            }
            catch (Exception e)
            {
                Log.Error("PathMetaData 파일 파싱 중 오류 발생: {0}", e.ToString());
            }
        }

        public static bool CheckLoaded()
        {
            return database != null && database.Count > 0;
        }

#if UNITY_EDITOR

        [MenuItem("Tools/Path/파일 경로 저장")]
        public static void UpdatePathMetaDataForMenu()
        {
            UpdatePathMetaData();
        }

#endif

        public static void UpdatePathMetaData()
        {
            Log.Info(LogTags.Path, "파일 경로 저장을 시작합니다.");

            database.Clear();

            // Resources 폴더 스캔
            string[] resourceDirPaths = Directory.GetDirectories(Application.dataPath, "Resources", SearchOption.AllDirectories);

            for (int i = 0; i < resourceDirPaths.Length; i++)
            {
                string dir = resourceDirPaths[i];
                string[] filePaths = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                for (int j = 0; j < filePaths.Length; j++)
                {
                    string filePath = filePaths[j];
                    AddPath(filePath, dir.Length);
                }
            }

            // Addressables 폴더 스캔
            string addressablesPath = Path.Combine(Application.dataPath, "Addressables");
            if (Directory.Exists(addressablesPath))
            {
                string[] filePaths = Directory.GetFiles(addressablesPath, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    string filePath = filePaths[i];
                    AddAddressablePath(filePath);
                }
            }

            Log.Info(LogTags.Path, "파일 경로 저장을 완료합니다.");

            string path = Path.Combine(Application.dataPath, writeAddressablePath);
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                _ = Directory.CreateDirectory(directory);
            }

            string text = JsonUtility.ToJson(new XSerialization<string, string>(database));
            WriteFile(path, text);
        }

        private static void AddPath(string filePath, int directoryLength)
        {
            if (CheckIgnore(filePath))
            {
                return;
            }

            string relPath = ToUnixPath(filePath.Remove(0, directoryLength));
            string fileName = Path.GetFileName(filePath);

            if (fileName.StartsWith("."))
            {
                return;
            }

            if (!database.ContainsKey(fileName))
            {
                string leafPath = relPath.Trim('/');
                // 확장자를 포함하여 저장
                database.Add(fileName, leafPath);
            }
            else
            {
                Log.Warning(LogTags.Path, "같은 이름을 가진 파일이 이미 등록되어있습니다. 파일 이름: {0}", fileName);
            }
        }

        private static void AddAddressablePath(string filePath)
        {
            if (CheckIgnore(filePath))
            {
                return;
            }

            string fileName = Path.GetFileName(filePath);
            if (fileName.StartsWith("."))
            {
                return;
            }

            if (!database.ContainsKey(fileName))
            {
                // Addressables 파일의 경우 Assets 폴더 기준으로 상대 경로 생성
                string assetsPath = Application.dataPath;
                string relativePath = ToUnixPath(filePath.Remove(0, assetsPath.Length - 6)); // "Assets" 유지

                Log.Progress("파일 경로를 저장합니다: {0}, {1}", fileName, relativePath);

                // 확장자를 포함하여 저장
                database.Add(fileName, relativePath);
            }
            else
            {
                Log.Warning(LogTags.Path, "같은 이름을 가진 파일이 이미 등록되어있습니다. 파일 이름: {0}", fileName);
            }
        }

        private static bool CheckIgnore(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            if (ignoreExtensionSet.Contains(extension))
            {
                return true;
            }

            foreach (string ignoreDirectory in ignoreDirectorySet)
            {
                if (filePath.Contains(ignoreDirectory))
                {
                    return true;
                }
            }

            return false;
        }

        private static async void WriteFile(string filename, string text)
        {
            try
            {
                using StreamWriter file = new(filename, false, Encoding.UTF8);
                await file.WriteAsync(text);
                string content = $"Compeleted Write Path MetaData File. count: {database.Count}";
                DisplayDialog("Notice", content);
            }
            catch (UnauthorizedAccessException)
            {
                DisplayDialog("Notice", "You have no write permission in that folder.");
            }
            catch (Exception e)
            {
                DisplayDialog("Notice", e.ToString());
            }
        }

        public static void DisplayDialog(string content)
        {
            DisplayDialog("Notice", content);
        }

        private static void DisplayDialog(string title, string content)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog(title, content, "Ok");
#endif
        }

        #region Find

        private static string FindPathWithExtension(string name, string extension)
        {
            // 입력값 검증
            if (string.IsNullOrEmpty(name))
            {
                Log.Warning(LogTags.Path, "파일명이 null이거나 비어있습니다.");
                return null;
            }

            if (string.IsNullOrEmpty(extension))
            {
                Log.Warning(LogTags.Path, "확장자가 null이거나 비어있습니다.");
                return null;
            }

            // 확장자가 점으로 시작하지 않으면 추가
            string normalizedExtension = extension.StartsWith(".") ? extension : $".{extension}";

            // 파일명에서 확장자 제거 (중복 방지)
            string cleanName = name.EndsWith(normalizedExtension, StringComparison.OrdinalIgnoreCase)
                ? name.Substring(0, name.Length - normalizedExtension.Length)
                : name;

            string fileName = $"{cleanName}{normalizedExtension}";
            string result = Lookup(fileName);
            if (result == null)
            {
                Log.Warning(LogTags.Path, "파일을 찾을 수 없습니다: {0}, {1}", cleanName, fileName);
            }

            return result;
        }

        public static string FindPrefabPath(string name)
        {
            return FindPathWithExtension(name, ".prefab");
        }

        public static string FindAtlasPath(string name)
        {
            return FindPathWithExtension(name, ".spriteatlasv2");
        }

        public static string FindImagePath(string name)
        {
            return FindPathWithExtension(name, ".png");
        }

        public static string FindMaterialPath(string name)
        {
            return FindPathWithExtension(name, ".mat");
        }

        public static string FindShaderPath(string name)
        {
            return FindPathWithExtension(name, ".shader");
        }

        public static string FindAssetPath(string name)
        {
            return FindPathWithExtension(name, ".asset");
        }

        public static string FindVideoPath(string name)
        {
            return FindPathWithExtension(name, ".mp4");
        }

        public static string FindAddressablePrefabPath(string name)
        {
            // 입력값 검증
            if (string.IsNullOrEmpty(name))
            {
                Log.Warning(LogTags.Path, "어드레서블 프리팹 파일명이 null이거나 비어있습니다.");
                return null;
            }

            // 어드레서블 프리팹은 전체 경로로 저장되므로 직접 검색
            string result = Lookup($"{name}.prefab");

            if (result == null)
            {
                Log.Warning(LogTags.Path, "어드레서블 프리팹을 찾을 수 없습니다: {0}.prefab", name);
            }

            return result;
        }

        public static string[] FindAllAssetPath()
        {
            return FindAllPathsByExtension(".asset");
        }

        public static string[] FindAssetPaths(string key)
        {
            // 입력값 검증
            if (string.IsNullOrEmpty(key))
            {
                Log.Warning(LogTags.Path, "검색할 키가 null이거나 비어있습니다.");
                return new string[] { };
            }

            List<string> foundPaths = new();

            foreach (KeyValuePair<string, string> item in database)
            {
                // 정확한 .asset 확장자 매칭
                if (!item.Key.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // 파일명에서 확장자 제거 (확장자 제외한 파일명만 검색)
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.Key);

                // 키가 파일명에 포함되는지 확인
                if (fileNameWithoutExtension.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    foundPaths.Add(item.Value);
                }
            }

            if (foundPaths.Count > 0)
            {
                Log.Progress("'{0}' 키로 {1}개의 asset 파일을 찾았습니다.",
                    key, foundPaths.Count);
                return foundPaths.ToArray();
            }
            else
            {
                Log.Warning(LogTags.Path, "'{0}' 키로 asset 파일을 찾을 수 없습니다.", key);
                return new string[] { };
            }
        }

        public static string[] FindAllPathsByExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                Log.Warning(LogTags.Path, "확장자가 null이거나 비어있습니다.");
                return new string[] { };
            }

            string normalizedExtension = extension.StartsWith(".") ? extension : $".{extension}";
            List<string> foundPaths = new();

            foreach (KeyValuePair<string, string> item in database)
            {
                if (item.Key.EndsWith(normalizedExtension, StringComparison.OrdinalIgnoreCase))
                {
                    foundPaths.Add(item.Value);
                }
            }

            if (foundPaths.Count > 0)
            {
                Log.Progress("'{0}' 확장자 파일 {1}개를 찾았습니다.", normalizedExtension, foundPaths.Count);
                return foundPaths.ToArray();
            }
            else
            {
                Log.Warning(LogTags.Path, "'{0}' 확장자 파일을 찾을 수 없습니다.", normalizedExtension);
                return new string[] { };
            }
        }

        #endregion Find

        /// <summary>
        /// 데이터베이스에 등록된 파일 수를 반환합니다.
        /// </summary>
        /// <returns>등록된 파일 수</returns>
        public static int GetDatabaseCount()
        {
            return database.Count;
        }
    }
}