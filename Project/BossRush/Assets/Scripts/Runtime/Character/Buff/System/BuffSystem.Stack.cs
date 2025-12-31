using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class BuffSystem : XBehaviour
    {
        public int GetStack(BuffNames buffName)
        {
            if (_entities != null)
            {
                if (_entities.ContainsKey(buffName))
                {
                    return _entities[buffName].Stack;
                }
            }

            return 0;
        }

        public int GetStack(StateEffects stateEffect)
        {
            int stack = 0;

            if (stateEffect is StateEffects.Chilled or StateEffects.Jolted)
            {
                if (_stateEntities.TryGetValue(stateEffect, out List<BuffEntity> stateBuffEntities))
                {
                    for (int i = 0; i < stateBuffEntities.Count; i++)
                    {
                        stack += stateBuffEntities[i].Stack;
                    }
                }
            }

            return stack;
        }

        public int FindStack(StateEffects stateEffect)
        {
            int stack = 0;

            if (_stateEntities.TryGetValue(stateEffect, out List<BuffEntity> stateBuffEntities))
            {
                for (int i = 0; i < stateBuffEntities.Count; i++)
                {
                    stack += stateBuffEntities[i].Stack;
                }
            }

            return stack;
        }

        public void ExecuteMaxStack(BuffAssetData assetData, BuffEntity entity, Character caster)
        {
            if (assetData.Type is BuffTypes.StateEffect)
            {
                if (assetData.StateEffect != StateEffects.None)
                {
                    BuffStateEffectAsset stateEffectAsset = ScriptableDataManager.Instance.FindBuffStateEffect(assetData.StateEffect);
                    if (!stateEffectAsset.IsValid())
                    {
                        return;
                    }

                    int stack = GetStack(assetData.StateEffect);
                    int maxStack = stateEffectAsset.Data.MaxStack;

                    if (stack < maxStack)
                    {
                        return;
                    }

                    if (stateEffectAsset.Data.BuffOnMaxStack != BuffNames.None)
                    {
                        BuffAssetData assetDataOnMaxStack = ScriptableDataManager.Instance.FindBuffClone(stateEffectAsset.Data.BuffOnMaxStack);
                        if (assetDataOnMaxStack.IsValid())
                        {
                            Add(assetDataOnMaxStack, 1, caster, entity.position);
                        }
                    }

                    if (stateEffectAsset.Data.HitmarkOnMaxStack != HitmarkNames.None)
                    {
                        Owner.Attack.Activate(stateEffectAsset.Data.HitmarkOnMaxStack);
                    }
                }
            }
        }
    }
}