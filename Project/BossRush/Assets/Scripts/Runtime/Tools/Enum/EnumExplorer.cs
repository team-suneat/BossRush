using System.Collections.Generic;

namespace TeamSuneat
{
    public static class EnumExplorer
    {
        #region 사용하지 않는 능력치

        private static List<StatNames> NotUsedStatNames = new();

        public static void ClearUseStats()
        {
            NotUsedStatNames.Clear();
        }

        private static void InitializedUseStat()
        {
            if (NotUsedStatNames.Count == 0)
            {
                NotUsedStatNames.AddRange(EnumEx.GetValues<StatNames>(true));
            }
        }

        private static void RemoveUseStat(StatNames statName)
        {
            if (NotUsedStatNames.Contains(statName))
            {
                _ = NotUsedStatNames.Remove(statName);
            }
        }

        public static void LogNotUsedStats()
        {
            if (NotUsedStatNames != null)
            {
                for (int i = 0; i < NotUsedStatNames.Count; i++)
                {
                    StatNames statName = NotUsedStatNames[i];
                    Log.Warning("에셋 또는 데이터에서 사용하지 않는 능력치입니다. {0} : {1}",
                        statName.ToSelectString(), statName.ToLogString());
                }
            }
        }

        #endregion 사용하지 않는 능력치

        #region 사용하는 능력치

        private static bool CheckStatName(StatNames statName)
        {
            switch (statName)
            {
                // case StatNames.None:
                //     return true;

                default:
                    return false;
            }
        }

        public static void LogStat(string type, string key, StatNames statName)
        {
#if UNITY_EDITOR
            InitializedUseStat();
            RemoveUseStat(statName);
            if (CheckStatName(statName))
            {
                Log.Info(LogTags.Stat, "{0}(:{1})에서 능력치를 사용합니다. 능력치: {2}", key.ToValueString(), type, statName.ToLogString());
            }
#endif
        }

        public static void LogStat(string type, string key, StatNames[] statNames)
        {
#if UNITY_EDITOR
            InitializedUseStat();
            for (int i = 0; i < statNames.Length; i++)
            {
                RemoveUseStat(statNames[i]);
                if (CheckStatName(statNames[i]))
                {
                    Log.Info(LogTags.Stat, "{0}(:{1})에서 능력치를 사용합니다. 능력치: {2}", key.ToValueString(), type, statNames[i].ToLogString());
                }
            }
#endif
        }

        public static void LogStat(string type, string key, string key2, StatNames statName, float statValue)
        {
#if UNITY_EDITOR
            InitializedUseStat();
            RemoveUseStat(statName);

            if (CheckStatName(statName))
            {
                Log.Info(LogTags.Stat, "{0}({1}, {2})에서 능력치를 사용합니다. 능력치: {3}, 능력치 값: {4}",
                    key.ToValueString(), key2.ToValueString(), type,
                    statName.ToLogString(), statName.GetStatValueString(statValue, true));
            }

#endif
        }

        public static void LogStat(string type, string key, string key2, StatNames[] statNames, float[] statValues)
        {
#if UNITY_EDITOR
            InitializedUseStat();

            for (int i = 0; i < statNames.Length; i++)
            {
                RemoveUseStat(statNames[i]);

                if (CheckStatName(statNames[i]))
                {
                    if (statValues != null && statValues.Length > i)
                    {
                        Log.Info(LogTags.Stat, "{0}({1}, {2})에서 능력치를 사용합니다. 능력치: {3}, 능력치 값: {4}",
                        key.ToValueString(), key2.ToValueString(), type,
                        statNames[i].ToLogString(), statNames[i].GetStatValueString(statValues[i], true));
                    }
                    else
                    {
                        Log.Warning(LogTags.Stat, "{0}({1}, {2})에서 능력치를 사용합니다. 능력치: {3}, 능력치 값을 찾을 수 없습니다.",
                            key.ToValueString(), key2.ToValueString(), type,
                            statNames[i].ToLogString());
                    }
                }
            }
#endif
        }

        #endregion 사용하는 능력치

        #region 사용하는 버프

        private static bool CheckBuffName(BuffNames buffName)
        {
            switch (buffName)
            {
                // case BuffNames.Stun:
                //    return true;

                default:
                    return false;
            }
        }

        public static void LogBuff(string type, string key, BuffNames buffName)
        {
#if UNITY_EDITOR
            if (CheckBuffName(buffName))
            {
                Log.Info(LogTags.Buff, "{0}(:{1})에서 버프를 사용합니다. 능력치: {2}", key.ToValueString(), type, buffName.ToLogString());
            }
#endif
        }

        public static void LogBuff(string type, string key, BuffNames[] buffNames)
        {
#if UNITY_EDITOR
            for (int i = 0; i < buffNames.Length; i++)
            {
                if (CheckBuffName(buffNames[i]))
                {
                    Log.Info(LogTags.Buff, "{0}(:{1})에서 버프를 사용합니다. 능력치: {2}", key.ToValueString(), type, buffNames[i].ToLogString());
                }
            }
#endif
        }

        #endregion 사용하는 버프
    }
}