using UnityEngine;

namespace TeamSuneat
{
    public partial class PlayerCharacterAnimator
    {
        // 캐스트 스킬 재생 중인 애니메이션 이름
        private string _currentCastAnimationName;

        public override bool PlayCastAnimation(SkillName skillName = SkillName.None)
        {
            if (skillName == SkillName.None)
            {
                return false;
            }

            string animationName = skillName.ToString();
            _currentCastAnimationName = animationName;
            _animator.Play(animationName, 0);
            AnimatorLog.LogInfo("시전 애니메이션을 재생합니다. 스킬: {0}", skillName.ToLogString());
            return true;
        }

        private bool IsCurrentCastState(AnimatorStateInfo stateInfo)
        {
            if (string.IsNullOrEmpty(_currentCastAnimationName))
            {
                return false;
            }

            return CheckStateName(stateInfo, _currentCastAnimationName);
        }

        public override void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnAnimatorStateEnter(animator, stateInfo, layerIndex);

            if (IsCurrentCastState(stateInfo))
            {
                OnAnimatorCastStateEnter();
            }
        }

        public override void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnAnimatorStateExit(animator, stateInfo, layerIndex);

            if (IsCurrentCastState(stateInfo))
            {
                OnAnimatorCastStateExit();
            }
        }

        protected virtual void OnAnimatorCastStateEnter()
        {
            if (!_ignoreFlipOnAttacking)
            {
                LockFlip();
            }

            LockMovement();
            SetCasting(true);
            AnimatorLog.LogInfo("시전 상태의 애니메이션에 진입했습니다.");
        }

        protected virtual void OnAnimatorCastStateExit()
        {
            if (!_ignoreFlipOnAttacking)
            {
                UnlockFlip();
            }

            UnlockMovement();
            SetCasting(false);
            AnimatorLog.LogInfo("시전 상태의 애니메이션에서 퇴장했습니다.");
        }
    }
}
