using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UISelectController : XBehaviour
    {
        private enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        private const int MAX_RECURSION_COUNT = 3;
        private const float DEFAULT_WAIT_MOVE_TIME = 0.2f;
        private const float DETAIL_WAIT_MOVE_TIME = 0.12f;
        private const float JOYSTICK_INPUT_THRESHOLD = 0.5f;

        public int CurrentIndex;
        public UISelectFrame SelectFrame;
        public Dictionary<int, UISelectElement> SelectedEventSlots = new();

        private bool _isStart;
        private float _waitMoveTime = DEFAULT_WAIT_MOVE_TIME;
        private float _currentWaitMoveTime;
        private Coroutine _showSelectFrameCoroutine;

        public int LastIndex { get; set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            SelectFrame = GetComponentInChildren<UISelectFrame>();
        }

        private void Awake()
        {
            UISelectElement[] elements = GetComponentsInChildren<UISelectElement>();
            if (elements != null)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    RegisterOrReplacePointerEvent(elements[i]);
                }
            }
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent<UIPopupNames>.Register(GlobalEventType.GAME_POPUP_CLOSE, OnPopupClosed);
            GlobalEvent.Register(GlobalEventType.GAME_POPUP_CLOSE_ALL, OnPopupClosedAll);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent<UIPopupNames>.Unregister(GlobalEventType.GAME_POPUP_CLOSE, OnPopupClosed);
            GlobalEvent.Unregister(GlobalEventType.GAME_POPUP_CLOSE_ALL, OnPopupClosedAll);
        }

        private void OnPopupClosed(UIPopupNames popupName)
        {
            if (UIManager.Instance.PopupManager.CenterPopup == null)
            {
                HideSelectFrame();
            }
        }

        private void OnPopupClosedAll()
        {
            if (UIManager.Instance.PopupManager.CenterPopup == null)
            {
                HideSelectFrame();
            }
        }

        public void Clear()
        {
            if (CurrentIndex != 0)
            {
                Deselect(CurrentIndex);
            }

            SelectedEventSlots.Clear();

            Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 초기화합니다.");
        }

        public void Remove(UISelectElement[] pointEvents)
        {
            if (pointEvents == null)
            {
                return;
            }
            pointEvents = pointEvents.RemoveNull();
            for (int i = 0; i < pointEvents.Length; i++)
            {
                UISelectElement pointEvent = pointEvents[i];
                if (SelectedEventSlots.ContainsKey(pointEvent.SelectIndex))
                {
                    Deselect(pointEvent.SelectIndex);

                    _ = SelectedEventSlots.Remove(pointEvent.SelectIndex);
                    Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 삭제합니다. {0}({1})", pointEvent.GetHierarchyName(), pointEvent.SelectIndex);
                }
            }
        }

        public void LogicUpdate()
        {
            HandlePadInput();
            OpenPopupWhileUpdate();
            UpdateMoveInput();
        }

        public void HandlePadInput()
        {
            if (!_isStart || TSInputManager.Instance.CurrentControllerType != ControllerType.Joystick)
            {
                return;
            }

            TSInputManager inputManager = TSInputManager.Instance;
            bool isPreviousPressed = inputManager.CheckButtonState(ActionNames.UIHighPrevious, ButtonStates.ButtonUp);
            bool isNextPressed = inputManager.CheckButtonState(ActionNames.UIHighNext, ButtonStates.ButtonUp);

            if (!isPreviousPressed && !isNextPressed)
            {
                return;
            }

            if (!TryMovePopupFocus())
            {
                return;
            }

            if (isPreviousPressed || isNextPressed)
            {
                ProcessCenterPopupActivation();
            }
        }

        private void ProcessCenterPopupActivation()
        {
            UIPopup centerPopup = UIManager.Instance.PopupManager.CenterPopup;
            if (centerPopup == null)
            {
                return;
            }

            centerPopup.Activate();
            centerPopup.SelectFirstSlotEvent();
        }

        private void OpenPopupWhileUpdate()
        {
            if (_isStart)
            {
                return;
            }
            if (CurrentIndex == 0)
            {
                CurrentIndex = 1;
                Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 이벤트의 인덱스를 초기화합니다. SelectIndex: 1");
            }

            Open(CurrentIndex);
            _isStart = true;
        }

        private bool TryMovePopupFocus()
        {
            return UIManager.Instance.PopupManager.CenterPopup == null;
        }

        #region Open

        public void ResetAndOpenSelection(int startIndex = 1, UISelectElement[] events = null)
        {
            Clear();

            Open(startIndex, events);
        }

        public void Open(int startIndex = 1, UISelectElement[] events = null)
        {
            RegisterPointerEvents(events);
            Select(startIndex);
        }

        public void RegisterOrReplacePointerEvent(UISelectElement eventPointer)
        {
            if (eventPointer == null)
            {
                Log.Warning(LogTags.UI_SelectEvent, "(Controller) 등록 또는 대체하려는 PointerEvent가 유효하지 않습니다. (NULL)");
                return;
            }

            int selectIndex = eventPointer.SelectIndex;

            if (SelectedEventSlots.IsValid())
            {
                if (!SelectedEventSlots.ContainsKey(selectIndex))
                {
                    SelectedEventSlots.Add(selectIndex, eventPointer);
                    Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 추가합니다. {0}({1})", eventPointer.GetHierarchyName(), selectIndex);
                }
                else
                {
                    Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 대체합니다. {0} ▶ {1}({2})", SelectedEventSlots[selectIndex].GetHierarchyName(), eventPointer.GetHierarchyName(), selectIndex);

                    _ = SelectedEventSlots.Remove(selectIndex);
                    SelectedEventSlots.Add(selectIndex, eventPointer);
                }
            }
            else
            {
                SelectedEventSlots.Add(selectIndex, eventPointer);

                Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 추가합니다. {0}({1})", eventPointer.GetHierarchyName(), selectIndex);
            }
        }

        public void RegisterPointerEvent(UISelectElement pointerEvent)
        {
            if (pointerEvent == null)
            {
                return;
            }

            if (pointerEvent.LockTrigger || pointerEvent.SelectIndex == 0)
            {
                return;
            }

            if (!SelectedEventSlots.ContainsKey(pointerEvent.SelectIndex))
            {
                SelectedEventSlots.Add(pointerEvent.SelectIndex, pointerEvent);

                Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 추가합니다. {0}({1})", pointerEvent.GetHierarchyName(), pointerEvent.SelectIndex);
            }
        }

        public void RegisterPointerEvents(UISelectElement[] pointerEvents)
        {
            pointerEvents = pointerEvents.RemoveNull();
            if (!pointerEvents.IsValid())
            {
                return;
            }

            for (int i = 0; i < pointerEvents.Length; i++)
            {
                UISelectElement pointerEvent = pointerEvents[i];
                if (pointerEvent.LockTrigger || pointerEvent.SelectIndex == 0)
                {
                    continue;
                }

                if (SelectedEventSlots.ContainsKey(pointerEvent.SelectIndex))
                {
                    continue;
                }

                SelectedEventSlots.Add(pointerEvent.SelectIndex, pointerEvent);
                Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 추가합니다. {0}({1})", pointerEvent.GetHierarchyName(), pointerEvent.SelectIndex);
            }
        }

        public void UnregisterPointerEvent(UISelectElement pointerEvent)
        {
            if (pointerEvent == null)
            {
                return;
            }

            if (!SelectedEventSlots.IsValid())
            {
                return;
            }

            if (!SelectedEventSlots.ContainsKey(pointerEvent.SelectIndex))
            {
                return;
            }

            _ = SelectedEventSlots.Remove(pointerEvent.SelectIndex);

            Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 삭제합니다. {0}({1})", pointerEvent.GetHierarchyName(), pointerEvent.SelectIndex);
        }

        public void UnregisterPointerEvents(UISelectElement[] events)
        {
            if (events == null || events.Length == 0)
            {
                if (SelectedEventSlots.Count == 0)
                {
                    return;
                }
            }

            for (int i = 0; i < events.Length; i++)
            {
                UISelectElement pointerEvent = events[i];
                if (pointerEvent == null) { continue; }

                if (!SelectedEventSlots.ContainsKey(pointerEvent.SelectIndex)) { continue; }

                _ = SelectedEventSlots.Remove(pointerEvent.SelectIndex);

                Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 삭제합니다. {0}({1})", pointerEvent.GetHierarchyName(), pointerEvent.SelectIndex);
            }
        }

        #endregion Open

        public void UpdateMoveInput()
        {
            if (SelectedEventSlots.Count > 0)
            {
                if (TSInputManager.Instance.CurrentControllerType == ControllerType.Joystick)
                {
                    if (!TryMoveByJoystickDirection())
                    {
                        MoveByDirection();
                    }

                    TryCallButtonEvent();
                }
                else if (TSInputManager.Instance.CurrentControllerType == ControllerType.Keyboard)
                {
                    MoveByDirection();

                    TryCallButtonEvent();
                }
            }
        }

        private bool TryMoveByJoystickDirection()
        {
            _currentWaitMoveTime -= Time.unscaledDeltaTime;
            if (_currentWaitMoveTime >= 0)
            {
                return false;
            }

            var horizontalMovement = TSInputManager.Instance.InputPlayer.GetAxisRaw("UICursorHorizontal");
            var verticalMovement = TSInputManager.Instance.InputPlayer.GetAxisRaw("UICursorVertical");

            var uiMovement = new Vector2(horizontalMovement, verticalMovement);
            if (Mathf.Abs(uiMovement.x) > JOYSTICK_INPUT_THRESHOLD || Mathf.Abs(uiMovement.y) > JOYSTICK_INPUT_THRESHOLD)
            {
                int newIndex = GetDirectionalIndexFromMovement(CurrentIndex, uiMovement);
                if (newIndex != 0)
                {
                    Select(newIndex);
                    return true;
                }
            }

            return false;
        }

        private int GetDirectionalIndexFromMovement(int currentIndex, Vector2 movement)
        {
            if (movement.x < -JOYSTICK_INPUT_THRESHOLD)
            {
                return GetDirectionalIndex(currentIndex, Direction.Left);
            }

            if (movement.x > JOYSTICK_INPUT_THRESHOLD)
            {
                return GetDirectionalIndex(currentIndex, Direction.Right);
            }

            if (movement.y > JOYSTICK_INPUT_THRESHOLD)
            {
                return GetDirectionalIndex(currentIndex, Direction.Up);
            }

            return movement.y < -JOYSTICK_INPUT_THRESHOLD ? GetDirectionalIndex(currentIndex, Direction.Down) : 0;
        }

        private void MoveByDirection()
        {
            if (TSInputManager.Instance.CheckButtonState(ActionNames.UIMoveLeft, ButtonStates.ButtonDown))
            {
                Select(GetDirectionalIndex(CurrentIndex, Direction.Left));
            }
            else if (TSInputManager.Instance.CheckButtonState(ActionNames.UIMoveRight, ButtonStates.ButtonDown))
            {
                Select(GetDirectionalIndex(CurrentIndex, Direction.Right));
            }
            else if (TSInputManager.Instance.CheckButtonState(ActionNames.UIMoveUp, ButtonStates.ButtonDown))
            {
                Select(GetDirectionalIndex(CurrentIndex, Direction.Up));
            }
            else if (TSInputManager.Instance.CheckButtonState(ActionNames.UIMoveDown, ButtonStates.ButtonDown))
            {
                Select(GetDirectionalIndex(CurrentIndex, Direction.Down));
            }
        }

        private void TryCallButtonEvent()
        {
            bool isKeyboard = TSInputManager.Instance.CurrentControllerType == ControllerType.Keyboard;
            ButtonStates buttonState = GetButtonState(isKeyboard);

            if (buttonState == ButtonStates.Off)
            {
                return;
            }

            switch (buttonState)
            {
                case ButtonStates.ButtonDown:
                    CallButtonDownEvent(CurrentIndex);
                    break;

                case ButtonStates.ButtonPressed:
                    CallPressedEvent(CurrentIndex);
                    break;

                case ButtonStates.ButtonUp:
                    CallButtonUpEvent(CurrentIndex);
                    break;
            }
        }

        private ButtonStates GetButtonState(bool isKeyboard)
        {
            if (isKeyboard)
            {
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UISubmit, ActionNames.UISubmit2, ButtonStates.ButtonDown))
                    return ButtonStates.ButtonDown;
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UISubmit, ActionNames.UISubmit2, ButtonStates.ButtonPressed))
                    return ButtonStates.ButtonPressed;
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UISubmit, ActionNames.UISubmit2, ButtonStates.ButtonUp))
                    return ButtonStates.ButtonUp;
            }
            else
            {
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UISubmit, ButtonStates.ButtonDown))
                    return ButtonStates.ButtonDown;
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UISubmit, ButtonStates.ButtonPressed))
                    return ButtonStates.ButtonPressed;
                if (TSInputManager.Instance.CheckButtonState(ActionNames.UISubmit, ButtonStates.ButtonUp))
                    return ButtonStates.ButtonUp;
            }

            return ButtonStates.Off;
        }

        private int GetDirectionalIndex(int currentIndex, Direction direction, int recursionCount = 0)
        {
            if (recursionCount >= MAX_RECURSION_COUNT)
            {
                return 0;
            }

            if (!SelectedEventSlots.TryGetValue(currentIndex, out UISelectElement currentEvent))
            {
                Log.Warning(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 찾을 수 없습니다. SelectIndex: {0}", currentIndex);
                return 0;
            }

            int nextIndex = GetNextIndexByDirection(currentEvent, direction);

            if (nextIndex == 0)
            {
                return 0;
            }

            if (!SelectedEventSlots.TryGetValue(nextIndex, out UISelectElement nextEvent))
            {
                Log.Warning(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 찾을 수 없습니다. SelectIndex: {0}", nextIndex);
                return 0;
            }

            if (nextEvent.ActiveInHierarchy)
            {
                return nextIndex;
            }

            return GetDirectionalIndex(nextEvent.SelectIndex, direction, recursionCount + 1);
        }

        private int GetNextIndexByDirection(UISelectElement element, Direction direction)
        {
            return direction switch
            {
                Direction.Left => element.SelectLeftIndex,
                Direction.Right => element.SelectRightIndex,
                Direction.Up => element.SelectUpIndex,
                Direction.Down => element.SelectDownIndex,
                _ => 0
            };
        }

        public bool Select(int index, bool isMouseInput = false)
        {
            if (index == 0)
            {
                Log.Warning(LogTags.UI_SelectEvent, "(Controller) 선택할 수 없습니다. 선택 가능한 이벤트를 찾을 수 없습니다. Index: {0}", index);
                return false;
            }

            if (index == CurrentIndex)
            {
                if (!isMouseInput)
                {
                    if (SelectedEventSlots.TryGetValue(index, out UISelectElement pointerEvent))
                    {
                        Log.Warning(LogTags.UI_SelectEvent, "(Controller) 선택할 수 없습니다. 같은 이벤트를 선택하지 않습니다. {0}({1})", pointerEvent.GetHierarchyPath(), index);
                    }
                    else
                    {
                        Log.Warning(LogTags.UI_SelectEvent, "(Controller) 선택할 수 없습니다. 같은 이벤트 인덱스를 가집니다. 인덱스에 해당하는 이벤트를 찾을 수 없습니다. Index: {0}", index);
                    }
                    return false;
                }
            }

            LastIndex = CurrentIndex;
            Deselect(CurrentIndex);

            if (SelectedEventSlots.TryGetValue(index, out UISelectElement element))
            {
                CurrentIndex = index;
                if (!isMouseInput)
                {
                    element.OnPointerEnter();
                    ChangeDownUpMoveSpeed(index);
                    _currentWaitMoveTime = _waitMoveTime;
                }

                Log.Info(LogTags.UI_SelectEvent, "(Controller) 선택 가능한 이벤트를 선택합니다. Select Index: {0}", index);
                return true;
            }
            else
            {
                Log.Warning(LogTags.UI_SelectEvent, "(Controller) 선택할 수 없습니다. 선택 가능한 이벤트를 찾을 수 없습니다. Index: {0}", index);
                return false;
            }
        }

        private void ChangeDownUpMoveSpeed(int index)
        {
            if (!SelectedEventSlots.TryGetValue(index, out UISelectElement element))
            {
                return;
            }

            float customTime = element.CustomMoveWaitTime;
            bool isVerticalMovement = Mathf.Abs(TSInputManager.Instance.PrimaryMovement.y) > TSInputManager.ThresholdUI.y;

            if (customTime > 0 && isVerticalMovement)
            {
                _waitMoveTime = customTime;
            }
            else
            {
                _waitMoveTime = DEFAULT_WAIT_MOVE_TIME;
            }
        }

        public void Deselect(int index)
        {
            if (index == 0)
            {
                return;
            }

            if (SelectedEventSlots.TryGetValue(index, out UISelectElement element))
            {
                element.OnPointerExit();
            }
            else
            {
                Log.Warning(LogTags.UI_SelectEvent, "(Controller) 선택 해제할 수 없습니다. 선택 가능한 이벤트를 찾을 수 없습니다. SelectIndex: {0}", index);
            }
        }

        #region Click Event

        public void CallPressedEvent(int currentIndex)
        {
            if (currentIndex == 0)
            {
                return;
            }

            if (SelectedEventSlots.TryGetValue(currentIndex, out UISelectElement pointerEvent))
            {
                pointerEvent.OnPointerPressed();
            }
            else
            {
                Log.Warning(LogTags.UI_SelectEvent, "(Controller) 키가 존재하지 않아 PRESSED 이벤트를 하지 않습니다.  Index:{0}", currentIndex);
            }
        }

        public void CallButtonDownEvent(int currentIndex)
        {
            if (currentIndex == 0)
            {
                return;
            }

            if (SelectedEventSlots.TryGetValue(currentIndex, out UISelectElement pointerEvent))
            {
                pointerEvent.OnPointerClick();
            }
            else
            {
                Log.Warning(LogTags.UI_SelectEvent, "(Controller) 키가 존재하지 않아 BUTTON DOWN 이벤트를 하지 않습니다.  Index:{0}", currentIndex);
            }
        }

        public void CallButtonUpEvent(int currentIndex)
        {
            if (currentIndex == 0)
            {
                return;
            }

            if (SelectedEventSlots.TryGetValue(currentIndex, out UISelectElement pointerEvent))
            {
                if (pointerEvent.PadClickType == UISelectElement.PadClickTypes.Left)
                {
                    pointerEvent.OnPointerUpLeft();
                }
            }
            else
            {
                Log.Warning(LogTags.UI_SelectEvent, "(Controller) 키가 존재하지 않아 BUTTON UP 이벤트를 하지 않습니다.  Index:{0}", currentIndex);
            }
        }

        #endregion Click Event

        #region Select Slot Frame

        public void HideSelectFrame()
        {
            StopXCoroutine(ref _showSelectFrameCoroutine);

            if (SelectFrame != null)
            {
                SelectFrame.Hide();
            }
        }

        public void ShowSelectFrame(UISelectFrameTypes type, Vector2 sizeDelta, Vector3 offset, Transform target, Transform parent = null)
        {
            StopXCoroutine(ref _showSelectFrameCoroutine);

            if (SelectFrame != null)
            {
                _showSelectFrameCoroutine = StartXCoroutine(ProgressSetupSelectFrame(type, sizeDelta, offset, target, parent));
            }
        }

        private IEnumerator ProgressSetupSelectFrame(UISelectFrameTypes type, Vector2 sizeDelta, Vector3 offset, Transform target, Transform parent)
        {
            yield return null;

            SelectFrame.transform.SetParent(target);
            SelectFrame.rectTransform.sizeDelta = sizeDelta;
            SelectFrame.transform.localScale = Vector3.one;
            SelectFrame.anchoredPosition3D = Vector2.zero;
            SelectFrame.anchoredPosition3D += offset;

            if (parent == null)
            {
                SelectFrame.transform.SetParent(transform);
            }
            else
            {
                SelectFrame.transform.SetParent(parent);
            }

            SelectFrame.Show();
        }


        #endregion Select Slot Frame
    }
}