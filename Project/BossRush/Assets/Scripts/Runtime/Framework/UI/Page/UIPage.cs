using UnityEngine;

namespace TeamSuneat
{
    public class UIPage : XBehaviour
    {
        [SerializeField] private bool _startActive;

        public bool IsActive { get; private set; }

        private void Awake()
        {
            IsActive = true;

            if (_startActive)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            Initialize();
        }

        public virtual void Initialize()
        {
        }

        public virtual void Show()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            SetActive(true);
            OnShow();
        }

        public virtual void Hide()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;
            SetActive(false);
            OnHide();
        }

        protected virtual void OnShow()
        {
            Log.Progress(LogTags.UI_Page, $"{this.GetHierarchyName()} 페이지를 엽니다.");
        }

        protected virtual void OnHide()
        {
            Log.Progress(LogTags.UI_Page, $"{this.GetHierarchyName()} 페이지를 닫습니다.");
        }
    }
}