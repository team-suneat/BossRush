using Lean.Pool;
using System.Collections;
using TeamSuneat.XProjectile;
using UnityEngine;

namespace TeamSuneat
{
    // Motions

    public partial class Projectile
    {
        public Vital HomingTarget => _homingTarget;

        public void Translate(Vector2 direction, float moveSpeed)
        {
            if (transform != null)
            {
                transform.Translate(direction * moveSpeed * Time.fixedDeltaTime);
            }
        }

        public void SetMotionType(ProjectileMotionTypes motionType)
        {
            _motionType = motionType;
        }

        public void SetLinearMotion(Vector3 newDirection)
        {
            ProjectileMoverLinear mover = new ProjectileMoverLinear();

            mover.SetProjectile(this);
            mover.SetOwner(Owner);
            mover.SetLevel(Level);
            mover.SetDirection(newDirection);
            mover.Setup();

            this._mover = mover;
        }

        public void SetLinearToTargetMotion(Vector3 newDirection)
        {
            ProjectileMoverLinearToTarget mover = new ProjectileMoverLinearToTarget();

            mover.SetProjectile(this);
            mover.SetOwner(Owner);
            mover.SetLevel(Level);
            mover.SetDirection(newDirection);
            mover.Setup();

            this._mover = mover;
        }

        #region LinearCardinalToTarget

        public void SetLinearCardinalToTargetMotion(Vector3 newDirection)
        {
            SetEnabledCollider(true);
            SetTriggerCollider(true);

            ResetRigidBody();

            _targetDirection = VectorEx.Normalize(newDirection);

            RefreshRotationParent();
        }

        #endregion LinearCardinalToTarget

        #region DetectLinearToTarget

        public void SetDetectLinearToTargetMotion()
        {
            SetEnabledCollider(true);
            SetTriggerCollider(true);

            ResetRigidBody();

            DetectLinearTarget();

            if (HomingTarget == null)
            {
                Destroy();
            }

            RefreshRotationParent();
        }

        private void DetectLinearTarget()
        {
            if (_projectileData.DetectAreaType != DetectAreaTypes.Circle)
            {
                return;
            }

            Vital[] detectedVitals = VitalManager.Instance.FindInArea(position, _projectileData.DetectDistance, m_targetLayer);

            float minDistanceToTarget = float.MaxValue;

            for (int i = 0; i < detectedVitals.Length; i++)
            {
                if (detectedVitals[i] == null)
                {
                    continue;
                }

                float distanceToTarget = Vector2.Distance(position, detectedVitals[i].position);

                if (distanceToTarget > minDistanceToTarget)
                {
                    continue;
                }

                if (distanceToTarget < 1f)
                {
                    continue;
                }

                if (false == TryDamageToTarget(detectedVitals[i]))
                {
                    continue;
                }

                minDistanceToTarget = distanceToTarget;

                SetHomingTarget(detectedVitals[i]);

                _targetDirection = (detectedVitals[i].position - position).normalized;

                SetEnabledCollider(true);
            }
        }

        #endregion DetectLinearToTarget

        #region Physics

        public void SetPhysicsMotion(bool facingRight)
        {
            if (Mathf.Approximately(0f, _projectileData.DelayTime))
            {
                CreatePhysicsMover(facingRight);
            }
            else
            {
                StartXCoroutine(WaitSetPhysicsMotion(facingRight));
            }
        }

        private IEnumerator WaitSetPhysicsMotion(bool facingRight)
        {
            SetEnabledCollider(false);

            ResetRigidBody();

            yield return new WaitForSeconds(_projectileData.DelayTime);

            CreatePhysicsMover(facingRight);
        }

        private void CreatePhysicsMover(bool facingRight)
        {
            ProjectileMoverPhysics moverPhysics = new ProjectileMoverPhysics();

            moverPhysics.SetProjectile(this);
            moverPhysics.SetFacingRight(facingRight);
            moverPhysics.Setup();

            _mover = moverPhysics;
        }

        #endregion Physics

        #region HomingLinear

        public void SetHomingLinearMotion()
        {
            ProjectileMoverHomingLinear moverHomingLinear = new ProjectileMoverHomingLinear();

            moverHomingLinear.SetProjectile(this);
            moverHomingLinear.SetOwner(Owner);
            moverHomingLinear.SetLevel(Level);
            moverHomingLinear.SetTarget(HomingTarget);
            moverHomingLinear.Setup();

            _mover = moverHomingLinear;
        }

        #endregion HomingLinear

        #region Homing

        public void SetHomingMotion()
        {
            SetEnabledCollider(true);
            SetTriggerCollider(true);

            if (Body != null)
            {
                Body.ResetGravity();
                Body.SetBodyType(RigidbodyType2D.Dynamic);
                Body.ResetVelocity();

                if (false == _projectileData.RigidForce.IsZero())
                {
                    Body.AddForce(_projectileData.RigidForce * 100);
                }
            }

            SetHomingRotationSpeed();
            SetHomingDirection();
            RefreshRendererRotation();
            StartXCoroutine(ProcessChangeHomingRotateSpeed());
        }

        private IEnumerator ProcessChangeHomingRotateSpeed()
        {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            float diff = 1;

            if (HomingRotateSpeed > RotateSpeed)
            {
                diff = (HomingRotateSpeed - RotateSpeed) * 0.02f;
            }
            else
            {
                diff = (RotateSpeed - HomingRotateSpeed) * 0.02f;
            }

            while (true)
            {
                if (HomingRotateSpeed > RotateSpeed)
                {
                    HomingRotateSpeed -= diff;

                    if (HomingRotateSpeed <= RotateSpeed)
                    {
                        break;
                    }
                }
                else
                {
                    HomingRotateSpeed += diff;

                    if (HomingRotateSpeed >= RotateSpeed)
                    {
                        break;
                    }
                }

                yield return wait;
            }

            HomingRotateSpeed = RotateSpeed;
        }

        private void SetHomingRotationSpeed()
        {
            HomingRotateSpeed = 2048;

            if (false == Mathf.Approximately(_projectileData.RotateMinSpeed, _projectileData.RotateMaxSpeed))
            {
                RotateSpeed = RandomEx.Range(_projectileData.RotateMinSpeed, _projectileData.RotateMaxSpeed);
            }
            else
            {
                RotateSpeed = _projectileData.RotateMinSpeed;
            }
        }

        private void SetHomingDirection()
        {
            switch (_projectileData.HomingDirection)
            {
                case ProjectileHomingDirections.Up:
                    {
                        HomingDirection = 1;
                    }
                    break;

                case ProjectileHomingDirections.Down:
                    {
                        HomingDirection = -1;
                    }
                    break;

                case ProjectileHomingDirections.Random:
                    {
                        if (RandomEx.GetBoolValue())
                        {
                            HomingDirection = 1;
                        }
                        else
                        {
                            HomingDirection = -1;
                        }
                    }
                    break;

                case ProjectileHomingDirections.PingPong:
                    {
                        if (Mathf.Abs(HomingDirection) != 1)
                        {
                            HomingDirection = 1;
                        }
                        else
                        {
                            HomingDirection *= -1;
                        }
                    }
                    break;
            }
        }

        private void MoveHoming()
        {
            if (HomingTarget == null)
            {
                Log.Warning(LogTags.Projectile, "Failed to move homing projectile. homing target is null. {0}", Name.ToLogString());
                ApplyReturn();
                Destroy();
                return;
            }

            if (HomingTarget.IsAlive)
            {
                Log.Warning(LogTags.Projectile, "Failed to move homing projectile. homing target is died. {0}, {1}", Name.ToLogString(), HomingTarget.GetHierarchyPath());

                ApplyReturn();
                Destroy();
                return;
            }

            RefreshMoveSpeed();

            if (Body != null)
            {
                Vector2 dirToTarget = (Vector2)HomingTarget.position - (Vector2)position;
                dirToTarget = VectorEx.Normalize(dirToTarget);

                float rotateAmout = Vector3.Cross(dirToTarget, transform.up).z;
                Body.SetAngularVelocity(-rotateAmout * HomingRotateSpeed * HomingDirection);
                Body.SetVelocity(transform.up * CurrentMoveSpeed * HomingDirection);
            }
            else
            {
                Log.Error("Projectile의 Rigidbody가 설정되어있지 않습니다. {0}, {1}", Name.ToLogString(), _motionType);
            }

            RefreshRendererRotation();
            RefreshRendererFlip();
        }

        #endregion Homing

        #region DetectHoming

        public void SetDetectHomingMotion()
        {
            ResetRigidBody();

            if (_projectileData.DetectAreaType == DetectAreaTypes.Circle)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(position, _projectileData.DetectDistance, m_targetLayer);
                float minDistanceToTarget = float.MaxValue;

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i] == null)
                    {
                        continue;
                    }

                    if (false == hits[i].CompareTag(GameTags.Vital))
                    {
                        continue;
                    }

                    float distanceToTarget = Vector2.Distance(position, hits[i].transform.position);
                    if (distanceToTarget < minDistanceToTarget && distanceToTarget > 1f)
                    {
                        Vital targetVital = hits[i].GetComponent<Vital>();

                        if (targetVital == null)
                        {
                            continue;
                        }
                        if (targetVital.IsAlive)
                        {
                            continue;
                        }

                        minDistanceToTarget = distanceToTarget;

                        SetHomingTarget(targetVital);

                        SetEnabledCollider(true);
                    }
                }
            }

            if (HomingTarget == null)
            {
                Destroy();
            }
        }

        private void MoveDetectHomingMotion()
        {
            if (HomingTarget != null)
            {
                if (false == HomingTarget.IsAlive)
                {
                    RefreshMoveSpeed();

                    Vector2 forward = (Vector2)HomingTarget.position - (Vector2)position;
                    Vector2 direction = VectorEx.Normalize(forward);

                    transform.Translate(direction * CurrentMoveSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    Log.Exception("(DetectHoming) 유도발사체에 설정된 타겟이 이미 죽었다. Projectile:{0}, target:{1}", this.GetHierarchyPath(), HomingTarget.GetHierarchyPath());
                }
            }
            else
            {
                Log.Exception("(DetectHoming) 유도발사체에 설정된 타겟이 없다. Projectile:{0}", this.GetHierarchyPath());

                DetectLinearTarget();
            }
        }

        #endregion DetectHoming

        #region Raycast

        public void SetRaycastMotion(Vector3 newDirection)
        {
            SetEnabledCollider(false);
            ResetRigidBody();

            _targetDirection = VectorEx.Normalize(newDirection);

            DoAttack();

            SpawnAttackFX(FacingRightAtLaunch);
        }

        #endregion Raycast

        #region On The Spot

        public void SetOnTheSpotMotion()
        {
            ProjectileMoverOnTheSpot mover = new ProjectileMoverOnTheSpot();

            mover.SetProjectile(this);

            mover.Setup();

            this._mover = mover;
        }

        public void SetOnTheGroundMotion()
        {
            SetOnTheSpotMotion();

            Vector3 groundPosition = Vector3.zero;

            if (_projectileData.GroundCheckOnSpawn)
            {
                Vector3 checkPosition = position + (Vector3.down * Owner.DistanceFromGround);

                float checkGroundDistance = _projectileData.GroundCheckDistance;

                if (m_boxCollider2D != null)
                {
                    checkGroundDistance += m_boxCollider2D.size.y * 0.5f;
                }
                else if (m_circleCollider2D != null)
                {
                    checkGroundDistance += m_circleCollider2D.radius;
                }

                groundPosition = GetGroundPosition(checkPosition, checkGroundDistance);
            }

            if (false == groundPosition.IsZero())
            {
                position = groundPosition;
            }
        }

        public void SetOnTheTargetMotion()
        {
            SetOnTheSpotMotion();

            if (HomingTarget != null)
            {
                position = HomingTarget.position;
            }
        }

        public void SetOnTheTargetGroundMotion()
        {
            SetOnTheSpotMotion();

            if (HomingTarget != null)
            {
                Vector3 groundPosition = Vector3.zero;

                if (_projectileData.GroundCheckOnSpawn)
                {
                    if (HomingTarget.Owner != null)
                    {
                        Vector3 startPosition = HomingTarget.position + Vector3.down * HomingTarget.Owner.DistanceFromGround;

                        groundPosition = GetGroundPosition(startPosition, _projectileData.GroundCheckDistance);
                    }
                    else
                    {
                        groundPosition = GetGroundPosition(HomingTarget.position, _projectileData.GroundCheckDistance);
                    }
                }

                if (false == groundPosition.IsZero())
                {
                    position = groundPosition;
                }
            }
            else if (Owner != null)
            {
                if (_projectileData.GroundCheckOnSpawn)
                {
                    Vector3 startPosition = Owner.position + Vector3.down * Owner.DistanceFromGround;

                    position = GetGroundPosition(startPosition, _projectileData.GroundCheckDistance);
                }
            }
        }

        private Vector3 GetGroundPosition(Vector3 startPosition, float distanceToGround)
        {
            RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.down, distanceToGround, GameLayers.Mask.Collision);

            if (hit.collider != null)
            {
                if (hit.point.y >= hit.collider.bounds.max.y)
                {
                    Log.Info(LogTags.Projectile, "발사체 아래 땅을 찾았습니다. Projectile:{0}, Point:{1}, Ground:{2}",
                        Name.ToLogString(), hit.point, hit.collider.GetHierarchyPath());

                    return hit.point;
                }
                else if (hit.collider is BoxCollider2D)
                {
                    Vector2 endPosition = new Vector2(startPosition.x, hit.collider.bounds.max.y);

                    Log.Info(LogTags.Projectile, "발사체가 충돌체 안에 있습니다. 발사체 위치를 충돌체 가장 상단 위치로 설정합니다. Projectile:{0}, Point:{1}, Ground:{2}",
                        Name.ToLogString(), endPosition, hit.collider.GetHierarchyPath());

                    return endPosition;
                }
                else
                {
                    Log.Info(LogTags.Projectile, "발사체 아래 땅을 찾았습니다. Projectile:{0}, Point:{1}, Ground:{2}",
                        Name.ToLogString(), hit.point, hit.collider.GetHierarchyPath());

                    return hit.point;
                }
            }

            if (_projectileData.UseOwnerPointForGroundCheck)
            {
                if (Owner != null)
                {
                    startPosition = Owner.position + Vector3.down * Owner.DistanceFromGround;
                    hit = Physics2D.Raycast(startPosition, Vector2.down, distanceToGround, GameLayers.Mask.Collision);

                    if (hit.collider != null)
                    {
                        Log.Warning(LogTags.Projectile, "발사체 아래 땅을 찾을 수 없습니다. 발사체를 사용한 캐릭터 아래 땅을 찾았습니다. Projectile:{0}, Point:{1}, Ground:{2}",
                            Name.ToLogString(), hit.point, hit.collider.GetHierarchyPath());

                        return hit.point;
                    }
                }
            }

            return Vector3.zero;
        }

        #endregion On The Spot

        #region Return To Owner

        public void SetReturnToOwner()
        {
            SetEnabledCollider(true);

            ResetRigidBody();
        }

        public void StopMoveInDelayTime()
        {
            if (false == Mathf.Approximately(_projectileData.DelayTime, 0))
            {
                StartXCoroutine(ProcessStopMoveInDelayTime(_projectileData.DelayTime));
            }
        }

        private void MoveReturnToOwner()
        {
            if (Owner != null)
            {
                RefreshMoveSpeed();

                Vector2 directionToTarget = Owner.position - position;

                float distance = Vector2.Distance(Owner.position, position);

                transform.Translate(VectorEx.Normalize(directionToTarget) * CurrentMoveSpeed * Time.fixedDeltaTime);

                if (distance < 1)
                {
                    StartAttack();

                    Destroy();
                }
            }
            else
            {
                Destroy();

                Log.Warning(LogTags.Projectile, "{0}, Motion is [Return To Owner]. Owner is null.", this.GetHierarchyPath());
            }
        }

        #endregion Return To Owner
    }
}