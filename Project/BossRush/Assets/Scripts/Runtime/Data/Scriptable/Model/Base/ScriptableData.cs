using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.Data
{
    public class ScriptableData<TKey> : IData<TKey>
    {
        public virtual TKey GetKey()
        {
            return default;
        }

        public virtual void Refresh()
        {
        }

        public virtual void OnLoadData()
        {
        }

#if UNITY_EDITOR

        #region Inspector

        protected Color GetPassiveNameColor(PassiveNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetBuffNameColor(BuffNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetBuffTypeColor(BuffTypes key)
        {
            return GetFieldColor(key);
        }

        protected Color GetStatNameColor(StatNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetStateEffectColor(StateEffects key)
        {
            return GetFieldColor(key);
        }

        protected Color GetHitmarkColor(HitmarkNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetDamageTypeColor(DamageTypes damageType)
        {
            if (damageType == 0)
            {
                return GameColors.Red;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetAreaNameColor(AreaNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetStageNameColor(StageNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetItemNameColor(ItemNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetSkillNameColor(SkillNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetCurrencyNameColor(CurrencyNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetBoolColor(bool value)
        {
            if (value == false)
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetFloatColor(float value)
        {
            if (value.IsZero())
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetIntColor(int value)
        {
            if (value == 0)
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetFieldColor<T>(T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        #endregion Inspector

        #region Log

        protected void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.ScriptableData, format, args);
            }
        }

        protected void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.ScriptableData, format, args);
            }
        }

        protected void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.ScriptableData, format, args);
            }
        }

        #endregion Log

#endif
    }
}