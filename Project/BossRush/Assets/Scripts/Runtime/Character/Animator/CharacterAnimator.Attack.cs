using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        protected string _attackAnimationName;
        private bool _isSequenceAttackAnimation;

        protected UnityEvent<string> RefreshAttackCooldown { get; set; }

        private bool IsAttackState(AnimatorStateInfo stateInfo, bool isEnter)
        {
            if (!isEnter && _isSequenceAttackAnimation)
            {
                return stateInfo.IsName(_attackAnimationName + "Complete");
            }

            return stateInfo.IsName(_attackAnimationName);
        }

        public bool IsPlayingAttackAnimation()
        {
            if (_animator == null || string.IsNullOrEmpty(_attackAnimationName))
            {
                return false;
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (_isSequenceAttackAnimation)
            {
                return stateInfo.IsName(_attackAnimationName) || stateInfo.IsName(_attackAnimationName + "Complete");
            }

            return stateInfo.IsName(_attackAnimationName);
        }

        //
        public void PlayAttackAnimation(string animationName)
        {
            _animator.Play(animationName, 0);
            _attackAnimationName = animationName;
            _isSequenceAttackAnimation = false;

            AnimatorLog.LogInfo("공격 애니메이션을 재생합니다. {0}", animationName);
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
            if (!_ignoreFlipOnAttacking)
            {
                LockFlip();
            }
            SetAttacking(true);
        }

        protected virtual void OnAnimatorAttackStateExit()
        {
            if (!_ignoreFlipOnAttacking)
            {
                UnlockFlip();
            }
            SetAttacking(false);
            ProcessNextStep();
        }

        public void ForceStopAttack()
        {
            if (!_ignoreFlipOnAttacking)
            {
                UnlockFlip();
            }
            SetAttacking(false);
        }

        //

        protected void ProcessNextStep()
        {
            if (_owner.IsPlayer)
            {
                return;
            }

            MonsterCharacter enemy = _owner as MonsterCharacter;
            if (enemy != null && enemy.Pattern != null)
            {
                enemy.Pattern.OnAttackStateExited();
            }
        }

        //

        public void CallRefreshCooldownEvent()
        {
            RefreshAttackCooldown?.Invoke(_attackAnimationName);
        }

        public void RegisterRefreshCooldownEvent(UnityAction<string> action)
        {
            RefreshAttackCooldown ??= new UnityEvent<string>();

            RefreshAttackCooldown.AddListener(action);
        }

        public void UnregisterRefreshCooldownEvent(UnityAction<string> action)
        {
            RefreshAttackCooldown?.RemoveListener(action);
        }
    }
}