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
    }
}