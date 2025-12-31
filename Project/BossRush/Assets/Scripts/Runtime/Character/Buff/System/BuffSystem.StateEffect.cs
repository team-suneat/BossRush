using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class BuffSystem : XBehaviour
    {
        /// <summary>
        /// 상태이상 버프를 등록합니다.
        /// </summary>
        private void RegisterStateBuff(StateEffects stateEffect, BuffEntity entity)
        {
            if (stateEffect != StateEffects.None)
            {
                _stateEntities.Add(stateEffect, entity);
                _activeStateEffects.Add(stateEffect);

                LogInfo("상태이상 버프를 등록합니다. BuffType: {0}, BuffName: {1}", stateEffect.ToLogString(), entity.Name.ToLogString());

                List<BuffEntity> stateEntities;
                if (_stateEntities.TryGetValue(stateEffect, out stateEntities))
                {
                    if (stateEntities.Count == 1)
                    {
                        AddBuffOfStateEffect(stateEffect);
                        StartXCoroutine(SendAddedStateEffectGlobalEvent(stateEffect));
                    }
                }
            }
        }

        /// <summary>
        /// 상태이상 버프를 등록 해제합니다.
        /// </summary>
        private void UnregisterStateBuff(StateEffects stateEffect, BuffEntity entity)
        {
            if (stateEffect != StateEffects.None)
            {
                if (_stateEntities.Contains(stateEffect, entity))
                {
                    _stateEntities.Remove(stateEffect, entity);
                    _activeStateEffects.Remove(stateEffect);

                    LogInfo("상태이상 버프를 등록 해제합니다. BuffType: {0}, BuffName: {1}", stateEffect.ToLogString(), entity.Name.ToLogString());

                    if (!_stateEntities.ContainsKey(stateEffect))
                    {
                        RemoveBuffOfStateEffect(stateEffect);

                        StartXCoroutine(SendRemovedStateEffectGlobalEvent(stateEffect));
                    }
                }
            }
        }

        /// <summary>
        /// 등록된 모든 상태이상 버프를 등록해제합니다.
        /// </summary>
        private void ClearStateEffectBuff()
        {
            if (_stateEntities.IsValid())
            {
                LogInfo("상태이상 버프를 초기화합니다. Count: {0}", _stateEntities.Count);

                if (_stateEntities.Count > 0)
                {
                    foreach (var item in _stateEntities.Storage)
                    {
                        if (Owner.IsPlayer)
                        {
                            GlobalEvent<StateEffects>.Send(GlobalEventType.PLAYER_CHARACTER_REMOVE_STATE_EFFECT, item.Key);
                        }
                        else
                        {
                            GlobalEvent<Character, StateEffects>.Send(GlobalEventType.MONSTER_CHARACTER_REMOVE_STATE_EFFECT, Owner, item.Key);
                        }
                    }
                }

                _stateEntities.Clear();
                _activeStateEffects.Clear();
            }
        }

        private void AddBuffOfStateEffect(StateEffects stateEffect)
        {
            BuffStateEffectAsset asset = ScriptableDataManager.Instance.FindBuffStateEffect(stateEffect);
            if (asset != null)
            {
                if (asset.Data.BuffName != BuffNames.None)
                {
                    Add(ScriptableDataManager.Instance.FindBuffClone(asset.Data.BuffName), 1, Owner);
                }
            }
        }

        private void RemoveBuffOfStateEffect(StateEffects stateEffect)
        {
            BuffStateEffectAsset asset = ScriptableDataManager.Instance.FindBuffStateEffect(stateEffect);
            if (asset != null)
            {
                if (asset.Data.BuffName != BuffNames.None)
                {
                    Remove(asset.Data.BuffName);
                }
            }
        }
    }
}