using System;
using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Setting
{
    public class GameStatistics
    {
        public float StartTime;

        public List<string> DPSKeys = new();
        private Dictionary<string, DamagePerSeconds> _damagePerSeconds = new();

        public void AddDamage(DamageResult damageResult, float attackTime)
        {
            if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                return;
            }

            if (DPSKeys.Count == 0)
            {
                StartTime = attackTime;
            }

            string key = damageResult.HitmarkName.ToString();

            if (!DPSKeys.Contains(key))
            {
                DPSKeys.Add(key);
            }

            if (!_damagePerSeconds.ContainsKey(key))
            {
                DamagePerSeconds dps = new();
                _damagePerSeconds.Add(key, dps);
            }

            _damagePerSeconds[key].Add(damageResult);
        }

        public void Clear()
        {
            DPSKeys.Clear();
            _damagePerSeconds.Clear();

            StartTime = 0;
        }

        public float GetDPS(string key, float nowTime)
        {
            if (!_damagePerSeconds.ContainsKey(key))
            {
                return 0;
            }

            float damage = _damagePerSeconds[key].GetTotalValue();
            if (damage.IsZero())
            {
                return 0;
            }

            float duration = MathF.Floor(nowTime - StartTime);
            if (duration < 1)
            {
                return damage;
            }
            else
            {
                return damage.SafeDivide(duration);
            }
        }

        public float GetPhysicalDPS(string key, float nowTime)
        {
            if (!_damagePerSeconds.ContainsKey(key))
            {
                return 0;
            }

            float damage = _damagePerSeconds[key].FindValue(DamageTypes.Normal);
            if (damage.IsZero())
            {
                return 0;
            }

            float duration = MathF.Floor(nowTime - StartTime);
            if (duration < 1)
            {
                return damage;
            }
            else
            {
                return damage.SafeDivide(duration);
            }
        }

        public float GetMagicalDPS(string key, float nowTime)
        {
            if (!_damagePerSeconds.ContainsKey(key))
            {
                return 0;
            }

            float damage = _damagePerSeconds[key].FindValue(DamageTypes.DamageOverTime);

            if (damage.IsZero())
            {
                return 0;
            }

            float duration = MathF.Floor(nowTime - StartTime);
            if (duration < 1)
            {
                return damage;
            }
            else
            {
                return damage / duration;
            }
        }

        public string GetKeyString(string key)
        {
            if (_damagePerSeconds.ContainsKey(key))
            {
                return _damagePerSeconds[key].KeyString;
            }

            return string.Empty;
        }
    }

    public class DamagePerSeconds
    {
        public Dictionary<DamageTypes, float> Damages = new();
        public string KeyString;

        public void Add(DamageResult damageResult)
        {
            if (!Damages.ContainsKey(damageResult.DamageType))
            {
                Damages.Add(damageResult.DamageType, damageResult.DamageValue);
            }
            else
            {
                Damages[damageResult.DamageType] += damageResult.DamageValue;
            }

            KeyString = damageResult.HitmarkName.ToString();//.ToLogString();
        }

        public float FindValue(DamageTypes damageType)
        {
            if (Damages.ContainsKey(damageType))
            {
                return Damages[damageType];
            }

            return 0;
        }

        public float GetTotalValue()
        {
            float result = 0;
            foreach (KeyValuePair<DamageTypes, float> item in Damages)
            {
                result += item.Value;
            }

            return result;
        }
    }
}