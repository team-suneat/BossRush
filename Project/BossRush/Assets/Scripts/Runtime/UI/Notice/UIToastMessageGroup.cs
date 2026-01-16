using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIToastMessageGroup : XBehaviour
    {
        private const int MAX_ACTIVE_TOASTS = 5;

        [SerializeField]
        private string _toastPrefabName = "UIToastMessage";

        private VerticalLayoutGroup _layoutGroup;
        private List<UIToastMessage> _activeToasts = new List<UIToastMessage>();

        public int ActiveCount => _activeToasts.Count;
        public int MaxCount => MAX_ACTIVE_TOASTS;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
        }

        private void Awake()
        {
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
        }

        public void AddToast(string content, float duration = 2f)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            if (_layoutGroup == null)
            {
                Log.Warning(LogTags.UI_Notice, "토스트 그룹의 레이아웃 그룹이 설정되지 않았습니다.");
                return;
            }

            while (_activeToasts.Count >= MAX_ACTIVE_TOASTS)
            {
                UIToastMessage oldestToast = _activeToasts[0];
                if (oldestToast != null)
                {
                    oldestToast.Despawn();
                }
                else
                {
                    _activeToasts.RemoveAt(0);
                }
            }

            UIToastMessage newToast = ResourcesManager.SpawnToastMessage(transform, _toastPrefabName);
            if (newToast == null)
            {
                return;
            }

            _activeToasts.Add(newToast);

            newToast.Setup(content, duration);
            newToast.RegisterDespawnEvent(OnToastDespawned);

            newToast.transform.SetAsLastSibling();
        }

        private void OnToastDespawned(UIToastMessage toast)
        {
            RemoveToast(toast);
        }

        private void RemoveToast(UIToastMessage toast)
        {
            if (toast == null)
            {
                CleanupNullToasts();
                return;
            }

            int index = _activeToasts.IndexOf(toast);
            if (index < 0)
            {
                return;
            }

            _activeToasts.RemoveAt(index);
        }

        private void CleanupNullToasts()
        {
            for (int i = _activeToasts.Count - 1; i >= 0; i--)
            {
                if (_activeToasts[i] == null)
                {
                    _activeToasts.RemoveAt(i);
                }
            }
        }

        public void ClearAll()
        {
            for (int i = _activeToasts.Count - 1; i >= 0; i--)
            {
                UIToastMessage toast = _activeToasts[i];
                if (toast != null)
                {
                    toast.Despawn();
                }
            }

            _activeToasts.Clear();
        }

        public UIToastMessage FindToast(string content)
        {
            return _activeToasts.Find(toast => toast != null && toast.Content == content);
        }
    }
}