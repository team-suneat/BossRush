using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;

using UnityEngine;

namespace TeamSuneat
{
    public class CharacterGrabbed : CharacterAbility
    {
        //---------------------------------------------------------------------------------------------------------------

        [Title("#캐릭터 잡힘 능력 (Grabbed Ability)")]
        public CharacterHorizontalMovement HorizontalMovement;

        public CharacterForceVelocity ForceVelocity;

        //

        private Character _grabber;
        private CharacterGrab _grabberAbility;
        private Coroutine _grabbedCoroutine;

        //--------------------------------------------------------------------------------------------------------------
        private const string ANIMATOR_IS_GRABBED_PARAMETER_NAME = "IsGrabbed";

        private int _grabbednedAnimationParameter;

        //--------------------------------------------------------------------------------------------------------------

        public override Types Type => Types.Grabbed;

        public bool IsGrabbed => _grabbedCoroutine != null;

        //---------------------------------------------------------------------------------------------------------------

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            AbilityStartFeedbacks = this.FindComponent<GameFeedbacks>("Feedbacks/GrabbedFeedbacks");
            HorizontalMovement = GetComponent<CharacterHorizontalMovement>();
            ForceVelocity = GetComponent<CharacterForceVelocity>();
        }

        public void SetGrabber(Character character)
        {
            if (character != null)
            {
                _grabber = character;
                _grabberAbility = character.FindAbility<CharacterGrab>();

                LogInfo("잡기 캐릭터({0})를 설정합니다.", _grabber.Name.ToLogString());
            }
        }

        //

        public void StartGrabbed()
        {
            LogInfo("그랩 상태를 시작합니다");

            Owner.Attack?.DeactivateAll();
            ForceVelocity?.StopForceVelocity();

            HorizontalMovement?.LockMovement(this);

            Owner.LockFlip();
            Owner.ChangeConditionState(CharacterConditions.Grabbed);
            Owner.CharacterAnimator?.StopPlayingSkillAnimation(true);

            Controller.PausePhysics();

            if (_grabberAbility != null)
            {
                _grabberAbility.StartGrab(Owner);
            }

            AbilityStartFeedbacks?.PlayFeedbacks();
        }

        public void StopGrabbed()
        {
            ExitGrabbed();
        }

        private void ExitGrabbed()
        {
            LogInfo("그랩 상태를 종료합니다");

            AbilityStartFeedbacks?.StopFeedbacks();
            AbilityStopFeedbacks?.PlayFeedbacks();

            HorizontalMovement?.UnlockMovement(this);

            if (!Owner.IsAlive)
            {
                Owner.ChangeConditionState(CharacterConditions.Dead);
            }
            else
            {
                Owner.ChangeConditionState(CharacterConditions.Normal);
            }

            Owner.UnlockFlip();

            Controller.ResumePhysics();

            if (_grabberAbility != null)
            {
                _grabberAbility.StopGrab();
            }

            StopXCoroutine(ref _grabbedCoroutine);
        }

        // Animator

        protected override void InitializeAnimatorParameters()
        {
            LogInfo("grab 애니메이터 파라메터 설정");

            Animator?.AddAnimatorParameterIfExists(ANIMATOR_IS_GRABBED_PARAMETER_NAME, out _grabbednedAnimationParameter, AnimatorControllerParameterType.Bool, Owner.AnimatorParameters);
        }

        public override void UpdateAnimator()
        {
            bool isGrabbeded = (_condition.CurrentState == CharacterConditions.Grabbed);

            Animator?.UpdateAnimatorBool(_grabbednedAnimationParameter, isGrabbeded, Owner.AnimatorParameters, Owner.PerformAnimatorSanityChecks);
        }
    }
}