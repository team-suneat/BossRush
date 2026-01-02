using Sirenix.OdinInspector;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackAreaEntity : AttackEntity
    {
        [FoldoutGroup("#AttackAreaEntity-Chain")]
        public ChainLightning ChainLightning;

        private void AddTargetOfChainLightning(Transform targetPoint)
        {
            if (ChainLightning != null)
            {
                ChainLightning.AddTarget(targetPoint);
            }
        }

        private void ClearTargetsOfChainLightning()
        {
            if (ChainLightning != null)
            {
                ChainLightning.Clear();
            }
        }

        private void ActivateChainLightning()
        {
            if (ChainLightning != null)
            {
                ChainLightning.Activate();
            }
        }
    }
}