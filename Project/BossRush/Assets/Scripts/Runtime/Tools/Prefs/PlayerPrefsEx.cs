using UnityEngine;

namespace TeamSuneat
{
    public static class PlayerPrefsEx
    {
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static bool GetBool(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key) == 1;
            }

            return false;
        }

        public static bool GetBoolOrDefault(string key, bool defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key) == 1;
            }

            return defaultValue;
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }

            return defaultValue;
        }

        public static float GetFloat(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key);
            }

            return 0f;
        }

        public static string GetString(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetString(key);
            }

            return string.Empty;
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
            Log.Info(LogTags.GamePref, $"GamePrefs Set Bool. key:({key}), value:({value.ToBoolString()}).");
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
            Log.Info(LogTags.GamePref, $"GamePrefs Set Int. key:({key}), value:({value}).");
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
            Log.Info(LogTags.GamePref, $"GamePrefs Set Float. key:({key}), value:({value}).");
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
            Log.Info(LogTags.GamePref, $"GamePrefs Set String. key:({key}), value:({value}).");
        }

        public static void Delete(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                Log.Info(LogTags.GamePref, $"GamePrefs Delete. key:({key})");
            }
        }

        public static void Clear()
        {
            PlayerPrefs.DeleteAll();
            Log.Info(LogTags.GamePref, $"GamePrefs Clear. All PlayerPrefs deleted.");
        }
    }
}