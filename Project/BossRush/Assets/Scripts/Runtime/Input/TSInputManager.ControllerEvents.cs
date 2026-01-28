using Rewired;
using System.Collections.Generic;

namespace TeamSuneat
{
    public partial class TSInputManager
    {
        #region 이벤트 구독/해제

        public void SubscribeEvents()
        {
            if (!IsInitialized || InputPlayer == null)
            {
                return;
            }

            InputPlayer.controllers.ControllerAddedEvent += OnControllerAdded;
            InputPlayer.controllers.ControllerRemovedEvent += OnControllerRemoved;
            InputPlayer.controllers.AddLastActiveControllerChangedDelegate(OnCurrentJoystickChanged);
        }

        public void UnsubscribeEvents()
        {
            if (InputPlayer == null)
            {
                return;
            }

            InputPlayer.controllers.ControllerAddedEvent -= OnControllerAdded;
            InputPlayer.controllers.ControllerRemovedEvent -= OnControllerRemoved;
            InputPlayer.controllers.RemoveLastActiveControllerChangedDelegate(OnCurrentJoystickChanged);
        }

        #endregion 이벤트 구독/해제

        #region 컨트롤러 이벤트 핸들러

        private void OnControllerAdded(ControllerAssignmentChangedEventArgs args)
        {
            if (args == null)
            {
                Log.Error("OnControllerAdded 실패. args가 null입니다.");
                return;
            }
            OnControllerAdded(args.controller);
        }

        private void OnControllerAdded(Rewired.Controller controller)
        {
            if (controller == null)
            {
                Log.Error("OnControllerAdded 실패. controller가 null입니다.");
                return;
            }

            Log.Info(LogTags.Input, "컨트롤러가 추가되었습니다. 입력 정보를 갱신합니다. Type:{2}, Name:{0}(ID:{1})",
                controller.name, controller.identifier.controllerId, controller.type);

            if (controller.type == ControllerType.Joystick && CurrentJoystick == null)
            {
                CurrentJoystick = controller;
            }

            ProcessMappings(controller.type, controller.id, LoadDefaultMapping);
            ProcessMappings(controller.type, controller.id, LoadMapping);
            SetupButtonEvents();

            if (CheckPSJoystick(controller.name))
            {
                AddTouchPadMapping(controller);
                SwapOptionsMapping(controller);
                MapOptionsToSkip(controller);
            }

            _ = GlobalEvent<ControllerType>.Send(GlobalEventType.GAME_CONTROLLER_TYPE_ADDED, controller.type);

            for (int i = 0; i < _buttonList.Count; i++)
            {
                _buttonList[i].SetupKeys();
            }
        }

        private void OnControllerRemoved(ControllerAssignmentChangedEventArgs args)
        {
            if (args.controller != null)
            {
                Controller argController = args.controller;

                Log.Info(LogTags.Input, "컨트롤러가 제거되었습니다. 입력 정보를 갱신합니다. Type:{2}, Name:{0}(ID:{1})",
                    argController.name, argController.identifier.controllerId, argController.type);

                if (CurrentJoystick != null)
                {
                    if (CurrentJoystick.id == argController.id)
                    {
                        CurrentJoystick = null;
                    }
                }

                GlobalEvent<ControllerType>.Send(GlobalEventType.GAME_CONTROLLER_TYPE_REMOVED, args.controller.type);

                for (int i = 0; i < _buttonList.Count; i++)
                {
                    _buttonList[i].SetupKeys();
                }
            }
        }

        private void OnCurrentJoystickChanged(Player player, Controller controller)
        {
            if (player == null || InputPlayer == null)
            {
                Log.Warning(LogTags.Input, "현재 조이스틱이 변경되었으나,Rewired의 플레이어 또는 저장된 플레이어 클래스가 유효하지 않습니다.");
                return;
            }

            if (controller == null)
            {
                CurrentJoystick = null;
                _currentControllerType = ControllerType.Keyboard;
                SetupButtonEvents();
                _ = GlobalEvent<ControllerType>.Send(GlobalEventType.GAME_CONTROLLER_TYPE_CHANGED, CurrentControllerType);
                return;
            }

            if (controller.type != ControllerType.Joystick)
            {
                return;
            }

            bool isFirstTime = CurrentJoystick == null;
            bool isDifferent = CurrentJoystick != null && CurrentJoystick.name != controller.name;

            if (isFirstTime || isDifferent)
            {
                CurrentJoystick = controller;
                _currentControllerType = ControllerType.Joystick;
                SetupButtonEvents();

                if (CheckPSJoystick(controller.name))
                {
                    AddTouchPadMapping(controller);
                    SwapOptionsMapping(controller);
                    MapOptionsToSkip(controller);
                }

                _ = GlobalEvent<ControllerType>.Send(GlobalEventType.GAME_CONTROLLER_TYPE_CHANGED, CurrentControllerType);
            }
        }

        #endregion 컨트롤러 이벤트 핸들러

        #region 컨트롤러 설정 및 관리

        private void SetupController()
        {
            for (int i = 0; i < ReInput.controllers.Controllers.Count; i++)
            {
                Controller controller = ReInput.controllers.Controllers[i];
                Log.Info(LogTags.Input, "Connected {2}: {0}(ID:{1})", controller.name, controller.identifier.controllerId, controller.type);
            }
        }

        public void ReinitControllers()
        {
        }

        public List<ControllerType> GetActiveControllerTypes()
        {
            List<ControllerType> controllerList = new();
            for (int i = 0; i < ReInput.controllers.Controllers.Count; i++)
            {
                Controller controller = ReInput.controllers.Controllers[i];
                controllerList.Add(controller.type);
            }

            return controllerList;
        }

        public (bool hasKeyboard, bool hasJoystick) GetControllerAvailability()
        {
            List<ControllerType> activeControllerTypes = GetActiveControllerTypes();

            bool hasKeyboard = false;
            bool hasJoystick = false;
            bool isPCBuild = CheckPCBuild();

            for (int i = 0; i < activeControllerTypes.Count; i++)
            {
                ControllerType controllerType = activeControllerTypes[i];
                switch (controllerType)
                {
                    case ControllerType.Keyboard:
                    case ControllerType.Mouse:
                        hasKeyboard = isPCBuild;
                        break;

                    case ControllerType.Joystick:
                        hasJoystick = isPCBuild;
                        break;
                }
            }

            Log.Info(LogTags.Input, "컨트롤러 가용성 확인 - Keyboard: {0}, Joystick: {1}, PC Build: {2}",
                hasKeyboard.ToBoolString(), hasJoystick.ToBoolString(), isPCBuild.ToBoolString());

            return (hasKeyboard, hasJoystick);
        }

        #endregion 컨트롤러 설정 및 관리

        #region 컨트롤러 타입 갱신

        private void RefreshControllerType()
        {
            if (ReInput.controllers == null)
            {
                return;
            }

            ControllerType controllerType = ReInput.controllers.GetLastActiveControllerType();
#if UNITY_PS5 || UNITY_GAMECORE || UNITY_SWITCH
            controllerType = ControllerType.Joystick;
#endif

            if (CurrentControllerType != controllerType)
            {
                // PC가 아닌 플랫폼에서는 마우스/키보드로의 변경을 제한
                if (!CheckPCBuild() && CheckMouseOrKeyboardType(controllerType))
                {
                    Log.Info(LogTags.Input, "PC가 아닌 플랫폼에서 마우스/키보드 컨트롤러 타입 변경을 차단합니다. 요청된 타입: {0}", controllerType);
                    return;
                }
                _currentControllerType = controllerType;

                RefreshJoystickType();

                if ((CurrentControllerType == ControllerType.Keyboard && controllerType == ControllerType.Mouse)
                    || (CurrentControllerType == ControllerType.Mouse && controllerType == ControllerType.Keyboard))
                {
                    return;
                }
                else
                {
                    _ = GlobalEvent<ControllerType>.Send(GlobalEventType.GAME_CONTROLLER_TYPE_CHANGED, CurrentControllerType);
                }
            }
        }

        private void RefreshJoystickType()
        {
            if (CheckPSJoystick())
            {
                if (CurrentJoystick != null && CurrentJoystick.name.Contains("DualSense"))
                {
                    _currentJoystickType = JoystickTypes.PlayStation5;
                }
                else
                {
                    _currentJoystickType = JoystickTypes.PlayStation;
                }

                if (CurrentJoystick != null && CheckPSJoystick(CurrentJoystick.name))
                {
                    AddTouchPadMapping(CurrentJoystick);
                    SwapOptionsMapping(CurrentJoystick);
                    MapOptionsToSkip(CurrentJoystick);
                }
            }
            else if (CheckNintendoJoystick())
            {
                _currentJoystickType = JoystickTypes.Nintendo;
            }
            else if (CurrentControllerType == ControllerType.Joystick)
            {
                _currentJoystickType = JoystickTypes.Xbox;
            }
            else
            {
                _currentJoystickType = JoystickTypes.None;
            }
        }

        #endregion 컨트롤러 타입 갱신

        #region 컨트롤러 타입 확인 유틸리티

        public bool CheckPCBuild()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            return true;
#else
            return false;
#endif
        }

        private bool CheckMouseOrKeyboardType(ControllerType controllerType)
        {
            return controllerType is ControllerType.Mouse or ControllerType.Keyboard;
        }

        private bool CheckPSJoystick(string joystickName)
        {
#if UNITY_PS5
            return true;
#endif

            if (joystickName.Contains("Dual"))
            {
                return true;
            }

            return false;
        }

        private bool CheckPSJoystick()
        {
#if UNITY_PS5
            return true;
#endif

            if (CurrentJoystick == null)
            {
                return false;
            }

            if (CurrentJoystick.name.Contains("DualSense"))
            {
                return true;
            }

            return false;
        }

        private bool CheckNintendoJoystick()
        {
#if UNITY_SWITCH
            return true;
#endif
            if (CurrentJoystick == null)
            {
                return false;
            }

            if (CurrentJoystick.name.Contains("Nintendo") ||
                CurrentJoystick.name.Contains("Switch") ||
                CurrentJoystick.name.Contains("Joy-Con"))
            {
                return true;
            }

            return false;
        }

        #endregion 컨트롤러 타입 확인 유틸리티
    }
}