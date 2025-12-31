using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 구매 버튼 컴포넌트
    public class UIPurchaseButton : UIButton
    {
        [FoldoutGroup("#UIButton-Purchase"), SerializeField]
        private Image _currencyIconImage;

        [FoldoutGroup("#UIButton-Purchase"), SerializeField]
        private UILocalizedText _costCurrencyText;

        [FoldoutGroup("#UIButton-Purchase"), SerializeField]
        private TextMeshProUGUI _costText;

        protected CurrencyNames _currencyName;
        protected int _cost;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _currencyIconImage ??= this.FindComponent<Image>("Currency Icon Image");
            _costCurrencyText ??= this.FindComponent<UILocalizedText>("Currency Text");
            _costText ??= this.FindComponent<TextMeshProUGUI>("Cost Text");
        }

        public void Setup(CurrencyNames currencyName, int cost)
        {
            _currencyName = currencyName;
            _cost = cost;

            Refresh();
        }

        public virtual void Refresh()
        {
            RefreshCurrencyIcon();
            RefreshCurrencyText();
            RefreshCostText();
        }

        private void RefreshCurrencyIcon()
        {
            if (_currencyIconImage == null)
            {
                return;
            }

            if (_currencyName == CurrencyNames.None)
            {
                _currencyIconImage.enabled = false;
                return;
            }

            Sprite iconSprite = _currencyName.LoadSprite();
            if (iconSprite != null)
            {
                _currencyIconImage.sprite = iconSprite;
                _currencyIconImage.enabled = true;
            }
            else
            {
                _currencyIconImage.enabled = false;
            }
        }

        private void RefreshCurrencyText()
        {
            if (_costCurrencyText == null)
            {
                return;
            }

            string stringKey = _currencyName.GetStringKey();
            _costCurrencyText.SetStringKey(stringKey);
        }

        private void RefreshCostText()
        {
            if (_costText == null)
            {
                return;
            }

            _costText.SetText(_cost.ToString("N0"));
        }

        protected void ActivateFrameColor()
        {
            SetFrameImageColor(GameColors.MediumAquamarine);
        }

        protected void DeactivateFrameColor()
        {
            SetFrameImageColor(GameColors.IndianRed);
        }

        protected bool CanPurchase(VProfile profile)
        {
            return profile.Currency.CanUse(_currencyName, _cost);
        }

        protected bool TryPurchase(VProfile profile)
        {
            if (!profile.Currency.CanUseOrNotify(_currencyName, _cost))
            {
                return false;
            }

            profile.Currency.Use(_currencyName, _cost);
            return true;
        }

        protected override bool CheckClickable()
        {
            if (!base.CheckClickable())
            {
                return false;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return false;
            }

            return CanPurchase(profile);
        }

        protected override void OnClickSucceeded()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (TryPurchase(profile))
            {
                base.OnClickSucceeded();
            }
        }

        protected override void OnClickFailed()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (!TryPurchase(profile))
            {
                base.OnClickFailed();
            }
        }

        protected override void OnHoldSucceeded()
        {
            OnClickSucceeded();
        }

        protected override void OnHoldFailed()
        {
            OnClickFailed();
        }
    }
}