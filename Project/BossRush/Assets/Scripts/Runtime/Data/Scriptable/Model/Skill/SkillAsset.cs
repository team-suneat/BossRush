using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Skill", menuName = "TeamSuneat/Scriptable/Skill")]
    public partial class SkillAsset : XScriptableObject
    {
        public SkillAssetData Data;

        public SkillNames Name => Data.Name;
        public int TID => BitConvert.Enum32ToInt(Name);

        public override void OnLoadData()
        {
            base.OnLoadData();

            Data?.OnLoadData();

            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (Name == SkillNames.None)
            {
                Log.Error("스킬의 이름이 설정되지 않았습니다: {0}", name);
            }
            if (Data == null)
            {
                Log.Error("스킬의 데이터가 설정되지 않았습니다: {0}", name);
            }
            else
            {
                if (Data.Name == SkillNames.None)
                {
                    Log.Error("스킬의 이름이 설정되지 않았습니다: {0}", name);
                }
                if (Data.MaxLevel == 0)
                {
                    Log.Warning("스킬의 최대 레벨이 설정되지 않았습니다: {0}", name);
                }
            }
#endif
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                EnumEx.ConvertTo(ref Data.Name, NameString);
            }

            Data?.Validate();
        }

        public override void Refresh()
        {
            NameString = Name.ToString();

            Data?.Refresh();

            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            bool hasChanged = false;

            UpdateIfChanged(ref NameString, Name);

            if (Data != null)
            {
                if (Data.RefreshWithoutSave())
                {
                    hasChanged = true;
                }
            }

            base.RefreshWithoutSave();

            return hasChanged;
        }

        public override void Rename()
        {
            Rename("Skill");
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
            SkillNames[] skillNames = EnumEx.GetValues<SkillNames>();
            int skillCount = 0;

            Debug.LogFormat("모든 스킬 에셋의 갱신을 시작합니다: {0}", skillNames.Length);

            base.RefreshAll();

            for (int i = 1; i < skillNames.Length; i++)
            {
                if (skillNames[i] != SkillNames.None)
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

            Debug.LogFormat("모든 스킬 에셋의 갱신을 종료합니다: {0}/{1}", skillCount.ToSelectString(skillNames.Length), skillNames.Length);
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            SkillNames[] skillNames = EnumEx.GetValues<SkillNames>();
            for (int i = 1; i < skillNames.Length; i++)
            {
                if (skillNames[i] is SkillNames.None)
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
                            Name = skillNames[i]
                        };
                        asset.NameString = skillNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif
    }
}