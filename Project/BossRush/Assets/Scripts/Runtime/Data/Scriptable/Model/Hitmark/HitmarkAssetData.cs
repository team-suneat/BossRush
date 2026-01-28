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
        [GUIColor("GetHitmarkColor")]
        [SuffixLabel("히트마크 이름")]
        public HitmarkNames Name;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetAttackTargetTypeColor")]
        [SuffixLabel("목표 설정 방식")]
        public AttackTargetTypes AttackTargetType;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetAttackEntityTypeColor")]
        [SuffixLabel("독립체 방식")]
        public AttackEntityTypes EntityType;

        #region 피해 정보 (Damage Information)

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#피해 정보")]
        [SuffixLabel("피해 종류")]
        [GUIColor("GetDamageTypeColor")]
        public DamageTypes DamageType;

        // 피격

        [FoldoutGroup("#피해 정보")]
        [GUIColor("GetBoolColor")]
        [InfoBox("피격 애니메이션을 재생하지 않으면 피격 FV 또한 적용하지 않습니다.")]
        [SuffixLabel("피격 애니메이션 사용 안함")]
        public bool NotPlayDamageAnimation;

        [FoldoutGroup("#피해 정보")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("자기 자신에게 적용")]
        public bool ApplyToSelf;

        [FoldoutGroup("#피해 정보")]
        [GUIColor("GetFloatColor")]
        [EnableIf("ApplyToSelf")]
        [SuffixLabel("자기 자신에게 적용 배율(%)")]
        public float ApplyMultiplierToSelf;

        // 피해

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [InfoBox("피격자의 생명력 비율이 일정 이하라면 적을 처형합니다.")]
        [SuffixLabel("피격자의 처형 조건 생명력 비율")]
        [Range(0f, 1f)]
        public float ExecutionConditionalTargetLifeRate;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("고정 피해")]
        public float FixedDamage;

        [FoldoutGroup("#피해량")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("고정 성장 피해")]
        public float FixedDamageByLevel;

        #region 연결된 참조 피해량 (Linked Damage)

        [FoldoutGroup("#연결된 참조 피해량")]
        [InfoBox("피해량을 고정으로 설정하지 않고, 해당 값을 찾아 비례 피해를 입힙니다")]
        [GUIColor("GetLinkedDamageTypeColor")]
        [SuffixLabel("연결된 값 종류")]
        public LinkedDamageTypes LinkedDamageType;

        [FoldoutGroup("#연결된 참조 피해량")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [GUIColor("GetStateEffectColor")]
        [SuffixLabel("연결된 상태 이상")]
        public StateEffects LinkedStateEffect;

        [FoldoutGroup("#연결된 참조 피해량")]
        [GUIColor("GetFloatColor")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [SuffixLabel("연결된 값 배율(%)")]
        public float LinkedHitmarkMagnification;

        [FoldoutGroup("#연결된 참조 피해량")]
        [GUIColor("GetFloatColor")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [SuffixLabel("연결된 값 배율(%) (레벨별)")]
        public float LinkedValueMagnificationByLevel;

        [FoldoutGroup("#연결된 참조 피해량")]
        [GUIColor("GetFloatColor")]
        [DisableIf("LinkedDamageType", LinkedDamageTypes.None)]
        [SuffixLabel("연결된 값 배율(%) (스택별)")]
        public float LinkedValueMagnificationByStack;

        #endregion 연결된 참조 피해량 (Linked Damage)

        #endregion 피해 정보 (Damage Information)

        #region 자원 (Resource)

        [FoldoutGroup("#자원")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("자원이 부족하다면 공격 애니메이션을 멈춥니다")]
        public bool StopAttackAnimationOnResourceLack;

        [FoldoutGroup("#자원")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("공격 활성화 자원 소모")]
        public bool UseResourceOnActivate;

        [FoldoutGroup("#자원")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("공격 적용시 자원 사용")]
        public bool UseResourceOnApply;

        [FoldoutGroup("#자원")]
        [SuffixLabel("공격 성공시 자원 사용")]
        [GUIColor("GetBoolColor")]
        public bool UseResourceOnAttackSucceeded;

        [FoldoutGroup("#자원")]
        [SuffixLabel("공격 실패시 자원 사용")]
        [GUIColor("GetBoolColor")]
        public bool UseResourceOnAttackFailed;

        [FoldoutGroup("#자원")]
        [SuffixLabel("공격 비활성화시 자원 소모")]
        [GUIColor("GetBoolColor")]
        public bool UseResourceOnInactive;

        [FoldoutGroup("#자원")]
        [SuffixLabel("자원 소모량")]
        [GUIColor("GetFloatColor")]
        public float UseResourceValue;

        [FoldoutGroup("#자원")]
        [SuffixLabel("자원 회복량")]
        [GUIColor("GetFloatColor")]
        public float RestoreResourceValue;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#자원")]
        [SuffixLabel("자원 소모 방식")]
        [GUIColor("GetVitalConsumeTypeColor")]
        public VitalConsumeTypes ResourceConsumeType;

        [FoldoutGroup("#자원")]
        [InfoBox("자원이 부족해도 잔여 모든 자원을 사용합니다.")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("강제 자원 소모")]
        public bool ForceResourceConsume;

        [FoldoutGroup("#자원")]
        [SuffixLabel("소모를 통한 죽음 무시")]
        [GUIColor("GetBoolColor")]
        public bool IgnoreDeathByConsume;

        [FoldoutGroup("#자원")]
        [SuffixLabel("자원 소모 지연시간")]
        [GUIColor("GetFloatColor")]
        public float ConsumeDelayTime;

        #endregion 자원 (Resource)

        #region 강제 이동 (Force Velocity)

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#강제 이동")]
        [GUIColor("GetForceVelocityColor")]
        [SuffixLabel("공격 시 공격자 FV 이름")]
        public FVNames AttackFVName;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#강제 이동")]
        [GUIColor("GetForceVelocityColor")]
        [SuffixLabel("적중 시 넉백 FV 이름")]
        public FVNames DamageKnockbackFVName;

        #endregion 강제 이동 (Force Velocity)

        #region 패링 (Parry)

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#패링")]
        [GUIColor("GetParryTypeColor")]
        [SuffixLabel("패링 타입")]
        public ParryTypes ParryType;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#패링")]
        [GUIColor("GetKnockbackTypeColor")]
        [SuffixLabel("패링 넉백 타입")]
        public KnockbackType ParryKnockbackType;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#패링")]
        [GUIColor("GetForceVelocityColor")]
        [SuffixLabel("패링 성공 시 넉백 FV 이름")]
        public FVNames ParryKnockbackFVName;

        #endregion 패링 (Parry)

        #region Effect

        [FoldoutGroup("#효과")]
        [SuffixLabel("피격 시 FX 프리팹 이름")]
        public string HitFXPrefabName;

        #endregion Effect

        #region 스트링 (String)

        [FoldoutGroup("#String")] public string DamageTypeString;
        [FoldoutGroup("#String")] public string LinkedDamageTypeString;
        [FoldoutGroup("#String")] public string LinkedStateEffectString;
        [FoldoutGroup("#String")] public string NameOnHitString;
        [FoldoutGroup("#String")] public string DiminishingTypeString;
        [FoldoutGroup("#String")] public string AttackTargetTypeString;
        [FoldoutGroup("#String")] public string ResourceConsumeTypeString;
        [FoldoutGroup("#String")] public string AttackFVNameString;
        [FoldoutGroup("#String")] public string DamageKnockbackFVNameString;
        [FoldoutGroup("#String")] public string ParryTypeString;
        [FoldoutGroup("#String")] public string ParryKnockbackTypeString;
        [FoldoutGroup("#String")] public string ParryKnockbackFVNameString;

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
                if (!EnumEx.ConvertTo(ref ResourceConsumeType, ResourceConsumeTypeString))
                {
                    Log.Error("Hitmark 에셋 데이터의 ResourceConsumeTypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), ResourceConsumeTypeString);
                }
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
                if (!EnumEx.ConvertTo(ref AttackFVName, AttackFVNameString))
                {
                    Log.Error("HitmarkAssetData의 AttackFVName을 변환하지 못합니다. Name:{0}, {1}", Name, AttackFVNameString);
                }
                if (!EnumEx.ConvertTo(ref DamageKnockbackFVName, DamageKnockbackFVNameString))
                {
                    Log.Error("HitmarkAssetData의 DamageKnockbackFVName을 변환하지 못합니다. Name:{0}, {1}", Name, DamageKnockbackFVNameString);
                }
                if (!EnumEx.ConvertTo(ref ParryType, ParryTypeString))
                {
                    Log.Error("HitmarkAssetData의 ParryType을 변환하지 못합니다. Name:{0}, {1}", Name, ParryTypeString);
                }
                if (!EnumEx.ConvertTo(ref ParryKnockbackType, ParryKnockbackTypeString))
                {
                    Log.Error("HitmarkAssetData의 ParryKnockbackType을 변환하지 못합니다. Name:{0}, {1}", Name, ParryKnockbackTypeString);
                }
                if (!EnumEx.ConvertTo(ref ParryKnockbackFVName, ParryKnockbackFVNameString))
                {
                    Log.Error("HitmarkAssetData의 ParryKnockbackFVName을 변환하지 못합니다. Name:{0}, {1}", Name, ParryKnockbackFVNameString);
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            AttackTargetTypeString = AttackTargetType.ToString();
            ResourceConsumeTypeString = ResourceConsumeType.ToString();

            RefreshDamageString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            DamageTypeLog();
            ResourceConsumeTypeLog();
            EnumLog();
        }

        public HitmarkAssetData Clone()
        {
            HitmarkAssetData clone = new()
            {
                Name = Name,
                AttackTargetType = AttackTargetType,
                EntityType = EntityType,

                StopAttackAnimationOnResourceLack = StopAttackAnimationOnResourceLack,
                UseResourceOnActivate = UseResourceOnActivate,
                UseResourceOnInactive = UseResourceOnInactive,
                UseResourceOnApply = UseResourceOnApply,
                UseResourceOnAttackSucceeded = UseResourceOnAttackSucceeded,
                UseResourceOnAttackFailed = UseResourceOnAttackFailed,

                ResourceConsumeType = ResourceConsumeType,
                ForceResourceConsume = ForceResourceConsume,
                IgnoreDeathByConsume = IgnoreDeathByConsume,
                ConsumeDelayTime = ConsumeDelayTime,
                UseResourceValue = UseResourceValue,
                RestoreResourceValue = RestoreResourceValue,

                DamageType = DamageType,
                NotPlayDamageAnimation = NotPlayDamageAnimation,
                ApplyToSelf = ApplyToSelf,
                ApplyMultiplierToSelf = ApplyMultiplierToSelf,

                ExecutionConditionalTargetLifeRate = ExecutionConditionalTargetLifeRate,
                FixedDamage = FixedDamage,
                FixedDamageByLevel = FixedDamageByLevel,

                LinkedDamageType = LinkedDamageType,
                LinkedStateEffect = LinkedStateEffect,
                LinkedHitmarkMagnification = LinkedHitmarkMagnification,
                LinkedValueMagnificationByLevel = LinkedValueMagnificationByLevel,
                LinkedValueMagnificationByStack = LinkedValueMagnificationByStack,

                HitFXPrefabName = HitFXPrefabName,

                AttackFVName = AttackFVName,
                DamageKnockbackFVName = DamageKnockbackFVName,

                ParryType = ParryType,
                ParryKnockbackType = ParryKnockbackType,
                ParryKnockbackFVName = ParryKnockbackFVName,
            };

            return clone;
        }

        //

        private void RefreshDamageString()
        {
            DamageTypeString = DamageType.ToString();
            ParryTypeString = ParryType.ToString();
            ParryKnockbackTypeString = ParryKnockbackType.ToString();
            ParryKnockbackFVNameString = ParryKnockbackFVName.ToString();
            LinkedDamageTypeString = LinkedDamageType.ToString();
            LinkedStateEffectString = LinkedStateEffect.ToString();
            AttackFVNameString = AttackFVName.ToString();
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

        private void ResourceConsumeTypeLog()
        {
#if UNITY_EDITOR            
            if (ResourceConsumeType is VitalConsumeTypes.FixedResource or VitalConsumeTypes.FixedPulse)
            {
                if (UseResourceValue is < 0f or > 1f)
                {
                    Log.Error("HitmarkAssetData의 UseResourceValue가 0~1 범위를 벗어났습니다. Name:{0} ({1}), ResourceConsumeType:{2}, UseResourceValue:{3}", Name, Name.ToLogString(), ResourceConsumeType, UseResourceValue);
                }

                if (RestoreResourceValue is < 0f or > 1f)
                {
                    Log.Error("HitmarkAssetData의 RestoreResourceValue가 0~1 범위를 벗어났습니다. Name:{0} ({1}), ResourceConsumeType:{2}, RestoreResourceValue:{3}", Name, Name.ToLogString(), ResourceConsumeType, RestoreResourceValue);
                }
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
            if (ParryType != another.ParryType) { return false; }
            if (ParryKnockbackType != another.ParryKnockbackType) { return false; }
            if (ParryKnockbackFVName != another.ParryKnockbackFVName) { return false; }
            if (NotPlayDamageAnimation != another.NotPlayDamageAnimation) { return false; }
            if (ApplyToSelf != another.ApplyToSelf) { return false; }
            if (ApplyMultiplierToSelf != another.ApplyMultiplierToSelf) { return false; }
            if (ExecutionConditionalTargetLifeRate != another.ExecutionConditionalTargetLifeRate) { return false; }
            if (FixedDamage != another.FixedDamage) { return false; }
            if (FixedDamageByLevel != another.FixedDamageByLevel) { return false; }
            if (LinkedDamageType != another.LinkedDamageType) { return false; }
            if (LinkedStateEffect != another.LinkedStateEffect) { return false; }
            if (LinkedHitmarkMagnification != another.LinkedHitmarkMagnification) { return false; }
            if (LinkedValueMagnificationByLevel != another.LinkedValueMagnificationByLevel) { return false; }
            if (LinkedValueMagnificationByStack != another.LinkedValueMagnificationByStack) { return false; }
            if (AttackFVName != another.AttackFVName) { return false; }

            return true;
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref AttackTargetTypeString, AttackTargetType);
            UpdateIfChanged(ref ResourceConsumeTypeString, ResourceConsumeType);

            UpdateIfChanged(ref DamageTypeString, DamageType);
            UpdateIfChanged(ref ParryTypeString, ParryType);
            UpdateIfChanged(ref ParryKnockbackTypeString, ParryKnockbackType);
            UpdateIfChanged(ref ParryKnockbackFVNameString, ParryKnockbackFVName);
            UpdateIfChanged(ref LinkedDamageTypeString, LinkedDamageType);
            UpdateIfChanged(ref LinkedStateEffectString, LinkedStateEffect);
            UpdateIfChanged(ref AttackFVNameString, AttackFVName);

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

#endif
    }
}