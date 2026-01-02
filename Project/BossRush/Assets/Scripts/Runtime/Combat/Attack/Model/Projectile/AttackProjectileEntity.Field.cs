using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TeamSuneat.Projectiles;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackProjectileEntity : AttackEntity
    {
        //------------------------------------------------------------------------------------

        [FoldoutGroup("#Projectile")]
        public GameObject Prefab;

        [FoldoutGroup("#Projectile")]
        public GameObject[] Prefabs;

        [FoldoutGroup("#Projectile")]
        public bool UseShuffle;

        private readonly Deck<GameObject> _prefabDeck = new();

        [FoldoutGroup("#Projectile")]
        [SuffixLabel("비활성화시 사격 중지 무시")]
        public bool IgnoreStopShootingOnDeactivate;

        [FoldoutGroup("#Projectile")]
        [SuffixLabel("비활성화시 생성한 발사체 삭제")]
        public bool DespawnOnDeactivate;

        [FoldoutGroup("#Projectile")]
        [SuffixLabel("피격시 사격 중지")]
        public bool CanStopShootingOnDamage;

        [FoldoutGroup("#Projectile")]
        [SuffixLabel("사망시 사격 중지")]
        public bool CanStopShootingOnDeath = true;

        [FoldoutGroup("#Projectile")]
        [SuffixLabel("사망시 생성한 발사체 삭제")]
        public bool DespawnOnDeath;

        //------------------------------------------------------------------------------------

        [ReadOnly]
        [FoldoutGroup("#Projectile-Collider")]
        public Vector2 ProjectileColliderSize;

        [ReadOnly]
        [FoldoutGroup("#Projectile-Collider")]
        public Vector2 ProjectileColliderOffset;

        //------------------------------------------------------------------------------------

        [FoldoutGroup("#Projectile-Component")]
        [SuffixLabel("발사체 안내선 표시")]
        public ProjectileGuideLine GuideLine;

        [FoldoutGroup("#Projectile-Component")]
        [SuffixLabel("발사체 타겟 탐지")]
        public DetectSystem DetectSystem;

        [FoldoutGroup("#Projectile-Component")]
        [SuffixLabel("사격 시점에 목표 바이탈 고정")]
        public bool PinTargetVitalsOnShoot;

        [FoldoutGroup("#Projectile-Component")]
        [SuffixLabel("사격 시점에 목표 바이탈 고정 실패시 사격 중지")]
        public bool StopShootingOnFailedPinVital;

        #region 고정 (Pin)

        [FoldoutGroup("#Projectile-Pin")]
        [InfoBox("고정되는 값(생성 위치, 목표 방향, 목표 등)을 사격 지연 시간 전에 고정합니다. \n발사 지연 시간을 설정하면 해당 값을 설정할 수 있습니다.")]
        [SuffixLabel("지연 전 고정*")]
        [DisableIf("DelayTimeShooting", 0f)]
        public bool PinBeforeDelay;

        [FoldoutGroup("#Projectile-Pin")]
        [SuffixLabel("공격 시점에 생성 위치 고정")]
        public bool PinSpawnPositionOnShoot;

        [FoldoutGroup("#Projectile-Pin")]
        [SuffixLabel("공격 시점에 발사체 방향 고정")]
        public bool PinDirectionOnShoot;

        [FoldoutGroup("#Projectile-Pin")]
        [SuffixLabel("공격 시점에 목표 방향 고정")]
        public bool PinTargetPositionOnShoot;

        [FoldoutGroup("#Projectile-Pin")]
        [SuffixLabel("공격 종료까지 생성 위치를 한번만 설정할 지 여부")]
        public bool PinSpawnPositionForShootEnd;

        #endregion 고정 (Pin)

        #region 생성 (Spawn)

        [FoldoutGroup("#Projectile-Spawn")]
        [InfoBox("$SpawnPositionTypeMassage")]
        [SuffixLabel("생성 위치 종류")]
        public ProjectileSpawnTypes SpawnPositionType;

        [FoldoutGroup("#String", 99)]
        public string SpawnPositionTypeString;

        [NonSerialized]
        public string SpawnPositionTypeMassage;

        //------------------------------------------------------------------------------------------------------------------------------

        [FoldoutGroup("#Projectile-Spawn/PositionGroup")]
        [SuffixLabel("생성 위치 부모 그룹 사용")]
        public bool UseSpawnPositionGroup;

        [FoldoutGroup("#Projectile-Spawn/PositionGroup")]
        [SuffixLabel("생성 위치 후보 부모 그룹")]
        public ParentPositionGroup ParentSpawnPositionGroup;

        [FoldoutGroup("#Projectile-Spawn/PositionGroup")]
        [InfoBox("생성 위치 후보군이 설정된 포지션 그룹입니다. 그룹 내 위치 중 하나를 선택하여 발사체 위치를 설정합니다.")]
        [SuffixLabel("생성 위치 후보 그룹")]
        public PositionGroup PositionGroup;

        [FoldoutGroup("#Projectile-Spawn/PositionGroup")]
        [InfoBox("생성 위치 후보 그룹을 오너 캐릭터의 타겟 캐릭터 위치로 이동합니다.")]
        [SuffixLabel("생성 위치 후보 그룹 이동(타겟)")]
        public bool UseMovePositionGroupToTarget;

        [FoldoutGroup("#Projectile-Spawn/PositionGroup")]
        [InfoBox("생성 위치 후보 그룹을 오너 캐릭터의 타겟 캐릭터과 가장 가까운 지면 위치로 이동합니다.")]
        [SuffixLabel("생성 위치 후보 그룹 이동(타겟 지면)")]
        public bool UseMovePositionGroupToTargetGround;

        [FoldoutGroup("#Projectile-Spawn/PositionGroup")]
        [SuffixLabel("생성 위치 오프셋")]
        public Vector2 SpawnPositionOffset;

        [FoldoutGroup("#Projectile-Spawn/PositionGroup")]
        [DisableIf("IntervalPerShot", 0)]
        [InfoBox("사격 간격 시간마다 설정된 포지션 그룹을 재설정합니다. 포지션 그룹이 덱 정렬일 경우 덱을 섞거나 재구성합니다.")]
        [SuffixLabel("라운드 별 포지션 그룹 재설정")]
        public bool UseSetupPositionGroupPerRound;

        //------------------------------------------------------------------------------------------------------------------------------

        [FoldoutGroup("#Projectile-Spawn")]
        [InfoBox("생성 위치에 추가될 무작위 오프셋의 범위를 설정합니다.")]
        [SuffixLabel("생성 위치의 무작위 범위*")]
        public Vector2 AddRandomRangeOfSpawnPosition;

        [FoldoutGroup("#Projectile-Spawn/Ground")]
        [InfoBox("원하는 위치에서부터 아래로 땅을 검사합니다. GroundCheckDistance보다 멀리 땅이 위치한다면, 생성하지 않습니다.")]
        [SuffixLabel("땅 위에서만 생성*")]
        public bool SpawnOnlyOnGround;

        [FoldoutGroup("#Projectile-Spawn/Ground")]
        [EnableIf("SpawnOnlyOnGround")]
        [SuffixLabel("땅 위 여부 검사 거리")]
        public float GroundCheckDistance;

        [FoldoutGroup("#Projectile-Spawn/Ignore")]
        [InfoBox("미리 생성한 발사체의 위치가 일정 영역 내라면 발사체를 생성하지 않습니다.")]
        [SuffixLabel("영역 내 발사체 미생성")]
        public bool NoSpawnInDisntance;

        [FoldoutGroup("#Projectile-Spawn/Ignore")]
        [EnableIf("NoSpawnInDisntance")]
        [SuffixLabel("발사체 미생성 영역")]
        public float NoSpawnDisntance;

        [FoldoutGroup("#Projectile-Spawn/Ignore")]
        [SuffixLabel("타겟을 탐지하지 못했을 때 생성 무시")]
        public bool SkipSpawnOnTargetNotDetected;

        [FoldoutGroup("#Projectile-Spawn")]
        [SuffixLabel("지정된 최대 발사체 수만큼 생성")]
        public int MaxProjectileCount;

        [FoldoutGroup("#Projectile-Spawn")]
        [InfoBox("최대 발사체 수를 설정하는 능력치 이름입니다. 캐릭터의 능력치에 따라 최대 발사체 수가 늘어납니다.")]
        [SuffixLabel("지정된 최대 발사체 수만큼 생성 능력치 이름*")]
        public StatNames StatNameOfByMaxProjectileCount;

        [FoldoutGroup("#String", 99)]
        public string StatNameOfByMaxProjectileCountString;

        public enum ReactsReachMaxCount
        {
            None,               // 최대 발사체 수에 도달했을 때 아무런 반응을 하지 않음
            ExpireAndDestroy,   // 최대 발사체 수에 도달했을 때 기존 발사체를 만료시키고 파괴
            ResetTimeAndReuse,  // 최대 발사체 수에 도달했을 때 시간을 초기화하고 재사용
        }

        [FoldoutGroup("#Projectile-Spawn")]
        [SuffixLabel("지정된 최대 발사체 수에 도달했을 때 반응")]
        public ReactsReachMaxCount ReactReachMaxCount;

        public Character AttackMonster { get; set; }

        #endregion 생성 (Spawn)

        #region 시간 (Times)

        [FoldoutGroup("#Projectile-Times")]
        [InfoBox("한 번의 입력으로 발사하는 횟수를 설정합니다.")]
        [SuffixLabel("명령당 발사 횟수*")]
        public int ProjectilesPerShot = 1;

        [FoldoutGroup("#Projectile-Times")]
        [InfoBox("발사 횟수를 추가하는 능력치 이름입니다. 캐릭터의 능력치에 따라 발사 횟수가 늘어납니다.")]
        [SuffixLabel("추가 발사 횟수 능력치 이름*")]
        public StatNames StatNameOfExtraShots;

        [FoldoutGroup("#Projectile-Times")]
        [InfoBox("발사 횟수를 변경하는 능력치 이름입니다. 캐릭터의 능력치에 따라 발사 횟수가 변경됩니다.")]
        [SuffixLabel("능력치에 설정된 발사체 횟수로 변경*")]
        public StatNames StatNameOfPerShot;

        [FoldoutGroup("#String", 99)]
        public string StatNameOfExtraShotsString;

        [FoldoutGroup("#String", 99)]
        public string StatNameOfPerShotString;

        [FoldoutGroup("#Projectile-Times")]
        [InfoBox("한 번의 입력으로 사격이 시작될 때 일정 시간 발사를 대기합니다.")]
        [SuffixLabel("발사 지연 시간*")]
        public float DelayTimeShooting;

        [FoldoutGroup("#Projectile-Times")]
        [InfoBox("생성되는 발사체 사이의 시간을 설정합니다.")]
        [SuffixLabel("발사 간격 시간")]
        public float IntervalPerShot;

        [FoldoutGroup("#Projectile-Times")]
        [InfoBox("발사당 생성되는 발사체의 개수를 설정합니다. ")]
        [SuffixLabel("발사당 생성되는 발사체 수")]
        public int ShotSpread = 1;

        [FoldoutGroup("#Projectile-Times")]
        [SuffixLabel("취소 후 다시 날라가기까지의 지연시간")]
        public float CancelProjectilesWaitTime;

        [FoldoutGroup("#Projectile-Times")]
        [InfoBox("지정된 묶음 사격 발사 횟수를 모두 발사하면 발사 간격시간이 아닌 묶음 사격 간격 시간을 적용하여 대기합니다.")]
        [SuffixLabel("묶음 사격 사용*")]
        public bool UseGroupShoot;

        [FoldoutGroup("#Projectile-Times")]
        [EnableIf("UseGroupShoot")]
        [SuffixLabel("묶음 사격 발사 횟수")]
        public int GroupShotCount;

        [FoldoutGroup("#Projectile-Times")]
        [EnableIf("UseGroupShoot")]
        [SuffixLabel("묶음 사격 발사 간격 시간")]
        public float GroupShootInterval;

        [FoldoutGroup("#Projectile-Times")]
        [InfoBox("발사 마다 회전 Z 값의 변화량입니다.")]
        [SuffixLabel("발사 마다 회전 Z값 *")]
        public float AddRotationZPerShot;

        #endregion 시간 (Times)

        #region 방향 (Direction)

        [FoldoutGroup("#Projectile-Direction")]
        [InfoBox("$ProjectileDirectionMassage")]
        public ProjectileDirectionTypes ProjectileDirection;

        [FoldoutGroup("#String", 99)]
        public string ProjectileDirectionString;

        [NonSerialized]
        public string ProjectileDirectionMassage;

        [FoldoutGroup("#Projectile-Direction")]
        [SuffixLabel("방향 위치를 설정한 그룹")]
        public PositionGroup DirectionPositionGroup;

        [FoldoutGroup("#Projectile-Direction")]
        [SuffixLabel("방향을 설정할 때 오너의 방향을 무시")]
        [InfoBox("이 변수는 현재 Bidirectional에만 적용됩니다.")]
        public bool IgnoreOwnerFacingDirection;

        #endregion 방향 (Direction)

        #region 움직임 (Motion)

        [FoldoutGroup("#Projectile-Motion")]
        [InfoBox("$ProjectileMotionMassage")]
        public ProjectileMotionTypes ProjectileMotion;

        [FoldoutGroup("#String", 99)]
        public string ProjectileMotionString;

        [NonSerialized]
        public string ProjectileMotionMassage;

        [FoldoutGroup("#Projectile-Motion/TargetDirection", true)]
        [EnableIf("ProjectileMotion", ProjectileMotionTypes.TargetDirection)]
        [InfoBox("발사체 또는 소유자의 방향과 타겟과의 방향이 다를 때 방향을 조정하지 않습니다.")]
        public bool IgnoreTargetFacing;

        [FoldoutGroup("#Projectile-Motion/DetectTargetOrLinear", true)]
        [EnableIf("ProjectileMotion", ProjectileMotionTypes.DetectTargetOrLinear)]
        [SuffixLabel("탐지 목표 중복 가능 여부")]
        public bool CanOverlapDetectTarget;

        [FoldoutGroup("#Projectile-Motion/Angle", true)]
        [EnableIf("$IsShowAngle")]
        [SuffixLabel("발사체 생성 개수에 따른 자동 각도 조정")]
        public bool IsAutoAngleByTimes;

        [FoldoutGroup("#Projectile-Motion/Angle", true)]
        [EnableIf("$IsShowAngle")]
        public float AutoAngleMax = 90;

        [FoldoutGroup("#Projectile-Motion/Angle", true)]
        [EnableIf("$IsShowAngle")]
        public float AutoAngleMin = -90;

        [FoldoutGroup("#Projectile-Motion/Angle", true)]
        [EnableIf("$IsShowAngle")]
        public float Angle;

        [FoldoutGroup("#Projectile-Motion/Angle", true)]
        [EnableIf("$IsShowAngle")]
        public float AddAngle;

        [FoldoutGroup("#Projectile-Motion/Angle", true)]
        [EnableIf("$IsShowAngle")]
        public float RandomAngleArea;

        #endregion 움직임 (Motion)

        #region 대면 (Facing)

        [FoldoutGroup("#Projectile-Facing")]
        [SuffixLabel("발사체의 대면 방향 설정 방식")]
        public ProjectileFacingDirectionTypes FacingDirectionType = ProjectileFacingDirectionTypes.FollowOwner;

        #endregion 대면 (Facing)

        #region 목표 설정 (Target)

        public enum TargetSettings
        {
            PointOfTarget,              // 타겟의 현재 위치를 목표로 설정
            GroundPointOfTarget,        // 타겟의 지면 위치를 목표로 설정
            LastGroundPointOfTarget,    // 타겟의 마지막 지면 위치를 목표로 설정
            PointOfPositionGroup,       // 포지션 그룹의 위치를 목표로 설정
        }

        [FoldoutGroup("#Projectile-Target")]
        [SuffixLabel("타겟 위치 설정 방식")]
        public TargetSettings TargetSetting;

        [FoldoutGroup("#Projectile-Target")]
        [SuffixLabel("타겟 위치를 무작위로 섞습니다.")]
        public bool UseShuffleForTargetPositions;

        [FoldoutGroup("#Projectile-Target")]
        [SuffixLabel("타겟 위치에 추가될 무작위 오프셋의 범위를 설정합니다.")]
        public Vector2 AddRandomRangeOfTargetPosition;

        [FoldoutGroup("#Projectile-Target")]
        [SuffixLabel("타겟 위치에 추가될 고정 오프셋을 설정합니다.")]
        public Vector2 TargetPositionOffset;

        [FoldoutGroup("#Projectile-Target")]
        [SuffixLabel("타겟 위치에 추가될 포지션 그룹")]
        public PositionGroup TargetPositionGroup;

        #endregion 목표 설정 (Target)

        #region 전투 자원 (Vital Resource)

        [FoldoutGroup("#Projectile-Resource")]
        public VitalResourceInequalities ResourceInequalities;

        #endregion 전투 자원 (Vital Resource)

        /// <summary>
        /// '기본 발사체 수'
        /// : 메서드 내부에서 계산된 추가 발사체 수와 함께 사용되고, 결과적으로 실제 발사체의 총 수를 구하는 데 사용됩니다.
        /// </summary>
        private int _baseProjectilesPerShot;

        /// <summary>
        /// 활성 상태인 발사체
        /// </summary>
        private List<Projectile> _activeProjectiles = new();

        /// <summary>
        /// 현재 활성화된 코루틴 리스트
        /// </summary>
        private List<Coroutine> _activeShootingCoroutines = new();

        // 생성 위치 오프셋

        private float _minRangeXOfSpawnPositionX;
        private float _maxRangeXOfSpawnPositionX;

        // 사격 시 고정된 (Pined)

        private List<Vector3> _pinedSpawnPositions = new List<Vector3>();
        private List<Vector3> _pinedDirections = new List<Vector3>();
        private List<Vector3> _pinedTargetPositions = new List<Vector3>();
        private List<Vital> _pinedVitals = new List<Vital>();

        //

        private int _crisscrossCount = 0;
        private float _addRotationZPerShot = 0;

        //

        public Vector3 StartPosition { get; set; }

        public Vector3 AttackEnterTargetPoint { get; set; }

        private StatSystem OwnerStat
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.Stat;
                }
                else if (OwnerProjectile != null)
                {
                    if (OwnerProjectile.Owner != null)
                    {
                        return OwnerProjectile.Owner.Stat;
                    }
                }

                LogWarning("주인 발사체의 주인 캐릭터를 찾을 수 없습니다. {0}", this.GetHierarchyPath());
                return null;
            }
        }
    }
}