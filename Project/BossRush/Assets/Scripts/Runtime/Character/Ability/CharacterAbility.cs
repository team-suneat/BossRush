using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using TeamSuneat.Feedbacks;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAbility : XBehaviour
    {
        public enum Types
        {
            None,
            Dash,
            Stun,
            Skill,
            Interaction,
            Targeting,
            AutoAttack,
        }

        [FoldoutGroup("#Feedbacks")] public GameFeedbacks AbilityStartFeedbacks;
        [FoldoutGroup("#Feedbacks")] public GameFeedbacks AbilityStopFeedbacks;

        [FoldoutGroup("#Component")] public Character Owner;
        [FoldoutGroup("#Component")] public Vital Vital;
        [FoldoutGroup("#Component")] public Animator Animator;

        protected VProfile ProfileInfo => GameApp.GetSelectedProfile();

        public virtual bool IsAuthorized
        {
            get
            {
                if (Owner != null)
                {
                    if (!Owner.IsAlive)
                    {
                        return false;
                    }

                    if (!CheckBlockingConditionStates(BlockingConditionStates))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool AbilityInitialized => _abilityInitialized;

        public virtual Types Type => Types.None;

        public virtual CharacterConditions[] BlockingConditionStates { get; }


        protected StateMachine<CharacterConditions> _conditionState { get; private set; }

        protected bool _abilityInitialized = false;

        protected bool _startFeedbackIsPlaying = false;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = GetComponent<Character>();

            if (Owner != null)
            {
                Vital = Owner.MyVital;
                Animator = Owner.Animator;
            }
        }

        //

        public virtual void Initialization()
        {
            BindAnimator();

            if (Owner != null)
            {
                _conditionState = Owner.ConditionState;
            }

            LogInfo("캐릭터 생성시 캐릭터 능력을 초기화합니다.");

            _abilityInitialized = true;
        }

        public virtual void ResetAbility()
        {
            LogInfo("특정 시점에 캐릭터 능력을 초기화합니다.");
        }

        #region Event

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (Vital == null)
            {
                Vital = gameObject.GetComponentInChildren<Vital>();
            }

            if (Vital != null)
            {
                Vital.Health.RegisterOnDamageEvent(OnDamage);
                Vital.Health.RegisterOnDeathEvent(OnDeath);
                Vital.Health.RegisterOnReviveEvent(OnRespawn);
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            if (Vital != null)
            {
                Vital.Health.UnregisterOnDamageEvent(OnDamage);
                Vital.Health.UnregisterOnDeathEvent(OnDeath);
                Vital.Health.UnregisterOnReviveEvent(OnRespawn);
            }
        }

        protected virtual void OnRespawn()
        {
        }

        protected virtual void OnDeath(DamageResult damageResult)
        {
            StopStartFeedbacks();
        }

        protected virtual void OnDamage(DamageResult damageResult)
        {
        }

        #endregion Event

        #region Process

        public virtual void EarlyProcessAbility()
        {
        }

        public virtual void ProcessAbility()
        {
        }

        public virtual void LateProcessAbility()
        {
        }

        public virtual void PhysicsProcessAbility()
        {
        }

        #endregion Process

        #region Animator & Renderer

        public virtual void BindAnimator()
        {
            if (Animator != null)
            {
                InitializeAnimatorParameters();
            }
        }

        protected virtual void InitializeAnimatorParameters()
        {
        }

        public virtual void UpdateAnimator()
        {
        }

        #endregion Animator & Renderer

        #region Feedback

        protected virtual void PlayAbilityStartFeedbacks()
        {
            if (AbilityStartFeedbacks != null)
            {
                AbilityStartFeedbacks.PlayFeedbacks(transform.position, 0);
            }

            _startFeedbackIsPlaying = true;
        }

        public virtual void StopStartFeedbacks()
        {
            if (AbilityStartFeedbacks != null)
            {
                AbilityStartFeedbacks.StopFeedbacks();
            }

            _startFeedbackIsPlaying = false;
        }

        protected virtual void PlayAbilityStopFeedbacks()
        {
            if (AbilityStopFeedbacks != null)
            {
                AbilityStopFeedbacks.PlayFeedbacks();
            }
        }

        #endregion Feedback

        #region Conditions

        protected bool CheckBlockingConditionStates(CharacterConditions[] conditions)
        {
            if ((conditions != null) && (conditions.Length > 0))
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (conditions[i] == Owner.ConditionState.CurrentState)
                    {
                        LogProgress("{0} 조건 상태일 때 해당 능력을 사용할 수 없습니다.", Owner.ConditionState.CurrentState);
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion Conditions
    }
}