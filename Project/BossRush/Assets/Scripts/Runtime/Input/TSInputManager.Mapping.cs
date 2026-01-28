using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine;

namespace TeamSuneat
{
    public partial class TSInputManager
    {
        private static class InputConstants
        {
            public const string BUTTON_KEY_OPTIONS = "options";
            public const string BUTTON_KEY_TOUCHPAD = "touchpad/button";
            public const string BUTTON_KEY_TOUCHPAD_ALT = "touchpad_button";

            public const string KEYCODE_SHARE = "Share";
            public const string KEYCODE_OPTIONS = "Options";
            public const string KEYCODE_TOUCHPAD_BUTTON = "Touchpad Button";
            public const string KEYCODE_VIEW = "View";
            public const string KEYCODE_MENU = "Menu";
            public const string KEYCODE_GUIDE = "Guide";

            public const string KEYCODE_L2 = "L2";
            public const string KEYCODE_R2 = "R2";
            public const string KEYCODE_LEFT_TRIGGER = "Left Trigger";
            public const string KEYCODE_RIGHT_TRIGGER = "Right Trigger";

            public const string KEYCODE_CROSS = "Cross";
            public const string KEYCODE_CIRCLE = "Circle";
            public const string KEYCODE_SQUARE = "Square";
            public const string KEYCODE_TRIANGLE = "Triangle";
            public const string KEYCODE_A = "A";
            public const string KEYCODE_B = "B";
            public const string KEYCODE_X = "X";
            public const string KEYCODE_Y = "Y";

            public const string KEYCODE_L1 = "L1";
            public const string KEYCODE_R1 = "R1";
            public const string KEYCODE_LEFT_SHOULDER = "Left Shoulder";
            public const string KEYCODE_RIGHT_SHOULDER = "Right Shoulder";

            public const string KEYCODE_L3 = "L3";
            public const string KEYCODE_R3 = "R3";
            public const string KEYCODE_LEFT_STICK_BUTTON = "Left Stick Button";
            public const string KEYCODE_RIGHT_STICK_BUTTON = "Right Stick Button";

            public const string ELEMENT_LEFT_STICK = "Left Stick";
            public const string ELEMENT_RIGHT_STICK = "Right Stick";
            public const string ELEMENT_BUTTON = "Button";

            public const string CONTROLLER_NAME_DUAL = "Dual";
            public const string ACTION_PREFIX_UI = "UI";
            public const string CONTROLLER_TYPE_JOYSTICK = "Joystick";

            public const string NORMALIZE_SPACE = " ";
            public const string NORMALIZE_PLUS = "+";
            public const string NORMALIZE_MINUS = "-";

            public const string KEY_FORMAT = "{0}_{1}";
            public const string KEY_FORMAT_JOYSTICK = "Joystick_{0}";
        }

        private readonly Dictionary<string, string> _defaultKeyCodes = new();
        private readonly Dictionary<int, Dictionary<string, ActionElementMap>> _defaultJoystickElementMapByController = new();

        #region 매핑 유틸리티

        private bool ValidateInputPlayer()
        {
            if (InputPlayer == null)
            {
                Log.Warning(LogTags.Input, "InputPlayer가 할당되지 않았습니다.");
                return false;
            }
            return true;
        }

        private bool ValidateController(Controller controller)
        {
            if (controller == null)
            {
                Log.Warning(LogTags.Input, "컨트롤러가 null입니다.");
                return false;
            }
            return true;
        }

        private string NormalizeActionName(string actionDescriptiveName)
        {
            if (string.IsNullOrEmpty(actionDescriptiveName))
            {
                return string.Empty;
            }

            return actionDescriptiveName.Replace(InputConstants.NORMALIZE_SPACE, "")
                .Replace(InputConstants.NORMALIZE_PLUS, "")
                .Replace(InputConstants.NORMALIZE_MINUS, "");
        }

        private Controller.Button FindOptionsButton(Controller controller)
        {
            if (controller == null)
            {
                return null;
            }

            for (int i = 0; i < controller.Buttons.Count; i++)
            {
                Controller.Button button = controller.Buttons[i];
                if (button.elementIdentifier.key == InputConstants.BUTTON_KEY_OPTIONS)
                {
                    return button;
                }
            }

            return null;
        }

        private Controller.Button FindTouchPadButton(Controller controller)
        {
            if (controller == null)
            {
                return null;
            }

            for (int i = 0; i < controller.Buttons.Count; i++)
            {
                Controller.Button button = controller.Buttons[i];
                if (button.elementIdentifier.key == InputConstants.BUTTON_KEY_TOUCHPAD
                    || button.elementIdentifier.key == InputConstants.BUTTON_KEY_TOUCHPAD_ALT)
                {
                    return button;
                }
            }

            return null;
        }

        private ControllerMap[] GetControllerMapsForJoystick(Controller controller)
        {
            if (!ValidateInputPlayer() || !ValidateController(controller))
            {
                return null;
            }

            ControllerMap[] controllerMaps = InputPlayer.controllers?.maps?.GetMaps(ControllerType.Joystick, controller.id)?.ToArray();
            if (controllerMaps == null || controllerMaps.Length == 0)
            {
                Log.Warning(LogTags.Input, "컨트롤러 맵을 찾을 수 없습니다.");
                return null;
            }

            return controllerMaps;
        }

        private void SendKeyChangedEvent(ControllerType controllerType, ActionNames actionName, Pole axisContribution, string keyCode)
        {
            GlobalEvent<ControllerType, ActionNames, Pole, string>.Send(
                GlobalEventType.GAME_INPUT_KEY_CHANGED,
                controllerType,
                actionName,
                axisContribution,
                keyCode
            );
        }

        private bool CheckNonChangeableKey(string keyCode)
        {
            if (string.IsNullOrEmpty(keyCode))
            {
                return true;
            }

            switch (keyCode)
            {
                case InputConstants.KEYCODE_SHARE:
                case InputConstants.KEYCODE_OPTIONS:
                case InputConstants.KEYCODE_TOUCHPAD_BUTTON:
                case InputConstants.KEYCODE_VIEW:
                case InputConstants.KEYCODE_MENU:
                case InputConstants.KEYCODE_GUIDE:
                    return false;
            }

            return true;
        }

        private string GetOtherJoystickKeyCode(string keyCode)
        {
            if (string.IsNullOrEmpty(keyCode))
            {
                return keyCode;
            }

            switch (keyCode)
            {
                case InputConstants.KEYCODE_L2:
                    return InputConstants.KEYCODE_LEFT_TRIGGER;

                case InputConstants.KEYCODE_R2:
                    return InputConstants.KEYCODE_RIGHT_TRIGGER;

                case InputConstants.KEYCODE_LEFT_TRIGGER:
                    return InputConstants.KEYCODE_L2;

                case InputConstants.KEYCODE_RIGHT_TRIGGER:
                    return InputConstants.KEYCODE_R2;

                case InputConstants.KEYCODE_CROSS:
                    return InputConstants.KEYCODE_A;

                case InputConstants.KEYCODE_CIRCLE:
                    return InputConstants.KEYCODE_B;

                case InputConstants.KEYCODE_SQUARE:
                    return InputConstants.KEYCODE_X;

                case InputConstants.KEYCODE_TRIANGLE:
                    return InputConstants.KEYCODE_Y;

                case InputConstants.KEYCODE_A:
                    return InputConstants.KEYCODE_CROSS;

                case InputConstants.KEYCODE_B:
                    return InputConstants.KEYCODE_CIRCLE;

                case InputConstants.KEYCODE_X:
                    return InputConstants.KEYCODE_SQUARE;

                case InputConstants.KEYCODE_Y:
                    return InputConstants.KEYCODE_TRIANGLE;

                case InputConstants.KEYCODE_L1:
                    return InputConstants.KEYCODE_LEFT_SHOULDER;

                case InputConstants.KEYCODE_R1:
                    return InputConstants.KEYCODE_RIGHT_SHOULDER;

                case InputConstants.KEYCODE_LEFT_SHOULDER:
                    return InputConstants.KEYCODE_L1;

                case InputConstants.KEYCODE_RIGHT_SHOULDER:
                    return InputConstants.KEYCODE_R1;

                case InputConstants.KEYCODE_L3:
                    return InputConstants.KEYCODE_LEFT_STICK_BUTTON;

                case InputConstants.KEYCODE_R3:
                    return InputConstants.KEYCODE_RIGHT_STICK_BUTTON;

                case InputConstants.KEYCODE_LEFT_STICK_BUTTON:
                    return InputConstants.KEYCODE_L3;

                case InputConstants.KEYCODE_RIGHT_STICK_BUTTON:
                    return InputConstants.KEYCODE_R3;

                default:
                    return keyCode;
            }
        }

        private bool IsStickElement(string elementIdentifierName)
        {
            if (string.IsNullOrEmpty(elementIdentifierName))
            {
                return false;
            }

            return (elementIdentifierName.Contains(InputConstants.ELEMENT_LEFT_STICK) || elementIdentifierName.Contains(InputConstants.ELEMENT_RIGHT_STICK))
                && !elementIdentifierName.Contains(InputConstants.ELEMENT_BUTTON);
        }

        private ActionNames ConvertToActionNames(string actionDescriptiveName)
        {
            if (string.IsNullOrEmpty(actionDescriptiveName))
            {
                return ActionNames.None;
            }

            string actionNameString = NormalizeActionName(actionDescriptiveName);
            return EnumEx.ConvertTo<ActionNames>(actionNameString);
        }

        private void SetupAndInitializeButton(ActionNames actionName)
        {
            TSInputButton button = GetButton(actionName);
            if (button != null)
            {
                button.SetupKeys();
                button.InitializeState();
            }
        }

        private ActionElementMap FindDefaultElementData(Dictionary<string, ActionElementMap> defaultMaps, string keyCode)
        {
            if (defaultMaps == null || string.IsNullOrEmpty(keyCode))
            {
                return null;
            }

            return defaultMaps.Values.FirstOrDefault(x => x != null && x.elementIdentifierName == keyCode);
        }

        private bool TryGetJoystickElementData(int controllerId, string keyCode, out ControllerElementType elementType, out int elementIdentifierId)
        {
            elementType = ControllerElementType.Button;
            elementIdentifierId = -1;

            if (!_defaultJoystickElementMapByController.TryGetValue(controllerId, out Dictionary<string, ActionElementMap> defaultMaps))
            {
                return false;
            }

            ActionElementMap defaultElementData = FindDefaultElementData(defaultMaps, keyCode);
            if (defaultElementData != null)
            {
                elementType = defaultElementData.elementType;
                elementIdentifierId = defaultElementData.elementIdentifierId;
                return true;
            }

            return false;
        }

        private bool ValidateReInputControllers()
        {
            if (ReInput.controllers == null || ReInput.controllers.Controllers == null)
            {
                Log.Warning(LogTags.Input, "ReInput 컨트롤러를 찾을 수 없습니다.");
                return false;
            }
            return true;
        }

        private void ProcessAllControllers(Action<ControllerType> processAction)
        {
            if (!ValidateReInputControllers())
            {
                return;
            }

            for (int i = 0; i < ReInput.controllers.Controllers.Count; i++)
            {
                Controller controller = ReInput.controllers.Controllers[i];
                if (controller == null || controller.type == ControllerType.Mouse)
                {
                    continue;
                }

                processAction(controller.type);
            }
        }

        #endregion 매핑 유틸리티

        #region 매핑 처리

        private void ProcessMappings(ControllerType controllerType, Action<ControllerType, ActionElementMap, ControllerMap> processAction)
        {
            if (!ValidateInputPlayer())
            {
                return;
            }

            if (processAction == null)
            {
                Log.Warning(LogTags.Input, "처리 액션이 null입니다.");
                return;
            }

            ControllerMap[] controllerMaps = InputPlayer.controllers?.maps?.GetAllMaps(controllerType)?.ToArray();
            if (controllerMaps == null || controllerMaps.Length == 0)
            {
                return;
            }

            for (int i = 0; i < controllerMaps.Length; i++)
            {
                ControllerMap controllerMap = controllerMaps[i];
                if (controllerMap == null)
                {
                    continue;
                }

                ActionElementMap[] elementMaps = controllerMap.ElementMaps?.ToArray();
                if (elementMaps == null || elementMaps.Length == 0)
                {
                    continue;
                }

                for (int j = 0; j < elementMaps.Length; j++)
                {
                    ActionElementMap aem = elementMaps[j];
                    if (aem == null)
                    {
                        continue;
                    }

                    processAction(controllerType, aem, controllerMap);
                }
            }
        }

        private void ProcessMappings(ControllerType controllerType, int controllerId, Action<ControllerType, ActionElementMap, ControllerMap> processAction)
        {
            if (!ValidateInputPlayer())
            {
                return;
            }

            if (processAction == null)
            {
                Log.Warning(LogTags.Input, "처리 액션이 null입니다.");
                return;
            }

            ControllerMap[] controllerMaps = InputPlayer.controllers?.maps?.GetMaps(controllerType, controllerId)?.ToArray();
            if (controllerMaps == null || controllerMaps.Length == 0)
            {
                return;
            }

            for (int i = 0; i < controllerMaps.Length; i++)
            {
                ControllerMap controllerMap = controllerMaps[i];
                if (controllerMap == null)
                {
                    continue;
                }

                ActionElementMap[] elementMaps = controllerMap.ElementMaps?.ToArray();
                if (elementMaps == null || elementMaps.Length == 0)
                {
                    continue;
                }

                for (int j = 0; j < elementMaps.Length; j++)
                {
                    ActionElementMap aem = elementMaps[j];
                    if (aem == null)
                    {
                        continue;
                    }

                    processAction(controllerType, aem, controllerMap);
                }
            }
        }

        #endregion 매핑 처리

        #region 액션 재매핑

        public void RemapAction(ControllerType controllerType, ActionNames actionName, string keyCode)
        {
            RemapActionInternal(controllerType, actionName, keyCode);
        }

        public void RemapAction(ControllerType controllerType, ActionNames actionName, Pole axisContribution, string keyCode)
        {
            RemapActionInternal(controllerType, actionName, keyCode, axisContribution);
        }

        private void RemapActionInternal(ControllerType controllerType, ActionNames actionName, string keyCode, Pole? axisContribution = null)
        {
            if (!ValidateInputPlayer())
            {
                return;
            }

            if (controllerType == ControllerType.Joystick)
            {
                RemapJoystickAction(actionName, keyCode, axisContribution);
            }
            else
            {
                RemapKeyboardAction(actionName, keyCode, axisContribution);
            }
        }

        private void RemapJoystickAction(ActionNames actionName, string keyCode, Pole? axisContribution = null)
        {
            if (!ValidateInputPlayer())
            {
                return;
            }

            if (CurrentJoystick == null)
            {
                Log.Warning(LogTags.Input, "조이스틱이 연결되지 않았습니다.");
                return;
            }

            if (string.IsNullOrEmpty(keyCode))
            {
                Log.Warning(LogTags.Input, "키코드가 유효하지 않습니다.");
                return;
            }

            int controllerId = CurrentJoystick.id;
            ControllerMap[] controllerMaps = InputPlayer.controllers?.maps?.GetAllMaps(ControllerType.Joystick)?.ToArray();
            if (controllerMaps == null || controllerMaps.Length == 0)
            {
                Log.Warning(LogTags.Input, "조이스틱 컨트롤러 맵을 찾을 수 없습니다.");
                return;
            }

            for (int i = 0; i < controllerMaps.Length; i++)
            {
                ControllerMap controllerMap = controllerMaps[i];
                if (controllerMap == null)
                {
                    continue;
                }

                List<ActionElementMap> elementMaps = controllerMap.ElementMapsWithAction(actionName.ToString())?.ToList();
                if (elementMaps == null || elementMaps.Count == 0)
                {
                    continue;
                }

                for (int j = 0; j < elementMaps.Count; j++)
                {
                    ActionElementMap aem = elementMaps[j];
                    if (aem == null)
                    {
                        continue;
                    }

                    if (axisContribution.HasValue && aem.axisContribution != axisContribution.Value)
                    {
                        continue;
                    }

                    if (!IsStickElement(aem.elementIdentifierName))
                    {
                        controllerMap.DeleteElementMap(aem.id);
                    }

                    int elementIdentifierId = -1;
                    ControllerElementType elementType = aem.elementType;
                    string tempKeyCode = keyCode;

                    if (controllerMap.controllerId != controllerId)
                    {
                        tempKeyCode = GetOtherJoystickKeyCode(keyCode);
                    }

                    if (tempKeyCode != string.Empty && tempKeyCode != null)
                    {
                        if (!TryGetJoystickElementData(controllerMap.controllerId, tempKeyCode, out ControllerElementType foundElementType, out int foundElementIdentifierId))
                        {
                            Log.Warning(LogTags.Input, "기본 조이스틱 맵을 찾을 수 없습니다. ControllerId: {0}", controllerMap.controllerId);
                            continue;
                        }

                        elementType = foundElementType;
                        elementIdentifierId = foundElementIdentifierId;
                    }

                    controllerMap.CreateElementMap(aem.actionId, aem.axisContribution, elementIdentifierId, elementType, aem.axisRange, false);

                    ActionNames buttonActionName = actionName;
                    if (actionName == ActionNames.MoveHorizontal)
                    {
                        if (axisContribution.HasValue)
                        {
                            buttonActionName = axisContribution == Pole.Positive ? ActionNames.MoveRight : ActionNames.MoveLeft;
                        }
                    }
                    else if (actionName == ActionNames.MoveVertical)
                    {
                        if (axisContribution.HasValue)
                        {
                            buttonActionName = axisContribution == Pole.Positive ? ActionNames.MoveUp : ActionNames.MoveDown;
                        }
                    }

                    if (controllerMap.controllerId == controllerId)
                    {
                        SetupAndInitializeButton(buttonActionName);

                        SendKeyChangedEvent(ControllerType.Joystick, actionName, axisContribution ?? aem.axisContribution, tempKeyCode);
                        SaveMappings();
                    }
                }
            }
        }

        private void RemapKeyboardAction(ActionNames actionName, string keyCode, Pole? axisContribution = null)
        {
            if (!ValidateInputPlayer())
            {
                return;
            }

            if (string.IsNullOrEmpty(keyCode))
            {
                Log.Warning(LogTags.Input, "키코드가 유효하지 않습니다.");
                return;
            }

            if (!Enum.TryParse(keyCode, out KeyCode code))
            {
                Log.Warning(LogTags.Input, "키코드 파싱 실패. {0}", keyCode);
                return;
            }

            string actionNameString = actionName.ToString();
            ControllerMap[] controllerMaps = InputPlayer.controllers?.maps?.GetAllMaps(ControllerType.Keyboard)?.ToArray();
            if (controllerMaps == null || controllerMaps.Length == 0)
            {
                Log.Warning(LogTags.Input, "키보드 컨트롤러 맵을 찾을 수 없습니다.");
                return;
            }

            for (int i = 0; i < controllerMaps.Length; i++)
            {
                ControllerMap controllerMap = controllerMaps[i];
                if (controllerMap == null)
                {
                    continue;
                }

                IEnumerable<ActionElementMap> aemList = controllerMap.ElementMapsWithAction(actionNameString);
                if (aemList == null)
                {
                    continue;
                }

                List<ActionElementMap> elementMaps = aemList.ToList();
                if (elementMaps.Count == 0)
                {
                    continue;
                }

                if (elementMaps.Count > 1)
                {
                    Log.Warning(LogTags.Input, "액션 {0}에 {1}개의 매핑이 있습니다. 첫 번째 매핑만 변경합니다.", actionName, elementMaps.Count);
                }

                ActionElementMap aem = elementMaps[0];
                if (aem == null)
                {
                    continue;
                }

                controllerMap.DeleteElementMap(aem.id);

                if (axisContribution.HasValue)
                {
                    controllerMap.CreateElementMap(aem.actionId, axisContribution.Value, code, ModifierKeyFlags.None);
                }
                else
                {
                    controllerMap.CreateElementMap(aem.actionId, aem.axisContribution, code, ModifierKeyFlags.None);
                }

                UpdateButtonKeys(actionName);
                SendKeyChangedEvent(ControllerType.Keyboard, actionName, axisContribution ?? aem.axisContribution, keyCode);
                SaveMappings();

                return;
            }
        }

        #endregion 액션 재매핑

        #region 기본 매핑 로드

        public void LoadDefaultMappings()
        {
            ProcessAllControllers(controllerType => ProcessMappings(controllerType, LoadDefaultMapping));
        }

        private void LoadDefaultMapping(ControllerType controllerType, ActionElementMap aem, ControllerMap controllerMap)
        {
            if (aem == null || controllerMap == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(aem.actionDescriptiveName))
            {
                Log.Warning(LogTags.Input, "액션 설명 이름이 유효하지 않습니다.");
                return;
            }

            ActionNames actionName = ConvertToActionNames(aem.actionDescriptiveName);

            if (actionName == ActionNames.None)
            {
                Log.Warning(LogTags.Input, "지원하지 않는 액션입니다. {0}", aem.actionDescriptiveName);
                return;
            }

            if (controllerType == ControllerType.Joystick)
            {
                if (!_defaultJoystickElementMapByController.ContainsKey(controllerMap.controllerId))
                {
                    _defaultJoystickElementMapByController[controllerMap.controllerId] = new();
                }

                string actionNameString = NormalizeActionName(aem.actionDescriptiveName);
                if (!_defaultJoystickElementMapByController[controllerMap.controllerId].ContainsKey(actionNameString))
                {
                    _defaultJoystickElementMapByController[controllerMap.controllerId][actionNameString] = new ActionElementMap(aem);
                }
            }
            else
            {
                if (!_defaultKeyCodes.ContainsKey(actionName.ToString()))
                {
                    Log.Info(LogTags.Input, "{0}의 기본 키코드: {1}", actionName, aem.keyCode);
                    _defaultKeyCodes[actionName.ToString()] = aem.keyCode.ToString();
                }
            }
        }

        private void EnsureRequiredActionMappings()
        {
            if (InputPlayer == null)
            {
                return;
            }

            ControllerMap[] controllerMaps = InputPlayer.controllers.maps.GetAllMaps(ControllerType.Joystick).ToArray();
            for (int i = 0; i < controllerMaps.Length; i++)
            {
                ControllerMap controllerMap = controllerMaps[i];
                TryEnsureJoystickAction(controllerMap, ActionNames.Skip);
            }
        }

        private void TryEnsureJoystickAction(ControllerMap controllerMap, ActionNames actionName)
        {
            if (controllerMap.ElementMapsWithAction(actionName.ToString()).Any())
            {
                return;
            }

            if (!_defaultJoystickElementMapByController.TryGetValue(controllerMap.controllerId, out Dictionary<string, ActionElementMap> defaults))
            {
                Log.Warning(LogTags.Input, "조이스틱 기본 맵 정보를 찾을 수 없습니다. ControllerId: {0}, Action: {1}", controllerMap.controllerId, actionName);
                return;
            }

            if (!defaults.TryGetValue(actionName.ToString(), out ActionElementMap defaultMap) || defaultMap == null)
            {
                Log.Warning(LogTags.Input, "액션의 기본 매핑이 없습니다. ControllerId: {0}, Action: {1}", controllerMap.controllerId, actionName);
                return;
            }

            controllerMap.CreateElementMap(defaultMap.actionId, defaultMap.axisContribution, defaultMap.elementIdentifierId, defaultMap.elementType, defaultMap.axisRange, false);
            Log.Info(LogTags.Input, "누락된 조이스틱 매핑을 기본값으로 추가했습니다. ControllerId: {0}, Action: {1}, Key: {2}", controllerMap.controllerId, actionName, defaultMap.elementIdentifierName);
        }

        #endregion 기본 매핑 로드

        #region 저장된 매핑 로드

        private void LoadMappings()
        {
            ProcessAllControllers(controllerType => ProcessMappings(controllerType, LoadMapping));
        }

        private void LoadMapping(ControllerType controllerType, ActionElementMap aem, ControllerMap controllerMap)
        {
            if (aem == null || controllerMap == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(aem.actionDescriptiveName))
            {
                Log.Warning(LogTags.Input, "액션 설명 이름이 유효하지 않습니다.");
                return;
            }

            string actionNameString = NormalizeActionName(aem.actionDescriptiveName);
            string key = string.Format(InputConstants.KEY_FORMAT, controllerType, actionNameString);

            if (GamePrefs.HasKey(key))
            {
                string savedKeyCode = GamePrefs.GetString(key);
                if (string.IsNullOrEmpty(savedKeyCode))
                {
                    return;
                }

                if (controllerType == ControllerType.Joystick)
                {
                    if (!IsStickElement(aem.elementIdentifierName))
                    {
                        controllerMap.DeleteElementMap(aem.id);
                    }

                    int elementIdentifierId = -1;
                    ControllerElementType elementType = aem.elementType;
                    string tempKeyCode = savedKeyCode;

                    if (!_defaultJoystickElementMapByController.TryGetValue(controllerMap.controllerId, out Dictionary<string, ActionElementMap> maps))
                    {
                        Log.Warning(LogTags.Input, "기본 조이스틱 맵을 찾을 수 없습니다. ControllerId: {0}", controllerMap.controllerId);
                        return;
                    }

                    if (!maps.Values.ToList().Exists(x => x.elementIdentifierName == tempKeyCode))
                    {
                        tempKeyCode = GetOtherJoystickKeyCode(savedKeyCode);
                    }

                    if (tempKeyCode != string.Empty && tempKeyCode != null)
                    {
                        if (TryGetJoystickElementData(controllerMap.controllerId, tempKeyCode, out ControllerElementType foundElementType, out int foundElementIdentifierId))
                        {
                            elementType = foundElementType;
                            elementIdentifierId = foundElementIdentifierId;
                        }
                        else
                        {
                            Log.Error(LogTags.Input, "기본 요소 데이터를 찾을 수 없습니다: {0}", tempKeyCode);
                        }
                    }

                    controllerMap.CreateElementMap(aem.actionId, aem.axisContribution, elementIdentifierId, elementType, aem.axisRange, false);
                }
                else
                {
                    if (Enum.TryParse(savedKeyCode, out KeyCode keyCode))
                    {
                        Log.Info(LogTags.Input, "{0}의 저장된 키코드: {1} → {2}", aem.actionDescriptiveName, aem.keyCode, keyCode);
                        controllerMap.DeleteElementMap(aem.id);
                        controllerMap.CreateElementMap(aem.actionId, aem.axisContribution, keyCode, ModifierKeyFlags.None);
                    }
                }
            }
        }

        #endregion 저장된 매핑 로드

        #region 기본 매핑 설정

        public void SetDefaultMappings()
        {
            if (CurrentControllerType == ControllerType.Mouse)
            {
                ProcessMappings(ControllerType.Keyboard, SetDefaultMapping);
            }
            else
            {
                ProcessMappings(CurrentControllerType, SetDefaultMapping);
            }

            SaveMappings();
        }

        public void SetDefaultMappings(ControllerType controllerType)
        {
            if (!ValidateInputPlayer())
            {
                return;
            }

            if (controllerType == ControllerType.Joystick)
            {
                ControllerMap[] controllerMaps = InputPlayer.controllers?.maps?.GetAllMaps(ControllerType.Joystick)?.ToArray();
                if (controllerMaps == null || controllerMaps.Length == 0)
                {
                    Log.Warning(LogTags.Input, "조이스틱 컨트롤러 맵을 찾을 수 없습니다.");
                    return;
                }

                for (int i = 0; i < controllerMaps.Length; i++)
                {
                    ControllerMap controllerMap = controllerMaps[i];
                    if (controllerMap == null)
                    {
                        continue;
                    }

                    controllerMap.ClearElementMaps();

                    if (!_defaultJoystickElementMapByController.TryGetValue(controllerMap.controllerId, out Dictionary<string, ActionElementMap> defaultJoystickElementMap))
                    {
                        Log.Warning(LogTags.Input, "기본 조이스틱 맵을 찾을 수 없습니다. ControllerId: {0}", controllerMap.controllerId);
                        continue;
                    }
                    foreach (var aem in defaultJoystickElementMap.Values)
                    {
                        DeleteJoystickMapping(aem.actionDescriptiveName);
                        controllerMap.CreateElementMap(aem.actionId, aem.axisContribution, aem.elementIdentifierId, aem.elementType, aem.axisRange, false);
                    }
                }

                Controller joystickPS = InputPlayer.controllers.Controllers.FirstOrDefault(x => x.name.Contains(InputConstants.CONTROLLER_NAME_DUAL));
                if (joystickPS != null)
                {
                    AddTouchPadMapping(joystickPS);
                    SwapOptionsMapping(joystickPS);
                    MapOptionsToSkip(joystickPS);
                }

                SetupButtonEvents();

                GlobalEvent<ControllerType>.Send(GlobalEventType.GAME_CONTROLLER_TYPE_CHANGED, CurrentControllerType);
            }
            else
            {
                ProcessMappings(controllerType, SetDefaultMapping);
                ProcessMappings(controllerType, SaveMapping);
            }
        }

        private void SetDefaultMapping(ControllerType controllerType, ActionElementMap aem, ControllerMap controllerMap)
        {
            if (aem == null || controllerMap == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(aem.actionDescriptiveName))
            {
                Log.Warning(LogTags.Input, "액션 설명 이름이 유효하지 않습니다.");
                return;
            }

            ActionNames actionName = ConvertToActionNames(aem.actionDescriptiveName);

            if (_defaultKeyCodes.ContainsKey(actionName.ToString()))
            {
                KeyCode keyCode = Enum.Parse<KeyCode>(_defaultKeyCodes[actionName.ToString()]);
                if (keyCode != aem.keyCode)
                {
                    controllerMap.DeleteElementMap(aem.id);
                    controllerMap.CreateElementMap(aem.actionId, aem.axisContribution, keyCode, ModifierKeyFlags.None, out ActionElementMap testAem);
                    Log.Info(LogTags.Input, "{0}의 기본 키코드: {1}", actionName, keyCode);
                    Log.Info(LogTags.Input, "ActionChanged: KeyCode: {0}, ActionName: {1}", keyCode, actionName);
                    Log.Info(LogTags.Input, "ActionChanged: PrevAction: aem.actionId: {0}, aem.elementIdentifierName: {1}, aem.elementIdentifierId: {2}, aem.actionDescriptiveName: {3}", aem.actionId, aem.elementIdentifierName, aem.elementIdentifierId, aem.actionDescriptiveName);
                    Log.Info(LogTags.Input, "ActionChanged: ChangedAction: testAem.actionId: {0}, testAem.elementIdentifierName: {1}, testAem.elementIdentifierId: {2}, testAem.actionDescriptiveName: {3}", testAem.actionId, testAem.elementIdentifierName, testAem.elementIdentifierId, testAem.actionDescriptiveName);

                    SetupAndInitializeButton(actionName);
                    GlobalEvent<ControllerType, ActionNames, Pole, string>.Send(GlobalEventType.GAME_INPUT_KEY_CHANGED,
                        controllerType, actionName, aem.axisContribution, keyCode.ToString());
                }
            }
            else
            {
                Log.Error(LogTags.Input, "액션을 찾을 수 없습니다: {0}", actionName);
            }
        }

        #endregion 기본 매핑 설정

        #region 매핑 저장

        private void SaveMappings()
        {
            if (CurrentControllerType == ControllerType.Mouse)
            {
                ProcessMappings(ControllerType.Keyboard, SaveMapping);
            }
            else
            {
                ProcessMappings(CurrentControllerType, SaveMapping);
            }
        }

        private void SaveMapping(ControllerType controllerType, ActionElementMap aem, ControllerMap controllerMap)
        {
            if (aem == null || controllerMap == null)
            {
                return;
            }

            if (controllerType == ControllerType.Joystick)
            {
                if (string.IsNullOrEmpty(aem.actionDescriptiveName))
                {
                    return;
                }

                string actionNameString = NormalizeActionName(aem.actionDescriptiveName);

                if (!_defaultJoystickElementMapByController.TryGetValue(controllerMap.controllerId, out Dictionary<string, ActionElementMap> defaultMaps))
                {
                    return;
                }

                if (!defaultMaps.TryGetValue(actionNameString, out ActionElementMap defaultMap))
                {
                    return;
                }

                if (EnumEx.ConvertTo<ActionNames>(actionNameString) == ActionNames.None || defaultMap.elementIdentifierName == aem.elementIdentifierName)
                {
                    return;
                }

                string key = string.Format(InputConstants.KEY_FORMAT, controllerType, actionNameString);
                SaveJoystickKeyMapping(key, aem);
            }
            else
            {
                string key = string.Format(InputConstants.KEY_FORMAT, controllerType, aem.actionDescriptiveName);

                GamePrefs.SetString(key, aem.keyCode.ToString());
            }
        }

        private void SaveJoystickKeyMapping(string key, ActionElementMap aem)
        {
            if (aem == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if ((!string.IsNullOrEmpty(aem.elementIdentifierName) && IsStickElement(aem.elementIdentifierName))

            || !CheckNonChangeableKey(aem.elementIdentifierName)
            )
            {
                return;
            }

            if (key.Contains(InputConstants.ACTION_PREFIX_UI))
            {
                if (GamePrefs.HasKey(key))
                {
                    GamePrefs.Delete(key);
                }

                return;
            }

            string originValue = GamePrefs.GetString(key);

            if (originValue != aem.elementIdentifierId.ToString())
            {
                GamePrefs.SetString(key, aem.elementIdentifierName);
            }
        }

        private void DeleteJoystickMapping(string actionName)
        {
            string actionNameString = NormalizeActionName(actionName);
            string key = string.Format(InputConstants.KEY_FORMAT_JOYSTICK, actionNameString);

            if (GamePrefs.HasKey(key))
            {
                GamePrefs.Delete(key);
            }
        }

        #endregion 매핑 저장

        #region PlayStation 특수 매핑

        private void RemoveActionMappingFromOptionsButton(ControllerMap controllerMap, Controller.Button optionsButton, int actionId)
        {
            if (controllerMap == null || optionsButton == null)
            {
                return;
            }

            ActionElementMap[] buttonMaps = controllerMap.GetButtonMaps();
            if (buttonMaps == null)
            {
                return;
            }

            for (int i = 0; i < buttonMaps.Length; i++)
            {
                ActionElementMap buttonMap = buttonMaps[i];
                if (buttonMap != null &&
                    buttonMap.elementIdentifierId == optionsButton.elementIdentifier.id &&
                    buttonMap.actionId == actionId)
                {
                    controllerMap.DeleteElementMap(buttonMap.id);
                    break;
                }
            }
        }

        public void AddTouchPadMapping(Controller controller)
        {
            if (!ValidateController(controller) || !ValidateInputPlayer())
            {
                return;
            }

            const ActionNames actionName = ActionNames.PopupInventory;
            const int actionId = RewiredConsts.Action.PopupInventory;

            Controller.Button touchpadButtonPS = FindTouchPadButton(controller);
            if (touchpadButtonPS == null)
            {
                Log.Warning(LogTags.Input, "터치패드 버튼을 찾을 수 없습니다.");
                return;
            }

            ControllerMap[] controllerMaps = GetControllerMapsForJoystick(controller);
            if (controllerMaps == null || controllerMaps.Length == 0 || controllerMaps[0] == null)
            {
                return;
            }

            ControllerMap controllerMap = controllerMaps[0];
            Controller.Button optionsButtonPS = FindOptionsButton(controller);

            // GetButtonMaps()를 한 번만 호출하여 캐싱
            ActionElementMap[] buttonMaps = controllerMap.GetButtonMaps();
            if (buttonMaps == null)
            {
                return;
            }

            // 터치패드가 이미 PopupInventory로 매핑되어 있는지 확인
            ActionElementMap existingTouchPadMap = buttonMaps.FirstOrDefault(x =>
                x != null &&
                x.elementIdentifierId == touchpadButtonPS.elementIdentifier.id &&
                x.actionId == actionId);

            if (existingTouchPadMap != null)
            {
                // Options 버튼에서 PopupInventory 매핑만 정리하고 종료
                RemoveActionMappingFromOptionsButton(controllerMap, optionsButtonPS, actionId);
                Log.Info(LogTags.Input, "터치패드가 이미 PopupInventory로 매핑되어 있습니다. 스킵합니다.");
                return;
            }

            // Options 버튼에서 PopupInventory 매핑 제거
            RemoveActionMappingFromOptionsButton(controllerMap, optionsButtonPS, actionId);

            // 터치패드에 PopupInventory 매핑 생성
            controllerMap.CreateElementMap(actionId, Pole.Positive, touchpadButtonPS.elementIdentifier.id, touchpadButtonPS.type, AxisRange.Positive, false);
            SetupAndInitializeButton(actionName);
        }

        public void SwapOptionsMapping(Controller controller)
        {
            if (!ValidateController(controller) || !ValidateInputPlayer())
            {
                return;
            }

            const ActionNames actionName = ActionNames.PopupPause;
            const int actionId = RewiredConsts.Action.PopupPause;

            Controller.Button optionsButton = FindOptionsButton(controller);
            if (optionsButton == null)
            {
                Log.Warning(LogTags.Input, "Options 버튼을 찾을 수 없습니다.");
                return;
            }

            ControllerMap[] controllerMaps = GetControllerMapsForJoystick(controller);
            if (controllerMaps == null || controllerMaps.Length == 0 || controllerMaps[0] == null)
            {
                return;
            }

            ControllerMap controllerMap = controllerMaps[0];

            // GetButtonMaps()를 한 번만 호출하여 캐싱
            ActionElementMap[] buttonMaps = controllerMap.GetButtonMaps();
            if (buttonMaps == null)
            {
                return;
            }

            // 이미 PopupPause로 매핑되어 있는지 확인
            ActionElementMap existingCorrectMap = buttonMaps.FirstOrDefault(x =>
                x != null &&
                x.elementIdentifierId == optionsButton.elementIdentifier.id &&
                x.actionId == actionId);

            if (existingCorrectMap != null)
            {
                Log.Info(LogTags.Input, "Options 버튼이 이미 PopupPause로 매핑되어 있습니다. 스킵합니다.");
                return;
            }

            // 다른 액션에 매핑된 경우만 삭제 후 재매핑
            ActionElementMap existingMap = buttonMaps.FirstOrDefault(x =>
                x != null &&
                x.elementIdentifierId == optionsButton.elementIdentifier.id);

            if (existingMap != null)
            {
                controllerMap.DeleteElementMap(existingMap.id);
            }

            controllerMap.CreateElementMap(actionId, Pole.Positive, optionsButton.elementIdentifier.id, optionsButton.type, AxisRange.Positive, false);
            SetupAndInitializeButton(actionName);
        }

        public void MapOptionsToSkip(Controller controller)
        {
            if (!ValidateController(controller) || !ValidateInputPlayer())
            {
                return;
            }

            Controller.Button optionsButton = FindOptionsButton(controller);
            if (optionsButton == null)
            {
                Log.Warning(LogTags.Input, "Skip 매핑을 위한 Options 버튼을 찾을 수 없습니다.");
                return;
            }

            ControllerMap[] controllerMaps = GetControllerMapsForJoystick(controller);
            if (controllerMaps == null)
            {
                return;
            }

            const ActionNames skipActionName = ActionNames.Skip;
            const int skipActionId = RewiredConsts.Action.Skip;

            if (controllerMaps.Length > 0 && controllerMaps[0] != null)
            {
                var existingSkipMap = controllerMaps[0].GetButtonMaps()?.FirstOrDefault(x =>
                    x != null && x.actionId == skipActionId && x.elementIdentifierId == optionsButton.elementIdentifier.id);

                if (existingSkipMap == null)
                {
                    controllerMaps[0].CreateElementMap(skipActionId, Pole.Positive, optionsButton.elementIdentifier.id, optionsButton.type, AxisRange.Positive, false);

                    SetupAndInitializeButton(skipActionName);
                }
            }
        }

        #endregion PlayStation 특수 매핑
    }
}