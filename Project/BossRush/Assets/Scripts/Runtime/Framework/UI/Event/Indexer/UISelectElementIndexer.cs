using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UISelectElementIndexer : XBehaviour
    {
        [FoldoutGroup("#UISelectElementIndexer")]
        [LabelText("레이아웃 타입")]
        [SerializeField] private LayoutType _layoutType = LayoutType.Grid;

        [FoldoutGroup("#UISelectElementIndexer")]
        [LabelText("기본 시작 인덱스")]
        [SerializeField] private int _defaultIndex;

        [FoldoutGroup("#UISelectElementIndexer")]
        [ShowIf("_layoutType", LayoutType.Grid)]
        [MinValue(1)]
        [LabelText("행 수")]
        [SerializeField] private int _rows = 1;

        [FoldoutGroup("#UISelectElementIndexer")]
        [ShowIf("_layoutType", LayoutType.Grid)]
        [MinValue(1)]
        [LabelText("열 수")]
        [SerializeField] private int _columns = 1;

        [FoldoutGroup("#UISelectElementIndexer")]
        [LabelText("순환 선택 활성화")]
        [SerializeField] private bool _isCycling;

        [FoldoutGroup("#UISelectElementIndexer")]
        [LabelText("수평 방향 반전")]
        [SerializeField] private bool _isReverseHorizontal;

        [FoldoutGroup("#UISelectElementIndexer")]
        [LabelText("수직 방향 반전")]
        [Tooltip("true: 위쪽 버튼이 아래로, 아래쪽 버튼이 위로")]
        [SerializeField] private bool _isReverseVertical;

        [FoldoutGroup("#UISelectElementIndexer")]
        [LabelText("시작 시 첫 번째 요소 선택")]
        [SerializeField] private bool _selectFirstElementOnStart;

        [FoldoutGroup("#UISelectElementIndexer-Fixed")]
        [InfoBox("모든 요소에 적용되는 고정 인덱스")]
        [SerializeField] private FixedIndexOverrides _globalFixedIndices = new();

        [FoldoutGroup("#UISelectElementIndexer-Fixed")]
        [InfoBox("첫 번째 요소의 기본값(-1)을 덮어씁니다")]
        [SerializeField] private FixedIndexOverrides _firstElementOverrides = new();

        [FoldoutGroup("#UISelectElementIndexer-Fixed")]
        [InfoBox("마지막 요소의 기본값(-1)을 덮어씁니다")]
        [SerializeField] private FixedIndexOverrides _lastElementOverrides = new();

        [FoldoutGroup("#UISelectElementIndexer-Fixed")]
        [LabelText("고정된 마지막 이벤트")]
        [SerializeField] private UISelectElement _fixedLastSelectElement;

        [FoldoutGroup("#UISelectElementIndexer-Events")]
        [LabelText("관리할 요소 배열")]
        public UISelectElement[] Elements;

        public int ElementsCount => Elements?.Length ?? 0;

        public LayoutType LayoutType => _layoutType;
        public int DefaultIndex => _defaultIndex;
        public int Rows => _rows;
        public int Columns => _columns;
        public bool IsCycling => _isCycling;
        public bool IsReverseHorizontal => _isReverseHorizontal;
        public bool IsReverseVertical => _isReverseVertical;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            LoadElements();
            Log.Info(LogTags.UI_SelectEvent, "요소 컴포넌트를 로드했습니다. 총 {0}개의 요소", ElementsCount);
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (_selectFirstElementOnStart)
            {
                SelectFirstElement();
            }
        }

        [FoldoutGroup("#Custom Buttons", 1000)]
        [Button("Setup Index", ButtonSizes.Large)]
        private void ForceSetupIndex()
        {
            SetupIndex();
        }

        public void LoadElements()
        {
            Elements = GetComponentsInChildren<UISelectElement>();
            EnsureFixedLastElement();
            Log.Info(LogTags.UI_SelectEvent, "자식 오브젝트에서 요소를 로드했습니다. 총 {0}개의 요소", ElementsCount);
        }

        private void EnsureFixedLastElement()
        {
            if (_fixedLastSelectElement == null)
            {
                return;
            }

            if (Elements == null || Elements.Length == 0)
            {
                Elements = new[] { _fixedLastSelectElement };
                return;
            }

            if (Elements[^1] == _fixedLastSelectElement)
            {
                return;
            }

            List<UISelectElement> elementList = new(Elements.Length + 1);
            for (int i = 0; i < Elements.Length; i++)
            {
                UISelectElement currentElement = Elements[i];
                if (currentElement == _fixedLastSelectElement)
                {
                    continue;
                }

                elementList.Add(currentElement);
            }

            elementList.Add(_fixedLastSelectElement);
            Elements = elementList.ToArray();
        }

        public void SetupIndex()
        {
            EnsureFixedLastElement();

            if (Elements == null || Elements.Length == 0)
            {
                Log.Warning(LogTags.UI_SelectEvent, "Elements 배열이 비어 있습니다.");
                return;
            }

            Log.Info(LogTags.UI_SelectEvent, "인덱스 설정을 시작합니다. LayoutType: {0}, Rows: {1}, Columns: {2}, DefaultIndex: {3}",
                _layoutType, _rows, _columns, _defaultIndex);

            IndexerConfig config = CreateConfig();
            ILayoutIndexStrategy strategy = GetStrategy(_layoutType);

            if (strategy == null)
            {
                Log.Warning(LogTags.UI_SelectEvent, "지원하지 않는 레이아웃 타입입니다: {0}", _layoutType);
                return;
            }

            strategy.SetupIndices(Elements, config);
            SetupFixedSelectIndex();

            Log.Info(LogTags.UI_SelectEvent, "인덱스 설정이 완료되었습니다.");
        }

        private IndexerConfig CreateConfig()
        {
            return new IndexerConfig
            {
                DefaultIndex = _defaultIndex,
                Rows = _rows,
                Columns = _columns,
                IsCycling = _isCycling,
                IsReverseHorizontal = _isReverseHorizontal,
                IsReverseVertical = _isReverseVertical
            };
        }

        private ILayoutIndexStrategy GetStrategy(LayoutType layoutType)
        {
            return layoutType switch
            {
                LayoutType.Grid => new GridLayoutStrategy(),
                LayoutType.Horizontal => new HorizontalLayoutStrategy(),
                LayoutType.Vertical => new VerticalLayoutStrategy(),
                _ => null
            };
        }

        private void SetupFixedSelectIndex()
        {
            Log.Progress(LogTags.UI_SelectEvent, "고정 선택 인덱스 설정을 시작합니다.");
            if (!Elements.IsValid())
            {
                return;
            }

            for (int i = 0; i < Elements.Length; i++)
            {
                ApplyGlobalFixedIndices(Elements[i]);
            }

            if (Elements.Length > 0)
            {
                ApplyFirstElementOverrides(Elements[0]);
                ApplyLastElementOverrides(Elements[^1]);
            }

            Log.Progress(LogTags.UI_SelectEvent, "고정 선택 인덱스 설정이 완료되었습니다.");
        }

        private void ApplyGlobalFixedIndices(UISelectElement element)
        {
            if (element == null)
            {
                return;
            }

            if (_globalFixedIndices.UpIndex > 0)
            {
                element.SelectUpIndex = _globalFixedIndices.UpIndex;
                Log.Info(LogTags.UI_SelectEvent, "고정 위쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _globalFixedIndices.UpIndex);
            }

            if (_globalFixedIndices.DownIndex > 0)
            {
                element.SelectDownIndex = _globalFixedIndices.DownIndex;
                Log.Info(LogTags.UI_SelectEvent, "고정 아래쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _globalFixedIndices.DownIndex);
            }

            if (_globalFixedIndices.LeftIndex > 0)
            {
                element.SelectLeftIndex = _globalFixedIndices.LeftIndex;
                Log.Info(LogTags.UI_SelectEvent, "고정 왼쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _globalFixedIndices.LeftIndex);
            }

            if (_globalFixedIndices.RightIndex > 0)
            {
                element.SelectRightIndex = _globalFixedIndices.RightIndex;
                Log.Info(LogTags.UI_SelectEvent, "고정 오른쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _globalFixedIndices.RightIndex);
            }
        }

        private void ApplyFirstElementOverrides(UISelectElement element)
        {
            if (element == null)
            {
                return;
            }

            if (element.SelectUpIndex == -1 && _firstElementOverrides.UpIndex > 0)
            {
                element.SelectUpIndex = _firstElementOverrides.UpIndex;
                Log.Info(LogTags.UI_SelectEvent, "첫 번째 위쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _firstElementOverrides.UpIndex);
            }

            if (element.SelectDownIndex == -1 && _firstElementOverrides.DownIndex > 0)
            {
                element.SelectDownIndex = _firstElementOverrides.DownIndex;
                Log.Info(LogTags.UI_SelectEvent, "첫 번째 아래쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _firstElementOverrides.DownIndex);
            }

            if (element.SelectLeftIndex == -1 && _firstElementOverrides.LeftIndex > 0)
            {
                element.SelectLeftIndex = _firstElementOverrides.LeftIndex;
                Log.Info(LogTags.UI_SelectEvent, "첫 번째 왼쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _firstElementOverrides.LeftIndex);
            }

            if (element.SelectRightIndex == -1 && _firstElementOverrides.RightIndex > 0)
            {
                element.SelectRightIndex = _firstElementOverrides.RightIndex;
                Log.Info(LogTags.UI_SelectEvent, "첫 번째 오른쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _firstElementOverrides.RightIndex);
            }
        }

        private void ApplyLastElementOverrides(UISelectElement element)
        {
            if (element == null)
            {
                return;
            }

            if (element.SelectUpIndex == -1 && _lastElementOverrides.UpIndex > 0)
            {
                element.SelectUpIndex = _lastElementOverrides.UpIndex;
                Log.Info(LogTags.UI_SelectEvent, "마지막 위쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _lastElementOverrides.UpIndex);
            }

            if (element.SelectDownIndex == -1 && _lastElementOverrides.DownIndex > 0)
            {
                element.SelectDownIndex = _lastElementOverrides.DownIndex;
                Log.Info(LogTags.UI_SelectEvent, "마지막 아래쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _lastElementOverrides.DownIndex);
            }

            if (element.SelectLeftIndex == -1 && _lastElementOverrides.LeftIndex > 0)
            {
                element.SelectLeftIndex = _lastElementOverrides.LeftIndex;
                Log.Info(LogTags.UI_SelectEvent, "마지막 왼쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _lastElementOverrides.LeftIndex);
            }

            if (element.SelectRightIndex == -1 && _lastElementOverrides.RightIndex > 0)
            {
                element.SelectRightIndex = _lastElementOverrides.RightIndex;
                Log.Info(LogTags.UI_SelectEvent, "마지막 오른쪽 인덱스 설정: {0} -> {1}", element.GetHierarchyName(), _lastElementOverrides.RightIndex);
            }
        }

        #region Raycast

        private void SetRaycast(bool state)
        {
            if (Elements == null || Elements.Length == 0)
            {
                return;
            }

            for (int i = 0; i < Elements.Length; i++)
            {
                UISelectElement evt = Elements[i];
                if (evt == null)
                {
                    continue;
                }

                SetRaycastOnGraphics(evt, state);
            }
        }

        private void SetRaycastOnGraphics(UISelectElement element, bool state)
        {
            MaskableGraphic[] graphics = element.GetComponentsInChildren<MaskableGraphic>();
            if (graphics == null || graphics.Length == 0)
            {
                return;
            }

            for (int i = 0; i < graphics.Length; i++)
            {
                MaskableGraphic g = graphics[i];
                g.raycastTarget = state;
            }
        }

        public void ActivateRaycast()
        {
            Log.Info(LogTags.UI_SelectEvent, "레이캐스트를 활성화합니다.");
            SetRaycast(true);
        }

        public void DeactivateRaycast()
        {
            Log.Info(LogTags.UI_SelectEvent, "레이캐스트를 비활성화합니다.");
            SetRaycast(false);
        }

        #endregion Raycast

        public int FindIndex(UISelectElement element)
        {
            if (Elements == null)
            {
                return -1;
            }

            for (int i = 0; i < Elements.Length; i++)
            {
                if (Elements[i] == element)
                {
                    return i;
                }
            }

            return -1;
        }

        private void SelectFirstElement()
        {
            if (Elements == null || Elements.Length == 0)
            {
                Log.Warning(LogTags.UI_SelectEvent, "선택할 요소가 없습니다.");
                return;
            }

            UISelectElement firstElement = Elements[0];
            if (firstElement == null)
            {
                Log.Warning(LogTags.UI_SelectEvent, "첫 번째 요소가 null입니다.");
                return;
            }

            if (firstElement.SelectIndex <= 0)
            {
                Log.Warning(LogTags.UI_SelectEvent, "첫 번째 요소의 SelectIndex가 유효하지 않습니다: {0}", firstElement.SelectIndex);
                return;
            }

            UIManager.Instance.SelectController.Select(firstElement.SelectIndex);

            Log.Info(LogTags.UI_SelectEvent, "첫 번째 요소를 선택했습니다. SelectIndex: {0}", firstElement.SelectIndex);
        }
    }
}