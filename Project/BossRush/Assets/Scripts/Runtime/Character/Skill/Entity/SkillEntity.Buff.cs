using Lean.Pool;
using TeamSuneat.Data;


namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        /// <summary> 버프를 추가합니다. </summary>
        private void AddBuffsOnSetup()
        {
            if (!_skillData.IsValid()) return;
            if (!_skillData.AddBuffOnSetup) return;
            if (!_skillData.Buffs.IsValidArray()) return;

            for (int i = 0; i < _skillData.Buffs.Length; i++)
            {
                BuffNames buffName = _skillData.Buffs[i];
                if (buffName == BuffNames.None) { break; }

                BuffAssetData assetData = ScriptableDataManager.Instance.FindBuffClone(buffName);
                if (!assetData.IsValid()) { continue; }

                if (Owner.Buff.ContainsKey(buffName))
                {
                    Owner.Buff.SetLevel(buffName, Level);
                }
                else
                {
                    Owner.Buff.Add(assetData, Level, Owner);
                }
            }
        }

        /// <summary> 버프를 추가합니다. </summary>
        private void AddBuffs()
        {
            if (_skillData.IsValid())
            {
                BuffAssetData assetData;

                for (int i = 0; i < _skillData.Buffs.Length; i++)
                {
                    BuffNames buffName = _skillData.Buffs[i];
                    if (buffName == BuffNames.None)
                    {
                        break;
                    }

                    assetData = ScriptableDataManager.Instance.FindBuffClone(buffName);
                    if (assetData.IsValid())
                    {
                        Owner.Buff.Add(assetData, Level, Owner);
                    }
                }
            }
        }

        /// <summary> 활성화 단계 버프를 로드합니다. </summary>
        private void LoadActivatingStageBuffs()
        {
            if (_skillData.Passives.IsValidArray())
            {
                BuffNames activatingBuff = BuffNames.None;

                for (int i = 0; i < _skillData.Passives.Length; i++)
                {
                    PassiveNames passiveName = _skillData.Passives[i];

                    PassiveAsset passiveAsset = ScriptableDataManager.Instance.FindPassive(passiveName);
                    if (!passiveAsset.IsValid())
                    {
                        continue;
                    }
                    if (null == passiveAsset.EffectSettings)
                    {
                        continue;
                    }
                    if (!passiveAsset.EffectSettings.Buffs.IsValidArray())
                    {
                        continue;
                    }

                    for (int j = 0; j < passiveAsset.EffectSettings.Buffs.Length; j++)
                    {
                        BuffNames buffName = passiveAsset.EffectSettings.Buffs[j];
                        if (!ProfileInfo.Statistics.Contains(buffName))
                        {
                            continue;
                        }

                        BuffAssetData buffAssetData = ScriptableDataManager.Instance.FindBuffClone(buffName);
                        if (!buffAssetData.IsValid())

                        {
                            continue;
                        }
                        if (activatingBuff == buffName)
                        {
                            continue;
                        }

                        // 이미 등록되어 있는 버프라면 새 엔티티를 초기화하지 않고 추가하지 않습니다.
                        if (!Owner.Buff.ContainsKey(buffName))
                        {
                            Owner.Buff.Add(buffAssetData, Level, Owner);
                        }
                        else
                        {
                            BuffEntity entity = Owner.Buff.Find(buffName);
                            entity.AddUIBuffItem();
                        }

                        LogInfoActivatingStageBuffLoaded(buffName);
                    }
                }
            }
        }

        /// <summary> 버프를 제거합니다. </summary>
        private void RemoveBuffs()
        {
            if (_skillData.IsValid())
            {
                for (int i = 0; i < _skillData.Buffs.Length; i++)
                {
                    if (_skillData.Buffs[i] == BuffNames.None)
                    {
                        break;
                    }

                    LogInfoSkillBuffRemoved(_skillData.Buffs[i]);
                    Owner.Buff.Remove(_skillData.Buffs[i]);
                }
            }
        }

        /// <summary> 애니메이션 버프를 제거합니다. </summary>
        private void RemoveAnimationBuffs()
        {
            if (_skillData.IsValid())
            {
                BuffAsset buffAsset;
                for (int i = 0; i < _skillData.Buffs.Length; i++)
                {
                    if (_skillData.Buffs[i] != BuffNames.None)
                    {
                        buffAsset = ScriptableDataManager.Instance.FindBuff(_skillData.Buffs[i]);
                        if (buffAsset.IsValid())
                        {
                            if (buffAsset.Data.RemoveOnStopAnimation)
                            {
                                Owner.Buff.Remove(_skillData.Buffs[i]);
                            }
                        }
                    }
                }
            }
        }

        //

        private void LogInfoActivatingStageBuffLoaded(BuffNames buffName)
        {
            if (Log.LevelInfo)
            {
                LogInfo("활성화 단계에서 버프 {0}를 찾습니다.", buffName.ToLogString());
            }
        }

        private void LogInfoSkillBuffRemoved(BuffNames buffName)
        {
            if (Log.LevelInfo)
            {
                LogInfo("스킬 버프를 제거합니다. {0}", buffName.ToLogString());
            }
        }
    }
}