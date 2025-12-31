using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // HUD 재화 표시 컴포넌트 - 재화 아이콘 및 소지량 표시
    public class HUDCurrency : XBehaviour
    {
        [Title("#HUDCurrency")]
        [SerializeField] private CurrencyNames _currencyName;
        [SerializeField] private string _currencyNameString;
        [SerializeField] private Image _iconImage;
        [SerializeField] private UILocalizedText _nameText;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private PunchScaler _punchScaler;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _iconImage ??= this.FindComponent<Image>("Rect/Currency Icon Image");
            _nameText ??= this.FindComponent<UILocalizedText>("Rect/Currency Name Text");
            _valueText ??= this.FindComponent<TextMeshProUGUI>("Rect/Currency Value Text");
            _punchScaler ??= GetComponentInChildren<PunchScaler>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (_currencyName != CurrencyNames.None)
            {
                _currencyNameString = _currencyName.ToString();
            }
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref _currencyName, _currencyNameString);
        }

        public override void AutoNaming()
        {
            if (_currencyName != CurrencyNames.None)
            {
                SetGameObjectName($"HUDCurrency({_currencyName})");
            }
            else
            {
                SetGameObjectName($"HUDCurrency");
            }
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        private void OnPlayerCharacterBattleReady()
        {
            RefreshIcon();
            RefreshName();
            RefreshValue();
        }

        private void OnCurrencyChanged(CurrencyNames currencyName, int addAmount)
        {
            if (_currencyName != currencyName)
            {
                return;
            }
            VProfile profileInfo = GameApp.GetSelectedProfile();
            int amount = profileInfo.Currency.GetAmount(_currencyName);
            SetValue(amount);

            _punchScaler?.PlayPunchScaleAnimation();
        }

        private void RefreshIcon()
        {
            if (_iconImage == null)
            {
                return;
            }

            if (_currencyName == CurrencyNames.None)
            {
                _iconImage.enabled = false;
                return;
            }

            Sprite iconSprite = SpriteEx.LoadSprite(_currencyName);
            if (iconSprite != null)
            {
                _iconImage.sprite = iconSprite;
                _iconImage.enabled = true;
            }
            else
            {
                _iconImage.enabled = false;
            }
        }

        private void RefreshName()
        {
            if (_nameText != null)
            {
                string stringKey = _currencyName.GetStringKey();
                _nameText.SetStringKey(stringKey);
            }
        }

        private void RefreshValue()
        {
            if (_currencyName == CurrencyNames.None)
            {
                SetValue(0);
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null || profile.Currency == null)
            {
                SetValue(0);
                return;
            }

            int amount = profile.Currency.GetAmount(_currencyName);
            SetValue(amount);
        }

        private void SetValue(int amount)
        {
            if (_valueText == null)
            {
                return;
            }

            _valueText.SetText(amount.ToString("N0"));
        }

        //
    }
}