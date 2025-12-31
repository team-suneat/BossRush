using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class UITogglePageController : XBehaviour
    {
        [Title("#UITogglePageController")]
        [SerializeField] private UIToggleGroup _toggleGroup;
        [SerializeField] private UIPageGroup _pageGroup;

        private int _currentActivePageIndex = -1;

        private void Awake()
        {
            Initialize();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _toggleGroup ??= GetComponentInChildren<UIToggleGroup>();
            _pageGroup ??= GetComponentInChildren<UIPageGroup>();
        }

        private void Initialize()
        {
            AutoGetComponents();

            if (_toggleGroup == null)
            {
                Debug.LogError($"[UITogglePageController] UIToggleGroup을 찾을 수 없습니다. {gameObject.name}");
                return;
            }

            if (_pageGroup == null)
            {
                Debug.LogError($"[UITogglePageController] UIPageGroup을 찾을 수 없습니다. {gameObject.name}");
                return;
            }

            if (_toggleGroup.ToggleCount != _pageGroup.PageCount)
            {
                Debug.LogError($"[UITogglePageController] Toggle과 Page의 개수가 일치하지 않습니다. Toggle: {_toggleGroup.ToggleCount}, Page: {_pageGroup.PageCount}");
                return;
            }

            _toggleGroup.OnToggleChanged.AddListener(OnToggleChanged);
        }

        private void OnToggleChanged(int toggleIndex)
        {
            if (toggleIndex == -1)
            {
                // 토글이 닫힌 경우 - 현재 페이지 닫기
                if (_currentActivePageIndex >= 0)
                {
                    _pageGroup.HidePage(_currentActivePageIndex);
                    _currentActivePageIndex = -1;
                }
            }
            else
            {
                UIToggle uiToggle = _toggleGroup.GetUIToggle(toggleIndex);
                if (uiToggle != null && uiToggle.IsLocked)
                {
                    uiToggle.RequestLockMessage();
                    _toggleGroup.SetToggle(toggleIndex, false);
                    return;
                }

                // 다른 토글이 열린 경우 - 기존 페이지 닫고 새 페이지 열기
                if (_currentActivePageIndex >= 0 && _currentActivePageIndex != toggleIndex)
                {
                    _pageGroup.HidePage(_currentActivePageIndex);
                }

                _pageGroup.ShowPage(toggleIndex);
                _currentActivePageIndex = toggleIndex;
            }
        }

        public void OpenPage(int index)
        {
            if (_toggleGroup != null)
            {
                _toggleGroup.SetToggle(index, true);
            }
        }

        public void ClosePage(int index)
        {
            if (_toggleGroup != null)
            {
                _toggleGroup.SetToggle(index, false);
            }
        }

        public UIPage GetPage(int index)
        {
            return _pageGroup?.GetPage(index);
        }
    }
}

