using Lean.Pool;
using TeamSuneat.Data;


namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        private float GetRestTime(PassiveEffectSettings assetData)
        {
            float restTime = TSStatEx.GetValueByLevel(assetData.RestTime, assetData.RestTimeByLevel, Level);
            if (assetData.IsBuffDurationAddedToRestTime && assetData.Buffs.IsValid())
            {
                restTime += CalculateBuffDuration(assetData.Buffs);
            }

            return restTime;
        }

        private float CalculateBuffDuration(BuffNames[] buffs)
        {
            float totalBuffDuration = 0f;

            for (int i = 0; i < buffs.Length; i++)
            {
                BuffNames buffName = buffs[i];
                if (buffName == BuffNames.None) continue;

                float buffDuration = GetBuffDuration(buffName);
                totalBuffDuration += buffDuration;

                LogProgress("패시브의 유휴시간을 버프({0})의 지속시간({1})만큼 증가시킵니다.", buffName.ToLogString(), buffDuration);
            }

            return totalBuffDuration;
        }

        private float GetBuffDuration(BuffNames buffName)
        {
            if (Owner.Buff.ContainsKey(buffName))
            {
                return Owner.Buff.Find(buffName).Duration;
            }

            BuffAssetData buffAssetData = ScriptableDataManager.Instance.FindBuffClone(buffName);
            if (buffAssetData.IsValid())
            {
                return TSStatEx.GetValueByLevel(buffAssetData.Duration, buffAssetData.DurationByLevel, Level);
            }

            return 0f;
        }

        /// <summary>
        /// 패시브의 RestTime을 시작합니다.
        /// PassiveSystem의 RestTimeController를 통해 관리됩니다.
        /// </summary>
        /// <param name="assetData">패시브 효과 설정</param>
        public void StartRestTimer(PassiveEffectSettings assetData)
        {
            float restTime = GetRestTime(assetData);
            if (restTime <= 0f)
            {
                return;
            }

            LogProgress("패시브를 {0}초동안 유휴상태로 설정합니다.", restTime);

            // PassiveSystem의 RestTimeController를 통해 RestTime 관리
            Owner.Passive.StartPassiveRestTime(Name, restTime);
        }

        /// <summary>
        /// 패시브의 RestTime을 강제로 종료합니다.
        /// PassiveSystem의 RestTimeController를 통해 관리됩니다.
        /// </summary>
        public void StopRestTimer()
        {
            LogProgress("패시브를 유휴상태에서 해제합니다.");

            // PassiveSystem의 RestTimeController를 통해 RestTime 관리
            Owner.Passive.StopPassiveRestTime(Name);
        }
    }
}