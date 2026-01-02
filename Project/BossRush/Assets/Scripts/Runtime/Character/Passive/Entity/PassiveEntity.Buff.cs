using Lean.Pool;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        // Buff
        private void LoadBuffAsset()
        {
            if (_buffAssetDatas.IsValid())
            {
                return;
            }

            if (EffectSettings == null || !EffectSettings.Buffs.IsValidArray())
            {
                return;
            }

            _buffAssetDatas = new BuffAssetData[EffectSettings.Buffs.Length];
            for (int i = 0; i < EffectSettings.Buffs.Length; i++)
            {
                BuffNames buffName = EffectSettings.Buffs[i];
                if (buffName == BuffNames.None) continue;

                _buffAssetDatas[i] = ScriptableDataManager.Instance.FindBuffClone(buffName);
                if (!_buffAssetDatas[i].IsValid())
                {
                    LogFailedLoadBuff();
                }
            }
        }

        private void ResetBuffAssets()
        {
            _buffAssetDatas = null;
        }

        //

        private void AddBuff(PassiveEffectSettings assetData, Character targetCharacter, Vector3 damagePosition)
        {
            if (!_buffAssetDatas.IsValid())
            {
                LogFailedLoadBuff();
                return;
            }

            if (targetCharacter == null)
            {
                LogFailedAddBuff(assetData.Buffs);
                return;
            }

            if (!assetData.RendomApplyBuffs)
            {
                for (int i = 0; i < _buffAssetDatas.Length; i++)
                {
                    BuffAssetData buffAssetData = _buffAssetDatas[i];
                    LogAddBuff(buffAssetData.Name, targetCharacter);

                    targetCharacter.Buff.Add(buffAssetData, Level, Owner, damagePosition);
                }
            }
            else
            {
                int randomIndex = TSRandomEx.Range(_buffAssetDatas.Length);
                BuffAssetData buffAssetData = _buffAssetDatas[randomIndex];
                LogAddBuff(buffAssetData.Name, targetCharacter);

                targetCharacter.Buff.Add(buffAssetData, Level, Owner, damagePosition);
            }

            AppliedEffects |= AppliedEffects.BuffAdd; // 효과 추가 완료
        }

        private void SetBuffLevel(int level)
        {
            if (EffectSettings.IsValid())
            {
                if (_buffAssetDatas.IsValid())
                {
                    for (int i = 0; i < _buffAssetDatas.Length; i++)
                    {
                        Owner.Buff.SetLevel(_buffAssetDatas[i].Name, level);
                    }
                }
            }
        }

        //

        private void RemoveApplyBuff(PassiveEffectSettings assetData)
        {
            if (assetData.Buffs.IsValidArray())
            {
                for (int i = 0; i < assetData.Buffs.Length; i++)
                {
                    if (Owner != null)
                    {
                        Owner.Buff.Remove(assetData.Buffs[i]);
                    }
                }
            }
        }

        private void RemoveBuff(PassiveEffectSettings assetData, Character targetCharacter)
        {
            if (!assetData.IsValid())
            {
                return;
            }

            if (targetCharacter == null)
            {
                return;
            }

            if (assetData.ReleaseBuffs.IsValidArray())
            {
                HandleBuffRemoval(assetData.ReleaseBuffs, assetData.ReleaseBuffApplication, targetCharacter);
            }

            if (assetData.ReleaseStateEffects.IsValidArray())
            {
                HandleStateEffectRemoval(assetData.ReleaseStateEffects, assetData.ReleaseBuffApplication, targetCharacter);
            }
        }

        //

        private void HandleBuffRemoval(BuffNames[] buffs, BuffReleaseApplications application, Character targetCharacter)
        {
            for (int i = 0; i < buffs.Length; i++)
            {
                switch (application)
                {
                    case BuffReleaseApplications.RemoveBuff:
                        targetCharacter.Buff.Remove(buffs[i]);
                        AppliedEffects |= AppliedEffects.BuffRemove; // 효과 제거 완료
                        break;

                    case BuffReleaseApplications.RemoveOneStack:
                        targetCharacter.Buff.RemoveStack(buffs[i]);
                        AppliedEffects |= AppliedEffects.BuffRemove; // 효과 스택 제거 완료
                        break;

                    case BuffReleaseApplications.RemoveAllStackSequentially:
                        targetCharacter.Buff.RemoveStackSequentially(buffs[i]);
                        AppliedEffects |= AppliedEffects.BuffRemove; // 효과 스택 순차 제거 완료
                        break;
                }
            }
        }

        private void HandleStateEffectRemoval(StateEffects[] effects, BuffReleaseApplications application, Character targetCharacter)
        {
            for (int i = 0; i < effects.Length; i++)
            {
                switch (application)
                {
                    case BuffReleaseApplications.RemoveBuff:
                        targetCharacter.Buff.Remove(effects[i]);
                        AppliedEffects |= AppliedEffects.BuffRemove; // 상태 효과 제거 완료
                        break;

                    case BuffReleaseApplications.RemoveOneStack:
                        targetCharacter.Buff.RemoveStack(effects[i]);
                        AppliedEffects |= AppliedEffects.BuffRemove; // 상태 효과 스택 제거 완료
                        break;

                    case BuffReleaseApplications.RemoveAllStackSequentially:
                        targetCharacter.Buff.RemoveStackSequentially(effects[i]);
                        AppliedEffects |= AppliedEffects.BuffRemove; // 상태 효과 스택 순차 제거 완료
                        break;
                }
            }
        }

        public void ApplyDamageOverTimeAtOnce(PassiveEffectSettings assetData, Character targetCharacter)
        {
            if (!assetData.IsValid())
            {
                return;
            }

            if (targetCharacter == null)
            {
                return;
            }

            if (assetData.ApplyDamageOverTimeAtOnce != StateEffects.None)
            {
                targetCharacter.Buff.ApplyAtOnce(assetData.ApplyDamageOverTimeAtOnce);
                AppliedEffects |= AppliedEffects.BuffAdd; // 데미지 오버타임 적용 완료
            }
        }
    }
}