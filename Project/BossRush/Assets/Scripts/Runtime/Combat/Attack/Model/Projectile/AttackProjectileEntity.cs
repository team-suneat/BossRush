using System.Collections;
using System.Collections.Generic;
using TeamSuneat.Projectiles;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackProjectileEntity : AttackEntity
    {
        public override void Initialization()
        {
            base.Initialization();

            RefreshBaseProjectilePerShot();
        }

        public override void OnBattleReady()
        {
            base.OnBattleReady();

            _activeProjectiles.Clear();
        }

        public override void Activate()
        {
            if (DetermineActivate())
            {
                base.Activate();

                StartAttackForce();
                Apply();
            }
        }

        public override void Deactivate()
        {
            LogInfo("Attack Projectile Entity를 비활성화합니다.");

            base.Deactivate();

            if (!IgnoreStopShootingOnDeactivate)
            {
                StopShooting();
            }

            if (DespawnOnDeactivate)
            {
                DespawnAllProjectile();
            }
        }

        public override void OnOwnerDeath()
        {
            base.OnOwnerDeath();

            if (CanStopShootingOnDeath)
            {
                StopShooting();
            }

            if (DespawnOnDeath)
            {
                DespawnAllProjectile();
            }
        }

        public override void Apply()
        {
            LogInfo("발사체 공격 독립체를 적용합니다. 레벨:{0} ", Level);

            base.Apply();

            int projectilesPerShot = GetTotalProjectileCount();

            // 최대 발사체 수를 검사하고, 사격 여부를 결정
            bool canShoot = CheckAndHandleMaxProjectiles(projectilesPerShot);
            if (canShoot)
            {
                SetRangeXOfSpawnPositionOffset();
                StartShooting(projectilesPerShot);
            }
            else
            {
                LogInfo("사격을 중단했습니다.");
            }
        }

        private bool CheckAndHandleMaxProjectiles(int projectilesPerShot)
        {
            int totalProjectilesAfterShot = _activeProjectiles.Count + projectilesPerShot;
            int maxProjectileCount = MaxProjectileCount;
            if (StatNameOfByMaxProjectileCount != StatNames.None)
            {
                maxProjectileCount = Owner.Stat.FindValueOrDefaultToInt(StatNameOfByMaxProjectileCount);
            }

            if (maxProjectileCount > 0 && totalProjectilesAfterShot > maxProjectileCount)
            {
                int despawnCount = totalProjectilesAfterShot - maxProjectileCount;
                if (ReactReachMaxCount == ReactsReachMaxCount.ResetTimeAndReuse)
                {
                    for (int i = 0; i < despawnCount; i++)
                    {
                        _activeProjectiles[i].SetSpawnTimeToNow();
                    }
                    LogInfo("최대 발사체 수를 초과하여 {0}개의 발사체 시간을 초기화했습니다.", despawnCount);
                }
                else if (ReactReachMaxCount == ReactsReachMaxCount.ExpireAndDestroy)
                {
                    for (int i = 0; i < despawnCount; i++)
                    {
                        _activeProjectiles[i].SetSpawnTimeToAfterDuration();
                    }
                    LogInfo("최대 발사체 수를 초과하여 {0}개의 발사체를 파괴했습니다.", despawnCount);
                }

                // 추가적인 코너 케이스 처리
                totalProjectilesAfterShot = _activeProjectiles.Count + projectilesPerShot - despawnCount;
                if (totalProjectilesAfterShot > maxProjectileCount)
                {
                    int additionalDespawnCount = totalProjectilesAfterShot - maxProjectileCount;
                    for (int i = 0; i < additionalDespawnCount; i++)
                    {
                        if (ReactReachMaxCount == ReactsReachMaxCount.ResetTimeAndReuse)
                        {
                            _activeProjectiles[i + despawnCount].SetSpawnTimeToNow();
                        }
                        else if (ReactReachMaxCount == ReactsReachMaxCount.ExpireAndDestroy)
                        {
                            _activeProjectiles[i + despawnCount].SetSpawnTimeToAfterDuration();
                        }
                    }
                    LogInfo("추가로 {0}개의 발사체를 처리하여 최대치를 맞췄습니다.", additionalDespawnCount);
                }

                // 만약 ReactReachMaxCount가 FirstResetElapsedTime인 경우, 사격을 중단
                if (ReactReachMaxCount == ReactsReachMaxCount.ResetTimeAndReuse)
                {
                    return false;
                }
            }

            // 사격을 진행
            return true;
        }

        public override void SetOwner(Character ownerCharacter)
        {
            base.SetOwner(ownerCharacter);

            if (DetectSystem != null)
            {
                DetectSystem.Owner = ownerCharacter;
            }
        }

        private void LateUpdate()
        {
            if (CanStopShootingOnDamage)
            {
                if (Owner.CharacterAnimator.IsDamaging)
                {
                    StopShooting();
                }
            }
        }

        #region 사격 (Shooting)

        /// <summary> 사격을 시작합니다. </summary>
        private void StartShooting(int projectilesPerShot)
        {
            if (projectilesPerShot > 0)
            {
                LogInfo("사격을 시작합니다.");
                StartXCoroutine(SpawnProjectileWithRemoval(projectilesPerShot));
                OnAttack(false);
            }
            else
            {
                LogWarning("발사 횟수가 잘못 설정되었습니다. {0}", projectilesPerShot);
            }
        }

        /// <summary> 사격을 중지합니다. </summary>
        private void StopShooting()
        {
            LogInfo("사격을 종료합니다.");

            if (_activeShootingCoroutines.IsValid())
            {
                for (int i = 0; i < _activeShootingCoroutines.Count; i++)
                {
                    Coroutine coroutine = _activeShootingCoroutines[i];

                    StopXCoroutine(ref coroutine);
                }

                _activeShootingCoroutines.Clear();
            }
            if (ProjectileMotion == ProjectileMotionTypes.AllSpawnToActivateTarget)
            {
                StartXCoroutine(ProcessProjectilesMovable());
            }

            if (AttackMonster != null)
            {
                AttackMonster = null;
            }
        }

        private IEnumerator SpawnProjectileWithRemoval(int projectilesPerShot)
        {
            Coroutine currentCoroutine = StartXCoroutine(ProcessShoot(projectilesPerShot));
            RegisterShootingCoroutine(currentCoroutine);

            yield return currentCoroutine;

            UnregisterShootingCoroutine(currentCoroutine);
        }

        #region Process Shooting

        private IEnumerator ProcessShoot(int projectilesPerShot)
        {
            int totalProjectileCount = projectilesPerShot * ShotSpread;
            LogInfo("총 발사 갯수가 지정됩니다. : {0}", totalProjectileCount);

            // 첫 발사 전 대기 시간을 처리합니다.
            if (DelayTimeShooting > 0)
            {
                yield return HandlePreDelay(totalProjectileCount);
            }

            // 포지션 그룹이 Pin 설정 이전에 되어있어야, 적합한 생성 위치를 설정할 수 있습니다.
            SetupPositionGroupIfAvailable();

            if (!PinBeforeDelay)
            {
                PinProjectileSettings(totalProjectileCount);
            }

            for (int round = 0; round < projectilesPerShot; round++)
            {
                yield return ShootProjectiles(round);
                ResetAfterShoot();
                if (round < projectilesPerShot - 1)
                {
                    yield return new WaitForSeconds(IntervalPerShot);

                    if (UseSetupPositionGroupPerRound)
                    {
                        ShufflePositionGroupPerRound();
                    }
                }
            }
        }

        private IEnumerator HandlePreDelay(int totalProjectileCount)
        {
            LogInfo("첫 발사 전에 {0}초 대기합니다.", DelayTimeShooting);

            if (PinBeforeDelay)
            {
                PinProjectileSettings(totalProjectileCount);
            }

            yield return new WaitForSeconds(DelayTimeShooting);
        }

        // 발사체에 대한 설정을 핀 처리합니다.
        private void PinProjectileSettings(int totalProjectileCount)
        {
            PinDetectTargetVitals(); // 타겟의 중요한 데이터를 핀으로 고정합니다.
            PinSpawnPositions(totalProjectileCount); // 발사체 스폰 위치를 핀으로 고정합니다.
            PinedTargetPositions(totalProjectileCount); // 타겟 위치를 핀으로 고정합니다.
            PinDirections(totalProjectileCount); // 발사 방향을 핀으로 고정합니다.
        }

        // 발사체를 발사하는 루프를 처리합니다.
        private IEnumerator ShootProjectiles(int round)
        {
            Vector3 spawnPosition = Vector3.zero;
            if (PinSpawnPositionForShootEnd)
            {
                spawnPosition = GetSpawnPosition(0); // 첫 번째 스폰 위치를 가져옵니다.
            }

            for (int i = 0; i < ShotSpread; i++)
            {
                // 전체 발사체의 수에 맞게 인덱스를 설정합니다.
                int projectileIndex = (round * ShotSpread) + i;

                if (!PinSpawnPositionForShootEnd)
                {
                    // 각 발사체의 스폰 위치를 가져옵니다.
                    spawnPosition = GetSpawnPosition(projectileIndex);
                }

                if (!TrySpawnProjectile(spawnPosition))
                {
                    // 발사체 스폰에 실패하면 루프를 중단합니다.
                    break;
                }

                Projectile projectile = SpawnProjectile(spawnPosition);
                if (projectile != null)
                {
                    HandleProjectileHealthcycle(projectile, projectileIndex); // 발사체의 라이프사이클을 처리합니다.
                    HandleProjectileRotation(projectileIndex); // 발사체 회전을 처리합니다.
                    HandleChildProjectile(projectile); // 부모 발사체의 충돌 대상 목록을 자식에게 전달합니다.
                }

                yield return null;
            }
        }

        // 발사체의 라이프사이클을 처리합니다.
        private void HandleProjectileHealthcycle(Projectile projectile, int index)
        {
            _activeProjectiles.Add(projectile); // 활성화된 발사체 리스트에 추가합니다.
            projectile.Index = index; // 발사체의 인덱스를 설정합니다.

            if (TargetVital != null)
            {
                OnSpawnProjectile(projectile, index, TargetVital); // 타겟 데이터와 함께 발사체 스폰 시 콜백을 호출합니다.
            }
            else
            {
                OnSpawnProjectile(projectile, index); // 타겟 없이 발사체 스폰 시 콜백을 호출합니다.
            }
        }

        // 발사체 회전을 처리합니다.
        private void HandleProjectileRotation(int index)
        {
            if (AddRotationZPerShot > 0)
            {
                _addRotationZPerShot += AddRotationZPerShot;
                transform.Rotate(new Vector3(0, 0, _addRotationZPerShot)); // Z축 회전 적용
            }
        }

        public void HandleChildProjectile(Projectile childProjectile)
        {
            // 부모-자식 관계 설정
            if (childProjectile != null && OwnerProjectile != null)
            {
                // 부모 발사체의 ProjectileColliderController를 가져옴
                ProjectileColliderController parentController = OwnerProjectile.ColliderController;

                // 자식 발사체의 ProjectileColliderController를 가져옴
                ProjectileColliderController childController = childProjectile.ColliderController;

                // 부모-자식 관계 설정
                if (parentController != null && childController != null)
                {
                    // 부모의 충돌 대상 목록을 자식에게 전달
                    childController.SetParentCollisionTargets(parentController.TargetColliders);
                }
            }
        }

        // 발사 후 설정을 초기화합니다.
        private void ResetAfterShoot()
        {
            if (PinSpawnPositionForShootEnd)
            {
                _addRotationZPerShot = 0;
                transform.rotation = Quaternion.identity; // 회전을 초기화합니다.
            }

            if (ProjectileMotion == ProjectileMotionTypes.AllSpawnToActivateTarget)
            {
                ProjectilesMovable(); // 모든 발사체를 활성화 타겟으로 이동 가능하게 설정합니다.
            }
        }

        #endregion Process Shooting

        private bool DetermineIntervalWait(int projectileIndex)
        {
            if (ShotSpread == 1)
            {
                return true;
            }
            else if (projectileIndex != 0)
            {
                int remainder = (projectileIndex + 1) % ShotSpread;
                if (remainder == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void RegisterShootingCoroutine(Coroutine coroutine)
        {
            _activeShootingCoroutines.Add(coroutine);
            LogInfo("발사체 발사 코루틴을 추가합니다. 코루틴 수: {0}", _activeShootingCoroutines.Count);
        }

        private void UnregisterShootingCoroutine(Coroutine coroutine)
        {
            // 유효성 검사: 코루틴이 null인지 확인
            if (coroutine == null)
            {
                LogWarning("삭제하려는 코루틴이 null입니다. 이미 종료되었거나 잘못된 참조입니다.");
                return;
            }

            // 유효성 검사: 리스트에 코루틴이 포함되어 있는지 확인
            if (_activeShootingCoroutines.Contains(coroutine))
            {
                _activeShootingCoroutines.Remove(coroutine);
                LogInfo("발사체 발사 코루틴을 삭제합니다. 코루틴 수: {0}", _activeShootingCoroutines.Count);
            }
            else
            {
                LogWarning("코루틴이 이미 리스트에서 제거되었거나 존재하지 않습니다.");
            }
        }

        #endregion 사격 (Shooting)

        private bool TrySpawnProjectile(Vector3 spawnPosition)
        {
            if (spawnPosition.IsZero())
            {
                LogWarning("생성 위치를 설정하지 못하여 생성할 수 없습니다.");
                return false;
            }

            if (SpawnOnlyOnGround)
            {
                RaycastHit2D hit = Physics2D.Raycast(spawnPosition, Vector3.down, GroundCheckDistance, TSLayers.Mask.Obstacles);
                if (hit)
                {
                    return true;
                }

                TSDebugDrawEx.DrawRay(spawnPosition, Vector2.down * GroundCheckDistance, TSColors.CherryRed, 3f);
                TSDebugDrawEx.DrawCross(spawnPosition + (Vector3.down * GroundCheckDistance), 0.1f, TSColors.CherryRed, 3f);

                LogWarning("땅과의 거리가 멀어 발사체를 생성할 수 없습니다.");
                return false;
            }

            if (NoSpawnInDisntance)
            {
                if (!NoSpawnDisntance.IsZero())
                {
                    for (int i = 0; i < _activeProjectiles.Count; i++)
                    {
                        float distance = Vector2.Distance(spawnPosition, _activeProjectiles[i].position);
                        if (distance < NoSpawnDisntance)
                        {
                            LogWarning("생성한 발사체와 생성할 발사체의 위치가 너무 가까워 새 발사체를 생성할 수 없습니다.");
                            return false;
                        }
                    }
                }
            }

            if (ProjectileMotion == ProjectileMotionTypes.Homing)
            {
                if (PinTargetVitalsOnShoot)
                {
                    if (!_pinedVitals.IsValid() && StopShootingOnFailedPinVital)
                    {
                        LogWarning("설정된 타겟이 없어 유도 발사체를 생성할 수 없습니다.");
                        return false;
                    }
                }
            }

            return true;
        }

        private Projectile SpawnProjectile(Vector3 spawnPosition)
        {
            GameObject prefab = GetProjectilePrefab();

            if (prefab == null)
            {
                Log.Error("해당 공격 독립체({0})의 발사체 프리펩이 설정되지 않았습니다.", Name.ToLogString());
                return null;
            }

            switch (SpawnPositionType)
            {
                case ProjectileSpawnTypes.Parent:
                    {
                        return Resource.ResourcesManager.SpawnProjectile(prefab, transform);
                    }
                case ProjectileSpawnTypes.ParentCharacter:
                    {
                        Projectile projectile = Resource.ResourcesManager.SpawnProjectile(prefab, Owner.transform);
                        if (projectile != null)
                        {
                            projectile.position = position;
                        }

                        return projectile;
                    }

                default:
                    {
                        return Resource.ResourcesManager.SpawnProjectile(prefab, spawnPosition);
                    }
            }
        }

        /// <summary> 생성된 발사체에 위치, 방향 등을 설정합니다. </summary>
        private void OnSpawnProjectile(Projectile projectile, int projectileIndex, Vital targetVitalOfPassive = null)
        {
            LogInfo("{1} 번째 발사체({0})를 생성합니다.", projectile.GetHierarchyName(), projectileIndex + 1);
            projectile.SetOwner(Owner);
            if (targetVitalOfPassive == null)
            {
                Vital targetVital = GetTargetVital(projectileIndex);
                projectile.SetTarget(targetVital);
            }
            else
            {
                projectile.SetTarget(targetVitalOfPassive);
            }

            SetTargetPosition(projectile, projectileIndex);

            projectile.LateShowRenderer();
            projectile.Initialization();
            projectile.RegisterDespawnEvent(OnDespawnProjectile);

            SetProjectileFacingRight(projectile);
            SetDirectionByMotion(projectile, projectileIndex);
        }

        private void DespawnAllProjectile()
        {
            for (int i = 0; i < _activeProjectiles.Count; i++)
            {
                _activeProjectiles[i].DespawnSummonedCharacter();
                _activeProjectiles[i].Hit();
            }
        }

        private void OnDespawnProjectile(Projectile projectile)
        {
            if (_activeProjectiles.Contains(projectile))
            {
                _activeProjectiles.Remove(projectile);
            }
        }

        private int GetTotalProjectileCount()
        {
            if (ResourceInequalities.IsValid())
            {
                float addValue = ResourceInequalities.GetAddValue(Owner.MyVital);
                return _baseProjectilesPerShot + Mathf.RoundToInt(addValue);
            }

            return _baseProjectilesPerShot;
        }

        private GameObject GetProjectilePrefab()
        {
            if (!Prefabs.IsValidArray())
            {
                return Prefab;
            }

            if (_prefabDeck.Count > 0)
            {
                return _prefabDeck.DrawTop();
            }

            _prefabDeck.Set(Prefabs);

            if (UseShuffle)
            {
                _prefabDeck.Shuffle();
                return _prefabDeck.DrawTop();
            }

            if (TargetVital == null ||
                TargetVital.Owner == null ||
                TargetVital.Owner.Controller == null)
            {
                return _prefabDeck.Get(0);
            }

            if (TargetVital.Owner.Controller.State.IsGrounded)
            {
                return _prefabDeck.Get(0);
            }
            else
            {
                return _prefabDeck.Get(1);
            }
        }

        // 목표 위치 (Target Position)

        private void PinedTargetPositions(int projectileCount)
        {
            if (PinTargetPositionOnShoot)
            {
                if (TargetSetting == TargetSettings.PointOfPositionGroup)
                {
                    if (TryFindPositionGroupForTarget())
                    {
                        Vector3 offset = GetTargetPositionOffset();
                        List<Vector3> positions;
                        if (UseShuffleForTargetPositions)
                        {
                            positions = TargetPositionGroup.GetShufflePositions(position, projectileCount);
                        }
                        else
                        {
                            positions = TargetPositionGroup.GetPositions(position, projectileCount);
                        }

                        if (positions.IsValid())
                        {
                            for (int i = 0; i < positions.Count; i++)
                            {
                                _pinedTargetPositions.Add(positions[i] + offset);
                            }
                        }

                        _pinedTargetPositions.ToArray();
                    }
                }

                //------------------------------------------------------------------------------------------------------------==

                for (int i = 0; i < projectileCount; i++)
                {
                    switch (TargetSetting)
                    {
                        case TargetSettings.PointOfTarget:
                            {
                                if (TargetVital != null && TargetVital.Owner != null)
                                {
                                    Vector3 offset = GetTargetPositionOffset();
                                    _pinedTargetPositions.Add(TargetVital.Owner.position + offset);
                                }
                            }
                            break;

                        case TargetSettings.GroundPointOfTarget:
                            {
                                if (TargetVital != null && TargetVital.Owner != null)
                                {
                                    Vector3 offset = GetTargetPositionOffset();
                                    _pinedTargetPositions.Add(TargetVital.Owner.GroundPosition + offset);
                                }
                            }
                            break;

                        case TargetSettings.LastGroundPointOfTarget:
                            {
                                _pinedTargetPositions.Add(StartPosition);
                            }
                            break;
                    }
                }

                //------------------------------------------------------------------------------------------------------------==
            }
        }

        private Transform GetTargetTransform()
        {
            if (Owner != null && Owner.Target != null)
            {
                return Owner.Target;
            }
            else if (TargetVital != null)
            {
                Collider2D vitalCollider = TargetVital.GetNotGuardCollider();
                if (vitalCollider != null)
                {
                    return vitalCollider.transform;
                }
            }

            return null;
        }

        private Vector3 GetTargetPositionOffset()
        {
            Vector3 fixedOffset = TargetPositionOffset;
            Vector3 randomOffset = TSRandomEx.GetVector3Value(AddRandomRangeOfTargetPosition);

            return fixedOffset + randomOffset;
        }

        private void SetTargetPosition(Projectile projectile, int projectileIndex)
        {
            if (PinTargetPositionOnShoot)
            {
                if (_pinedTargetPositions.IsValid(projectileIndex))
                {
                    projectile.SetTargetPosition(_pinedTargetPositions[projectileIndex], Vector3.zero);
                    return;
                }
            }

            switch (TargetSetting)
            {
                case TargetSettings.PointOfTarget:
                    {
                        if (TargetVital != null)
                        {
                            Collider2D collider = TargetVital.GetNotGuardCollider();
                            if (collider != null)
                            {
                                Vector3 offset = GetTargetPositionOffset();
                                projectile.SetTargetPosition(collider.transform.position, offset);
                            }
                        }
                        else if (Owner.Target != null)
                        {
                            Vector3 offset = GetTargetPositionOffset();
                            projectile.SetTargetPosition(Owner.Target.position, offset);
                        }
                        else
                        {
                            LogWarning("발사체 생성에 필요한 타겟이 설정되지 않았습니다. {0}", Name.ToLogString());
                        }
                    }
                    break;

                case TargetSettings.GroundPointOfTarget:
                    {
                        if (TargetVital != null && TargetVital.Owner != null)
                        {
                            Vector3 offset = GetTargetPositionOffset();
                            projectile.SetTargetPosition(TargetVital.Owner.GroundPosition, offset);
                        }
                        else if (Owner.TargetCharacter != null)
                        {
                            Vector3 offset = GetTargetPositionOffset();
                            projectile.SetTargetPosition(Owner.TargetCharacter.GroundPosition, offset);
                        }
                        else if (Owner.Target != null)
                        {
                            RaycastHit2D hit = Physics2D.Raycast(Owner.Target.position, Vector2.down, 999, TSLayers.Mask.Obstacles);
                            if (hit)
                            {
                                Vector3 offset = GetTargetPositionOffset();
                                projectile.SetTargetPosition(hit.point, offset);
                            }
                            else
                            {
                                LogWarning("발사체 생성에 필요한 타겟으로부터 가까운 땅이 설정되지 않았습니다. {0}", Name.ToLogString());
                            }
                        }
                        else
                        {
                            LogWarning("발사체 생성에 필요한 타겟이 설정되지 않았습니다. {0}", Name.ToLogString());
                        }
                    }
                    break;

                case TargetSettings.LastGroundPointOfTarget:
                    {
                        projectile.SetTargetPosition(StartPosition, Vector2.zero);
                    }
                    break;

                case TargetSettings.PointOfPositionGroup:
                    {
                        if (TargetPositionGroup != null)
                        {
                            Vector3 offset = GetTargetPositionOffset();

                            projectile.SetTargetPosition(TargetPositionGroup.GetPosition(position), offset);
                        }
                        else
                        {
                            LogWarning("발사체 생성에 필요한 타겟 포지션 그룹이 설정되지 않았습니다. {0}", Name.ToLogString());
                        }
                    }
                    break;
            }
        }

        // 발사체 방향 (Projectile Direction)

        private void PinDirections(int projectileCount)
        {
            if (!PinDirectionOnShoot)
            {
                return;
            }

            Vector3 direction;
            _pinedDirections.Clear();

            for (int i = 0; i < projectileCount; i++)
            {
                if (PinSpawnPositionOnShoot)
                {
                    if (_pinedSpawnPositions.Count > i)
                    {
                        direction = GetDirectionByType(_pinedSpawnPositions[i], i);
                        _pinedDirections.Add(direction);
                        continue;
                    }
                }

                direction = GetDirectionByType(position, i);
                _pinedDirections.Add(direction);
            }
        }

        private void SetProjectileFacingRight(Projectile projectile)
        {
            bool isFacingRight;
            if (GetFacingRight(out isFacingRight))
            {
                projectile.SetFacingRight(isFacingRight);

            }
            else
            {
                LogProgress("발사체의 기본 방향 설정을 사용합니다.");
                // 발사체의 기본 방향을 사용하므로 별도의 설정을 하지 않습니다.
            }
        }

        // 발사체 목표 방향 (Projectile Target Direction)

        private Vector3 GetTargetDirection(Transform target, Vector3 spawnPosition)
        {
            // 가이드라인 우선 사용
            if (GuideLine != null)
            {
                if (!GuideLine.Direction.IsZero())
                {
                    return GuideLine.Direction;
                }
            }

            // 타겟이 존재하는 경우
            if (target != null)
            {
                Vector3 directionToTarget = (target.position - spawnPosition).normalized;
                if (!IgnoreTargetFacing)
                {
                    Vector3 currentDirection = Vector3.zero;
                    if (OwnerProjectile != null)
                    {
                        currentDirection = OwnerProjectile.Direction;
                    }
                    else if (Owner != null)
                    {
                        currentDirection = Owner.IsFacingRight ? transform.right : -transform.right;
                    }

                    if (directionToTarget.x < 0 && currentDirection.x > 0)
                    {
                        return transform.right;
                    }
                    else if (directionToTarget.x > 0 && currentDirection.x < 0)
                    {
                        return -transform.right;
                    }
                }

                return directionToTarget;
            }
            else if (OwnerProjectile != null)
            {
                return OwnerProjectile.Direction;
            }
            else if (Owner != null)
            {
                return Owner.IsFacingRight ? transform.right : -transform.right;
            }

            return Vector3.zero;
        }

        private void ProjectilesMovable()
        {
            for (int i = 0; i < _activeProjectiles.Count; i++)
            {
                SetDirectionByMotion(_activeProjectiles[i], i);

                _activeProjectiles[i].StartMovement();
            }
        }

        private IEnumerator ProcessProjectilesMovable()
        {
            yield return new WaitForSeconds(CancelProjectilesWaitTime);

            ProjectilesMovable();
        }

        // 발사체 목표 탐지 (Projectile Detect Target)

        private void PinDetectTargetVitals()
        {
            if (PinTargetVitalsOnShoot)
            {
                if (DetectSystem != null)
                {
                    _pinedVitals.Clear();

                    List<Vital> detectedVitals = DetectSystem.DoDetectVitalList();
                    if (detectedVitals.IsValid())
                    {
                        _pinedVitals.AddRange(detectedVitals);
                    }
                }
            }
        }

        private Vital GetPinedTargetVital(int index)
        {
            if (_pinedVitals.IsValid())
            {
                if (_pinedVitals.Count > index)
                {
                    return _pinedVitals[index];
                }
                else
                {
                    return _pinedVitals[index % _pinedVitals.Count];
                }
            }

            return null;
        }

        // 발사체 목표 바이탈 (Projectile`s Target Vital)

        private Vital GetTargetVital(int projectileIndex)
        {
            if (PinTargetVitalsOnShoot)
            {
                Vital pinedTarget = GetPinedTargetVital(projectileIndex);
                if (pinedTarget != null)
                {
                    return pinedTarget;
                }
            }

            if (TargetVital != null)
            {
                return TargetVital;
            }

            if (Owner != null && Owner.TargetCharacter != null)
            {
                return Owner.TargetCharacter.MyVital;
            }

            LogWarning("타겟 바이탈을 설정할 수 없습니다. Projectile Index: {0}", projectileIndex);
            return null;
        }
    }
}