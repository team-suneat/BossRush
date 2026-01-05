namespace TeamSuneat
{
    public class GameDefine
    {
        public static bool IS_DEVELOPMENT_BUILD
        {
            get
            {
#if DEVELOPMENT_BUILD
                return true;
#endif

                return false;
            }
        }

        public static bool IS_EDITOR_OR_DEVELOPMENT_BUILD
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return true;
#endif

                return false;
            }
        }

        public const bool USE_DEBUG_LOG_ERROR = true;
        public const bool USE_DEBUG_LOG_WARNING = true;

        public const float DEFAULT_SCREEN_WIDTH = 1920;
        public const float DEFAULT_SCREEN_HEIGHT = 1080;

        public const bool DEFAULT_GAME_OPTION_VIDEO_FULL_SCREEN = true;
        public const bool DEFAULT_GAME_OPTION_VIDEO_BORDERLESS = false;
        public const bool DEFAULT_GAME_OPTION_VIDEO_V_SYNC = true;

        public const float INPUT_WAIT_TIME = 0.3f;

        public static bool SCRIPTABLE_OBJECT_CHECK_DEFAULT = false;
        public static bool SCRIPTABLE_OBJECT_FORCE_REFRESH_ALL = false;
    }
}