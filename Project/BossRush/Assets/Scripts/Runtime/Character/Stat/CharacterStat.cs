using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [Serializable]
    public class CharacterStat
    {
        public StatNames Name;
        public float BaseValue;
        private float _additionalMaxValue;

        private float _totalValue;
        private bool _isDirty;

        private readonly List<StatModifier> _statModifiers;

        [NonSerialized]
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    _totalValue = CalculateFinalValue();
                    SetDirtyState(false, "값 재계산 완료");
                }
                return _totalValue;
            }
        }

        public int ModifierCount => _statModifiers.Count;

        public string ValueString
        {
            get
            {
                if (_statModifiers != null && _statModifiers.Count > 0)
                {
                    switch (_statModifiers[0].Type)
                    {
                        case StatModType.PercentAdd:
                        case StatModType.PercentMulti:
                            return ValueStringEx.GetPercentString(Value, true);
                    }
                }

                return ValueStringEx.GetValueString(Value, true);
            }
        }

        public CharacterStat()
        {
            _statModifiers = new List<StatModifier>();
            StatModifiers = _statModifiers.AsReadOnly();
        }

        public CharacterStat(StatNames statName, float statDefaultValue)
        {
            _statModifiers = new List<StatModifier>();
            StatModifiers = _statModifiers.AsReadOnly();

            Name = statName;
            BaseValue = statDefaultValue;
        }

        #region Modifier

        private void SetDirtyState(bool isDirty, string reason)
        {
            if (_isDirty == isDirty)
            {
                return;
            }

            _isDirty = isDirty;

            if (Log.LevelInfo)
            {
                string stateText = isDirty ? "갱신 필요" : "최신 상태";
                string reasonText = string.IsNullOrEmpty(reason) ? "사유 미기재" : reason;
                Log.Info(LogTags.Stat, "{0} 능력치 캐시 상태 변경: {1} (사유: {2}) >> {3}", Name.ToLogString(), stateText, reasonText, CalculateFinalValue());
            }
        }

        public void MarkDirty(string reason = "외부 요청")
        {
            SetDirtyState(true, reason);
        }

        public void AddModifier(StatModifier modifier)
        {
            _statModifiers.Add(modifier);
            _statModifiers.Sort(CompareModifierOrder);
            MarkDirty("수정자 추가");
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (_statModifiers.Contains(modifier))
            {
                if (_statModifiers.Remove(modifier))
                {
                    MarkDirty("수정자 제거");
                    return true;
                }
            }
            else
            {
                Log.Error($"능력치({Name.ToLogString()})를 삭제할 수 없습니다. 저장된 modifier가 존재하지 않습니다.");
            }

            return false;
        }

        public void ClearModifiers()
        {
            _statModifiers.Clear();
            MarkDirty("수정자 전체 초기화");
        }

        //───────────────────────────────────────────────────────────────────────────────────────

        public List<StatModifier> GetModifiers()
        {
            return _statModifiers;
        }

        public StatModifier GetStatModifier(float Value)
        {
            for (int i = 0; i < _statModifiers.Count; i++)
            {
                if (_statModifiers[i].Value == Value)
                {
                    return _statModifiers[i];
                }
            }

            return null;
        }

        public List<StatModifier> GetModifiers(Component source)
        {
            List<StatModifier> result = new();

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                if (_statModifiers[i].Source == null)
                {
                    continue;
                }
                if (_statModifiers[i].Source != source)
                {
                    continue;
                }

                result.Add(_statModifiers[i]);
            }

            return result;
        }

        public List<StatModifier> GetModifiers(Type sourceType)
        {
            List<StatModifier> result = new();

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                if (_statModifiers[i].Source == null)
                {
                    continue;
                }

                if (_statModifiers[i].Source != null)
                {
                    if (_statModifiers[i].Source.GetType() != sourceType)
                    {
                        continue;
                    }
                }

                result.Add(_statModifiers[i]);
            }

            return result;
        }

        public List<StatModifier> GetModifiersByName(string sourceName)
        {
            List<StatModifier> result = new();

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                if (_statModifiers[i].SourceName != sourceName)
                {
                    continue;
                }

                result.Add(_statModifiers[i]);
            }

            return result;
        }

        public List<StatModifier> GetModifiersByType(string sourceType)
        {
            List<StatModifier> result = new();

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                if (_statModifiers[i].SourceType != sourceType)
                {
                    continue;
                }

                result.Add(_statModifiers[i]);
            }

            return result;
        }

        public List<StatModifier> GetModifiersBySource(Component source)
        {
            List<StatModifier> result = new();

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                if (_statModifiers[i].Source != source)
                {
                    continue;
                }

                result.Add(_statModifiers[i]);
            }

            return result;
        }

        public List<StatModifier> GetModifiers(SID itemSID)
        {
            List<StatModifier> result = new();

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                if (_statModifiers[i].SID == 0) { continue; }
                if (_statModifiers[i].SID != itemSID) { continue; }

                result.Add(_statModifiers[i]);
            }

            return result;
        }

        public List<StatModifier> GetModifiers(SID itemSID, int optionIndex)
        {
            List<StatModifier> result = new();

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                if (_statModifiers[i].SID == 0) { continue; }
                if (_statModifiers[i].SID != itemSID) { continue; }
                if (_statModifiers[i].OptionIndex != optionIndex) { continue; }

                result.Add(_statModifiers[i]);
            }

            return result;
        }

        #endregion Modifier

        #region Additional

        public void SetAdditionalValue(float additionalMaxValue)
        {
            _additionalMaxValue = additionalMaxValue;
        }

        #endregion Additional

        #region Find

        public float FindValueBySource(Component source)
        {
            float result = 0;
            List<StatModifier> modifiers = GetModifiers(source);

            for (int i = 0; i < modifiers.Count; i++)
            {
                result += modifiers[i].Value;
            }

            return result;
        }

        public float FindValueByType(Type sourceType)
        {
            float result = 0;
            List<StatModifier> modifiers = GetModifiers(sourceType);

            for (int i = 0; i < modifiers.Count; i++)
            {
                result += modifiers[i].Value;
            }

            return result;
        }

        public float FindValueByType(string sourceType)
        {
            float result = 0;
            List<StatModifier> modifiers = GetModifiersByType(sourceType);

            for (int i = 0; i < modifiers.Count; i++)
            {
                result += modifiers[i].Value;
            }

            return result;
        }

        #endregion Find

        #region Calculate

        private float CalculateFinalValue()
        {
            float finalValue = BaseValue;

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                StatModifier mod = _statModifiers[i];
                switch (mod.Type)
                {
                    case StatModType.Flat:
                    case StatModType.PercentAdd:
                        {
                            finalValue += mod.Value;
                        }
                        break;

                    case StatModType.PercentMulti:
                        {
                            finalValue *= 1 + mod.Value;
                        }
                        break;

                    case StatModType.Use:
                        {
                            finalValue = 1;
                        }
                        break;
                }
            }

            StatData statData = JsonDataManager.FindStatData(Name);
            if (statData.IsValid() && statData.UseRange)
            {
                float minValue = statData.MinValue;
                float maxValue = statData.MaxValue + _additionalMaxValue;

                if (finalValue < minValue)
                {
                    if (Log.LevelWarning)
                    {
                        Log.Warning(LogTags.Stat, "{0} 능력치 값이 너무 낮습니다. 허용 범위의 최소값을 적용합니다. {1} ▶ {2}", Name.ToLogString(), finalValue, minValue);
                    }
                }
                else if (finalValue > maxValue)
                {
                    if (Log.LevelWarning)
                    {
                        Log.Warning(LogTags.Stat, "{0} 능력치 값이 너무 높습니다. 허용 범위의 최대값을 적용합니다. {1} ▶ {2}", Name.ToLogString(), finalValue, maxValue);
                    }
                }

                finalValue = Mathf.Clamp(finalValue, minValue, maxValue);
            }

            return (float)Math.Round(finalValue, 4);
        }

        private int CompareModifierOrder(StatModifier modifierA, StatModifier modifierB)
        {
            if (modifierA.Order < modifierB.Order)
            {
                return -1;
            }
            else if (modifierA.Order > modifierB.Order)
            {
                return 1;
            }

            return 0;
        }

        #endregion Calculate
    }
}