using TeamSuneat.Projectiles;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackProjectileEntity : AttackEntity
    {
        private Vector3 GetDirectionByType(Vector3 spawnPosition, int index)
        {
            switch (ProjectileDirection)
            {
                case ProjectileDirectionTypes.None:
                    {
                        LogProgress("발사체의 목표 방향을 설정하지 않습니다. {0}", ProjectileDirection.ToLogString());
                    }
                    return Vector3.zero;

                case ProjectileDirectionTypes.Facing:
                    return GetFacingDirection();

                case ProjectileDirectionTypes.FacingReverse:
                    return GetFacingDirection().FlipX();

                case ProjectileDirectionTypes.Cone:
                    return GetConeDirection(index);

                case ProjectileDirectionTypes.RandomCone:
                    return GetRandomConeDirection(index);

                case ProjectileDirectionTypes.Bidirectional:
                    return GetBidirectionalDirection(index);

                case ProjectileDirectionTypes.TargetDirection:
                    return GetTargetDirectionWithAngleAdjustment(spawnPosition, index);

                case ProjectileDirectionTypes.TargetBoomerang:
                    return GetTargetBoomerangDirection(spawnPosition);

                case ProjectileDirectionTypes.AllSpawnToActivateTarget:
                    return GetTargetShortestDirection(spawnPosition);

                case ProjectileDirectionTypes.DetectTargetOrLinear:
                    return GetDirectionOfDetectTargetOrLinear(spawnPosition, index);

                case ProjectileDirectionTypes.HorizontalTrack:
                    return GetHorizontalTrackDirection(spawnPosition);

                case ProjectileDirectionTypes.PointDirection:
                    return GetPointDirection(spawnPosition);

                case ProjectileDirectionTypes.CustomDirection:
                    return GetCustomDirection();

                default:
                    {
                        LogError("발사체의 목표 방향을 설정할 수 없습니다. {0}", ProjectileDirection.ToLogString());
                    }
                    return Vector3.zero;
            }
        }

        private Vector3 GetFacingDirection()
        {
            if (OwnerProjectile != null)
            {
                if (OwnerProjectile.IsDirectionRight) { return transform.right; }
                else { return -transform.right; }
            }
            else if (Owner != null)
            {
                if (Owner.IsFacingRight) { return transform.right; }
                else { return -transform.right; }
            }
            else
            {
                return Vector3.zero;
            }
        }

        private bool GetFacingRight(out bool isFacingRight)
        {
            switch (FacingDirectionType)
            {
                case ProjectileFacingDirectionTypes.FollowOwner:
                    {
                        if (Owner != null)
                        {
                            isFacingRight = Owner.IsFacingRight;
                            return true;
                        }
                    }
                    break;

                case ProjectileFacingDirectionTypes.FollowParent:
                    {
                        if (Owner != null && OwnerProjectile != null)
                        {
                            if (IsSpawnPositionInProjectileAreaX())
                            {
                                LogProgress("부모 발사체의 실제 이동 방향을 기준이어도, 시전자와 발사체와의 거리가 너무 가깝다면 시전자의 방향에 따라 대면 방향을 설정합니다. IsFacingRight:{0}",
                                    Owner.IsFacingRight.ToBoolString());

                                isFacingRight = Owner.IsFacingRight;
                                return true;
                            }
                            // 부모 발사체의 실제 이동 방향을 기준으로 대면 방향을 설정
                            isFacingRight = OwnerProjectile.IsDirectionRight;
                            LogProgress("부모 발사체의 실제 이동 방향을 기준으로 대면 방향을 설정합니다. IsDirectionRight: {0}", isFacingRight.ToBoolString());
                            return true;
                        }
                        else if (OwnerProjectile != null)
                        {
                            // 부모 발사체의 실제 이동 방향을 기준으로 대면 방향을 설정
                            isFacingRight = OwnerProjectile.IsDirectionRight;
                            LogProgress("부모 발사체의 실제 이동 방향을 기준으로 대면 방향을 설정합니다. IsDirectionRight: {0}", isFacingRight.ToBoolString());
                            return true;
                        }
                        else
                        {
                            LogError("부모 발사체가 없습니다. 공격 수평 방향을 따를 수 없습니다.");
                        }
                    }
                    break;

                case ProjectileFacingDirectionTypes.Random:
                    {
                        isFacingRight = TSRandomEx.GetBoolValue();
                        return true;
                    }
            }
            isFacingRight = true; // default
            return false;
        }

        private bool IsSpawnPositionInProjectileAreaX()
        {
            if (Owner == null || OwnerProjectile == null || OwnerProjectile.ProjectileCollider == null)
                return false;

            // 발사체의 충돌체에서 직접 x축 범위 계산
            Collider2D collider = OwnerProjectile.ProjectileCollider;
            float projectileX = OwnerProjectile.transform.position.x;
            float colliderWidth = collider.bounds.size.x;

            // 발사체의 x축 범위 계산
            float projectileMinX = projectileX - (colliderWidth / 2);
            float projectileMaxX = projectileX + (colliderWidth / 2);

            // 발사체 생성 x 위치
            float spawnPositionX = position.x;

            // 시전자가 발사체의 x축 범위 안에 있는지 확인
            return spawnPositionX >= projectileMinX && spawnPositionX <= projectileMaxX;
        }

        private bool IsSpawnPositionInProjectileArea()
        {
            if (OwnerProjectile == null || OwnerProjectile.ProjectileCollider == null)
                return false;

            // 발사체의 충돌체 영역 계산
            Collider2D collider = OwnerProjectile.ProjectileCollider;
            Bounds bounds = collider.bounds;

            // 부모 발사체의 생성 위치가 현재 발사체의 충돌체 영역 안에 있는지 확인
            return bounds.Contains(OwnerProjectile.SpawnPosition);
        }

        private Vector3 GetLinearDirection()
        {
            return GetFacingDirection();
        }

        private Vector3 GetOscillatingDirection(Vector3 spawnPosition, int index)
        {
            if (DirectionPositionGroup != null)
            {
                Vector3 targetPosition = DirectionPositionGroup.GetPosition(spawnPosition, index);
                return (targetPosition - spawnPosition).normalized;
            }
            else
            {
                return GetFacingDirection();
            }
        }

        private Vector3 GetBidirectionalDirection(int index)
        {
            bool facingByIndex = index % 2 == 0;

            if (IgnoreOwnerFacingDirection)
            {
                if (facingByIndex) { return transform.right; }
                else { return -transform.right; }
            }
            else if (OwnerProjectile != null)
            {
                if (facingByIndex == OwnerProjectile.IsDirectionRight) { return transform.right; }
                else { return -transform.right; }
            }
            else if (Owner != null)
            {
                if (facingByIndex == Owner.IsFacingRight) { return transform.right; }
                else { return -transform.right; }
            }
            else
            {
                return Vector3.zero;
            }
        }

        private Vector3 GetConeDirection(int index)
        {
            // 기본 중앙 각도
            float finalAngle = Angle;
            int projectileCount = _baseProjectilesPerShot * ShotSpread;

            if (IsAutoAngleByTimes && projectileCount > 1)
            {
                // ProjectilesPerShot이 2발 이상이면 index에 따라 최소~최대 각도를 균등하게 배분
                finalAngle = Mathf.Lerp(AutoAngleMin, AutoAngleMax, (float)index / (projectileCount - 1));
            }
            else
            {
                // 자동 각도 조정 비활성화 또는 발사체가 1발일 경우 기존 방식 사용
                finalAngle = Angle + (AddAngle * index);
            }

            // 부모 발사체의 방향을 기준으로 각도 적용
            Vector3 baseDirection = Vector3.zero;
            if (OwnerProjectile != null)
            {
                // 시전자가 발사체 영역 내에 있는지 확인
                if (IsSpawnPositionInProjectileArea())
                {
                    // 시전자의 방향에 따라 방향 설정
                    baseDirection = Owner.IsFacingRight ? Vector3.right : Vector3.left;
                }
                else
                {
                    // 부모 발사체의 방향을 기준으로 사용
                    baseDirection = OwnerProjectile.Direction;
                }
            }
            else if (Owner != null)
            {
                // 부모 발사체가 없으면 시전자의 방향을 기준으로 사용
                baseDirection = Owner.IsFacingRight ? transform.right : -transform.right;
            }
            else
            {
                // 둘 다 없으면 기본 방향 사용
                baseDirection = transform.right;
            }

            // 기준 방향의 각도를 구함
            float baseAngle = TSAngleEx.ToAngle(baseDirection);

            // 기준 각도에 추가 각도를 적용
            float finalAngleWithBase = baseAngle + finalAngle;

            // 새로운 각도로 방향 벡터 생성
            return TSAngleEx.ToVector3(finalAngleWithBase);
        }

        private Vector3 GetRandomConeDirection(int index)
        {
            // 부모 발사체의 방향을 기준으로 각도 범위 설정
            Vector3 baseDirection = Vector3.zero;
            if (OwnerProjectile != null)
            {
                if (IsSpawnPositionInProjectileArea())
                {
                    // 시전자의 방향에 따라 방향 설정
                    baseDirection = Owner.IsFacingRight ? Vector3.right : Vector3.left;
                }
                else
                {
                    // 부모 발사체의 방향을 기준으로 사용
                    baseDirection = OwnerProjectile.Direction;
                }
            }
            else if (Owner != null)
            {
                // 부모 발사체가 없으면 시전자의 방향을 기준으로 사용
                baseDirection = Owner.IsFacingRight ? transform.right : -transform.right;
            }
            else
            {
                // 둘 다 없으면 기본 방향 사용
                baseDirection = transform.right;
            }

            // 기준 방향의 각도를 구함
            float baseAngle = TSAngleEx.ToAngle(baseDirection);

            // 기준 각도에 최소/최대 각도를 적용
            float minAngle = baseAngle - Angle - (AddAngle * index);
            float maxAngle = baseAngle + Angle + (AddAngle * index);

            // 랜덤 각도 생성
            float randomAngle = TSRandomEx.Range(minAngle, maxAngle);

            // 새로운 각도로 방향 벡터 생성
            return TSAngleEx.ToVector3(randomAngle);
        }

        private Vector3 GetTargetDirectionWithAngleAdjustment(Vector3 spawnPosition, int index)
        {
            Transform target = GetTargetTransform();
            if (target != null)
            {
                // 기존의 타겟 방향 계산 로직
                Vector3 targetDirection = GetTargetDirection(target, spawnPosition);

                if (RandomAngleArea > 0)
                {
                    float angle = TSAngleEx.ToAngle(targetDirection);
                    float addAngle = TSRandomEx.Range(-RandomAngleArea, RandomAngleArea);
                    return TSAngleEx.ToVector3(angle + addAngle);
                }
                if (AddAngle > 0)
                {
                    float angle = TSAngleEx.ToAngle(targetDirection) + Angle;
                    float addAngle = AddAngle * (index % ShotSpread);
                    return TSAngleEx.ToVector3(angle + addAngle);
                }
                return targetDirection;
            }
            return Vector3.zero;
        }

        private Vector3 GetTargetBoomerangDirection(Vector3 spawnPosition)
        {
            Transform target = GetTargetTransform();
            if (target != null)
            {
                return (AttackEnterTargetPoint != Vector3.zero)
                    ? (AttackEnterTargetPoint - spawnPosition).normalized
                    : (target.position - spawnPosition).normalized;
            }
            return Vector3.zero;
        }

        private Vector3 GetTargetShortestDirection(Vector3 spawnPosition)
        {
            Transform target = GetTargetTransform();
            return target != null ? (target.position - spawnPosition).normalized : Vector3.zero;
        }

        private Vector3 GetHorizontalTrackDirection(Vector3 spawnPosition)
        {
            Transform target = GetTargetTransform();
            return target != null ? GetTargetDirection(target, spawnPosition) : Vector3.zero;
        }

        private Vector3 GetPointDirection(Vector3 spawnPosition)
        {
            if (DirectionPositionGroup != null)
            {
                Vector3 directionPosition = DirectionPositionGroup.GetPosition(spawnPosition);
                return (directionPosition - spawnPosition).normalized;
            }
            return Vector3.zero;
        }

        private Vector3 GetCustomDirection()
        {
            Vector3 direction = TSAngleEx.ToVector3(Angle);
            if (OwnerProjectile != null)
            {
                if (OwnerProjectile.IsDirectionRight) { return direction; }
                else { return direction.FlipX(); }
            }
            else if (Owner != null)
            {
                if (Owner.IsFacingRight) { return direction; }
                else { return direction.FlipX(); }
            }

            return direction;
        }

        private Vector3 GetDirectionOfDetectTargetOrLinear(Vector3 spawnPosition, int index)
        {
            Transform target = GetDetectTarget(index);
            if (target != null)
            {
                Vector3 direction = target.position - spawnPosition;
                return direction.normalized;
            }
            else
            {
                return GetLinearDirection();
            }
        }

        private Transform GetDetectTarget(int index)
        {
            if (_pinedVitals != null)
            {
                Collider2D vitalCollider = null;
                if (CanOverlapDetectTarget && _pinedVitals.Count > 0)
                {
                    int indexInRange = index % _pinedVitals.Count;
                    vitalCollider = _pinedVitals[indexInRange].GetNotGuardCollider();
                }
                else if (!CanOverlapDetectTarget && _pinedVitals.Count > index)
                {
                    vitalCollider = _pinedVitals[index].GetNotGuardCollider();
                }

                if (vitalCollider != null)
                {
                    return vitalCollider.transform;
                }
            }

            return Owner.Target;
        }

        //----------------------------------------------------------------------------------------------------------------------------

        private void SetDirectionByMotion(Projectile projectile, int index)
        {
            if (TrySetDirectionByPin(projectile, index)) { return; }

            switch (ProjectileDirection)
            {
                case ProjectileDirectionTypes.None:
                    break;

                case ProjectileDirectionTypes.TargetPoisitonOfProjectile:
                    {
                        SetTargetPoisitonOfProjectileDirection(projectile);
                    }
                    break;

                case ProjectileDirectionTypes.AllSpawnToActivateTarget:
                    {
                        SetAllSpawnToActivateTargetDirection(projectile, index);
                    }
                    break;

                case ProjectileDirectionTypes.CustomTargetDirection:
                    {
                        SetCustomTargetDirection(projectile);
                    }
                    break;

                case ProjectileDirectionTypes.Crisscross:
                    {
                        SetCrisscrossDirection(projectile, index);
                    }
                    break;

                case ProjectileDirectionTypes.Follow:
                    {
                        SetFollowDirection(projectile);
                    }
                    break;

                default:
                    {
                        Vector3 direction = GetDirectionByType(projectile.position, index);
                        projectile.SetDirectionExtern(direction);
                    }
                    break;
            }
        }

        private bool TrySetDirectionByPin(Projectile projectile, int index)
        {
            if (PinDirectionOnShoot)
            {
                if (_pinedDirections.IsValid(index))
                {
                    projectile.SetDirectionExtern(_pinedDirections[index]);
                    return true;
                }
            }

            return false;
        }

        private void SetTargetPoisitonOfProjectileDirection(Projectile projectile)
        {
            projectile.position = projectile.TargetPoisiton;
        }

        private void SetCustomTargetDirection(Projectile projectile)
        {
            Vector3 direction = (projectile.TargetPoisiton - projectile.position).normalized;
            projectile.SetDirectionExtern(direction);
        }

        private void SetAllSpawnToActivateTargetDirection(Projectile projectile, int index)
        {
            if (Owner.Target != null)
            {
                Vector3 direction = GetDirectionByType(projectile.position, index);
                projectile.SetDirectionExtern(direction);
                projectile.StopMovement();
            }
            else
            {
                projectile.SetDirectionExtern(Vector3.zero);
            }
        }

        private void SetCrisscrossDirection(Projectile projectile, int index)
        {
            Vector3 direction = Vector3.zero;
            switch (_crisscrossCount)
            {
                case 0: { projectile.SetDirectionExtern((transform.rotation * Vector3.right).normalized); } break;
                case 1: { projectile.SetDirectionExtern((transform.rotation * Vector3.left).normalized); } break;
                case 2: { projectile.SetDirectionExtern((transform.rotation * Vector3.up).normalized); } break;
                case 3: { projectile.SetDirectionExtern((transform.rotation * Vector3.down).normalized); } break;
            }

            if (_crisscrossCount >= 3) { _crisscrossCount = 0; }
            else { _crisscrossCount += 1; }
        }

        private void SetFollowDirection(Projectile projectile)
        {
            if (projectile is FollowProjectile)
            {
                FollowProjectile followProjectile = projectile as FollowProjectile;
                followProjectile.FollowTarget = transform;
            }
        }
    }
}