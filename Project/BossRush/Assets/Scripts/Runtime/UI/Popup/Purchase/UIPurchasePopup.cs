using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIPurchasePopup : UIPopup
    {
        [FoldoutGroup("#UIPurchasePopup")]
        [SerializeField]
        private UILocalizedText _contentText;

        [FoldoutGroup("#UIPurchasePopup")]
        [SerializeField]
        private UIPurchaseButton _purchaseButton;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _contentText ??= this.FindComponent<UILocalizedText>("Content Text");
            _purchaseButton ??= this.FindComponent<UIPurchaseButton>("Purchase Button");
        }

        public override UIPopupNames Name => UIPopupNames.Purchase;

        public void Setup(string content, CurrencyNames currencyName, int price)
        {
            _contentText?.SetText(content);
            _purchaseButton?.Setup(currencyName, price);
            _purchaseButton?.RegisterClickSuccessEvent(() => OnClickSuccess());
        }

        private void OnClickSuccess()
        {
            CoroutineNextTimer(0.3f, CloseWithSuccess);
        }
    }
}