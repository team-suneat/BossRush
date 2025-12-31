using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class StatSystem : XBehaviour
    {
        public void Add(StatNames statName, float statValue)
        {
            AddWithSource(statName, statValue, null);
        }

        public void AddWithSource(StatNames statName, float statValue, Component source)
        {
            if (statName == StatNames.None) { return; }
            if (statValue.IsNaN()) { return; }

            StatData statData = JsonDataManager.FindStatData(statName);
            if (statData.IsValid())
            {
                if (statData.Mod == StatModType.Once)
                {
                    OnAdd(statName, statValue);
                }
                else
                {
                    StatModifier modifier = new()
                    {
                        Value = statValue,
                        Type = statData.Mod,
                        Source = source,
                        SourceName = source != null ? source.name : "None",
                    };
                    AddWithModifier(statData, modifier);
                }
            }
            else
            {
                _logHandler.LogStatDataError(statName);
            }
        }

        public void AddWithSourceInfo(StatNames statName, float statValue, Component source, string sourceName, string sourceType)
        {
            if (statName == StatNames.None) { return; }
            if (statValue.IsNaN()) { return; }

            StatData statData = JsonDataManager.FindStatData(statName);
            if (statData.IsValid())
            {
                if (statData.Mod == StatModType.Once)
                {
                    OnAdd(statName, statValue);
                }
                else
                {
                    StatModifier modifier = new()
                    {
                        Value = statValue,
                        Type = statData.Mod,
                        Source = source,
                        SourceName = sourceName,
                        SourceType = sourceType,
                    };
                    AddWithModifier(statData, modifier);
                }
            }
            else
            {
                _logHandler.LogStatDataError(statName);
            }
        }

        public void AddWithModifier(StatData statData, StatModifier modifier)
        {
            if (!statData.IsValid())
            {
                _logHandler.LogStatDataError();
                return;
            }

            if (modifier.Value.IsZero())
            {
                Log.Progress(LogTags.Stat, "능력치의 값이 0일 때, 능력치를 추가하지 않습니다: {0}({1})", statData.Name, statData.Name.ToLogString());
                return;
            }

            StatNames statName = statData.Name;

            if (!_stats.ContainsKey(statName))
            {
                CharacterStat characterStat = new(statName, statData.DefaultValue);
                characterStat.AddModifier(modifier);
                _stats.Add(characterStat.Name, characterStat);
            }
            else
            {
                CharacterStat characterStat = _stats[statName];
                List<StatModifier> modifiers = characterStat.GetModifiers(modifier.SID, modifier.OptionIndex);
                if (modifiers.IsValid())
                {
                    Log.Error($"같은 SID({modifier.SID.ToSelectString()})를 가진 아이템({modifier.SourceName.ToSelectString()})의 능력치({statName.ToLogString()}, {modifier.Value})가 이미 등록되어있습니다. 중복 적용할 수 없습니다.");
                    return;
                }

                _stats[statName].AddModifier(modifier);
            }

            _logHandler.LogAdd(statName, modifier, _stats[statName].Value);
            OnAdd(statName, modifier.Value);
            SendAddStatGlobalEvent(statName);
        }

        private void SendAddStatGlobalEvent(StatNames statName)
        {
            if (!Owner.IsPlayer)
            {
                // 플레이어의 능력치가 아니라면 글로벌 이벤트를 전송하지 않습니다.
                return;
            }

            CoroutineNextFrame(() =>
            {
                GlobalEvent<StatNames>.Send(GlobalEventType.PLAYER_CHARACTER_ADD_STAT, statName);
            });
        }
    }
}