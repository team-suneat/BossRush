using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    [RequireComponent(typeof(CollisionController))]
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class PhysicsController : XBehaviour
    {
        [ReadOnly]
        public Vector3 Velocity;

        [ReadOnly]
        public Character Owner;

        [ReadOnly]
        public CollisionController Controller;

        [ReadOnly]
        public Rigidbody2D Rigidbody;

        #region Values

        [FoldoutGroup("#Values")]
        public float DefaultMoveSpeed = 3;

        [FoldoutGroup("#Values")]
        public float MoveSpeedMultiplier = 1;

        [FoldoutGroup("#Values")]
        public float MoveSpeedResult;

        [FoldoutGroup("#Values")]
        public float JumpMinHeight;

        [FoldoutGroup("#Values")]
        public float JumpMaxHeight;

        [FoldoutGroup("#Values")]
        public float TimeToJumpApex;

        [FoldoutGroup("#Values")]
        [SerializeField]
        protected float m_velocityXSmoothing;

        [FoldoutGroup("#Values")]
        [SerializeField]
        protected float m_velocityYSmoothing;

        [FoldoutGroup("#Values")]
        [SerializeField]
        protected float m_smoothTime;

        [FoldoutGroup("#Values")]
        public float gravity;

        [FoldoutGroup("#Values")]
        public float maxJumpVelocity;

        [FoldoutGroup("#Values")]
        public float minJumpVelocity;

        [FoldoutGroup("#Values")]
        public int wallDirX;

        [FoldoutGroup("#Values")]
        public float timeToWallUnstick;

        [FoldoutGroup("#Values")]
        [SerializeField]
        protected Vector2 m_directionalInput;

        [FoldoutGroup("#Values")]
        public bool IsLockVelcotiyX;

        [FoldoutGroup("#Values")]
        public bool IsLockVelcotiyY;

        #endregion Values

        #region Following

        [FoldoutGroup("#Following")]
        public bool UseFollowing;

        [FoldoutGroup("#Following")]
        public bool UseOnceFollowing;

        [FoldoutGroup("#Following")]
        [ReadOnly] public Transform Following;

        [FoldoutGroup("#Following")]
        [ReadOnly] public Vector3 FollowPosition;

        [FoldoutGroup("#Following")]
        public float MinDistanceToOwner;

        [FoldoutGroup("#Following")]
        public float MinSpeedToOwner;

        [FoldoutGroup("#Following")]
        [ReadOnly] [SerializeField] protected float m_distanceBetweenFollowing;

        #endregion Following

        protected UnityAction OnceFollowCallback;

        [FoldoutGroup("#MovingPlatform")]
        public Vector3 AdditionalVelocity;

        [FoldoutGroup("#Landing")]
        public string LandingVFX;

        [FoldoutGroup("#Landing")]
        public Transform LandingPoint;

        protected float _higherHeight;

        [Title("#Force Velocity")]
        protected ForceVelocityMover FVMover;

        protected Vector3 _prevPosition;

        protected Coroutine _forceCoroutine;

        [Title("For Not Player")]
        [SuffixLabel("몬스터의 점프 가변높이")]
        public float customJumpVelocity;

        public UnityAction<int> ForceVelocityCompletedEvent; // ForceVelocityTID

        private bool _prevIsGrounded;

        private bool _isFalling;
    }
}