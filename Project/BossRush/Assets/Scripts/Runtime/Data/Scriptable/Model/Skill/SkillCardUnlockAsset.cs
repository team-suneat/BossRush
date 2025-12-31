using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "SkillCardUnlock", menuName = "TeamSuneat/Scriptable/SkillCardUnlock")]
    public partial class SkillCardUnlockAsset : XScriptableObject
    {
        [FoldoutGroup("#UnlockData")]
        [InfoBox("레벨별 스킬 카드 해금 조건입니다. 각 스킬마다 해금 레벨을 설정합니다.")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "SkillName")]
        public List<SkillCardUnlockAssetData> UnlockDataList = new List<SkillCardUnlockAssetData>();

        public override void OnLoadData()
        {
            base.OnLoadData();
            Validate();

            if (UnlockDataList != null)
            {
                foreach (var data in UnlockDataList)
                {
                    data?.OnLoadData();
                }
            }
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (UnlockDataList == null || UnlockDataList.Count == 0)
            {
                Log.Warning(LogTags.ScriptableData, "[SkillCardUnlock] 해금 데이터 리스트가 비어있습니다: {0}", name);
            }
            else
            {
                for (int i = 0; i < UnlockDataList.Count; i++)
                {
                    SkillCardUnlockAssetData data = UnlockDataList[i];
                    data?.Validate();
                }
            }
        }

        public override void Refresh()
        {
            if (UnlockDataList != null)
            {
                for (int i = 0; i < UnlockDataList.Count; i++)
                {
                    SkillCardUnlockAssetData data = UnlockDataList[i];
                    data?.Refresh();
                }
            }

            base.Refresh();
        }

        public override void Rename()
        {
            Rename("SkillCardUnlock");
        }

#endif

        public int GetUnlockLevel(SkillNames skillName)
        {
            if (UnlockDataList == null)
            {
                return 0;
            }

            for (int i = 0; i < UnlockDataList.Count; i++)
            {
                SkillCardUnlockAssetData data = UnlockDataList[i];
                if (data.SkillName == skillName)
                {
                    return data.UnlockLevel;
                }
            }

            return 0;
        }
    }
}