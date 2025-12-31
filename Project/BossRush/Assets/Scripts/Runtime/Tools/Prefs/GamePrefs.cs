using System.Linq;

namespace TeamSuneat
{
    public static class GamePrefs
    {
        public static string GetGameName()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return "DEV_PROJECT_SLAYER_";
#else
            return "PROJECT_SLAYER_";
#endif
        }

        private static string GetKey(GamePrefTypes type)
        {
            return $"{GetGameName()}{type.ToUpperString()}";
        }

        private static string GetKey(string type)
        {
            return $"{GetGameName()}{type.ToUpperString()}";
        }

        public static bool HasKey(string type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefsEx.HasKey(key);
            }

            return false;
        }

        public static bool HasKey(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefsEx.HasKey(key);
            }

            return false;
        }

        public static bool GetBool(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefsEx.GetBool(key);
            }

            return false;
        }

        public static bool GetBoolOrDefault(GamePrefTypes type, bool defaultValue)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefsEx.GetBoolOrDefault(key, defaultValue);
            }

            return defaultValue;
        }

        public static int GetInt(GamePrefTypes type, int defaultValue = 0)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefsEx.GetInt(key, defaultValue);
            }

            return defaultValue;
        }

        public static float GetFloat(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefsEx.GetFloat(key);
            }

            return 0;
        }

        public static string GetString(GamePrefTypes type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefsEx.GetString(key);
            }

            return string.Empty;
        }

        public static string GetString(string type)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefsEx.GetString(key);
            }

            return string.Empty;
        }

        public static void SetString(string type, string value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                PlayerPrefsEx.SetString(key, value);
            }
        }

        public static void SetString(GamePrefTypes type, string value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                PlayerPrefsEx.SetString(key, value);
            }
        }

        public static void SetBool(GamePrefTypes type, bool value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                PlayerPrefsEx.SetBool(key, value);
            }
        }

        public static void SetInt(GamePrefTypes type, int value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                PlayerPrefsEx.SetInt(key, value);
            }
        }

        public static void SetFloat(GamePrefTypes type, float value)
        {
            string key = GetKey(type);
            if (false == string.IsNullOrEmpty(key))
            {
                PlayerPrefsEx.SetFloat(key, value);
            }
        }

        public static void ClearOnEntryPoint()
        {
            Delete(GamePrefTypes.EARLY_ACCESS_ENTER_NOTICE);
        }

        public static void Clear()
        {
            PlayerPrefsEx.Clear();
        }

        public static void Delete(GamePrefTypes type)
        {
            string key = GetKey(type);

            if (false == string.IsNullOrEmpty(key))
            {
                PlayerPrefsEx.Delete(key);
            }
        }

        public static void Delete(string type)
        {
            string key = GetKey(type);

            if (false == string.IsNullOrEmpty(key))
            {
                PlayerPrefsEx.Delete(key);
            }
        }

        public static void DeleteAllJoystickInput()
        {
            GamePrefTypes[] types = EnumEx.GetValues<GamePrefTypes>().Where(x => x.ToString().Contains("JOYSTICK")).ToArray();

            for (int i = 0; i < types.Length; i++)
            {
                Delete(types[i]);
            }
        }       
    }
}