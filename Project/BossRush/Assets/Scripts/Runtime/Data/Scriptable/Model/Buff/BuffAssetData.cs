using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [Serializable]
    public partial class BuffAssetData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [GUIColor("GetBuffNameColor")]
        public BuffNames Name;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetBuffTargetTypeColor")]
        public BuffTargetTypes Target;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetBuffApplicationColor")]
        public BuffApplications Application;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetBuffTypeColor")]
        public BuffTypes Type;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetStateEffectColor")]
        [EnableIf("Type", BuffTypes.StateEffect)]
        public StateEffects StateEffect;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetBuffNameColor")]
        [Tooltip("버프가 추가될 때 호환불가 버프를 삭제합니다.")]
        [SuffixLabel("호환불가 버프*")]
        public BuffNames Incompatible;

        [EnableIf("IsChangingAsset")]
        [Tooltip("버프가 추가될 때 호환불가한 상태이상에 해당하는 모든 버프를 삭제합니다.")]
        [SuffixLabel("호환불가 상태이상*")]
        [GUIColor("GetStateEffectColor")]
        public StateEffects IncompatibleStateEffect;

        //

        [SuffixLabel("특수 제작 프리펩 사용 (In Resources Folder)")]
        [GUIColor("GetBoolColor")]
        public bool UseSpawnCustomPrefab;

        [SuffixLabel("제작 프리펩 사용 시 포지션 초기화 여부")]
        [EnableIf("UseSpawnCustomPrefab")]
        [GUIColor("GetBoolColor")]
        public bool InitBuffEntityPositionZero;

        [SuffixLabel("제작 프리펩 사용 시 시전자 위치 고정 여부")]
        [EnableIf("UseSpawnCustomPrefab")]
        [GUIColor("GetBoolColor")]
        public bool InitBuffEntityPositionCaster;

        [SuffixLabel("오너가 사망했을때도 발동할지 여부")]
        [GUIColor("GetBoolColor")]
        public bool IgnoreCheckOwnerAlive;

        [SuffixLabel("실행중인 버프가 중첩될때 레벨을 초기화 시키지 않을지 여부")]
        [GUIColor("GetBoolColor")]
        [EnableIf("@Application == BuffApplications.Overlap")]
        public bool IgnoreResetLevel;

        #region Level

        [FoldoutGroup("#Level")]
        [GUIColor("GetIntColor")]
        public int MaxLevel;

        #endregion Level

        #region Stack

        [FoldoutGroup("#Stack")]
        [EnableIf("@Type == BuffTypes.Stat || Type == BuffTypes.Trigger || Type == BuffTypes.StateEffect")]
        [GUIColor("GetIntColor")]
        public int MaxStack;

        [FoldoutGroup("#Stack")]
        [EnableIf("@Type == BuffTypes.Stat || Type == BuffTypes.Trigger || Type == BuffTypes.StateEffect")]
        [GUIColor("GetIntColor")]
        public int MaxStackByLevel;

        [FoldoutGroup("#Stack/Range")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("전설 장비 옵션으로 설정되는 최대 스택의 최소 범위")]
        public int MaxStackOptionMinRange;

        [FoldoutGroup("#Stack/Range")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("전설 장비 옵션으로 설정되는 최대 스택의 최대 범위")]
        public int MaxStackOptionMaxRange;

        [FoldoutGroup("#Stack/Range")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("전설 장비 옵션에 의해 설정되는 최대 스택 증가 단위")]
        public int MaxStackOptionStep;

        [FoldoutGroup("#Stack")]
        [GUIColor("GetStatNameColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("능력치에 따른 최대 스택")]
        public StatNames MaxStackByStat;

        [FoldoutGroup("#Stack")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("능력치에 따라 최대 스택 (TRUE: 증가, FALSE: 설정)")]
        public bool IsAddMaxStackByStat;

        [FoldoutGroup("#Stack")]
        [EnableIf("@Type == BuffTypes.Stat || Type == BuffTypes.Trigger || StateEffect == StateEffects.Jolted || StateEffect == StateEffects.Chilled")]
        [GUIColor("GetFloatColor")]
        public float ReleaseTimeByStack;

        [FoldoutGroup("#Stack")]
        [GUIColor("GetStatApplicationsByStackColor")]
        [EnableIf("IsChangingAsset")]
        public StatApplicationsByStack StatApplicationByStack;

        /// <summary> 지난 시간에 따라 버프의 스택을 적용합니다. (1초:1스택) </summary>
        [FoldoutGroup("#Stack")]
        [SuffixLabel("버프 적용시 지난 시간만큼 스택을 추가합니다.")]
        public bool SetStackByElapsedTimeOnApply;

        [FoldoutGroup("#Stack")]
        [SuffixLabel("최대 스택 도달시 버프 삭제")]
        public bool RemoveBuffOnMaxStack;

        [FoldoutGroup("#Stack")]
        [SuffixLabel("스택 변경에 따라 Soliloquy 사용 안함")]
        public bool BlockSpawnSoliloquyOnStackChanged;

        #endregion Stack

        #region Time

        [FoldoutGroup("#Time")]
        [SuffixLabel("조건부 지속 버프")]
        public bool ConditionalDuration;

        [FoldoutGroup("#Time")]
        [SuffixLabel("스택이 오를 때 지난 시간을 초기화하지 않습니다")]
        public bool IgnoreElapsedTimeResetOnOverlap;

        [FoldoutGroup("#Time")]
        [GUIColor("GetFloatColor")]
        public float DelayTime;

        [FoldoutGroup("#Time")]
        [GUIColor("GetFloatColor")]
        public float Duration;

        [FoldoutGroup("#Time")]
        [GUIColor("GetFloatColor")]
        public float DurationByLevel;

        [FoldoutGroup("#Time")]
        [GUIColor("GetFloatColor")]
        public float DurationByStack;

        [FoldoutGroup("#Time")]
        [GUIColor("GetStatNameColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("능력치에 따라 지속시간 증가")]
        public StatNames DurationByStat;

        #region Time-Range

        [FoldoutGroup("#Time/Range")]
        [GUIColor("GetFloatColor")]
        public float MinDuration;

        [FoldoutGroup("#Time/Range")]
        [GUIColor("GetFloatColor")]
        public float MaxDuration;

        [FoldoutGroup("#Time/Range")]
        [GUIColor("GetFloatColor")]
        public float GrowDuration;

        #endregion Time-Range

        [FoldoutGroup("#Time")]
        [GUIColor("GetFloatColor")]
        public float Interval;

        [FoldoutGroup("#Time")]
        [GUIColor("GetFloatColor")]
        public float RestTime;

        [FoldoutGroup("#Time")]
        [GUIColor("GetFloatColor")]
        public float RestTimeByLevel;

        #endregion Time

        //──────────────────────────────────────────────────────────────────────────────────────────

        #region Apply

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#Apply")]
        [GUIColor("GetHitmarkColor")]
        public HitmarkNames Hitmark;

        [NonSerialized]
        public HitmarkAssetData HitmarkAssetData;

        #region 버프 능력치 (Stat)

        [FoldoutGroup("#Apply")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("능력치의 값이 음수일 때 붉은 색 표기")]
        public bool UseStatRedColor;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#Apply")]
        [GUIColor("GetStatNamesColor")]
        public StatNames[] Stats;

        [FoldoutGroup("#Apply")]
        [GUIColor("GetStatValuesColor")]
        public float[] StatValues;

        [FoldoutGroup("#Apply")]
        [GUIColor("GetStatValuesByLevelColor")]
        public float[] StatValuesByLevel;

        [FoldoutGroup("#Apply")]
        [EnableIf("IsChangingAsset")]
        public LinkedBuffStatTypes[] LinkedBuffStatTypes;

        [FoldoutGroup("#Apply")]
        [GUIColor("GetLinkedBuffStatDivisorsColor")]
        public float[] LinkedBuffStatDivisors;

        #region 전설 장비 옵션 (Legendary Equipment Option)

        [FoldoutGroup("#Apply/#Range")]
        [GUIColor("GetMinStatValuesColor")]
        public float[] MinStatValues;

        [FoldoutGroup("#Apply/#Range")]
        [GUIColor("GetMaxStatValuesColor")]
        public float[] MaxStatValues;

        [FoldoutGroup("#Apply/#Range")]
        [GUIColor("GetStatValuesIncreaseInRangeColor")]
        public float[] StatValuesIncreaseInRange;

        #endregion 전설 장비 옵션 (Legendary Equipment Option)

        #endregion 버프 능력치 (Stat)

        #region 버프 발동 (Trigger)

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#Apply-Trigger")]
        public TriggerTypes TriggerType;

        [FoldoutGroup("#Apply-Trigger")]
        [DisableIf("TriggerType", TriggerTypes.None)]
        public float TriggerValue;

        [FoldoutGroup("#Apply-Trigger")]
        [DisableIf("TriggerType", TriggerTypes.None)]
        public float TriggerValueRatio;

        [FoldoutGroup("#Apply-Trigger")]
        [DisableIf("TriggerType", TriggerTypes.None)]
        public bool SaveTriggerValue;

        #endregion 버프 발동 (Trigger)

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetBuffNameColor")]
        [FoldoutGroup("#Apply")]
        [SuffixLabel("버프가 끝났을 때 새롭게 추가되는 버프")]
        public BuffNames BuffOnRelease;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#Apply")]
        [SuffixLabel("버프가 끝났을 때 꺼줄 버프")]
        public BuffNames[] DeactiveBuffs;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#Apply")]
        [SuffixLabel("버프가 끝났을 때 꺼줄 버프타입")]
        public StateEffects[] DeactiveStateEffects;

        #endregion Apply

        #region Effect

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#Effect")]
        public SoundNames SFXName;

        [FoldoutGroup("#Effect")]
        [GUIColor("GetBoolColor")]
        public bool IsLoopSFX;

        #endregion Effect

        [FoldoutGroup("#Animation")]
        public string AnimationBool;

        [FoldoutGroup("#Animation")]
        [GUIColor("GetBoolColor")]
        public bool RemoveOnStopAnimation;

        [FoldoutGroup("#UI")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("자세한 버프 보기 옵션 없이도 버프를 HUD에 표시합니다")]
        public bool ShowBuffIcon;

        [FoldoutGroup("#UI")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("어떤 경우에도 버프를 HUD에 표시하지 않습니다")]
        public bool HideBuffIcon;

        [FoldoutGroup("#UI")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("버프 UI에서 스택 텍스트를 표시하지 않습니다")]
        public bool HideStackText;

        //

        [FoldoutGroup("#String")] public string TargetAsString;
        [FoldoutGroup("#String")] public string ApplicationAsString;
        [FoldoutGroup("#String")] public string TypeAsString;
        [FoldoutGroup("#String")] public string StateEffectAsString;
        [FoldoutGroup("#String")] public string IncompatibleAsString;
        [FoldoutGroup("#String")] public string IncompatibleStateEffectAsString;

        [FoldoutGroup("#String")] public string MaxStackByStatAsString;

        [FoldoutGroup("#String")] public string DurationByStatAsString;

        [FoldoutGroup("#String")] public string HitmarkAsString;
        [FoldoutGroup("#String")] public string[] StatsAsString;
        [FoldoutGroup("#String")] public string[] LinkedBuffStatTypesAsString;
        [FoldoutGroup("#String")] public string TriggerAsString;
        [FoldoutGroup("#String")] public string BuffOnReleaseAsString;
        [FoldoutGroup("#String")] public string[] DeactiveBuffsAsString;
        [FoldoutGroup("#String")] public string[] DeactiveStateEffectAsString;
        [FoldoutGroup("#String")] public string SFXNameAsString;
    }
}