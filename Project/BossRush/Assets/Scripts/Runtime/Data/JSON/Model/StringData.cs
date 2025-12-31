using TeamSuneat.Setting;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class StringData : IData<string>
    {
        public string ID;
        public string Korean;
        public string English;
        public int Arguments;

        public StringData()
        {
        }

        public string GetKey()
        {
            return ID;
        }

        public void Refresh()
        {
        }

        public void OnLoadData()
        {
        }

        public string GetString(LanguageNames languageName = LanguageNames.None)
        {
            if (languageName == LanguageNames.None)
            {
                languageName = GameSetting.Instance.Language.Name;
            }

            return languageName switch
            {
                LanguageNames.Korean => Korean,
                LanguageNames.English => English,
                _ => English,
            };
        }
    }
}