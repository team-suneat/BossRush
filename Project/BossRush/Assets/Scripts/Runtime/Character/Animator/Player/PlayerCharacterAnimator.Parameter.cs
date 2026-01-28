namespace TeamSuneat
{
    public partial class PlayerCharacterAnimator
    {
        public override void Initialize()
        {
            base.Initialize();
            _animator.UpdateAnimatorBool(ANIMATOR_IS_CASTING_PARAMETER_ID, false, AnimatorParameters);
        }
    }
}
