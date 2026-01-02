using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterHorizontalMovement
    {
        private const string ANIMATOR_SPEED_PARAMETER_NAME = "Speed";
        private const string ANIMATOR_WALK_PARAMETER_NAME = "Walk";
        private const string ANIMATOR_IS_WALKING_PARAMETER_NAME = "Walking";

        private int _speedAnimationParameter;
        private int _walkAnimationParameter;
        private int _walkingAnimationParameter;

        protected override void InitializeAnimatorParameters()
        {
            Animator?.AddAnimatorParameterIfExists(ANIMATOR_SPEED_PARAMETER_NAME, out _speedAnimationParameter, AnimatorControllerParameterType.Float, Owner.AnimatorParameters);
            Animator?.AddAnimatorParameterIfExists(ANIMATOR_WALK_PARAMETER_NAME, out _walkAnimationParameter, AnimatorControllerParameterType.Bool, Owner.AnimatorParameters);
            Animator?.AddAnimatorParameterIfExists(ANIMATOR_IS_WALKING_PARAMETER_NAME, out _walkingAnimationParameter, AnimatorControllerParameterType.Bool, Owner.AnimatorParameters);
        }

        public override void UpdateAnimator()
        {
            Animator?.UpdateAnimatorFloat(_speedAnimationParameter, Mathf.Abs(_horizontalNormalizedSpeed), Owner.AnimatorParameters, Owner.PerformAnimatorSanityChecks);
            Animator?.UpdateAnimatorBool(_walkingAnimationParameter, _movement.CurrentState == MovementStates.Walking, Owner.AnimatorParameters, Owner.PerformAnimatorSanityChecks);
        }

        private void UpdateWalkAnimationParameter(bool value)
        {
            Animator?.UpdateAnimatorBool(_walkAnimationParameter, value, Owner.AnimatorParameters, Owner.PerformAnimatorSanityChecks);
        }
    }
}