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

        protected Color GetCharacterColor(CharacterNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetStatColor(StatNames key)
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

        protected Color GetLinkedDamageTypeColor(LinkedDamageTypes key)
        {
            return GetFieldColor(key);
        }

        protected Color GetParryTypeColor(ParryTypes key)
        {
            return GetFieldColor(key);
        }

        protected Color GetKnockbackTypeColor(KnockbackType key)
        {
            return GetFieldColor(key);
        }

        protected Color GetAttackTargetTypeColor(AttackTargetTypes key)
        {
            return GetFieldColor(key);
        }

        protected Color GetAttackEntityTypeColor(AttackEntityTypes key)
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

        protected Color GetStageNameColor(StageNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetItemNameColor(ItemNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetCurrencyNameColor(CurrencyNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetForceVelocityColor(FVNames key)
        {
            return GetFieldColor(key);
        }

        protected Color GetCharmTypeColor(CharmType key)
        {
            return GetFieldColor(key);
        }

        protected Color GetCharmApplicationTypeColor(CharmApplicationType key)
        {
            return GetFieldColor(key);
        }

        protected Color GetBuffNameColor(BuffName key)
        {
            return GetFieldColor(key);
        }

        protected Color GetBuffTypeColor(BuffType key)
        {
            return GetFieldColor(key);
        }

        protected Color GetSkillNameColor(SkillName key)
        {
            return GetFieldColor(key);
        }

        protected Color GetPassiveNameColor(PassiveName key)
        {
            return GetFieldColor(key);
        }

        protected Color GetSubjectColor(FVSubjects key)
        {
            return GetFieldColor(key);
        }

        protected Color GetApplicationColor(ApplicationTypes key)
        {
            return GetFieldColor(key);
        }

        protected Color GetStatsColor(StatNames[] stats)
        {
            if (stats == null || stats.Length == 0)
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        protected Color GetDirectionColor(FVDirections key)
        {
            return GetFieldColor(key);
        }

        protected Color GetDirectionalTypeColor(FVDirectionalType key)
        {
            return GetFieldColor(key);
        }

        protected Color GetGravityTypeColor(FVGravityType key)
        {
            return GetFieldColor(key);
        }

        protected Color GetAccelerationTypeColor(FVAccelerationType key)
        {
            return GetFieldColor(key);
        }

        protected Color GetFrictionTypeColor(FVFrictionType key)
        {
            return GetFieldColor(key);
        }

        protected Color GetStopOnCollisionTypeColor(FVStopOnCollisionType key)
        {
            return GetFieldColor(key);
        }

        protected Color GetIgnoreTypeColor(FVIgnoreType key)
        {
            return GetFieldColor(key);
        }

        //

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

        //

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