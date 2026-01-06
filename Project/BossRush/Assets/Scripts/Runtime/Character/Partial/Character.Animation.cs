using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        // 애니메이션 (Animation)

        protected virtual void InitializeAnimatorParameters()
        {
            if (Animator != null)
            {
                AnimatorParameters = new HashSet<int>();

                Animator.AddAnimatorParameterIfExists(ANIMATOR_IDLE_PARAMETER_NAME, out _idleSpeedAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_ALIVE_PARAMETER_NAME, out _aliveAnimationParameter, AnimatorControllerParameterType.Bool, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_RANDOM_PARAMETER_NAME, out _randomAnimationParameter, AnimatorControllerParameterType.Float, AnimatorParameters);
                Animator.AddAnimatorParameterIfExists(ANIMATOR_RANDOM_CONSTANT_PARAMETER_NAME, out _randomConstantAnimationParameter, AnimatorControllerParameterType.Int, AnimatorParameters);

                // constant float 애니메이션 파라미터를 설정합니다.
                int randomConstant = RandomEx.Range(0, 1000);
                _ = Animator.UpdateAnimatorInteger(_randomConstantAnimationParameter, randomConstant, AnimatorParameters);
            }
            else
            {
                Log.Warning(LogTags.Animation, $"캐릭터의 애니메이터를 찾을 수 없습니다: {this.GetHierarchyName()}");
            }

            if (CharacterAnimator != null)
            {
                CharacterAnimator.Initialize();
            }
            else
            {
                Log.Warning(LogTags.Animation, $"캐릭터 애니메이터를 찾을 수 없습니다: {this.GetHierarchyName()}");
            }
        }

        protected virtual void UpdateAnimators()
        {
            if (Animator != null)
            {
                bool isAlive = StateMachine == null || StateMachine.CurrentState != CharacterState.Dead;
                _ = Animator.UpdateAnimatorBool(_aliveAnimationParameter, isAlive, AnimatorParameters);

                UpdateAnimationRandomNumber();

                _ = Animator.UpdateAnimatorFloat(_randomAnimationParameter, _animatorRandomNumber, AnimatorParameters);
            }

            // 물리 값 기반 애니메이터 파라미터 업데이트
            UpdatePhysicsAnimatorParameters();
        }

        protected virtual void UpdatePhysicsAnimatorParameters()
        {
            if (CharacterAnimator == null || Physics == null)
            {
                return;
            }

            // Bool 파라미터 업데이트
            CharacterAnimator.SetIsGrounded(Physics.IsGrounded);
            CharacterAnimator.SetIsLeftCollision(Physics.IsLeftCollision);
            CharacterAnimator.SetIsRightCollision(Physics.IsRightCollision);

            // 속도 파라미터 업데이트
            Vector2 velocity = Physics.RigidbodyVelocity;
            CharacterAnimator.SetSpeedX(Mathf.Abs(velocity.x));
            CharacterAnimator.SetSpeedY(velocity.y);

            // 방향 파라미터 업데이트 (하위 클래스에서 오버라이드 가능)
            UpdateDirectionalParameters();
        }

        protected virtual void UpdateDirectionalParameters()
        {
            // 기본 구현: FacingDirection을 사용
            if (CharacterAnimator != null && Physics != null)
            {
                float directionalX = Physics.FacingDirection;
                CharacterAnimator.SetDirectionalX(directionalX);
                CharacterAnimator.SetDirectionalY(0f);
            }
        }

        protected virtual void UpdateAnimationRandomNumber()
        {
            _animatorRandomNumber = RandomEx.Range(0f, 1f);
        }

        public void PlaySpawnAnimation()
        {
            CharacterAnimator?.PlaySpawnAnimation();
        }

        // 애니메이터 (Animator)

        public virtual void ChangeAnimator(Animator newAnimator)
        {
            Animator = newAnimator;

            if (Animator != null)
            {
                InitializeAnimatorParameters();
            }
            else
            {
                Log.Warning(LogTags.Animation, $"캐릭터의 애니메이터를 찾을 수 없습니다: {this.GetHierarchyName()}");
            }
        }

        public virtual void AssignAnimator()
        {
            if (Animator != null)
            {
                InitializeAnimatorParameters();
            }
            else
            {
                Log.Warning(LogTags.Animation, $"캐릭터의 애니메이터를 찾을 수 없습니다: {this.GetHierarchyName()}");
            }
        }

        public virtual void SetupAnimatorLayerWeight()
        {
        }
    }
}