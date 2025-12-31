namespace TeamSuneat.Data
{
    public partial class ScriptableDataManager
    {
        #region 몬스터 능력치 (Stat)
        public MonsterStatConfigAsset GetMonsterStatConfigAsset()
        {
            return _monsterStatConfigAsset;
        }

        public void RefreshMonsterStatConfig()
        {
            _monsterStatConfigAsset?.Refresh();
        }
        #endregion Monster Drop

        #region 몬스터 드랍 (Drop)

        public MonsterDropConfigAsset GetMonsterDropConfigAsset()
        {
            return _monsterDropConfigAsset;
        }

        public void RefreshMonsterDropConfig()
        {
            _monsterDropConfigAsset?.Refresh();
        }

        #endregion Monster Drop
    }
}