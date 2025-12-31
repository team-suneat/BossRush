namespace TeamSuneat
{
    public partial class BuffSystem : XBehaviour
    {
        /// <summary>
        /// 호환 불가한 버프에 해당 버프가 포함되어있는지 확인합니다.
        /// </summary>
        private bool ContainsIncompatibleBuff(BuffNames buffName)
        {
            return _incompatibleBuffs.ContainsValue(buffName);
        }

        /// <summary>
        /// 호환 불가 상태이상이 등록되었는지 확인합니다.
        /// </summary>
        private bool ContainsIncompatibleStateEffect(StateEffects stateEffect)
        {
            return _incompatibleStateEffects.ContainsValue(stateEffect);
        }

        /// <summary>
        /// 호환 불가 버프를 등록합니다.
        /// </summary>
        public void RegisterIncompatibleBuff(BuffNames incompatibleBuff, BuffNames castBuff)
        {
            if (incompatibleBuff != BuffNames.None)
            {
                _incompatibleBuffs.Add(incompatibleBuff, castBuff);

                LogInfo("호환 불가한 버프를 추가합니다. {0}, CastBuff:{1}", incompatibleBuff.ToErrorString(), castBuff.ToLogString());
            }
        }

        /// <summary>
        /// 호환 불가 버프를 등록 해제합니다.
        /// </summary>
        public void UnregisterIncompatibleBuff(BuffNames incompatibleBuff, BuffNames castBuff)
        {
            if (incompatibleBuff != BuffNames.None)
            {
                _incompatibleBuffs.Remove(incompatibleBuff, castBuff);

                LogInfo("호환 불가한 버프를 삭제합니다. {0}, CastBuff:{1}", incompatibleBuff.ToErrorString(), castBuff.ToLogString());
            }
        }

        /// <summary>
        /// 등록된 모든 호환 불가 버프를 등록 해제합니다.
        /// </summary>
        private void ClearIncompatibleBuffs()
        {
            if (_incompatibleBuffs.IsValid())
            {
                LogInfo("등록된 호환 불가 버프를 초기화합니다. Count: {0}", _incompatibleBuffs.Count);

                _incompatibleBuffs.Clear();
            }
        }

        /// <summary>
        /// 호환 불가 상태이상을 등록 해제합니다.
        /// </summary>
        public void RegisterIncompatibleBuffType(StateEffects incompatibleStateEffect, BuffNames castBuff)
        {
            if (incompatibleStateEffect != StateEffects.None)
            {
                _incompatibleStateEffects.Add(incompatibleStateEffect, castBuff);

                LogInfo("호환 불가한 상태이상을 추가합니다. {0}, CastBuff:{1}", incompatibleStateEffect.ToErrorString(), castBuff.ToLogString());
            }
        }

        /// <summary>
        /// 호환 불가 상태이상을 등록 해제합니다.
        /// </summary>
        public void UnregisterIncompatibleBuffType(StateEffects incompatibleStateEffect, BuffNames castBuff)
        {
            if (incompatibleStateEffect != StateEffects.None)
            {
                _incompatibleStateEffects.Remove(incompatibleStateEffect, castBuff);

                LogInfo("호환 불가한 상태이상을 삭제합니다. {0}, CastBuff:{1}", incompatibleStateEffect.ToErrorString(), castBuff.ToLogString());
            }
        }

        /// <summary>
        /// 등록된 모든 호환 불가 버프타입을 등록 해제합니다.
        /// </summary>
        private void ClearIncompatibleStateEffects()
        {
            if (_incompatibleStateEffects.IsValid())
            {
                LogInfo("등록된 호환 불가 상태이상을 초기화합니다. Count: {0}", _incompatibleStateEffects.Count);

                _incompatibleStateEffects.Clear();
            }
        }
    }
}