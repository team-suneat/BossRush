using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class BuffAssetData : ScriptableData<int>
    {
        [SuffixLabel("개별 에셋 변경 모드")]
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("버프 이름")]
        public BuffName Name;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("버프 타입")]
        [GUIColor("GetBuffTypeColor")]
        public BuffType Type;

        [FoldoutGroup("#버프 정보")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("지속시간(초)")]
        public float Duration;

        [FoldoutGroup("#버프 정보")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("DoT용 주기(초, 0이면 1회 적용)")]
        public float Interval;

        [FoldoutGroup("#버프 정보")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("스탯 증가량 또는 초당 피해량")]
        public float Value;

        [FoldoutGroup("#버프 정보")]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetStatColor")]
        [SuffixLabel("스탯 버프용")]
        public StatNames Stat;

        [FoldoutGroup("#버프 정보")]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetStateEffectColor")]
        [SuffixLabel("스턴 등 상태이상용")]
        public StateEffects State;

        // 스트링
        [FoldoutGroup("#String")] public string TypeString;
        [FoldoutGroup("#String")] public string StatString;
        [FoldoutGroup("#String")] public string StateString;

        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public void Validate()
        {
            if (!IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref Type, TypeString))
                {
                    Log.Error("BuffAssetData의 TypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), TypeString);
                }
                if (!EnumEx.ConvertTo(ref Stat, StatString))
                {
                    Log.Error("BuffAssetData의 StatString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), StatString);
                }
                if (!EnumEx.ConvertTo(ref State, StateString))
                {
                    Log.Error("BuffAssetData의 StateString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), StateString);
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            TypeString = Type.ToString();
            StatString = Stat.ToString();
            StateString = State.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            TypeLog();
        }

        public BuffAssetData Clone()
        {
            BuffAssetData clone = new()
            {
                Name = Name,
                Type = Type,
                Duration = Duration,
                Interval = Interval,
                Value = Value,
                Stat = Stat,
                State = State,
            };

            return clone;
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref TypeString, Type);
            UpdateIfChanged(ref StatString, Stat);
            UpdateIfChanged(ref StateString, State);

            return _hasChangedWhiteRefreshAll;
        }

        private bool _hasChangedWhiteRefreshAll = false;

        private void UpdateIfChanged<TEnum>(ref string target, TEnum newValue) where TEnum : System.Enum
        {
            string newString = newValue?.ToString();
            if (target != newString)
            {
                target = newString;
                _hasChangedWhiteRefreshAll = true;
            }
        }

        private void TypeLog()
        {
            if (Type == BuffType.None)
            {
                Log.Warning("BuffAssetData의 Type이 올바르지 않을 수 있습니다. Name:{0}, {1}", Name.ToLogString(), Type);
            }
        }

#endif
    }
}