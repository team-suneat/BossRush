using Sirenix.OdinInspector;
using TeamSuneat;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class SkillAssetData : ScriptableData<int>
    {
        [SuffixLabel("개별 에셋 변경 모드")]
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("스킬 이름")]
        public SkillName Name;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#스킬")]
        [SuffixLabel("스킬 타입")]
        [GUIColor("GetSkillTypeColor")]
        public SkillType Type;

        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#스킬")]
        [SuffixLabel("발동 방식")]
        [GUIColor("GetSkillTriggerTypeColor")]
        public SkillTriggerType TriggerType;

        public bool IsActiveType => Type == SkillType.Active;
        public bool IsInputCastTrigger => TriggerType == SkillTriggerType.InputCast;
        public bool IsOnAcquireTrigger => TriggerType == SkillTriggerType.OnAcquire;
        public bool IsConditionalTrigger => TriggerType == SkillTriggerType.Conditional;

        #region 재사용 대기 시간 (Cooldown)

        [ShowIf("IsActiveType")]
        [FoldoutGroup("#재사용 대기 시간")]
        [SuffixLabel("재사용 대기 시간 (초)")]
        [GUIColor("GetFloatColor")]
        public float Cooldown;

        #endregion 재사용 대기 시간 (Cooldown)

        #region 자원 (Resource)

        [ShowIf("IsActiveType")]
        [FoldoutGroup("#자원")]
        [SuffixLabel("스킬 활성화 자원 소모")]
        public bool UseResourceOnActivate;

        [ShowIf("IsActiveType")]
        [FoldoutGroup("#자원")]
        [SuffixLabel("스킬 적용시 자원 사용")]
        public bool UseResourceOnApply;

        [ShowIf("IsActiveType")]
        [FoldoutGroup("#자원")]
        [SuffixLabel("자원 소모량")]
        [GUIColor("GetFloatColor")]
        public float UseResourceValue;

        [ShowIf("IsActiveType")]
        [FoldoutGroup("#자원")]
        [SuffixLabel("자원 회복량")]
        [GUIColor("GetFloatColor")]
        public float RestoreResourceValue;

        [ShowIf("IsActiveType")]
        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#자원")]
        [SuffixLabel("자원 소모 방식")]
        [GUIColor("GetVitalConsumeTypeColor")]
        public VitalConsumeTypes ResourceConsumeType;

        [ShowIf("IsActiveType")]
        [FoldoutGroup("#자원")]
        [InfoBox("자원이 부족해도 잔여 모든 자원을 사용합니다.")]
        [SuffixLabel("강제 자원 소모")]
        public bool ForceResourceConsume;

        #endregion 자원 (Resource)

        #region 강제 이동 (Force Velocity)

        [ShowIf("IsActiveType")]
        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#강제 이동")]
        [GUIColor("GetForceVelocityColor")]
        [SuffixLabel("스킬 시전 시 시전자 FV 이름")]
        public FVNames SkillFVName;

        #endregion 강제 이동 (Force Velocity)

        #region 버프 및 패시브

        [EnableIf("IsChangingAsset")]
        [ShowIf("IsOnAcquireTrigger")]
        [FoldoutGroup("#버프 및 패시브")]
        [SuffixLabel("버프 이름")]
        [GUIColor("GetBuffNameColor")]
        public BuffName BuffName;

        [EnableIf("IsChangingAsset")]
        [ShowIf("IsOnAcquireTrigger")]
        [FoldoutGroup("#버프 및 패시브")]
        [SuffixLabel("패시브 이름")]
        [GUIColor("GetPassiveNameColor")]
        public PassiveName PassiveName;

        #endregion 버프 및 패시브

        [FoldoutGroup("#String")] public string TypeString;
        [FoldoutGroup("#String")] public string TriggerTypeString;
        [FoldoutGroup("#String")] public string SkillFVNameString;
        [FoldoutGroup("#String")] public string BuffNameString;
        [FoldoutGroup("#String")] public string PassiveNameString;

        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public void Validate()
        {
            if (!IsChangingAsset)
            {
                // Name은 Asset에서 처리
                if (!EnumEx.ConvertTo(ref Type, TypeString))
                {
                    Log.Error("SkillAssetData의 TypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), TypeString);
                }
                if (!EnumEx.ConvertTo(ref TriggerType, TriggerTypeString))
                {
                    Log.Error("SkillAssetData의 TriggerTypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), TriggerTypeString);
                }
                if (!EnumEx.ConvertTo(ref SkillFVName, SkillFVNameString))
                {
                    Log.Error("SkillAssetData의 SkillFVNameString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), SkillFVNameString);
                }
                if (!EnumEx.ConvertTo(ref BuffName, BuffNameString))
                {
                    Log.Error("SkillAssetData의 BuffNameString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), BuffNameString);
                }
                if (!EnumEx.ConvertTo(ref PassiveName, PassiveNameString))
                {
                    Log.Error("SkillAssetData의 PassiveNameString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), PassiveNameString);
                }
            }

            // 기존 에셋 호환: 타입을 설정하지 않았다면 기본값(Active)으로 보정합니다.
            if (Type == SkillType.None)
            {
                Type = SkillType.Active;
            }

            // 기존 에셋 호환: 발동 방식이 없다면 타입 기반으로 기본값을 보정합니다.
            if (TriggerType == SkillTriggerType.None)
            {
                TriggerType = Type == SkillType.Active ? SkillTriggerType.InputCast : SkillTriggerType.OnAcquire;
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            TypeString = Type.ToString();
            TriggerTypeString = TriggerType.ToString();
            SkillFVNameString = SkillFVName.ToString();
            BuffNameString = BuffName.ToString();
            PassiveNameString = PassiveName.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();
        }

        public SkillAssetData Clone()
        {
            SkillAssetData clone = new()
            {
                Name = Name,
                Type = Type,
                TriggerType = TriggerType,
                Cooldown = Cooldown,
                UseResourceOnActivate = UseResourceOnActivate,
                UseResourceOnApply = UseResourceOnApply,
                UseResourceValue = UseResourceValue,
                RestoreResourceValue = RestoreResourceValue,
                ResourceConsumeType = ResourceConsumeType,
                ForceResourceConsume = ForceResourceConsume,
                SkillFVName = SkillFVName,
                BuffName = BuffName,
                PassiveName = PassiveName,
            };

            return clone;
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            // Name은 Asset에서 처리

            return _hasChangedWhiteRefreshAll;
        }

        private bool _hasChangedWhiteRefreshAll = false;

#endif
    }
}
