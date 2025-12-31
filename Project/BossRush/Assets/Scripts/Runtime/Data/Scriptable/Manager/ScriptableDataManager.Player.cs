namespace TeamSuneat.Data
{
    public partial class ScriptableDataManager
    {
        public PlayerCharacterStatConfigAsset GetPlayerCharacterStatAsset()
        {
            return _playerCharacterStatAsset;
        }

        public void RefreshPlayerCharacterStat()
        {
            _playerCharacterStatAsset?.Refresh();
        }

        #region 경험치 (EXP)

        public ExperienceConfigAsset GetExperienceConfigAsset()
        {
            return _experienceConfigAsset;
        }

        public void RefreshExperienceConfig()
        {
            _experienceConfigAsset?.Refresh();
        }

        #endregion 경험치 (EXP)

        #region 강화 (enhancement)

        public EnhancementConfigAsset GetEnhancementDataAsset()
        {
            return _enhancementDataAsset;
        }

        public EnhancementConfigData GetEnhancementData(StatNames statName)
        {
            if (_enhancementDataAsset == null)
            {
                return null;
            }

            return _enhancementDataAsset.FindEnhancementData(statName);
        }

        public void RefreshEnhancement()
        {
            _enhancementDataAsset?.Refresh();
        }

        #endregion 강화 (enhancement)

        #region 성장 (Growth)

        public GrowthConfigAsset GetGrowthDataAsset()
        {
            return _growthDataAsset;
        }

        public GrowthConfigData GetGrowthData(CharacterGrowthTypes growthType)
        {
            if (_growthDataAsset == null)
            {
                return null;
            }

            return _growthDataAsset.FindGrowthData(growthType);
        }

        public void RefreshGrowth()
        {
            _growthDataAsset?.Refresh();
        }

        #endregion 성장 (Growth)
    }
}