using System;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VItem
    {
        [NonSerialized]
        public ItemNames Name;
        public string NameString;

        public int Level;

        public VItem()
        { }

        public VItem(ItemNames itemName)
        {
            Name = itemName;
            NameString = itemName.ToString();
            Level = 1;
        }

        public void OnLoadGameData()
        {
            _ = EnumEx.ConvertTo(ref Name, NameString);
        }

        public void LevelUp()
        {
            Level++;
        }
    }
}