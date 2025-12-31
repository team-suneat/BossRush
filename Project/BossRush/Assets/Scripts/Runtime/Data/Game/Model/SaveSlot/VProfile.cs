namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public partial class VProfile
    {
        /// <summary> 할당한 아이템의 고유 번호</summary>
        public int IssuedItemSID;
        public VCharacterLevel Level;
        public VCharacterEnhancement Enhancement;
        public VCharacterGrowth Growth;

        public VCharacterWeapon Weapon;
        public VCharacterAccessory Accessory;
        public VCharacterItem Item;
        public VCurrency Currency;

        public VCharacterStage Stage;
        public VCharacterSlot Slot;
        public VCharacterSkill Skill;
        public VStatistics Statistics;

        public void OnLoadGameData()
        {
            CreateEmptyData();

            Weapon.OnLoadGameData();
            Accessory.OnLoadGameData();
            Item.OnLoadGameData();
            Currency.OnLoadGameData();

            Stage.OnLoadGameData();
            Slot.OnLoadGameData();
            Skill.OnLoadGameData();
            Enhancement.OnLoadGameData();
            Growth.OnLoadGameData();
            Statistics.OnLoadGameData();
        }

        public void CreateEmptyData()
        {
            Level ??= VCharacterLevel.CreateDefault();
            Weapon ??= VCharacterWeapon.CreateDefault();
            Accessory ??= VCharacterAccessory.CreateDefault();
            Item ??= VCharacterItem.CreateDefault();
            Currency ??= VCurrency.CreateDefault();
            Stage ??= VCharacterStage.CreateDefault();
            Slot ??= VCharacterSlot.CreateDefault();
            Skill ??= VCharacterSkill.CreateDefault();
            Enhancement ??= VCharacterEnhancement.CreateDefault();
            Growth ??= VCharacterGrowth.CreateDefault();
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