using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Buff", menuName = "TeamSuneat/Scriptable/Buff")]
    public class BuffAsset : XScriptableObject
    {
        [Title("#BuffAsset")]
        public BuffAssetData Data;

        public BuffName Name
        {
            get => Data.Name;
            set => Data.Name = value;
        }

        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public override void OnLoadData()
        {
            base.OnLoadData();

            if (Data.IsChangingAsset)
            {
                Log.Error("Asset의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }

            Data.OnLoadData();
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref Data.Name, NameString))
                {
                    Log.Error($"버프 에셋의 이름 갱신에 실패했습니다. {name}({NameString})");
                }

                Data.Validate();
            }
        }

        public override void Rename()
        {
            Rename("Buff");
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
#endif

            BuffName[] buffNames = EnumEx.GetValues<BuffName>();
            int buffCount = 0;

            Log.Info("모든 버프 에셋의 갱신을 시작합니다: {0}", buffNames.Length);

            base.RefreshAll();

            for (int i = 1; i < buffNames.Length; i++)
            {
                if (buffNames[i] != BuffName.None)
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

            Log.Info("모든 버프 에셋의 갱신을 종료합니다: {0}/{1}", buffCount.ToSelectString(0), buffNames.Length);
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            BuffName[] buffNames = EnumEx.GetValues<BuffName>();
            for (int i = 1; i < buffNames.Length; i++)
            {
                if (buffNames[i] == BuffName.None)
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

        public BuffAssetData CreateDataClone()
        {
            return Data.Clone();
        }
    }
}