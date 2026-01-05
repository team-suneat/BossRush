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
    }
}