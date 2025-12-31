using System.Collections.Generic;
using System.Linq;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public partial class StatSystem : XBehaviour
    {
        public Character Owner;

        private readonly Dictionary<StatNames, CharacterStat> _stats = new();

        // 핸들러 시스템
        private readonly StatEventHandler _eventHandler;

        private readonly StatLogHandler _logHandler;
        private readonly StatStrategyHandler _strategyHandler;

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// StatSystem 생성자
        /// </summary>
        public StatSystem()
        {
            _eventHandler = new StatEventHandler();
            _logHandler = new StatLogHandler();
            _strategyHandler = new StatStrategyHandler(_eventHandler, _logHandler);

            _strategyHandler.InitializeStrategies(this);
        }

        protected VProfile ProfileInfo => GameApp.GetSelectedProfile();

        public CharacterStat[] AllStats => _stats.Values.ToArray();

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
        }

        public override void AutoNaming()
        {
            SetGameObjectName($"Stat({Owner.Name})");
        }

        public bool ContainsKey(StatNames statName)
        {
            return _stats.ContainsKey(statName);
        }

        public bool ContainsKey(StatNames statName, Component source)
        {
            if (_stats.ContainsKey(statName))
            {
                List<StatModifier> modifiers = _stats[statName].GetModifiersBySource(source);
                if (modifiers.IsValid())
                {
                    return true;
                }
            }

            return false;
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        public void Replace(StatNames statName, float statValue, Component source = null)
        {
            if (statName == StatNames.None) { return; }
            if (_stats.ContainsKey(statName))
            {
                _stats[statName].ClearModifiers();
            }

            if (statValue.IsNaN()) { return; }
            if (statValue.IsZero()) { return; }

            StatData statData = JsonDataManager.FindStatData(statName);
            if (!statData.IsValid()) { return; }

            if (statData.Mod == StatModType.Once)
            {
                OnAdd(statName, 1);
            }
            else
            {
                StatModifier modifier = new()
                {
                    Value = statValue,
                    Type = statData.Mod,
                    Source = source,
                    SourceName = source.name,
                };

                if (!_stats.ContainsKey(statData.Name))
                {
                    CharacterStat characterStat = new(statData.Name, statData.DefaultValue);
                    characterStat.AddModifier(modifier);

                    _stats.Add(characterStat.Name, characterStat);
                }
                else
                {
                    _stats[statData.Name].AddModifier(modifier);
                }

                OnAdd(statData.Name, modifier.Value);
                SendAddStatGlobalEvent(statData.Name);

                _logHandler.LogReplaceStat(statName, modifier);
            }
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        public void OnAdd(StatNames statName, float addStatValue)
        {
            _strategyHandler.ProcessAdd(statName, addStatValue);
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        private void OnRemove(StatNames statName, float removeStatValue)
        {
            _strategyHandler.ProcessRemove(statName, removeStatValue);
        }

        public void Clear()
        {
            _logHandler.LogClearAllStats();

            _stats.Clear();
        }

        public void OnLevelUp()
        {
            _logHandler.LogLevelUp();
        }

        public StatNames[] GetMyStatNames()
        {
            return _stats.Keys.ToArray();
        }

        public CharacterStat GetCharacterStat(StatNames statName)
        {
            if (_stats.ContainsKey(statName))
            {
                return _stats[statName];
            }

            return null;
        }

        public List<StatModifier> GetStatModifiers(StatNames statName)
        {
            if (statName != StatNames.None)
            {
                if (ContainsKey(statName))
                {
                    return _stats[statName].GetModifiers();
                }
            }

            return default;
        }

        public List<StatModifier> GetStatModifiersBySourceType(StatNames statName, string sourceType)
        {
            if (statName != StatNames.None)
            {
                if (ContainsKey(statName))
                {
                    return _stats[statName].GetModifiersByType(sourceType);
                }
            }

            return default;
        }

        public List<StatModifier> GetStatModifiersBySourceName(StatNames statName, string sourceName)
        {
            if (statName != StatNames.None)
            {
                if (ContainsKey(statName))
                {
                    return _stats[statName].GetModifiersByName(sourceName);
                }
            }

            return default;
        }
    }
}