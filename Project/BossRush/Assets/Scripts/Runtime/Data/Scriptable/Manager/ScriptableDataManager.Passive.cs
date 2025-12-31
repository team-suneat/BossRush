using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 패시브 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Passive Get Methods

        /// <summary>
        /// 패시브 에셋을 가져옵니다.
        /// </summary>
        public PassiveAsset GetPassiveAsset(PassiveNames passiveName)
        {
            int key = BitConvert.Enum32ToInt(passiveName);
            return _passiveAssets.TryGetValue(key, out var asset) ? asset : null;
        }

        #endregion Passive Get Methods

        #region Passive Find Methods

        /// <summary>
        /// 패시브 에셋을 찾습니다.
        /// </summary>
        public PassiveAsset FindPassive(PassiveNames key)
        {
            return FindPassive(BitConvert.Enum32ToInt(key));
        }

        private PassiveAsset FindPassive(int tid)
        {
            if (_passiveAssets.ContainsKey(tid))
            {
                return _passiveAssets[tid];
            }

            return null;
        }

        #endregion Passive Find Methods

        #region Passive FindClone Methods

        /// <summary>
        /// 패시브 트리거 설정을 찾습니다.
        /// </summary>
        public PassiveTriggerSettings FindPassiveTrigger(PassiveNames passiveName)
        {
            if (passiveName != PassiveNames.None)
            {
                int passiveTID = BitConvert.Enum32ToInt(passiveName);
                if (_passiveAssets.ContainsKey(passiveTID))
                {
                    return _passiveAssets[passiveTID].TriggerSettings;
                }
                else
                {
                    Log.Warning(LogTags.ScriptableData, "패시브의 트리거(Trigger) 데이터를 찾을 수 없습니다. {0} ({1})", passiveName, passiveName.ToLogString());
                }
            }

            return null;
        }

        /// <summary>
        /// 패시브 조건 설정을 찾습니다.
        /// </summary>
        public PassiveConditionSettings FindPassiveCondition(PassiveNames passiveName)
        {
            if (passiveName != PassiveNames.None)
            {
                int passiveTID = BitConvert.Enum32ToInt(passiveName);
                if (_passiveAssets.ContainsKey(passiveTID))
                {
                    return _passiveAssets[passiveTID].ConditionSettings;
                }
                else
                {
                    Log.Warning(LogTags.ScriptableData, "패시브의 조건(Condition) 데이터를 찾을 수 없습니다. {0} ({1})", passiveName, passiveName.ToLogString());
                }
            }

            return null;
        }

        /// <summary>
        /// 패시브 효과 설정을 찾습니다.
        /// </summary>
        public PassiveEffectSettings FindPassiveEffect(PassiveNames passiveName)
        {
            if (passiveName != PassiveNames.None)
            {
                int passiveTID = BitConvert.Enum32ToInt(passiveName);
                if (_passiveAssets.ContainsKey(passiveTID))
                {
                    return _passiveAssets[passiveTID].EffectSettings;
                }
                else
                {
                    Log.Warning(LogTags.ScriptableData, "패시브의 효과(Effect) 데이터를 찾을 수 없습니다. {0} ({1})", passiveName, passiveName.ToLogString());
                }
            }

            return null;
        }

        #endregion Passive FindClone Methods

        #region Passive Load Methods

        /// <summary>
        /// 패시브 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadPassiveSync(string filePath)
        {
            if (!filePath.Contains("Passive_"))
            {
                return false;
            }

            PassiveAsset asset = ResourcesManager.LoadResource<PassiveAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 패시브 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_passiveAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 패시브가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _passiveAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _passiveAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Passive Load Methods

        #region Passive Refresh Methods

        /// <summary>
        /// 모든 패시브 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllPassive()
        {
            foreach (KeyValuePair<int, PassiveAsset> item in _passiveAssets) { Refresh(item.Value); }
        }

        private void Refresh(PassiveAsset passiveAsset)
        {
            passiveAsset?.Refresh();
        }

        #endregion Passive Refresh Methods

        #region Passive Validation Methods

        /// <summary>
        /// 패시브 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidPassivesOnLoadAssets()
        {
#if UNITY_EDITOR
            PassiveNames[] keys = EnumEx.GetValues<PassiveNames>();
            int tid = 0;
            for (int i = 1; i < keys.Length; i++)
            {
                tid = BitConvert.Enum32ToInt(keys[i]);
                if (!_passiveAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "패시브 에셋이 설정되지 않았습니다. {0}({1})", keys[i], keys[i].ToLogString());
                }
            }
#endif
        }

        /// <summary>
        /// 사용하지 않는 패시브 트리거를 검사합니다.
        /// </summary>
        private void CheckNotUsePassiveTrigger()
        {
#if UNITY_EDITOR

            Dictionary<PassiveTriggers, int> _triggerUseCount = new();
            foreach (PassiveAsset asset in _passiveAssets.Values)
            {
                if (asset.TriggerSettings == null)
                {
                    continue;
                }

                PassiveTriggers trigger = asset.TriggerSettings.Trigger;
                if (_triggerUseCount.ContainsKey(trigger))
                {
                    _triggerUseCount[trigger] += 1;
                }
                else
                {
                    _triggerUseCount.Add(trigger, 1);
                }
            }

            PassiveTriggers[] triggers = EnumEx.GetValues<PassiveTriggers>(true);
            for (int triggerIndex = 0; triggerIndex < triggers.Length; triggerIndex++)
            {
                PassiveTriggers trigger = triggers[triggerIndex];
                if (!_triggerUseCount.ContainsKey(trigger))
                {
                    Log.Warning(LogTags.ScriptableData, $"{trigger.ToLogString()}를 사용하지 않습니다.");
                }
            }

            Log.Info(LogTags.ScriptableData, $"패시브 트리거 {triggers.Length}개 중 {_triggerUseCount.Count}개를 사용합니다.");

#endif
        }

        #endregion Passive Validation Methods
    }
}