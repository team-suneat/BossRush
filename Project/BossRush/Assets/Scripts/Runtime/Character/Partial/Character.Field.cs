using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;
using TeamSuneat.Passive;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        public enum ControllerTypes
        { Player, AI }

        public enum FacingDirections
        { Left, Right }

        public enum SpawnFacingDirections
        { Left, Right }

        [FoldoutGroup("#Character")] public CharacterNames Name;
        [FoldoutGroup("#Character")] public string NameString;
        [FoldoutGroup("#Character")] public bool FixedTargetCharacterCamp;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public CharacterAnimator CharacterAnimator;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public CharacterRenderer CharacterRenderer;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public Animator Animator;

        [FoldoutGroup("#Character/Component")]
        [ChildGameObjectsOnly] public PhysicsController PhysicsController;

        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public AttackSystem Attack;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public BuffSystem Buff;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public PassiveSystem Passive;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public SkillSystem Skill;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public StatSystem Stat;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public Vital MyVital;
        [FoldoutGroup("#Character/Component")][ChildGameObjectsOnly] public CharacterAbility[] Abilities;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Abilities")]
        public CharacterHorizontalMovement HorizontalMovement;

        [FoldoutGroup("#Character/Abilities")]
        public CharacterFly Fly;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform ShieldPoint;
        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform WarningTextPoint;
        [FoldoutGroup("#Character/Component/Point")][ChildGameObjectsOnly] public Transform MinimapPoint;

        [FoldoutGroup("#Character/Component/Point")] public Vector2 BuffSpawnArea;
        [FoldoutGroup("#Character/Component/Point")] public bool UseCustomBuffVFXPosition;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Component-Feedbacks")]
        [SuffixLabel("전투 시작 완료 피드백")]
        public GameFeedbacks OnBattleFeedbacks;

        [FoldoutGroup("#Character/Component-Feedbacks")]
        [SuffixLabel("레벨 업 피드백")]
        public GameFeedbacks OnLevelUpFeedbacks;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Model")]
        [Tooltip("이 옵션은 일반 게임오브젝트와 달리 캐릭터가 SpriteRenderer가 캐릭터가 아닌 다른 게임오브젝트에 있는 경우 이를 자식으로 설정합니다. " +
            "그래서 캐릭터 캐릭터 전체를 회전시키고 아래로 떨어뜨립니다. 이는 3D 모델 캐릭터를 만들기 위함입니다. " +
            "이 안에 있는 스프라이트는 캐릭터가 아니고 다른 것이며, 이는 충돌을 피하기 위해 자식으로 설정된다는 의미입니다. " +
            "\n캐릭터를 회전시키는 것은 '모델'(캐릭터 본체가 아닌 시각적 오브젝트). 이렇게 하면 충돌을 피하기 위해 충돌/내부 콜라이더/스탯에서 분리(이 분리)합니다.")]
        public GameObject CharacterModel;

        [FoldoutGroup("#Character/Model")]
        [Tooltip("이 캐릭터의 카메라 타겟이 될 게임 오브젝트")]
        public GameObject CameraTarget;

        [FoldoutGroup("#Character/Model")]
        [Tooltip("카메라 타겟이 이동하는 속도")]
        public float CameraTargetSpeed = 5f;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Direction")]
        public SpawnFacingDirections DirectionOnStart = SpawnFacingDirections.Right;

        [FoldoutGroup("#Character/Direction")]
        [Tooltip("여기서 캐릭터가 스폰될 때 향해야 하는 방향을 강제할 수 있습니다. 기본값으로 설정하면 모델의 초기 방향과 일치합니다")]
        public SpawnFacingDirections DirectionOnSpawn = SpawnFacingDirections.Right;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Events")]
        [Tooltip("이 옵션이 true이면 캐릭터의 상태 변경 시 이벤트를 보내거나 받을 수 있는 이벤트를 활성화합니다")]
        public bool SendStateChangeEvents = true;

        [FoldoutGroup("#Character/Events")]
        [Tooltip("이 옵션이 true이면 상태 업데이트 이벤트에 추가 정보가 추가되고 이벤트를 활성화합니다")]
        public bool SendStateUpdateEvents = true;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/State Machine")]
        public StateMachine<MovementStates> MovementState;

        [FoldoutGroup("#Character/State Machine")]
        public StateMachine<CharacterConditions> ConditionState;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        protected Vector3 _raycastOrigin;
        protected RaycastHit2D _raycastHit2D;

        #region Animator Parameters

        protected const string ANIMATOR_IDLE_PARAMETER_NAME = "Idle";
        protected const string ANIMATOR_ALIVE_PARAMETER_NAME = "Alive";
        protected const string ANIMATOR_RANDOM_PARAMETER_NAME = "Random";
        protected const string ANIMATOR_RANDOM_CONSTANT_PARAMETER_NAME = "RandomConstant";

        protected int _idleSpeedAnimationParameter;
        protected int _aliveAnimationParameter;
        protected int _randomAnimationParameter;
        protected int _randomConstantAnimationParameter;

        #endregion Animator Parameters

        protected Color _initialColor;
        protected float _originalGravity;

        protected Vector3 _cameraOffset = Vector3.zero;

        protected float _animatorRandomNumber;
        protected CharacterConditions _conditionStateBeforeFreeze;
    }
}