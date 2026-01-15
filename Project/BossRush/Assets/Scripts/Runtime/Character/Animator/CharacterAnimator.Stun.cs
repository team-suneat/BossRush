using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        public bool PlayStunAnimation()
        {
            if (_animator.UpdateAnimatorTrigger(ANIMATOR_STUN_PARAMETER_ID, AnimatorParameters))
            {
                AnimatorLog.LogInfo("스턴 애니메이션을 재생합니다.");
                return true;
            }

            AnimatorLog.LogWarning("스턴 애니메이션 재생에 실패했습니다.");
            return false;
        }

        protected virtual void OnAnimatorStunStateEnter()
        {
            SetStunned(true);
            AnimatorLog.LogInfo("스턴 상태의 애니메이션에 진입했습니다.");
        }

        protected virtual void OnAnimatorStunStateExit()
        {
            SetStunned(false);
            AnimatorLog.LogInfo("스턴 상태의 애니메이션에서 퇴장했습니다.");
        }
    }
}
