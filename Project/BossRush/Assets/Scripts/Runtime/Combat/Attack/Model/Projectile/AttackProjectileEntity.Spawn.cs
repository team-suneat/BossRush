using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackProjectileEntity : AttackEntity
    {
        private const float GROUND_RAYCAST_DISTANCE = 20f;
        private const float GROUND_OFFSET = 0.1f; // 발사체가 땅에 닿을 수 있도록 함

        // 발사체 생성 위치 (Spawn Position)

        /// <summary> 발사체 그룹이 설정되어 있으면 모든 설정을 진행합니다. </summary>
        private void SetupPositionGroupIfAvailable()
        {
            if (TryFindPositionGroupForSpawn())
            {
                if (ParentSpawnPositionGroup != null)
                {
                    ParentSpawnPositionGroup.ShuffleNow();
                }

                if (PositionGroup != null)
                {
                    if (!PinSpawnPositionForShootEnd)
                    {
                        PositionGroup.ClearPositionDeck(); // 발사체 위치 초기화입니다.
                    }

                    PositionGroup.SetupDeck(); // 발사체 그룹을 설정합니다.
                }
            }
        }

        private bool TryFindPositionGroupForSpawn()
        {
            // 이미 그룹이 설정되어 있으면 그냥 반환
            if (ParentSpawnPositionGroup != null || PositionGroup != null)
            {
                return true;
            }

            // 먼저 부모 그룹을 검색
            ParentSpawnPositionGroup = PositionGroupManager.Instance.FindParent(Name);
            if (ParentSpawnPositionGroup != null)
            {
                return true;
            }

            // 부모 그룹이 없으면 자식 그룹을 검색
            PositionGroup = PositionGroupManager.Instance.Find(Name);
            if (PositionGroup != null)
            {
                return true;
            }

            return false;
        }

        private bool TryFindPositionGroupForTarget()
        {
            if (TargetPositionGroup != null)
            {
                return true;
            }

            TargetPositionGroup = PositionGroupManager.Instance.Find(Name, PositionGroup.Types.Target);
            if (TargetPositionGroup != null)
            {
                return true;
            }

            return false;
        }

        private void SetRangeXOfSpawnPositionOffset()
        {
            if (SpawnPositionType is ProjectileSpawnTypes.OwnerGround or ProjectileSpawnTypes.RandomGround)
            {
                float minX = -AddRandomRangeOfSpawnPosition.x;
                float maxX = AddRandomRangeOfSpawnPosition.x;

                RaycastHit2D leftHit = Physics2D.Raycast(position, Vector2.left, AddRandomRangeOfSpawnPosition.x, TSLayers.Mask.Obstacles);
                if (leftHit)
                {
                    minX = position.x.GetDifference(leftHit.point.x) * -1;
                    minX += ProjectileColliderSize.x * 0.5f;
                }

                RaycastHit2D rightHit = Physics2D.Raycast(position, Vector2.right, AddRandomRangeOfSpawnPosition.x, TSLayers.Mask.Obstacles);
                if (rightHit)
                {
                    maxX = position.x.GetDifference(rightHit.point.x);
                    maxX -= ProjectileColliderSize.x * 0.5f;
                }

                _minRangeXOfSpawnPositionX = minX;
                _maxRangeXOfSpawnPositionX = maxX;

                TSDebugDrawEx.DrawLine(new Vector2(position.x + minX, position.y), new Vector2(position.x + maxX, position.y), TSColors.Dev, 3f);
            }
        }

        private void ShufflePositionGroupPerRound()
        {
            ParentSpawnPositionGroup?.ShuffleNow();
            PositionGroup?.SetupDeck(); // 발사체 그룹을 설정합니다.
        }

        #region 미리 등록된 발사체 생성 위치 (Pined Spawn Position)

        private void PinSpawnPositions(int projectileCount)
        {
            if (!PinSpawnPositionOnShoot)
            {
                return;
            }

            List<Vector3> spawnPositionsOfGroup = GetSpawnPositionsOfGroup(projectileCount);
            _pinedSpawnPositions.Clear();

            switch (SpawnPositionType)
            {
                case ProjectileSpawnTypes.Point:
                    PinSpawnPointPositions(projectileCount, spawnPositionsOfGroup);
                    break;

                case ProjectileSpawnTypes.PointGround:
                    PinSpawnPointGroundPositions(projectileCount, spawnPositionsOfGroup);
                    break;

                case ProjectileSpawnTypes.Target:
                    PinSpawnTargetPositions(projectileCount, spawnPositionsOfGroup);
                    break;

                case ProjectileSpawnTypes.TargetGround:
                    PinSpawnTargetGroundPositions(projectileCount, spawnPositionsOfGroup);
                    break;

                default:
                    PinSpawnDefaultPositions(projectileCount);
                    break;
            }
        }

        private void PinSpawnPointPositions(int projectileCount, List<Vector3> spawnPositionsOfGroup)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 offset = TSRandomEx.GetVector3Value(AddRandomRangeOfSpawnPosition) + (Vector3)SpawnPositionOffset;
                if (spawnPositionsOfGroup.IsValid(i))
                {
                    LogInfo("발사체의 생성 위치가 여기서 설정된 그룹의 위치로 설정됩니다.");
                    _pinedSpawnPositions.Add(spawnPositionsOfGroup[i] + offset);
                }
                else
                {
                    _pinedSpawnPositions.Add(position + offset);
                }
            }
        }

        private void PinSpawnPointGroundPositions(int projectileCount, List<Vector3> spawnPositionsOfGroup)
        {
            Vector3 spawnPosition;

            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 offset = TSRandomEx.GetVector3Value(AddRandomRangeOfSpawnPosition) + (Vector3)SpawnPositionOffset;
                if (spawnPositionsOfGroup.IsValid(i))
                {
                    spawnPosition = CalculateNearestGroundPosition(spawnPositionsOfGroup[i], offset);
                }
                else
                {
                    spawnPosition = CalculateNearestGroundPosition(position, offset);
                }

                _pinedSpawnPositions.Add(spawnPosition);
            }
        }

        private void PinSpawnTargetPositions(int projectileCount, List<Vector3> spawnPositionsOfGroup)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 offset = TSRandomEx.GetVector3Value(AddRandomRangeOfSpawnPosition) + (Vector3)SpawnPositionOffset;
                Vector3 targetPosition = position;
                if (ParentSpawnPositionGroup != null || PositionGroup != null)
                {
                    LogInfo("발사체의 생성 위치가 목표 캐릭터의 위치로 설정되어 있으므로 여기서 설정된 그룹의 위치로 설정됩니다.");
                    targetPosition = spawnPositionsOfGroup[i];
                }
                else
                {
                    Vital targetVital = GetTargetVital(i);
                    if (targetVital != null)
                    {
                        LogInfo("발사체의 생성 위치가 목표 캐릭터의 활성화된 충돌체 위치로 설정됩니다.");
                        Collider2D vitalCollider = targetVital.GetNotGuardCollider();
                        if (vitalCollider != null)
                        {
                            targetPosition = vitalCollider.transform.position;
                        }
                        else if (targetVital != null)
                        {
                            targetPosition = targetVital.position;
                        }

                        _pinedSpawnPositions.Add(targetPosition + offset);
                    }
                    else
                    {
                        LogWarning("발사체의 생성 위치가 목표의 위치로 설정되어 있으나 목표이 없어서 생성할 수 없습니다. 목표이 설정되지 않았습니다.");
                    }
                }
            }
        }

        private void PinSpawnTargetGroundPositions(int projectileCount, List<Vector3> spawnPositionsOfGroup)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 offset = TSRandomEx.GetVector3Value(AddRandomRangeOfSpawnPosition) + (Vector3)SpawnPositionOffset;

                Vital targetVital = GetTargetVital(i);
                if (targetVital != null && targetVital.Owner != null)
                {
                    if (PositionGroup != null)
                    {
                        LogInfo("발사체의 생성 위치가 목표 캐릭터의 위치로 설정되어 있으므로 여기서 설정된 그룹의 위치로 설정됩니다.");
                        _pinedSpawnPositions.Add(spawnPositionsOfGroup[i] + offset);
                    }
                    else
                    {
                        LogInfo("발사체의 생성 위치가 목표 캐릭터의 땅 위치로 설정됩니다.");
                        _pinedSpawnPositions.Add(targetVital.Owner.GroundPosition + offset);
                    }
                }
                else
                {
                    LogWarning("발사체의 생성 위치가 목표의 위치로 설정되어 있으나 목표이 없어서 생성할 수 없습니다. 목표이 설정되지 않았습니다.");
                }
            }
        }

        private void PinSpawnDefaultPositions(int projectileCount)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 offset = TSRandomEx.GetVector3Value(AddRandomRangeOfSpawnPosition) + (Vector3)SpawnPositionOffset;
                Vector3 spawnPosition = GetSpawnPosition(i);
                _pinedSpawnPositions.Add(spawnPosition + offset);
            }
        }

        #endregion 미리 등록된 발사체 생성 위치 (Pined Spawn Position)

        #region 매번마다 발사체 생성 위치 (Spawn Position On Per Shoot)

        private Vector3 GetSpawnPosition(int index)
        {
            if (PinSpawnPositionOnShoot)
            {
                if (_pinedSpawnPositions.IsValid(index))
                {
                    return _pinedSpawnPositions[index];
                }
                else
                {
                    LogWarning("미리 등록된 발사체 생성 위치가 설정되지 않았습니다. {0}번째 발사체를 생성할 수 없습니다.", index.ToSelectString());
                    return Vector3.zero;
                }
            }

            Vector3 offset = GetSpawnOffset();
            switch (SpawnPositionType)
            {
                case ProjectileSpawnTypes.Point:
                    return GetPointSpawnPosition(index, offset);

                case ProjectileSpawnTypes.PointGround:
                    return GetPointGroundSpawnPosition(index, offset);

                case ProjectileSpawnTypes.Target:
                    return GetTargetSpawnPosition(index, offset);

                case ProjectileSpawnTypes.AttackMonster:
                    return GetAttackMonsterSpawnPosition(offset);

                case ProjectileSpawnTypes.OwnerGround:
                    return GetOwnerGroundSpawnPosition(offset);

                case ProjectileSpawnTypes.TargetGround:
                    return GetTargetGroundSpawnPosition(index, offset);

                case ProjectileSpawnTypes.RandomGround:
                    return GetRandomGroundSpawnPosition(offset);

                default:
                    return position + TSRandomEx.GetVector3Value(AddRandomRangeOfSpawnPosition);
            }
        }

        private Vector3 GetSpawnOffset()
        {
            return TSRandomEx.GetVector3Value(AddRandomRangeOfSpawnPosition) + (Vector3)SpawnPositionOffset;
        }

        private Vector3 GetPointSpawnPosition(int index, Vector3 offset)
        {
            if (ParentSpawnPositionGroup != null)
            {
                LogInfo("발사체의 생성 위치가 부모에서 설정된 그룹의 위치로 설정됩니다.");
                // 필요시 부모 그룹 위치를 목표캐릭터로 이동시킴
                if (UseMovePositionGroupToTarget && Owner.TargetCharacter != null)
                {
                    ParentSpawnPositionGroup.position = Owner.TargetCharacter.position;
                }
                else if (UseMovePositionGroupToTargetGround && Owner.TargetCharacter != null)
                {
                    ParentSpawnPositionGroup.position = Owner.TargetCharacter.GroundPosition;
                }

                List<Vector3> positions = ParentSpawnPositionGroup.GetPositions(position);
                if (positions != null && positions.Count > index)
                {
                    return positions[index] + offset;
                }
            }
            else if (PositionGroup != null)
            {
                LogInfo("발사체의 생성 위치가 여기서 설정된 그룹의 위치로 설정됩니다.");
                if (UseMovePositionGroupToTarget && Owner.TargetCharacter != null)
                {
                    PositionGroup.position = Owner.TargetCharacter.position;
                }
                else if (UseMovePositionGroupToTargetGround && Owner.TargetCharacter != null)
                {
                    PositionGroup.position = Owner.TargetCharacter.GroundPosition;
                }
                return PositionGroup.GetPosition(position) + offset;
            }
            return position + offset;
        }

        private Vector3 GetPointGroundSpawnPosition(int index, Vector3 offset)
        {
            if (PositionGroup != null)
            {
                Vector3 pointPosition = PositionGroup.GetPosition(position);
                return CalculateNearestGroundPosition(pointPosition, offset);
            }
            else
            {
                return CalculateNearestGroundPosition(position, offset);
            }
        }

        private Vector3 GetTargetSpawnPosition(int index, Vector3 offset)
        {
            Vital targetVital = GetTargetVital(index);
            if (targetVital != null)
            {
                Collider2D vitalCollider = targetVital.GetNotGuardCollider();
                if (vitalCollider != null)
                {
                    LogInfo("발사체의 생성 위치가 목표 캐릭터의 활성화된 충돌체 위치로 설정됩니다.");
                    Vector3 targetPosition = vitalCollider.transform.position;
                    return targetPosition + offset;
                }
                else
                {
                    LogWarning("발사체의 생성 위치 타입이 '목표의 위치'로 설정되어 있으나 목표의 활성화된 충돌체 위치가 없습니다. 공격 독립체의 위치를 생성 위치로 설정합니다.");
                    return position;
                }
            }
            else if (!SkipSpawnOnTargetNotDetected)
            {
                LogWarning("발사체의 생성 위치 타입이 '목표의 위치'로 설정되어 있으나 목표 캐릭터를 찾을 수 없습니다. 공격 독립체의 위치를 생성 위치로 설정합니다.");
                return position + offset;
            }
            else
            {
                return Vector3.zero;
            }
        }

        private Vector3 GetAttackMonsterSpawnPosition(Vector3 offset)
        {
            if (AttackMonster != null)
            {
                return AttackMonster.position + offset;
            }
            return position + offset;
        }

        private Vector3 GetOwnerGroundSpawnPosition(Vector3 offset)
        {
            if (Owner != null)
            {
                Vector3 spawnPosition = CalculateNearestGroundPosition(Owner.FootPosition);
                if (!spawnPosition.IsZero())
                {
                    return spawnPosition + offset;
                }
                else
                {
                    LogWarning("캐릭터의 발 아래 땅 위치를 찾을 수 없습니다.");
                }
            }
            else
            {
                LogWarning("캐릭터를 찾을 수 없어서 캐릭터의 발 아래 땅 위치를 찾을 수 없습니다.");
            }
            return position + offset;
        }

        private Vector3 GetTargetGroundSpawnPosition(int index, Vector3 offset)
        {
            Vital targetVital = GetTargetVital(index);
            if (targetVital != null && targetVital.Owner != null)
            {
                if (targetVital.Owner.IsFlying)
                {
                    LogProgress("목표 캐릭터가 비행 캐릭터이므로 캐릭터의 발 위치를 설정합니다.");
                    return targetVital.Owner.FootPosition + offset;
                }
                else
                {
                    Vector3 spawnPosition = CalculateNearestGroundPosition(targetVital.Owner.FootPosition);
                    if (!spawnPosition.IsZero())
                    {
                        return spawnPosition + offset;
                    }
                    else
                    {
                        LogWarning("목표 캐릭터의 발 아래 땅 위치를 찾을 수 없습니다.");
                    }
                }
            }
            else if (!SkipSpawnOnTargetNotDetected)
            {
                if (Owner != null)
                {
                    Vector3 spawnPosition = CalculateNearestGroundPosition(Owner.FootPosition);
                    if (!spawnPosition.IsZero())
                    {
                        LogInfo("발사체의 생성 위치 타입이 '목표의 지면 위치'로 설정되어 있으나 목표 캐릭터를 찾을 수 없습니다. 오너 캐릭터의 가까운 지면 위치로 설정합니다.");
                        return spawnPosition + offset;
                    }

                    LogInfo("발사체의 생성 위치 타입이 '목표의 지면 위치'로 설정되어 있으나 목표 캐릭터를 찾을 수 없습니다. 공격 독립체의 위치를 생성 위치로 설정합니다.");
                    return position + offset;
                }
            }

            return Vector3.zero;
        }

        private Vector3 GetRandomGroundSpawnPosition(Vector3 offset)
        {
            Vector3 spawnPosition = CalculateNearestGroundPosition(position);
            if (!spawnPosition.IsZero())
            {
                return spawnPosition + offset;
            }
            else
            {
                LogWarning("발사체가 생성된 위치의 땅 위치를 찾을 수 없습니다.");
            }
            return position + offset;
        }

        #endregion 매번마다 발사체 생성 위치 (Spawn Position On Per Shoot)

        //

        private Vector3 CalculateNearestGroundPosition(Vector3 originPosition)
        {
            Vector3 offset = new(TSRandomEx.Range(_minRangeXOfSpawnPositionX, _maxRangeXOfSpawnPositionX), GROUND_OFFSET); // 발사체가 땅에 닿을 수 있도록 함
            RaycastHit2D downHit = Physics2D.Raycast(originPosition + offset, Vector2.down, GROUND_RAYCAST_DISTANCE, TSLayers.Mask.Obstacles);
            if (downHit)
            {
                TSDebugDrawEx.DrawLine(originPosition, downHit.point, TSColors.Dev, 3f);
                TSDebugDrawEx.DrawCross(downHit.point, 0.2f, TSColors.Dev, 3f);

                LogInfo("발사체의 위치에서 가장 가까운 땅 위치를 찾았습니다.");
                return (Vector3)downHit.point;
            }

            LogWarning("땅을 찾지 못해서 발사체를 생성할 수 없습니다.");
            return Vector3.zero;
        }

        private Vector3 CalculateNearestGroundPosition(Vector3 pointPosition, Vector3 offset)
        {
            RaycastHit2D hit = Physics2D.Raycast(pointPosition, Vector2.down, GROUND_RAYCAST_DISTANCE, TSLayers.Mask.Obstacles);
            if (hit)
            {
                LogInfo("발사체의 위치에서 가장 가까운 땅 위치를 찾았습니다.");
                return (Vector3)hit.point + offset;
            }
            else
            {
                LogWarning("땅을 찾지 못해서 발사체를 생성할 수 없습니다.");
                return Vector3.zero;
            }
        }

        //
        private List<Vector3> GetSpawnPositionsOfGroup(int projectileCount)
        {
            // 먼저 ParentSpawnPositionGroup을 확인하고, 없으면 PositionGroup을 사용합니다.
            if (ParentSpawnPositionGroup != null)
            {
                return GetSpawnPositionsFromParentGroup(projectileCount);
            }
            else if (PositionGroup != null)
            {
                return GetSpawnPositionsFromChildGroup(projectileCount);
            }
            return null;
        }

        /// <summary>
        /// ParentSpawnPositionGroup과 PositionGroup의 다른 GetPositions 함수는 projectileCount를 사용하여 반환하므로,
        /// 부모 그룹에서는 전체 발사체 수에 맞게 반환합니다.
        /// </summary>
        private List<Vector3> GetSpawnPositionsFromParentGroup(int projectileCount)
        {
            // 필요시 부모 그룹의 위치를 목표 캐릭터의 위치로 설정합니다.
            if (UseMovePositionGroupToTarget && Owner.TargetCharacter != null)
            {
                ParentSpawnPositionGroup.position = Owner.TargetCharacter.position;
            }
            else if (UseMovePositionGroupToTargetGround && Owner.TargetCharacter != null)
            {
                ParentSpawnPositionGroup.position = Owner.TargetCharacter.GroundPosition;
            }

            // ParentSpawnPositionGroup에서는 전체 발사체 수를 반환합니다.
            List<Vector3> positions = ParentSpawnPositionGroup.GetPositions(position);
            if (positions != null && projectileCount > 0 && projectileCount < positions.Count)
            {
                positions = positions.Take(projectileCount).ToList();
            }
            return positions;
        }

        /// <summary>
        /// PositionGroup은 그냥, GetPositions 함수를 projectileCount를 사용하여 바로 호출합니다.
        /// </summary>
        private List<Vector3> GetSpawnPositionsFromChildGroup(int projectileCount)
        {
            if (UseMovePositionGroupToTarget && Owner.TargetCharacter != null)
            {
                PositionGroup.position = Owner.TargetCharacter.position;
            }
            else if (UseMovePositionGroupToTargetGround && Owner.TargetCharacter != null)
            {
                PositionGroup.position = Owner.TargetCharacter.GroundPosition;
            }
            return PositionGroup.GetPositions(position, projectileCount);
        }
    }
}