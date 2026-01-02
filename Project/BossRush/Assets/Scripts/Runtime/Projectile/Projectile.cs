using Lean.Pool;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class Projectile : XBehaviour, IPoolable
    {
        public bool TryMove()
        {
            if (_isDestroy)
            {
                return false;
            }

            if (_isDespawn)
            {
                return false;
            }

            if (_isStopMoveInDelayTime)
            {
                if (false == IsStopRotateInDelayTime)
                {
                    RefreshRendererRotation();

                    RefreshRendererFlip();
                }

                return false;
            }

            if (TryDetroyWithHealthTime())
            {
                DetroyWithHealthTime();
                return false;
            }

            return true;
        }

        public void Move()
        {
            switch (_motionType)
            {
                case ProjectileMotionTypes.Linear:
                case ProjectileMotionTypes.LinearToTarget:
                case ProjectileMotionTypes.Physics:
                case ProjectileMotionTypes.HomingLinear:
                    {
                        if (_mover != null)
                        {
                            _mover.Move();
                        }
                    }
                    break;

                case ProjectileMotionTypes.LinearCardinalToTarget:
                case ProjectileMotionTypes.DetectLinearToTarget:
                    {
                        RefreshMoveSpeed();

                        if (transform != null)
                        {
                            transform.Translate(Vector3.down * CurrentMoveSpeed * Time.fixedDeltaTime);
                        }
                    }
                    break;

                case ProjectileMotionTypes.Homing:
                    {
                        MoveHoming();
                    }
                    break;

                case ProjectileMotionTypes.DetectHoming:
                    {
                        MoveDetectHomingMotion();
                    }
                    break;

                case ProjectileMotionTypes.ReturnToOwner: // 위치
                    {
                        MoveReturnToOwner();
                    }
                    break;

                case ProjectileMotionTypes.OnTheSpot:
                    {
                        if (IsDestroyOnTheSpotMove)
                        {
                            Destroy();
                        }
                        else if (_mover != null)
                        {
                            _mover.Move();
                        }
                    }
                    break;
            }
        }

        public void OnMove()
        {
            if (_projectileData.GroundCheck)
            {
                DoGroundedResult(CheckGround());
            }

            DestroyWithDistanceCondition();

            DoArrivalResult();

            if (position.y < GameDefine.DEFAULT_PHYSICS_MIN_POSITION_Y)
            {
                Despawn();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isDestroy)
            {
                return;
            }

            if (_isStopMoveInDelayTime)
            {
                return;
            }

            if (m_isIgnoreTrigger)
            {
                return;
            }

            if (TryDestroyOnHitErase(collision))
            {
                DestroyOnHitErase(collision);
            }
            else if (CheckHitCollision(collision.gameObject))
            {
                if (CollisionCount < 1)
                {
                    DoCollisionResult();
                }
                else
                {
                    CollisionCount--;
                }
            }
            else
            {
                Vital targetVital = VitalManager.Instance.Find(collision);

                if (targetVital != null)
                {
                    if (TryAttackOnHit(targetVital))
                    {
                        AttackOnHit(targetVital);
                    }
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (CheckHitCollision(collision.gameObject))
            {
                SpawnCollisionFX();

                if (CollisionCount < 1)
                {
                    DoCollisionResult();
                }
                else
                {
                    CollisionCount--;
                }
            }
        }

        private bool TryDestroyOnHitErase(Collider2D collision)
        {
            if (LayerEx.IsInMask(collision.gameObject.layer, m_eraseLayer))
            {
                return true;
            }

            return false;
        }

        private void DestroyOnHitErase(Collider2D collision)
        {
            if (LayerEx.IsInMask(collision.gameObject.layer, GameLayers.Mask.Projectile))
            {
                Projectile projectile = ProjectileManager.Instance.Find(collision);
                if (projectile != null)
                {
                    projectile.DoResult(projectile._projectileData.EraseResult);
                }

                return;
            }

            if (LayerEx.IsInMask(collision.gameObject.layer, GameLayers.Mask.Breakable))
            {
                Vital targetVital = VitalManager.Instance.Find(collision);

                if (targetVital != null)
                {
                    targetVital.Suicide(Owner);
                }

                return;
            }
        }

        private bool CheckHitCollision(GameObject go)
        {
            if (LayerEx.IsInMask(go.layer, m_collisionLayer))
            {
                for (int i = 0; i < _projectileData.IgnoreCollisionTag.Length; i++)
                {
                    if (_projectileData.IgnoreCollisionTag[i] == GameTags.None)
                    {
                        continue;
                    }

                    if (go.CompareTag(_projectileData.IgnoreCollisionTag[i]))
                    {
                        return false;
                    }
                }

                if (go.CompareTag(_projectileData.CollisionTag))
                {
                    return true;
                }
            }

            return false;
        }

        private void DoCollisionResult()
        {
            DoResult(_projectileData.CollisionResult);

            if (Animator != null)
            {
                Animator.UpdateCollisionParameter(true);
            }
        }

        public void RefreshRendererFlip()
        {
            if (Renderer != null)
            {
                if (false == _targetDirection.IsZero())
                {
                    SetFlip(_targetDirection.x > 0);
                }
                else if (HomingTarget != null)
                {
                    SetFlip(position.x < HomingTarget.position.x);
                }
                else
                {
                    SetFlip(FacingRightAtLaunch);
                }
            }
        }

        private void ResetValuesOnSpawn()
        {
            _spawnTime = Time.time;
            _isSpawnedAnother = false;
            _isSpawnedReturn = false;
            _colliderBottom = Vector3.zero;
            _anotherCount = 0;
            _anotherMaxCount = 0;
            _throughCount = 0;
            _isDestroy = false;
            _isDespawn = false;
            _targetDirection = Vector3.zero;
            _targetPosition = Vector3.zero;
            _prevPosition = Vector3.zero;
            CollisionCount = _projectileData.CollisionCount;
            _despawnCoroutine = null;
        }

        private void ResetValuesOnDespawn()
        {
            _spawnTime = 0;
            _isSpawnedAnother = false;
            _isSpawnedReturn = false;
            _isExecutedTimeResult = false;
            _isExecutedDistanceResult = false;
            _colliderBottom = Vector3.zero;
            _anotherCount = 0;
            _anotherMaxCount = 0;
            _throughCount = 0;
            _isDestroy = false;
            _spawnPosition = Vector2.zero;
            _targetDirection = Vector3.zero;
            _targetPosition = Vector3.zero;
            _prevPosition = Vector3.zero;
        }

        public void Initialize()
        {
            Log.Info(LogTags.Projectile, "발사체를 초기화합니다. {0}", Name.ToLogString());

            if (Animator != null)
            {
                Animator.OnSpawn();
            }

            InitializeAttack();

            InitializeMaxCount();

            StartHealthTimeCondition();
        }

        public void OnSpawn()
        {
            Log.Info(LogTags.Projectile, "발사체가 생성되었습니다. {0}", Name.ToLogString());

            ResetValuesOnSpawn();

            ResetHomingTarget();

            ResetVitalValues();

            RegisterVital();

            RegisterAnimatorCallback();

            CallSpawnEvent();

            UpdateFreezeRotation();

            StartAutoRotate();

            ProjectileManager.Instance.Add(this);

            SetOrder(ProjectileManager.Instance.SerialNumber);
        }

        public void OnDespawn()
        {
            Log.Info(LogTags.Projectile, "발사체가 삭제되었습니다. {0}", Name.ToLogString());

            ResetValuesOnDespawn();

            ResetHomingTarget();

            StopAutoRotate();

            ResetRotation();

            UnregisterVital();

            StopAttack();

            StopAnotherAttack();

            ProjectileManager.Instance.Remove(this);

            UnregisterAnimatorCallback();

            LockDetector();
        }

        public void Despawn()
        {
            if (_isDespawn)
            {
                return;
            }

            Log.Info(LogTags.Projectile, "발사체를 삭제합니다. {0}", Name.ToLogString());

            _isDespawn = true;

            if (_projectileData.DeactivePhysicsOnDespawn)
            {
                StopMove();
            }

            if (_despawnCoroutine == null)
            {
                _despawnCoroutine = StartXCoroutine(ProcessDespawn());
            }
        }

        private IEnumerator ProcessDespawn()
        {
            yield return new WaitForEndOfFrame();

            StopAttack();

            ReleaseReturn();

            DespawnGameObject();

            CallDespawnEvent();

            UnregisterProjectile();

            _despawnCoroutine = null;
        }

        private void DespawnGameObject()
        {
            ResourcesManager.Despawn(gameObject);
        }

        private void UnregisterProjectile()
        {
            if (Owner != null)
            {
                if (Owner.projectileTraceSystem != null && AttackSystem != null)
                {
                    Owner.projectileTraceSystem.UnregisterProjectile(this, AttackSystem.GetAttackMethod());
                }
            }
        }

        public void Destroy()
        {
            if (_isDestroy || _isDespawn)
            {
                return;
            }

            Log.Info(LogTags.Projectile, "발사체를 파괴합니다. {0}", Name.ToLogString());

            if (_projectileData.DeactivePhysicsOnDestroy)
            {
                StopMove();
            }

            if (transform != null)
            {
                _destroyPosition = position;
            }

            _isDestroy = true;

            SpawnDestroyFX(FacingRightAtLaunch);

            DespawnSpawnFX();

            if (Animator != null)
            {
                if (TryPlayAnimation())
                {
                    if (false == PlayHitAnimation())
                    {
                        Despawn();
                    }
                }
            }
            else
            {
                Despawn();
            }
        }

        public void DestroyByAttack(DamageCalculator damageInfo)
        {
            if (damageInfo.Attacker != null)
            {
                SetFlip(damageInfo.Attacker.FacingRight);

                SpawnDamageFX(damageInfo.Attacker.FacingRight);
            }
            else
            {
                SpawnDamageFX(FacingRightAtLaunch);
            }

            DoResult(_projectileData.DamageResult);
        }

        public bool TryPlayAnimation()
        {
            return Animator.TryPlayAnimation();
        }

        public bool PlayHitAnimation()
        {
            if (Animator != null)
            {
                return Animator.PlayHitAnimation();
            }

            return false;
        }

        public bool PlayAttackAnimation()
        {
            if (Animator != null)
            {
                return Animator.PlayAttackAnimation();
            }

            return false;
        }

        private void DestroyWithDistanceCondition()
        {
            if (_projectileData == null)
            {
                return;
            }

            if (_isExecutedDistanceResult)
            {
                return;
            }

            float distance = _projectileData.Distance + AdditionalDistance;

            if (Mathf.Approximately(distance, 0f))
            {
                return;
            }

            float moveDistance = Vector3.Distance(_spawnPosition, position);
            if (moveDistance > distance)
            {
                DoResult(_projectileData.DistanceResult);

                _isExecutedDistanceResult = true;
            }
        }

        private void StartHealthTimeCondition()
        {
            if (_projectileData == null)
            {
                return;
            }

            if (_isExecutedTimeResult)
            {
                return;
            }

            _destroyTime = Time.time + _projectileData.HealthTime + AdditionalHealthTime;
        }

        protected bool TryDetroyWithHealthTime()
        {
            if (false == _useLiftTime)
            {
                return false;
            }

            if (_destroyTime.IsZero())
            {
                return false;
            }

            if (_destroyTime > Time.time)
            {
                return false;
            }

            return true;
        }

        protected void DetroyWithHealthTime()
        {
            DoResult(_projectileData.TimeResult);

            _destroyTime = 0;

            _isExecutedTimeResult = true;
        }

        private void DoResult(ProjectileResults projectileResult)
        {
            switch (projectileResult)
            {
                case ProjectileResults.Destroy:
                    {
                        Destroy();
                    }
                    break;

                case ProjectileResults.DestroyWithAttack:
                    {
                        StartAttack();

                        Destroy();
                    }
                    break;

                case ProjectileResults.DestroyWithAnother:
                    {
                        DoAnotherAttack();

                        Destroy();
                    }
                    break;

                case ProjectileResults.DestroyWithReturn:
                    {
                        ApplyReturn();

                        Destroy();
                    }
                    break;

                case ProjectileResults.DestroyWithAllAttack:
                    {
                        StartAttack();

                        DoAnotherAttack();

                        ApplyReturn();

                        Destroy();
                    }
                    break;

                case ProjectileResults.StopMove:
                    {
                        StopMove();
                    }
                    break;

                case ProjectileResults.Attack:
                    {
                        StartAttack();
                    }
                    break;

                case ProjectileResults.ActivateDetector:
                    {
                        UnlockDetector();
                    }
                    break;

                case ProjectileResults.DeactivateDetector:
                    {
                        LockDetector();
                    }
                    break;

                case ProjectileResults.Immunity:
                    {
                        ResetVital();
                    }
                    break;

                case ProjectileResults.ActivateTrigger:
                    {
                        m_isIgnoreTrigger = false;
                    }
                    break;

                case ProjectileResults.DeactivateTrigger:
                    {
                        m_isIgnoreTrigger = true;
                    }
                    break;
            }
        }

        public void SetGrade(int grade)
        {
            Grade = grade;
        }

        public void SetLevel(int level)
        {
            Level = level;

            SetAddtionalHealthTime();

            SetAddtionalDistance();
        }

        public void SetSpawnWorldPosition(Vector3 spawnPosition)
        {
            position = spawnPosition;

            _spawnPosition = position;
        }

        public void SetChainProjectileInfo(XChainProjectile info)
        {
            ChainProjectileInfo.Replace(info);

            if (AttackSystem != null)
            {
                AttackSystem.SetAttackChainProjectileInfo(info);
                AttackSystem.SetAnotherChainProjectileInfo(info);
                AttackSystem.SetReturnChainProjectileInfo(info);
            }
        }

        public void SetOwner(Character owner)
        {
            Owner = owner;

            if (AttackSystem != null)
            {
                AttackSystem.SetOwnerCharacter(owner);
            }
        }

        public void SetOrder(int order)
        {
            if (Renderer != null)
            {
                Renderer.SetOrder(order);
            }
        }

        public void SetAnotherCount(int anotherCount)
        {
            _anotherCount = anotherCount;

            if (AttackSystem != null)
            {
                AttackSystem.SetAnotherCount(_anotherCount);

                AttackSystem.SetAnotherMaxCount(AnotherMaxCount);
            }
        }

        public void SetHomingTarget(Vital targetVital)
        {
            switch (_motionType)
            {
                case ProjectileMotionTypes.OnTheTarget:
                case ProjectileMotionTypes.OnTheTargetGround:
                case ProjectileMotionTypes.LinearToTarget:
                case ProjectileMotionTypes.DetectLinearToTarget:
                case ProjectileMotionTypes.Homing:
                case ProjectileMotionTypes.HomingLinear:
                case ProjectileMotionTypes.DetectHoming:
                    {
                        if (ChainProjectileInfo.ContainsTarget(targetVital))
                        {
                            if (ChainProjectileInfo.RegisterTargetType == RegisterTargetTypes.Infinity)
                            {
                                Log.Warning(LogTags.Projectile, "발사체의 타겟을 설정할 수 없습니다. 이미 타격한 적을 타겟으로 삼을 수 없습니다. {0}, Target:{1}", this.GetHierarchyPath(), targetVital.GetHierarchyPath());
                                return;
                            }
                        }

                        _homingTarget = targetVital;

                        Log.Info(LogTags.Projectile, "발사체의 타겟을 설정합니다. {0}, {1}, {2}", Name.ToLogString(), targetVital.GetHierarchyPath(), this.GetHierarchyPath());
                    }
                    break;
            }
        }

        public void ResetHomingTarget()
        {
            _homingTarget = null;
            Log.Info(LogTags.Projectile, "발사체의 타겟을 초기화합니다. {0}, {1}", Name.ToLogString(), this.GetHierarchyPath());
        }

        public void SetFacingRightAtLaunch(bool facingRight)
        {
            _facingRightAtLaunch = facingRight;
        }

        public void SetFlip(bool facingRight)
        {
            if (FlipController != null)
            {
                FlipController.SetFlip(facingRight);
            }
        }

        public void ResetFlip()
        {
            if (FlipController != null)
            {
                FlipController.ResetFlip();
            }
        }

        public void BlockFlip()
        {
            if (FlipController != null)
            {
                FlipController.BlockFlip();
            }
        }

        public void RefreshCoreMaterial()
        {
            if (Renderer == null)
            {
                return;
            }

            CoreTypes core = GetAttackCoreType();

            Renderer.SetMaterial(core);
        }

        public bool CheckSpawnConditions()
        {
            if (UseCheckWall)
            {
                Vector3 right = position + Vector3.right * CheckWallDistance * 0.5f;

                Vector3 left = position + Vector3.left * CheckWallDistance * 0.5f;

                RaycastHit2D hit = Physics2D.Linecast(left, right, GameLayers.Mask.Collision);

                if (hit.collider != null)
                {
                    Log.Warning(LogTags.Projectile, "벽과 충돌하여 발사체를 생성할 수 없습니다. {0}", Name.ToLogString());

                    return false;
                }
            }

            if (_projectileData.GroundCheck)
            {
                if (false == CheckGround())
                {
                    Log.Warning(LogTags.Projectile, "땅과 충돌하지 않아 발사체를 생성할 수 없습니다. {0}", Name.ToLogString());
                    return false;
                }
            }

            return true;
        }

        protected IEnumerator ProcessStopMoveInDelayTime(float delayTime)
        {
            _isStopMoveInDelayTime = true;

            SetEnabledCollider(false);

            yield return new WaitForSeconds(delayTime);

            DetectVitalsInArea();

            _isStopMoveInDelayTime = false;

            SetEnabledCollider(true);
        }

        private void DetectVitalsInArea()
        {
            Vital[] vitals = null;

            if (m_boxCollider2D != null)
            {
                vitals = VitalManager.Instance.FindInArea(position, m_boxCollider2D.size, m_targetLayer);
            }
            else if (m_circleCollider2D != null)
            {
                vitals = VitalManager.Instance.FindInArea(position, m_circleCollider2D.radius, m_targetLayer);
            }
            else
            {
                Log.Warning(LogTags.Projectile, "failed to detect vitals in area. collider is null. {0}", this.GetHierarchyPath());
                return;
            }

            if (vitals == null)
            {
                return;
            }

            for (int i = 0; i < vitals.Length; i++)
            {
                if (vitals[i] == null)
                {
                    continue;
                }

                if (TryAttackOnHit(vitals[i]))
                {
                    AttackOnHit(vitals[i]);
                }
            }
        }

        #region Detector

        public void LockDetector()
        {
            if (Detectors == null)
            {
                return;
            }

            for (int i = 0; i < Detectors.Length; i++)
            {
                if (Detectors[i] != null)
                {
                    Detectors[i].Lock();
                }
            }
        }

        private void UnlockDetector()
        {
            if (Detectors == null)
            {
                return;
            }

            for (int i = 0; i < Detectors.Length; i++)
            {
                if (Detectors[i] != null)
                {
                    Detectors[i].Unlock();
                }
            }
        }

        #endregion Detector

        #region Event

        public void RegisterSpawnEvent(UnityAction<Projectile> action)
        {
            OnSpawnCallback.AddListener(action);
        }

        public void UnregisterSpawnEvent(UnityAction<Projectile> action)
        {
            OnSpawnCallback.RemoveListener(action);
        }

        public void UnregisterDespawnEvent(UnityAction<Projectile> action)
        {
            OnDespawnCallback.RemoveListener(action);
        }

        public void RegisterDespawnEvent(UnityAction<Projectile> action)
        {
            OnDespawnCallback.AddListener(action);
        }

        public void CallSpawnEvent()
        {
            if (OnSpawnCallback != null)
            {
                OnSpawnCallback.Invoke(this);
            }
        }

        public void CallDespawnEvent()
        {
            if (OnDespawnCallback != null)
            {
                OnDespawnCallback.Invoke(this);
            }
        }

        #endregion Event

        private void RegisterAnimatorCallback()
        {
            if (Animator != null)
            {
                Animator.RegisterAttackAnimationExitCallback(Despawn);

                Animator.RegisterHitAnimationExitCallback(Despawn);
            }
        }

        private void UnregisterAnimatorCallback()
        {
            if (Animator != null)
            {
                Animator.UnregisterAllAnimationCallback();
            }
        }

        #region Vital

        private void ResetVital()
        {
            if (Vital != null)
            {
                Vital.Health.ResetCurrentValue();
            }
        }

        private void ResetVitalValues()
        {
            if (Vital != null)
            {
                Vital.Health?.ResetCurrentValue();
                Vital.Mana?.ResetCurrentValue();
                Vital.Shield?.ResetCurrentValue();
            }
        }

        private void RegisterVital()
        {
            if (Vital != null)
            {
                VitalManager.Instance.Add(Vital);
            }
        }

        private void UnregisterVital()
        {
            if (Vital != null)
            {
                VitalManager.Instance.Remove(Vital);
            }
        }

        #endregion Vital

        #region Rotation

        private void StartAutoRotate()
        {
            if (RotationController != null)
            {
                RotationController.StartAutoRotate();
            }
        }

        private void StopAutoRotate()
        {
            if (RotationController != null)
            {
                RotationController.StopAutoRotate();
            }
        }

        private void ResetRotation()
        {
            if (RotationController != null)
            {
                RotationController.ResetRotation();
            }
        }

        private void RefreshRotationParent()
        {
            if (RotationController != null)
            {
                RotationController.RefreshRotationParent(HomingTarget, _targetDirection);
            }
        }

        public void RefreshRendererRotation()
        {
            if (RotationController != null)
            {
                RotationController.RefreshRendererRotation(HomingTarget, _targetDirection);
            }
        }

        public void SetRotation(Vector3 direction)
        {
            if (transform == null)
            {
                return;
            }

            if (direction.IsZero())
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                float resultAngle = AngleEx.ToAngle(position, position + direction);

                Quaternion resultQuaternion = Quaternion.Euler(0, 0, resultAngle);

                if (Renderer != null)
                {
                    transform.rotation = Quaternion.Slerp(Renderer.transform.rotation, resultQuaternion, 1);
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, resultQuaternion, 1);
                }
            }
        }

        public void SetRotation(Quaternion rotation)
        {
            if (transform == null)
            {
                return;
            }

            if (false == UseEntityRotation)
            {
                return;
            }

            transform.rotation = rotation;
        }

        public void SetRendererRotation(Vector2 direction)
        {
            if (RotationController != null)
            {
                RotationController.RefreshRendererRotation(direction);
            }
        }

        #endregion Rotation
    }
}