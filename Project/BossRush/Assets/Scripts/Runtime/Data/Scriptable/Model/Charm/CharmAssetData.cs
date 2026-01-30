using Sirenix.OdinInspector;
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
        [EnableIf("HasSkillApplication")]
        [SuffixLabel("스킬 이름")]
        [GUIColor("GetSkillNameColor")]
        public SkillName SkillName;

        // 스트링
        [FoldoutGroup("#String")] public string TypeString;
        [FoldoutGroup("#String")] public string ApplicationTypeString;
        [FoldoutGroup("#String")] public string SkillNameString;

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
                if (HasSkillApplication() && !EnumEx.ConvertTo(ref SkillName, SkillNameString))
                {
                    Log.Error("CharmAssetData의 SkillNameString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), SkillNameString);
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            TypeString = Type.ToString();
            ApplicationTypeString = ApplicationType.ToString();
            SkillNameString = SkillName.ToString();

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
                SkillName = SkillName,
            };

            return clone;
        }

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;
            
            if (GameDefine.IS_EDITOR)
            {
                UpdateIfChanged(ref TypeString, Type);
                UpdateIfChanged(ref ApplicationTypeString, ApplicationType);
                UpdateIfChanged(ref SkillNameString, SkillName);
            }

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

        private bool HasSkillApplication()
        {
            if (!GameDefine.IS_EDITOR)
            {
                return false;
            }

            return IsChangingAsset && (ApplicationType & CharmApplicationType.Skill) != 0;
        }

        private void TypeLog()
        {
            if (!GameDefine.IS_EDITOR)
            {
                return;
            }

            if (Type == CharmType.None)
            {
                Log.Warning("CharmAssetData의 Type이 올바르지 않을 수 있습니다. Name:{0}, {1}", Name.ToLogString(), Type);
            }
            if (ApplicationType == CharmApplicationType.None)
            {
                Log.Warning("CharmAssetData의 ApplicationType이 올바르지 않을 수 있습니다. Name:{0}, {1}", Name.ToLogString(), ApplicationType);
            }
        }
    }
}