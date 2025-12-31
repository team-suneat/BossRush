using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 성장 시스템 개별 능력치 데이터
    /// </summary>
    [Serializable]
    public class GrowthConfigData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetGrowthTypeColor")]
        [Tooltip("성장 타입 종류")]
        public CharacterGrowthTypes GrowthType;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetStatNameColor")]
        [Tooltip("능력치 종류")]
        public StatNames StatName;

        [Tooltip("최대 레벨")]
        public int MaxLevel;

        [Tooltip("능력치 레벨당 능력치 증가량")]
        public float StatIncreasePerLevel;

        #region String

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string GrowthTypeAsString;

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string StatNameAsString;

        #endregion String

        //──────────────────────────────────────────────────────────────────────────────────────────

        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(StatName);
        }

        public int TID => BitConvert.Enum32ToInt(GrowthType);

        public void Validate()
        {
            if (IsChangingAsset)
            {
                return;
            }

            if (!EnumEx.ConvertTo(ref GrowthType, GrowthTypeAsString))
            {
                Log.Error("Growth Config Data 내 GrowthType 변수 변환에 실패했습니다. {0}", StatName.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref StatName, StatNameAsString))
            {
                Log.Error("Growth Config Data 내 StatName 변수 변환에 실패했습니다. {0}", GrowthType.ToLogString());
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            GrowthTypeAsString = GrowthType.ToString();
            StatNameAsString = StatName.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();
            CustomLog();
        }

        public float CalculateStatValue(int level)
        {
            return level * StatIncreasePerLevel;
        }



#if UNITY_EDITOR

        private bool _hasChangedWhiteRefreshAll = false;

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref GrowthTypeAsString, GrowthType);
            UpdateIfChanged(ref StatNameAsString, StatName);

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

        private void CustomLog()
        {
            if (GrowthType == CharacterGrowthTypes.None)
            {
                Log.Error("성장 시스템 데이터의 성장 타입이 설정되지 않았습니다: {0}", StatName);
            }
            if (StatName == StatNames.None)
            {
                Log.Error("성장 시스템 데이터의 능력치 이름이 설정되지 않았습니다: {0}", GrowthType);
            }
            if (MaxLevel == 0)
            {
                Log.Error("성장 시스템 데이터의 최대 레벨이 설정되지 않았습니다: {0}", StatName);
            }
        }

        protected Color GetGrowthTypeColor(CharacterGrowthTypes key)
        {
            return GetFieldColor(key);
        }

#endif
    }
}