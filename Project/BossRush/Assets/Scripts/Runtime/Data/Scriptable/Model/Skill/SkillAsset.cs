using Sirenix.OdinInspector;
using TeamSuneat;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Skill", menuName = "TeamSuneat/Scriptable/Skill")]
    public class SkillAsset : XScriptableObject
    {
        [Title("#SkillAsset")]
        public SkillAssetData Data;

        public SkillName Name
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
                    Log.Error($"스킬 에셋의 이름 갱신에 실패했습니다. {name}({NameString})");
                }

                Data.Validate();
            }
        }

        public override void Rename()
        {
            Rename("Skill");
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

            SkillName[] skillNames = EnumEx.GetValues<SkillName>();
            int skillCount = 0;

            Log.Info("모든 스킬 에셋의 갱신을 시작합니다: {0}", skillNames.Length);

            base.RefreshAll();

            for (int i = 1; i < skillNames.Length; i++)
            {
                if (skillNames[i] != SkillName.None)
                {
                    SkillAsset asset = ScriptableDataManager.Instance.FindSkill(skillNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.RefreshWithoutSave())
                        {
                            skillCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(skillNames.Length);
                EditorUtility.DisplayProgressBar("모든 스킬 에셋의 갱신", skillNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 스킬 에셋의 갱신을 종료합니다: {0}/{1}", skillCount.ToSelectString(0), skillNames.Length);
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            SkillName[] skillNames = EnumEx.GetValues<SkillName>();
            for (int i = 1; i < skillNames.Length; i++)
            {
                if (skillNames[i] == SkillName.None)
                {
                    continue;
                }

                SkillAsset asset = ScriptableDataManager.Instance.FindSkill(skillNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<SkillAsset>("Skill", skillNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new SkillAssetData
                        {
                            Name = skillNames[i],
                            Type = SkillType.Active,
                            TriggerType = SkillTriggerType.InputCast,
                        };
                        asset.NameString = skillNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif

        public SkillAssetData CreateDataClone()
        {
            return Data.Clone();
        }
    }
}
