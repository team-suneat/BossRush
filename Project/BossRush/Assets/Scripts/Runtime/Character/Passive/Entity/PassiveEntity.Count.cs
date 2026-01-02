using Lean.Pool;
using System.Collections;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        private Coroutine _resetApplyCountCoroutine;

        public int ApplyCount { get; private set; }

        private bool IsFullApplyCount => _resetApplyCountCoroutine != null;

        /// <summary> 패시브 발동(적용) 횟수를 추가합니다. </summary>        
        /// <returns> 최대 적용 횟수 도달 여부</returns>
        public bool TryAddApplyCount(PassiveEffectSettings assetData)
        {
            if (assetData.IsValid())
            {
                if (assetData.ApplyMaxCount > 0)
                {
                    ApplyCount += 1;

                    if (assetData.ApplyMaxCount == ApplyCount)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void ResetApplyCount(PassiveEffectSettings assetData)
        {
            if (assetData.IsValid())
            {
                if (assetData.ApplyMaxCount > 0)
                {
                    ApplyCount = 0;
                }
            }
        }

        private void StartRestApplyMaxCount(float resetTime)
        {
            if (_resetApplyCountCoroutine == null)
            {
                _resetApplyCountCoroutine = StartXCoroutine(ProcessResetApplyMaxCount(resetTime));
            }
        }

        private IEnumerator ProcessResetApplyMaxCount(float resetTime)
        {
            yield return new WaitForSeconds(resetTime);

            _resetApplyCountCoroutine = null;
        }
    }
}