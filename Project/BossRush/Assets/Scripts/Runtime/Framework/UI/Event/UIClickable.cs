using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamSuneat.UserInterface
{
    [System.Serializable]
    public class ClickableEvent : UnityEvent<PointerEventData>
    { }

    public class UIClickable : XBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [FoldoutGroup("#Event")]
        public ClickableEvent OnPointerClickLeftEvent;

        [FoldoutGroup("#Event")]
        public ClickableEvent OnPointerClickRightEvent;

        [FoldoutGroup("#Event")]
        public ClickableEvent OnPointerPressLeftEvent;

        [FoldoutGroup("#Event")]
        public ClickableEvent OnPointerUpLeftEvent;

        internal UnityEvent OnPointerClickLeftCallback = new();
        internal UnityEvent OnPointerClickRightCallback = new();
        internal UnityEvent OnPointerPressLeftCallback = new();
        internal UnityEvent OnPointerUpLeftCallback = new();

        private bool _buttonPressed;

        public bool ButtonPressed
        {
            get => _buttonPressed;
            private set => _buttonPressed = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                StartPointerPress(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                StopPointerPress();

                CallPointerUpLeftEvent(eventData);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                CallPointerClickLeftEvent(eventData);
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                CallPointerClickRightEvent(eventData);
            }
        }

        #region Press Left

        private void StartPointerPress(PointerEventData eventData)
        {
            if (!ButtonPressed)
            {
                ButtonPressed = true;
                StartXCoroutine(ProcessPress(eventData));
            }
        }

        private void StopPointerPress()
        {
            ButtonPressed = false;
        }

        private IEnumerator ProcessPress(PointerEventData eventData)
        {
            while (ButtonPressed)
            {
                yield return null;
                CallPointerPressLeftEvent(eventData);
            }
        }

        public void RegisterPointerPressLeftEvent(UnityAction<PointerEventData> action)
        {
            AddListenerToEvent(OnPointerPressLeftEvent, action);
        }

        public void UnregisterPointerPressLeftEvent(UnityAction<PointerEventData> action)
        {
            RemoveListenerFromEvent(OnPointerPressLeftEvent, action);
        }

        public void ClearPointerPressLeftEvent()
        {
            ClearEventListeners(OnPointerPressLeftEvent);
        }

        public void CallPointerPressLeftEvent(PointerEventData eventData)
        {
            InvokeEvent(OnPointerPressLeftEvent, eventData);
            InvokeEvent(OnPointerPressLeftCallback);
        }

        #endregion Press Left

        #region Up Left

        public void RegisterPointerUpLeftEvent(UnityAction<PointerEventData> action)
        {
            AddListenerToEvent(OnPointerUpLeftEvent, action);
        }

        public void UnregisterPointerUpLeftEvent(UnityAction<PointerEventData> action)
        {
            RemoveListenerFromEvent(OnPointerUpLeftEvent, action);
        }

        public void ClearPointerUpLeftEvent()
        {
            ClearEventListeners(OnPointerUpLeftEvent);
        }

        public void CallPointerUpLeftEvent(PointerEventData eventData)
        {
            InvokeEvent(OnPointerUpLeftEvent, eventData);
            InvokeEvent(OnPointerUpLeftCallback);
        }

        #endregion Up Left

        #region Click Left

        public void RegisterPointerClickLeftEvent(UnityAction<PointerEventData> action)
        {
            AddListenerToEvent(OnPointerClickLeftEvent, action);
        }

        public void UnregisterPointerClickLeftEvent(UnityAction<PointerEventData> action)
        {
            RemoveListenerFromEvent(OnPointerClickLeftEvent, action);
        }

        public void ClearPointerClickLeftEvent()
        {
            ClearEventListeners(OnPointerClickLeftEvent);
        }

        public void CallPointerClickLeftEvent(PointerEventData eventData)
        {
            InvokeEvent(OnPointerClickLeftEvent, eventData);
            InvokeEvent(OnPointerClickLeftCallback);
        }

        #endregion Click Left

        #region Click Right

        public void RegisterPointerClickRightEvent(UnityAction<PointerEventData> action)
        {
            AddListenerToEvent(OnPointerClickRightEvent, action);
        }

        public void UnregisterPointerClickRightEvent(UnityAction<PointerEventData> action)
        {
            RemoveListenerFromEvent(OnPointerClickRightEvent, action);
        }

        public void ClearPointerClickRightEvent()
        {
            ClearEventListeners(OnPointerClickRightEvent);
        }

        public void CallPointerClickRightEvent(PointerEventData eventData)
        {
            InvokeEvent(OnPointerClickRightEvent, eventData);
            InvokeEvent(OnPointerClickRightCallback);
        }

        #endregion Click Right

        #region Event Utilities

        private void AddListenerToEvent<T>(UnityEvent<T> unityEvent, UnityAction<T> action)
        {
            unityEvent?.AddListener(action);
        }

        private void RemoveListenerFromEvent<T>(UnityEvent<T> unityEvent, UnityAction<T> action)
        {
            unityEvent?.RemoveListener(action);
        }

        private void ClearEventListeners<T>(UnityEvent<T> unityEvent)
        {
            unityEvent?.RemoveAllListeners();
        }

        private void InvokeEvent<T>(UnityEvent<T> unityEvent, T param)
        {
            unityEvent?.Invoke(param);
        }

        private void InvokeEvent(UnityEvent unityEvent)
        {
            unityEvent?.Invoke();
        }

        #endregion Event Utilities
    }
}
