using JF_SpriteTrail;
using Sirenix.OdinInspector;
using System.Collections;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class CharacterForceVelocity : CharacterAbility
    {
        [FoldoutGroup("#ForceVelocity")] public SpriteTrail[] SpriteTrails;
        [FoldoutGroup("#ForceVelocity")] public FVNames[] GuideFVs;
        [FoldoutGroup("#ForceVelocity")] public string[] GuideFVStrings;

        [FoldoutGroup("#ForceVelocity")] public CharacterHorizontalMovement HorizontalMovementAbility;
        [FoldoutGroup("#ForceVelocity")] public CharacterFly FlyAbility;
        [FoldoutGroup("#ForceVelocity")] public CharacterGrab _grabAbility;
        private ProjectileGuideLine _guideLine;

        private ForceVelocityAsset _asset;
        private Vector2 _appliedForce;
        private Vector3 _initialForce;

        private float _forceDelayTime;
        private float _forceDuration;
        private float _forceElapsedTime;
        private object _forceVelocitySource;
        private Coroutine _forceCoroutine;

        public override Types Type => Types.ForceVelocity;

        public override CharacterConditions[] BlockingConditionStates => new CharacterConditions[]
        {
            CharacterConditions.Dead,
        };

        /// <summary> 시전자가 자기 자신일 때 금지되는 조건 상태 </summary>
        public CharacterConditions[] BlockingConditionStates2 => new CharacterConditions[]
        {
            CharacterConditions.Frozen,
            CharacterConditions.Stunned,
            CharacterConditions.Grabbed,
            CharacterConditions.Snared
        };

        public FVNames CurrentForceVelocityName
        {
            get
            {
                if (_asset.IsValid())
                {
                    return _asset.Data.Name;
                }

                return FVNames.None;
            }
        }

        public float ForceVelocityLastTime
        {
            get
            {
                if (IsProcessing)
                {
                    return Mathf.Max(0, _forceDuration - _forceElapsedTime);
                }

                return 0;
            }
        }

        public bool IsProcessing => _asset.IsValid();

        private const string ANIMATOR_FORCE_SPEED_X_PARAMETER_NAME = "ForceSpeedX";
        private const string ANIMATOR_FORCE_SPEED_Y_PARAMETER_NAME = "ForceSpeedY";
        private int _forceSpeedXAnimationParameter;
        private int _forceSpeedYAnimationParameter;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            SpriteTrails = this.FindComponentsInChildren<SpriteTrail>("Model/#ForceVelocity");
            CharacterHorizontalMovement HorizontalMovement = GetComponent<CharacterHorizontalMovement>();
            CharacterFly Fly = GetComponent<CharacterFly>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            GuideFVStrings = GuideFVs.ToStringArray();
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref GuideFVs, GuideFVStrings);
        }

        //----------------------------------------------------------------------------------------------------

        private void Awake()
        {
            _grabAbility = Owner.FindAbility<CharacterGrab>();
            _guideLine = GetComponentInChildren<ProjectileGuideLine>();
        }

        public override void Initialization()
        {
            base.Initialization();

            LogInfo("캐릭터의 FV 능력을 초기화합니다.");
        }

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            StopForceVelocity();
        }

        public override void ResetAbility()
        {
            base.ResetAbility();

            StopForceVelocity();
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary> 새로운 이동의 적용을 결정합니다. </summary>
        private bool DetermineForceVelocity(ForceVelocityAsset asset)
        {
            if (!asset.IsValid())
            {
                LogWarning("새로운 강제 이동의 데이터가 유효하지 않습니다.");
                return false;
            }

            LogInfo($"새로운 강제 이동({asset.Name})의 적용을 검사합니다.");

            if (!IsAuthorized)
            {
                LogWarning("강제이동을 실행할 수 없는 상태입니다. 새로운 강제 이동을 설정할 수 없습니다.");
                return false;
            }

            if (Controller == null)
            {
                LogWarning("Physics Controller가 설정되어있지 않습니다. 새로운 강제이동을 무시합니다.");
                return false;
            }

            if (asset.Data.Target == FVTargets.Owner)
            {
                if (!CheckBlockingConditionStates(BlockingConditionStates2))
                {
                    return false;
                }
            }

            if (_asset.IsValid())
            {
                if (ComparePriority(asset.Data.Priority))
                {
                    LogInfo("새로운 강제 이동의 우선순위가 같거나 높아 기존 강제이동을 정지합니다. {0}({1}) < {2}({3})",
                        _asset.Data.Name, _asset.Data.Priority, asset.Name, asset.Data.Priority);

                    StopForceVelocity();
                }
                else
                {
                    LogWarning("새로운 강제 이동의 우선순위가 낮아 무시합니다. {0}({1}) > {2}({3})",
                            _asset.Data.Name, _asset.Data.Priority, asset.Name, asset.Data.Priority);

                    return false;
                }
            }
            else if (_forceCoroutine != null)
            {
                LogWarning("설정한 강제이동은 없으나 강제이동을 실행중입니다. 강제이동을 무시합니다.");
                return false;
            }

            return true;
        }

        /// <summary> 새로운 이동과 현재 이동의 우선순위를 비교합니다. </summary>
        public bool ComparePriority(int priority)
        {
            if (_asset.IsValid() && _asset.Data.IsValid())
            {
                if (_asset.Data.Priority > priority)
                {
                    return false;
                }
            }

            return true;
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary> 새로운 강제 이동을 시작합니다. </summary>
        /// <param name="source"> 강제 이동 시전 오브젝트 </param>
        public void StartForceVelocity(ForceVelocityAsset asset, bool isDamageRight, object source)
        {
            if (!DetermineForceVelocity(asset))
            {
                return;
            }

            _asset = asset;
            _forceDelayTime = asset.Data.ForceDelayTime;
            _forceDuration = asset.Data.ForceDuration;
            _forceVelocitySource = source;

            Owner.SetGizmoFVString($"FV:{CurrentForceVelocityName}, IsBlockRaycast:{Controller.IsBlockRaycast}");
            LogInfo("캐릭터의 FV 능력을 시작합니다. {0}", asset.Name);

            if (_asset.Data.IgnoreFlipWhileForce)
            {
                Owner.LockFlip();
            }
            if (_asset.Data.IgnoreMovementByInputWhileForce)
            {
                LockMovement();
            }
            if (_asset.Data.InvincibleWhileFV)
            {
                Vital.Health.SetTemporarilyInvulnerable(this);
            }
            if (_asset.Data.IsBlockRaycastWhileMove)
            {
                Controller.BlockRaycast();
            }

            FlipFV(isDamageRight);
            ActivateSpriteTrail(asset.Name);

            _forceCoroutine = StartXCoroutine(ProcessForceVelocity());
        }

        public void StartForceVelocity(ForceVelocityAsset asset, bool isDamageRight, float forceDuration, object source)
        {
            if (!DetermineForceVelocity(asset))
            {
                return;
            }

            _asset = asset;
            _forceDuration = forceDuration;
            _forceVelocitySource = source;

            Owner.SetGizmoFVString($"FV:{CurrentForceVelocityName}, IsBlockRaycast:{Controller.IsBlockRaycast}");
            LogInfo("캐릭터의 FV 능력을 시작합니다. {0}", asset.Name);

            if (_asset.Data.IgnoreFlipWhileForce)
            {
                Owner.LockFlip();
            }
            if (_asset.Data.IgnoreMovementByInputWhileForce)
            {
                LockMovement();
            }
            if (_asset.Data.InvincibleWhileFV)
            {
                Vital.Health.SetTemporarilyInvulnerable(this);
            }
            if (_asset.Data.IsBlockRaycastWhileMove)
            {
                Controller.BlockRaycast();
            }

            FlipFV(isDamageRight);
            ActivateSpriteTrail(asset.Name);

            _forceCoroutine = StartXCoroutine(ProcessForceVelocity());
        }

        /// <summary> 진행 중인 강제 이동을 정지합니다. </summary>
        public void StopForceVelocity()
        {
            if (_asset.IsValid())
            {
                LogInfo("캐릭터의 FV 능력을 정지합니다. {0}", _asset.Data.Name);

                if (_asset.Data.IgnoreFlipWhileForce)
                {
                    Owner.UnlockFlip();
                }
                if (_asset.Data.IgnoreMovementByInputWhileForce)
                {
                    UnlockMovement();
                }
                if (_asset.Data.InvincibleWhileFV)
                {
                    Vital.Health.ResetTemporarilyInvulnerable(this);
                }
                if (_asset.Data.IsResetVelocityOnStop)
                {
                    Controller.ResetForceExtern();
                }
                if (_asset.Data.IsStopGrabOnStop)
                {
                    if (_grabAbility != null)
                    {
                        _grabAbility.ForceStopGrab();
                    }
                }
                if (_asset.Data.IsBlockRaycastWhileMove)
                {
                    Controller.UnblockRaycast();
                }

                if (_asset.Data.VFXOnStop != null)
                {
                    VFXManager.Spawn(_asset.Data.VFXOnStop, Owner);
                }

                DeactivateSpriteTrail(_asset.Name);

                _asset = null;
                Owner.SetGizmoFVString(null);
            }

            _appliedForce = Vector2.zero;

            StopXCoroutine(ref _forceCoroutine);
        }

        /// <summary> 시전 오브젝트에 해당하는 강제 이동을 정지합니다. </summary>
        public void StopForceVelocity(object source)
        {
            if (_forceVelocitySource != default)
            {
                if (_forceVelocitySource != source)
                {
                    return;
                }
            }

            StopForceVelocity();
        }

        /// <summary> 진행 중인 강제 이동과 같다면 정지합니다. </summary>
        public void StopForceVelocity(FVNames fvName)
        {
            if (_asset.IsValid())
            {
                if (_asset.Name == fvName)
                {
                    StopForceVelocity();
                }
            }
        }

        public void StopForceVelocityByStateOrDamage()
        {
            if (_asset.IsValid())
            {
                if (_asset.Data.Target == FVTargets.Target)
                {
                    switch (_asset.Name)
                    {
                        case FVNames.PlayerDamage:
                        case FVNames.PlayerPowerfulDamage:
                            {
                                StopForceVelocity();
                            }
                            break;

                        default:
                            {
                                LogProgress("상태이상에 의해 캐릭터의 피격 FV 능력을 정지할 수 없습니다: {0}", _asset.Data.Name);
                                return;
                            }
                    }
                }
            }

            StopForceVelocity();
        }

        // 이동 (Movement)

        private IEnumerator ProcessForceVelocity()
        {
            float gravity = 0;
            _forceElapsedTime = 0;

            if (_asset.Data.IsStopOnStart)
            {
                Controller.ResetForceExtern();
            }

            Vector3 destination = GetDestination();
            RefreshFace(_asset.Data.FaceOnStart, destination);
            SpawnGuideFV(_asset.Data.Name, destination); // Face를 통해 반전된 후 가이드를 생성합니다.

            if (_forceDelayTime > 0)
            {
                yield return new WaitForSeconds(_forceDelayTime);
            }

            Vector3 startPosition = position;

            while (_forceDuration > _forceElapsedTime)
            {
                if (!IsAuthorized)
                {
                    break;
                }

                _forceElapsedTime += Time.fixedDeltaTime;
                float timeRate = _forceElapsedTime.SafeDivide01(_forceDuration);
                switch (_asset.Data.Motion)
                {
                    case FVMotions.Physics:
                        {
                            MovementPhysics(gravity, timeRate);
                            if (_asset.Data.ApplyGravity)
                            {
                                if (Owner.Controller.State.IsGrounded)
                                {
                                    // 캐릭터가 땅에 부딪혔다면 중력을 초기화합니다.
                                    gravity = 0;
                                }
                                else if (!_asset.Data.FlyingCharacterIgnoreGravity || !Owner.IsFlying)
                                {
                                    gravity += Controller.CurrentGravity * _asset.Data.GravityMultiplier * Time.fixedDeltaTime;
                                }
                                else
                                {
                                    // 날아다니는 몬스터는 강제 이동(FV)의 중력 적용을 무시합니다.
                                }
                            }
                        }
                        break;

                    case FVMotions.Linear:
                        {
                            MovementLinear(destination, timeRate);
                        }
                        break;

                    case FVMotions.Jump:
                        {
                            MovementJump(startPosition, destination, timeRate);
                        }
                        break;
                }

                RefreshFace(_asset.Data.FaceWhileMove, destination);

                if (TryExitForceVelocity(destination))
                {
                    break;
                }

                yield return new WaitForFixedUpdate();
            }

            RefreshFace(_asset.Data.FaceOnEnd, destination);

            if (_asset.Data.IsStopAtEndOfDuration)
            {
                StopForceVelocity();
            }
        }

        private void MovementLinear(Vector3 destination, float timeRate)
        {
            Vector3 directionToTarget = (destination - position).normalized;
            _appliedForce = directionToTarget * _asset.Data.TargetToForce * _asset.Data.ForceCurve.Evaluate(timeRate);

            Controller.SetForce(_appliedForce);
        }

        private void MovementPhysics(float gravity, float timeRate)
        {
            // 강제 이동 중 사용자의 수평 입력에 따라 이동 방향을 전환
            if (_asset.Data.ApplyDirectionXByHorizontalInput)
            {
                if (TSInputManager.Instance.PrimaryMovement.x < 0 && _initialForce.x > 0)
                {
                    _initialForce = _initialForce.FlipX();
                }
                else if (TSInputManager.Instance.PrimaryMovement.x > 0 && _initialForce.x < 0)
                {
                    _initialForce = _initialForce.FlipX();
                }
            }

            // 시간에 따른 힘의 변화 계산
            _appliedForce = _initialForce * _asset.Data.ForceCurve.Evaluate(timeRate);

            // 왼쪽 충돌 시 X 방향 힘을 0으로 설정, 그러나 오른쪽 입력이 있다면 _initialForce 적용
            if (Owner.Controller.State.IsCollidingLeft && _appliedForce.x < 0)
            {
                _appliedForce = _appliedForce.ResetX();
                if (_asset.Data.ApplyDirectionXByHorizontalInput && TSInputManager.Instance.PrimaryMovement.x > 0)
                {
                    _appliedForce = _initialForce;
                }
            }
            // 오른쪽 충돌 시 X 방향 힘을 0으로 설정, 그러나 왼쪽 입력이 있다면 _initialForce 적용
            else if (Owner.Controller.State.IsCollidingRight && _appliedForce.x > 0)
            {
                _appliedForce = _appliedForce.ResetX();
                if (_asset.Data.ApplyDirectionXByHorizontalInput && TSInputManager.Instance.PrimaryMovement.x < 0)
                {
                    _appliedForce = _initialForce;
                }
            }

            // 아래쪽 충돌 시 Y 방향 힘을 0으로 설정
            if (Owner.Controller.State.IsCollidingBelow && _appliedForce.y < 0)
            {
                _appliedForce = _appliedForce.ResetY();
            }
            else
            {
                // 중력 적용 및 Y 방향 힘 계산
                float forceY = Mathf.Clamp(_appliedForce.y + gravity, -Controller.Parameters.MaxVelocity.y, Controller.Parameters.MaxVelocity.y);
                _appliedForce = _appliedForce.ApplyY(forceY);
            }

            // 최종 계산된 힘을 컨트롤러에 적용
            Controller.SetForce(_appliedForce);
        }

        private void MovementJump(Vector3 startPosition, Vector3 destination, float timeRate)
        {
            float jumpOffset = Mathf.Sin(timeRate * Mathf.PI) * _asset.Data.JumpHeight;
            float currentX = Mathf.Lerp(startPosition.x, destination.x, timeRate);
            float currentY = startPosition.y + jumpOffset;

            position = new Vector2(currentX, currentY);
        }

        private void SpawnGuideFV(FVNames fvName, Vector3 destination)
        {
            if (GuideFVs != null)
            {
                for (int i = 0; i < GuideFVs.Length; i++)
                {
                    FVNames item = GuideFVs[i];
                    if (item == fvName)
                    {
                        _guideLine.SetDirection(destination - position);
                        _guideLine.Show();
                    }
                }
            }
        }

        private void RefreshFace(FaceTypes faceType, Vector3 destination)
        {
            if (Owner == null)
            {
                LogWarning("Owner가 null입니다.");
                return;
            }

            switch (faceType)
            {
                case FaceTypes.FaceTarget:
                    {
                        Owner.FaceToTarget();
                    }
                    break;

                case FaceTypes.FaceDestination:
                    {
                        Owner.ForceFace(destination);
                    }
                    break;

                case FaceTypes.UnfaceDestination:
                    {
                        Owner.ForceFace(destination.FlipX());
                    }
                    break;

                case FaceTypes.InputHorizontal:
                    {
                        if (TSInputManager.Instance.PrimaryMovement.x < 0 && Owner.IsFacingRight)
                        {
                            Owner.ForceFlip();
                        }
                        else if (TSInputManager.Instance.PrimaryMovement.x > 0 && !Owner.IsFacingRight)
                        {
                            Owner.ForceFlip();
                        }
                    }
                    break;
            }
        }

        private bool TryExitForceVelocity(Vector3 destination)
        {
            if (_asset.Data.IsStopWhenForceXIsZero)
            {
                if (_appliedForce.x.IsZero())
                {
                    return true;
                }
            }
            if (_asset.Data.IsStopWhenForceYIsZero)
            {
                if (_appliedForce.y.IsZero())
                {
                    return true;
                }
            }
            if (_asset.Data.IsStopWhenGrounded)
            {
                if (Owner.Controller.State.JustGotGrounded)
                {
                    return true;
                }
            }

            if (_asset.Data.IsAvoidFalling)
            {
                if (Owner.CheckForHoles(_initialForce.x > 0))
                {
                    return true;
                }
            }

            if (_asset.Data.IsStopOnArrive)
            {
                if (!destination.IsZero())
                {
                    float betweenDistance = Vector2.Distance(destination, position);
                    if (betweenDistance < _asset.Data.ArriveDistance)
                    {
                        return true;
                    }
                }
            }
            else if (_asset.Data.IsStopOnArriveX)
            {
                if (!destination.IsZero())
                {
                    float betweenDistance = destination.x.GetDifference(position.x);
                    if (betweenDistance < _asset.Data.ArriveDistance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // 이동 중 값을 설정 또는 반환합니다.

        /// <summary> 피해 방향에 따라 이동 힘의 X를 반전합니다. </summary>
        private void FlipFV(bool isDamageRight)
        {
            if (_asset.Data.Motion != FVMotions.Physics)
            {
                return;
            }

            Vector2 force = CalculateInitialForce();
            _initialForce = isDamageRight ? force : force.FlipX();
        }

        /// <summary> 초기 힘을 계산합니다. </summary>
        private Vector2 CalculateInitialForce()
        {
            Vector2 force = new Vector2(_asset.Data.ForceX, _asset.Data.ForceY);

            if (_asset.Data.IsApplyForceProportionalToDistanceFromTarget && Owner.Target != null)
            {
                int distance = (int)Vector2.Distance(Owner.position, Owner.Target.position);
                force = new Vector2(force.x * distance, force.y);
            }

            return force;
        }

        /// <summary> 강제 이동의 목적지를 반환합니다. </summary>
        private Vector3 GetDestination()
        {
            Vector2 destination = position;
            Vector3 offset = Vector3.zero;

            switch (_asset.Data.Destination)
            {
                case FVDestinations.Player:
                    {
                        destination = GetDirectionToPlayer(offset);
                    }
                    break;

                case FVDestinations.Target:
                    {
                        destination = GetDirectionOfTarget(offset);
                    }
                    break;

                case FVDestinations.PositionGroup:
                    {
                        destination = GetDirectionOfPositionGroup(offset);
                    }
                    break;

                case FVDestinations.Direction:
                    {
                        destination = GetDirectionDestination();
                    }
                    break;
            }

            if (_asset.Data.IgnoreTargetOfPositionY)
            {
                destination = destination.ApplyY(position.y);
            }

            return destination;
        }

        private Vector3 GetDirectionToPlayer(Vector3 offset)
        {
            if (CharacterManager.Instance != null && CharacterManager.Instance.Player != null)
            {
                Vector3 playerPosition = CharacterManager.Instance.Player.position;
                return playerPosition + _asset.Data.DestinationOffset;
            }
            else
            {
                Log.Warning("{0}, FV에서 Player를 찾을 수 없습니다. {1}", Owner.Name.ToLogString(), _asset.Name.ToLogString());
                return position;
            }
        }

        private Vector3 GetDirectionOfTarget(Vector3 offset)
        {
            if (Owner.Target != null)
            {
                if (!_asset.Data.DestinationOffset.IsZero())
                {
                    if (!Owner.IsFacingRight)
                    {
                        offset = new Vector3(-(_asset.Data.DestinationOffset.x), _asset.Data.DestinationOffset.y, _asset.Data.DestinationOffset.z);
                    }
                    else
                    {
                        offset = _asset.Data.DestinationOffset;
                    }
                }

                return Owner.Target.position + offset;
            }
            else
            {
                Log.Warning("{0}, FV의 Target을 찾을 수 없습니다: {1}", Owner.Name.ToLogString(), _asset.Name.ToLogString());
                return position;
            }
        }

        private Vector3 GetDirectionOfPositionGroup(Vector3 offset)
        {
            PositionGroup positionGroup = PositionGroupManager.Instance.Find(_asset.Name);
            if (positionGroup != null)
            {
                if (!_asset.Data.DestinationOffset.IsZero())
                {
                    if (!Owner.IsFacingRight)
                    {
                        offset = new Vector3(-_asset.Data.DestinationOffset.x, _asset.Data.DestinationOffset.y, _asset.Data.DestinationOffset.z);
                    }
                    else
                    {
                        offset = _asset.Data.DestinationOffset;
                    }
                }

                return positionGroup.GetPosition(Owner.transform.position) + offset;
            }
            else
            {
                Log.Error("FV의 PositionGroup을 찾을 수 없습니다. {0}", _asset.Name.ToLogString());
                return position;
            }
        }

        private Vector3 GetDirectionDestination()
        {
            if (_asset.Data.Direction == FVDirections.AttackerFace)
            {
                if (Owner.IsFacingRight)
                {
                    return position + (_asset.Data.TargetToForce * Vector3.right);
                }
                else
                {
                    return position + (_asset.Data.TargetToForce * Vector3.left);
                }
            }
            else if (_asset.Data.Direction == FVDirections.AttackerFaceReverse)
            {
                if (Owner.IsFacingRight)
                {
                    return position + (_asset.Data.TargetToForce * Vector3.left);
                }
                else
                {
                    return position + (_asset.Data.TargetToForce * Vector3.right);
                }
            }
            else if (_asset.Data.Direction == FVDirections.Target)
            {
                if (Owner.Target != null)
                {
                    Vector3 direction = Owner.Target.position - position;
                    return position + (direction.normalized * _asset.Data.TargetToForce);
                }
            }

            return position;
        }

        // 입력을 통한 이동을 잠금/해금합니다.

        private void LockMovement()
        {
            if (Owner.HorizontalMovement != null)
            {
                Owner.HorizontalMovement.LockMovement(_asset.Data.Name);
            }

            if (Owner.Fly != null)
            {
                Owner.Fly.FlyForbidden = true;
            }
        }

        private void UnlockMovement()
        {
            if (Owner.HorizontalMovement != null)
            {
                Owner.HorizontalMovement.UnlockMovement(_asset.Data.Name);
            }

            if (Owner.Fly != null)
            {
                Owner.Fly.FlyForbidden = false;
            }
        }

        // 강제이동 중 트레일을 활성화/비활성화합니다.

        private void ActivateSpriteTrail(FVNames fvName)
        {
            if (SpriteTrails.IsValid())
            {
                for (int i = 0; i < SpriteTrails.Length; i++)
                {
                    if (SpriteTrails[i].m_TrailName == fvName.ToString())
                    {
                        SpriteTrails[i].enabled = true;
                        break;
                    }
                }
            }
        }

        private void DeactivateSpriteTrail(FVNames fvName)
        {
            if (SpriteTrails.IsValid())
            {
                for (int i = 0; i < SpriteTrails.Length; i++)
                {
                    if (SpriteTrails[i].m_TrailName == fvName.ToString())
                    {
                        SpriteTrails[i].enabled = false;
                        break;
                    }
                }
            }
        }

        // Animator

        protected override void InitializeAnimatorParameters()
        {
            Animator?.AddAnimatorParameterIfExists(ANIMATOR_FORCE_SPEED_X_PARAMETER_NAME, out _forceSpeedXAnimationParameter, AnimatorControllerParameterType.Float, Owner.AnimatorParameters);
            Animator?.AddAnimatorParameterIfExists(ANIMATOR_FORCE_SPEED_Y_PARAMETER_NAME, out _forceSpeedYAnimationParameter, AnimatorControllerParameterType.Float, Owner.AnimatorParameters);
        }

        public override void UpdateAnimator()
        {
            Animator?.UpdateAnimatorFloat(_forceSpeedXAnimationParameter, Mathf.Abs(_appliedForce.x), Owner.AnimatorParameters);
            Animator?.UpdateAnimatorFloat(_forceSpeedYAnimationParameter, Mathf.Abs(_appliedForce.y), Owner.AnimatorParameters);
        }

        // LOG

        protected override void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.ForceVelocity, string.Format("{0}, {1}", Owner.Name.ToLogString(), content));
            }
        }

        protected override void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                LogProgress(string.Format(format, args));
            }
        }

        protected override void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.ForceVelocity, string.Format("{0}, {1}", Owner.Name.ToLogString(), content));
            }
        }

        protected override void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                LogInfo(string.Format(format, args));
            }
        }

        protected override void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.ForceVelocity, string.Format("{0}, {1}", Owner.Name.ToLogString(), content));
            }
        }

        protected override void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                LogWarning(string.Format(format, args));
            }
        }
    }
}