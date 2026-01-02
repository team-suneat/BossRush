using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine;

namespace TeamSuneat
{
    public partial class TSInputManager
    {
        // Remap
        private readonly Dictionary<string, string> _defaultKeyCodes = new();

        // 컨트롤러 ID별로 기본 매핑된 ActionElementMap을 저장합니다.
        private readonly Dictionary<int, Dictionary<string, ActionElementMap>> _defaultJoystickElementMapByController = new();

        /// <summary>
        /// 입력 액션을 내부적으로 재매핑합니다.
        /// </summary>
        /// <param name="controllerType">컨트롤러 타입 (예: 키보드, 조이스틱)</param>
        /// <param name="actionName">입력 액션 이름</param>
        /// <param name="keyCode">키 코드</param>
        /// <param name="axisContribution">축 방향 (옵션)</param>
        private void RemapActionInternal(ControllerType controllerType, ActionNames actionName, string keyCode, Pole? axisContribution = null)
        {
            // InputPlayer가 할당되지 않은 상태에서 매핑을 시도할 경우 경고를 출력합니다.
            if (InputPlayer == null)
            {
                Log.Warning(LogTags.Input, "InputPlayer가 할당되지 않았습니다.");
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

        /// <summary>
        /// 키 변경 이벤트를 전송합니다.
        /// </summary>
        /// <param name="controllerType">컨트롤러 타입</param>
        /// <param name="actionName">액션 이름</param>
        /// <param name="axisContribution">축 방향</param>
        /// <param name="keyCode">키 코드</param>
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

        /// <summary>
        /// 조이스틱 입력 액션을 재매핑합니다.
        /// </summary>
        /// <param name="actionName">입력 액션 이름</param>
        /// <param name="keyCode">키 코드</param>
        /// <param name="axisContribution">축 방향 (옵션)</param>
        private void RemapJoystickAction(ActionNames actionName, string keyCode, Pole? axisContribution = null)
        {
            // 컨트롤러 ID별로 기본 매핑된 ActionElementMap을 저장합니다.
            int controllerId = CurrentJoystick.id;
            ControllerMap[] controllerMaps = InputPlayer.controllers.maps.GetAllMaps(ControllerType.Joystick).ToArray();

            for (int i = 0; i < controllerMaps.Length; i++)
            {
                ControllerMap controllerMap = controllerMaps[i];

                // 액션 이름에 해당하는 요소들을 가져옵니다.
                List<ActionElementMap> elementMaps = controllerMap.ElementMapsWithAction(actionName.ToString()).ToList();

                for (int j = 0; j < elementMaps.Count; j++)
                {
                    ActionElementMap aem = elementMaps[j];

                    // 축이 다른 경우 매핑을 수정하지 않습니다.
                    if (axisContribution.HasValue && aem.axisContribution != axisContribution.Value)
                    {
                        continue;
                    }

                    // 아날로그 스틱의 이동 기능은 변경 불가이므로 기존 매핑을 삭제하지 않습니다.
                    if (!((aem.elementIdentifierName.Contains("Left Stick") || aem.elementIdentifierName.Contains("Right Stick")) && !aem.elementIdentifierName.Contains("Button")))
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
                        Dictionary<string, ActionElementMap> defaultJoystickElementMapByController = _defaultJoystickElementMapByController[controllerMap.controllerId];
                        Dictionary<string, ActionElementMap>.ValueCollection values = defaultJoystickElementMapByController.Values;
                        ActionElementMap defaultElementData = null;
                        foreach (ActionElementMap value in values)
                        {
                            if (value.elementIdentifierName == tempKeyCode)
                            {
                                defaultElementData = value;
                                break;
                            }
                        }

                        if (defaultElementData != null)
                        {
                            elementType = defaultElementData.elementType;
                            elementIdentifierId = defaultElementData.elementIdentifierId;
                        }
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
                        TSInputButton button = GetButton(buttonActionName);

                        if (button != null)
                        {
                            button.SetupKeys();
                            button.InitializeState();

                            SendKeyChangedEvent(ControllerType.Joystick, actionName, axisContribution ?? aem.axisContribution, tempKeyCode);
                            SaveMappings();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 키보드 입력 액션을 재매핑합니다.
        /// </summary>
        /// <param name="actionName">입력 액션 이름</param>
        /// <param name="keyCode">키 코드</param>
        /// <param name="axisContribution">축 방향 (옵션)</param>
        private void RemapKeyboardAction(ActionNames actionName, string keyCode, Pole? axisContribution = null)
        {
            if (!Enum.TryParse(keyCode, out KeyCode code))
            {
                Log.Warning("키코드 파싱 실패. {0}", keyCode);
                return;
            }

            string actionNameString = actionName.ToString();
            ControllerMap[] controllerMaps = InputPlayer.controllers.maps.GetAllMaps(ControllerType.Keyboard).ToArray();

            for (int i = 0; i < controllerMaps.Length; i++)
            {
                ControllerMap controllerMap = controllerMaps[i];

                // 액션 이름에 해당하는 요소들을 가져옵니다.
                IEnumerable<ActionElementMap> aemList = controllerMap.ElementMapsWithAction(actionNameString);
                if (aemList == null)
                {
                    continue;
                }
                List<ActionElementMap> elementMaps = aemList.ToList();

                for (int j = 0; j < elementMaps.Count; j++)
                {
                    ActionElementMap aem = elementMaps[j];

                    controllerMap.DeleteElementMap(aem.id);

                    // 지원하지 않는 ActionNames이거나 axisContribution 인자가 없을 경우 기본 매핑을 저장합니다.
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

                    // 첫 번째 요소를 처리했으므로 종료합니다.
                    return;
                }
            }
        }

        public void RemapAction(ControllerType controllerType, ActionNames actionName, string keyCode)
        {
            RemapActionInternal(controllerType, actionName, keyCode);
        }

        public void RemapAction(ControllerType controllerType, ActionNames actionName, Pole axisContribution, string keyCode)
        {
            RemapActionInternal(controllerType, actionName, keyCode, axisContribution);
        }

        //

        /// <summary> 컨트롤러 타입별로 모든 매핑을 처리 </summary>
        /// <param name="controllerType">컨트롤러 타입</param>
        /// <param name="processAction">매핑 처리 메서드</param>
        private void ProcessMappings(ControllerType controllerType, Action<ControllerType, ActionElementMap, ControllerMap> processAction)
        {
            ControllerMap[] controllerMaps = InputPlayer.controllers.maps.GetAllMaps(controllerType).ToArray();
            for (int i = 0; i < controllerMaps.Length; i++)
            {
                ControllerMap controllerMap = controllerMaps[i];
                ActionElementMap[] elementMaps = controllerMap.ElementMaps.ToArray();
                for (int j = 0; j < elementMaps.Length; j++)
                {
                    ActionElementMap aem = elementMaps[j];
                    processAction(controllerType, aem, controllerMap);
                }
            }
        }

        /// <summary> controllerId에 해당하는 컨트롤러의 ElementMap만 매핑 진행 </summary>
        private void ProcessMappings(ControllerType controllerType, int controllerId, Action<ControllerType, ActionElementMap, ControllerMap> processAction)
        {
            ControllerMap[] controllerMaps = InputPlayer.controllers?.maps?.GetMaps(controllerType, controllerId)?.ToArray();
            if (controllerMaps == null)
            {
                return;
            }
            for (int i = 0; i < controllerMaps.Length; i++)
            {
                ControllerMap controllerMap = controllerMaps[i];
                ActionElementMap[] elementMaps = controllerMap.ElementMaps.ToArray();
                for (int j = 0; j < elementMaps.Length; j++)
                {
                    ActionElementMap aem = elementMaps[j];
                    processAction(controllerType, aem, controllerMap);
                }
            }
        }

        /// <summary> 기본 매핑을 불러옵니다 </summary>
        public void LoadDefaultMappings()
        {
            for (int i = 0; i < ReInput.controllers.Controllers.Count; i++)
            {
                Controller controller = ReInput.controllers.Controllers[i];
                if (controller.type == ControllerType.Mouse)
                {
                    continue;
                }

                ProcessMappings(controller.type, LoadDefaultMapping);
            }
        }

        /// <summary> 기본 매핑을 저장합니다 </summary>
        private void LoadDefaultMapping(ControllerType controllerType, ActionElementMap aem, ControllerMap controllerMap)
        {
            string actionNameString = aem.actionDescriptiveName.Replace(" ", "")
                .Replace("+", "")
                .Replace("-", "");
            ActionNames actionName = EnumEx.ConvertTo<ActionNames>(actionNameString);

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

                if (!_defaultJoystickElementMapByController[controllerMap.controllerId].ContainsKey(actionName.ToString()))
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

        /// <summary> 저장된 매핑을 불러옵니다 </summary>
        private void LoadMappings()
        {
            for (int i = 0; i < ReInput.controllers.Controllers.Count; i++)
            {
                Controller controller = ReInput.controllers.Controllers[i];
                if (controller.type == ControllerType.Mouse)
                {
                    continue;
                }

                ProcessMappings(controller.type, LoadMapping);
            }
        }

        /// <summary>
        /// 신규 액션 추가 시 기존 저장 데이터에 누락된 조이스틱 매핑을 기본값으로만 보충합니다.
        /// 사용자 커스텀 매핑은 유지합니다.
        /// </summary>
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

        /// <summary>
        /// 특정 조이스틱 맵에 액션 매핑이 없으면 기본 맵 정보로만 추가합니다.
        /// </summary>
        private void TryEnsureJoystickAction(ControllerMap controllerMap, ActionNames actionName)
        {
            // 이미 매핑이 있으면 유지
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

        /// <summary>
        /// 저장된 매핑을 불러올 때 사용
        /// </summary>
        private void LoadMapping(ControllerType controllerType, ActionElementMap aem, ControllerMap controllerMap)
        {
            string actionNameString = aem.actionDescriptiveName.Replace(" ", "")
                .Replace("+", "")
                .Replace("-", "");

            string key = $"{controllerType}_{actionNameString}";

            if (GamePrefs.HasKey(key))
            {
                string savedKeyCode = GamePrefs.GetString(key);

                if (controllerType == ControllerType.Joystick)
                {
                    if (!((aem.elementIdentifierName.Contains("Left Stick") || aem.elementIdentifierName.Contains("Right Stick")) && !aem.elementIdentifierName.Contains("Button")))
                    {
                        controllerMap.DeleteElementMap(aem.id);
                    }

                    int elementIdentifierId = -1;
                    ControllerElementType elementType = aem.elementType;
                    string tempKeyCode = savedKeyCode;
                    Dictionary<string, ActionElementMap> maps = _defaultJoystickElementMapByController[controllerMap.controllerId];

                    if (!maps.Values.ToList().Exists(x => x.elementIdentifierName == tempKeyCode))
                    {
                        tempKeyCode = GetOtherJoystickKeyCode(savedKeyCode);
                    }

                    if (tempKeyCode != string.Empty && tempKeyCode != null)
                    {
                        ActionElementMap defaultElementData = maps.Values.FirstOrDefault(x => x.elementIdentifierName == tempKeyCode);
                        if (defaultElementData != null)
                        {
                            elementType = defaultElementData.elementType;
                            elementIdentifierId = defaultElementData.elementIdentifierId;
                        }
                        else
                        {
                            Log.Error("defaultElementData 를 찾을 수 없습니다: {0}", tempKeyCode);
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

        /// <summary> 조이스틱 매핑 삭제 </summary>
        private void DeleteJoystickMapping(string actionName)
        {
            string actionNameString = actionName.Replace(" ", "")
                .Replace("+", "")
                .Replace("-", "");

            string key = $"Joystick_{actionNameString}";

            if (GamePrefs.HasKey(key))
            {
                GamePrefs.Delete(key);
            }
        }

        /// <summary> 기본 매핑으로 되돌리기 </summary>
        /// <summary> 기본 매핑을 저장합니다 </summary>
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

        /// <summary> 기본 매핑을 저장합니다 </summary>
        public void SetDefaultMappings(ControllerType controllerType)
        {
            // 조이스틱 매핑의 경우 이동쪽 매핑을 수정하면 이동 관련 ElementMap이 남아있고 새로 생성되는 현상이 있어, 기존 ElementMap을 모두 삭제 후 DefaultMap으로 새로 생성합니다.
            if (controllerType == ControllerType.Joystick)
            {
                ControllerMap[] controllerMaps = InputPlayer.controllers.maps.GetAllMaps(ControllerType.Joystick).ToArray();
                for (int i = 0; i < controllerMaps.Length; i++)
                {
                    ControllerMap controllerMap = controllerMaps[i];
                    controllerMap.ClearElementMaps();

                    Dictionary<string, ActionElementMap> defaultJoystickElementMap = _defaultJoystickElementMapByController[controllerMap.controllerId];
                    foreach (var aem in defaultJoystickElementMap.Values)
                    {
                        DeleteJoystickMapping(aem.actionDescriptiveName);
                        controllerMap.CreateElementMap(aem.actionId, aem.axisContribution, aem.elementIdentifierId, aem.elementType, aem.axisRange, false);
                    }
                }
                // 플스 패드의 경우 터치패드 버튼이 따로 매핑되어 있지 않기 때문에 예외처리합니다.
                Controller joystickPS = InputPlayer.controllers.Controllers.FirstOrDefault(x => x.name.Contains("Dual"));
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

        /// <summary> 기본 매핑을 저장합니다 </summary>
        private void SetDefaultMapping(ControllerType controllerType, ActionElementMap aem, ControllerMap controllerMap)
        {
            string actionNameString;

            actionNameString = aem.actionDescriptiveName;
            actionNameString = actionNameString.Replace(" ", "");
            actionNameString = actionNameString.Replace("+", "");
            actionNameString = actionNameString.Replace("-", "");

            ActionNames actionName = EnumEx.ConvertTo<ActionNames>(actionNameString);

            if (_defaultKeyCodes.ContainsKey(actionName.ToString()))
            {
                KeyCode keyCode = Enum.Parse<KeyCode>(_defaultKeyCodes[actionName.ToString()]);
                if (keyCode != aem.keyCode)
                {
                    controllerMap.DeleteElementMap(aem.id);
                    controllerMap.CreateElementMap(aem.actionId, aem.axisContribution, keyCode, ModifierKeyFlags.None, out ActionElementMap testAem);
                    Log.Info(LogTags.Input, "{0}의 기본 키코드: {1}", actionName, keyCode);

                    Log.Info(LogTags.Input, $"ActionChanged: KeyCode: {keyCode}, ActionName: {actionName}");
                    Log.Info(LogTags.Input, $"ActionChanged: PrevAction: aem.actionId: {aem.actionId}, aem.elementIdentifierName: {aem.elementIdentifierName}, aem.elementIdentifierId: {aem.elementIdentifierId}, aem.actionDescriptiveName: {aem.actionDescriptiveName}");
                    Log.Info(LogTags.Input, $"ActionChanged: ChangedAction: testAem.actionId: {testAem.actionId}, testAem.elementIdentifierName: {testAem.elementIdentifierName}, testAem.elementIdentifierId: {testAem.elementIdentifierId}, testAem.actionDescriptiveName: {testAem.actionDescriptiveName}");

                    TSInputButton button = GetButton(actionName);
                    if (button != null)
                    {
                        button.SetupKeys();
                        button.InitializeState();
                        GlobalEvent<ControllerType, ActionNames, Pole, string>.Send(GlobalEventType.GAME_INPUT_KEY_CHANGED,
                            controllerType, actionName, aem.axisContribution, keyCode.ToString());
                    }
                }
            }
            else
            {
                Log.Error("actionNameString에 Action을 찾을 수 없습니다. {0}", actionNameString);
            }
        }

        /// <summary>
        /// 컨트롤러 타입에 따른 전체 매핑을 저장합니다
        /// </summary>
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

        /// <summary>
        /// 매핑을 저장합니다
        /// </summary>

        private void SaveMapping(ControllerType controllerType, ActionElementMap aem, ControllerMap controllerMap)
        {
            if (controllerType == ControllerType.Joystick)
            {
                string actionNameString = aem.actionDescriptiveName.Replace(" ", "")
                    .Replace("+", "")
                    .Replace("-", "");

                //지원하지 않는 ActionNames이거나 기본 매핑과 동일한 경우 저장하지 않습니다.
                if (EnumEx.ConvertTo<ActionNames>(actionNameString) == ActionNames.None || _defaultJoystickElementMapByController[controllerMap.controllerId][actionNameString].elementIdentifierName == aem.elementIdentifierName)

                {
                    return;
                }

                string key = $"{controllerType}_{actionNameString}";
                SaveJoystickKeyMapping(key, aem);
            }
            else
            {
                string key = $"{controllerType}_{aem.actionDescriptiveName}";

                GamePrefs.SetString(key, aem.keyCode.ToString());
            }
        }

        private void SaveJoystickKeyMapping(string key, ActionElementMap aem)
        {
            if ((!string.IsNullOrEmpty(aem.elementIdentifierName) && (aem.elementIdentifierName.Contains("Left Stick") || aem.elementIdentifierName.Contains("Right Stick")) && !aem.elementIdentifierName.Contains("Button"))

            || !CheckNonChangeableKey(aem.elementIdentifierName)
            )
            {
                return;
            }

            //UI 조작 요소는 저장하지 않으며, 관련 GamePrefs가 있을 경우 삭제합니다.
            if (key.Contains("UI"))
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

        //플스 패드용 터치패드 버튼 예외처리입니다. Rewired의 게임패드 템플릿에서는 터치패드 항목이 없어 별도 예외처리합니다.
        public void AddTouchPadMapping(Controller controller)
        {
            const ActionNames actionName = ActionNames.PopupInventory;
            const int actionId = RewiredConsts.Action.PopupInventory;
            Debug.Log($"Touchpad action: {actionId}: {actionName}");

            Controller.Button touchpadButtonPS = null;
            for (int i = 0; i < controller.Buttons.Count; i++)
            {
                Controller.Button buttonPS = controller.Buttons[i];
                if (buttonPS.elementIdentifier.key == "touchpad/button"
                    || buttonPS.elementIdentifier.key == "touchpad_button")
                {
                    touchpadButtonPS = buttonPS;
                    break;
                }
            }

            if (touchpadButtonPS == null)
            {
                Debug.LogWarning($"Touchpad Button == null");
                return;
            }

            ControllerMap[] controllerMaps = InputPlayer.controllers.maps.GetMaps(ControllerType.Joystick, controller.id).ToArray();
            if (controllerMaps.Length == 0)
            {
                Debug.LogWarning($"controllerMaps.Length == 0");
                return;
            }

            // Delete only the options button mapped to the PopupInventory action
            Controller.Button optionsButtonPS = null;
            for (int i = 0; i < controller.Buttons.Count; i++)
            {
                Controller.Button buttonPS = controller.Buttons[i];
                if (buttonPS.elementIdentifier.key == "options")
                {
                    optionsButtonPS = buttonPS;
                    break;
                }
            }

            if (optionsButtonPS != null)
            {
                ActionElementMap[] buttonMaps = controllerMaps[0].GetButtonMaps();
                for (int i = 0; i < buttonMaps.Length; i++)
                {
                    ActionElementMap buttonMap = buttonMaps[i];
                    if (buttonMap.elementIdentifierId == optionsButtonPS.elementIdentifier.id
                        && buttonMap.actionId == actionId)
                    {
                        controllerMaps[0].DeleteElementMap(buttonMap.id);
                        break;
                    }
                }
            }

            // Map the touchpad button to PopupInventory
            controllerMaps[0].CreateElementMap(actionId, Pole.Positive, touchpadButtonPS.elementIdentifier.id, touchpadButtonPS.type, AxisRange.Positive, false);

            TSInputButton button = GetButton(actionName);
            if (button == null)
            {
                Debug.Log($"GetButton({actionName}) == null");
                return;
            }

            button.SetupKeys();
            button.InitializeState();
        }

        // Viridian: Options button (right of touchpad) should pause the game. Touchpad already opens inventory.
        public void SwapOptionsMapping(Controller controller)
        {
            const ActionNames actionName = ActionNames.PopupPause;
            const int actionId = RewiredConsts.Action.PopupPause;
            Debug.Log($"Options action: {actionId}: {actionName}");

            Controller.Button optionsButton = null;
            for (int i = 0; i < controller.Buttons.Count; i++)
            {
                Controller.Button buttonPS = controller.Buttons[i];
                if (buttonPS.elementIdentifier.key == "options")
                {
                    optionsButton = buttonPS;
                    break;
                }
            }
            if (optionsButton == null)
            {
                Debug.Log($"Options Button == null");
                return;
            }

            ControllerMap[] controllerMaps = InputPlayer.controllers.maps.GetMaps(ControllerType.Joystick, controller.id).ToArray();
            if (controllerMaps.Length == 0)
            {
                Debug.Log($"controllerMaps.Length == 0");
                return;
            }

            var optionsInventoryMap = controllerMaps[0].GetButtonMaps().FirstOrDefault(x => x.elementIdentifierId == optionsButton.elementIdentifier.id);
            if (optionsInventoryMap != null)
            {
                Debug.Log($"Deleting existing Options button mapping. ActionID: {optionsInventoryMap.actionId}, ElementID: {optionsInventoryMap.elementIdentifierId}, ElementName: {optionsInventoryMap.elementIdentifierName}");
                controllerMaps[0].DeleteElementMap(optionsInventoryMap.id);
            }

            // PopupPause 액션에 Options 버튼 매핑
            controllerMaps[0].CreateElementMap(actionId, Pole.Positive, optionsButton.elementIdentifier.id, optionsButton.type, AxisRange.Positive, false);

            TSInputButton button = GetButton(actionName);
            if (button == null)
            {
                Debug.Log($"GetButton({actionName}) == null");
                return;
            }

            button.SetupKeys();
            button.InitializeState();
        }

        // 새 함수 추가: Skip 액션 전용
        public void MapOptionsToSkip(Controller controller)
        {
            Controller.Button optionsButton = null;
            for (int i = 0; i < controller.Buttons.Count; i++)
            {
                Controller.Button buttonPS = controller.Buttons[i];
                if (buttonPS.elementIdentifier.key == "options")
                {
                    optionsButton = buttonPS;
                    break;
                }
            }
            if (optionsButton == null)
            {
                Debug.Log($"Options Button == null for Skip mapping");
                return;
            }

            ControllerMap[] controllerMaps = InputPlayer.controllers.maps.GetMaps(ControllerType.Joystick, controller.id).ToArray();
            if (controllerMaps.Length == 0)
            {
                Debug.Log($"controllerMaps.Length == 0 for Skip mapping");
                return;
            }

            const ActionNames skipActionName = ActionNames.Skip;
            const int skipActionId = RewiredConsts.Action.Skip;

            // Skip 액션에 이미 Options 버튼이 매핑되어 있는지 확인
            var existingSkipMap = controllerMaps[0].GetButtonMaps().FirstOrDefault(x =>
                x.actionId == skipActionId && x.elementIdentifierId == optionsButton.elementIdentifier.id);

            if (existingSkipMap == null)
            {
                controllerMaps[0].CreateElementMap(skipActionId, Pole.Positive, optionsButton.elementIdentifier.id, optionsButton.type, AxisRange.Positive, false);
                Debug.Log($"Mapped Options button to Skip action. ActionID: {skipActionId}, ElementID: {optionsButton.elementIdentifier.id}");

                TSInputButton skipButton = GetButton(skipActionName);
                if (skipButton != null)
                {
                    skipButton.SetupKeys();
                    skipButton.InitializeState();
                }
                else
                {
                    Debug.LogWarning($"GetButton({skipActionName}) == null");
                }
            }
        }

        // 패드들의 중앙 버튼들은 변경할 수 없는 키값들이며 해당 입력 시 false를 리턴하는 함수입니다.
        private bool CheckNonChangeableKey(string keyCode)
        {
            switch (keyCode)
            {
                case "Share":
                case "Options":
                case "Touchpad Button":
                case "View":
                case "Menu":
                case "Guide":
                    return false;
            }

            return true;
        }

        //엑박 패드 및 플스 패드의 버튼 키값을 서로 변환하는 함수입니다.
        private string GetOtherJoystickKeyCode(string keyCode)
        {
            switch (keyCode)
            {
                case "L2":
                    return "Left Trigger";

                case "R2":
                    return "Right Trigger";

                case "Left Trigger":
                    return "L2";

                case "Right Trigger":
                    return "R2";

                case "Cross":
                    return "A";

                case "Circle":
                    return "B";

                case "Square":
                    return "X";

                case "Triangle":
                    return "Y";

                case "A":
                    return "Cross";

                case "B":
                    return "Circle";

                case "X":
                    return "Square";

                case "Y":
                    return "Triangle";

                case "L1":
                    return "Left Shoulder";

                case "R1":
                    return "Right Shoulder";

                case "Left Shoulder":
                    return "L1";

                case "Right Shoulder":
                    return "R1";

                case "L3":
                    return "Left Stick Button";

                case "R3":
                    return "Right Stick Button";

                case "Left Stick Button":
                    return "L3";

                case "Right Stick Button":
                    return "R3";

                default:
                    return keyCode;
            }
        }
    }
}