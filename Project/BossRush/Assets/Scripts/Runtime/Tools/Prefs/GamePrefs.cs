using System.Linq;
using UnityEngine;

namespace TeamSuneat
{
    public static class GamePrefs
    {
        private static string GAME_NAME
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || TS_DEVELOPMENT_BUILD
                return "DEV_BOSS_RUSH_";
#else
                return "BOSS_RUSH_";
#endif
            }
        }

        public static bool HasKey(string type)
        {
            string key = GAME_NAME + type.ToUpperString();
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefs.HasKey(key);
            }

            return false;
        }

        public static bool HasKey(GamePrefTypes type)
        {
            string key = GAME_NAME + type.ToUpperString();
            if (false == string.IsNullOrEmpty(key))
            {
                return PlayerPrefs.HasKey(key);
            }

            return false;
        }

        public static bool GetBool(GamePrefTypes type)
        {
            string key = GAME_NAME + type.ToUpperString();

            if (false == string.IsNullOrEmpty(key))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return PlayerPrefs.GetInt(key) == 1;
                }
            }

            return false;
        }

        public static bool GetBoolOrDefault(GamePrefTypes type, bool defaultValue)
        {
            string key = GAME_NAME + type.ToUpperString();

            if (false == string.IsNullOrEmpty(key))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return PlayerPrefs.GetInt(key) == 1;
                }
                else
                {
                    return defaultValue;
                }
            }

            return false;
        }

        public static int GetInt(GamePrefTypes type, int defaultValue = 0)
        {
            string key = GAME_NAME + type.ToUpperString();

            if (false == string.IsNullOrEmpty(key))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return PlayerPrefs.GetInt(key);
                }
            }

            return defaultValue;
        }

        public static float GetFloat(GamePrefTypes type)
        {
            string key = GAME_NAME + type.ToUpperString();

            if (false == string.IsNullOrEmpty(key))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return PlayerPrefs.GetFloat(key);
                }
            }

            return 0;
        }

        public static string GetString(GamePrefTypes type)
        {
            string key = GAME_NAME + type.ToUpperString();

            if (false == string.IsNullOrEmpty(key))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return PlayerPrefs.GetString(key);
                }
            }

            return string.Empty;
        }

        public static string GetString(string type)
        {
            string key = GAME_NAME + type.ToUpperString();
            if (false == string.IsNullOrEmpty(key))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    return PlayerPrefs.GetString(key);
                }
            }

            return string.Empty;
        }

        public static void SetString(string type, string value)
        {
            string key = GAME_NAME + type.ToUpperString();
            if (false == string.IsNullOrEmpty(key))
            {
                Log.Info(LogTags.GamePref, $"Set String. key:({key}), value:({value}).");

                PlayerPrefs.SetString(key, value);
                PlayerPrefs.Save();
            }
        }

        public static void SetString(GamePrefTypes type, string value)
        {
            string key = GAME_NAME + type.ToUpperString();
            if (false == string.IsNullOrEmpty(key))
            {
                Log.Info(LogTags.GamePref, $"Set String. key:({key}), value:({value}).");

                PlayerPrefs.SetString(key, value);
                PlayerPrefs.Save();
            }
        }

        public static void SetBool(GamePrefTypes type, bool value)
        {
            string key = GAME_NAME + type.ToUpperString();
            if (false == string.IsNullOrEmpty(key))
            {
                int valueToInt = value ? 1 : 0;

                Log.Info(LogTags.GamePref, $"Set Int. key:({key}), value:({value.ToBoolString()}).");

                PlayerPrefs.SetInt(key, valueToInt);
                PlayerPrefs.Save();
            }
        }

        public static void SetInt(GamePrefTypes type, int value)
        {
            string key = GAME_NAME + type.ToUpperString();
            if (false == string.IsNullOrEmpty(key))
            {
                Log.Info(LogTags.GamePref, $"Set Int. key:({key}), value:({value}).");

                PlayerPrefs.SetInt(key, value);
                PlayerPrefs.Save();
            }
        }

        public static void SetFloat(GamePrefTypes type, float value)
        {
            string key = GAME_NAME + type.ToUpperString();
            if (false == string.IsNullOrEmpty(key))
            {
                Log.Info(LogTags.GamePref, $"Set Float. key:({key}), value:({value}).");

                PlayerPrefs.SetFloat(key, value);
                PlayerPrefs.Save();
            }
        }

        public static void Clear()
        {
            GamePrefTypes[] types = EnumEx.GetValues<GamePrefTypes>();
            for (int i = 0; i < types.Length; i++)
            {
                Delete(types[i]);
            }

            PlayerPrefs.DeleteAll();
        }

        public static void Delete(GamePrefTypes type)
        {
            string key = GAME_NAME + type.ToUpperString();

            if (false == string.IsNullOrEmpty(key))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }
        }

        public static void Delete(string type)
        {
            string key = GAME_NAME + type.ToUpperString();
            if (false == string.IsNullOrEmpty(key))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(key);
                }
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

        public static void DeleteAllInput()
        {
            Delete(GamePrefTypes.KEYBOARD_MOVEUP);
            Delete(GamePrefTypes.KEYBOARD_MOVEDOWN);
            Delete(GamePrefTypes.KEYBOARD_MOVELEFT);
            Delete(GamePrefTypes.KEYBOARD_MOVERIGHT);
            Delete(GamePrefTypes.KEYBOARD_JUMP);
            Delete(GamePrefTypes.KEYBOARD_ATTACK);
            Delete(GamePrefTypes.KEYBOARD_SUBATTACK);
            Delete(GamePrefTypes.KEYBOARD_CAST1);
            Delete(GamePrefTypes.KEYBOARD_CAST2);
            Delete(GamePrefTypes.KEYBOARD_CAST3);
            Delete(GamePrefTypes.KEYBOARD_CAST4);
            Delete(GamePrefTypes.KEYBOARD_POTION1);
            Delete(GamePrefTypes.KEYBOARD_INTERACT);
            Delete(GamePrefTypes.KEYBOARD_ORDERINTERACT);
            Delete(GamePrefTypes.KEYBOARD_POPUPSKILL);
            Delete(GamePrefTypes.KEYBOARD_POPUPINVENTORY);
            Delete(GamePrefTypes.KEYBOARD_POPUPITEM);
            Delete(GamePrefTypes.KEYBOARD_COMPARE);
            Delete(GamePrefTypes.KEYBOARD_SYNERGY);
            Delete(GamePrefTypes.KEYBOARD_KEYBINDING);
            Delete(GamePrefTypes.KEYBOARD_WORLDDIFFICULTY);
        }
    }
}