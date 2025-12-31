using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterItem
    {
        public Dictionary<string, VItem> Items = new();
        public List<string> UnlockedItems = new();

        public List<ItemNames> GetItemNames()
        {
            List<ItemNames> itemNames = new();
            ItemNames itemName = ItemNames.None;
            foreach (KeyValuePair<string, VItem> kvp in Items)
            {
                if (EnumEx.ConvertTo(ref itemName, kvp.Key))
                {
                    itemNames.Add(itemName);
                }
            }
            return itemNames;
        }

        public void OnLoadGameData()
        {
            if (Items.IsValid())
            {
                foreach (KeyValuePair<string, VItem> item in Items)
                {
                    item.Value.OnLoadGameData();
                }
            }
        }

        //

        public void UnlockItem(ItemNames itemName)
        {
            string key = itemName.ToString();
            if (!UnlockedItems.Contains(key))
            {
                UnlockedItems.Add(key);
            }
        }

        //

        public void AddItem(ItemNames itemName)
        {
            string key = itemName.ToString();
            if (!Items.ContainsKey(key))
            {
                Items.Add(key, new VItem(itemName));
            }
        }

        public void LevelUpItem(ItemNames itemName)
        {
            string key = itemName.ToString();
            if (Items.ContainsKey(key))
            {
                Items[key].LevelUp();
            }
        }

        public static VCharacterItem CreateDefault()
        {
            return new VCharacterItem();
        }
    }
}