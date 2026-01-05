using System.Collections;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        private Coroutine _flipLockCoroutine;

        private void LockFlip()
        {
            _owner.LockFlip();
        }

        private void UnlockFlip()
        {
            _owner.UnlockFlip();

            StopXCoroutine(ref _flipLockCoroutine);
        }

        private void StartFlipLock(float animationLength)
        {
            LockFlip();
        }

        private IEnumerator ProcessFlipLock(float delayTime)
        {
            AnimatorLog.LogInfo("캐릭터 반전의 자동 잠금해제를 시작합니다. {0}초 뒤 자동 잠금해제됩니다.", delayTime);

            yield return new WaitForSeconds(delayTime);

            AnimatorLog.LogInfo("캐릭터 반전의 자동 잠금해제를 완료합니다.");

            UnlockFlip();
        }
    }
}