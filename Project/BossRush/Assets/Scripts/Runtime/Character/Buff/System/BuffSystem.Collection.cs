using System.Collections.Generic;
using System.Linq;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class BuffSystem : XBehaviour
    {
        #region 검색 (Find)

        public BuffEntity Find(BuffNames buffName)
        {
            if (_entities.IsValid())
            {
                if (_entities.ContainsKey(buffName))
                {
                    return _entities[buffName];
                }
            }

            return null;
        }

        public bool ContainsKey(BuffNames buffName)
        {
            if (_entities.IsValid())
            {
                if (_entities.ContainsKey(buffName))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsStateEffect(StateEffects stateEffect)
        {
            if (stateEffect == StateEffects.DamageOverTime)
            {
                StateEffects[] damageOverTimes = { StateEffects.Burning, StateEffects.Poisoning, StateEffects.Jolted, StateEffects.Bleeding };

                return ContainsStateEffectInArray(damageOverTimes);
            }
            else if (stateEffect == StateEffects.Incapacitated)
            {
                StateEffects[] incapacitated = { StateEffects.Stun, StateEffects.Freeze, StateEffects.ElectricShock, StateEffects.Paralysis };

                return ContainsStateEffectInArray(incapacitated);
            }
            else
            {
                return _activeStateEffects.Contains(stateEffect);
            }
        }

        private bool ContainsStateEffectInArray(StateEffects[] stateEffectsArray)
        {
            foreach (StateEffects stateEffect in stateEffectsArray)
            {
                if (_activeStateEffects.Contains(stateEffect))
                {
                    return true;
                }
            }
            return false;
        }

        public int GetStateEffectCount()
        {
            return _stateEntities.KeysCount;
        }

        #endregion 검색 (Find)

        #region 추가 (Add)

        private bool DetermineAdd(BuffAssetData assetData, Character caster)
        {
            if (!assetData.IsValid())
            {
                LogWarning("버프 에셋이 올바르지 않습니다. 버프({0})를 추가할 수 없습니다.", assetData.Name.ToLogString());
                return false;
            }

            if (ContainsRestingBuff(assetData.Name))
            {
                LogWarning("유휴중인 버프({0})를 추가할 수 없습니다.", assetData.Name.ToLogString());

                return false;
            }

            if (ContainsIncompatibleBuff(assetData.Name))
            {
                LogWarning("호환 불가한 버프({0})를 추가할 수 없습니다.", assetData.Name.ToLogString());

                return false;
            }

            if (ContainsIncompatibleStateEffect(assetData.StateEffect))
            {
                LogWarning("호환 불가한 상태이상({1})을 가진 버프({0})를 추가할 수 없습니다.", assetData.Name.ToLogString(), assetData.StateEffect.ToLogString());

                return false;
            }

            if (!caster.IsAlive)
            {
                LogWarning("시전자가 사망했다면 버프({0})를 추가할 수 없습니다. 시전자: {1}", assetData.Name.ToLogString(), caster.GetHierarchyPath());

                return false;
            }

            if (!Owner.IsAlive)
            {
                if (!assetData.IgnoreCheckOwnerAlive)
                {
                    LogWarning("오너 캐릭터가 사망했다면 버프({0})를 추가할 수 없습니다.", assetData.Name.ToLogString());

                    return false;
                }
            }

            if (assetData.StateEffect.IsCrowdControl())
            {
                if (Owner.IgnoreCrowdControl)
                {
                    if (Log.LevelProgress)
                    {
                        LogProgress($"행동불가 상태이상 면역상태입니다. " +
                            $"버프({assetData.Name.ToLogString()})를 추가할 수 없습니다. " +
                            $"시전자: {caster.Name.ToLogString()}, " +
                            $"버프 타입: {assetData.Type.ToLogString()}");
                    }

                    return false;
                }

                if (Owner.IsPlayer)
                {
                    if (GameSetting.Instance.Cheat.NotCrowdControl)
                    {
                        LogWarning($"치트에 의한 행동불가 상태이상 면역상태입니다. " +
                            $"버프({assetData.Name.ToLogString()})를 추가할 수 없습니다. " +
                            $"시전자: {caster.Name.ToLogString()}, " +
                            $"버프 타입: {assetData.Type.ToLogString()}");

                        return false;
                    }
                }

                if (Owner.IsBoss)
                {
                    LogWarning($"모든 보스는 행동불가 상태이상 면역상태입니다. " +
                            $"버프({assetData.Name.ToLogString()})를 추가할 수 없습니다. " +
                            $"시전자: {caster.Name.ToLogString()}, " +
                            $"버프 타입: {assetData.Type.ToLogString()}");

                    return false;
                }
            }

            if (ContainsKey(assetData.Name))
            {
                if (assetData.Application == BuffApplications.Ignore)
                {
                    LogWarning("버프를 추가할 수 없습니다. 해당 버프({0})는 미리 등록된 버프가 있다면 무시합니다. (Ignore)",
                        assetData.Name.ToLogString());

                    return false;
                }
            }

            return true;
        }

        public void AddDuration(StateEffects stateEffect, float duration, float maxDuration)
        {
            if (_stateEntities.TryGetValue(stateEffect, out List<BuffEntity> stateBuffEntities))
            {
                for (int i = 0; i < stateBuffEntities.Count; i++)
                {
                    BuffEntity entity = stateBuffEntities[i];
                    entity.SetAdditionalDuration(duration, maxDuration);
                }
            }
        }

        public void Add(BuffAssetData assetData, int buffLevel, Character caster)
        {
            Add(assetData, buffLevel, caster, Vector3.zero);
        }

        public void Add(BuffAssetData assetData, int buffLevel, Character caster, Vector3 spawnPosition)
        {
            if (!DetermineAdd(assetData, caster)) { return; }
            if (assetData.InitBuffEntityPositionZero) { spawnPosition = Vector3.zero; }

            BuffNames buffName = assetData.Name;
            switch (assetData.Application)
            {
                case BuffApplications.Ignore:
                    {
                        HandleApplicationIgnore(assetData, buffLevel, caster, spawnPosition);
                    }
                    break;

                case BuffApplications.Overlap:
                    {
                        HandleApplicationOverlap(assetData, buffLevel, caster, spawnPosition);
                    }
                    break;

                default:
                    {
                        Log.Error("버프({0})의 적용 방식({1})이 설정되어있지 않습니다.", buffName.ToLogString(), assetData.Application);
                    }
                    return;
            }
        }

        private void HandleApplicationIgnore(BuffAssetData assetData, int buffLevel, Character caster, Vector3 spawnPosition)
        {
            BuffNames buffName = assetData.Name;
            if (ContainsKey(buffName))
            {
                return;
            }

            BuffEntity buffEntity = SpawnBuffEntity(assetData, buffLevel, caster);
            if (buffEntity != null)
            {
                LogInfo("{0}, 버프를 추가합니다. ", buffName.ToLogString());

                _entities.Add(buffName, buffEntity);
                ActivateEntity(buffEntity, spawnPosition);
                OnAdd(buffEntity);
            }

            ExecuteMaxStack(assetData, buffEntity, caster);
        }

        private void HandleApplicationOverlap(BuffAssetData assetData, int buffLevel, Character caster, Vector3 spawnPosition)
        {
            BuffNames buffName = assetData.Name;
            BuffEntity buffEntity;

            if (ContainsKey(buffName))
            {
                buffEntity = _entities[buffName];
                OverlapEntity(buffEntity, buffLevel);
            }
            else
            {
                buffEntity = SpawnBuffEntity(assetData, buffLevel, caster);
                if (buffEntity != null)
                {
                    LogInfo("{0}, 버프를 추가합니다. ", buffName.ToLogString());
                    _entities[buffName] = buffEntity;
                    ActivateEntity(buffEntity, spawnPosition);
                    OnAdd(buffEntity);
                }
            }

            ExecuteMaxStack(assetData, buffEntity, caster);
        }

        private void HandleApplicationRefresh(BuffAssetData assetData, int buffLevel, Character caster, Vector3 spawnPosition)
        {
            BuffNames buffName = assetData.Name;
            BuffEntity buffEntity;
            if (ContainsKey(buffName))
            {
                buffEntity = _entities[buffName];

                ActivateEntity(buffEntity, spawnPosition);
            }
            else
            {
                buffEntity = SpawnBuffEntity(assetData, buffLevel, caster);
                if (buffEntity != null)
                {
                    LogInfo("{0}, 버프를 추가합니다. ", buffName.ToLogString());

                    _entities[buffName] = buffEntity;

                    ActivateEntity(buffEntity, spawnPosition);

                    OnAdd(buffEntity);
                }
            }

            ExecuteMaxStack(assetData, buffEntity, caster);
        }

        /// <summary>
        /// 새롭게 추가된 버프가 있다면 해당 과정을 처리합니다.
        /// </summary>
        private void OnAdd(BuffEntity buffEntity)
        {
            // 신규 버프가 생성될 때에만 버프의 VFXObject를 생성합니다.
            buffEntity.SpawnBuffStateVFX();
            buffEntity.SpawnFloatyTextByType();

            RegisterStateBuff(buffEntity.StateEffect, buffEntity);
            RegisterIncompatibleBuff(buffEntity.AssetData.Incompatible, buffEntity.Name);
            RegisterIncompatibleBuffType(buffEntity.AssetData.IncompatibleStateEffect, buffEntity.Name);

            CallRefreshEvents(buffEntity.Name, buffEntity.Level);

            _ = StartXCoroutine(SendAddedBuffGlobalEvent(buffEntity.Name, buffEntity.Level));
        }

        #endregion 추가 (Add)

        #region 삭제 (Remove)

        public void Remove(BuffNames buffName)
        {
            if (ContainsKey(buffName))
            {
                Remove(_entities[buffName]);
            }
        }

        public void Remove(StateEffects stateEffect)
        {
            if (!_stateEntities.TryGetValue(stateEffect, out List<BuffEntity> stateEntities))
            {
                return;
            }

            for (int i = stateEntities.Count - 1; i >= 0; i--)
            {
                Remove(stateEntities[i]);
            }
        }

        public void Remove(BuffEntity buffEntity)
        {
            if (buffEntity == null)
            {
                LogWarning("버프 엔티티가 null입니다.");
                return;
            }

            BuffNames buffName = buffEntity.Name;

            UnregisterStateBuff(buffEntity.StateEffect, buffEntity);
            UnregisterIncompatibleBuff(buffEntity.AssetData.Incompatible, buffName);
            UnregisterIncompatibleBuffType(buffEntity.AssetData.IncompatibleStateEffect, buffName);

            buffEntity.Deactivate();
            buffEntity.Despawn();

            _ = _entities.Remove(buffName);

            LogInfo("{0}, 버프를 삭제했습니다. 버프 타입: {1}", buffName.ToLogString(), buffEntity.Type.ToLogString());

            CallRefreshEvents(buffName, 0);
            CallRemovedBuffGlobalEvent(buffEntity);
        }

        /// <summary> 버프의 스택을 하나 삭제합니다. 스택이 0에 도달하면 버프 독립체를 삭제합니다. </summary>
        public void RemoveStack(BuffNames buffName)
        {
            if (ContainsKey(buffName))
            {
                if (_entities[buffName].RemoveStackCount())
                {
                    if (_entities[buffName].Stack > 0)
                    {
                        _entities[buffName].RefreshStats();
                    }
                    else
                    {
                        Remove(buffName);
                    }
                }
                else
                {
                    Remove(buffName);
                }
            }
        }

        /// <summary> 버프의 스택을 하나 삭제합니다. 스택이 0에 도달하면 버프 독립체를 삭제합니다. </summary>
        public void RemoveStack(StateEffects stateEffect)
        {
            if (_stateEntities.TryGetValue(stateEffect, out List<BuffEntity> stateBuffEntities))
            {
                for (int i = stateBuffEntities.Count - 1; i >= 0; i--)
                {
                    BuffEntity buffEntity = stateBuffEntities[i];
                    if (buffEntity.RemoveStackCount())
                    {
                        if (buffEntity.Stack > 0)
                        {
                            buffEntity.RefreshStats();
                        }
                        else
                        {
                            Remove(buffEntity);
                        }
                    }
                }
            }
        }

        /// <summary> 버프의 스택을 순차적으로 삭제합니다. 스택이 0에 도달하면 버프 독립체를 삭제합니다. </summary>
        public void RemoveStackSequentially(BuffNames buffName)
        {
            if (ContainsKey(buffName))
            {
                _entities[buffName].StartRemoveStackSequentially();
            }
        }

        /// <summary> 버프의 스택을 순차적으로 삭제합니다. 스택이 0에 도달하면 버프 독립체를 삭제합니다. </summary>
        public void RemoveStackSequentially(StateEffects stateEffect)
        {
            if (_stateEntities.TryGetValue(stateEffect, out List<BuffEntity> stateBuffEntities))
            {
                for (int i = stateBuffEntities.Count - 1; i >= 0; i--)
                {
                    BuffEntity buffEntity = stateBuffEntities[i];
                    buffEntity.StartRemoveStackSequentially();
                }
            }
        }

        public void Clear()
        {
            LogInfo("소지한 모든 버프를 모두 초기화합니다.");

            if (_entities.IsValid())
            {
                BuffNames[] buffNames = _entities.Keys.ToArray();
                BuffEntity[] entities = _entities.Values.ToArray();

                for (int i = 0; i < entities.Length; i++)
                {
                    BuffEntity entity = entities[i];
                    if (entity != null)
                    {
                        CallRemovedBuffGlobalEvent(entity);

                        entity.Deactivate();
                        entity.Despawn();
                    }
                    else
                    {
                        if (buffNames.Length > i)
                        {
                            Log.Error("등록된 버프 독립체가 유효하지 않습니다. {0}", buffNames[i].ToLogString());
                        }
                        else
                        {
                            Log.Error("등록된 버프 독립체가 유효하지 않습니다.");
                        }
                    }
                }

                _entities.Clear();
            }

            ClearStateEffectBuff();
            ClearIncompatibleBuffs();
            ClearIncompatibleStateEffects();
        }

        private void CallRemovedBuffGlobalEvent(BuffEntity buffEntity)
        {
            if (!buffEntity.CallRemoveGlobalEvent)
            {
                buffEntity.CallRemoveGlobalEvent = true;

                _ = StartXCoroutine(SendRemovedBuffGlobalEvent(buffEntity.Name));
            }
        }

        #endregion 삭제 (Remove)

        #region 갱신 (Refresh)

        public void RegisterRefreshEvent(UnityAction<BuffNames, int> action)
        {
            _onRefreshEvent.AddListener(action);

            LogProgress($"버프 갱신 이벤트를 등록합니다. {action.Method.Name}, {action.Target}");
        }

        public void UnregisterRefreshEvent(UnityAction<BuffNames, int> action)
        {
            _onRefreshEvent.RemoveListener(action);

            LogProgress($"버프 갱신 이벤트를 등록해제합니다. {action.Method.Name}, {action.Target}");
        }

        private void CallRefreshEvents(BuffNames buffName, int buffLevel)
        {
            _onRefreshEvent.Invoke(buffName, buffLevel);
        }

        #endregion 갱신 (Refresh)
    }
}