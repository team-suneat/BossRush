using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public abstract class UCGameOptionBase : XBehaviour
    {
        [FoldoutGroup("#Base")]
        [SerializeField] protected UISelectElementIndexer _indexer;
        public UISelectElementIndexer Indexer => _indexer;

        public UnityAction HideCallback { get; set; }

        public virtual void Show()
        {
            ActivateRaycast();
            SetActive(true);
            OnShow();
        }

        public virtual void Hide()
        {
            HideCallback?.Invoke();
            SetActive(false);
            OnHide();
        }

        internal virtual void InternalHide()
        {
            SetActive(false);
            OnHide();
        }

        protected virtual void OnShow()
        {
            if (_indexer != null)
            {
                _ = UIManager.Instance.SelectController.Select(_indexer.DefaultIndex);
            }
        }

        protected virtual void OnHide()
        {
        }

        public virtual void ActivateRaycast()
        {
            _indexer?.ActivateRaycast();
        }

        public virtual void DeactivateRaycast()
        {
            _indexer?.DeactivateRaycast();
        }

        protected void SetActiveEventButton(UISelectButton element, bool isActive)
        {
            element?.SetActive(isActive);
        }

        protected void HideUnderlineEventButton(UISelectButton element)
        {
            UIInteractiveElement interactiveElement = element.GetComponentNoAlloc<UIInteractiveElement>();
            interactiveElement?.HideUnderline();
        }
    }
}