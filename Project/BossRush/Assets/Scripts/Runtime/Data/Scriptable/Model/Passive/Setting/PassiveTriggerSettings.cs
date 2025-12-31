// TriggerSettings.cs
using Sirenix.OdinInspector;
using System;
using System.Text;
using TeamSuneat.Passive;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    public enum PassiveTriggerCondition
    {
        None,           // 아무 것도 하지 않음
        AttemptTrigger, // 발동을 시도함 (성공할 수도 실패할 수도 있음)
        ForceTrigger    // 강제로 발동함
    }

    [CreateAssetMenu(fileName = "Trigger", menuName = "TeamSuneat/Scriptable/Passive Setting/TriggerSettings")]
    public class PassiveTriggerSettings : XScriptableObject
    {
        #region Field

        public bool IsChangingAsset;
        public PassiveNames Name;

        // 기본 조건
        [FoldoutGroup("패시브 발동 조건")]
        [GUIColor("GetPassiveTriggerColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건")]
        [InfoBox("$TriggerMessege")]
        public PassiveTriggers Trigger;

        [FoldoutGroup("패시브 발동 조건")]
        [GUIColor("GetPassiveTriggerConditionColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("생성시 패시브 발동")]
        public PassiveTriggerCondition InitTriggerCondition;

        [FoldoutGroup("패시브 발동 조건")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 검사 종류")]
        public TriggerCheckerTypes TriggerCheckType;

        [FoldoutGroup("패시브 발동 조건")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("최대 스택일때만 작동할지 여부*")]
        public bool TriggerMaxStack;

        [FoldoutGroup("패시브 발동 조건")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("패시브 발동 시 유휴 상태를 확인할지 여부")]
        public bool TriggerCheckPassiveRestTime;

        [FoldoutGroup("패시브 발동 조건")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("패시브 역실행 검사시 조건 발동을 실행검사와 똑같이 확인할지 여부")]
        public bool IsReleaseCheckSameBuffType;

        // 기술

        [FoldoutGroup("패시브 발동 조건/기술")]
        [GUIColor("GetTriggerSkillElementsColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 기술 속성들")]
        public GameElements[] TriggerSkillElements;

        // 피해
        [FoldoutGroup("패시브 발동 조건/피해")]
        [GUIColor("GetTriggerDamageTypesColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 피해 종류")]
        public DamageTypes[] TriggerDamageTypes;

        [FoldoutGroup("패시브 발동 조건/피해")]
        [GUIColor("GetTriggerHitmarksColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 히트마크")]
        public HitmarkNames[] TriggerHitmarks;

        [FoldoutGroup("패시브 발동 조건/피해")]
        [GUIColor("GetTriggerIgnoreHitmarksColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 무시 히트마크")]
        public HitmarkNames[] TriggerIgnoreHitmarks;

        [FoldoutGroup("패시브 발동 조건/피해")]
        [GUIColor("GetTriggerIgnoreDamageTypesColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 무시 피해 종류들")]
        public DamageTypes[] TriggerIgnoreDamageTypes;

        // 대상
        [FoldoutGroup("패시브 발동 조건/대상")]
        [GUIColor("GetPassiveTargetTypeColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 소유 캐릭터")]
        public PassiveTargetTypes TriggerOwner;

        [FoldoutGroup("패시브 발동 조건/대상")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 능력치")]
        [GUIColor("GetTriggerStatsColor")]
        public StatNames[] TriggerStats;

        [FoldoutGroup("패시브 발동 조건/대상")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건 아이템 등급")]
        [GUIColor("GetTriggerGradesColor")]
        public GradeNames[] TriggerGrades;

        // 환경 조건
        [FoldoutGroup("패시브 발동 조건/환경 조건")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건 맵 오브젝트")]
        [GUIColor("GetTriggerMapObjectsColor")]
        public MapObjectNames[] TriggerMapObjects;

        [FoldoutGroup("패시브 발동 조건/환경 조건")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건 맵 지역")]
        [GUIColor("GetTriggerMapTypesColor")]
        public MapTypes[] TriggerMapTypes;

        [FoldoutGroup("패시브 발동 조건/환경 조건")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건 재화")]
        [GUIColor("GetTriggerCurrenciesColor")]
        public CurrencyNames[] TriggerCurrencies;

        // 버프 및 상태이상
        [FoldoutGroup("패시브 발동 조건/버프 및 상태이상")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 버프")]
        [GUIColor("GetTriggerBuffsColor")]
        public BuffNames[] TriggerBuffs;

        [FoldoutGroup("패시브 발동 조건/버프 및 상태이상")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 버프 종류")]
        [GUIColor("GetBuffTypeColor")]
        public BuffTypes TriggerBuffType;

        [FoldoutGroup("패시브 발동 조건/버프 및 상태이상")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 무시 버프 종류")]
        [GUIColor("GetBuffTypeColor")]
        public BuffTypes TriggerIgnoreBuffOnHit;

        [FoldoutGroup("패시브 발동 조건/버프 및 상태이상")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 상태이상들")]
        [GUIColor("GetTriggerStateEffectsColor")]
        public StateEffects[] TriggerStateEffects;

        [FoldoutGroup("패시브 발동 조건/버프 및 상태이상")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 무시 상태이상")]
        [GUIColor("GetStateEffectColor")]
        public StateEffects TriggerIgnoreStateEffectOnHit;

        [FoldoutGroup("패시브 발동 조건/버프 및 상태이상")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건 몬스터의 상태이상")]
        [GUIColor("GetTriggerMonsterCountStateEffectsColor")]
        public StateEffects[] TriggerMonsterCountStateEffects;

        [FoldoutGroup("패시브 발동 조건/버프 및 상태이상")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("서로 다른 속성 상태이상의 수")]
        public int DifferentElementalStateEffectCount;

        [FoldoutGroup("패시브 발동 조건/버프 및 상태이상")]
        [SuffixLabel("서로 다른 속성 상태이상의 범위 연산자")]
        public PassiveOperator DifferentElementalStateEffectOperator;

        // 자원 조건
        [FoldoutGroup("패시브 발동 조건/자원 조건")]
        public VitalResourceTypes TriggerResourceType;

        [FoldoutGroup("패시브 발동 조건/자원 조건")]
        public PassiveOperator TriggerResourceOperator;

        [FoldoutGroup("패시브 발동 조건/자원 조건")]
        public int TriggerResourceValue;

        [FoldoutGroup("패시브 발동 조건/자원 조건")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건 자원 비율 비교 연산자")]
        public PassiveOperator TriggerOperator;

        [FoldoutGroup("패시브 발동 조건/자원 조건")]
        [GUIColor("GetFloatColor")]
        [Range(0f, 1f)]
        [SuffixLabel("패시브 발동 조건 자원 비율")]
        public float TriggerPercent;

        // 능력치 조건
        [FoldoutGroup("패시브 발동 조건/능력치 조건")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건 능력치")]
        [GUIColor("GetTriggerOperatorStatsColor")]
        public StatNames[] TriggerOperatorStats;

        [FoldoutGroup("패시브 발동 조건/능력치 조건")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건 능력치 비율 범위 연산자")]
        public PassiveOperator TriggerStatOperator;

        [FoldoutGroup("패시브 발동 조건/능력치 조건")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("패시브 발동 조건 능력치 비율")]
        public int TriggerStatValue;

        // 몬스터 조건
        [FoldoutGroup("패시브 발동 조건/몬스터 조건")]
        [GUIColor("GetIntColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 조건 몬스터 수")]
        public int TriggerMonsterCount;

        [FoldoutGroup("패시브 발동 조건/몬스터 조건")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 몬스터 수 비교 연산자")]
        public PassiveOperator TriggerMonsterCountOperator;

        [FoldoutGroup("패시브 발동 조건/몬스터 조건")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("패시브 발동 몬스터와의  거리")]
        public float TriggerMonsterRange;

        // 발동 횟수
        [FoldoutGroup("패시브 발동 조건/발동 횟수")]
        [GUIColor("GetIntColor")]
        [Tooltip("설정된 발동 횟수만큼 발동했을 때 패시브를 적용합니다.")]
        [SuffixLabel("패시브 적용 제한 발동 횟수*")]
        public int TriggerCount;

        [FoldoutGroup("패시브 발동 조건/발동 횟수")]
        [GUIColor("GetBoolColor")]
        [Tooltip("적용 제한 발동 횟수를 스테이지 이동 시에 저장하지 않을지 여부입니다.")]
        [SuffixLabel("패시브 적용 제한 발동 횟수 저장하지 않을지 여부*")]
        public bool TriggerCountDontSave;

        // 확률

        [FoldoutGroup("패시브 발동 조건/확률 (Chance)")]
        [GUIColor("GetTriggerChanceColor")]
        [SuffixLabel("패시브 발동 확률 타입")]
        public PassiveTriggerChanceCalcType TriggerChanceCalcType;

        [FoldoutGroup("패시브 발동 조건/확률 (Chance)/Fixed")]
        [EnableIf("TriggerChanceCalcType", PassiveTriggerChanceCalcType.Fixed)]
        [GUIColor("GetFloatColor")]
        [Range(0, 1)]
        [SuffixLabel("패시브 발동 확률")]
        public float TriggerChance;

        [FoldoutGroup("패시브 발동 조건/확률 (Chance)/Fixed")]
        [EnableIf("TriggerChanceCalcType", PassiveTriggerChanceCalcType.Fixed)]
        [GUIColor("GetFloatColor")]
        [Range(0, 1)]
        [SuffixLabel("레벨별 패시브 발동 확률")]
        public float TriggerChanceByLevel;

        [FoldoutGroup("패시브 발동 조건/확률 (Chance)/FromStat")]
        [EnableIf("TriggerChanceCalcType", PassiveTriggerChanceCalcType.FromStat)]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 발동 확률 능력치")]
        [GUIColor("GetStatNameColor")]
        public StatNames TriggerChanceByStat;

        [FoldoutGroup("패시브 발동 조건/확률 (Chance)/FromOptionRange")]
        [EnableIf("TriggerChanceCalcType", PassiveTriggerChanceCalcType.FromOptionRange)]
        [GUIColor("GetFloatColor")]
        [Range(0, 1)]
        [SuffixLabel("전설 장비 옵션으로 설정되는 패시브 발동 확률의 최소 범위")]
        public float TriggerChanceOptionMinRange;

        [FoldoutGroup("패시브 발동 조건/확률 (Chance)/FromOptionRange")]
        [EnableIf("TriggerChanceCalcType", PassiveTriggerChanceCalcType.FromOptionRange)]
        [GUIColor("GetFloatColor")]
        [Range(0, 1)]
        [SuffixLabel("전설 장비 옵션으로 설정되는 패시브 발동 확률의 최대 범위")]
        public float TriggerChanceOptionMaxRange;

        [FoldoutGroup("패시브 발동 조건/확률 (Chance)/FromOptionRange")]
        [EnableIf("TriggerChanceCalcType", PassiveTriggerChanceCalcType.FromOptionRange)]
        [GUIColor("GetFloatColor")]
        [Range(0, 1)]
        [SuffixLabel("전설 장비 옵션에 의해 설정되는 패시브 발동 확률 증가 단위")]
        public float TriggerChanceOptionStep;

        //

        //

        #region String

        [HideInInspector]
        public string TriggerMessege;  // 발동 메시지 추가

        [FoldoutGroup("#String/Custom", 3)] public string TriggerAsString;
        [FoldoutGroup("#String/Custom", 3)] public string TriggerOwnerAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerDamageTypesAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerHitmarksAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerIgnoreHitmarksAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerStatsAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerGradesAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerMapObjectsAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerMapTypesAsString;

        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerCurrenciesAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerBuffsAsString;
        [FoldoutGroup("#String/Custom", 3)] public string TriggerBuffTypeAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerStateEffectsAsString;
        [FoldoutGroup("#String/Custom", 3)] public string TriggerIgnoreBuffOnHitAsString;
        [FoldoutGroup("#String/Custom", 3)] public string TriggerIgnoreStateEffectOnHitAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerIgnoreDamageTypesAsString;
        [FoldoutGroup("#String/Custom", 3)] public string TriggerCheckTypeAsString;
        [FoldoutGroup("#String/Custom", 3)] public string TriggerOperatorAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerMonsterCountStateEffectAsString;
        [FoldoutGroup("#String/Custom", 3)] public string TriggerMonsterCountOperatorAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] TriggerOperatorStatsAsString;
        [FoldoutGroup("#String/Custom", 3)] public string TriggerStatOperatorAsString;
        [FoldoutGroup("#String/Custom", 3)] public string DifferentElementalStateEffectOperatorAsString;

        [FoldoutGroup("#String/Custom", 3)] public string TriggerChanceByStatString;
        [FoldoutGroup("#String/Custom", 3)] public string TriggerChanceCalcTypeString;

        #endregion String

        #endregion Field

        public override void Rename()
        {
            Rename("Trigger");
        }

        public override void Validate()
        {
            if (!IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref Name, NameString))
                {
                    Log.Error("{0}({1}) 패시브의 NameString 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), NameString);
                }
                if (!EnumEx.ConvertTo(ref Trigger, TriggerAsString))
                {
                    Log.Error("{0}({1}) 패시브의 Trigger 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerAsString);
                }
                if (!EnumEx.ConvertTo(ref TriggerOwner, TriggerOwnerAsString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerTarget 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerOwnerAsString);
                }

                if (TriggerDamageTypesAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerDamageTypes, TriggerDamageTypesAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerDamageTypes 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerDamageTypesAsString.JoinToString());
                    }
                }
                if (TriggerHitmarksAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerHitmarks, TriggerHitmarksAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerHitmarks 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerHitmarksAsString.JoinToString());
                    }
                }
                if (TriggerIgnoreHitmarksAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerIgnoreHitmarks, TriggerIgnoreHitmarksAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerIgnoreHitmarks 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerIgnoreHitmarksAsString.JoinToString());
                    }
                }
                if (TriggerStatsAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerStats, TriggerStatsAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerStatsAsString 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerStatsAsString);
                    }
                }
                if (TriggerBuffsAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerBuffs, TriggerBuffsAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerBuffs 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerBuffsAsString);
                    }
                }
                if (!EnumEx.ConvertTo(ref TriggerBuffType, TriggerBuffTypeAsString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerBuffType 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerBuffTypeAsString);
                }
                if (TriggerStateEffectsAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerStateEffects, TriggerStateEffectsAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerStateEffects 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerStateEffectsAsString);
                    }
                }
                if (!EnumEx.ConvertTo(ref TriggerIgnoreBuffOnHit, TriggerIgnoreBuffOnHitAsString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerIgnoreBuffOnHit 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerIgnoreBuffOnHitAsString);
                }
                if (!EnumEx.ConvertTo(ref TriggerIgnoreStateEffectOnHit, TriggerIgnoreStateEffectOnHitAsString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerIgnoreStateEffectOnHit 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerIgnoreStateEffectOnHitAsString);
                }
                if (!EnumEx.ConvertTo(ref TriggerOperator, TriggerOperatorAsString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerOperator 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerOperatorAsString);
                }
                if (TriggerOperatorStatsAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerOperatorStats, TriggerOperatorStatsAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerOperatorStats 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerOperatorStatsAsString);
                    }
                }
                if (!EnumEx.ConvertTo(ref TriggerStatOperator, TriggerStatOperatorAsString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerStatOperator 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerStatOperatorAsString);
                }
                if (TriggerGradesAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerGrades, TriggerGradesAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerGrades 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerGradesAsString);
                    }
                }
                if (TriggerMapObjectsAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerMapObjects, TriggerMapObjectsAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerMapObjects 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerMapObjectsAsString);
                    }
                }
                if (TriggerMapTypesAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerMapTypes, TriggerMapTypesAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerMapTypes 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerMapTypesAsString);
                    }
                }

                if (TriggerCurrenciesAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerCurrencies, TriggerCurrenciesAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerCurrencies 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerCurrenciesAsString);
                    }
                }
                if (TriggerIgnoreDamageTypesAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref TriggerIgnoreDamageTypes, TriggerIgnoreDamageTypesAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 TriggerCurrencies 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerCurrenciesAsString);
                    }
                }
                if (!EnumEx.ConvertTo(ref TriggerCheckType, TriggerCheckTypeAsString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerCheckType 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerCheckTypeAsString);
                }
                if (!EnumEx.ConvertTo(ref TriggerMonsterCountStateEffects, TriggerMonsterCountStateEffectAsString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerMonsterCountStateEffectAsString 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerMonsterCountStateEffectAsString);
                }
                if (!EnumEx.ConvertTo(ref TriggerMonsterCountOperator, TriggerMonsterCountOperatorAsString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerMonsterCountOperator 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerMonsterCountOperatorAsString);
                }
                if (!EnumEx.ConvertTo(ref DifferentElementalStateEffectOperator, DifferentElementalStateEffectOperatorAsString))
                {
                    Log.Error("{0}({1}) 패시브의 DifferentElementalStateEffectOperator 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), DifferentElementalStateEffectOperatorAsString);
                }

                if (!EnumEx.ConvertTo(ref TriggerChanceCalcType, TriggerChanceCalcTypeString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerChanceCalcTypeString 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerChanceCalcTypeString);
                }
                if (!EnumEx.ConvertTo(ref TriggerChanceByStat, TriggerChanceByStatString))
                {
                    Log.Error("{0}({1}) 패시브의 TriggerChanceByStatString 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), TriggerChanceByStatString);
                }

                RefreshTriggerMassage();
            }
        }

        //

        public override void Refresh()
        {
            NameString = Name.ToString();
            TriggerAsString = Trigger.ToString();
            TriggerOwnerAsString = TriggerOwner.ToString();
            TriggerDamageTypesAsString = TriggerDamageTypes.ToStringArray();
            TriggerHitmarksAsString = TriggerHitmarks.ToStringArray();
            TriggerIgnoreHitmarksAsString = TriggerIgnoreHitmarks.ToStringArray();
            TriggerStatsAsString = TriggerStats.ToStringArray();
            TriggerBuffsAsString = TriggerBuffs.ToStringArray();
            TriggerBuffTypeAsString = TriggerBuffType.ToString();
            TriggerStateEffectsAsString = TriggerStateEffects.ToStringArray();
            TriggerIgnoreBuffOnHitAsString = TriggerIgnoreBuffOnHit.ToString();
            TriggerIgnoreStateEffectOnHitAsString = TriggerIgnoreStateEffectOnHit.ToString();
            TriggerOperatorAsString = TriggerOperator.ToString();
            TriggerOperatorStatsAsString = TriggerOperatorStats.ToStringArray();
            TriggerStatOperatorAsString = TriggerStatOperator.ToString();
            TriggerGradesAsString = TriggerGrades.ToStringArray();
            TriggerMapObjectsAsString = TriggerMapObjects.ToStringArray();
            TriggerMapTypesAsString = TriggerMapTypes.ToStringArray();
            TriggerCurrenciesAsString = TriggerCurrencies.ToStringArray();
            TriggerIgnoreDamageTypesAsString = TriggerIgnoreDamageTypes.ToStringArray();
            TriggerCheckTypeAsString = TriggerCheckType.ToString();
            TriggerMonsterCountStateEffectAsString = TriggerMonsterCountStateEffects.ToStringArray();
            TriggerMonsterCountOperatorAsString = TriggerMonsterCountOperator.ToString();
            DifferentElementalStateEffectOperatorAsString = DifferentElementalStateEffectOperator.ToString();

            TriggerChanceCalcTypeString = TriggerChanceCalcType.ToString();
            TriggerChanceByStatString = TriggerChanceByStat.ToString();

            IsChangingAsset = false;
            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref NameString, Name);
            UpdateIfChanged(ref TriggerAsString, Trigger);
            UpdateIfChanged(ref TriggerOwnerAsString, TriggerOwner);
            UpdateIfChangedArray(ref TriggerDamageTypesAsString, TriggerDamageTypes.ToStringArray());
            UpdateIfChangedArray(ref TriggerHitmarksAsString, TriggerHitmarks.ToStringArray());
            UpdateIfChangedArray(ref TriggerIgnoreHitmarksAsString, TriggerIgnoreHitmarks.ToStringArray());
            UpdateIfChangedArray(ref TriggerStatsAsString, TriggerStats.ToStringArray());
            UpdateIfChangedArray(ref TriggerBuffsAsString, TriggerBuffs.ToStringArray());
            UpdateIfChanged(ref TriggerBuffTypeAsString, TriggerBuffType);
            UpdateIfChangedArray(ref TriggerStateEffectsAsString, TriggerStateEffects.ToStringArray());
            UpdateIfChanged(ref TriggerIgnoreBuffOnHitAsString, TriggerIgnoreBuffOnHit);
            UpdateIfChanged(ref TriggerIgnoreStateEffectOnHitAsString, TriggerIgnoreStateEffectOnHit);
            UpdateIfChanged(ref TriggerOperatorAsString, TriggerOperator);
            UpdateIfChangedArray(ref TriggerOperatorStatsAsString, TriggerOperatorStats.ToStringArray());
            UpdateIfChanged(ref TriggerStatOperatorAsString, TriggerStatOperator);
            UpdateIfChangedArray(ref TriggerGradesAsString, TriggerGrades.ToStringArray());
            UpdateIfChangedArray(ref TriggerMapObjectsAsString, TriggerMapObjects.ToStringArray());
            UpdateIfChangedArray(ref TriggerMapTypesAsString, TriggerMapTypes.ToStringArray());
            UpdateIfChangedArray(ref TriggerCurrenciesAsString, TriggerCurrencies.ToStringArray());
            UpdateIfChangedArray(ref TriggerIgnoreDamageTypesAsString, TriggerIgnoreDamageTypes.ToStringArray());
            UpdateIfChanged(ref TriggerCheckTypeAsString, TriggerCheckType);
            UpdateIfChangedArray(ref TriggerMonsterCountStateEffectAsString, TriggerMonsterCountStateEffects.ToStringArray());
            UpdateIfChanged(ref TriggerMonsterCountOperatorAsString, TriggerMonsterCountOperator);
            UpdateIfChanged(ref DifferentElementalStateEffectOperatorAsString, DifferentElementalStateEffectOperator);

            UpdateIfChanged(ref TriggerChanceCalcTypeString, TriggerChanceCalcType);
            UpdateIfChanged(ref TriggerChanceByStatString, TriggerChanceByStat);

            if (IsChangingAsset)
            {
                IsChangingAsset = false;
                _hasChangedWhiteRefreshAll = true;
            }

            _ = base.RefreshWithoutSave();

            return _hasChangedWhiteRefreshAll;
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
            PassiveNames[] passiveNames = EnumEx.GetValues<PassiveNames>();
            int passiveCount = 0;

            Log.Info("모든 패시브 효과 에셋의 갱신을 시작합니다: {0}", passiveNames.Length);

            base.RefreshAll();

            for (int i = 1; i < passiveNames.Length; i++)
            {
                if (passiveNames[i] != PassiveNames.None)
                {
                    PassiveAsset asset = ScriptableDataManager.Instance.FindPassive(passiveNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.TriggerSettings == null)
                        {
                            continue;
                        }

                        if (asset.TriggerSettings.RefreshWithoutSave())
                        {
                            passiveCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(passiveNames.Length);
                EditorUtility.DisplayProgressBar("모든 패시브 발동 에셋의 갱신", passiveNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 패시브 발동 에셋의 갱신을 종료합니다: {0}/{1}",
                passiveCount.ToSelectString(passiveNames.Length), passiveNames.Length);
#endif
        }

        //

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
            EnumLog();
        }

        public void Compare(PassiveTriggerSettings other)
        {
            if (Name == other.Name) { Log.Warning($"{Name} != {other.Name}"); }
            if (Trigger == other.Trigger) { Log.Warning($"{Trigger} != {other.Trigger}"); }
            if (InitTriggerCondition == other.InitTriggerCondition) { Log.Warning($"{InitTriggerCondition} != {other.InitTriggerCondition}"); }
            if (TriggerOwner == other.TriggerOwner) { Log.Warning($"{TriggerOwner} != {other.TriggerOwner}"); }
            if (TriggerDamageTypes == other.TriggerDamageTypes) { Log.Warning($"{TriggerDamageTypes} != {other.TriggerDamageTypes}"); }
            if (TriggerHitmarks == other.TriggerHitmarks) { Log.Warning($"{TriggerHitmarks} != {other.TriggerHitmarks}"); }
            if (TriggerIgnoreHitmarks == other.TriggerIgnoreHitmarks) { Log.Warning($"{TriggerIgnoreHitmarks} != {other.TriggerIgnoreHitmarks}"); }
            if (TriggerStats == other.TriggerStats) { Log.Warning($"{TriggerStats} != {other.TriggerStats}"); }
            if (TriggerGrades == other.TriggerGrades) { Log.Warning($"{TriggerGrades} != {other.TriggerGrades}"); }
            if (TriggerMapObjects == other.TriggerMapObjects) { Log.Warning($"{TriggerMapObjects} != {other.TriggerMapObjects}"); }
            if (TriggerMapTypes == other.TriggerMapTypes) { Log.Warning($"{TriggerMapTypes} != {other.TriggerMapTypes}"); }
            if (TriggerCurrencies == other.TriggerCurrencies) { Log.Warning($"{TriggerCurrencies} != {other.TriggerCurrencies}"); }
            if (TriggerBuffs == other.TriggerBuffs) { Log.Warning($"{TriggerBuffs} != {other.TriggerBuffs}"); }
            if (TriggerBuffType == other.TriggerBuffType) { Log.Warning($"{TriggerBuffType} != {other.TriggerBuffType}"); }
            if (TriggerStateEffects == other.TriggerStateEffects) { Log.Warning($"{TriggerStateEffects} != {other.TriggerStateEffects}"); }
            if (TriggerIgnoreBuffOnHit == other.TriggerIgnoreBuffOnHit) { Log.Warning($"{TriggerIgnoreBuffOnHit} != {other.TriggerIgnoreBuffOnHit}"); }
            if (TriggerIgnoreStateEffectOnHit == other.TriggerIgnoreStateEffectOnHit) { Log.Warning($"{TriggerIgnoreStateEffectOnHit} != {other.TriggerIgnoreStateEffectOnHit}"); }
            if (TriggerIgnoreDamageTypes == other.TriggerIgnoreDamageTypes) { Log.Warning($"{TriggerIgnoreDamageTypes} != {other.TriggerIgnoreDamageTypes}"); }
            if (TriggerCount == other.TriggerCount) { Log.Warning($"{TriggerCount} != {other.TriggerCount}"); }
            if (TriggerCountDontSave == other.TriggerCountDontSave) { Log.Warning($"{TriggerCountDontSave} != {other.TriggerCountDontSave}"); }
            if (TriggerMaxStack == other.TriggerMaxStack) { Log.Warning($"{TriggerMaxStack} != {other.TriggerMaxStack}"); }
            if (TriggerCheckType == other.TriggerCheckType) { Log.Warning($"{TriggerCheckType} != {other.TriggerCheckType}"); }
            if (TriggerChance == other.TriggerChance) { Log.Warning($"{TriggerChance} != {other.TriggerChance}"); }
            if (TriggerChanceByLevel == other.TriggerChanceByLevel) { Log.Warning($"{TriggerChanceByLevel} != {other.TriggerChanceByLevel}"); }
            if (TriggerChanceOptionMinRange == other.TriggerChanceOptionMinRange) { Log.Warning($"{TriggerChanceOptionMinRange} != {other.TriggerChanceOptionMinRange}"); }
            if (TriggerChanceOptionMaxRange == other.TriggerChanceOptionMaxRange) { Log.Warning($"{TriggerChanceOptionMaxRange} != {other.TriggerChanceOptionMaxRange}"); }
            if (TriggerChanceOptionStep == other.TriggerChanceOptionStep) { Log.Warning($"{TriggerChanceOptionStep} != {other.TriggerChanceOptionStep}"); }
            if (TriggerOperator == other.TriggerOperator) { Log.Warning($"{TriggerOperator} != {other.TriggerOperator}"); }
            if (TriggerPercent == other.TriggerPercent) { Log.Warning($"{TriggerPercent} != {other.TriggerPercent}"); }
            if (TriggerMonsterCount == other.TriggerMonsterCount) { Log.Warning($"{TriggerMonsterCount} != {other.TriggerMonsterCount}"); }
            if (TriggerMonsterCountOperator == other.TriggerMonsterCountOperator) { Log.Warning($"{TriggerMonsterCountOperator} != {other.TriggerMonsterCountOperator}"); }
            if (TriggerMonsterRange == other.TriggerMonsterRange) { Log.Warning($"{TriggerMonsterRange} != {other.TriggerMonsterRange}"); }
            if (TriggerOperatorStats == other.TriggerOperatorStats) { Log.Warning($"{TriggerOperatorStats} != {other.TriggerOperatorStats}"); }
            if (TriggerStatOperator == other.TriggerStatOperator) { Log.Warning($"{TriggerStatOperator} != {other.TriggerStatOperator}"); }
            if (TriggerStatValue == other.TriggerStatValue) { Log.Warning($"{TriggerStatValue} != {other.TriggerStatValue}"); }
            if (TriggerCheckPassiveRestTime == other.TriggerCheckPassiveRestTime) { Log.Warning($"{TriggerCheckPassiveRestTime} != {other.TriggerCheckPassiveRestTime}"); }
            if (IsReleaseCheckSameBuffType == other.IsReleaseCheckSameBuffType) { Log.Warning($"{IsReleaseCheckSameBuffType} != {other.IsReleaseCheckSameBuffType}"); }
            if (DifferentElementalStateEffectCount == other.DifferentElementalStateEffectCount) { Log.Warning($"{DifferentElementalStateEffectCount} != {other.DifferentElementalStateEffectCount}"); }
            if (DifferentElementalStateEffectOperator == other.DifferentElementalStateEffectOperator) { Log.Warning($"{DifferentElementalStateEffectOperator} != {other.DifferentElementalStateEffectOperator}"); }
        }

        public void CheckDefault()
        {
            StringBuilder sb = new();
            _ = sb.Append($"{Name.ToLogString()}");

            if (Trigger != default) { _ = sb.Append($", Trigger:{Trigger.ToLogString()}"); }
            if (InitTriggerCondition != default) { _ = sb.Append($", InitTriggerCondition:{InitTriggerCondition.ToSelectString()}"); }
            if (TriggerOwner != default) { _ = sb.Append($", TriggerOwner:{TriggerOwner.ToSelectString()}"); }
            if (TriggerDamageTypes.IsValidArray()) { _ = sb.Append($", TriggerDamageTypes:{TriggerDamageTypes.ToLogString()}"); }
            if (TriggerHitmarks.IsValidArray()) { _ = sb.Append($", TriggerHitmarks:{TriggerHitmarks.ToLogString()}"); }
            if (TriggerIgnoreHitmarks.IsValidArray()) { _ = sb.Append($", TriggerIgnoreHitmarks:{TriggerIgnoreHitmarks.ToLogString()}"); }
            if (TriggerStats.IsValidArray()) { _ = sb.Append($", TriggerStats:{TriggerStats.ToLogString()}"); }
            if (TriggerGrades.IsValidArray()) { _ = sb.Append($", TriggerGrades:{TriggerGrades.ToLogString()}"); }
            if (TriggerMapObjects.IsValidArray()) { _ = sb.Append($", TriggerMapObjects:{TriggerMapObjects.JoinToString()}"); }
            if (TriggerMapTypes.IsValidArray()) { _ = sb.Append($", TriggerMapTypes:{TriggerMapTypes.JoinToString()}"); }
            if (TriggerCurrencies.IsValidArray()) { _ = sb.Append($", TriggerCurrencies:{TriggerCurrencies.JoinToString()}"); }
            if (TriggerBuffs.IsValidArray()) { _ = sb.Append($", TriggerBuffs:{TriggerBuffs.ToLogString()}"); }
            if (TriggerBuffType != default) { _ = sb.Append($", TriggerBuffType:{TriggerBuffType.ToLogString()}"); }
            if (TriggerStateEffects.IsValidArray()) { _ = sb.Append($", TriggerStateEffects:{TriggerStateEffects.ToLogString()}"); }
            if (TriggerIgnoreBuffOnHit != default) { _ = sb.Append($", TriggerIgnoreBuffOnHit:{TriggerIgnoreBuffOnHit.ToLogString()}"); }
            if (TriggerIgnoreStateEffectOnHit != default) { _ = sb.Append($", TriggerIgnoreStateEffectOnHit:{TriggerIgnoreStateEffectOnHit.ToLogString()}"); }
            if (TriggerIgnoreDamageTypes.IsValidArray()) { _ = sb.Append($", TriggerIgnoreDamageTypes:{TriggerIgnoreDamageTypes.JoinToString()}"); }
            if (TriggerCount != default) { _ = sb.Append($", TriggerCount:{TriggerCount.ToSelectString()}"); }
            if (TriggerCountDontSave != default) { _ = sb.Append($", TriggerCountDontSave:{TriggerCountDontSave.ToSelectString()}"); }
            if (TriggerMaxStack != default) { _ = sb.Append($", TriggerMaxStack:{TriggerMaxStack.ToSelectString()}"); }
            if (TriggerCheckType != default) { _ = sb.Append($", TriggerCheckType:{TriggerCheckType.ToSelectString()}"); }
            if (TriggerChance != default) { _ = sb.Append($", TriggerChance:{TriggerChance.ToSelectString()}"); }
            if (TriggerChanceByLevel != default) { _ = sb.Append($", TriggerChanceByLevel:{TriggerChanceByLevel.ToSelectString()}"); }
            if (TriggerChanceOptionMinRange != default) { _ = sb.Append($", TriggerChanceOptionMinRange:{TriggerChanceOptionMinRange.ToSelectString()}"); }
            if (TriggerChanceOptionMaxRange != default) { _ = sb.Append($", TriggerChanceOptionMaxRange:{TriggerChanceOptionMaxRange.ToSelectString()}"); }
            if (TriggerChanceOptionStep != default) { _ = sb.Append($", TriggerChanceOptionStep:{TriggerChanceOptionStep.ToSelectString()}"); }
            if (TriggerOperator != default) { _ = sb.Append($", TriggerOperator:{TriggerOperator.ToSelectString()}"); }
            if (TriggerPercent != default) { _ = sb.Append($", TriggerPercent:{TriggerPercent.ToSelectString()}"); }
            if (TriggerMonsterCount != default) { _ = sb.Append($", TriggerMonsterCount:{TriggerMonsterCount.ToSelectString()}"); }
            if (TriggerMonsterCountOperator != default) { _ = sb.Append($", TriggerMonsterCountOperator:{TriggerMonsterCountOperator.ToSelectString()}"); }
            if (TriggerMonsterRange != default) { _ = sb.Append($", TriggerMonsterRange:{TriggerMonsterRange.ToSelectString()}"); }
            if (TriggerOperatorStats.IsValidArray()) { _ = sb.Append($", TriggerOperatorStats:{TriggerOperatorStats.ToLogString()}"); }
            if (TriggerStatOperator != default) { _ = sb.Append($", TriggerStatOperator:{TriggerStatOperator.ToSelectString()}"); }
            if (TriggerStatValue != default) { _ = sb.Append($", TriggerStatValue:{TriggerStatValue.ToSelectString()}"); }
            if (TriggerCheckPassiveRestTime != default) { _ = sb.Append($", TriggerCheckPassiveRestTime:{TriggerCheckPassiveRestTime.ToSelectString()}"); }
            if (IsReleaseCheckSameBuffType != default) { _ = sb.Append($", IsReleaseCheckSameBuffType:{IsReleaseCheckSameBuffType.ToSelectString()}"); }
            if (DifferentElementalStateEffectCount != default) { _ = sb.Append($", DifferentElementalStateEffectCount:{DifferentElementalStateEffectCount.ToSelectString()}"); }
            if (DifferentElementalStateEffectOperator != default) { _ = sb.Append($", DifferentElementalStateEffectOperator:{DifferentElementalStateEffectOperator.ToSelectString()}"); }

            if (TriggerChanceCalcType != default) { _ = sb.Append($", TriggerChanceCalcType:{TriggerChanceCalcType.ToSelectString()}"); }
            if (TriggerChanceByStat != default) { _ = sb.Append($", TriggerChanceByStat:{TriggerChanceByStat.ToSelectString()}"); }

            if (sb.Length > 1)
            {
                Log.Progress(sb.ToString());
            }
        }

        //

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "PassiveTrigger".ToColorString(GameColors.Passive);
            EnumExplorer.LogStat(type, Name.ToString(), TriggerStats);
            EnumExplorer.LogStat(type, Name.ToString(), TriggerOperatorStats);
            EnumExplorer.LogBuff(type, Name.ToString(), TriggerBuffs);
#endif
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (IsChangingAsset)
            {
                Log.Error("패시브의 발동 에셋의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }
            if (string.IsNullOrEmpty(NameString))
            {
                Log.Error("패시브 이름이 설정되지 않았습니다: {0}", name);
            }
            if (Trigger == 0)
            {
                Log.Error("패시브의 발동 트리거가 설정되지 않았습니다. {0}({1})", Name.ToLogString(), name);
            }

            if (!CheckValidTriggerChance())
            {
                Log.Error("이 패시브의 발동 확률이 올바르지 않습니다: {0}", Name);
            }
#endif
        }

        private bool CheckValidTriggerChance()
        {
            switch (TriggerChanceCalcType)
            {
                case PassiveTriggerChanceCalcType.Fixed:
                    if (TriggerChance.IsZero())
                    {
                        return false;
                    }
                    break;

                case PassiveTriggerChanceCalcType.FromOptionRange:
                    if (TriggerChanceOptionMinRange.IsZero() || TriggerChanceOptionMaxRange.IsZero())
                    {
                        return false;
                    }
                    break;

                case PassiveTriggerChanceCalcType.FromStat:
                    if (TriggerChanceByStat == StatNames.None)
                    {
                        return false;
                    }
                    break;

                default: // None 타입인데도 값이 설정되어 있으면 잘못된 설정으로 간주 ▶ CherryRed

                    if (!TriggerChance.IsZero())
                    {
                        return false;
                    }
                    if (!TriggerChanceOptionMinRange.IsZero() && !TriggerChanceOptionMaxRange.IsZero())
                    {
                        return false;
                    }
                    if (TriggerChanceByStat != StatNames.None)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        //

        #region GUI-Color

        protected Color GetPassiveTriggerColor(PassiveTriggers triggerName)
        {
            if (triggerName == 0)
            {
                return GameColors.CherryRed;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetPassiveTargetTypeColor(PassiveTargetTypes triggerName)
        {
            if (triggerName == 0)
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetPassiveTriggerConditionColor(PassiveTriggerCondition triggerCondition)
        {
            if (triggerCondition == 0)
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        private Color GetTriggerHitmarksColor()
        {
            if (TriggerHitmarks.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        private Color GetTriggerIgnoreHitmarksColor()
        {
            if (TriggerIgnoreHitmarks.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerSkillElementsColor()
        {
            if (TriggerSkillElements.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerDamageTypesColor()
        {
            if (TriggerDamageTypes.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerStatsColor()
        {
            if (TriggerStats.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerGradesColor()
        {
            if (TriggerGrades.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerMapObjectsColor()
        {
            if (TriggerMapObjects.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerMapTypesColor()
        {
            if (TriggerMapTypes.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerCurrenciesColor()
        {
            if (TriggerCurrencies.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerBuffsColor()
        {
            if (TriggerBuffs.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerMonsterCountStateEffectsColor()
        {
            if (TriggerMonsterCountStateEffects.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerStateEffectsColor()
        {
            if (TriggerStateEffects.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerIgnoreDamageTypesColor()
        {
            if (TriggerIgnoreDamageTypes.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        protected Color GetTriggerOperatorStatsColor()
        {
            if (TriggerOperatorStats.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        private Color GetTriggerChanceColor()
        {
            switch (TriggerChanceCalcType)
            {
                case PassiveTriggerChanceCalcType.Fixed:
                    if (!TriggerChance.IsZero())
                    {
                        return GameColors.GreenYellow;
                    }
                    else
                    {
                        return GameColors.CherryRed;
                    }

                case PassiveTriggerChanceCalcType.FromOptionRange:
                    if (!TriggerChanceOptionMinRange.IsZero() && !TriggerChanceOptionMaxRange.IsZero())
                    {
                        return GameColors.GreenYellow;
                    }
                    else
                    {
                        return GameColors.CherryRed;
                    }

                case PassiveTriggerChanceCalcType.FromStat:
                    if (TriggerChanceByStat != StatNames.None)
                    {
                        return GameColors.GreenYellow;
                    }
                    else
                    {
                        return GameColors.CherryRed;
                    }

                default:
                    // None 타입인데도 값이 설정되어 있으면 잘못된 설정으로 간주 ▶ CherryRed
                    if (!TriggerChance.IsZero())
                    {
                        return GameColors.CherryRed;
                    }
                    if (!TriggerChanceOptionMinRange.IsZero() && !TriggerChanceOptionMaxRange.IsZero())
                    {
                        return GameColors.CherryRed;
                    }
                    if (TriggerChanceByStat != StatNames.None)
                    {
                        return GameColors.CherryRed;
                    }

                    return GameColors.DarkGray;
            }
        }

        #endregion GUI-Color

        private void RefreshTriggerMassage()
        {
            switch (Trigger)
            {
                case PassiveTriggers.Activate:
                    {
                        TriggerMessege = "패시브를 활성화합니다";
                    }
                    break;

                case PassiveTriggers.AttackMonster:
                    {
                        TriggerMessege = "몬스터를 공격합니다";
                    }
                    break;

                case PassiveTriggers.AttackMonsterCritical:
                    {
                        TriggerMessege = "몬스터를 치명타로 공격합니다.";
                    }
                    break;

                case PassiveTriggers.AttackMonsterNonCritical:
                    {
                        TriggerMessege = "몬스터를 치명타 없이 공격합니다.";
                    }
                    break;

                case PassiveTriggers.MonsterKill:
                    {
                        TriggerMessege = "몬스터를 처치 합니다.";
                    }
                    break;

                //

                case PassiveTriggers.CastSkill:
                    {
                        TriggerMessege = "기술을 사용합니다";
                    }
                    break;

                case PassiveTriggers.AssignSkill:
                    {
                        TriggerMessege = "기술을 할당합니다";
                    }
                    break;

                case PassiveTriggers.UnassignSkill:
                    {
                        TriggerMessege = "기술을 할당해제합니다";
                    }
                    break;

                case PassiveTriggers.RefreshCooldownSkill:
                    {
                        TriggerMessege = "기술의 재사용 대기시간을 갱신합니다.";
                    }
                    break;

                case PassiveTriggers.StartSkillCooldown:
                    {
                        TriggerMessege = "기술의 재사용 대기를 시작합니다.";
                    }
                    break;

                case PassiveTriggers.StopSkillCooldown:
                    {
                        TriggerMessege = "기술의 재사용 대기를 종료합니다.";
                    }
                    break;

                //

                case PassiveTriggers.ExecuteAttack:
                    {
                        TriggerMessege = "공격을 실행합니다";
                    }
                    break;

                case PassiveTriggers.PlayerHealed:
                    {
                        TriggerMessege = "플레이어가 회복합니다.";
                    }
                    break;

                case PassiveTriggers.PlayerChangeHealth:
                    {
                        TriggerMessege = "플레이어의 생명력이 변경됩니다";
                    }
                    break;

                case PassiveTriggers.PlayerChangeVitalResource:
                    {
                        TriggerMessege = "플레이어의 자원이 변경됩니다.";
                    }
                    break;

                case PassiveTriggers.PlayerUseVitalResource:
                    {
                        TriggerMessege = "플레이어의 자원을 사용합니다.";
                    }
                    break;

                //

                case PassiveTriggers.PlayerChangeStat:
                    {
                        TriggerMessege = "플레이어의 능력치를 갱신합니다.";
                    }
                    break;

                case PassiveTriggers.PlayerAddStat:
                    {
                        TriggerMessege = "플레이어의 능력치를 추가합니다.";
                    }
                    break;

                case PassiveTriggers.PlayerRemoveStat:
                    {
                        TriggerMessege = "플레이어의 능력치를 삭제합니다.";
                    }
                    break;

                //

                case PassiveTriggers.RestoreMonsterVitalResource:
                    {
                        TriggerMessege = "몬스터가 전투 자원을 회복합니다.";
                    }
                    break;

                case PassiveTriggers.PlayerLevelUp:
                    {
                        TriggerMessege = "플레이어가 레벨업을 합니다.";
                    }
                    break;

                case PassiveTriggers.PlayerOperateMapObject:
                    {
                        TriggerMessege = "플레이어가 오브젝트를 작동합니다";
                    }
                    break;

                case PassiveTriggers.PlayerMoveToStage:
                    {
                        TriggerMessege = "플레이어가 스테이지를 이동합니다.";
                    }
                    break;

                case PassiveTriggers.PlayerEvasion:
                    {
                        TriggerMessege = "플레이어가 공격을 회피합니다.";
                    }
                    break;

                case PassiveTriggers.PlayerDamaged:
                    {
                        TriggerMessege = "플레이어가 피격됩니다.";
                    }
                    break;

                case PassiveTriggers.PlayerDamagedCritical:
                    {
                        TriggerMessege = "치명타로 피격됩니다.";
                    }
                    break;

                case PassiveTriggers.PlayerDeathDefiance:
                    {
                        TriggerMessege = "플레이어가 죽음을 저항합니다.";
                    }
                    break;

                case PassiveTriggers.MonsterDamaged:
                    {
                        TriggerMessege = "몬스터가 피격됩니다.";
                    }
                    break;

                case PassiveTriggers.DestroyRelic:
                    {
                        TriggerMessege = "아이템을 파괴합니다.";
                    }
                    break;

                case PassiveTriggers.AddBuffToPlayer:
                    {
                        TriggerMessege = "플레이어에게 버프를 추가합니다";
                    }
                    break;

                case PassiveTriggers.RemoveBuffToPlayer:
                    {
                        TriggerMessege = "플레이어에게 버프를 삭제합니다";
                    }
                    break;

                case PassiveTriggers.AddStateEffectToPlayer:
                    {
                        TriggerMessege = "플레이어에게 상태이상을 추가합니다";
                    }
                    break;

                case PassiveTriggers.RemoveStateEffectToPlayer:
                    {
                        TriggerMessege = "플레이어에게 상태이상을 삭제합니다";
                    }
                    break;

                case PassiveTriggers.AddStateEffectToMonster:
                    {
                        TriggerMessege = "몬스터에게 상태이상을 추가합니다";
                    }
                    break;

                case PassiveTriggers.RemoveStateEffectToMonster:
                    {
                        TriggerMessege = "몬스터에게 상태이상을 삭제합니다";
                    }
                    break;

                case PassiveTriggers.AddBuffToMonster:
                    {
                        TriggerMessege = "몬스터에게 버프를 추가합니다";
                    }
                    break;

                case PassiveTriggers.RemoveBuffToMonster:
                    {
                        TriggerMessege = "몬스터에게 버프를 삭제합니다";
                    }
                    break;

                case PassiveTriggers.AddBuffStackToPlayer:
                    {
                        TriggerMessege = "플레이어의 버프 스택이 올라갑니다.";
                    }
                    break;

                case PassiveTriggers.AddBuffStackToMonster:
                    {
                        TriggerMessege = "몬스터의 버프 스택이 올라갑니다.";
                    }
                    break;

                case PassiveTriggers.UsePotion:
                    {
                        TriggerMessege = "포션을 사용합니다";
                    }
                    break;

                case PassiveTriggers.TakePotion:
                    {
                        TriggerMessege = "포션을 획득합니다.";
                    }
                    break;

                case PassiveTriggers.EatFood:
                    {
                        TriggerMessege = "요리를 먹습니다.";
                    }
                    break;

                case PassiveTriggers.RerollBuyItem:
                    {
                        TriggerMessege = "아이템을 리롤합니다.";
                    }
                    break;

                case PassiveTriggers.ThrowRelic:
                    {
                        TriggerMessege = "아이템을 버립니다";
                    }
                    break;

                case PassiveTriggers.TakeRelicOperate:
                    {
                        TriggerMessege = "상호작용을 통해 유물을 획득합니다.";
                    }
                    break;

                case PassiveTriggers.UseCurrency:
                    {
                        TriggerMessege = "재화를 소모합니다.";
                    }
                    break;

                case PassiveTriggers.CurrencyChanged:
                    {
                        TriggerMessege = "재화를 갱신합니다.";
                    }
                    break;

                case PassiveTriggers.PlayerShieldCharge:
                    {
                        TriggerMessege = "플레이어의 보호막이 충전됩니다.";
                    }
                    break;

                case PassiveTriggers.PlayerShieldDestroy:
                    {
                        TriggerMessege = "플레이어의 보호막이 파괴됩니다.";
                    }
                    break;

                default:
                    Log.Warning("패시브의 트리거 안내를 갱신할 수 없습니다. {0}, {1}", name, Trigger.ToLogString());
                    break;
            }
        }

        public string GetString()
        {
            StringBuilder sb = new();

            // bool 타입 필드들 (true일 때만 포함)
            if (TriggerMaxStack)
            {
                _ = sb.Append($"최대 스택일때만 작동할지 여부: {TriggerMaxStack.ToBoolString()}, ");
            }

            if (TriggerCheckPassiveRestTime)
            {
                _ = sb.Append($"패시브 발동 시 유휴 상태를 확인할지 여부: {TriggerCheckPassiveRestTime.ToBoolString()}, ");
            }

            if (IsReleaseCheckSameBuffType)
            {
                _ = sb.Append($"패시브 역실행 검사시 조건 발동을 실행검사와 똑같이 확인할지 여부: {IsReleaseCheckSameBuffType.ToBoolString()}, ");
            }

            if (TriggerCountDontSave)
            {
                _ = sb.Append($"패시브 적용 제한 발동 횟수 저장하지 않을지 여부: {TriggerCountDontSave.ToBoolString()}, ");
            }

            // int 타입 필드들 (0이 아닐 때만 포함)
            if (TriggerCount != 0)
            {
                _ = sb.Append($"패시브 적용 제한 발동 횟수: {TriggerCount.ToSelectString(0)}, ");
            }

            if (DifferentElementalStateEffectCount != 0)
            {
                _ = sb.Append($"서로 다른 속성 상태이상의 수: {DifferentElementalStateEffectCount.ToSelectString(0)}, ");
            }

            if (TriggerResourceValue != 0)
            {
                _ = sb.Append($"패시브 발동 조건 자원 값: {TriggerResourceValue.ToSelectString(0)}, ");
            }

            if (TriggerStatValue != 0)
            {
                _ = sb.Append($"패시브 발동 조건 능력치 비율: {TriggerStatValue.ToSelectString(0)}, ");
            }

            if (TriggerMonsterCount != 0)
            {
                _ = sb.Append($"패시브 발동 조건 몬스터 수: {TriggerMonsterCount.ToSelectString(0)}, ");
            }

            // float 타입 필드들 (0이 아닐 때만 포함)
            if (TriggerPercent != 0f)
            {
                _ = sb.Append($"패시브 발동 조건 자원 비율: {TriggerPercent.ToSelectString(0)}, ");
            }

            if (TriggerMonsterRange != 0f)
            {
                _ = sb.Append($"패시브 발동 몬스터와의 거리: {TriggerMonsterRange.ToSelectString(0)}, ");
            }

            if (TriggerChance != 0f)
            {
                _ = sb.Append($"패시브 발동 확률: {ValueStringEx.GetPercentString(TriggerChance, true)}, ");
            }

            if (TriggerChanceByLevel != 0f)
            {
                _ = sb.Append($"레벨별 패시브 발동 확률: {ValueStringEx.GetPercentString(TriggerChanceByLevel, true)}, ");
            }

            if (TriggerChanceOptionMinRange != 0f)
            {
                _ = sb.Append($"전설 장비 옵션으로 설정되는 패시브 발동 확률의 최소 범위: {ValueStringEx.GetPercentString(TriggerChanceOptionMinRange, true)}, ");
            }

            if (TriggerChanceOptionMaxRange != 0f)
            {
                _ = sb.Append($"전설 장비 옵션으로 설정되는 패시브 발동 확률의 최대 범위: {ValueStringEx.GetPercentString(TriggerChanceOptionMaxRange, true)}, ");
            }

            if (TriggerChanceOptionStep != 0f)
            {
                _ = sb.Append($"전설 장비 옵션에 의해 설정되는 패시브 발동 확률 증가 단위: {ValueStringEx.GetPercentString(TriggerChanceOptionStep, true)}, ");
            }

            // enum 타입 필드들 (0이 아닐 때만 포함)
            if (Trigger != 0)
            {
                _ = sb.Append($"패시브 발동 조건: {Trigger}({TriggerMessege}), ");
            }

            if (InitTriggerCondition != 0)
            {
                _ = sb.Append($"생성시 패시브 발동: {InitTriggerCondition}, ");
            }

            if (TriggerCheckType != 0)
            {
                _ = sb.Append($"패시브 발동 검사 종류: {TriggerCheckType}, ");
            }

            if (TriggerOwner != 0)
            {
                _ = sb.Append($"패시브 발동 소유 캐릭터: {TriggerOwner}, ");
            }

            if (TriggerBuffType != 0)
            {
                _ = sb.Append($"패시브 발동 버프 종류: {TriggerBuffType.ToLogString()}, ");
            }

            if (TriggerIgnoreBuffOnHit != 0)
            {
                _ = sb.Append($"패시브 발동 무시 버프 종류: {TriggerIgnoreBuffOnHit.ToLogString()}, ");
            }

            if (TriggerIgnoreStateEffectOnHit != 0)
            {
                _ = sb.Append($"패시브 발동 무시 상태이상: {TriggerIgnoreStateEffectOnHit.ToLogString()}, ");
            }

            if (TriggerResourceType != 0)
            {
                _ = sb.Append($"패시브 발동 조건 자원 타입: {TriggerResourceType}, ");
            }

            if (TriggerOperator != 0)
            {
                _ = sb.Append($"패시브 발동 조건 자원 비율 비교 연산자: {TriggerOperator}, ");
            }

            if (TriggerStatOperator != 0)
            {
                _ = sb.Append($"패시브 발동 조건 능력치 비율 범위 연산자: {TriggerStatOperator}, ");
            }

            if (TriggerMonsterCountOperator != 0)
            {
                _ = sb.Append($"패시브 발동 몬스터 수 비교 연산자: {TriggerMonsterCountOperator}, ");
            }

            if (DifferentElementalStateEffectOperator != 0)
            {
                _ = sb.Append($"서로 다른 속성 상태이상의 범위 연산자: {DifferentElementalStateEffectOperator}, ");
            }

            if (TriggerChanceCalcType != 0)
            {
                _ = sb.Append($"패시브 발동 확률 타입: {TriggerChanceCalcType}, ");
            }

            if (TriggerChanceByStat != 0)
            {
                _ = sb.Append($"패시브 발동 확률 능력치: {TriggerChanceByStat.ToLogString()}, ");
            }

            // 배열 필드들 (유효한 배열일 때만 포함)
            if (TriggerSkillElements.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 기술 속성들: {string.Join(", ", TriggerSkillElements.JoinToString())}, ");
            }

            if (TriggerDamageTypes.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 피해 종류: {string.Join(", ", TriggerDamageTypes.ToLogString())}, ");
            }

            if (TriggerHitmarks.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 히트마크: {string.Join(", ", TriggerHitmarks.ToLogString())}, ");
            }

            if (TriggerIgnoreHitmarks.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 무시 히트마크: {string.Join(", ", TriggerIgnoreHitmarks.ToLogString())}, ");
            }

            if (TriggerIgnoreDamageTypes.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 무시 피해 종류들: {string.Join(", ", TriggerIgnoreDamageTypes.ToLogString())}, ");
            }

            if (TriggerStats.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 능력치: {string.Join(", ", TriggerStats.ToLogString())}, ");
            }

            if (TriggerGrades.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 조건 아이템 등급: {string.Join(", ", TriggerGrades.ToLogString())}, ");
            }

            if (TriggerMapObjects.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 조건 맵 오브젝트: {string.Join(", ", TriggerMapObjects.JoinToString())}, ");
            }

            if (TriggerMapTypes.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 조건 맵 지역: {string.Join(", ", TriggerMapTypes.JoinToString())}, ");
            }

            if (TriggerCurrencies.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 조건 재화: {string.Join(", ", TriggerCurrencies.JoinToString())}, ");
            }

            if (TriggerBuffs.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 버프: {string.Join(", ", TriggerBuffs.ToLogString())}, ");
            }

            if (TriggerStateEffects.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 상태이상들: {string.Join(", ", TriggerStateEffects.JoinToString())}, ");
            }

            if (TriggerMonsterCountStateEffects.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 조건 몬스터의 상태이상: {string.Join(", ", TriggerMonsterCountStateEffects.ToLogString())}, ");
            }

            if (TriggerOperatorStats.IsValidArray())
            {
                _ = sb.Append($"패시브 발동 조건 능력치: {string.Join(", ", TriggerOperatorStats.ToLogString())}, ");
            }

            // 마지막 쉼표와 공백 제거
            if (sb.Length > 2)
            {
                sb.Length -= 2; // 마지막 ", " 제거
            }

            return sb.ToString();
        }
    }
}