namespace TeamSuneat
{
    public enum ToggleSlotTypes
    {
        None,

        Equipment, // 장비 - UI EquipmentShop Popup
        Rune,   // 룬 - UI Inventory Popup
        Quest,  // 퀘스트 - UI Worldmap Popup

        TradeWeapons,   // 거래창 - 무기
        TradeArmors,    // 거래창 - 갑옷
        TradeOddments,  // 거래창 - 장신구

        Storage1,   // 창고 - UI Storage Popup
        Storage2,   // 창고 - UI Storage Popup
        Storage3,   // 창고 - UI Storage Popup
        Storage4,   // 창고 - UI Storage Popup

        Status,         // 캐릭터 상태창 - UI Inventory Popup
        Inventory,      // 캐릭터 상태창 - UI Inventory Popup
        CharacterSkill, // 캐릭터 스킬트리 - UI SkillTree Popup

        CharacterItem,  // 캐릭터 아이템 - UI Character Item Popup
        CharacterRelic, // 소지 유물 - UI Character Item Popup
        CharacterEssence, // 소지 정수 - UI Character Item Popup

        Blacksmith,
        ItemEngraving, // 각인 (제련의 망치) - UI Blacksmith Popup
        ItemEnhance, // 아이템 강화 (초월의 불씨) - UI Blacksmith Popup
        ItemTranscend,// 아이템 초월 (룬 담금질) - UI Blacksmith Popup

        ItemSalvage,  // 아이템 분해 - UI Salvage Popup

        PotionCrafting, // 물약 제조 (연금술사) ㅇ
        BookOfRunes, // 전설 장비 제작 (가하즈) ㅇ

        WorldMap,
        Map,

        // 옵션
        Options,

        // 월드 퀘스트 팝업
        MainQuest,
        RequiredQuest,
        SubQuest,
        HuntingQuest,
    }

    public static class ToggleSlotChecker
    {
        public static bool IsStorage(this ToggleSlotTypes toggleSlot)
        {
            switch (toggleSlot)
            {
                case ToggleSlotTypes.Storage1:
                case ToggleSlotTypes.Storage2:
                case ToggleSlotTypes.Storage3:
                case ToggleSlotTypes.Storage4:
                    return true;
            }

            return false;
        }
    }
}
