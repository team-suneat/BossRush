using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TeamSuneat.Data
{
    [Serializable]
    public partial class SkillAssetData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetSkillNameColor")]
        public SkillNames Name;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetSkillAttributeColor")]
        public SkillAttributeTypes Attribute;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetSkillTypeColor")]
        public SkillTypes Type;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetGradeColor")]
        public GradeNames Grade;

        #region 기본 정보

        [FoldoutGroup("#기본 정보")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("최대 레벨")]
        public int MaxLevel;

        [FoldoutGroup("#기본 정보")]
        [TextArea(3, 5)]
        [SuffixLabel("스킬 설명")]
        public string Description;

        [FoldoutGroup("#기본 정보")]
        [SuffixLabel("스킬 아이콘")]
        public Sprite Icon;

        [FoldoutGroup("#기본 정보")]
        [SuffixLabel("스킬 이펙트 프리펩")]
        public GameObject EffectPrefab;

        #endregion 기본 정보

        #region 쿨타임 정보

        [FoldoutGroup("#쿨타임")]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetCooldownTypeColor")]
        [SuffixLabel("쿨타임 타입")]
        public CooldownTypes CooldownType;

        [FoldoutGroup("#쿨타임")]
        [EnableIf("@CooldownType == CooldownTypes.TimeBased")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("기본 대기 시간 (초)")]
        public float BaseCooldownTime;

        [FoldoutGroup("#쿨타임")]
        [EnableIf("@CooldownType == CooldownTypes.TimeBased")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("레벨당 시간 감소 (초)")]
        public float CooldownTimeByLevel;

        [FoldoutGroup("#쿨타임")]
        [EnableIf("@CooldownType == CooldownTypes.AttackCountBased")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("기본 필요 공격수")]
        public int BaseCooldownAttackCount;

        [FoldoutGroup("#쿨타임")]
        [EnableIf("@CooldownType == CooldownTypes.AttackCountBased")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("레벨당 공격수 감소")]
        public int CooldownAttackCountByLevel;

        #endregion 쿨타임 정보

        #region MP 소모

        [FoldoutGroup("#MP 소모")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("기본 MP 소모")]
        public int BaseManaCost;

        [FoldoutGroup("#MP 소모")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("레벨당 MP 소모 변화")]
        public int ManaCostByLevel;

        #endregion MP 소모

        #region 효과 데이터

        [FoldoutGroup("#효과 데이터")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("기본 효과 값 (%)")]
        public float BaseEffectValue;

        [FoldoutGroup("#효과 데이터")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("레벨당 증가량 (%)")]
        public float EffectValueByLevel;

        [FoldoutGroup("#효과 데이터")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("효과 배율 (예: 1.5, 3.0)")]
        public float EffectMultiplier = 1.0f;

        [FoldoutGroup("#효과 데이터")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("기본 공격 횟수")]
        public int BaseHitCount = 1;

        [FoldoutGroup("#효과 데이터")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("레벨당 공격 횟수 증가")]
        public int HitCountByLevel;

        [FoldoutGroup("#효과 데이터")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("기본 범위")]
        public float BaseRange;

        [FoldoutGroup("#효과 데이터")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("레벨당 범위 증가")]
        public float RangeByLevel;

        #endregion 효과 데이터

        #region 특수 효과

        [FoldoutGroup("#특수 효과")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("빙결 효과")]
        public bool HasFreezeEffect;

        [FoldoutGroup("#특수 효과")]
        [EnableIf("HasFreezeEffect")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("빙결 확률 (0~1)")]
        [Range(0f, 1f)]
        public float FreezeChance;

        [FoldoutGroup("#특수 효과")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("스턴 효과")]
        public bool HasStunEffect;

        [FoldoutGroup("#특수 효과")]
        [EnableIf("HasStunEffect")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("스턴 확률 (0~1)")]
        [Range(0f, 1f)]
        public float StunChance;

        [FoldoutGroup("#특수 효과")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("시전 중 시간 정지")]
        public bool HasTimeStop;

        [FoldoutGroup("#특수 효과")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("관통 공격")]
        public bool IsPiercing;

        #endregion 특수 효과

        #region 레벨업 비용

        [FoldoutGroup("#레벨업 비용")]
        [InfoBox("레벨별 에메랄드 비용 배열. 인덱스는 레벨(1부터 시작)을 의미합니다.")]
        [SuffixLabel("레벨별 에메랄드 비용")]
        public int[] LevelUpCosts;

        #endregion 레벨업 비용

        #region String

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string AttributeAsString;

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string TypeAsString;

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string GradeAsString;

        [FoldoutGroup("#String")]
        [EnableIf("@!IsChangingAsset")]
        public string CooldownTypeAsString;

        #endregion String


        //

        public string Index; // 슬롯 인덱스
        public CharacterNames[] Owners; // 사용 캐릭터
        public StateEffects[] StateEffectDetails; // 상태 정보창 표기
        public SkillSources Source; // 출처
        public SkillCategories Category;  // 분류
        public SkillUseTypes Use; // 사용 방법
        public SkillApplications Application; // 적용
        public SkillNames ReplacedSkill;
        public SkillTags Tag; // 기술 태그
        public GameElements Element; // 기술 속성
        public bool UseCostOnCast; // 기술 시전 시 자원 소모
        public bool UseCooldownOnCast; // 기술 시전 시 재사용 대기 시작
        public SkillNames AlternateSkill; // 대체 기술
        public SkillNames[] Conditions; // 필요조건 기술
        public int[] ConditionsLevel;  // 필요조건 기술레벨
        public bool AcitavteOnInit; // 초기화시 활성화 : 몬스터 기술에 사용, 캐릭터 초기화시 스킬을 활성화, 일정 시간마다 스킬을 사용하지 않음, 일회성 스킬에 한하여 사용.
        public SkillNames[] Incompatibles; // 호환불가 기술
        public PassiveNames[] Passives; // 기술의 패시브
        public PassiveNames[] OverlapPassives; // 기술의 덮어씌워지는 패시브 (기술을 배우면 해당 패시브를 삭제합니다)
        public BuffNames[] Buffs;// 기술의 버프
        public bool AddBuffOnSetup; // 초기화시 버프를 적용합니다. (아이템 기술 / 보조 기술 사용)
        public bool ApplyBuffWithoutAnimation; // 공중 시전시 애니메이션 재생 없이 버프를 적용합니다. (보조 기술 사용)
        public HitmarkNames[] Hitmarks; // 기술의 히트마크
        public bool ActivateHitmarkWithoutAnimation; // 애니메이션 재생 없이 히트마크를 사용합니다. (정예 몬스터 사용)
        public FVNames ForceVelocity; // 기술의 강제이동
        public int Price; // 기술의 구매 비용 (재화)
        public int PricePoint; // 기술의 습득 비용 (기술 수여 포인트)
        public int RequiredPoint;
        public bool UseAddLevel; // 추가 레벨 사용
        public int MaxCount; // 최대 횟수
        public float Cooldown; // 재사용대기시간
        public bool UseCancelDuringCooldowning; // 재사용 대기 중 시전 취소 사용
        public bool StartCooldownWithoutAnimation; // 설정된 애니메이션이 없을 시 즉시 재사용 대기 시작
        public VitalConsumeTypes VitalConsumeType;// 자원 소모 종류
        public float ResourceCostOnStart; // 기술 시작 자원소모값
        public float ResourceCostByLevel;  // 레벨별 자원소모값
        public int MinResourceCost; // 최소 자원값

        public string LinkedItemName; // 연결된 아이템 이름
        public string IconSkillName;// 아이콘에 사용하는 기술이름

        // For InGame
        public ItemNames ItemName;


        //──────────────────────────────────────────────────────────────────────────────────────────

        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public SkillAssetData Clone()
        {
            SkillAssetData assetData = new()
            {
                Name = Name,
                Attribute = Attribute,
                Type = Type,
                Grade = Grade,
                MaxLevel = MaxLevel,
                Description = Description,
                Icon = Icon,
                EffectPrefab = EffectPrefab,

                CooldownType = CooldownType,
                BaseCooldownTime = BaseCooldownTime,
                BaseCooldownAttackCount = BaseCooldownAttackCount,
                CooldownTimeByLevel = CooldownTimeByLevel,
                CooldownAttackCountByLevel = CooldownAttackCountByLevel,

                BaseManaCost = BaseManaCost,
                ManaCostByLevel = ManaCostByLevel,

                BaseEffectValue = BaseEffectValue,
                EffectValueByLevel = EffectValueByLevel,
                EffectMultiplier = EffectMultiplier,
                BaseHitCount = BaseHitCount,
                HitCountByLevel = HitCountByLevel,
                BaseRange = BaseRange,
                RangeByLevel = RangeByLevel,

                HasFreezeEffect = HasFreezeEffect,
                HasStunEffect = HasStunEffect,
                FreezeChance = FreezeChance,
                StunChance = StunChance,
                HasTimeStop = HasTimeStop,
                IsPiercing = IsPiercing,

                LevelUpCosts = LevelUpCosts != null ? (int[])LevelUpCosts.Clone() : null,
            };

            return assetData;
        }
    }
}