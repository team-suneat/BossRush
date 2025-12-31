#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TeamSuneat.Data;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Develop
{
    public class PassiveEditorWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Odin/패시브 관리")]
        private static void OpenWindow()
        {
            var window = GetWindow<PassiveEditorWindow>();
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        [VerticalGroup("LeftPanel")]
        [TabGroup("LeftPanel/패시브 관리", "리스트")]
        [LabelText("전체 패시브 목록 (트리거 필터 적용)")]
        [DisableIf("@FilteredAssets == null || FilteredAssets.Count == 0")]
        public List<PassiveAsset> FilteredAssets = new();

        [VerticalGroup("LeftPanel")]
        [TabGroup("LeftPanel/패시브 관리", "리스트")]
        [Button("패시브 에셋 불러오기", ButtonSizes.Large)]
        private void LoadAll()
        {
            AllAssets = Resources.LoadAll<PassiveAsset>("Scriptable/Passive").ToList();
            ApplyTriggerFilter();
            Debug.Log($"로드 완료: {AllAssets.Count}개");
        }

        private List<PassiveAsset> AllAssets = new();

        [VerticalGroup("LeftPanel")]
        [TabGroup("LeftPanel/패시브 관리", "리스트")]
        [LabelText("Trigger 타입 필터")]
        [ValueDropdown("PassiveTriggersDropdown")]
        public PassiveTriggers TriggerFilter = PassiveTriggers.None;

        private IEnumerable<PassiveTriggers> PassiveTriggersDropdown()
        {
            foreach (var trigger in System.Enum.GetValues(typeof(PassiveTriggers)))
                yield return (PassiveTriggers)trigger;
        }

        [VerticalGroup("LeftPanel")]
        [TabGroup("LeftPanel/패시브 관리", "리스트")]
        [Button("트리거 필터 적용", ButtonSizes.Large)]
        private void ApplyTriggerFilter()
        {
            if (TriggerFilter == PassiveTriggers.None)
            {
                FilteredAssets = new List<PassiveAsset>(AllAssets);
            }
            else
            {
                FilteredAssets = AllAssets
                    .Where(p => p != null && p.TriggerSettings != null && p.TriggerSettings.Trigger == TriggerFilter)
                    .ToList();
            }
        }

        [VerticalGroup("LeftPanel")]
        [TabGroup("LeftPanel/패시브 관리", "리스트")]
        [Button("모든 패시브 정비", ButtonSizes.Large)]
        private void RefreshAllPassives()
        {
            foreach (var passive in AllAssets)
            {
                passive.RefreshWithoutSave();
            }
            Debug.Log("♻️ 모든 패시브 RefreshWithoutSave() 실행 완료");
        }

        [VerticalGroup("LeftPanel")]
        [TabGroup("LeftPanel/패시브 관리", "리스트")]
        [Button("미사용 이름 탐색", ButtonSizes.Large)]
        private void FindUnusedPassiveNames()
        {
            var usedNames = AllAssets
                .Where(p => p != null)
                .Select(p => p.Name)
                .ToHashSet();

            var allNames = System.Enum.GetValues(typeof(PassiveNames)).Cast<PassiveNames>();

            var unused = allNames
                .Where(n => !usedNames.Contains(n) && n != PassiveNames.None)
                .ToList();

            if (unused.Count == 0)
                Debug.Log("🎉 모든 PassiveNames가 사용 중입니다!");
            else
                Debug.LogWarning($"🔍 미사용 패시브 이름 {unused.Count}개:\n- " + string.Join("\n- ", unused));
        }

        [VerticalGroup("RightPanel")]
        [TabGroup("RightPanel/패시브 관리", "상세")]
        [LabelText("패시브 선택")]
        [Searchable]
        [ValueDropdown("FilteredAssets")]
        [OnValueChanged("OnPassiveSelected")]
        public PassiveAsset Selected;

        private void OnPassiveSelected()
        {
            SelectedPassive = Selected;
        }

        [VerticalGroup("RightPanel")]
        [TabGroup("RightPanel/패시브 관리", "상세")]
        [LabelText("패시브 상세 보기")]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [HideLabel]
        [ShowIf("@SelectedPassive != null")]
        public PassiveAsset SelectedPassive;

        [VerticalGroup("RightPanel")]
        [TabGroup("RightPanel/패시브 관리", "기능")]
        [Button("선택된 패시브 Refresh", ButtonSizes.Large)]
        private void RefreshSelectedPassive() => SelectedPassive?.Refresh();

        [VerticalGroup("RightPanel")]
        [TabGroup("RightPanel/패시브 관리", "기능")]
        [Button("선택된 패시브 Validate", ButtonSizes.Large)]
        private void ValidateSelectedPassive()
        {
            ValidationResult = null;
            if (SelectedPassive == null)
            {
                ValidationResult = "⚠️ 선택된 패시브가 없습니다.";
                return;
            }

            try
            {
                SelectedPassive.Validate();
                ValidationResult = "✅ 유효성 검사 통과";
            }
            catch (System.Exception e)
            {
                ValidationResult = $"❌ 오류 발생: {e.Message}";
            }
        }

        [VerticalGroup("RightPanel")]
        [TabGroup("RightPanel/패시브 관리", "기능")]
        [InfoBox("@ValidationResult", InfoMessageType.Error, VisibleIf = "HasValidationError")]
        public string ValidationResult;
        private bool HasValidationError => !string.IsNullOrEmpty(ValidationResult);

        [VerticalGroup("RightPanel")]
        [TabGroup("RightPanel/패시브 관리", "기능")]
        [Button("선택된 패시브 Rename", ButtonSizes.Large)]
        private void RenameSelectedPassive()
        {
            SelectedPassive?.Rename();
            Debug.Log($"📝 이름 변경 완료: {SelectedPassive?.name}");
        }

        [VerticalGroup("RightPanel")]
        [TabGroup("RightPanel/패시브 관리", "기능")]
        [Button("TriggerSettings Refresh", ButtonSizes.Large)]
        private void RefreshTrigger() => SelectedPassive?.TriggerSettings?.Refresh();

        [VerticalGroup("RightPanel")]
        [TabGroup("RightPanel/패시브 관리", "기능")]
        [Button("ConditionSettings Refresh", ButtonSizes.Large)]
        private void RefreshCondition() => SelectedPassive?.ConditionSettings?.Refresh();

        [VerticalGroup("RightPanel")]
        [TabGroup("RightPanel/패시브 관리", "기능")]
        [Button("EffectSettings Refresh", ButtonSizes.Large)]
        private void RefreshEffect() => SelectedPassive?.EffectSettings?.Refresh();
    }
}
#endif