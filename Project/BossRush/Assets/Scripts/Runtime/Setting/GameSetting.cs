using TeamSuneat;

namespace TeamSuneat.Setting
{
    public class GameSetting : Singleton<GameSetting>
    {
        public GameAudio Audio = new GameAudio();
        public GameLanguage Language = new GameLanguage();
        public GameInput Input = new GameInput();
        public GameCheat Cheat = new GameCheat();
        public GameStatistics Statistics = new GameStatistics();
        public GamePlay Play = new GamePlay();

        public void Initialize()
        {
            Audio.Load();
            Language.Load();
            Input.ResetInput();
            Statistics.Clear();
            Play.Load();
        }
    }
}