using System;

namespace TeamSuneat.Data
{
    public partial class SkillAssetData
    {
        public void Validate()
        {
            if (IsChangingAsset)
            {
                return;
            }

            if (!EnumEx.ConvertTo(ref Attribute, AttributeAsString))
            {
                Log.Error("Skill Asset 내 Attribute 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref Type, TypeAsString))
            {
                Log.Error("Skill Asset 내 Type 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref Grade, GradeAsString))
            {
                Log.Error("Skill Asset 내 Grade 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            AttributeAsString = Attribute.ToString();
            TypeAsString = Type.ToString();
            GradeAsString = Grade.ToString();
            CooldownTypeAsString = CooldownType.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();
            CustomLog();
        }

#if UNITY_EDITOR

        private bool _hasChangedWhiteRefreshAll = false;

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref AttributeAsString, Attribute);
            UpdateIfChanged(ref TypeAsString, Type);
            UpdateIfChanged(ref GradeAsString, Grade);
            UpdateIfChanged(ref CooldownTypeAsString, CooldownType);

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

#endif
    }
}