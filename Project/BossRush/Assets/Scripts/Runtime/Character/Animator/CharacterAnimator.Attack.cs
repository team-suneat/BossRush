using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        // CharacterAnimator에서 사용하는 Attack은 몬스터의 공격으로 제한합니다.

        // 공격 애니메이션 이름
        protected string _attackAnimationName;

        // 순서대로 재생되는 공격 애니메이션 여부
        // 돌진과 같이 시작-반복-완료 애니메이션이 각각 설정된 공격 애니메이션을 뜻합니다.
        private bool _isSequenceAttackAnimation;

        // 재생되고 있는 공격 애니메이션 이름
        protected string AttackingAnimationName
        {
            get
            {
                if (_isSequenceAttackAnimation)
                {
                    return _attackAnimationName + "Complete";
                }

                return _attackAnimationName;
            }
        }

        protected UnityEvent<string> RefreshAttackCooldown { get; set; }

        private bool IsAttackState(AnimatorStateInfo stateInfo, bool isEnter)
        {
            if (!isEnter && _isSequenceAttackAnimation)
            {
                return stateInfo.IsName(_attackAnimationName + "Complete");
            }

            return stateInfo.IsName(_attackAnimationName);
        }

        public void PlayAttackAnimation(string animationName)
        {
            _animator.Play(animationName, 0);
            AnimatorLog.LogInfo("공격 애니메이션을 재생합니다. {0}", animationName);
            _attackAnimationName = animationName;
            _isSequenceAttackAnimation = false;
        }

        public bool PlaySequenceAttackAnimation(string animationName)
        {
            if (_animator.UpdateAnimatorTriggerIfExists(animationName))
            {
                AnimatorLog.LogInfo("연속되는 공격 애니메이션을 재생합니다. {0}", animationName);

                _attackAnimationName = animationName;

                _isSequenceAttackAnimation = true;

                _animator.UpdateAnimatorBoolIfExists(animationName + "Progress", true);

                return true;
            }

            AnimatorLog.LogWarning("연속되는 공격 애니메이션 재생에 실패했습니다. {0}", animationName);

            return false;
        }

        public void StopSequenceAttackAnimation(string animationName)
        {
            _animator.UpdateAnimatorBoolIfExists(animationName + "Progress", false);
        }

        //

        protected virtual void OnAnimatorAttackStateEnter()
        {
            if (_ignoreFlipOnAttacking)
            {
                LockFlip();
            }

            SetAttacking(true);
        }

        protected virtual void OnAnimatorAttackStateExit()
        {
            StopAttacking();

            SetAttacking(false);
        }

        public void StopAttacking()
        {
            CancelAttackParameter();

            _animator.UpdateAnimatorBool(ANIMATOR_IS_ATTACKING_PARAMETER_ID, false, AnimatorParameters);

            if (_ignoreFlipOnAttacking)
            {
                UnlockFlip();
            }
        }

        private void CancelAttackParameter()
        {
        }

        //

        public void CallRefreshCooldownEvent()
        {
            RefreshAttackCooldown?.Invoke(_attackAnimationName);
        }

        public void RegisterRefreshCooldownEvent(UnityAction<string> action)
        {
            if (RefreshAttackCooldown == null)
            {
                RefreshAttackCooldown = new UnityEvent<string>();
            }

            RefreshAttackCooldown.AddListener(action);
        }

        public void UnregisterRefreshCooldownEvent(UnityAction<string> action)
        {
            if (RefreshAttackCooldown != null)
            {
                RefreshAttackCooldown.RemoveListener(action);
            }
        }
    }
}