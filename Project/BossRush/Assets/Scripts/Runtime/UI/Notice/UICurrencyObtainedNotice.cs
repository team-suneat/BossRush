using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 재화 획득 알림 Notice 컴포넌트
    public class UICurrencyObtainedNotice : UINoticeBase
    {
        [FoldoutGroup("Notice-CurrencyObtain")]
        [SerializeField]
        private Image _backgroundImage;

        [FoldoutGroup("Notice-CurrencyObtain")]
        [SerializeField]
        private RectTransform _textGroup;

        [FoldoutGroup("Notice-CurrencyObtain")]
        [SerializeField]
        private UILocalizedText _currencyNameText;

        public override void AutoNaming()
        {
            SetGameObjectName("UICurrencyObtainedNotice");
        }

        public void Setup(CurrencyNames currencyName, int amount)
        {
            if (_currencyNameText != null)
            {
                _currencyNameText.SetStringKey(currencyName.GetStringKey());
            }

            string formattedAmount = ValueStringEx.GetNoDigitString(amount);
            SetContent(formattedAmount);
        }
    }
}