using Rewired;
using System.Collections.Generic;

namespace TeamSuneat
{
    public partial class TSInputManager
    {
        private Dictionary<ActionNames, TSInputButton> _buttonDictionary;
        private List<TSInputButton> _buttonList;

        private void SetupButtonEvents()
        {
            _buttonDictionary = new();
            _buttonList = new();

            ActionNames[] actionNames = EnumEx.GetValues<ActionNames>(true);
            for (int i = 0; i < actionNames.Length; i++)
            {
                ActionNames actionName = actionNames[i];
                switch (actionName)
                {
                    case ActionNames.None:
                    case ActionNames.MoveHorizontal:
                    case ActionNames.MoveVertical:
                        continue;
                }

                TSInputButton button = new(actionName, OnButtonDown, OnButtonPressed, OnButtonUp);
                _buttonDictionary.Add(actionName, button);
                _buttonList.Add(button);

                Log.Info(LogTags.Input, "{0} 버튼이 생성되고, 초기화되었습니다.", actionName);
            }
        }

        private void OnButtonDown(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.TryGetValue(actionName, out TSInputButton button))
                {
                    button.State.ChangeState(ButtonStates.ButtonDown);
                }
            }
        }

        private void OnButtonPressed(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.TryGetValue(actionName, out TSInputButton button))
                {
                    button.State.ChangeState(ButtonStates.ButtonPressed);
                }
            }
        }

        private void OnButtonUp(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.TryGetValue(actionName, out TSInputButton button))
                {
                    button.State.ChangeState(ButtonStates.ButtonUp);
                }
            }
        }

        public bool CheckButtonState(ActionNames action1Name, ActionNames action2Name, ButtonStates buttonState)
        {
            if (CheckButtonState(action1Name, buttonState) || CheckButtonState(action2Name, buttonState))
            {
                return true;
            }

            return false;
        }

        public bool CheckButtonState(ActionNames actionName, ButtonStates buttonState)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.ContainsKey(actionName))
                {
                    if (_buttonDictionary[actionName].CheckState(buttonState))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckButtonState(ActionNames actionName, ButtonStates buttonState1, ButtonStates buttonState2)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.ContainsKey(actionName))
                {
                    TSInputButton button = _buttonDictionary[actionName];
                    if (button.CheckState(buttonState1) || button.CheckState(buttonState2))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckButtonTimeSinceLastButtonDown(ActionNames actionName, float bufferDuration)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.ContainsKey(actionName))
                {
                    if (_buttonDictionary[actionName].TimeSinceLastButtonDown < bufferDuration)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void TriggerButtonUp(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.TryGetValue(actionName, out TSInputButton button))
                {
                    button.TriggerButtonUp();
                }
            }
        }

        public TSInputButton GetButton(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.ContainsKey(actionName))
                {
                    return _buttonDictionary[actionName];
                }
            }

            return null;
        }

        /// <summary>
        /// 특정 액션의 버튼을 지정한 시간 동안 잠급니다.
        /// </summary>
        /// <param name="actionName">잠글 액션</param>
        /// <param name="duration">잠금 유지 시간(초)</param>
        public void LockButtonForDuration(ActionNames actionName, float duration)
        {
            if (!_buttonDictionary.IsValid())
            {
                return;
            }

            if (_buttonDictionary.TryGetValue(actionName, out TSInputButton button))
            {
                button.LockForDuration(duration);
            }
        }

        /// <summary>
        /// 특정 액션의 버튼 잠금을 해제합니다.
        /// </summary>
        /// <param name="actionName">해제할 액션</param>
        public void UnlockButton(ActionNames actionName)
        {
            if (!_buttonDictionary.IsValid())
            {
                return;
            }

            if (_buttonDictionary.TryGetValue(actionName, out TSInputButton button))
            {
                button.Unlock();
            }
        }

        public string GetKey(ActionNames actionName)
        {
            return GetKey(CurrentControllerType, actionName);
        }

        public string GetKey(ControllerType controllerType, ActionNames actionName)
        {
            if (_buttonDictionary.IsValid())
            {
                if (_buttonDictionary.ContainsKey(actionName))
                {
                    return _buttonDictionary[actionName].GetKey(controllerType);
                }
            }

            return null;
        }

        /// <summary>
        /// 특정 액션의 키 정보를 업데이트합니다.
        /// </summary>
        /// <param name="actionName">업데이트할 액션 이름</param>
        public void UpdateButtonKeys(ActionNames actionName)
        {
            if (_buttonDictionary.IsValid() && _buttonDictionary.ContainsKey(actionName))
            {
                TSInputButton button = _buttonDictionary[actionName];
                button.SetupKeys();
                button.InitializeState();

                Log.Info(LogTags.Input, "{0} 액션의 키 정보가 업데이트되었습니다.", actionName);
            }
        }

        /// <summary>
        /// 모든 버튼의 키 정보를 업데이트합니다.
        /// </summary>
        public void UpdateAllButtonKeys()
        {
            if (_buttonList.IsValid())
            {
                for (int i = 0; i < _buttonList.Count; i++)
                {
                    TSInputButton button = _buttonList[i];
                    if (button != null)
                    {
                        button.SetupKeys();
                        button.InitializeState();
                    }
                }

                Log.Info(LogTags.Input, "모든 버튼의 키 정보가 업데이트되었습니다.");
            }
        }
    }
}