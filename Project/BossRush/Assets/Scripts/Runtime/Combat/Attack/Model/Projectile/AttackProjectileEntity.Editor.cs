using System.Collections.Generic;
using Sirenix.OdinInspector;
using TeamSuneat.Projectiles;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackProjectileEntity : AttackEntity
    {
#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (NoSpawnInDisntance)
            {
                TSGizmoEx.DrawWireSphere(position, NoSpawnDisntance, TSColors.Dev);
                TSGizmoEx.DrawText("No Spawn Disntance", position + Vector3.down * NoSpawnDisntance, TSColors.Dev);
            }
        }

        protected override void Validate()
        {
            base.Validate();

            EnumEx.ConvertTo(ref StatNameOfByMaxProjectileCount, StatNameOfByMaxProjectileCountString);
            EnumEx.ConvertTo(ref StatNameOfExtraShots, StatNameOfExtraShotsString);
            EnumEx.ConvertTo(ref StatNameOfPerShot, StatNameOfPerShotString);
            EnumEx.ConvertTo(ref SpawnPositionType, SpawnPositionTypeString);
            if (!EnumEx.ConvertTo(ref ProjectileDirection, ProjectileDirectionString))
            {
                Log.Error("발사체의 방향을 갱신하지 못합니다. {0}, {1}", ProjectileDirectionString, this.GetHierarchyPath());
            }

            EnumEx.ConvertTo(ref ProjectileMotion, ProjectileMotionString);

            RefreshSpawnPositionTypeMassage();
            RefreshProjectileDirectionMassage();
            RefreshProjectileMotionMassage();
            LoadProjectileColliderInfo();

            // ValidateMotions();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            if (GuideLine == null)
            {
                GuideLine = GetComponentInChildren<ProjectileGuideLine>();
            }

            if (DetectSystem == null)
            {
                DetectSystem = GetComponentInChildren<DetectSystem>();
            }
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (StatNameOfByMaxProjectileCount != StatNames.None)
            {
                StatNameOfByMaxProjectileCountString = StatNameOfByMaxProjectileCount.ToString();
            }
            if (StatNameOfExtraShots != StatNames.None)
            {
                StatNameOfExtraShotsString = StatNameOfExtraShots.ToString();
            }
            if (StatNameOfPerShot != StatNames.None)
            {
                StatNameOfPerShotString = StatNameOfPerShot.ToString();
            }
            if (SpawnPositionType != ProjectileSpawnTypes.None)
            {
                SpawnPositionTypeString = SpawnPositionType.ToString();
            }
            if (ProjectileMotion != ProjectileMotionTypes.None)
            {
                ProjectileMotionString = ProjectileMotion.ToString();
            }
            if (ProjectileDirection != ProjectileDirectionTypes.None)
            {
                ProjectileDirectionString = ProjectileDirection.ToString();
            }
        }

        private void RefreshSpawnPositionTypeMassage()
        {
            switch (SpawnPositionType)
            {
                case ProjectileSpawnTypes.Parent:
                    {
                        SpawnPositionTypeMassage = " 부모 설정: 발사체를 생성하는 게임 오브젝트의 위치를 따릅니다";
                    }
                    break;

                case ProjectileSpawnTypes.ParentCharacter:
                    {
                        SpawnPositionTypeMassage = "부모 설정: 발사체를 생성하는 캐릭터의 위치를 따릅니다";
                    }
                    break;

                case ProjectileSpawnTypes.Point:
                    {
                        SpawnPositionTypeMassage = "지점: 발사체를 생성하는 게임 오브젝트 또는 포지션 그룹의 지정 위치에 생성합니다";
                    }
                    break;

                case ProjectileSpawnTypes.PointGround:
                    {
                        SpawnPositionTypeMassage = "지점 지면: 발사체를 생성하는 게임 오브젝트 또는 포지션 그룹의 지정 위치과 가장 가까운 지면에 생성합니다";
                    }
                    break;

                case ProjectileSpawnTypes.Target:
                    {
                        SpawnPositionTypeMassage = "타겟: 타겟으로 삼은 캐릭터의 위치에서 생성합니다";
                    }
                    break;

                case ProjectileSpawnTypes.OwnerGround:
                    {
                        SpawnPositionTypeMassage = "시전자 지면: 시전자의 지면 위에서 생성합니다.";
                    }
                    break;

                case ProjectileSpawnTypes.TargetGround:
                    {
                        SpawnPositionTypeMassage = "타겟 캐릭터 지면: 타겟 캐릭터의 지면 위에서 생성합니다.";
                    }
                    break;

                case ProjectileSpawnTypes.RandomGround:
                    {
                        SpawnPositionTypeMassage = "무작위 지면: 발사체를 생성하는 게임 오브젝트의 위치를 기준으로 설정된 영역 내 무작위 지면 위치에서 생성합니다";
                    }
                    break;

                case ProjectileSpawnTypes.AttackMonster:
                    {
                        SpawnPositionTypeMassage = "몬스터의 공격 위치에서 생성합니다.";
                    }
                    break;
            }
        }

        private void RefreshProjectileDirectionMassage()
        {
            switch (ProjectileDirection)
            {
                case ProjectileDirectionTypes.None:
                    ProjectileDirectionMassage = "발사체의 방향을 설정하지 않습니다.";
                    break;

                case ProjectileDirectionTypes.Facing:
                    { ProjectileDirectionMassage = "바라봄 : 공격 캐릭터가 바라보는 수평 방향으로 이동합니다."; }
                    break;

                case ProjectileDirectionTypes.FacingReverse:
                    { ProjectileDirectionMassage = "반대 바라봄: 공격 캐릭터가 바라보는 반대 수평 방향으로 이동합니다."; }
                    break;

                case ProjectileDirectionTypes.Cone:
                    { ProjectileDirectionMassage = "원뿔형: 원뿔형으로 방사됩니다."; }
                    break;

                case ProjectileDirectionTypes.TargetDirection:
                    { ProjectileDirectionMassage = "목표 방향: 생성 시 타겟 캐릭터가 위치한 지점을 기준으로 방향 설정하여 이동합니다."; }
                    break;

                case ProjectileDirectionTypes.Bidirectional:
                    { ProjectileDirectionMassage = "양방향: 오너 캐릭터가 오른쪽을 바라보는 기준으로 짝수는 오른쪽, 홀수는 왼쪽으로 이동합니다. 왼쪽을 바라보고있다면 반대 방향으로 이동합니다."; }
                    break;

                case ProjectileDirectionTypes.TargetBoomerang:
                    { ProjectileDirectionMassage = "목표 부메랑: 목표 캐릭터의 위치로 이동합니다. 이동이 끝나면 다시 되돌아옵니다."; }
                    break;

                case ProjectileDirectionTypes.AllSpawnToActivateTarget:
                    { ProjectileDirectionMassage = "생성 후 목표 이동: 모든 발사체가 생성 후 목표 캐릭터 위치로 이동합니다."; }
                    break;

                case ProjectileDirectionTypes.PointDirection:
                    { ProjectileDirectionMassage = "지점 방향: 지정된 위치로 이동합니다."; }
                    break;

                case ProjectileDirectionTypes.CustomDirection:
                    { ProjectileDirectionMassage = "지정 방향: 지정된 방향으로 이동합니다."; }
                    break;

                case ProjectileDirectionTypes.RandomCone:
                    { ProjectileDirectionMassage = "무작위 원뿔형: 지정된 원뿔형 방향 안에서 무작위로 이동합니다."; }
                    break;

                case ProjectileDirectionTypes.HorizontalTrack:
                    { ProjectileDirectionMassage = "수평 추격: 적을 찾아 수평으로 추격합니다."; }
                    break;

                case ProjectileDirectionTypes.DetectTargetOrLinear:
                    { ProjectileDirectionMassage = "탐지한 적에게 날아가거나: 직선 방향으로 날아갑니다."; }
                    break;

                default:
                    ProjectileDirectionMassage = ProjectileDirection.ToString();
                    break;
            }
        }

        private void RefreshProjectileMotionMassage()
        {
            switch (ProjectileMotion)
            {
                case ProjectileMotionTypes.Point:
                    ProjectileMotionMassage = "지점: 지정 위치에서 이동하지 않습니다.";
                    break;

                case ProjectileMotionTypes.Linear:
                    { ProjectileMotionMassage = "단방향: 공격 캐릭터가 바라보는 수평 방향으로 이동합니다."; }
                    break;

                case ProjectileMotionTypes.LinearGravity:
                    { ProjectileMotionMassage = "단방향: 공격 캐릭터가 바라보는 수평 방향으로 이동합니다. 중력의 영향을 받습니다."; }
                    break;

                case ProjectileMotionTypes.Oscillating:
                    { ProjectileMotionMassage = "수직 진동: 정해진 방향으로 이동과 함께 일정한 주기로 위아래로 움직이는 발사체입니다. 공격 캐릭터가 바라보는 방향에 영향을 받습니다. "; }
                    break;

                case ProjectileMotionTypes.Homing:
                    { ProjectileMotionMassage = "유도: 타겟 캐릭터를 따라 이동합니다."; }
                    break;

                case ProjectileMotionTypes.Cone:
                    { ProjectileMotionMassage = "원뿔형: 원뿔형으로 방사됩니다."; }
                    break;

                case ProjectileMotionTypes.TargetDirection:
                    { ProjectileMotionMassage = "목표 방향: 생성 시 타겟 캐릭터가 위치한 지점을 기준으로 방향 설정하여 이동합니다."; }
                    break;

                case ProjectileMotionTypes.Bidirectional:
                    { ProjectileMotionMassage = "양방향: 오너 캐릭터가 오른쪽을 바라보는 기준으로 짝수는 오른쪽, 홀수는 왼쪽으로 이동합니다. 왼쪽을 바라보고있다면 반대 방향으로 이동합니다."; }
                    break;

                case ProjectileMotionTypes.Physics:
                    { ProjectileMotionMassage = "물리: 발사체가 중력을 가집니다. 방향과 힘을 설정하여 발사체를 던집니다."; }
                    break;

                case ProjectileMotionTypes.Parabola:
                    { ProjectileMotionMassage = "포물선: 목표 위치를 설정하면 포물선을 그리며 도착합니다."; }
                    break;

                case ProjectileMotionTypes.AreaPoint:
                    { ProjectileMotionMassage = "영역: 지정 위치에서 지정된 영역 안에 무작위 위치에서 생성합니다. 이동하지 않습니다."; }
                    break;

                case ProjectileMotionTypes.TargetBoomerang:
                    { ProjectileMotionMassage = "목표 부메랑: 목표 캐릭터의 위치로 이동합니다. 이동이 끝나면 다시 되돌아옵니다."; }
                    break;

                case ProjectileMotionTypes.HorizontalBoomerang:
                    { ProjectileMotionMassage = "수평방향 부메랑: 시전자가 바라보는 수평 방향으로 던지고, 돌아옵니다."; }
                    break;

                case ProjectileMotionTypes.AllSpawnToActivateTarget:
                    { ProjectileMotionMassage = "생성 후 목표 이동: 모든 발사체가 생성 후 목표 캐릭터 위치로 이동합니다."; }
                    break;

                case ProjectileMotionTypes.PointDirection:
                    { ProjectileMotionMassage = "지점 방향: 지정된 위치로 이동합니다."; }
                    break;

                case ProjectileMotionTypes.CustomDirection:
                    { ProjectileMotionMassage = "지정 방향: 지정된 방향으로 이동합니다."; }
                    break;

                case ProjectileMotionTypes.RandomCone:
                    { ProjectileMotionMassage = "무작위 원뿔형: 지정된 원뿔형 방향 안에서 무작위로 이동합니다."; }
                    break;

                case ProjectileMotionTypes.HorizontalTrack:
                    { ProjectileMotionMassage = "수평 추격: 적을 찾아 수평으로 추격합니다."; }
                    break;

                case ProjectileMotionTypes.CustomTargetDirection:
                    { ProjectileMotionMassage = "미리 지정된 타겟 방향으로 날아갑니다."; }
                    break;

                case ProjectileMotionTypes.Crisscross:
                    { ProjectileMotionMassage = "십자형으로 발사합니다."; }
                    break;

                case ProjectileMotionTypes.HorizontalPatrol:
                    { ProjectileMotionMassage = "수평으로 순찰합니다."; }
                    break;

                case ProjectileMotionTypes.Orbit:
                    { ProjectileMotionMassage = "궤도를 가지고 돕니다. "; }
                    break;

                case ProjectileMotionTypes.Follow:
                    { ProjectileMotionMassage = "특정 위치를 따라다닙니다. "; }
                    break;

                case ProjectileMotionTypes.DetectTargetOrLinear:
                    { ProjectileMotionMassage = "탐지한 적에게 날아가거나: 직선 방향으로 날아갑니다."; }
                    break;

                default:
                    ProjectileMotionMassage = ProjectileMotion.ToString();
                    break;
            }
        }

        private void LoadProjectileColliderInfo()
        {
            if (Prefab != null)
            {
                BoxCollider2D boxCoolider = Prefab.GetComponent<BoxCollider2D>();
                if (boxCoolider != null)
                {
                    ProjectileColliderSize = boxCoolider.size;
                    ProjectileColliderOffset = boxCoolider.offset;
                }
                else
                {
                    CircleCollider2D circleCollider = Prefab.GetComponent<CircleCollider2D>();
                    if (circleCollider != null)
                    {
                        ProjectileColliderSize = Vector2.one * circleCollider.radius;
                        ProjectileColliderOffset = circleCollider.offset;
                    }
                    else
                    {
                        CapsuleCollider2D capsuleCollider = Prefab.GetComponent<CapsuleCollider2D>();
                        if (capsuleCollider != null)
                        {
                            ProjectileColliderSize = capsuleCollider.size;
                            ProjectileColliderOffset = capsuleCollider.offset;
                        }
                    }
                }
            }

            if (Prefabs.IsValidArray())
            {
                BoxCollider2D boxCoolider = Prefabs[0].GetComponent<BoxCollider2D>();
                if (boxCoolider != null)
                {
                    ProjectileColliderSize = boxCoolider.size;
                    ProjectileColliderOffset = boxCoolider.offset;
                }
                else
                {
                    CircleCollider2D circleCollider = Prefabs[0].GetComponent<CircleCollider2D>();
                    if (circleCollider != null)
                    {
                        ProjectileColliderSize = Vector2.one * circleCollider.radius;
                        ProjectileColliderOffset = circleCollider.offset;
                    }
                    else
                    {
                        CapsuleCollider2D capsuleCollider = Prefabs[0].GetComponent<CapsuleCollider2D>();
                        if (capsuleCollider != null)
                        {
                            ProjectileColliderSize = capsuleCollider.size;
                            ProjectileColliderOffset = capsuleCollider.offset;
                        }
                    }
                }
            }
        }

        private void ValidateMotions()
        {
            if (Prefab != null)
            {
                Projectile projectile = Prefab.GetComponent<Projectile>();
                if (projectile.Motion != ProjectileMotion)
                {
                    Log.Error("모션이 다른 발사체입니다. {0}!={1}, {2}", ProjectileMotion.ToLogString(), projectile.Motion.ToLogString(), this.GetHierarchyPath());
                }
                else
                {
                    Log.Progress("모션이 같은 발사체입니다. {0}", this.GetHierarchyPath());
                }
            }
        }

        #region Field

        private bool IsShowAngle()
        {
            switch (ProjectileMotion)
            {
                case ProjectileMotionTypes.CustomDirection:
                case ProjectileMotionTypes.Cone:
                case ProjectileMotionTypes.RandomCone:
                    return true;
            }

            return false;
        }

        #endregion Field

        #region Buttons

        [FoldoutGroup("#Custom Buttons", 1000)]
        [Button(ButtonSizes.Medium)]
        private void ValidateMotion()
        {
            if (Prefab != null)
            {
                Projectile projectile = Prefab.GetComponent<Projectile>();
                LogValidateMotion(projectile);
            }
            if (Prefabs != null)
            {
                foreach (GameObject prefab in Prefabs)
                {
                    Projectile projectile = prefab.GetComponent<Projectile>();
                    LogValidateMotion(projectile);
                }
            }
        }

        private void LogValidateMotion(Projectile projectile)
        {
            if (projectile.Motion != ProjectileMotion)
            {
                if (projectile.Motion == ProjectileMotionTypes.None)
                {
                    LogWarning("{0}, 생성하는 발사체({1})의 MOTION이 일치하지 않습니다. ENTITY`S: {2}, PROJECTILE`S: {3}", Name.ToLogString(), projectile.Name.ToLogString(), ProjectileMotion.ToLogString(), projectile.Motion.ToLogString());
                }
                else
                {
                    Log.Error("{0}, 생성하는 발사체({1})의 MOTION이 일치하지 않습니다. ENTITY`S: {2}, PROJECTILE`S: {3}", Name.ToLogString(), projectile.Name.ToLogString(), ProjectileMotion.ToLogString(), projectile.Motion.ToLogString());
                }
            }
            else
            {
                LogProgress("생성하는 발사체({0})의 MOTION이 올바르게 설정되어있습니다. MOTION: {1}", projectile.Name.ToLogString(), ProjectileMotion.ToLogString());
            }
        }

        [FoldoutGroup("#Custom Buttons", 1000)]
        [Button("캐릭터가 사용하는 발사체 정보 확인", ButtonSizes.Medium)]
        private void CheckNotUseMotionByAttackProjectiles()
        {
            List<Character> characters = LoadPrefabsWithComponent<Character>("Assets/Resources/Prefabs/Character", null);
            foreach (var character in characters)
            {
                AttackProjectileEntity[] entities = character.GetComponentsInChildren<AttackProjectileEntity>();
                foreach (AttackProjectileEntity entity in entities)
                {
                    Log.Info("SPAWN: {0}, MOTION: {1}, PATH: {2}",
                        entity.SpawnPositionType.ToValueString(), entity.ProjectileMotion.ToLogString(), entity.GetHierarchyPath());

                    if (!entity.UseSetupPositionGroupPerRound && entity.IntervalPerShot > 0 && entity.ShotSpread > 1)
                    {
                        Log.Error("{0}, 간격 시간:{1}, 한 번에 생성하는 발사체 수:{2}, 간격마다 포지션 그룹 재설정: {3}",
                            entity.Name.ToLogString(), entity.IntervalPerShot, entity.ShotSpread, entity.UseSetupPositionGroupPerRound.ToBoolString());
                    }
                }
            }
        }

        [FoldoutGroup("#Custom Buttons", 1000)]
        [Button("플래그를 사용하는 발사체 공격 독립체 정보 확인", ButtonSizes.Medium)]
        private void CheckFlagProjectiles()
        {
            List<Character> characters = LoadPrefabsWithComponent<Character>("Assets/Resources/Prefabs/Character", null);
            foreach (Character character in characters)
            {
                AttackProjectileEntity[] entities = character.GetComponentsInChildren<AttackProjectileEntity>();
                if (!entities.IsValid()) continue;
                foreach (AttackProjectileEntity entity in entities)
                {
                    if (entity.Prefab == null)
                    {
                        Log.Error("발사체 생성 공격 독립체에 프리펩이 설정되지 않았습니다: {0}", entity.Name.ToLogString());
                        continue;
                    }

                    AttackProjectileEntity[] entitiesInProjectiles = entity.Prefab.GetComponentsInChildren<AttackProjectileEntity>();
                    if (!entitiesInProjectiles.IsValid()) continue;

                    foreach (AttackProjectileEntity entitiesInProjectile in entitiesInProjectiles)
                    {
                    }
                }
            }
        }

        [FoldoutGroup("#Custom Buttons", 1000)]
        [Button("발사체 방향(DirectionType) 정보 확인", ButtonSizes.Medium)]
        private void CheckProjectileDirection()
        {
            List<Character> characters = LoadPrefabsWithComponent<Character>("Assets/Resources/Prefabs/Character", null);
            foreach (Character character in characters)
            {
                AttackProjectileEntity[] entities = character.GetComponentsInChildren<AttackProjectileEntity>();
                if (!entities.IsValid()) continue;

                foreach (AttackProjectileEntity entity in entities)
                {
                    Log.Info("{0}가 생성한 발사체는 ({1}) 움직임 타입, ({2}) 방향 타입을 사용합니다.", entity.GetHierarchyPath(),
                                            entity.ProjectileMotion.ToLogString(),
                                            entity.ProjectileDirection.ToLogString());

                    if (entity.Prefab != null)
                    {
                        AttackProjectileEntity[] prefabEntities = entity.Prefab.GetComponentsInChildren<AttackProjectileEntity>();
                        if (!prefabEntities.IsValid()) continue;

                        foreach (AttackProjectileEntity prefabEntity in prefabEntities)
                        {
                            Log.Info("{0}가 생성한 발사체는 ({1}) 움직임 타입, ({2}) 방향 타입을 사용합니다.", prefabEntity.GetHierarchyPath(),
                                prefabEntity.ProjectileMotion.ToLogString(),
                                prefabEntity.ProjectileDirection.ToLogString());
                        }
                    }
                }
            }
        }

        #endregion Buttons

#endif
    }
}