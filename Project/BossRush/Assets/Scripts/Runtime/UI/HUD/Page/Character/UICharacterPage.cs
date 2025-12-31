using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 캐릭터 페이지 - 강화/성장/승급 탭 전환 관리
    public class UICharacterPage : UIPage
    {
        private const int TAB_INDEX_ENHANCEMENT = 0;
        private const int TAB_INDEX_GROWTH = 1;
        private const int TAB_INDEX_PROMOTION = 2;

        [Title("#UICharacterPage")]
        [SerializeField] private UITogglePageController _togglePageController;
        [SerializeField] private UICharacterLevelGroup _characterLevelGroup;

        private int _currentTabIndex = -1;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _togglePageController ??= GetComponentInChildren<UITogglePageController>();
            _characterLevelGroup ??= GetComponentInChildren<UICharacterLevelGroup>();
        }

        public override void Initialize()
        {
            base.Initialize();

            // 기본 탭 열기 (강화 탭)
            if (_togglePageController != null && _currentTabIndex < 0)
            {
                _togglePageController.OpenPage(TAB_INDEX_ENHANCEMENT);
            }
        }

        protected override void OnShow()
        {
            base.OnShow();

            RegisterEvents();
            // RefreshCharacterLevelGroup();
        }

        protected override void OnHide()
        {
            base.OnHide();

            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            if (_togglePageController != null)
            {
                UIToggleGroup toggleGroup = _togglePageController.GetComponentInChildren<UIToggleGroup>();
                if (toggleGroup != null)
                {
                    toggleGroup.OnToggleChanged.AddListener(OnTabChanged);
                }
            }
        }

        private void UnregisterEvents()
        {
            if (_togglePageController != null)
            {
                UIToggleGroup toggleGroup = _togglePageController.GetComponentInChildren<UIToggleGroup>();
                if (toggleGroup != null)
                {
                    toggleGroup.OnToggleChanged.RemoveListener(OnTabChanged);
                }
            }
        }

        private void OnTabChanged(int tabIndex)
        {
            _currentTabIndex = tabIndex;

            // 캐릭터 정보 영역 표시/숨김 처리
            UpdateCharacterLevelGroupVisibility(tabIndex);

            // 각 페이지 갱신
            RefreshCurrentPage(tabIndex);
        }

        private void UpdateCharacterLevelGroupVisibility(int tabIndex)
        {
            if (_characterLevelGroup == null)
            {
                return;
            }

            // 강화(0), 성장(1) 탭에서는 표시, 승급(2) 탭에서는 숨김
            bool isVisible = tabIndex == TAB_INDEX_ENHANCEMENT || tabIndex == TAB_INDEX_GROWTH;
            _characterLevelGroup.SetVisible(isVisible);

            if (isVisible)
            {
                _characterLevelGroup.Refresh();
            }
        }

        private void RefreshCurrentPage(int tabIndex)
        {
            if (_togglePageController == null)
            {
                return;
            }

            UIPage currentPage = _togglePageController.GetPage(tabIndex);
            if (currentPage == null)
            {
                return;
            }

            // Enhancement 페이지인 경우 아이템 갱신
            if (tabIndex == TAB_INDEX_ENHANCEMENT && currentPage is UIEnhancementPage enhancementPage)
            {
                enhancementPage.RefreshAllItems();
            }
        }

        private void RefreshCharacterLevelGroup()
        {
            if (_characterLevelGroup != null)
            {
                _characterLevelGroup.Refresh();
            }
        }

        public void OpenEnhancementTab()
        {
            _togglePageController?.OpenPage(TAB_INDEX_ENHANCEMENT);
        }

        public void OpenGrowthTab()
        {
            _togglePageController?.OpenPage(TAB_INDEX_GROWTH);
        }

        public void OpenPromotionTab()
        {
            _togglePageController?.OpenPage(TAB_INDEX_PROMOTION);
        }

        public int GetCurrentTabIndex()
        {
            return _currentTabIndex;
        }
    }
}