using Sirenix.OdinInspector;
using System;
using UnityEngine;
using static TeamSuneat.Feedbacks.GameFeedbacks;
using static TeamSuneat.ObjectPaddingInOrder;

namespace TeamSuneat.Data
{
    [Serializable]
    public partial class ProjectileAssetData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [GUIColor("GetProjectileNameColor")]
        public ProjectileNames Name;
        public ProjectileTypes Type;

        ///

        public HitmarkNames Hitmark; // 공격히트마크
        public HitmarkNames AnotherHitmark; // 추가히트마크
        public HitmarkNames ReturnHitmark; // 반환히트마크

        ///

        public LayerMask DamageLayer;
        public LayerMask EraseLayer;
        public LayerMask CollisionLayer;

        public GameTags CollisionTag;
        public GameTags[] IgnoreCollisionTag;

        public int CollisionCount;
        public string DamageMask;
        public string EraseMask;
        public string CollisionMask;

        ///

        public bool GroundCheck;
        public float GroundCheckDistance;
        public bool GroundCheckOnSpawn;
        public bool UseOwnerPointForGroundCheck;

        ///

        public bool ForceReturn; // 강제반환
        public bool IsAttackOnSpawn; // 생성시 공격
        public bool IsAttackOnHit; // 충돌시 공격

        ///

        public bool IsThrough; // 관통여부
        public int ThroughMaxCount; // 관통횟수

        ///

        ///

        public bool IsFirstChain; // 연쇄발사체

        public bool IsAnotherChain; // 연쇄발사체

        public RegisterTargetTypes RegisterTargetType; // 타겟등록방식

        public int ChainCount; // 다음발사체개수

        ///

        public bool DeactivePhysicsOnDestroy; // 파괴시 물리비활성화

        public bool DeactivePhysicsOnDespawn; // 디스폰시 물리비활성화

        public bool IsIgnoreParrying; // 패링 무시

        ///

        public float Speed; // 속도

        public string AddSpeed_; // 추가 속도

        public float[] AddSpeed; // 추가 속도

        public float Acceleration; // 가속도

        public float DelayTime; // 지연시간

        public float HealthTime; // 지속시간

        public float AddHealthTime; // 추가 지속시간

        public float Distance; // 이동거리

        public float ArrivalDistance; // 도착 거리

        ///

        public ProjectileResults TimeResult; // 최대 지속시간 이후

        public ProjectileResults DistanceResult; // 최대 이동거리 이후

        public ProjectileResults ArrivalResult; // 타겟에게 도착한 이후

        public ProjectileResults DamageResult; // 피해 레이어 충돌 이후

        public ProjectileResults EraseResult; // 제거 레이어 충돌 이후

        public ProjectileResults CollisionResult; // 충돌 레이어 충돌 이후

        public ProjectileResults LandingResult; // 착지 레이어 충돌 이후

        public ProjectileResults InAirResult; // 착지 레이어 충돌 이후

        public ProjectileResults AttackResult; // 공격 이후

        ///

        public bool IsLinkedToOwner;

        ///

        public DetectTypes DetectType; // 탐지

        public bool IgnoreDetectSelf; // 자기자신 탐지여부

        public TargetSelections HomingTarget; // 멀티타겟여부

        public float DetectAngle; // 탐지각도

        public DetectAreaTypes DetectAreaType; // 탐지영역타입

        public float DetectDistance; // 탐지거리

        public float DetectMinDistance;

        public string DetectAreaOffset_;

        public string DetectBoxSize_;

        public Vector2 DetectAreaOffset; // 탐지영역 위치

        public Vector2 DetectBoxSize; // 탐지박스영역 크기

        public CharacterTypes[] DetectCharacter; // 탐지캐릭터

        public string DetectCharacter_;

        public DetectPriorityTypes DetectPriority;

        ///

        ///

        public ProjectileLaunchTypes LaunchType; // 발사타입

        public int Times; // 연사수

        public string AddTimes_; // 연사수

        public int[] AddTimes; // 연사수

        public float Interval; // 연사간격

        public int CountAtTime; // 산탄 수

        public string AddCountAtTime_; // 레벨별 추가 산탄수

        public int[] AddCountAtTime; // 레벨별 추가 산탄수

        ///

        public ProjectileSpawnOffsets SpawnOffset; // 생성위치타입

        public float BoxWidth; // 생성박스너비

        public float BoxHeight; // 생성박스높이

        public ObjectAlignments PointAlignments; // 발사지점 정렬

        public float PointsInterval; // 발사지점간격

        public Directions RaycastDirection;

        public float RaycastDistance;

        ///

        public float Angle; // 발사각도

        public float MaxAngle; // 최대발사각도

        public float SymmetryAngle; // 대칭각도

        public float AngleInterval; // 발사각도간격

        public int AngleIntervalCount;

        public AngleApplications AngleApplication;

        ///

        ///

        public ProjectileMotionTypes MotionType; // 이동방식

        public ProjectileMotionTypes MotionType2; // 이동방식2

        public float GravityRate; // 중력배율
        public float RotateMinSpeed; // 회전속도
        public float RotateMaxSpeed; // 회전속도

        public ProjectileHomingDirections HomingDirection;

        ///

        public string PrefabName; // 프리펩이름
        public string SpawnVFX;

        ///

        public Vector2 RigidForce;
        public Vector2 RigidMaxForce;

        [FoldoutGroup("#String")]
        public string NameAsString;
    }
}