using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Hitmark", menuName = "TeamSuneat/Scriptable/Hitmark")]
    public class HitmarkAsset : XScriptableObject
    {
        [Title("#HitmarkAsset")]
        public HitmarkAssetData Data;

        public HitmarkNames Name
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
                    Log.Error($"히트마크 에셋의 이름 갱신에 실패했습니다. {name}({NameString})");
                }

                Data.Validate();
            }
        }

        public override void Rename()
        {
            Rename("Hitmark");
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

            base.RefreshWithoutSave();
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

            HitmarkNames[] hitmarkNames = EnumEx.GetValues<HitmarkNames>();
            int hitmarkCount = 0;

            Log.Info("모든 히트마크 에셋의 갱신을 시작합니다: {0}", hitmarkNames.Length);

            base.RefreshAll();

            for (int i = 1; i < hitmarkNames.Length; i++)
            {
                if (hitmarkNames[i] != HitmarkNames.None)
                {
                    HitmarkAsset asset = ScriptableDataManager.Instance.FindHitmark(hitmarkNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.RefreshWithoutSave())
                        {
                            hitmarkCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(hitmarkNames.Length);
                EditorUtility.DisplayProgressBar("모든 히트마크 에셋의 갱신", hitmarkNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 히트마크 에셋의 갱신을 종료합니다: {0}/{1}", hitmarkCount.ToSelectString(0), hitmarkNames.Length);
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            HitmarkNames[] hitmarkNames = EnumEx.GetValues<HitmarkNames>();
            for (int i = 1; i < hitmarkNames.Length; i++)
            {
                if (hitmarkNames[i] == HitmarkNames.None)
                {
                    continue;
                }

                HitmarkAsset asset = ScriptableDataManager.Instance.FindHitmark(hitmarkNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<HitmarkAsset>("Hitmark", hitmarkNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new HitmarkAssetData
                        {
                            Name = hitmarkNames[i]
                        };
                        asset.NameString = hitmarkNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif

        public HitmarkAssetData CreateDataClone()
        {
            return Data.Clone();
        }
    }
}