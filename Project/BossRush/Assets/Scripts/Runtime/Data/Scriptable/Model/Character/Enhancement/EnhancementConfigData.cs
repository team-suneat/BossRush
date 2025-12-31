using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 강화 시스템 개별 능력치 데이터
    /// </summary>
    [Serializable]
    public class EnhancementConfigData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetStatNameColor")]
        [Tooltip("능력치 종류")]
        public StatNames StatName;

        [Tooltip("최대 레벨")]
        public int MaxLevel;

        [Tooltip("능력치 성장값 (레벨당 증가량)")]
        public float GrowthValue;

        [Tooltip("초기 비용")]
        public int InitialCost;

        [Tooltip("비용 성장률 (레벨당 비용 증가 배율)")]
        public float CostGrowthRate;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetStatNameColor")]
        [Tooltip("요구 능력치 (None이면 요구사항 없음)")]
        public StatNames RequiredStatName = StatNames.None;

        [Tooltip("요구 능력치 레벨 (0이면 요구사항 없음)")]
        public int RequiredStatLevel = 0;

        #region String

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string StatNameAsString;

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string RequiredStatNameAsString;

        #endregion String

        //──────────────────────────────────────────────────────────────────────────────────────────

        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(StatName);
        }

        public void Validate()
        {
            if (IsChangingAsset)
            {
                return;
            }

            if (!EnumEx.ConvertTo(ref StatName, StatNameAsString))
            {
                Log.Error("강화 시스템 데이터 내 StatName 변수 변환에 실패했습니다. {0}", StatName.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref RequiredStatName, RequiredStatNameAsString))
            {
                Log.Error("강화 시스템 데이터 내 RequiredStatName 변수 변환에 실패했습니다. {0}", RequiredStatName.ToLogString());
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            StatNameAsString = StatName.ToString();
            RequiredStatNameAsString = RequiredStatName.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();
            CustomLog();
        }

        public float CalculateStatValue(int level)
        {
            // 능력치 값 = 레벨 × 성장값
            return level * GrowthValue;
        }


        public bool HasRequirement => RequiredStatName != StatNames.None && RequiredStatLevel > 0;

#if UNITY_EDITOR

        private bool _hasChangedWhiteRefreshAll = false;

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref StatNameAsString, StatName);
            UpdateIfChanged(ref RequiredStatNameAsString, RequiredStatName);

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
            if (StatName == StatNames.None)
            {
                Log.Error("강화 시스템 데이터의 능력치 이름이 설정되지 않았습니다: {0}", StatName);
            }
            if (MaxLevel == 0)
            {
                Log.Error("강화 시스템 데이터의 최대 레벨이 설정되지 않았습니다: {0}", StatName);
            }
        }

#endif
    }
}

