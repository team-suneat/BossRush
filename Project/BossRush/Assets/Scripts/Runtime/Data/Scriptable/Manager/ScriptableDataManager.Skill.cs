using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 스킬 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Skill Find Methods

        /// <summary>
        /// 스킬 에셋을 찾습니다.
        /// </summary>
        public SkillAsset FindSkill(SkillName key)
        {
            return FindSkill(BitConvert.Enum32ToInt(key));
        }

        private SkillAsset FindSkill(int tid)
        {
            if (_skillAssets.ContainsKey(tid))
            {
                return _skillAssets[tid];
            }

            return null;
        }

        #endregion Skill Find Methods

        #region Skill FindClone Methods

        /// <summary>
        /// 스킬 데이터 클론을 찾습니다.
        /// </summary>
        public SkillAssetData FindSkillClone(SkillName skillName)
        {
            if (skillName != SkillName.None)
            {
                SkillAssetData assetData = FindSkillClone(BitConvert.Enum32ToInt(skillName));
                if (!assetData.IsValid())
                {
                    Log.Warning(LogTags.ScriptableData, "스킬 데이터를 찾을 수 없습니다. {0}({1})", skillName, skillName.ToLogString());
                }
                return assetData;
            }

            return new SkillAssetData();
        }

        public SkillAssetData FindSkillClone(int skillTID)
        {
            if (_skillAssets.ContainsKey(skillTID))
            {
                return _skillAssets[skillTID].CreateDataClone();
            }

#if UNITY_EDITOR
            SkillName skillName = skillTID.ToEnum<SkillName>();
            Log.Warning(LogTags.ScriptableData, "스킬 데이터를 찾을 수 없습니다. {0}({1})", skillName, skillName.ToLogString());
#endif

            return new SkillAssetData();
        }

        #endregion Skill FindClone Methods

        #region Skill Refresh Methods

        /// <summary>
        /// 모든 스킬 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllSkill()
        {
            foreach (KeyValuePair<int, SkillAsset> item in _skillAssets) { Refresh(item.Value); }
        }

        private void Refresh(SkillAsset skillAsset)
        {
            skillAsset?.Refresh();
        }

        #endregion Skill Refresh Methods

        #region Skill Validation Methods

        /// <summary>
        /// 스킬 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidSkillsOnLoadAssets()
        {
#if UNITY_EDITOR
            SkillName[] keys = EnumEx.GetValues<SkillName>();
            int tid = 0;
            for (int i = 1; i < keys.Length; i++)
            {
                tid = BitConvert.Enum32ToInt(keys[i]);
                if (!_skillAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "스킬 에셋이 설정되지 않았습니다. {0}({1})", keys[i], keys[i].ToLogString());
                }
            }
#endif
        }

        #endregion Skill Validation Methods
    }
}
