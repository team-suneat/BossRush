using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        // CharacterAnimator에서 사용하는 Attack은 몬스터의 공격으로 제한합니다.

        /// <summary>
        /// 공격 애니메이션 이름
        /// </summary>
        protected string _attackAnimationName;

        /// <summary>
        /// 순서대로 재생되는 공격 애니메이션 여부
        /// : 돌진과 같이 시작-반복-완료 애니메이션이 각각 설정된 공격 애니메이션을 뜻합니다.
        /// </summary>
        private bool _isSequenceAttackAnimation;

        /// <summary>
        /// 재생되고 있는 공격 애니메이션 이름
        /// </summary>
        protected string AttackingAnimationName
        {
            get
            {
                if (_isSequenceAttackAnimation)
                {
                    return _attackAnimationName + "Complete";
                }
                else
                {
                    return _attackAnimationName;
                }
            }
        }

        protected UnityEvent<string> RefreshAttackCooldown { get; set; }

        private bool IsAttackState(AnimatorStateInfo stateInfo, bool isEnter)
        {
            if (!isEnter && _isSequenceAttackAnimation)
            {
                if (stateInfo.IsName(_attackAnimationName + "Complete"))
                {
                    return true;
                }
            }
            else if (stateInfo.IsName(_attackAnimationName))
            {
                return true;
            }

            return false;
        }

        public bool PlayAttackAnimation(string animationName)
        {
            if (_animator.UpdateAnimatorTriggerIfExists(animationName))
            {
                AnimatorLog.LogInfo("공격 애니메이션을 재생합니다. {0}", animationName);

                _attackAnimationName = animationName;

                _isSequenceAttackAnimation = false;

                return true;
            }

            AnimatorLog.LogWarning("공격 애니메이션 재생에 실패했습니다. {0}", animationName);

            return false;
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
            AnimatorLog.LogInfo("공격 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);

            _animator.UpdateAnimatorBool(ANIMATOR_IS_ATTACKING_PARAMETER_ID, true, AnimatorParameters);

            _owner.ChangeMovementState(MovementStates.Attack);

            if (IgnoreFlipOnAttacking)
            {
                LockFlip();
            }

            IsAttacking = true;

            //

            if (DetermineMovementLockWhileAttack(AttackingAnimationName))
            {
                LockMovement();
            }
        }

        protected virtual void OnAnimatorAttackStateExit()
        {
            UnlockMovement();

            StopAttacking();

            IsAttacking = false;

            ResetBlockDamageAnimationWhileAttack();
        }

        public void StopAttacking()
        {
            CancelAttackParameter();

            _animator.UpdateAnimatorBool(ANIMATOR_IS_ATTACKING_PARAMETER_ID, false, AnimatorParameters);

            if (_owner.MovementState.Compare(MovementStates.Attack))
            {
                _owner.ChangeMovementState(MovementStates.Idle);
            }

            if (IgnoreFlipOnAttacking)
            {
                UnlockFlip();
            }
        }

        private void CancelAttackParameter()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_ATTACKING_1_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_ATTACKING_2_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_ATTACKING_3_PARAMETER_ID, false, AnimatorParameters);
        }

        //

        public void SetBlockDamageAnimationWhileAttack()
        {
            IsBlockingDamageAnimationWhileAttack = true;
            AnimatorLog.LogInfo($"공격 중 피격 애니메이션 재생 차단: {true.ToBoolString()}");
        }

        public void ResetBlockDamageAnimationWhileAttack()
        {
            IsBlockingDamageAnimationWhileAttack = false;
            AnimatorLog.LogInfo($"공격 중 피격 애니메이션 재생 차단: {false.ToBoolString()}");
        }

        public void SetBlockDamageAnimationWhileCast(SkillNames skillName)
        {
            if (!BlockingDamageAnimationWhileCast.Contains(skillName))
            {
                BlockingDamageAnimationWhileCast.Add(skillName);
                AnimatorLog.LogInfo($"시전({skillName}) 중 피격 애니메이션 재생 차단: {true.ToBoolString()}");
            }
        }

        public void ResetBlockDamageAnimationWhileCast(SkillNames skillName)
        {
            if (BlockingDamageAnimationWhileCast.Contains(skillName))
            {
                BlockingDamageAnimationWhileCast.Remove(skillName);
                AnimatorLog.LogInfo($"시전({skillName}) 중 피격 애니메이션 재생 차단: {false.ToBoolString()}");
            }
        }

        public void ClearBlockDamageAnimationWhileCast()
        {
            BlockingDamageAnimationWhileCast.Clear();
            AnimatorLog.LogInfo($"등록된 모든 기술 시전 중 피격 애니메이션 재생 차단: {false.ToBoolString()}");
        }

        public void SetBlockDamageAnimationWhileGrab()
        {
            IsBlockingDamageAnimationWhileGrab = true;
            AnimatorLog.LogInfo($"잡기 중 피격 애니메이션 재생 차단: {true.ToBoolString()}");
        }

        public void ResetBlockDamageAnimationWhileGrab()
        {
            IsBlockingDamageAnimationWhileGrab = true;
            AnimatorLog.LogInfo($"잡기 중 피격 애니메이션 재생 차단: {false.ToBoolString()}");
        }

        public void CallRefreshCooldwonEvent()
        {
            RefreshAttackCooldown?.Invoke(_attackAnimationName);
        }

        public void RegisterRefreshCooldwonEvent(UnityAction<string> action)
        {
            if (RefreshAttackCooldown == null)
            {
                RefreshAttackCooldown = new UnityEvent<string>();
            }

            RefreshAttackCooldown.AddListener(action);
        }

        public void UnregisterRefreshCooldwonEvent(UnityAction<string> action)
        {
            if (RefreshAttackCooldown != null)
            {
                RefreshAttackCooldown.RemoveListener(action);
            }
        }
    }
}