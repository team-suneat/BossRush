using TeamSuneat.Setting;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class StringDialogueData : IData<string>
    {
        public string ID;
        public string TimelineName;
        public int Index;
        public CharacterNames SpeakerName;
        public string Korean;
        public string English;
        public float Duration;
        public int Arguments;

        public StringDialogueData()
        {
        }

        public string GetKey()
        {
            if (!string.IsNullOrEmpty(ID))
            {
                return ID;
            }

            return $"{TimelineName}_{Index:D2}";
        }

        public void Refresh()
        {
        }

        public void OnLoadData()
        {
            // ID가 비어있으면 자동 생성
            if (string.IsNullOrEmpty(ID) && !string.IsNullOrEmpty(TimelineName))
            {
                ID = GetKey();
            }
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
