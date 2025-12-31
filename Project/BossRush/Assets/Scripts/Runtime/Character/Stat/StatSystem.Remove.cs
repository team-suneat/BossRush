using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public partial class StatSystem : XBehaviour
    {
        public void RemoveByValue(StatNames statName, float statValue)
        {
            if (ContainsKey(statName))
            {
                StatModifier statModifier = _stats[statName].GetStatModifier(statValue);
                RemoveModifier(statName, statModifier);
                OnRemove(statName, statValue);
            }
        }

        public void RemoveBySID(StatNames statName, int itemSID)
        {
            if (ContainsKey(statName))
            {
                List<StatModifier> statModifiers = _stats[statName].GetModifiers(itemSID);
                if (statModifiers.IsValid())
                {
                    _logHandler.LogItemRemoval(statName);

                    float removedStatValue = 0f;
                    for (int i = 0; i < statModifiers.Count; i++)
                    {
                        removedStatValue += statModifiers[i].Value;

                        RemoveModifier(statName, statModifiers[i]);
                    }

                    OnRemove(statName, removedStatValue);

                    _ = StartXCoroutine(SendRemoveStatGlobalEvent(statName));
                }
            }
        }

        public void RemoveBySource(StatNames statName, Component source)
        {
            if (ContainsKey(statName))
            {
                List<StatModifier> statModifiers = _stats[statName].GetModifiers(source);
                if (statModifiers.IsValid())
                {
                    _logHandler.LogComponentRemoval(source, statName);

                    float removedStatValue = 0f;
                    for (int i = 0; i < statModifiers.Count; i++)
                    {
                        removedStatValue += statModifiers[i].Value;

                        RemoveModifier(statName, statModifiers[i]);
                    }

                    OnRemove(statName, removedStatValue);

                    _ = StartXCoroutine(SendRemoveStatGlobalEvent(statName));
                }
            }
        }

        public void RemoveBySourceInfo(StatNames statName, Component source, string sourceName, string sourceType)
        {
            if (ContainsKey(statName))
            {
                List<StatModifier> statModifiers = _stats[statName].GetModifiers(source);
                if (statModifiers.IsValid())
                {
                    _logHandler.LogComponentRemoval(source, statName);

                    float removedStatValue = 0f;
                    for (int i = 0; i < statModifiers.Count; i++)
                    {
                        StatModifier modifier = statModifiers[i];
                        if (modifier.SourceName != sourceName) continue;
                        if (modifier.SourceType != sourceType) continue;

                        removedStatValue += modifier.Value;
                        RemoveModifier(statName, modifier);
                    }

                    OnRemove(statName, removedStatValue);

                    _ = StartXCoroutine(SendRemoveStatGlobalEvent(statName));
                }
            }
        }

        public void RemoveByModifier(StatNames statName, StatModifier statModifier)
        {
            if (statModifier == null)
            {
                return;
            }

            if (ContainsKey(statName))
            {
                RemoveModifier(statName, statModifier);
                OnRemove(statName, statModifier.Value);
                _ = StartXCoroutine(SendRemoveStatGlobalEvent(statName));
            }
        }

        //

        private void RemoveModifier(StatNames statName, StatModifier statModifier)
        {
            _ = _stats[statName].RemoveModifier(statModifier);
            if (_stats[statName].ModifierCount == 0)
            {
                _ = _stats.Remove(statName);
            }
            _logHandler.LogModifierRemoval(statName, statModifier);
        }

        private IEnumerator SendRemoveStatGlobalEvent(StatNames statName)
        {
            if (Owner.IsPlayer)
            {
                yield return null;
                _ = GlobalEvent<StatNames>.Send(GlobalEventType.PLAYER_CHARACTER_REMOVE_STAT, statName);
            }
        }
    }
}