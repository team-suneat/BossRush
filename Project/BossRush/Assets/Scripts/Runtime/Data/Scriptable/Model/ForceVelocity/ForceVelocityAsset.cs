#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "ForceVelocity", menuName = "TeamSuneat/Scriptable/ForceVelocity")]
    public class ForceVelocityAsset : XScriptableObject
    {
        [Title("#ForceVelocityAsset")]
        public ForceVelocityAssetData Data;

        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public FVNames Name => Data.Name;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogError();
            Data.OnLoadData();
        }

        private void LogError()
        {
#if UNITY_EDITOR

            if (Data.IsChangingAsset)
            {
                Log.Error("Asset의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }

#endif
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref Data.Name, NameString))
                {
                    Log.Error($"ForceVelocity 에셋의 이름 갱신에 실패했습니다. {name}({NameString})");
                }

                Data.Validate();
            }
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

            UpdateIfChanged(ref NameString, Name);
            if (Data.RefreshWithoutSave())
            {
                _hasChangedWhiteRefreshAll = true;
            }

            _ = base.RefreshWithoutSave();
            return _hasChangedWhiteRefreshAll;
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }

            FVNames[] fvNames = EnumEx.GetValues<FVNames>();
            int fvCount = 0;

            Log.Info("모든 ForceVelocity 에셋의 갱신을 시작합니다: {0}", fvNames.Length);

            base.RefreshAll();

            for (int i = 1; i < fvNames.Length; i++)
            {
                if (fvNames[i] != FVNames.None)
                {
                    ForceVelocityAsset asset = ScriptableDataManager.Instance.FindForceVelocity(fvNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.RefreshWithoutSave())
                        {
                            fvCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(fvNames.Length);
                EditorUtility.DisplayProgressBar("모든 ForceVelocity 에셋의 갱신", fvNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 ForceVelocity 에셋의 갱신을 종료합니다: {0}/{1}", fvCount.ToSelectString(0), fvNames.Length);
#endif
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            FVNames[] fvNames = EnumEx.GetValues<FVNames>();
            for (int i = 1; i < fvNames.Length; i++)
            {
                if (fvNames[i] == FVNames.None)
                {
                    continue;
                }

                ForceVelocityAsset asset = ScriptableDataManager.Instance.FindForceVelocity(fvNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<ForceVelocityAsset>("ForceVelocity", fvNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new ForceVelocityAssetData
                        {
                            Name = fvNames[i]
                        };
                        asset.NameString = fvNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

        public override void Rename()
        {
            Rename("ForceVelocity");
        }

#endif

        public ForceVelocityAssetData Clone()
        {
            return Data.Clone();
        }
    }
}