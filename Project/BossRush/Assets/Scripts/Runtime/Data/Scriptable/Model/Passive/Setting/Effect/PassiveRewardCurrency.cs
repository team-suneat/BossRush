using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    public class PassiveRewardCurrency : XScriptableObject
    {
        public bool IsChangingAsset;
        public PassiveNames Name;

        [GUIColor("GetCurrenciesColor")]
        [EnableIf("IsChangingAsset")]
        public CurrencyNames[] Currencies;

        [GUIColor("GetIntColor")]
        public int Amount;

        [GUIColor("GetFloatColor")]
        public float Rate;

        [FoldoutGroup("#String")]
        public string[] CurrenciesAsString;

        public override void Rename()
        {
            Rename("RewardCurrency");
        }

        public override void Validate()
        {
            if (!IsChangingAsset)
            {
                _ = EnumEx.ConvertTo(ref Name, NameString);
                _ = EnumEx.ConvertTo(ref Currencies, CurrenciesAsString);
            }
        }

        public override void Refresh()
        {
            NameString = Name.ToString();
            CurrenciesAsString = Currencies.ToStringArray();
            IsChangingAsset = false;
            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref NameString, Name);
            UpdateIfChangedArray(ref CurrenciesAsString, Currencies.ToStringArray());

            base.RefreshWithoutSave();
            IsChangingAsset = false;
            return _hasChangedWhiteRefreshAll;
        }

        public void Apply(Transform parent)
        {
            if (Currencies.IsValid())
            {
                foreach (CurrencyNames currencyName in Currencies)
                {
                    Apply(parent, currencyName);
                }
            }
        }

        public void Apply(Transform parent, CurrencyNames currencyName)
        {
            Game.VProfile profileInfo = GameApp.GetSelectedProfile();
            if (currencyName != CurrencyNames.None)
            {
                if (Amount > 0)
                {
                    profileInfo.Currency.Add(currencyName, Amount);
                    _ = ResourcesManager.SpawnCurrencyFloatyText(currencyName, Amount, parent);
                }
                else if (Amount < 0)
                {
                    int useAmount = Mathf.Abs(Amount);
                    int myAmount = profileInfo.Currency.GetAmount(currencyName);
                    if (useAmount > myAmount)
                    {
                        useAmount = myAmount;
                    }

                    profileInfo.Currency.Use(currencyName, useAmount);
                    _ = ResourcesManager.SpawnCurrencyFloatyText(currencyName, -useAmount, parent);
                }

                if (Rate > 0)
                {
                    int useAmount = Mathf.RoundToInt(profileInfo.Currency.GetAmount(currencyName) * Rate);
                    profileInfo.Currency.Add(currencyName, useAmount);
                    _ = ResourcesManager.SpawnCurrencyFloatyText(currencyName, useAmount, parent);
                }
                else if (Rate < 0)
                {
                    int useAmount = Mathf.RoundToInt(profileInfo.Currency.GetAmount(currencyName) * Mathf.Abs(Rate));
                    int myAmount = profileInfo.Currency.GetAmount(currencyName);
                    if (useAmount > myAmount)
                    {
                        useAmount = myAmount;
                    }

                    profileInfo.Currency.Use(currencyName, useAmount);
                    _ = ResourcesManager.SpawnCurrencyFloatyText(currencyName, -useAmount, parent);
                }
            }
        }

        public void Compare(PassiveRewardCurrency[] others)
        {
            if (others.IsValid())
            {
                Currencies = new CurrencyNames[others.Length];
                for (int i = 0; i < others.Length; i++)
                {
                    PassiveRewardCurrency other = others[i];
                    for (int j = 0; j < Currencies.Length; j++)
                    {
                        if (Currencies[j] != other.Currencies[j])
                        {
                            Log.Warning($"{Currencies[j]}!= {other.Currencies[j]}");
                        }
                    }

                    if (Amount != other.Amount)
                    {
                        Log.Warning($"{Amount} != {other.Amount}");
                    }

                    if (Rate != other.Rate)
                    {
                        Log.Warning($"{Rate} != {other.Rate}");
                    }
                }
            }
        }

        private Color GetCurrenciesColor()
        {
            if (Currencies.IsValid())
            {
                return GameColors.GreenYellow;
            }

            return GameColors.BestRed;
        }

        public string GetString()
        {
            StringBuilder sb = new StringBuilder();

            // int 타입 필드들 (0이 아닐 때만 포함)
            if (Amount != 0) sb.Append($"재화 수량: {Amount}, ");

            // float 타입 필드들 (0이 아닐 때만 포함)
            if (Rate != 0f) sb.Append($"재화 비율: {ValueStringEx.GetPercentString(Rate)}, ");

            // 배열 필드들 (유효한 배열일 때만 포함)
            if (Currencies.IsValidArray()) sb.Append($"재화 종류: {string.Join(", ", Currencies.JoinToString())}, ");

            // 마지막 쉼표와 공백 제거
            if (sb.Length > 2)
            {
                sb.Length -= 2; // 마지막 ", " 제거
            }

            return sb.ToString();
        }
    }
}