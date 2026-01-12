using Sirenix.OdinInspector;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UCGameOptionLanguage : UCGameOptionBase
    {
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectElement _englishButton;
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectElement _koreanButton;
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectElement _simplifiedChineseButton;
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectElement _traditionalChineseButton;
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectElement _frenchButton;
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectElement _germanButton;
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectElement _italianButton;
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectElement _spanishButton;
        [FoldoutGroup("#Language")]
        [SerializeField] private UISelectElement _backButton;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _indexer ??= GetComponentInChildren<UISelectElementIndexer>();

            _englishButton = this.FindComponent<UISelectElement>("#Content/English Button");
            _koreanButton = this.FindComponent<UISelectElement>("#Content/Korean Button");
            _simplifiedChineseButton = this.FindComponent<UISelectElement>("#Content/Simplified Chinese Button");
            _traditionalChineseButton = this.FindComponent<UISelectElement>("#Content/Traditional Chinese Button");
            _frenchButton = this.FindComponent<UISelectElement>("#Content/French Button");
            _germanButton = this.FindComponent<UISelectElement>("#Content/German Button");
            _italianButton = this.FindComponent<UISelectElement>("#Content/Italian Button");
            _spanishButton = this.FindComponent<UISelectElement>("#Content/Spanish Button");
            _backButton = this.FindComponent<UISelectElement>("#Content/Back Button");
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

            HideUnderlineEventButton(_englishButton);
            HideUnderlineEventButton(_koreanButton);
            HideUnderlineEventButton(_simplifiedChineseButton);
            HideUnderlineEventButton(_traditionalChineseButton);
            HideUnderlineEventButton(_frenchButton);
            HideUnderlineEventButton(_germanButton);
            HideUnderlineEventButton(_italianButton);
            HideUnderlineEventButton(_spanishButton);

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
            SetActiveEventButton(_englishButton, GameSetting.Instance.Language.Name == LanguageNames.English);
            SetActiveEventButton(_koreanButton, GameSetting.Instance.Language.Name == LanguageNames.Korean);
            SetActiveEventButton(_simplifiedChineseButton, GameSetting.Instance.Language.Name == LanguageNames.SimplifiedChinese);
            SetActiveEventButton(_traditionalChineseButton, GameSetting.Instance.Language.Name == LanguageNames.TraditionalChinese);
            SetActiveEventButton(_frenchButton, GameSetting.Instance.Language.Name == LanguageNames.French);
            SetActiveEventButton(_germanButton, GameSetting.Instance.Language.Name == LanguageNames.German);
            SetActiveEventButton(_italianButton, GameSetting.Instance.Language.Name == LanguageNames.Italian);
            SetActiveEventButton(_spanishButton, GameSetting.Instance.Language.Name == LanguageNames.Spanish);
        }
    }
}