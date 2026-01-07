using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        private Coroutine _movementLockCoroutine;
        private bool _isMovementLocked = false;

        public bool IsMovementLocked => _isMovementLocked;

        protected virtual bool DetermineMovementLockWhileAttack(string animationName)
        {
            // 기본적으로 공격 중에는 입력에 따른 캐릭터의 이동을 잠금합니다.
            return true;
        }

        public void LockMovement()
        {
            _isMovementLocked = true;
            AnimatorLog.LogInfo("입력에 따른 캐릭터의 이동을 잠금합니다.");
        }

        public void UnlockMovement()
        {
            _isMovementLocked = false;
            AnimatorLog.LogInfo("입력에 따른 캐릭터의 이동을 잠금해제합니다.");

            StopXCoroutine(ref _movementLockCoroutine);
        }

        private IEnumerator ProcessMovementLock(float delayTime)
        {
            AnimatorLog.LogInfo("캐릭터 이동의 자동 잠금해제를 시작합니다. {0}초 뒤 자동 잠금해제됩니다.", delayTime);

            yield return new WaitForSeconds(delayTime);

            AnimatorLog.LogInfo("캐릭터 이동의 자동 잠금해제를 완료합니다.");

            UnlockMovement();
        }
    }
}