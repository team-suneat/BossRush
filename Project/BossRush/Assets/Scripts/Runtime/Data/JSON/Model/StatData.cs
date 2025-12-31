namespace TeamSuneat.Data
{
    [System.Serializable]
    public class StatData : IData<int>
    {
        public StatNames Name;
        public string DisplayName;
        public string DisplayDesc;
        public float DefaultValue;
        public int Digit;

        public bool UseRange;
        public float MinValue;
        public float MaxValue;

        public StatModType Mod;

        public StatData()
        {
        }

        public int GetKey()
        {
            return Name.ToInt();
        }

        public void Refresh()
        {
        }

        public void OnLoadData()
        {
        }
    }
}