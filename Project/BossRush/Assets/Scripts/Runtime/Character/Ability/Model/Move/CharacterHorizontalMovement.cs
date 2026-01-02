using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Feedbacks;

using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterHorizontalMovement : CharacterAbility
    {
        public override Types Type => Types.HorizontalMovement;

        private const float PENALTY_1_SPEED = 6.5f;
        private const float PENALTY_2_SPEED = 8f;
        private const float PENALTY_1_MULTIPIRER = 0.8f; // 20%
        private const float PENALTY_2_MULTIPIRER = 0.5f; // 50%

        public override bool IsAuthorized
        {
            get
            {
                if (!AbilityPermitted)
                {
                    return false;
                }
                return base.IsAuthorized;
            }
        }

        public float MovementSpeed { get; set; }

        public float MovementSpeedMultiplier { get; set; } = 1f;

        public float AIMovementSpeedMultiplier { get; set; } = 1f;

        [ReadOnly]
        [SerializeField]
        [FoldoutGroup("#Horizontal Movement")] private float _calculatedMovementSpeed;

        [LabelText("방향에 따라 바라봅니다 (Face)")]
        [FoldoutGroup("#Horizontal Movement")] public bool FaceDependingOnDirection;

        [LabelText("방향에 따라 돌아섭니다 (Turn)")]
        [FoldoutGroup("#Horizontal Movement")] public bool TurnDependingOnDirection;

        [Tooltip("캐릭터가 땅에 떨어질 때 재생할 GameFeedbacks")]
        [FoldoutGroup("#Horizontal Movement-Touching the Ground")] public GameFeedbacks TouchTheGroundFeedback;

        [SerializeField]
        [FoldoutGroup("#Horizontal Movement-Touching the Ground")] protected float _horizontalMovement;

        [SerializeField]
        [FoldoutGroup("#Horizontal Movement-Touching the Ground")] protected float _horizontalNormalizedSpeed;

        [FoldoutGroup("#Horizontal Movement-Permissions")]
        public bool AbilityPermitted = true;

        [FoldoutGroup("#Horizontal Movement-Feedback")] public GameFeedbacks OnStartFeedbacks;
        [FoldoutGroup("#Horizontal Movement-Feedback")] public GameFeedbacks OnStopFeedbacks;

        //

        protected float _lastHorizontalMovement;
        protected float _lastGroundedHorizontalMovement;
        protected float _horizontalMovementForce;

        protected float _lastTimeGrounded = 0f;
        protected float _initialMovementSpeedMultiplier;

        private CharacterPhysicsAsset _physicsAsset;

        public override void Initialization()
        {
            base.Initialization();

            LoadPhysicsData();

            if (_physicsAsset.IsValid())
            {
                SetupMovementSpeed();
                UnlockAllMovement();
            }
            else
            {
                Log.Error("캐릭터의 물리 에셋을 불러올 수 없습니다. {0}", Owner.Name.ToLogString());
            }
        }

        private void LoadPhysicsData()
        {
            _physicsAsset = ScriptableDataManager.Instance.FindCharacterPhysics(Owner.Name);
        }

        private void SetupMovementSpeed()
        {
            MovementSpeed = _physicsAsset.Data.WalkSpeed;
            MovementSpeedMultiplier = 1f;
            AIMovementSpeedMultiplier = 1f;
            _initialMovementSpeedMultiplier = MovementSpeedMultiplier;
        }

        //

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (TryStopStartFeedbacks())
            {
                StopStartFeedbacks();
            }

            if (TryHandleHorizontalMovement())
            {
                HandleHorizontalMovement();
            }

            DetectWalls();
        }

        protected override void HandleInput()
        {
            if (!_physicsAsset.Data.ReadInput)
            {
                return;
            }

            _lastHorizontalMovement = _horizontalMovement;

            if (Owner.IsPlayer)
            {
                _horizontalMovement = base._horizontalInput;

                if (_physicsAsset.Data.AirControl < 1f)
                {
                    if (!Owner.PhysicsController.Controller.IsGrounded)
                    {
                        _horizontalMovement = Mathf.Lerp(_lastGroundedHorizontalMovement, base._horizontalInput, _physicsAsset.Data.AirControl);
                    }
                }
            }
        }

        public override void ResetInput()
        {
            base.ResetInput();

            ResetHorizontalMove();
        }

        public virtual void SetAirControlDirection(float newInputValue)
        {
            _lastGroundedHorizontalMovement = newInputValue;
        }

        public void SetHorizontalMove(float value)
        {
            _horizontalMovement = value;
        }

        public void ResetHorizontalMove()
        {
            _horizontalMovement = 0f;
        }

        private bool TryHandleHorizontalMovement()
        {
            if (!IsAuthorized)
            {
                return false;
            }

            if (!_physicsAsset.Data.ActiveAfterDeath)
            {
                if (_condition.CurrentState != CharacterConditions.Normal)
                {
                    return false;
                }
            }

            return true;
        }

        private void HandleHorizontalMovement()
        {
            CheckJustGotGrounded();

            StoreLastTimeGrounded();

            if (MovementForbidden)
            {
                _horizontalMovement = Owner.IsAirborne ? Controller.Speed.x * Time.deltaTime : 0f;
            }

            ComputeNormalizedHorizontalSpeed();

            if (DetermineFlip())
            {
                if (FaceDependingOnDirection)
                {
                    Face();
                }
                else if (TurnDependingOnDirection)
                {
                    Turn();
                }
            }

            if (Controller.State.IsGrounded && (_horizontalNormalizedSpeed != 0) && _movement.Or(MovementStates.Idle, MovementStates.Dangling, MovementStates.Falling))
            {
                UpdateWalkAnimationParameter(true);
                Owner.ChangeMovementState(MovementStates.Walking);
                PlayAbilityStartFeedbacks();
            }
            else
            {
                UpdateWalkAnimationParameter(false);
            }

            if (Controller.State.IsGrounded && (_movement.CurrentState == MovementStates.Jumping))
            {
                if (Controller.TimeAirborne >= Owner.AirborneMinimumTime)
                {
                    Owner.ChangeMovementState(MovementStates.Idle);
                }
            }

            if (_movement.CurrentState == MovementStates.Walking)
            {
                if (_horizontalNormalizedSpeed == 0)
                {
                    Owner.ChangeMovementState(MovementStates.Idle);

                    PlayAbilityStopFeedbacks();
                }
            }

            if (!Controller.State.IsGrounded)
            {
                if (_movement.CurrentState is MovementStates.Walking or MovementStates.Idle)
                {
                    Owner.ChangeMovementState(MovementStates.Falling);
                }
            }

            /// 필요한 경우 순간 가속을 적용합니다.
            if (_physicsAsset.Data.InstantAcceleration)
            {
                if (_horizontalNormalizedSpeed > 0f) { _horizontalNormalizedSpeed = 1f; }
                if (_horizontalNormalizedSpeed < 0f) { _horizontalNormalizedSpeed = -1f; }
            }

            float movementFactor = GetMovementFactor();
            _calculatedMovementSpeed = CalculateMovementSpeed();

            if (_physicsAsset.Data.InstantAcceleration)
            {
                _horizontalMovementForce = _calculatedMovementSpeed;
                if (Mathf.Abs(Controller.ExternalForce.x) > 0)
                {
                    _horizontalMovementForce += Controller.ExternalForce.x;
                }
            }
            else
            {
                _horizontalMovementForce = Mathf.Lerp(Controller.Speed.x, _calculatedMovementSpeed, Time.deltaTime * movementFactor);
            }

            _horizontalMovementForce = HandleFriction(_horizontalMovementForce);

            Controller.SetHorizontalForce(_horizontalMovementForce);

            ResetHorizontalMove();

            if (Controller.State.IsGrounded)
            {
                _lastGroundedHorizontalMovement = _horizontalMovement;
            }
        }

        private void ComputeNormalizedHorizontalSpeed()
        {
            if (_horizontalMovement > 0)
            {
                if (_physicsAsset.Data.InstantAcceleration)
                {
                    if (_lastHorizontalMovement <= base._horizontalInput)
                    {
                        _horizontalNormalizedSpeed = _horizontalMovement;
                    }
                    else
                    {
                        _horizontalNormalizedSpeed = 0;
                    }
                }
                else
                {
                    _horizontalNormalizedSpeed = _horizontalMovement;
                }
            }
            else if (_horizontalMovement < 0)
            {
                if (_physicsAsset.Data.InstantAcceleration)
                {
                    if (_lastHorizontalMovement >= base._horizontalInput)
                    {
                        _horizontalNormalizedSpeed = _horizontalMovement;
                    }
                    else
                    {
                        _horizontalNormalizedSpeed = 0;
                    }
                }
                else
                {
                    _horizontalNormalizedSpeed = _horizontalMovement;
                }
            }
            else
            {
                _horizontalNormalizedSpeed = 0;
            }
        }

        private bool DetermineFlip()
        {
            if (MovementForbidden)
            {
                return false;
            }

            if (Owner.CharacterAnimator.IsDamaging)
            {
                // 피격 중에는 방향을 전환할 수 없습니다.
                return false;
            }

            if (!_physicsAsset.Data.AllowFlipInTheAir)
            {
                if (!Controller.State.IsGrounded)
                {
                    return false;
                }
            }

            return true;
        }

        private void Face()
        {
            if (_horizontalNormalizedSpeed > 0)
            {
                Owner.Face(Character.FacingDirections.Right);
            }
            else if (_horizontalNormalizedSpeed < 0)
            {
                Owner.Face(Character.FacingDirections.Left);
            }
        }

        private void Turn()
        {
            if (_horizontalNormalizedSpeed > 0)
            {
                if (!Owner.IsFacingRight)
                {
                    Owner.CharacterAnimator.PlayTurnAnimation();
                }
            }
            else if (_horizontalNormalizedSpeed < 0)
            {
                if (Owner.IsFacingRight)
                {
                    Owner.CharacterAnimator.PlayTurnAnimation();
                }
            }
        }

        private float GetMovementFactor()
        {
            float groundAcceleration = Controller.Parameters.SpeedAccelerationOnGround;
            float airAcceleration = Controller.Parameters.SpeedAccelerationInAir;

            if (Mathf.Abs(_horizontalMovement) < 0)
            {
                if (Controller.Parameters.UseSeparateDecelerationOnGround)
                {
                    groundAcceleration = Controller.Parameters.SpeedDecelerationOnGround;
                }
                if (Controller.Parameters.UseSeparateDecelerationInAir)
                {
                    airAcceleration = Controller.Parameters.SpeedDecelerationInAir;
                }
            }

            return Controller.State.IsGrounded ? groundAcceleration : airAcceleration;
        }

        private float CalculateMovementSpeed()
        {
            float result = _horizontalNormalizedSpeed * MovementSpeed * Controller.Parameters.SpeedFactor * MovementSpeedMultiplier * AIMovementSpeedMultiplier;
            float penaltySpeed = 0;

            if (result > PENALTY_2_SPEED)
            {
                // ((총 이속 - 패널티1 기준 이동속도) * 패널티1 이동속도 배율) + ( (패널티2 기준 이동속도- 패널티1 기준 이동속도) ＊ 패널티2 이동속도 배율) + 패널티1 기준 이동속도
                penaltySpeed = (result - PENALTY_2_SPEED) * PENALTY_2_MULTIPIRER;
                penaltySpeed += (PENALTY_2_SPEED - PENALTY_1_SPEED) * PENALTY_1_MULTIPIRER;
                penaltySpeed += PENALTY_1_SPEED;

                Log.Info(LogTags.Physics, "이동속도 페널티를 부여합니다. {0} ▶ {1}", result, penaltySpeed);

                result = penaltySpeed;
            }
            else if (result > PENALTY_1_SPEED)
            {
                // ((총 이속 - 패널티1 기준 이동속도) * 패널티1 이동속도 배율) + 패널티1 기준 이동속도
                penaltySpeed = (result - PENALTY_1_SPEED) * PENALTY_1_MULTIPIRER;
                penaltySpeed += PENALTY_1_SPEED;

                Log.Info(LogTags.Physics, "이동속도 페널티를 부여합니다. {0} ▶ {1}", result, penaltySpeed);

                result = penaltySpeed;
            }

            return result;
        }

        protected virtual void DetectWalls()
        {
            if (!_physicsAsset.Data.StopWalkingWhenCollidingWithAWall)
            {
                return;
            }

            if (_movement.CurrentState is MovementStates.Walking or MovementStates.Running)
            {
                if (Controller.State.IsCollidingLeft || Controller.State.IsCollidingRight)
                {
                    Owner.ChangeMovementState(MovementStates.Idle);
                }
            }
        }

        protected virtual void CheckJustGotGrounded()
        {
            // 캐릭터가 방금 접지 된 경우
            if (_movement.CurrentState == MovementStates.Dashing)
            {
                return;
            }

            if (Controller.State.JustGotGrounded)
            {
                if (_movement.CurrentState != MovementStates.Jumping)
                {
                    if (Controller.State.ColliderResized)
                    {
                        Owner.ChangeMovementState(MovementStates.Crouching);
                    }
                    else
                    {
                        Owner.ChangeMovementState(MovementStates.Idle);
                    }
                }

                Controller.SlowFall(0f);
                if (Time.time - _lastTimeGrounded > _physicsAsset.Data.MinimumAirTimeBeforeFeedback)
                {
                    TouchTheGroundFeedback?.PlayFeedbacks();
                }
            }
        }

        protected virtual void StoreLastTimeGrounded()
        {
            if (Controller.State.IsGrounded
                || (Owner.MovementState.CurrentState == MovementStates.LadderClimbing)
                || (Owner.MovementState.CurrentState == MovementStates.LedgeClimbing)
                || (Owner.MovementState.CurrentState == MovementStates.LedgeHanging)
                || (Owner.MovementState.CurrentState == MovementStates.Gripping)
                || (Owner.MovementState.CurrentState == MovementStates.SwimmingIdle))
            {
                _lastTimeGrounded = Time.time;
            }
        }

        protected virtual float HandleFriction(float force)
        {
            if (Controller.Friction > 1)
            {
                force /= Controller.Friction;
            }

            if (Controller.Friction is < 1 and > 0)
            {
                force = Mathf.Lerp(Controller.Speed.x, force, Time.deltaTime * Controller.Friction * 10);
            }

            return force;
        }

        public virtual void ResetHorizontalSpeed()
        {
            if (MovementSpeedMultiplier != _physicsAsset.Data.WalkSpeed)
            {
                MovementSpeed = _physicsAsset.Data.WalkSpeed;
                Log.Info(LogTags.Physics, "{0}, 수평 이동 속도를 불러옵니다. {1}", Owner.Name.ToLogString(), ValueStringEx.GetPercentString(MovementSpeedMultiplier));
            }
        }

        public virtual void SetMovementSpeedMultiplier(float movementSpeedMultiplier)
        {
            if (MovementSpeedMultiplier != movementSpeedMultiplier)
            {
                MovementSpeedMultiplier = movementSpeedMultiplier;
                Log.Info(LogTags.Physics, "{0}, 수평 이동 속도 배율을 설정합니다. {1}", Owner.Name.ToLogString(), ValueStringEx.GetPercentString(MovementSpeedMultiplier));
            }
        }

        public virtual void ResetMovementSpeedMultiplier()
        {
            if (MovementSpeedMultiplier != _initialMovementSpeedMultiplier)
            {
                MovementSpeedMultiplier = _initialMovementSpeedMultiplier;
                Log.Info(LogTags.Physics, "{0}, 수평 이동 속도 배율을 초기화합니다. {1}", Owner.Name.ToLogString(), ValueStringEx.GetPercentString(MovementSpeedMultiplier));
            }
        }

        protected override void PlayAbilityStartFeedbacks()
        {
            base.PlayAbilityStartFeedbacks();

            if (OnStartFeedbacks != null)
            {
                OnStartFeedbacks.PlayFeedbacks(transform.position, 0);
            }
        }

        private bool TryStopStartFeedbacks()
        {
            if (_movement.CurrentState != MovementStates.Walking)
            {
                if (_startFeedbackIsPlaying)
                {
                    return true;
                }
            }

            return false;
        }

        public override void StopStartFeedbacks()
        {
            base.StopStartFeedbacks();

            if (OnStartFeedbacks != null)
            {
                OnStartFeedbacks.StopFeedbacks();
            }
        }

        protected override void PlayAbilityStopFeedbacks()
        {
            base.PlayAbilityStopFeedbacks();

            if (OnStopFeedbacks != null)
            {
                OnStopFeedbacks.PlayFeedbacks(transform.position, 0);
            }
        }

        protected virtual void OnRevive()
        {
            Initialization();
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (Vital != null)
            {
                Vital.Health.RegisterOnReviveEvent(OnRevive);
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            if (Vital != null)
            {
                Vital.Health.UnregisterOnReviveEvent(OnRevive);
            }
        }

        //
    }
}