using Sirenix.OdinInspector;

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
        [FoldoutGroup("#Popup-Option")] public UCGameOptionButtons OptionButtons;

        [FoldoutGroup("#Popup-Option/Indexer")]
        public UISelectElementIndexer[] Indexers;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Accessibility = GetComponentInChildren<UCGameOptionAccessibility>();
            Language = GetComponentInChildren<UCGameOptionLanguage>();
            Video = GetComponentInChildren<UCGameOptionVideo>();
            Audio = GetComponentInChildren<UCGameOptionAudio>();
            OptionButtons = GetComponentInChildren<UCGameOptionButtons>();

            Indexers = GetComponentsInChildren<UISelectElementIndexer>();

            CancelButton ??= this.FindComponent<UISelectButton>("Rect/#Content/#Buttons/Back Button");
        }

        protected override void OnStart()
        {
            base.OnStart();

            OptionButtons?.Initialize(IsPausePopup);
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

        public override void LogicUpdate()
        {
            TryOptionLogicUpdate();
        }

        public override void Activate()
        {
            base.Activate();

            if (!IsPausePopup)
            {
                SetupButtonIndex();
                UIManager.Instance.SelectController.RegisterPointerEvents(PointerEvents);
            }
        }

        public override void SelectFirstSlotEvent()
        {
            base.SelectFirstSlotEvent();

            UISelectButton firstSlot = OptionButtons?.AccessibilityButton;
            if (firstSlot != null)
            {
                UIManager.Instance.SelectController.Select(firstSlot.SelectIndex);
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
    }
}