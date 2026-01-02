using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.TextCore.Text;

namespace TeamSuneat
{
    /// <summary>
    /// 캐릭터 비행
    /// </summary>
    public class CharacterFly : CharacterAbility
    {
        public override Types Type => Types.Fly;

        // 상수 정의
        private const float MOVEMENT_THRESHOLD = 0.1f;
        private const float DEFAULT_MOVEMENT_SPEED_MULTIPLIER = 1f;

        [FoldoutGroup("Spawn")]
        [Tooltip("캐릭터가 스폰 시 바닥에서 부터 일정거리를 떨어트릴지 여부")]
        public bool SpawnToAwayFormGround = false;

        [FoldoutGroup("Spawn")]
        [Tooltip("캐릭터가 스폰 시 바닥에서 부터 거리")]
        public float SpawnToAwayFormGroundDistance = 0;

        [InfoBox("이 구성 요소를 사용하면 캐릭터가 x 및 y 축 모두에서 중력 없이 이동하여 비행할 수 있습니다. " +
            "여기에서 비행 속도와 캐릭터가 항상 비행하는지 여부를 정의할 수 있습니다(이 경우 키를 누를 필요가 없습니다. 버튼). " +
            "중요 참고 사항: 현재 슬로프 천장은 지원되지 않습니다.")]
        [Tooltip("캐릭터가 날아야 하는 속도")]
        public float FlySpeed = 6f;

        /// <summary>
        /// 비행 속도를 높이거나 낮추기 위해 타겟팅할 수 있는 승수
        /// </summary>
        public float MovementSpeedMultiplier { get; private set; }

        [Tooltip("캐릭터가 항상 비행 중인지 여부, 이 경우 중력에 면역이 됩니다.")]
        public bool AlwaysFlying = false;

        [Tooltip("캐릭터가 사망 시 비행을 중지해야 하는지 여부")]
        public bool StopFlyingOnDeath = false;

        private float _horizontalMovement;
        private float _verticalMovement;
        private bool _flying;

        // animation parameters

        private const string _flyingAnimationParameterName = "Flying";
        private const string _flySpeedAnimationParameterName = "FlySpeed";

        private float _initialMovementSpeedMultiplier;
        private int _flyingAnimationParameter;
        private int _flySpeedAnimationParameter;

        public bool FlyForbidden { get; set; }

        public override void Initialization()
        {
            base.Initialization();

            MovementSpeedMultiplier = DEFAULT_MOVEMENT_SPEED_MULTIPLIER;

            Owner.IsFlying = true;

            _initialMovementSpeedMultiplier = MovementSpeedMultiplier;

            if (AlwaysFlying)
            {
                StartFlight();
            }

            if (SpawnToAwayFormGround)
            {
                Vector3 spawnPoint = Owner.GroundPosition + new Vector3(0, SpawnToAwayFormGroundDistance);

                Owner.transform.position = spawnPoint;
            }
        }

        public void SetMovementSpeedMultiplier(float movementSpeedMultiplier)
        {
            MovementSpeedMultiplier = movementSpeedMultiplier;
        }

        public void ResetMovementSpeedMultiplier()
        {
            MovementSpeedMultiplier = _initialMovementSpeedMultiplier;
        }

        public void SetHorizontalMove(float value)
        {
            _horizontalMovement = value;
        }

        public void SetVerticalMove(float value)
        {
            _verticalMovement = value;
        }

        public void StartFlight()
        {
            if ((!IsAuthorized)
                || (_movement.CurrentState == MovementStates.Dashing)
                || (_movement.CurrentState == MovementStates.Gripping)
                || (_condition.CurrentState != CharacterConditions.Normal))
            {
                return;
            }

            if (_movement.CurrentState != MovementStates.Flying)
            {
                PlayAbilityStartFeedbacks();
                _flying = true;
            }

            Owner.ChangeMovementState(MovementStates.Flying);

            MovementSpeedMultiplier = DEFAULT_MOVEMENT_SPEED_MULTIPLIER;

            Controller.GravityActive(false);
        }

        public void StopFlight()
        {
            if (_movement.CurrentState == MovementStates.Flying)
            {
                StopStartFeedbacks();
                PlayAbilityStopFeedbacks();
            }

            Controller.GravityActive(true);
            _flying = false;
            _movement.RestorePreviousState();
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (StopFlyingOnDeath && (Owner.ConditionState.CurrentState == CharacterConditions.Dead))
            {
                return;
            }

            if (AlwaysFlying)
            {
                if (_movement.CurrentState != MovementStates.Flying)
                {
                    Owner.ChangeMovementState(MovementStates.Flying);
                }
                _flying = true;
            }

            if (_flying)
            {
                Controller.GravityActive(false);
            }

            HandleMovement();

            if (_movement.CurrentState != MovementStates.Flying && _startFeedbackIsPlaying)
            {
                StopStartFeedbacks();
            }

            if (_movement.CurrentState != MovementStates.Flying && _flying)
            {
                StopFlight();
            }

            if (Controller.State.IsCollidingAbove && (_movement.CurrentState != MovementStates.Flying))
            {
                Controller.SetVerticalForce(0);
            }
        }

        private void HandleMovement()
        {
            HandleMovementFeedback();
            
            if (!CanMove())
            {
                return;
            }

            HandleMovementInput();
            FlipOnMovement();
            ApplyMovementForces();
        }

        /// <summary>
        /// 이동 관련 피드백을 처리합니다.
        /// </summary>
        private void HandleMovementFeedback()
        {
            if (_movement.CurrentState != MovementStates.Flying && _startFeedbackIsPlaying)
            {
                StopStartFeedbacks();
            }
        }

        /// <summary>
        /// 이동이 가능한지 확인합니다.
        /// </summary>
        /// <returns>이동 가능 여부</returns>
        private bool CanMove()
        {
            return IsAuthorized
                && (_condition.CurrentState == CharacterConditions.Normal)
                && (_movement.CurrentState != MovementStates.Gripping);
        }

        /// <summary>
        /// 이동 입력을 처리합니다.
        /// </summary>
        private void HandleMovementInput()
        {
            if (FlyForbidden)
            {
                _horizontalMovement = 0f;
                _verticalMovement = 0f;
            }
        }

        /// <summary>
        /// 계산된 이동 힘을 컨트롤러에 적용합니다.
        /// </summary>
        private void ApplyMovementForces()
        {
            if (!FlyForbidden && _flying)
            {
                // 컨트롤러에 적용해야 하는 수평 힘을 전달합니다.
                float horizontalMovementSpeed = _horizontalMovement * FlySpeed * Controller.Parameters.SpeedFactor * MovementSpeedMultiplier;
                float verticalMovementSpeed = _verticalMovement * FlySpeed * Controller.Parameters.SpeedFactor * MovementSpeedMultiplier;

                // 새로 계산된 속도를 컨트롤러에 설정합니다.
                Controller.SetHorizontalForce(horizontalMovementSpeed);
                Controller.SetVerticalForce(verticalMovementSpeed);
            }
        }

        private void FlipOnMovement()
        {
            if (Owner.FlipModelOnTarget && Owner.TargetCharacter != null)
            {
                if (position.x < Owner.TargetCharacter.position.x)
                {
                    if (!Owner.IsFacingRight)
                    {
                        Owner.TryFlip();
                    }
                }
                else if (position.x > Owner.TargetCharacter.position.x)
                {
                    if (Owner.IsFacingRight)
                    {
                        Owner.TryFlip();
                    }
                }
            }
            else
            {
                if (_horizontalMovement > MOVEMENT_THRESHOLD)
                {
                    if (!Owner.IsFacingRight)
                    {
                        Owner.TryFlip();
                    }
                }
                else if (_horizontalMovement < -MOVEMENT_THRESHOLD)
                {
                    if (Owner.IsFacingRight)
                    {
                        Owner.TryFlip();
                    }
                }
            }
        }

        /// <summary>
        /// 캐릭터가 부활할 때 다시 초기화합니다.
        /// </summary>
        private void OnRevive()
        {
            Initialization();
        }

        /// <summary>
        /// 필요한 경우 캐릭터가 죽을 때 비행을 멈춥니다.
        /// </summary>
        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            if (StopFlyingOnDeath)
            {
                StopFlight();
            }
        }

        /// <summary>
        /// 플레이어가 부활하면 이 에이전트를 복원합니다.
        /// </summary>
        /// <param name="checkpoint">Checkpoint.</param>
        /// <param name="player">Player.</param>

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

        protected override void InitializeAnimatorParameters()
        {
            Animator?.AddAnimatorParameterIfExists(_flyingAnimationParameterName, out _flyingAnimationParameter, AnimatorControllerParameterType.Bool, Owner.AnimatorParameters);
            Animator?.AddAnimatorParameterIfExists(_flySpeedAnimationParameterName, out _flySpeedAnimationParameter, AnimatorControllerParameterType.Float, Owner.AnimatorParameters);
        }

        public override void UpdateAnimator()
        {
            Animator?.UpdateAnimatorBool(_flyingAnimationParameter, _movement.CurrentState == MovementStates.Flying, Owner.AnimatorParameters, Owner.PerformAnimatorSanityChecks);
            Animator?.UpdateAnimatorFloat(_flySpeedAnimationParameter, Mathf.Abs(Controller.Speed.magnitude), Owner.AnimatorParameters, Owner.PerformAnimatorSanityChecks);
        }

        public override void ResetAbility()
        {
            base.ResetAbility();

            StopFlight();

            Animator?.UpdateAnimatorBool(_flyingAnimationParameter, false, Owner.AnimatorParameters, Owner.PerformAnimatorSanityChecks);
            Animator?.UpdateAnimatorFloat(_flySpeedAnimationParameter, 0f, Owner.AnimatorParameters, Owner.PerformAnimatorSanityChecks);
        }
    }
}