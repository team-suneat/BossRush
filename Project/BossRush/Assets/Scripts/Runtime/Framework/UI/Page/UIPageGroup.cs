using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class UIPageGroup : XBehaviour
    {
        [Title("#UIPageGroup")]
        [SerializeField] private UIPage[] _pages;

        public int PageCount => _pages.Length;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _pages = this.GetComponentsInDirectChildren<UIPage>();
        }

        private void Awake()
        {
            CloseAllPages();
        }

        public void ShowPage(int index)
        {
            if (index < 0 || index >= _pages.Length)
            {
                Debug.LogWarning($"[UIPageGroup] 유효하지 않은 인덱스입니다: {index}");
                return;
            }

            if (_pages[index] != null)
            {
                _pages[index].Show();
            }
        }

        public void HidePage(int index)
        {
            if (index < 0 || index >= _pages.Length)
            {
                Debug.LogWarning($"[UIPageGroup] 유효하지 않은 인덱스입니다: {index}");
                return;
            }

            if (_pages[index] != null)
            {
                _pages[index].Hide();
            }
        }

        public void CloseAllPages()
        {
            foreach (var page in _pages)
            {
                if (page != null)
                {
                    page.Hide();
                }
            }
        }

        public UIPage GetPage(int index)
        {
            if (index < 0 || index >= _pages.Length)
            {
                return null;
            }

            return _pages[index];
        }
    }
}