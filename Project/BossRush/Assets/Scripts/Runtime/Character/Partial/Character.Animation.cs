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
                _ = Animator.UpdateAnimatorBool(_aliveAnimationParameter, ConditionState.CurrentState != CharacterConditions.Dead, AnimatorParameters);

                UpdateAnimationRandomNumber();

                _ = Animator.UpdateAnimatorFloat(_randomAnimationParameter, _animatorRandomNumber, AnimatorParameters);

                for (int i = 0; i < Abilities.Length; i++)
                {
                    if (Abilities[i].enabled && Abilities[i].AbilityInitialized)
                    {
                        Abilities[i].UpdateAnimator();
                    }
                }
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