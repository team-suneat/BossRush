using Sirenix.OdinInspector;
using System;

namespace TeamSuneat.Data
{
    [Serializable]
    public partial class SkillCardUnlockAssetData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetSkillNameColor")]
        public SkillNames SkillName;

        [GUIColor("GetIntColor")]
        [SuffixLabel("해금 레벨")]
        public int UnlockLevel;

        [FoldoutGroup("#RequiredItem")]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetItemNameColor")]
        [SuffixLabel("필요 아이템 (None 가능)")]
        [InfoBox("스킬 학습에 필요한 아이템입니다. 무기/악세사리 또는 속성석을 사용합니다.")]
        public ItemNames RequiredItemName = ItemNames.None;

        [FoldoutGroup("#RequiredItem")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("필요 개수")]
        [ShowIf("RequiredItemName", ItemNames.None, false)]
        public int RequiredItemCount;

        [FoldoutGroup("#RequiredCurrency")]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetCurrencyNameColor")]
        [SuffixLabel("필요 속성석 (None 가능)")]
        [InfoBox("스킬 학습에 필요한 속성석입니다. CurrencyNames enum을 사용합니다.")]
        public CurrencyNames RequiredCurrencyName = CurrencyNames.None;

        [FoldoutGroup("#RequiredCurrency")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("필요 개수")]
        [ShowIf("RequiredCurrencyName", CurrencyNames.None, false)]
        public int RequiredCurrencyCount;

        #region String

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string SkillNameAsString;

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string RequiredItemNameAsString;

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string RequiredCurrencyNameAsString;

        #endregion String

        public override void OnLoadData()
        {
            base.OnLoadData();

            Validate();
        }

        public void Validate()
        {
            if (IsChangingAsset)
            {
                return;
            }

            if (!EnumEx.ConvertTo(ref SkillName, SkillNameAsString))
            {
                Log.Error("SkillCardUnlockAssetData에서 SkillName enum 변환이 실패했습니다. {0}", SkillNameAsString);
            }
            if (!EnumEx.ConvertTo(ref RequiredItemName, RequiredItemNameAsString))
            {
                Log.Error("SkillCardUnlockAssetData에서 RequiredItemName enum 변환이 실패했습니다. {0}", RequiredItemNameAsString);
            }
            if (!EnumEx.ConvertTo(ref RequiredCurrencyName, RequiredCurrencyNameAsString))
            {
                Log.Error("SkillCardUnlockAssetData에서 RequiredCurrencyName enum 변환이 실패했습니다. {0}", RequiredCurrencyNameAsString);
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            SkillNameAsString = SkillName.ToString();
            RequiredItemNameAsString = RequiredItemName.ToString();
            RequiredCurrencyNameAsString = RequiredCurrencyName.ToString();

            IsChangingAsset = false;
        }

#if UNITY_EDITOR

        private bool _hasChangedWhiteRefreshAll = false;

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref SkillNameAsString, SkillName);
            UpdateIfChanged(ref RequiredItemNameAsString, RequiredItemName);
            UpdateIfChanged(ref RequiredCurrencyNameAsString, RequiredCurrencyName);

            IsChangingAsset = false;

            return _hasChangedWhiteRefreshAll;
        }

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
