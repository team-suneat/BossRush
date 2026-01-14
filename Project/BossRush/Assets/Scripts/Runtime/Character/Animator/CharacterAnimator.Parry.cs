using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        public bool PlayParryAnimation()
        {
            if (_animator.UpdateAnimatorTrigger(ANIMATOR_PARRY_PARAMETER_ID, AnimatorParameters))
            {
                AnimatorLog.LogInfo("패리 애니메이션을 재생합니다.");
                return true;
            }

            AnimatorLog.LogWarning("패리 애니메이션 재생에 실패했습니다.");
            return false;
        }

        protected virtual void OnAnimatorParryStateEnter()
        {
            SetParrying(true);
            AnimatorLog.LogInfo("패리 상태의 애니메이션에 진입했습니다.");
        }

        protected virtual void OnAnimatorParryStateExit()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_PARRY_SUCCESS_PARAMETER_ID, false, AnimatorParameters);
            SetParrying(false);
            AnimatorLog.LogInfo("패리 상태의 애니메이션에서 퇴장했습니다.");
        }

        public void SetParrySuccess(bool isSuccess)
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_PARRY_SUCCESS_PARAMETER_ID, isSuccess, AnimatorParameters);
            AnimatorLog.LogInfo("패링 성공 여부를 설정합니다: {0}", isSuccess);
        }

        public void ReleaseParryState()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_PARRY_SUCCESS_PARAMETER_ID, false, AnimatorParameters);
            SetParrying(false);
            AnimatorLog.LogInfo("패링 상태를 해제합니다.");
        }

    }
}

