using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        private bool IsJumpState(AnimatorStateInfo stateInfo)
        {
            return CheckStateNames(stateInfo, "JumpReady", "JumpApex", "JumpGround", "Landing");
        }

        private bool IsJumpReadyState(AnimatorStateInfo stateInfo)
        {
            return CheckStateName(stateInfo, "JumpReady");
        }

        private bool IsLandingState(AnimatorStateInfo stateInfo)
        {
            return CheckStateName(stateInfo, "Landing");
        }

        protected void OnAnimatorJumpStateEnter()
        {
            SetJumping(true);
            LockFlip();
        }

        protected void OnAnimatorJumpStateExit()
        {
            SetJumping(false);
            UnlockFlip();
        }
    }
}
