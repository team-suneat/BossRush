using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    // 승급 페이지 - 캐릭터 승급 관리
    public class UIPromotionPage : UIPage
    {
        [Title("#UIPromotionPage")]
        [SerializeField] private Transform _contentParent;

        // TODO: 승급 관련 UI 구현

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        public void Refresh()
        {
            // TODO: 승급 페이지 갱신 로직 구현
            Log.Info(LogTags.UI_Page, "승급 페이지가 열렸습니다.");
        }
    }
}