using Rewired;
using Sirenix.OdinInspector;
using TSFramework;

namespace TeamSuneat.UserInterface
{
    public class UIGameOptionPopup : UIPopup
    {
        public override UIPopupNames Name => UIPopupNames.GameOption;

        [FoldoutGroup("#Popup-Option")] public bool IsPausePopup;
        [FoldoutGroup("#Popup-Option")] public UCGameOptionAccessibility Accessibility;
        [FoldoutGroup("#Popup-Option")] public UCGameOptionLanguage Language;
        [FoldoutGroup("#Popup-Option")] public UCGameOptionVideo Video;
        [FoldoutGroup("#Popup-Option")] public UCGameOptionAudio Audio;
        [FoldoutGroup("#Popup-Option")] public UISelectElementIndexer ContentIndexer;

        [FoldoutGroup("#Popup-Option/Indexer")]
        public UISelectElementIndexer[] Indexers;

        [FoldoutGroup("#Popup-Option/Buttons")] public UISelectElement AccessibilityButton;
        [FoldoutGroup("#Popup-Option/Buttons")] public UISelectElement LanguageButton;
        [FoldoutGroup("#Popup-Option/Buttons")] public UISelectElement VideoButton;
        [FoldoutGroup("#Popup-Option/Buttons")] public UISelectElement AudioButton;
        [FoldoutGroup("#Popup-Option/Buttons")] public UISelectElement BackButton;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Accessibility = GetComponentInChildren<UCGameOptionAccessibility>();
            Language = GetComponentInChildren<UCGameOptionLanguage>();
            Video = GetComponentInChildren<UCGameOptionVideo>();
            Audio = GetComponentInChildren<UCGameOptionAudio>();
            ContentIndexer = this.FindComponent<UISelectElementIndexer>("Rect/#Option/#Content");

            Indexers = GetComponentsInChildren<UISelectElementIndexer>();

            AccessibilityButton = this.FindComponent<UISelectElement>("Rect/#Option/#Content/Accessibility Button");
            LanguageButton = this.FindComponent<UISelectElement>("Rect/#Option/#Content/Language Button");
            VideoButton = this.FindComponent<UISelectElement>("Rect/#Option/#Content/Video Button");
            AudioButton = this.FindComponent<UISelectElement>("Rect/#Option/#Content/Audio Button");
            BackButton = this.FindComponent<UISelectElement>("Rect/#Option/#Content/Back Button");
        }

        protected override void OnStart()
        {
            base.OnStart();

            IsPausePopup = this is UIPausePopup;

            AccessibilityButton?.RegisterClickEvent(ShowAccessibility);
            LanguageButton?.RegisterClickEvent(ShowLanguage);
            VideoButton?.RegisterClickEvent(ShowVideo);
            AudioButton?.RegisterClickEvent(ShowAudio);

            if (!IsPausePopup)
            {
                BackButton?.RegisterClickEvent(CloseWithFailure);
            }

            if (Accessibility != null)
            {
                Accessibility.HideCallback = OnHideAccessibility;
            }
            if (Language != null)
            {
                Language.HideCallback = OnHideLanguage;
            }
            if (Video != null)
            {
                Video.HideCallback = OnHideVideo;
            }
            if (Audio != null)
            {
                Audio.HideCallback = OnHideAudio;
            }
        }

        public override void Open()
        {
            base.Open();

            UIManager.Instance.DetailsManager.Clear();

            Accessibility?.SetActive(false);
            Language?.SetActive(false);
            Video?.SetActive(false);
            Audio?.SetActive(false);
        }

        public override void OnClose(bool result)
        {
            base.OnClose(result);

            UIManager.Instance.PopupManager.SelectController.Clear();
        }

        public override void LogicUpdate()
        {
            if (UIManager.Instance.PopupManager.IsLockPopupWhileOpen)
            {
                return;
            }

            _ = TryOptionLogicUpdate();
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsPausePopup)
            {
                SetupButtonIndex();

                UIManager.Instance.PopupManager.SelectController.RegisterPointerEvents(PointerEvents);
            }
        }

        public override void SelectFirstSlotEvent()
        {
            base.SelectFirstSlotEvent();

            UISelectElement firstSlot = AccessibilityButton;
            if (firstSlot != null)
            {
                UIManager.Instance.PopupManager.SelectController.Select(firstSlot.SelectIndex);
            }
        }

        protected bool TryOptionLogicUpdate()
        {
            if (Accessibility != null && Accessibility.ActiveSelf)
            {
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UICancel, ButtonStates.ButtonDown))
                {
                    Accessibility.Hide();
                }
                return true;
            }
            else if (Language != null && Language.ActiveSelf)
            {
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UICancel, ButtonStates.ButtonDown))
                {
                    Language.Hide();
                }
                return true;
            }
            else if (Video != null && Video.ActiveSelf)
            {
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UICancel, ButtonStates.ButtonDown))
                {
                    Video.Hide();
                }
                else
                {
                    Video.LogicUpdate();
                }
                return true;
            }
            else if (Audio != null && Audio.ActiveSelf)
            {
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UICancel, ButtonStates.ButtonDown))
                {
                    Audio.Hide();
                }
                else
                {
                    Audio.LogicUpdate();
                }
                return true;
            }

            return false;
        }

        private void SetupButtonIndex()
        {
            if (Indexers != null)
            {
                for (int i = 0; i < Indexers.Length; i++)
                {
                    Indexers[i].SetupIndex();
                }
            }
        }

        private void ActivateButtonsRaycast()
        {
            AccessibilityButton?.ActivateRaycast();
            LanguageButton?.ActivateRaycast();
            VideoButton?.ActivateRaycast();
            AudioButton?.ActivateRaycast();
            BackButton?.ActivateRaycast();
        }

        private void DeactivateButtonsRaycast()
        {
            AccessibilityButton?.DeactivateRaycast();
            LanguageButton?.DeactivateRaycast();
            VideoButton?.DeactivateRaycast();
            AudioButton?.DeactivateRaycast();
            BackButton?.DeactivateRaycast();
        }

        private void ShowAccessibility()
        {
            DeactivateButtonsRaycast();
            Accessibility.Show();

            if (!IsPausePopup)
            {
                UIManager.Instance.PopupManager.LockPopup();
            }
        }

        private void OnHideAccessibility()
        {
            ResetUnderlineEventButton(AccessibilityButton);
            ResetUnderlineEventButton(LanguageButton);

            Accessibility.DeactivateRaycast();
            ActivateButtonsRaycast();

            UIManager.Instance.PopupManager.SelectController.Select(AccessibilityButton.SelectIndex);
            if (!IsPausePopup)
            {
                UIManager.Instance.PopupManager.UnlockPopup(GameDefine.INPUT_WAIT_TIME);
            }
        }

        private void ShowLanguage()
        {
            DeactivateButtonsRaycast();
            Language.Show();

            if (!IsPausePopup)
            {
                UIManager.Instance.PopupManager.LockPopup();
            }
        }

        private void OnHideLanguage()
        {
            ActivateButtonsRaycast();
            Language.DeactivateRaycast();

            UIManager.Instance.PopupManager.SelectController.Select(LanguageButton.SelectIndex);
            if (!IsPausePopup)
            {
                UIManager.Instance.PopupManager.UnlockPopup(GameDefine.INPUT_WAIT_TIME);
            }
        }

        private void ShowVideo()
        {
            DeactivateButtonsRaycast();
            Video.Show();

            if (!IsPausePopup)
            {
                UIManager.Instance.PopupManager.LockPopup();
            }
        }

        private void OnHideVideo()
        {
            ActivateButtonsRaycast();
            Video.DeactivateRaycast();

            UIManager.Instance.PopupManager.SelectController.Select(VideoButton.SelectIndex);

            if (!IsPausePopup)
            {
                UIManager.Instance.PopupManager.UnlockPopup(GameDefine.INPUT_WAIT_TIME);
            }
        }

        private void ShowAudio()
        {
            DeactivateButtonsRaycast();
            Audio.Show();

            if (!IsPausePopup)
            {
                UIManager.Instance.PopupManager.LockPopup();
            }
        }

        private void OnHideAudio()
        {
            ActivateButtonsRaycast();
            Audio.DeactivateRaycast();

            UIManager.Instance.PopupManager.SelectController.Select(AudioButton.SelectIndex);

            if (!IsPausePopup)
            {
                UIManager.Instance.PopupManager.UnlockPopup(GameDefine.INPUT_WAIT_TIME);
            }
        }

        private void ResetUnderlineEventButton(UISelectElement button)
        {
            if (button != null)
            {
                button.ResetUnderline();
            }
        }
    }
}