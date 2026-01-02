using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using TeamSuneat.Feedbacks;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAbility : XBehaviour
    {
        public enum Types
        {
            None,
            HorizontalMovement,
            Fly,
            Crouch,
            Jump,
            WallJump,
            Ladder,
            LedgeHang,
            Dash,
            Dive,
            Grab,
            Grabbed,
            Stun,
            Skill,
            Portal,
            Interaction,
            ConsumePotion,
            ForceVelocity,
            MoveToDestination,
        }

        [FoldoutGroup("#Feedbacks")] public GameFeedbacks AbilityStartFeedbacks;
        [FoldoutGroup("#Feedbacks")] public GameFeedbacks AbilityStopFeedbacks;

        [FoldoutGroup("#Component")] public Character Owner;
        [FoldoutGroup("#Component")] public PhysicsController Controller;
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

                    if (!CheckBlockingMovementStates())
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

        public virtual MovementStates[] BlockingMovementStates { get; }

        public virtual CharacterConditions[] BlockingConditionStates { get; }

        protected StateMachine<MovementStates> _movement { get; private set; }

        protected StateMachine<CharacterConditions> _condition { get; private set; }

        protected bool _abilityInitialized = false;
        protected float _verticalInput;
        protected float _horizontalInput;
        protected bool _startFeedbackIsPlaying = false;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = GetComponent<Character>();
            Controller = GetComponent<TSPhysicsController>();

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
                _movement = Owner.MovementState;
                _condition = Owner.ConditionState;
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
            InternalHandleInput();
        }

        public virtual void ProcessAbility()
        {
        }

        public virtual void LateProcessAbility()
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

        #region Input

        protected virtual void InternalHandleInput()
        {
            if (TSInputManager.Instance == null)
            {
                return;
            }

            if (!Owner.IsAlive)
            {
                return;
            }

            if (Owner.IsBlockInput || GameSetting.Instance.Input.IsBlockCharacterInput)
            {
                ResetInput();

                return;
            }

            if (Owner.IsPlayer)
            {
                _verticalInput = TSInputManager.Instance.PrimaryMovement.y;
                _horizontalInput = TSInputManager.Instance.PrimaryMovement.x;
            }

            HandleInput();
        }

        protected virtual void HandleInput()
        {
        }

        public virtual void ResetInput()
        {
            _horizontalInput = 0f;
            _verticalInput = 0f;
        }

        #endregion Input

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

        private bool CheckBlockingMovementStates()
        {
            if ((BlockingMovementStates != null) && (BlockingMovementStates.Length > 0))
            {
                for (int i = 0; i < BlockingMovementStates.Length; i++)
                {
                    if (BlockingMovementStates[i] == Owner.MovementState.CurrentState)
                    {
                        LogProgress("{0} 이동 상태일 때 해당 능력을 사용할 수 없습니다.", Owner.MovementState.CurrentState);
                        return false;
                    }
                }
            }

            return true;
        }

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