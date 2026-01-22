using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIShortcutElement : XBehaviour
    {
        [FoldoutGroup("UIShortcutElement")]
        public ActionNames Name;

        [FoldoutGroup("UIShortcutElement")]
        public bool AutoMode;

        [FoldoutGroup("UIShortcutElement")]
        public bool RefreshTextPoint;

        [FoldoutGroup("UIShortcutElement-Component")]
        public UIShortcut Shortcut;

        [FoldoutGroup("UIShortcutElement-Component")]
        public UILocalizedText ShortcutText;

        [FoldoutGroup("UIShortcutElement-Component")]
        public HorizontalLayoutGroup LayoutGroup;

        [FoldoutGroup("UIShortcutElement-Enable Mode")]
        public ShortcutEnableMode EnableMode = ShortcutEnableMode.All;

        [FoldoutGroup("UIShortcutElement-Enable Mode")]
        [SerializeField, HideInInspector] private ShortcutEnableMode _lastAppliedEnableMode = ShortcutEnableMode.None;

        [FoldoutGroup("#String")]
        public string NameString;

        #region Editor

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Shortcut = GetComponentInChildren<UIShortcut>();
            ShortcutText = GetComponentInChildren<UILocalizedText>();
            LayoutGroup = GetComponent<HorizontalLayoutGroup>();
        }

        public override void AutoNaming()
        {
            if (Name != ActionNames.None)
            {
                SetGameObjectName(string.Format("UIShortcutElement({0})", Name));
            }
            else
            {
                SetGameObjectName("UIShortcutElement");
            }
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Name != ActionNames.None)
            {
                NameString = Name.ToString();
                Shortcut.ActionName = Name;
            }
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
            SyncEnableModeState();
        }

        #endregion Editor

        #region Unity Event

        protected void Awake()
        {
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            Refresh();
        }

        #endregion Unity Event

        #region Global Event

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent<ControllerType>.Register(GlobalEventType.GAME_CONTROLLER_TYPE_CHANGED, OnGameControllerTypeChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent<ControllerType>.Unregister(GlobalEventType.GAME_CONTROLLER_TYPE_CHANGED, OnGameControllerTypeChanged);
        }

        private void OnGameControllerTypeChanged(ControllerType controllerType)
        {
            if (AutoMode)
            {
                Refresh();
            }
        }


        #endregion Global Event

        public void Refresh(ActionNames actionName, string stringKey = null)
        {
            Name = actionName;

            if (Shortcut != null)
            {
                Shortcut.ActionName = Name;
                Shortcut.EnableMode = EnableMode; // EnableMode 동기화
                Shortcut.Refresh();
            }

            if (ShortcutText != null)
            {
                if (!string.IsNullOrEmpty(stringKey))
                {
                    ShortcutText.SetStringKey(stringKey);
                }
            }
        }

        public void Refresh()
        {
            if (Shortcut != null)
            {
                Shortcut.ActionName = Name;
                Shortcut.EnableMode = EnableMode; // EnableMode 동기화
                Shortcut.Refresh();
            }
        }

        public void RefreshSizeX()
        {
            if (Shortcut != null && ShortcutText != null)
            {
                float sizeX = Shortcut.rectTransform.sizeDelta.x + ShortcutText.rectTransform.sizeDelta.x;
                float sizeY = rectTransform.sizeDelta.y;
                rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
            }
        }

        private void SyncEnableModeState()
        {
            if (_lastAppliedEnableMode != EnableMode)
            {
                _lastAppliedEnableMode = EnableMode;

                // 내부 UIShortcut의 EnableMode 동기화
                if (Shortcut != null)
                {
                    Shortcut.EnableMode = EnableMode;
                    Shortcut.Refresh();
                }
            }
        }

        private bool ShouldDisplayForController(ControllerType controllerType)
        {
            if (EnableMode == ShortcutEnableMode.None)
            {
                return false;
            }

            // 컨트롤러 타입을 플래그로 변환
            ShortcutEnableMode currentControllerFlag = controllerType switch
            {
                ControllerType.Joystick => ShortcutEnableMode.Joystick,
                ControllerType.Mouse => ShortcutEnableMode.Mouse,
                _ => ShortcutEnableMode.Keyboard, // Keyboard나 기타 타입은 Keyboard로 처리
            };

            // 지정된 컨트롤러 타입이 EnableMode에 포함되어 있는지 확인
            return (EnableMode & currentControllerFlag) != ShortcutEnableMode.None;
        }

    }
}