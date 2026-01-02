using Sirenix.OdinInspector;
using TeamSuneat.Assets.Scripts.Runtime.Transform;
using TeamSuneat.Data;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class Projectile
    {
        [Title("#Projectile")]
        public ProjectileNames Name;
        public string NameString;

        public int Grade = 1;

        public int Level = 1;

        [FoldoutGroup("#Components")] public Character Owner;
        [FoldoutGroup("#Components")] public Vital Vital;
        [FoldoutGroup("#Components")] public ProjectileAnimator Animator;
        [FoldoutGroup("#Components")] public ProjectileRenderer Renderer;
        [FoldoutGroup("#Components")] public ProjectileDetector[] Detectors;
        [FoldoutGroup("#Components")] public ProjectileRotationController RotationController;
        [FoldoutGroup("#Components")] public FlipController FlipController;
        [FoldoutGroup("#Components")] public ProjectileAttackSystem AttackSystem;

        [FoldoutGroup("#Values")][SerializeField] protected LayerMask m_targetLayer;
        [FoldoutGroup("#Values")][SerializeField] protected LayerMask m_eraseLayer;
        [FoldoutGroup("#Values")][SerializeField] protected LayerMask m_collisionLayer;
        [FoldoutGroup("#Values")][ShowIf("ShowReadOnly")][ReadOnly] public XChainProjectile ChainProjectileInfo;
        [FoldoutGroup("#Values")][ShowIf("ShowReadOnly")][ReadOnly] public int DamageIndex;
        [FoldoutGroup("#Values/Positions")][SerializeField] protected Vector2 _spawnPosition;
        [FoldoutGroup("#Values/Positions")][SerializeField] protected Vector2 _destroyPosition;

        protected bool _isDestroy;
        protected bool _isDespawn;

        [FoldoutGroup("#Values/Bool")][ShowIf("ShowReadOnly")][ReadOnly][SerializeField] protected bool _isSpawnedAnother;
        [FoldoutGroup("#Values/Bool")][ShowIf("ShowReadOnly")][ReadOnly][SerializeField] protected bool _isSpawnedReturn;
        [FoldoutGroup("#Values/Bool")][ShowIf("ShowReadOnly")][ReadOnly][SerializeField] protected bool _isStopMoveInDelayTime;
        [FoldoutGroup("#Values/Bool")] public bool IsStopRotateInDelayTime;
        [FoldoutGroup("#Values/Bool")] public bool UseEntityRotation;
        [FoldoutGroup("#Values/Bool")] public bool IsDestroyOnTheSpotMove;

        protected bool _isExecutedTimeResult;
        protected bool _isExecutedDistanceResult;
        protected bool _useLiftTime;
        protected float _destroyTime;

        [FoldoutGroup("#Event")] public UnityEvent<Projectile> OnSpawnCallback;
        [FoldoutGroup("#Event")] public UnityEvent<Projectile> OnDespawnCallback;

        protected ProjectileAssetData _projectileData = new ProjectileAssetData();

        [FoldoutGroup("#Motions")][ShowIf("ShowReadOnly")][ReadOnly][SerializeField] protected ProjectileMotionTypes _motionType;

        [FoldoutGroup("#Motions")][SuffixLabel("Linear")][SerializeField] protected Vector2 _targetDirection;

        [FoldoutGroup("#Motions")][SuffixLabel("Easing")][SerializeField] protected Vector2 _targetPosition;

        [FoldoutGroup("#Motions")][SuffixLabel("Easing")][SerializeField] protected float _spawnTime;

        [FoldoutGroup("#Motions")][SuffixLabel("Physics")][ShowIf("ShowReadOnly")][ReadOnly][SerializeField] protected Vector2 _prevPosition;

        [FoldoutGroup("#Motions")][SuffixLabel("Homing")][ShowIf("ShowReadOnly")][ReadOnly][SerializeField] protected Vital _homingTarget;

        [FoldoutGroup("#Motions")][SuffixLabel("Homing")][ShowIf("ShowReadOnly")][ReadOnly] public float HomingRotateSpeed;

        [FoldoutGroup("#Motions")][SuffixLabel("Homing")][ShowIf("ShowReadOnly")][ReadOnly] public float RotateSpeed;

        [FoldoutGroup("#Motions")][SuffixLabel("Homing")][ShowIf("ShowReadOnly")][ReadOnly] public float HomingDirection = 1;

        protected Collider2D m_collider2D;

        protected BoxCollider2D m_boxCollider2D;

        protected CircleCollider2D m_circleCollider2D;

        [FoldoutGroup("#Physics")] public XRigidbody Body;

        [FoldoutGroup("#Physics")][ShowIf("ShowReadOnly")][ReadOnly][SerializeField] protected Vector3 _colliderBottom;

        [FoldoutGroup("#Physics")] public bool UseCheckWall;

        [FoldoutGroup("#Physics")][ShowIf("UseCheckWall")] public float CheckWallDistance;

        [FoldoutGroup("#Physics")] public int CollisionCount;

        private bool _facingRightAtLaunch;

        private Coroutine _despawnCoroutine;

        private IProjectileMover _mover;
    }
}