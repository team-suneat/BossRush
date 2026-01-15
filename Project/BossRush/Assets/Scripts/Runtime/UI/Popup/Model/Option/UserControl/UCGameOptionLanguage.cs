using Sirenix.OdinInspector;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UCGameOptionLanguage : UCGameOptionBase
    {
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectButton _englishButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectButton _koreanButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectButton _simplifiedChineseButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectButton _traditionalChineseButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectButton _frenchButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectButton _germanButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectButton _italianButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectButton _spanishButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectButton _backButton;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _indexer ??= GetComponentInChildren<UISelectElementIndexer>();

            _englishButton = this.FindComponent<UISelectButton>("#Content/English Button");
            _koreanButton = this.FindComponent<UISelectButton>("#Content/Korean Button");
            _simplifiedChineseButton = this.FindComponent<UISelectButton>("#Content/Simplified Chinese Button");
            _traditionalChineseButton = this.FindComponent<UISelectButton>("#Content/Traditional Chinese Button");
            _frenchButton = this.FindComponent<UISelectButton>("#Content/French Button");
            _germanButton = this.FindComponent<UISelectButton>("#Content/German Button");
            _italianButton = this.FindComponent<UISelectButton>("#Content/Italian Button");
            _spanishButton = this.FindComponent<UISelectButton>("#Content/Spanish Button");
            _backButton = this.FindComponent<UISelectButton>("#Content/Back Button");
        }

        protected override void OnStart()
        {
            base.OnStart();

            _englishButton?.OnPointerClickLeftEvent.AddListener(SetEnglishLanguage);
            _koreanButton?.OnPointerClickLeftEvent.AddListener(SetKoreanLanguage);
            _simplifiedChineseButton?.OnPointerClickLeftEvent.AddListener(SetSimplifiedChineseLanguage);
            _traditionalChineseButton?.OnPointerClickLeftEvent.AddListener(SetTraditionalChineseLanguage);
            _frenchButton?.OnPointerClickLeftEvent.AddListener(SetFrenchLanguage);
            _germanButton?.OnPointerClickLeftEvent.AddListener(SetGermanLanguage);
            _italianButton?.OnPointerClickLeftEvent.AddListener(SetItalianLanguage);
            _spanishButton?.OnPointerClickLeftEvent.AddListener(SetSpanishLanguage);
            _backButton?.OnPointerClickLeftEvent.AddListener(Hide);
        }

        protected override void OnShow()
        {
            base.OnShow();

            SetActiveEventButtonAll();
        }

        private void SetEnglishLanguage()
        {
            GameSetting.Instance.Language.SetLanguage(LanguageNames.English);
            SetActiveEventButtonAll();
        }

        private void SetKoreanLanguage()
        {
            GameSetting.Instance.Language.SetLanguage(LanguageNames.Korean);
            SetActiveEventButtonAll();
        }

        private void SetSimplifiedChineseLanguage()
        {
            GameSetting.Instance.Language.SetLanguage(LanguageNames.SimplifiedChinese);
            SetActiveEventButtonAll();
        }

        private void SetTraditionalChineseLanguage()
        {
            GameSetting.Instance.Language.SetLanguage(LanguageNames.TraditionalChinese);
            SetActiveEventButtonAll();
        }

        private void SetFrenchLanguage()
        {
            GameSetting.Instance.Language.SetLanguage(LanguageNames.French);
            SetActiveEventButtonAll();
        }

        private void SetGermanLanguage()
        {
            GameSetting.Instance.Language.SetLanguage(LanguageNames.German);
            SetActiveEventButtonAll();
        }

        private void SetItalianLanguage()
        {
            GameSetting.Instance.Language.SetLanguage(LanguageNames.Italian);
            SetActiveEventButtonAll();
        }

        private void SetSpanishLanguage()
        {
            GameSetting.Instance.Language.SetLanguage(LanguageNames.Spanish);
            SetActiveEventButtonAll();
        }

        private void SetActiveEventButtonAll()
        {
            SetButtonSelected(_englishButton, GameSetting.Instance.Language.Name == LanguageNames.English);
            SetButtonSelected(_koreanButton, GameSetting.Instance.Language.Name == LanguageNames.Korean);
            SetButtonSelected(_simplifiedChineseButton, GameSetting.Instance.Language.Name == LanguageNames.SimplifiedChinese);
            SetButtonSelected(_traditionalChineseButton, GameSetting.Instance.Language.Name == LanguageNames.TraditionalChinese);
            SetButtonSelected(_frenchButton, GameSetting.Instance.Language.Name == LanguageNames.French);
            SetButtonSelected(_germanButton, GameSetting.Instance.Language.Name == LanguageNames.German);
            SetButtonSelected(_italianButton, GameSetting.Instance.Language.Name == LanguageNames.Italian);
            SetButtonSelected(_spanishButton, GameSetting.Instance.Language.Name == LanguageNames.Spanish);
        }
    }
}