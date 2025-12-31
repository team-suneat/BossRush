using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 캐릭터 외형 탭 페이지 (향후 구현)
    public class UICharacterAppearancePage : UIPage
    {
        [Title("#UICharacterAppearancePage")]
        [SerializeField] private UILocalizedText _comingSoonText;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _comingSoonText ??= this.FindComponent<UILocalizedText>("Coming Soon Text");
        }

        protected override void OnShow()
        {
            base.OnShow();

            // 향후 구현 - 현재는 빈 페이지
            if (_comingSoonText != null)
            {
                string comingSoonMessage = Data.JsonDataManager.FindStringClone("ComingSoon");
                if (string.IsNullOrEmpty(comingSoonMessage))
                {
                    comingSoonMessage = "준비 중...";
                }
                _comingSoonText.SetText(comingSoonMessage);
            }
        }

        public void Refresh()
        {
            // 향후 구현
        }
    }
}

