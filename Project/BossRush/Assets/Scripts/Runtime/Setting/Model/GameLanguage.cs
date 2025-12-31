namespace TeamSuneat.Setting
{
    public class GameLanguage
    {
        public LanguageNames Name { get; private set; }

        public GameLanguage()
        {
            SetDefaultLanguage();
        }

        public void Load()
        {
            if (GamePrefs.HasKey(GamePrefTypes.OPTION_LANGUAGE))
            {
                int tid = GamePrefs.GetInt(GamePrefTypes.OPTION_LANGUAGE);
                LanguageNames loaded = tid.ToEnum<LanguageNames>();
                if (loaded != LanguageNames.None)
                {
                    Name = loaded;
                }
            }

            Log.Info(LogTags.Setting, "게임 언어 불러오기: ", Name.ToString());
        }

        public void SetLanguage(int index)
        {
            SetLanguage((index + 1).ToEnum<LanguageNames>());
        }

        public void SetLanguage(LanguageNames newLanguage)
        {
            if (newLanguage == LanguageNames.None)
            {
                return;
            }

            Name = newLanguage;
            GamePrefs.SetInt(GamePrefTypes.OPTION_LANGUAGE,
                             BitConvert.Enum32ToInt(newLanguage));

            if (!Scenes.XScene.IsChangeScene)
            {
                GlobalEvent.Send(GlobalEventType.GAME_LANGUAGE_CHANGED);
            }

            Log.Info(LogTags.Setting, "게임 언어 설정: ", Name.ToString());
        }

        /// <summary>
        /// enum 순서대로 언어를 한 칸씩 순환합니다.
        /// (None은 건너뜀)
        /// </summary>
        public void SwitchLanguage()
        {
            int count = System.Enum.GetValues(typeof(LanguageNames)).Length;
            int next = ((int)Name + 1) % count;
            if (next == (int)LanguageNames.None)     // None 건너뛰기
            {
                next = (next + 1) % count;
            }

            SetLanguage((LanguageNames)next);
        }

        private void SetDefaultLanguage()
        {
#if UNITY_EDITOR
            Name = LanguageNames.Korean;
#else
            Name = LanguageNames.English;
#endif
        }
    }
}