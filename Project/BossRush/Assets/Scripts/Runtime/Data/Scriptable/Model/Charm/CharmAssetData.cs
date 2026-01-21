using Sirenix.OdinInspector;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class CharmAssetData : ScriptableData<int>
    {
        [SuffixLabel("개별 에셋 변경 모드")]
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("부적 이름")]
        public CharmName Name;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("부적 타입")]
        [GUIColor("GetCharmTypeColor")]
        public CharmType Type;

        [FoldoutGroup("#부적 정보")]
        [SuffixLabel("부적 설명")]
        [TextArea(3, 5)]
        public string Description;

        [FoldoutGroup("#부적 정보")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("적용 방식")]
        [GUIColor("GetCharmApplicationTypeColor")]
        public CharmApplicationType ApplicationType;

        [FoldoutGroup("#부적 정보")]
        [EnableIf("HasBuffApplication")]
        [SuffixLabel("버프 이름")]
        [GUIColor("GetBuffNameColor")]
        public BuffName BuffName;

        [FoldoutGroup("#부적 정보")]
        [EnableIf("HasSkillApplication")]
        [SuffixLabel("스킬 이름")]
        [GUIColor("GetSkillNameColor")]
        public SkillName SkillName;

        [FoldoutGroup("#부적 정보")]
        [EnableIf("HasPassiveApplication")]
        [SuffixLabel("패시브 이름")]
        [GUIColor("GetPassiveNameColor")]
        public PassiveName PassiveName;

        // 스트링
        [FoldoutGroup("#String")] public string TypeString;
        [FoldoutGroup("#String")] public string ApplicationTypeString;
        [FoldoutGroup("#String")] public string BuffNameString;
        [FoldoutGroup("#String")] public string SkillNameString;
        [FoldoutGroup("#String")] public string PassiveNameString;

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
                    Log.Error("CharmAssetData의 TypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), TypeString);
                }
                if (!EnumEx.ConvertTo(ref ApplicationType, ApplicationTypeString))
                {
                    Log.Error("CharmAssetData의 ApplicationTypeString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), ApplicationTypeString);
                }
                if (HasBuffApplication() && !EnumEx.ConvertTo(ref BuffName, BuffNameString))
                {
                    Log.Error("CharmAssetData의 BuffNameString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), BuffNameString);
                }
                if (HasSkillApplication() && !EnumEx.ConvertTo(ref SkillName, SkillNameString))
                {
                    Log.Error("CharmAssetData의 SkillNameString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), SkillNameString);
                }
                if (HasPassiveApplication() && !EnumEx.ConvertTo(ref PassiveName, PassiveNameString))
                {
                    Log.Error("CharmAssetData의 PassiveNameString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), PassiveNameString);
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            TypeString = Type.ToString();
            ApplicationTypeString = ApplicationType.ToString();
            BuffNameString = BuffName.ToString();
            SkillNameString = SkillName.ToString();
            PassiveNameString = PassiveName.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            TypeLog();
        }

        public CharmAssetData Clone()
        {
            CharmAssetData clone = new()
            {
                Name = Name,
                Type = Type,
                Description = Description,
                ApplicationType = ApplicationType,
                BuffName = BuffName,
                SkillName = SkillName,
                PassiveName = PassiveName,
            };

            return clone;
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref TypeString, Type);
            UpdateIfChanged(ref ApplicationTypeString, ApplicationType);
            UpdateIfChanged(ref BuffNameString, BuffName);
            UpdateIfChanged(ref SkillNameString, SkillName);
            UpdateIfChanged(ref PassiveNameString, PassiveName);

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

        private bool HasBuffApplication()
        {
            return IsChangingAsset && (ApplicationType & CharmApplicationType.Buff) != 0;
        }

        private bool HasSkillApplication()
        {
            return IsChangingAsset && (ApplicationType & CharmApplicationType.Skill) != 0;
        }

        private bool HasPassiveApplication()
        {
            return IsChangingAsset && (ApplicationType & CharmApplicationType.Passive) != 0;
        }

        private void TypeLog()
        {
            if (Type == CharmType.None)
            {
                Log.Warning("CharmAssetData의 Type이 올바르지 않을 수 있습니다. Name:{0}, {1}", Name.ToLogString(), Type);
            }
            if (ApplicationType == CharmApplicationType.None)
            {
                Log.Warning("CharmAssetData의 ApplicationType이 올바르지 않을 수 있습니다. Name:{0}, {1}", Name.ToLogString(), ApplicationType);
            }
        }

#endif
    }
}
