using System;
using TeamSuneat;

namespace TeamSuneat.Data
{
    public partial class CharacterAssetData
    {
        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public void Validate()
        {
            if (IsChangingAsset)
            {
                return;
            }

            if (!EnumEx.ConvertTo(ref Name, NameAsString))
            {
                Log.Error("Character Asset 내 Name 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            NameAsString = Name.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            CustomLog();
        }

        public CharacterAssetData Clone()
        {
            CharacterAssetData assetData = new()
            {
                Name = Name,
                IsChangingAsset = IsChangingAsset,
                NameAsString = NameAsString
            };

            return assetData;
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref NameAsString, Name);

            IsChangingAsset = false;

            return _hasChangedWhiteRefreshAll;
        }

        private bool _hasChangedWhiteRefreshAll = false;

        private void UpdateIfChanged<TEnum>(ref string target, TEnum newValue) where TEnum : Enum
        {
            string newString = newValue?.ToString();
            if (target != newString)
            {
                target = newString;
                _hasChangedWhiteRefreshAll = true;
            }
        }

#endif
    }
}