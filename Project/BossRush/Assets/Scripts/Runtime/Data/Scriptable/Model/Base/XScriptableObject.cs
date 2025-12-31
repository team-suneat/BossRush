using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    public class XScriptableObject : ScriptableObject
    {
        [FoldoutGroup("#String", 2)]
        public string NameString;

        public virtual void OnLoadData()
        {
#if UNITY_EDITOR
            Validate();
#endif
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Button", true, 3)]
        [Button("갱신 및 저장", ButtonSizes.Large)]
        public virtual void Refresh()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        public virtual bool RefreshWithoutSave()
        {
#if UNITY_EDITOR
            if (_hasChangedWhiteRefreshAll || GameDefine.DEV_SCRIPTABLE_OBJECT_FORCE_REFRESH_ALL)
            {
                Debug.LogFormat("에셋의 값을 갱신했습니다. {0}", name.ToSelectString());
                EditorUtility.SetDirty(this);
            }
#endif

            return _hasChangedWhiteRefreshAll;
        }

        protected bool _hasChangedWhiteRefreshAll = false;

        protected void UpdateIfChanged<TEnum>(ref string target, TEnum newValue) where TEnum : Enum
        {
            string newString = newValue?.ToString();
            if (target != newString)
            {
                target = newString;
                _hasChangedWhiteRefreshAll = true;
            }
        }

        protected void UpdateIfChangedArray(ref string[] target, string[] newArray)
        {
            try
            {
                // 두 배열이 모두 null이면 업데이트할 필요가 없습니다.
                if (target == null && newArray == null)
                {
                    return;
                }

                // 두 배열이 모두 null이 아니고 값이 동일하면 업데이트하지 않습니다.
                if (target != null && newArray != null && target.SequenceEqual(newArray))
                {
                    return;
                }

                // 배열이 변경되었으므로 newArray로 업데이트하고 플래그를 설정합니다.
                target = newArray;
                _hasChangedWhiteRefreshAll = true;
                Log.Info(LogTags.ScriptableData, "배열이 변경되어 업데이트되었습니다.");
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTags.ScriptableData, "배열 업데이트 중 예외가 발생했습니다.", ex);
            }
        }

        [FoldoutGroup("#Button", true, 4)]
        [Button("에셋 이름 자동 설정", ButtonSizes.Large)]
        public virtual void Rename()
        {
            // 필요에 따라 기본 Rename 동작 정의 (현재는 빈 구현)
        }

        public void Rename(string title)
        {
#if UNITY_EDITOR
            string newName = $"{title}_{NameString}";
            PerformRename(newName);
#endif
        }

        public void Rename(string title, string desc)
        {
#if UNITY_EDITOR
            string newName = $"{title}_{NameString}_{desc}";
            PerformRename(newName);
#endif
        }

#if UNITY_EDITOR

        /// <summary>
        /// 에셋의 경로를 확인하고, 현재 파일 이름과 다르면 에셋 이름을 변경합니다.
        /// </summary>
        /// <param name="newName">새 에셋 이름 (확장자 제외)</param>
        protected void PerformRename(string newName)
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            string currentFileName = Path.GetFileName(assetPath);
            string newFileName = newName + ".asset";

            if (newFileName != currentFileName)
            {
                Log.Info($"스크립터블 오브젝트의 이름을 변경합니다, {currentFileName} ▶ {(newFileName).ToSelectString()}");
                UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
            }
        }

#endif

        //──────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        private void OnValidate()
        {
            Validate();
        }

        public virtual void Validate()
        {
        }

        private void LoadData()
        {
            if (!PathManager.CheckLoaded())
            {
                PathManager.LoadAllSync();
            }
            if (!ScriptableDataManager.Instance.CheckLoaded())
            {
                ScriptableDataManager.Instance.LoadScriptableAssetsSyncByLabel(AddressableLabels.ScriptableSync);
                ScriptableDataManager.Instance.LoadScriptableAssetsSyncByLabel(AddressableLabels.Scriptable);
                ScriptableDataManager.Instance.LoadScriptableAssetsSyncByLabel(string.Format(AddressableLabels.AreaFormat, 1));
            }
            if (!JsonDataManager.CheckLoaded())
            {
                JsonDataManager.LoadJsonSheetsSync();
            }
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Button", false)]
        [Button("해당 타입의 모든 수정된 에셋 갱신", ButtonSizes.Large)]
        protected virtual void RefreshAll()
        {
            LoadData();
        }

        protected void OnRefreshAll()
        {
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Button")]
        [Button("미생성 에셋 자동 생성", ButtonSizes.Large)]
        protected virtual void CreateAll()
        {
            LoadData();
        }

#if UNITY_EDITOR

        /// <summary>
        /// 부모 폴더가 지정되지 않은 경우 기본 경로를 사용하여 에셋을 생성합니다.
        /// </summary>
        protected T CreateAsset<T>(string assetType, string assetName, bool isCreateAsset) where T : XScriptableObject
        {
            return CreateAssetInternal<T>(null, assetType, assetName, isCreateAsset);
        }

        /// <summary>
        /// 부모 폴더가 지정된 경우 해당 폴더 경로를 사용하여 에셋을 생성합니다.
        /// </summary>
        protected T CreateAsset<T>(string parentFolder, string assetType, string assetName, bool isCreateAsset) where T : XScriptableObject
        {
            return CreateAssetInternal<T>(parentFolder, assetType, assetName, isCreateAsset);
        }

        /// <summary>
        /// 에셋 생성의 공통 로직을 담당하는 내부 메서드입니다.
        /// </summary>
        private T CreateAssetInternal<T>(string parentFolder, string assetType, string assetName, bool isCreateAsset) where T : XScriptableObject
        {
            if (isCreateAsset)
            {
                Log.Info(LogTags.ScriptableData, $"{assetType} 에셋을 생성합니다. AssetName({assetName})");

                // 부모 폴더가 없으면 기본 경로 사용, 있으면 해당 경로 사용
                string assetPath = string.IsNullOrEmpty(parentFolder)
                    ? $"Assets/Addressables/Scriptable/{assetType}/{assetType}_{assetName}.asset"
                    : $"Assets/Addressables/Scriptable/{parentFolder}/{assetType}/{assetType}_{assetName}.asset";

                T asset = ScriptableObject.CreateInstance<T>();
                UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                UnityEditor.AssetDatabase.ImportAsset(assetPath);

                return asset;
            }
            else
            {
                Log.Info(LogTags.ScriptableData, $"생성할 {assetType} 에셋의 이름을 확인합니다. AssetName({assetName})");
                return null;
            }
        }

#endif

        protected T CreateAssetInSameFolder<T>(string assetType, string assetName, bool isCreateAsset) where T : XScriptableObject
        {
            if (isCreateAsset)
            {
#if UNITY_EDITOR
                Log.Info(LogTags.ScriptableData, $"{assetType} 에셋을 생성합니다. AssetName({assetName})");

                // 호출한 스크립터블 오브젝트의 에셋 경로를 가져옵니다.
                string callerAssetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
                string targetFolder;

                if (string.IsNullOrEmpty(callerAssetPath))
                {
                    // 만약 호출하는 에셋의 경로를 찾지 못했다면 기본 경로를 사용합니다.
                    Log.Error(LogTags.ScriptableData, "호출하는 에셋의 경로를 찾을 수 없습니다. 기본 경로를 사용합니다.");
                    targetFolder = $"Assets/Resources/Scriptable/{assetType}";
                }
                else
                {
                    // 에셋의 경로에서 폴더 경로만 추출합니다.
                    targetFolder = Path.GetDirectoryName(callerAssetPath);
                }

                // 새 에셋의 경로를 targetFolder 내에 생성합니다.
                string assetPath = Path.Combine(targetFolder, $"{assetType}_{assetName}.asset");
                // Path.Combine은 OS에 따라 역슬래시를 사용할 수 있으므로 Unity 에셋 경로 형식에 맞게 슬래시로 변환합니다.
                assetPath = assetPath.Replace("\\", "/");

                // T 타입의 인스턴스를 생성하고 에셋으로 등록합니다.
                T asset = ScriptableObject.CreateInstance<T>();
                UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                UnityEditor.AssetDatabase.ImportAsset(assetPath);

                return asset;
#endif
            }
            else
            {
                Log.Info(LogTags.ScriptableData, $"생성할 {assetType} 에셋의 이름을 확인합니다. AssetName({assetName})");
            }

            return null;
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Button", true, 5)]
        [Button("경로 갱신", ButtonSizes.Large)]
        protected virtual void Save()
        {
            PathManager.UpdatePathMetaData();
        }

        #region Color Utility

        /// <summary>
        /// 일반 값(T)이 기본 값(default)과 동일하면 DarkGray, 그렇지 않으면 GreenYellow를 반환합니다.
        /// (bool, int, enum 등 대부분의 타입에 적용 가능)
        /// </summary>
        protected Color GetColor<T>(T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        /// <summary>
        /// float 타입의 경우 IsZero 확장 메서드를 사용해 값이 0에 가까운지 판별합니다.
        /// </summary>
        protected Color GetColor(float value)
        {
            if (value.IsZero())
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        #endregion Color Utility

        #region Inspector Color Methods

        protected Color GetPlayerColor(CharacterNames key)
        {
            return GetColor(key);
        }

        protected Color GetMonsterColor(CharacterNames key)
        {
            return GetColor(key);
        }

        protected Color GetPassiveNameColor(PassiveNames key)
        {
            return GetColor(key);
        }

        protected Color GetBuffNameColor(BuffNames key)
        {
            return GetColor(key);
        }

        protected Color GetBuffTypeColor(BuffTypes key)
        {
            return GetColor(key);
        }

        protected Color GetStatNameColor(StatNames key)
        {
            return GetColor(key);
        }

        protected Color GetStateEffectColor(StateEffects key)
        {
            return GetColor(key);
        }

        protected Color GetHitmarkColor(HitmarkNames key)
        {
            return GetColor(key);
        }

        protected Color GetAreaNameColor(AreaNames key)
        {
            return GetColor(key);
        }

        protected Color GetStageNameColor(StageNames key)
        {
            return GetColor(key);
        }

        protected Color GetSkillNameColor(SkillNames key)
        {
            return GetColor(key);
        }

        //

        protected Color GetBoolColor(bool value)
        {
            return GetColor(value);
        }

        protected Color GetFloatColor(float value)
        {
            return GetColor(value);
        }

        protected Color GetIntColor(int value)
        {
            return GetColor(value);
        }

        #endregion Inspector Color Methods
    }
}