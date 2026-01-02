using System;
using System.Collections.Generic;
using Lean.Pool;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        // 보상 (Reward)

        private void EarnReward(PassiveEffectSettings assetData)
        {
            if (assetData.RewardOperator == ConditionalOperators.And)
            {
                EarnRewardPotions(assetData);
                EarnRewardRelics(assetData);
                EarnRewardCurrencies(assetData);
                EarnRewardExperience(assetData);
                AppliedEffects |= AppliedEffects.Reward; // 보상 획득 완료
            }
            else if (assetData.RewardOperator == ConditionalOperators.Or)
            {
                List<Action> rewardOptions = new();

                if (assetData.PotionRewards.IsValid())
                {
                    rewardOptions.Add(() => EarnRewardPotions(assetData));
                }

                if (assetData.RelicRewards.IsValid())
                {
                    rewardOptions.Add(() => EarnRewardRelics(assetData));
                }

                if (assetData.CurrencyRewards.IsValid())
                {
                    rewardOptions.Add(() => EarnRewardCurrencies(assetData));
                }

                if (assetData.IsValid() && (assetData.RewardLevel > 0 || assetData.RewardExperience > 0))
                {
                    rewardOptions.Add(() => EarnRewardExperience(assetData));
                }

                if (rewardOptions.Count > 0)
                {
                    int index = TSRandomEx.Range(rewardOptions.Count);
                    rewardOptions[index].Invoke();
                    AppliedEffects |= AppliedEffects.Reward; // 보상 획득 완료
                }
            }
        }

        private void EarnRewardPotions(PassiveEffectSettings assetData)
        {
            if (!assetData.IsValid()) { return; }
            if (!assetData.PotionRewards.IsValid()) { return; }
            if (this == null || gameObject == null) { return; }

            Transform cachedTransform = transform;
            for (int i = 0; i < assetData.PotionRewards.Length; i++)
            {
                PassiveRewardPotion reward = assetData.PotionRewards[i];
                if (reward == null)
                {
                    continue;
                }

                reward.Apply(cachedTransform);
            }
        }

        private void EarnRewardRelics(PassiveEffectSettings assetData)
        {
            if (!assetData.IsValid()) { return; }
            if (!assetData.RelicRewards.IsValid())
            {
                return;
            }

            for (int i = 0; i < assetData.RelicRewards.Length; i++)
            {
                assetData.RelicRewards[i].Apply();
            }
        }

        private void EarnRewardCurrencies(PassiveEffectSettings assetData)
        {
            if (!assetData.IsValid()) { return; }
            if (!assetData.CurrencyRewards.IsValid()) { return; }
            if (this == null || gameObject == null) { return; }

            Transform cachedTransform = transform;
            for (int i = 0; i < assetData.CurrencyRewards.Length; i++)
            {
                PassiveRewardCurrency reward = assetData.CurrencyRewards[i];
                if (reward == null) { continue; }
                reward.Apply(cachedTransform);
            }
        }

        private void EarnRewardExperience(PassiveEffectSettings assetData)
        {
            VCharacter characterInfo = GameApp.GetSelectedCharacter();
            if (characterInfo == null) { return; }

            if (!assetData.IsValid()) { return; }
            if (assetData.RewardLevel <= 0 && assetData.RewardExperience <= 0)
            {
                return;
            }

            if (assetData.RewardLevel > 0)
            {
                characterInfo.LevelUp(assetData.RewardLevel);
            }

            if (assetData.RewardExperience > 0)
            {
                characterInfo.AddExperience(assetData.RewardExperience);
            }
        }
    }
}