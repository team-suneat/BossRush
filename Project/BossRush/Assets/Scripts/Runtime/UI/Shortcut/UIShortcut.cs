using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIShortcut : XBehaviour
    {
        [FoldoutGroup("UIShortcut")] public ActionNames ActionName;
        [FoldoutGroup("UIShortcut")] public string ActionNameString;
        [FoldoutGroup("UIShortcut")] public Image ShortcutImage;

        [InfoBox("명령 이름에 따라 이미지가 변경되지 않습니다.")]
        [FoldoutGroup("UIShortcut")] public bool Custom;

        [FoldoutGroup("UIShortcut-Joystick")]
        public bool UseStickSprite;

        [FoldoutGroup("UIShortcut-Joystick")]
        public Vector3 JoystickOffset;

        // EnableMode는 이제 UIShortcutElement에서만 관리됨
        internal ShortcutEnableMode EnableMode = ShortcutEnableMode.All;

        private Vector3 _defaultPosition;
        private IShortcutSpriteStrategy _spriteStrategy;
        private ControllerType _controllerType = ControllerType.Custom;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            ShortcutImage = GetComponent<Image>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (ActionName != ActionNames.None)
            {
                ActionNameString = ActionName.ToString();
            }
        }

        private void OnValidate()
        {
            if (!Custom)
            {
                if (!EnumEx.ConvertTo(ref ActionName, ActionNameString))
                {
                    Debug.LogWarningFormat("ActionName({0})을 변환할 수 없습니다. {1}", ActionNameString, this.GetHierarchyPath());
                }
            }
            else
            {
                ActionName = ActionNames.None;
                ActionNameString = string.Empty;
            }
        }

        public override void AutoNaming()
        {
            if (ActionName != ActionNames.None)
            {
                SetGameObjectName(string.Format("UIShortcut({0})", ActionName));
            }
            else
            {
                SetGameObjectName("UIShortcut");
            }
        }

        //------------------------------------------------------------------------

        protected void Awake()
        {
            if (ShortcutImage == null)
            {
                ShortcutImage = GetComponentInChildren<Image>();
            }

            _defaultPosition = anchoredPosition3D;
        }

        protected override void OnStart()
        {
            base.OnStart();

            InitShortcut();
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            InitShortcut();
        }

        //------------------------------------------------------------------------

        private void InitShortcut()
        {
            if (!Custom)
            {
                ControllerType controllerType = TSInputManager.Instance.CurrentControllerType;
                if (ShouldHideShortcut(controllerType))
                {
                    ShortcutImage.enabled = false;
                    return;
                }

                Refresh();
            }
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent<ControllerType>.Register(GlobalEventType.GAME_CONTROLLER_TYPE_CHANGED, OnGameControllerTypeChanged);
            GlobalEvent<ControllerType, ActionNames, Pole, string>.Register(GlobalEventType.GAME_INPUT_KEY_CHANGED, OnInputKeyChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent<ControllerType>.Unregister(GlobalEventType.GAME_CONTROLLER_TYPE_CHANGED, OnGameControllerTypeChanged);
            GlobalEvent<ControllerType, ActionNames, Pole, string>.Unregister(GlobalEventType.GAME_INPUT_KEY_CHANGED, OnInputKeyChanged);
        }

        private void OnGameControllerTypeChanged(ControllerType controllerType)
        {
            if (Custom) { return; }
            if (controllerType is ControllerType.Mouse) { controllerType = ControllerType.Keyboard; }
            if (_controllerType == controllerType) { return; }

            _controllerType = controllerType;
            Refresh();
        }

        private void OnInputKeyChanged(ControllerType controllerType, ActionNames actionName, Pole axisContribution, string keyCode)
        {
            if (ActionName == actionName)
            {
                RefreshShortcutSpriteByAction();
            }
        }

        public void SetAction(ActionNames newActionName)
        {
            ActionName = newActionName;

            Refresh();
        }

        public void Refresh()
        {
            RefreshAnchoredPosition3D();
            RefreshActiveShortcutImage();
            RefreshShortcutSpriteByAction();
        }

        private void RefreshShortcutSpriteByAction()
        {
            if (!ShortcutImage.enabled)
            {
                return;
            }

            _spriteStrategy = ShortcutSpriteStrategyFactory.GetStrategy(ActionName, UseStickSprite);
            _spriteStrategy?.ApplySprite(ShortcutImage, ActionName);
        }

        //

        private void RefreshAnchoredPosition3D()
        {
            if (TSInputManager.Instance.CurrentControllerType == ControllerType.Joystick)
            {
                anchoredPosition3D = _defaultPosition + JoystickOffset;
            }
            else
            {
                anchoredPosition3D = _defaultPosition;
            }
        }

        private void RefreshActiveShortcutImage()
        {
            ControllerType controllerType = TSInputManager.Instance.CurrentControllerType;
            if (ShouldHideShortcut(controllerType))
            {
                ShortcutImage.enabled = false;
                return;
            }

            if (!ShortcutImage.enabled)
            {
                ShortcutImage.enabled = true;
            }
        }

        //

        public override void SetActive(bool value)
        {
            if (value)
            {
                if (ActionName != ActionNames.None)
                {
                    base.SetActive(true);
                    return;
                }
            }

            base.SetActive(value);
        }


        private bool ShouldHideShortcut(ControllerType controllerType)
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
            return (EnableMode & currentControllerFlag) == ShortcutEnableMode.None;
        }
    }
}