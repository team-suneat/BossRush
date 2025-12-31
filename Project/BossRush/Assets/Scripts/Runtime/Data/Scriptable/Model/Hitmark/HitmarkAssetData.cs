using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class HitmarkAssetData : ScriptableData<int>
    {
        [SuffixLabel("개별 에셋 변경 모드")]
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("히트마크 이름")]
        public HitmarkNames Name;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("목표 설정 방식")]
        public AttackTargetTypes AttackTargetType;

        [SuffixLabel("군중제어기 여부")]
        public bool IsCrowdControl;

        #region 피해 정보 (Damage Information)

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#피해 정보")]
        [SuffixLabel("피해 종류")]
        [GUIColor("GetDamageTypeColor")]
        public DamageTypes DamageType;

        [FoldoutGroup("#피해 정보 - 토글")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("회피 불가 공격")]
        public bool IgnoreEvasion;

        // 피격

        [FoldoutGroup("#피해 정보 - 토글/애니메이션", true)]
        [GUIColor("GetBoolColor")]
        [Tooltip("피격 애니메이션을 재생하지 않으면 피격 FV 또한 적용하지 않습니다.")]
        [SuffixLabel("피격 애니메이션 사용 안함*")]
        public bool NotPlayDamageAnimation;

        [FoldoutGroup("#피해 정보 - 토글")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("자기 자신에게 적용")]
        public bool ApplyToSelf;

        [FoldoutGroup("#피해 정보 - 토글")]
        [GUIColor("GetFloatColor")]
        [EnableIf("ApplyToSelf")]
        [SuffixLabel("자기 자신에게 적용 배율(%)")]
        public float ApplyMultiplierToSelf;

        // 피해

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [Tooltip("피해 종류가 물리 또는 마법일 때 피격자의 생명력 비율이 일정 이하라면 적을 처형합니다.")]
        [SuffixLabel("피격자의 처형 조건 생명력 비율*")]
        [Range(0f, 1f)]
        public float ExecutionConditionalTargetHealthRate;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("최소 피해량")]
        public int MinDamageValue = 1;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("고정 피해")]
        public float FixedDamage;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("고정 성장 피해")]
        public float FixedDamageByLevel;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("무기 피해 사용")]
        public bool UseWeaponDamage;

        // 연결된 값

        [FoldoutGroup("연결된 참조 피해량")]
        [Tooltip("피해량을 고정으로 설정하지 않고, 해당 값을 찾아 비례 피해를 입힙니다")]
        [GUIColor("GetLinkedDamageTypeColor")]
        [SuffixLabel("연결된 값 종류*")]
        public LinkedDamageTypes LinkedDamageType;

        [FoldoutGroup("연결된 참조 피해량")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [GUIColor("GetStateEffectColor")]
        [SuffixLabel("연결된 상태 이상")]
        public StateEffects LinkedStateEffect;

        [FoldoutGroup("연결된 참조 피해량")]
        [GUIColor("GetFloatColor")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [SuffixLabel("연결된 값 배율(%)")]
        public float LinkedHitmarkMagnification;

        [FoldoutGroup("연결된 참조 피해량")]
        [GUIColor("GetFloatColor")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [SuffixLabel("연결된 값 배율(%) (레벨별)")]
        public float LinkedValueMagnificationByLevel;

        [FoldoutGroup("연결된 참조 피해량")]
        [GUIColor("GetFloatColor")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [SuffixLabel("연결된 값 배율(%) (스택별)")]
        public float LinkedValueMagnificationByStack;

        // 스트링

        [FoldoutGroup("#String")] public string DamageTypeString;
        [FoldoutGroup("#String")] public string LinkedDamageTypeString;
        [FoldoutGroup("#String")] public string LinkedStateEffectString;
        [FoldoutGroup("#String")] public string NameOnHitString;
        [FoldoutGroup("#String")] public string BuffOnHitString;
        [FoldoutGroup("#String")] public string UseWeaponDamageString;

        #endregion 피해 정보 (Damage Information)

        #region 피해 점감 (Damage Diminishing Returns)

        public enum DiminishingTypes
        { None, HitCount, ApplyCount }

        [FoldoutGroup("#피해 점감 (Damage Diminishing Returns)")]
        [SuffixLabel("피해량 점감 종류")]
        [InfoBox("$DiminishingTypeMassage")]
        public DiminishingTypes DiminishingType;

        [NonSerialized]
        public string DiminishingTypeMassage;

        [FoldoutGroup("#피해 점감 (Damage Diminishing Returns)")]
        [DisableIf("DiminishingType", DiminishingTypes.None)]
        [SuffixLabel("피해량 점감 배율")]
        [Range(0, 1)]
        [GUIColor("GetFloatColor")]
        public float DiminishingRate;

        [FoldoutGroup("#피해 점감 (Damage Diminishing Returns)")]
        [DisableIf("DiminishingType", DiminishingTypes.None)]
        [SuffixLabel("최초 피해부터 피해량 점감")]
        public bool ApplyDiminishingFromFirst;

        #endregion 피해 점감 (Damage Diminishing Returns)

        #region 자원 (Resource)

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원이 부족하다면 공격 애니메이션을 멈춥니다")]
        public bool StopAttackAnimationOnResourceLack;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 활성화 자원 소모*")]
        public bool UseResourceOnActivate;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 적용시 자원 사용*")]
        public bool UseResourceOnApply;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격시 자원 사용*")]
        public bool UseResourceOnAttack;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 성공시 자원 사용*")]
        public bool UseResourceOnAttackSucceeded;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 실패시 자원 사용*")]
        public bool UseResourceOnAttackFailed;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("공격 비활성화시 자원 소모")]
        public bool UseResourceOnInactive;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원 소모량")]
        [GUIColor("GetFloatColor")]
        public float UseResourceValue;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원 회복량")]
        [GUIColor("GetFloatColor")]
        public float RestoreResourceValue;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원 소모 방식")]
        public VitalConsumeTypes ResourceConsumeType;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("강제 자원 소모*")]
        [Tooltip("자원이 부족해도 잔여 모든 자원을 사용합니다.")]
        public bool ForceResourceConsume;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("소모를 통한 죽음 무시*")]
        public bool IgnoreDeathByConsume;

        [FoldoutGroup("#자원 (Resource)")]
        [SuffixLabel("자원 소모 지연시간")]
        [GUIColor("GetFloatColor")]
        public float ConsumeDelayTime;

        #endregion 자원 (Resource)

        #region 스트링 (String)

        [FoldoutGroup("#String")] public string AttackTargetTypeString;
        [FoldoutGroup("#String")] public string DiminishingTypeString;
        [FoldoutGroup("#String")] public string ResourceConsumeTypeString;

        #endregion 스트링 (String)

        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public void Validate()
        {
            if (!IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref AttackTargetType, AttackTargetTypeString))
                {
                    Log.Error("Hitmark 에셋 데이터의 AttackTargetTypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), AttackTargetTypeString);
                }
                if (!EnumEx.ConvertTo(ref DiminishingType, DiminishingTypeString))
                {
                    Log.Error("Hitmark 에셋 데이터의 DiminishingTypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), DiminishingTypeString);
                }
                if (!EnumEx.ConvertTo(ref ResourceConsumeType, ResourceConsumeTypeString))
                {
                    Log.Error("Hitmark 에셋 데이터의 ResourceConsumeTypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), ResourceConsumeTypeString);
                }

                ValidateDamageData();
                RefreshDiminishingTypeMassage();
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            AttackTargetTypeString = AttackTargetType.ToString();
            DiminishingTypeString = DiminishingType.ToString();
            ResourceConsumeTypeString = ResourceConsumeType.ToString();

            RefreshDamageString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            DamageTypeLog();
            EnumLog();
        }

        public HitmarkAssetData Clone()
        {
            HitmarkAssetData clone = new()
            {
                Name = Name,
                AttackTargetType = AttackTargetType,

                IsCrowdControl = IsCrowdControl,

                DiminishingType = DiminishingType,
                DiminishingRate = DiminishingRate,
                ApplyDiminishingFromFirst = ApplyDiminishingFromFirst,

                StopAttackAnimationOnResourceLack = StopAttackAnimationOnResourceLack,
                UseResourceOnActivate = UseResourceOnActivate,
                UseResourceOnInactive = UseResourceOnInactive,
                UseResourceOnApply = UseResourceOnApply,
                UseResourceOnAttack = UseResourceOnAttack,
                UseResourceOnAttackSucceeded = UseResourceOnAttackSucceeded,
                UseResourceOnAttackFailed = UseResourceOnAttackFailed,

                ResourceConsumeType = ResourceConsumeType,
                ForceResourceConsume = ForceResourceConsume,
                IgnoreDeathByConsume = IgnoreDeathByConsume,
                ConsumeDelayTime = ConsumeDelayTime,
                UseResourceValue = UseResourceValue,
                RestoreResourceValue = RestoreResourceValue,

                DamageType = DamageType,
                IgnoreEvasion = IgnoreEvasion,
                NotPlayDamageAnimation = NotPlayDamageAnimation,
                ApplyToSelf = ApplyToSelf,
                ApplyMultiplierToSelf = ApplyMultiplierToSelf,

                ExecutionConditionalTargetHealthRate = ExecutionConditionalTargetHealthRate,
                MinDamageValue = MinDamageValue,
                FixedDamage = FixedDamage,
                FixedDamageByLevel = FixedDamageByLevel,
                UseWeaponDamage = UseWeaponDamage,

                LinkedDamageType = LinkedDamageType,
                LinkedStateEffect = LinkedStateEffect,
                LinkedHitmarkMagnification = LinkedHitmarkMagnification,
                LinkedValueMagnificationByLevel = LinkedValueMagnificationByLevel,
                LinkedValueMagnificationByStack = LinkedValueMagnificationByStack,
            };

            return clone;
        }

        //

        private void RefreshDiminishingTypeMassage()
        {
            if (DiminishingType == DiminishingTypes.HitCount)
            {
                DiminishingTypeMassage = "목표 또는 영역 공격 충돌 횟수에 따라 점감이 적용됩니다.\n독립체의 적용 함수에서 충돌 횟수가 초기화됩니다.";
            }
            else if (DiminishingType == DiminishingTypes.ApplyCount)
            {
                DiminishingTypeMassage = "목표 또는 영역 공격 적용 횟수에 따라 점감이 적용됩니다.\n독립체의 초기화 함수에서 적용 횟수가 초기화됩니다.";
            }
            else
            {
                DiminishingTypeMassage = "피해 점감 기능을 사용하지 않습니다.";
            }
        }

        private void ValidateDamageData()
        {
            if (!EnumEx.ConvertTo(ref DamageType, DamageTypeString))
            {
                Log.Error("HitmarkAssetData의 DamageType을 변환하지 못합니다. Name:{0}, {1}", Name, DamageTypeString);
            }
            if (!EnumEx.ConvertTo(ref LinkedDamageType, LinkedDamageTypeString))
            {
                Log.Error("HitmarkAssetData의 LinkedDamageType을 변환하지 못합니다. Name:{0}, {1}", Name, LinkedDamageTypeString);
            }
            if (!EnumEx.ConvertTo(ref LinkedStateEffect, LinkedStateEffectString))
            {
                Log.Error("HitmarkAssetData의 LinkedStateEffect을 변환하지 못합니다. Name:{0}, {1}", Name, LinkedStateEffectString);
            }

            if (!string.IsNullOrWhiteSpace(UseWeaponDamageString))
            {
                if (!bool.TryParse(UseWeaponDamageString, out bool parsedUseWeaponDamage))
                {
                    Log.Error("HitmarkAssetData의 UseWeaponDamage를 변환하지 못합니다. Name:{0}, {1}", Name, UseWeaponDamageString);
                }
                else
                {
                    UseWeaponDamage = parsedUseWeaponDamage;
                }
            }

            DamageTypeLog();
            EnumLog();
        }

        private void RefreshDamageString()
        {
            DamageTypeString = DamageType.ToString();
            LinkedDamageTypeString = LinkedDamageType.ToString();
            LinkedStateEffectString = LinkedStateEffect.ToString();
            UseWeaponDamageString = UseWeaponDamage.ToString();
        }

        private void DamageTypeLog()
        {
#if UNITY_EDITOR
            if (DamageType == DamageTypes.None)
            {
                Log.Warning("HitmarkAssetData의 DamageType이 올바르지 않을 수 있습니다. Name:{0}, {1}", Name.ToLogString(), DamageType);
            }
#endif
        }

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "HitmarkAssetData".ToSelectString();
#endif
        }

        public bool CompareDamage(HitmarkAssetData another)
        {
            if (Name != another.Name) { return false; }
            if (DamageType != another.DamageType) { return false; }
            if (IgnoreEvasion != another.IgnoreEvasion) { return false; }
            if (ApplyToSelf != another.ApplyToSelf) { return false; }
            if (ApplyMultiplierToSelf != another.ApplyMultiplierToSelf) { return false; }
            if (ExecutionConditionalTargetHealthRate != another.ExecutionConditionalTargetHealthRate) { return false; }
            if (MinDamageValue != another.MinDamageValue) { return false; }
            if (FixedDamage != another.FixedDamage) { return false; }
            if (FixedDamageByLevel != another.FixedDamageByLevel) { return false; }
            if (UseWeaponDamage != another.UseWeaponDamage) { return false; }
            if (LinkedDamageType != another.LinkedDamageType) { return false; }
            if (LinkedStateEffect != another.LinkedStateEffect) { return false; }
            if (LinkedHitmarkMagnification != another.LinkedHitmarkMagnification) { return false; }
            if (LinkedValueMagnificationByLevel != another.LinkedValueMagnificationByLevel) { return false; }
            if (LinkedValueMagnificationByStack != another.LinkedValueMagnificationByStack) { return false; }
            if (NotPlayDamageAnimation != another.NotPlayDamageAnimation) { return false; }

            return true;
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref AttackTargetTypeString, AttackTargetType);
            UpdateIfChanged(ref DiminishingTypeString, DiminishingType);
            UpdateIfChanged(ref ResourceConsumeTypeString, ResourceConsumeType);

            UpdateIfChanged(ref DamageTypeString, DamageType);
            UpdateIfChanged(ref LinkedDamageTypeString, LinkedDamageType);
            UpdateIfChanged(ref LinkedStateEffectString, LinkedStateEffect);
            UpdateIfChangedBool(ref UseWeaponDamageString, UseWeaponDamage);

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

        private void UpdateIfChangedArray(ref string[] target, string[] newArray)
        {
            if (!target.SequenceEqual(newArray))
            {
                target = newArray;
                _hasChangedWhiteRefreshAll = true;
            }
        }

        private void UpdateIfChangedBool(ref string target, bool newValue)
        {
            string newString = newValue.ToString();
            if (target != newString)
            {
                target = newString;
                _hasChangedWhiteRefreshAll = true;
            }
        }

        private Color GetLinkedDamageTypeColor(LinkedDamageTypes key)
        {
            return GetFieldColor(key);
        }

#endif
    }
}