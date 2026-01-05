using Rewired;
using System.Collections.Generic;

namespace TeamSuneat
{
    /// <summary>
    /// TSInputManager의 컨트롤러 이벤트 관련 기능을 담당하는 파셜 클래스
    /// </summary>
    public partial class TSInputManager
    {
        #region 이벤트 구독/해제

        /// <summary>
        /// 컨트롤러 이벤트를 구독합니다.
        /// </summary>
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

        /// <summary>
        /// 컨트롤러 이벤트 구독을 해제합니다.
        /// </summary>
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
                Log.Error("Failed to OnControllerAdded. args is null.");
                return;
            }
            OnControllerAdded(args.controller);
        }

        /// <summary>
        /// 컨트롤러가 추가되었을 때 호출되는 이벤트 핸들러
        /// </summary>
        /// <param name="controller">컨트롤러 할당 변경 이벤트 인수</param>
        private void OnControllerAdded(Rewired.Controller controller)
        {
            if (controller == null)
            {
                Log.Error("Failed to OnControllerAdded. args is null.");
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

        /// <summary>
        /// 컨트롤러가 제거되었을 때 호출되는 이벤트 핸들러
        /// </summary>
        /// <param name="args">컨트롤러 할당 변경 이벤트 인수</param>
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

        /// <summary>
        /// 현재 조이스틱이 변경되었을 때 호출되는 이벤트 핸들러
        /// </summary>
        /// <param name="player">플레이어</param>
        /// <param name="controller">컨트롤러</param>
        private void OnCurrentJoystickChanged(Player player, Controller controller)
        {
            if (player == null || InputPlayer == null)
            {
                Log.Warning(LogTags.Input, "현재 조이스틱이 변경되었으나,Rewired의 플레이어 또는 저장된 플레이어 클래스가 유효하지 않습니다.");
                return;
            }

            if (controller == null)
            {
#if VIRIDIAN_PATCH
#else
                CurrentJoystick = null;
                _currentControllerType = ControllerType.Keyboard;
                SetupButtonEvents();
                _ = GlobalEvent<ControllerType>.Send(GlobalEventType.GAME_CONTROLLER_TYPE_CHANGED, CurrentControllerType);
#endif
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

        /// <summary>
        /// 컨트롤러를 설정합니다.
        /// </summary>
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
#if VIRIDIAN
            // Added here because Rewired is "clearing" controllers on init
            for (int i = 0; i < ReInput.controllers.Controllers.Count; i++)
            {
                Controller controller = ReInput.controllers.Controllers[i];
                if (controller.type != ControllerType.Joystick)
                {
                    continue;
                }
                OnControllerAdded(controller);
            }
#endif
        }

        /// <summary>
        /// 활성화된 컨트롤러 타입 목록을 가져옵니다.
        /// </summary>
        /// <returns>활성화된 컨트롤러 타입 목록</returns>
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

        /// <summary>
        /// 현재 연결된 컨트롤러의 가용성을 확인합니다.
        /// </summary>
        /// <returns>(키보드/마우스 사용 가능, 조이스틱 사용 가능)</returns>
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

        /// <summary>
        /// 컨트롤러 타입을 갱신합니다.
        /// </summary>
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
#if VIRIDIAN_PATCH
#else
                // PC가 아닌 플랫폼에서는 마우스/키보드로의 변경을 제한
                if (!CheckPCBuild() && CheckMouseOrKeyboardType(controllerType))
                {
                    Log.Info(LogTags.Input, "PC가 아닌 플랫폼에서 마우스/키보드 컨트롤러 타입 변경을 차단합니다. 요청된 타입: {0}", controllerType);
                    return;
                }
#endif
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

        /// <summary>
        /// 조이스틱 타입을 갱신합니다.
        /// </summary>
        private void RefreshJoystickType()
        {
#if VIRIDIAN_PATCH
            /*Controller types are invariant on consoles*/
#else
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
#endif
        }

        #endregion 컨트롤러 타입 갱신

        #region 컨트롤러 타입 확인 유틸리티

        /// <summary>
        /// 현재 빌드가 PC 빌드인지 확인합니다.
        /// </summary>
        /// <returns>PC 빌드 여부</returns>
        public bool CheckPCBuild()
        {
#if VIRIDIAN_PATCH
            return false;
#else
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            return true;
#else
            return false;
#endif
#endif
        }

        /// <summary>
        /// 지정된 컨트롤러 타입이 마우스 또는 키보드 타입인지 확인합니다.
        /// </summary>
        /// <param name="controllerType">확인할 컨트롤러 타입</param>
        /// <returns>마우스/키보드 타입 여부</returns>
        private bool CheckMouseOrKeyboardType(ControllerType controllerType)
        {
            return controllerType is ControllerType.Mouse or ControllerType.Keyboard;
        }

        /// <summary>
        /// PlayStation 조이스틱인지 확인합니다.
        /// </summary>
        /// <param name="joystickName">조이스틱 이름</param>
        /// <returns>PlayStation 조이스틱 여부</returns>
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

        /// <summary>
        /// 현재 조이스틱이 PlayStation 조이스틱인지 확인합니다.
        /// </summary>
        /// <returns>PlayStation 조이스틱 여부</returns>
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

        /// <summary>
        /// 현재 조이스틱이 Nintendo 조이스틱인지 확인합니다.
        /// </summary>
        /// <returns>Nintendo 조이스틱 여부</returns>
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