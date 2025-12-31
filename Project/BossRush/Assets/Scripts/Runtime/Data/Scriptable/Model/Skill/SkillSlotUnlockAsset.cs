using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "SkillSlotUnlock", menuName = "TeamSuneat/Scriptable/SkillSlotUnlock")]
    public class SkillSlotUnlockAsset : XScriptableObject
    {
        [FoldoutGroup("#UnlockData")]
        [InfoBox("레벨별 스킬 슬롯 해금 조건입니다. 배열의 인덱스는 슬롯 번호(0부터 시작)이고, 값은 해금 레벨입니다.")]
        [SuffixLabel("레벨")]
        public int[] UnlockLevels = new int[10]; // 최대 10개 슬롯

        public override void OnLoadData()
        {
            base.OnLoadData();
            Validate();
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (UnlockLevels == null || UnlockLevels.Length == 0)
            {
                Log.Warning(LogTags.ScriptableData, "[SkillSlotUnlock] 해금 레벨 배열이 비어있습니다: {0}", name);
            }
        }

        public override void Refresh()
        {
            base.Refresh();
        }

        public override void Rename()
        {
            Rename("SkillSlotUnlock");
        }

        [FoldoutGroup("#Button")]
        [Button("기본값 설정", ButtonSizes.Medium)]
        public void SetDefaultValues()
        {
            if (UnlockLevels == null || UnlockLevels.Length == 0)
            {
                UnlockLevels = new int[10];
            }

            // 슬롯 0번은 레벨 1부터 시작, 이후 5레벨씩 증가
            for (int i = 0; i < UnlockLevels.Length; i++)
            {
                UnlockLevels[i] = 1 + (i * 5);
            }

            EditorUtility.SetDirty(this);
            Log.Info(LogTags.ScriptableData, "[SkillSlotUnlock] 기본값이 설정되었습니다: {0}", name);
        }

#endif
    }
}