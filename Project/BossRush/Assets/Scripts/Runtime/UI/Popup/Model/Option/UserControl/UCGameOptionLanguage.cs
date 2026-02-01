using Sirenix.OdinInspector;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UCGameOptionLanguage : UCGameOptionBase
    {
        [FoldoutGroup("#Language")]
        [SerializeField] private UIImmediateButton _englishButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UIImmediateButton _koreanButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UIImmediateButton _simplifiedChineseButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UIImmediateButton _traditionalChineseButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UIImmediateButton _frenchButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UIImmediateButton _germanButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UIImmediateButton _italianButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UIImmediateButton _spanishButton;

        [FoldoutGroup("#Language")]
        [SerializeField] private UIImmediateButton _backButton;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _indexer ??= GetComponentInChildren<UISelectElementIndexer>();

            _englishButton = this.FindComponent<UIImmediateButton>("#Content/English Button");
            _koreanButton = this.FindComponent<UIImmediateButton>("#Content/Korean Button");
            _simplifiedChineseButton = this.FindComponent<UIImmediateButton>("#Content/Simplified Chinese Button");
            _traditionalChineseButton = this.FindComponent<UIImmediateButton>("#Content/Traditional Chinese Button");
            _frenchButton = this.FindComponent<UIImmediateButton>("#Content/French Button");
            _germanButton = this.FindComponent<UIImmediateButton>("#Content/German Button");
            _italianButton = this.FindComponent<UIImmediateButton>("#Content/Italian Button");
            _spanishButton = this.FindComponent<UIImmediateButton>("#Content/Spanish Button");
            _backButton = this.FindComponent<UIImmediateButton>("#Content/Back Button");
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