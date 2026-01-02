using System.Collections;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        private Coroutine _movementLockCoroutine;

        protected virtual bool DetermineMovementLockWhileAttack(string animationName)
        {
            // 기본적으로 공격 중에는 입력에 따른 캐릭터의 이동을 잠금합니다.
            return true;
        }

        private void LockMovement()
        {
            AnimatorLog.LogInfo("입력에 따른 캐릭터의 이동을 잠금합니다.");

            _abilityMovement?.LockMovement(this);

            if (_abilityFly != null)
            {
                _abilityFly.FlyForbidden = true;
            }
        }

        private void UnlockMovement()
        {
            AnimatorLog.LogInfo("입력에 따른 캐릭터의 캐릭터 이동을 잠금해제합니다.");

            _abilityMovement?.UnlockMovement(this);

            if (_abilityFly != null)
            {
                _abilityFly.FlyForbidden = false;
            }

            StopXCoroutine(ref _movementLockCoroutine);
        }

        private void StartMovementLock(float animationLength)
        {
            SkillAnimationAsset playingAnimationAsset = GetPlayingAnimation();
            if (playingAnimationAsset == null) { return; }
            if (playingAnimationAsset.Movable) { return; }

            LockMovement();

            if (playingAnimationAsset.ReleaseMovableTimeRate.InRange(0, 1))
            {
                StopXCoroutine(ref _movementLockCoroutine);

                float releaseDelayTime = animationLength * playingAnimationAsset.ReleaseMovableTimeRate;
                _movementLockCoroutine = StartXCoroutine(ProcessMovementLock(releaseDelayTime));
            }
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