using Rewired;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    [System.Serializable]
    public class SelectableOverEvent : UnityEvent
    { }

    [System.Serializable]
    public class SelectableEvent : UnityEvent<PointerEventData>
    { }

    public class UISelectable : XBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int Index;

        [FoldoutGroup("#Event")]
        public SelectableEvent OnPointerEnterEvent;

        [FoldoutGroup("#Event")]
        public SelectableOverEvent OnPointerOverEvent;

        [FoldoutGroup("#Event")]
        public SelectableEvent OnPointerExitEvent;

        internal UnityEvent OnPointerEnterCallback = new();
        internal UnityEvent OnPointerExitCallback = new();

        private bool _isEntered;

        public override void AutoSetting()
        {
            base.AutoSetting();
            EnableRaycastOnImage();
        }

        private void EnableRaycastOnImage()
        {
            Image image = GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = true;
            }
        }

        private void Update()
        {
            if (_isEntered && OnPointerOverEvent != null)
            {
                OnPointerOver();
            }
        }

        private void OnPointerOver()
        {
            InvokeEvent(OnPointerOverEvent);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (IsMouseController())
            {
                HandlePointerEnter(eventData);
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (IsMouseController())
            {
                HandlePointerExit(eventData);
            }
        }

        private bool IsMouseController()
        {
            return TSInputManager.Instance.CurrentControllerType == ControllerType.Mouse;
        }

        private void HandlePointerEnter(PointerEventData eventData)
        {
            InvokeEvent(OnPointerEnterEvent, eventData);
            _isEntered = true;
        }

        private void HandlePointerExit(PointerEventData eventData)
        {
            InvokeEvent(OnPointerExitEvent, eventData);
            _isEntered = false;
        }

        #region Pointer Enter Event Management

        public void RegisterPointerEnterEvent(UnityAction<PointerEventData> action)
        {
            AddListenerToEvent(OnPointerEnterEvent, action);
        }

        public void UnregisterPointerEnterEvent(UnityAction<PointerEventData> action)
        {
            RemoveListenerFromEvent(OnPointerEnterEvent, action);
        }

        public void ClearPointerEnterEvent()
        {
            ClearEventListeners(OnPointerEnterEvent);
        }

        public void CallPointerEnterEvent(PointerEventData eventData)
        {
            InvokeEvent(OnPointerEnterEvent, eventData);
            InvokeEvent(OnPointerEnterCallback);
        }

        #endregion Pointer Enter Event Management

        #region Pointer Exit Event Management

        public void RegisterPointerExitEvent(UnityAction<PointerEventData> action)
        {
            AddListenerToEvent(OnPointerExitEvent, action);
        }

        public void UnregisterPointerExitEvent(UnityAction<PointerEventData> action)
        {
            RemoveListenerFromEvent(OnPointerExitEvent, action);
        }

        public void ClearPointerExitEvent()
        {
            ClearEventListeners(OnPointerExitEvent);
        }

        public void CallPointerExitEvent(PointerEventData eventData)
        {
            InvokeEvent(OnPointerExitEvent, eventData);
            InvokeEvent(OnPointerExitCallback);
        }

        #endregion Pointer Exit Event Management

        #region Pointer Over Event Management

        public void RegisterPointerOverEvent(UnityAction action)
        {
            AddListenerToEvent(OnPointerOverEvent, action);
        }

        public void UnregisterPointerOverEvent(UnityAction action)
        {
            RemoveListenerFromEvent(OnPointerOverEvent, action);
        }

        public void ClearPointerOverEvent()
        {
            ClearEventListeners(OnPointerOverEvent);
        }

        public void CallPointerOverEvent()
        {
            InvokeEvent(OnPointerOverEvent);
        }

        #endregion Pointer Over Event Management

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

        private void AddListenerToEvent(UnityEvent unityEvent, UnityAction action)
        {
            unityEvent?.AddListener(action);
        }

        private void RemoveListenerFromEvent(UnityEvent unityEvent, UnityAction action)
        {
            unityEvent?.RemoveListener(action);
        }

        private void ClearEventListeners(UnityEvent unityEvent)
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
