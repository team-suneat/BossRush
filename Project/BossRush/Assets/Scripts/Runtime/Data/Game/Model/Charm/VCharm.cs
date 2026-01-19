using System;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharm
    {
        [NonSerialized]
        public CharmName Name;
        public string NameString;
        public int Level;

        private VCharm()
        {
            NameString = string.Empty;
            Level = 0;
        }

        public VCharm(CharmName charmName)
        {
            Name = charmName;
            NameString = charmName.ToString();
            Level = 1;
        }

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }
    }
}
