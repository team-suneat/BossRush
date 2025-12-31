using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // HUD 재화 버튼 컴포넌트 - 버튼 클릭 시 해당 재화를 획득
    public class HUDCurrencyButton : UIButton
    {
        [Title("#HUDCurrencyButton")]
        [SerializeField] private CurrencyNames _currencyName;
        [SerializeField] private string _currencyNameString;
        [SerializeField] private int _earnAmount = 1000;

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
                SetGameObjectName($"HUDCurrencyButton({_currencyName})");
            }
            else
            {
                SetGameObjectName($"HUDCurrencyButton");
            }
        }

        protected override void OnButtonClick()
        {
            base.OnButtonClick();
            AddCurrency();
        }

        protected override void OnButtonHold()
        {
            base.OnButtonHold();
            AddCurrency();
        }

        private void AddCurrency()
        {
            if (_currencyName == CurrencyNames.None)
            {
                Log.Warning(LogTags.UI, "재화 타입이 설정되지 않았습니다.");
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null || profile.Currency == null)
            {
                Log.Warning(LogTags.UI, "프로필 또는 재화 시스템을 찾을 수 없습니다.");
                return;
            }

            profile.Currency.Add(_currencyName, _earnAmount);
            Log.Info(LogTags.UI, "{0} {1}개를 획득했습니다.", _currencyName, _earnAmount);
        }
    }
}