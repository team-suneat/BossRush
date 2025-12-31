using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Buff", menuName = "TeamSuneat/Scriptable/Buff")]
    public class BuffAsset : XScriptableObject
    {
        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public BuffNames Name => Data.Name;

        public BuffAssetData Data;

        public override void OnLoadData()
        {
            base.OnLoadData();

            LogError();
            EnumLog();

            Data.OnLoadData();
        }

        private void LogError()
        {
#if UNITY_EDITOR

            if (Data.IsChangingAsset)
            {
                Log.Error("Asset의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }
            if (Data.Target == BuffTargetTypes.None)
            {
                Log.Warning(LogTags.ScriptableData, "[Buff] Buff Asset의 Target 변수가 설정되지 않았습니다. {0}", name);
            }
            if (Data.Application == BuffApplications.None)
            {
                Log.Warning(LogTags.ScriptableData, "[Buff] Buff Asset의 Application 변수가 설정되지 않았습니다. {0}", name);
            }
            if (Data.Type == BuffTypes.None)
            {
                Log.Warning(LogTags.ScriptableData, "[Buff] Buff Asset의 Type 변수가 설정되지 않았습니다. {0}", name);
            }
            if (Data.MaxLevel == 0)
            {
                Log.Warning(LogTags.ScriptableData, "[Buff] Buff Asset의 Max Level 변수가 설정되지 않았습니다. {0}", name);
            }

            if (Data.UseSpawnCustomPrefab)
            {
                string prefabPath = PathManager.FindPrefabPath($"BuffEntity({Name})");
                if (string.IsNullOrEmpty(prefabPath))
                {
                    Log.Error("버프의 커스텀 프리펩의 경로를 추적할 수 없습니다: {0}", Name.ToLogString());
                }
            }
#endif
        }

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "BuffAsset".ToSelectString();
            EnumExplorer.LogBuff(type, Name.ToString(), Data.Incompatible);
            EnumExplorer.LogBuff(type, Name.ToString(), Data.BuffOnRelease);
            EnumExplorer.LogBuff(type, Name.ToString(), Data.DeactiveBuffs);
#endif
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                _ = EnumEx.ConvertTo(ref Data.Name, NameString);
            }

            Data.Validate();
        }

        public override void Refresh()
        {
            NameString = Data.Name.ToString();
            Data.Refresh();

            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref NameString, Data.Name);
            if (Data.RefreshWithoutSave())
            {
                _hasChangedWhiteRefreshAll = true;
            }

            _ = base.RefreshWithoutSave();

            return _hasChangedWhiteRefreshAll;
        }

        public override void Rename()
        {
            Rename("Buff");
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
#endif
            BuffNames[] buffNames = EnumEx.GetValues<BuffNames>();
            int buffCount = 0;

            Log.Info("모든 버프 에셋의 갱신을 시작합니다: {0}", buffNames.Length);

            base.RefreshAll();

            for (int i = 1; i < buffNames.Length; i++)
            {
                if (buffNames[i] != BuffNames.None)
                {
                    BuffAsset asset = ScriptableDataManager.Instance.FindBuff(buffNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.RefreshWithoutSave())
                        {
                            buffCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(buffNames.Length);
                EditorUtility.DisplayProgressBar("모든 버프 에셋의 갱신", buffNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 버프 에셋의 갱신을 종료합니다: {0}/{1}", buffCount.ToSelectString(buffNames.Length), buffNames.Length);
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            BuffNames[] buffNames = EnumEx.GetValues<BuffNames>();
            for (int i = 1; i < buffNames.Length; i++)
            {
                if (buffNames[i] is BuffNames.None)
                {
                    continue;
                }

                BuffAsset asset = ScriptableDataManager.Instance.FindBuff(buffNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<BuffAsset>("Buff", buffNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new BuffAssetData
                        {
                            Name = buffNames[i]
                        };
                        asset.NameString = buffNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif

        public BuffAssetData Clone()
        {
            return Data.Clone();
        }
    }
}