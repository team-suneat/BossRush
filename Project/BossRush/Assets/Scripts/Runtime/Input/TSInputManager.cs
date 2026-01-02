using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public enum JoystickTypes
    {
        None,
        Xbox,
        PlayStation,
        PlayStation5,
        Nintendo,
    }

    public partial class TSInputManager : Singleton<TSInputManager>
    {
#if VIRIDIAN_PATCH
        const ControllerType DEFAULT_CONTROLLER = ControllerType.Joystick;

        /*Default joystick type - console invariant*/
#if UNITY_SWITCH
        const JoystickTypes DEFAULT_JOYSTICK = JoystickTypes.Nintendo;
#elif UNITY_GAMECORE
        const JoystickTypes DEFAULT_JOYSTICK = JoystickTypes.Xbox;
#elif UNITY_PS5
        const JoystickTypes DEFAULT_JOYSTICK = JoystickTypes.PlayStation5;
#elif UNITY_PS4
        const JoystickTypes DEFAULT_JOYSTICK = JoystickTypes.PlayStation;
#else
        /*editor/catch all*/
        const JoystickTypes DEFAULT_JOYSTICK = JoystickTypes.Nintendo;
#endif

#else
        private const ControllerType DEFAULT_CONTROLLER = ControllerType.Keyboard;
        private const JoystickTypes DEFAULT_JOYSTICK = JoystickTypes.None;
#endif

        public static Vector2 ThresholdCharacter = new(0.1f, 0.4f);
        public static Vector2 ThresholdUI = new(0.5f, 0.5f);

        private static readonly Action _rewiredInitCallback = OnRewiredInitialized;

        private Vector2 _primaryMovement = Vector2.zero;
        private Vector2 _rightPadMovement = Vector2.zero;

        private ControllerType _fixedControllerType = ControllerType.Custom;
        private ControllerType _currentControllerType = DEFAULT_CONTROLLER;
        private JoystickTypes _currentJoystickType = DEFAULT_JOYSTICK;

        #region Parameters

        public Player InputPlayer { get; private set; }

        public ControllerType FixedControllerType
        {
            get
            {
                if (TSGameDefine.EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    return ControllerType.Custom;
                }

                return _fixedControllerType;
            }
            set
            {
                if (TSGameDefine.EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    _fixedControllerType = value;
                }
            }
        }

        public JoystickTypes FixedJoystickType
        {
            get
            {
                if (TSGameDefine.EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    return DEFAULT_JOYSTICK;
                }

                return DEFAULT_JOYSTICK;
            }
        }

        public ControllerType CurrentControllerType
        {
            get
            {
                if (FixedControllerType != ControllerType.Custom)
                {
                    return FixedControllerType;
                }
                return _currentControllerType;
            }
        }

        public JoystickTypes CurrentJoystickType
        {
            get
            {
                if (FixedJoystickType != JoystickTypes.None)
                {
                    return FixedJoystickType;
                }

                return _currentJoystickType;
            }
        }

        public Controller CurrentJoystick { get; private set; }
        public Vector2 PrimaryMovement => _primaryMovement;
        public Vector2 RightPadMovement => _rightPadMovement;

        public bool IsInitialized { get; set; }

        #endregion Parameters

        public void Initialize()
        {
            if (!ReInput.isReady)
            {
                ReInput.InitializedEvent -= _rewiredInitCallback;
                ReInput.InitializedEvent += _rewiredInitCallback;
                return;
            }

            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;

            InputPlayer = ReInput.players.GetPlayer(0);

            LoadDefaultMappings();
            LoadMappings();
            EnsureRequiredActionMappings();
            SetupController();
            SetupButtonEvents();

            // 초기화 시 이미 연결된 PlayStation 패드가 있을 경우 매핑 변경
            if (CurrentJoystick != null && CheckPSJoystick(CurrentJoystick.name))
            {
                AddTouchPadMapping(CurrentJoystick);
                SwapOptionsMapping(CurrentJoystick);
                MapOptionsToSkip(CurrentJoystick);
            }

            SubscribeEvents();
        }

        private static void OnRewiredInitialized()
        {
            ReInput.InitializedEvent -= _rewiredInitCallback;
            Instance.Initialize();
        }

        //

        //

        public List<string> GetElementMapsWithAction(ControllerType controllerType, string actionName)
        {
            try
            {
                if (!ValidatePlayer())
                {
                    return null;
                }

                // 1. 액션 이름·방향 보정
                (string targetAction, Pole axis) = ResolveAction(controllerType, actionName);

                // 2. 컨트롤러 ID & 조이스틱 맵 확보
                int controllerId = CurrentJoystick != null ? CurrentJoystick.id : 0;
                Dictionary<string, ActionElementMap> joystickMap =
                    EnsureJoystickMap(controllerId);

                // 3. 매핑 수집
                List<ActionElementMap> elementMaps = new();
                _ = InputPlayer.controllers.maps.GetElementMapsWithAction(
                    controllerType, controllerId, targetAction, true, elementMaps);

                if (elementMaps.Count == 0)
                {
                    Log.Warning(LogTags.Input_ButtonState, "컨트롤러의 매핑이 유효하지 않습니다. {0}, {1}", controllerType, actionName);
                }

                // 4. 반복 처리
                List<string> result = new();
                for (int i = 0; i < elementMaps.Count; i++)
                {
                    ActionElementMap aem = elementMaps[i];
                    ProcessElementMap(aem, controllerType, actionName, axis, joystickMap, result);
                }

                return result;
            }
            catch (Exception e)
            {
                Log.Error("GetElementMapsWithAction 실패: {0}\n{1}", actionName, e);
                return null;
            }
        }

        private bool ValidatePlayer()
        {
            if (InputPlayer != null)
            {
                return true;
            }

            Log.Warning(LogTags.Input, "InputPlayer is null");
            return false;
        }

        private (string, Pole) ResolveAction(ControllerType type, string act)
        {
            if (type != ControllerType.Joystick)
            {
                return (act, Pole.Positive);
            }

            return act switch
            {
                "MoveUp" => (ActionNames.MoveVertical.ToString(), Pole.Positive),
                "MoveDown" => (ActionNames.MoveVertical.ToString(), Pole.Negative),
                "MoveLeft" => (ActionNames.MoveHorizontal.ToString(), Pole.Negative),
                "MoveRight" => (ActionNames.MoveHorizontal.ToString(), Pole.Positive),
                _ => (act, Pole.Positive)
            };
        }

        private Dictionary<string, ActionElementMap> EnsureJoystickMap(int id)
        {
            if (!_defaultJoystickElementMapByController.TryGetValue(id, out Dictionary<string, ActionElementMap> map))
            {
                map = new Dictionary<string, ActionElementMap>();
                _defaultJoystickElementMapByController[id] = map;
            }
            return map;
        }

        private void ProcessElementMap(ActionElementMap aem, ControllerType ctrlType, string actionName, Pole targetAxis, Dictionary<string, ActionElementMap> joyMap, List<string> result)
        {
            // 키보드·마우스
            if (ctrlType != ControllerType.Joystick && aem.keyCode != KeyCode.None)
            {
                AddResult(aem.keyCode.ToString(), ctrlType, actionName, result);
                return;
            }

            // 축 방향 체크
            if (aem.axisContribution != targetAxis)
            {
                return;
            }

            // Identifier 이름 확보
            string keyName = aem.elementIdentifierName;
            if (string.IsNullOrEmpty(keyName) && aem.elementIdentifierId != -1)
            {
                if (joyMap.TryGetValue(aem.elementIdentifierId.ToString(), out ActionElementMap m) && m != null)
                {
                    keyName = m.elementIdentifierName;
                }
            }

            if (string.IsNullOrEmpty(keyName) || keyName.Contains("None"))
            {
                return;
            }

            // 스틱 문자열 변환
            keyName = keyName switch
            {
                "Left Stick Y" => targetAxis == Pole.Positive ? "Left Stick Up" : "Left Stick Down",
                "Left Stick X" => targetAxis == Pole.Positive ? "Left Stick Right" : "Left Stick Left",
                "Right Stick Y" => targetAxis == Pole.Positive ? "Right Stick Up" : "Right Stick Down",
                "Right Stick X" => targetAxis == Pole.Positive ? "Right Stick Right" : "Right Stick Left",
                _ => keyName
            };

            AddResult(keyName, ctrlType, actionName, result);
        }

        private void AddResult(string keyName, ControllerType ctrlType, string actionName, List<string> result)
        {
            if (result.Contains(keyName))
            {
                return;
            }

            result.Add(keyName);
            Log.Progress(LogTags.Input, "{0}:{1} → {2}", ctrlType, actionName, keyName.ToSelectString("None"));
        }

        public void GetInputState()
        {
            if (InputPlayer == null)
            {
                Log.Warning(LogTags.Input, "InputPlayer is null");
                return;
            }

            UpdateMovementAxes();

            if (_buttonList == null)
            {
                return;
            }

            for (int i = 0; i < _buttonList.Count; i++)
            {
                TSInputButton button = _buttonList[i];
                if (button == null) { continue; }
                if (CurrentControllerType == ControllerType.Mouse)
                {
                    if (!button.IsValidKey(ControllerType.Keyboard))
                    {
                        continue;
                    }
                }
                else if (!button.IsValidKey(CurrentControllerType))
                {
                    continue;
                }

                ProcessButtonInput(button);
            }

            RefreshControllerType();
        }

        public TSInputButton GetJoystickInputButton()
        {
            if (InputPlayer == null)
            {
                Log.Warning(LogTags.Input, "InputPlayer is null");
                return null;
            }

            if (_buttonList == null)
            {
                return null;
            }

            if (_buttonList != null)
            {
                for (int i = 0; i < _buttonList.Count; i++)
                {
                    TSInputButton button = _buttonList[i];
                    if (button == null)
                    {
                        continue;
                    }
                    if (button.CheckState(ButtonStates.ButtonDown))
                    {
                        return button;
                    }
                }
            }

            return null;
        }

        private void UpdateMovementAxes()
        {
            _primaryMovement.x = InputPlayer.GetAxisRaw("MoveHorizontal");
            _primaryMovement.y = InputPlayer.GetAxisRaw("MoveVertical");

            _rightPadMovement.x = InputPlayer.GetAxisRaw("UICursorHorizontal2");
            _rightPadMovement.y = InputPlayer.GetAxisRaw("UICursorVertical2");
        }

        private void ProcessButtonInput(TSInputButton button)
        {
            switch (CurrentControllerType)
            {
                case ControllerType.Keyboard:
                case ControllerType.Mouse:
                    ProcessKeyboardOrMouseInput(button);
                    break;

                case ControllerType.Joystick:
                    ProcessJoystickInput(button);
                    break;

                default:
                    Log.Warning(LogTags.Input, "Unhandled controller type: " + CurrentControllerType);
                    break;
            }
        }

        private void ProcessKeyboardOrMouseInput(TSInputButton button)
        {
            List<KeyCode> keyCodes = button.GetKeyCodes(CurrentControllerType);
            bool anyKeyActive = false;
            for (int i = 0; i < keyCodes.Count; i++)
            {
                KeyCode keyCode = keyCodes[i];
                if (Input.GetKeyDown(keyCode))
                {
                    ProcessMovementInput(button);
                    button.TriggerButtonDown();
                    anyKeyActive = true;
                }
                else if (Input.GetKeyUp(keyCode))
                {
                    button.TriggerButtonUp();
                    anyKeyActive = true;
                }
                else if (Input.GetKey(keyCode))
                {
                    ProcessMovementInput(button);
                    button.TriggerButtonPressed();
                    anyKeyActive = true;
                }
            }

            if (!anyKeyActive)
            {
                if (button.CheckState(ButtonStates.ButtonDown) || button.CheckState(ButtonStates.ButtonPressed))
                {
                    Log.Progress(LogTags.Input, "키보드 & 마우스 입력이 없는 경우 버튼 상태를 Off로 전환하여 버튼 상태를 초기화합니다: {0}, {1}", button.ActionName, button.State);
                    button.State.ChangeState(ButtonStates.Off);
                }
            }
        }

        private void ProcessMovementInput(TSInputButton button)
        {
            if (button.ActionName is ActionNames.MoveUp)
            {
                _primaryMovement.y = 1;
            }
            else if (button.ActionName is ActionNames.MoveDown)
            {
                _primaryMovement.y = -1;
            }
            else if (button.ActionName is ActionNames.MoveLeft)
            {
                _primaryMovement.x = -1;
            }
            else if (button.ActionName is ActionNames.MoveRight)
            {
                _primaryMovement.x = 1;
            }
        }

        private void ProcessJoystickInput(TSInputButton button)
        {
            if (InputPlayer.GetButtonDown(button.ButtonID))
            {
                ProcessMovementInput(button);
                button.TriggerButtonDown();
            }
            else if (InputPlayer.GetButtonUp(button.ButtonID))
            {
                button.TriggerButtonUp();
            }
            else if (InputPlayer.GetButton(button.ButtonID))
            {
                ProcessMovementInput(button);
                button.TriggerButtonPressed();
            }
            else if (button.CheckState(ButtonStates.ButtonDown) || button.CheckState(ButtonStates.ButtonPressed))
            {
                Log.Progress(LogTags.Input, "패드 버튼 입력이 없는 경우 버튼 상태를 Off로 전환하여 버튼 상태를 초기화합니다: {0}, {1}", button.ActionName, button.State);
                button.State.ChangeState(ButtonStates.Off);
            }
        }

        public void ProcessButtonStates()
        {
            if (_buttonList != null)
            {
                for (int i = 0; i < _buttonList.Count; i++)
                {
                    TSInputButton button = _buttonList[i];
                    if (button == null)
                    {
                        continue;
                    }

                    if (button.CheckState(ButtonStates.ButtonDown))
                    {
                        button.State.ChangeState(ButtonStates.ButtonPressed);
                    }
                    else if (button.CheckState(ButtonStates.ButtonUp))
                    {
                        button.State.ChangeState(ButtonStates.Off);
                    }
                }
            }
        }

        public void ResetButtonStates()
        {
            if (_buttonList != null)
            {
                for (int i = 0; i < _buttonList.Count; i++)
                {
                    TSInputButton button = _buttonList[i];
                    if (button == null)
                    {
                        continue;
                    }

                    if (button.CheckState(ButtonStates.ButtonDown))
                    {
                        button.State.ChangeState(ButtonStates.Off);
                    }
                    if (button.CheckState(ButtonStates.ButtonPressed))
                    {
                        button.State.ChangeState(ButtonStates.Off);
                    }
                }
            }
        }

        public void ResetButtonState(ActionNames actionName)
        {
            if (!_buttonDictionary.IsValid())
            {
                return;
            }

            if (_buttonDictionary.ContainsKey(actionName))
            {
                TSInputButton button = _buttonDictionary[actionName];
                if (button.CheckState(ButtonStates.ButtonDown))
                {
                    button.State.ChangeState(ButtonStates.Off);
                }
                else if (button.CheckState(ButtonStates.ButtonPressed))
                {
                    button.State.ChangeState(ButtonStates.Off);
                }
            }
        }

        //

        //

        public bool CheckUIMoveLeft()
        {
            if (CheckButtonState(ActionNames.UIMoveLeft, ButtonStates.ButtonDown) ||
                PrimaryMovement.x < -ThresholdUI.x ||
                RightPadMovement.x < -ThresholdUI.x)
            {
                return true;
            }

            return false;
        }

        public bool CheckUIMoveRight()
        {
            if (CheckButtonState(ActionNames.UIMoveRight, ButtonStates.ButtonDown) ||
                PrimaryMovement.x > ThresholdUI.x ||
                RightPadMovement.x > ThresholdUI.x)
            {
                return true;
            }

            return false;
        }

        public bool CheckUIMoveUp()
        {
            if (CheckButtonState(ActionNames.UIMoveUp, ButtonStates.ButtonDown) ||
                PrimaryMovement.y > ThresholdUI.y ||
                RightPadMovement.y > ThresholdUI.y)
            {
                return true;
            }

            return false;
        }

        public bool CheckUIMoveDown()
        {
            if (CheckButtonState(ActionNames.UIMoveDown, ButtonStates.ButtonDown) ||
                PrimaryMovement.y < -ThresholdUI.y ||
                RightPadMovement.y < -ThresholdUI.y)
            {
                return true;
            }

            return false;
        }
    }
}