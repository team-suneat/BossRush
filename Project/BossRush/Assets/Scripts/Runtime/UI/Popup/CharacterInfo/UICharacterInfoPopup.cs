using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 캐릭터 정보 메인 팝업
    public class UICharacterInfoPopup : UIPopup
    {
        [FoldoutGroup("#UIPopup-CharacterInfo")]
        [SerializeField] private UITogglePageController _tabController;

        [FoldoutGroup("#UIPopup-CharacterInfo")]
        [SerializeField] private UICharacterBasicInfoPage _basicInfoPage;

        [FoldoutGroup("#UIPopup-CharacterInfo")]
        [SerializeField] private UICharacterAppearancePage _appearancePage;

        private const int TAB_INDEX_BASIC_INFO = 0;
        private const int TAB_INDEX_APPEARANCE = 1;

        public override UIPopupNames Name => UIPopupNames.CharacterInfo;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _tabController ??= GetComponentInChildren<UITogglePageController>(true);
            _basicInfoPage ??= GetComponentInChildren<UICharacterBasicInfoPage>(true);
            _appearancePage ??= GetComponentInChildren<UICharacterAppearancePage>(true);
        }

        protected override void Awake()
        {
            base.Awake();
            AutoGetComponents();
        }

        public override void Open()
        {
            base.Open();
            RefreshTitleText();
            OpenDefaultTab();
            RefreshAll();
        }

        private void OpenDefaultTab()
        {
            if (_tabController != null)
            {
                _tabController.OpenPage(TAB_INDEX_BASIC_INFO);
            }
        }

        public void RefreshAll()
        {
            _basicInfoPage?.Refresh();
            _appearancePage?.Refresh();
        }

        public void OpenBasicInfoTab()
        {
            if (_tabController != null)
            {
                _tabController.OpenPage(TAB_INDEX_BASIC_INFO);
            }
        }

        public void OpenAppearanceTab()
        {
            if (_tabController != null)
            {
                _tabController.OpenPage(TAB_INDEX_APPEARANCE);
            }
        }

        protected override void RefreshTitleText()
        {
            if (TitleText != null)
            {
                string title = Data.JsonDataManager.FindStringClone(StringDataLabels.POPUP_TITLE_CHARACTER_INFO);
                if (string.IsNullOrEmpty(title))
                {
                    title = "캐릭터 정보";
                }
                TitleText.SetText(title);
            }
        }
    }
}