namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public partial class VProfile
    {
        /// <summary> 할당한 아이템의 고유 번호</summary>
        public int IssuedItemSID;
        public VCharacterCharm Charm;
        public VCurrency Currency;
        public VCharacterStage Stage;
        public VCharacterSlot Slot;
        public VStatistics Statistics;

        public void OnLoadGameData()
        {
            CreateEmptyData();

            Charm.OnLoadGameData();
            Currency.OnLoadGameData();

            Stage.OnLoadGameData();
            Slot.OnLoadGameData();
            Statistics.OnLoadGameData();
        }

        public void CreateEmptyData()
        {
            Charm ??= VCharacterCharm.CreateDefault();
            Currency ??= VCurrency.CreateDefault();
            Stage ??= VCharacterStage.CreateDefault();
            Slot ??= VCharacterSlot.CreateDefault();
            Statistics ??= VStatistics.CreateDefault();
        }

        public static VProfile CreateDefault()
        {
            Log.Info(LogTags.GameData, $"새로운 게임 데이터를 생성합니다.");
            VProfile defaultProfile = new();
            defaultProfile.CreateEmptyData();

            return defaultProfile;
        }

        public int GenerateItemSID()
        {
            return ++IssuedItemSID;
        }

        internal int GetAdditionalTreasureClassCurrentDifficulty()
        {
            return 0;
        }
    }
}