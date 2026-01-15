using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UCGameOptionButtons : XBehaviour
    {
        [FoldoutGroup("#Buttons")]
        [SerializeField] private UISelectButton _accessibilityButton;

        [FoldoutGroup("#Buttons")]
        [SerializeField] private UISelectButton _languageButton;

        [FoldoutGroup("#Buttons")]
        [SerializeField] private UISelectButton _videoButton;

        [FoldoutGroup("#Buttons")]
        [SerializeField] private UISelectButton _audioButton;

        [FoldoutGroup("#Options")]
        [SerializeField] private UCGameOptionAccessibility _accessibility;

        [FoldoutGroup("#Options")]
        [SerializeField] private UCGameOptionLanguage _language;

        [FoldoutGroup("#Options")]
        [SerializeField] private UCGameOptionVideo _video;

        [FoldoutGroup("#Options")]
        [SerializeField] private UCGameOptionAudio _audio;

        private bool _isPausePopup;

        public UISelectButton AccessibilityButton => _accessibilityButton;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _accessibilityButton = this.FindComponent<UISelectButton>("#Buttons/Accessibility Button");
            _languageButton = this.FindComponent<UISelectButton>("#Buttons/Language Button");
            _videoButton = this.FindComponent<UISelectButton>("#Buttons/Video Button");
            _audioButton = this.FindComponent<UISelectButton>("#Buttons/Audio Button");

            _accessibility = GetComponentInChildren<UCGameOptionAccessibility>();
            _language = GetComponentInChildren<UCGameOptionLanguage>();
            _video = GetComponentInChildren<UCGameOptionVideo>();
            _audio = GetComponentInChildren<UCGameOptionAudio>();
        }

        public void Initialize(bool isPausePopup)
        {
            _isPausePopup = isPausePopup;

            _accessibilityButton?.OnPointerClickLeftEvent.AddListener(() => ShowOption(_accessibility, _accessibilityButton));
            _languageButton?.OnPointerClickLeftEvent.AddListener(() => ShowOption(_language, _languageButton));
            _videoButton?.OnPointerClickLeftEvent.AddListener(() => ShowOption(_video, _videoButton));
            _audioButton?.OnPointerClickLeftEvent.AddListener(() => ShowOption(_audio, _audioButton));

            if (_accessibility != null)
            {
                _accessibility.HideCallback = () => OnHideOption(_accessibility, _accessibilityButton);
            }
            if (_language != null)
            {
                _language.HideCallback = () => OnHideOption(_language, _languageButton);
            }
            if (_video != null)
            {
                _video.HideCallback = () => OnHideOption(_video, _videoButton);
            }
            if (_audio != null)
            {
                _audio.HideCallback = () => OnHideOption(_audio, _audioButton);
            }
        }

        public void ActivateAllButtonsRaycast()
        {
            _accessibilityButton?.ActivateRaycast();
            _languageButton?.ActivateRaycast();
            _videoButton?.ActivateRaycast();
            _audioButton?.ActivateRaycast();
        }

        public void DeactivateAllButtonsRaycast()
        {
            _accessibilityButton?.DeactivateRaycast();
            _languageButton?.DeactivateRaycast();
            _videoButton?.DeactivateRaycast();
            _audioButton?.DeactivateRaycast();
        }

        private void ShowOption(UCGameOptionBase option, UISelectButton button)
        {
            if (option == null) return;

            DeactivateAllButtonsRaycast();
            option.Show();

            if (!_isPausePopup)
            {
                UIManager.Instance.PopupManager.LockPopup();
            }
        }

        private void OnHideOption(UCGameOptionBase option, UISelectButton button)
        {
            if (option == null || button == null) return;

            ActivateAllButtonsRaycast();
            option.DeactivateRaycastAll();

            UIManager.Instance.SelectController.Select(button.SelectIndex);

            if (!_isPausePopup)
            {
                UIManager.Instance.PopupManager.UnlockPopup(GameDefine.INPUT_WAIT_TIME);
            }
        }
    }
}