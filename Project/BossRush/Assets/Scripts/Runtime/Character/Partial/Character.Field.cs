using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;
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

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Component-Feedbacks")]
        [SuffixLabel("전투 시작 완료 피드백")]
        public GameFeedbacks OnBattleFeedbacks;

        [FoldoutGroup("#Character/Component-Feedbacks")]
        [SuffixLabel("레벨 업 피드백")]
        public GameFeedbacks OnLevelUpFeedbacks;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

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


        [FoldoutGroup("#Character/Target")]
        public LayerMask TargetMask;

        // ───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Character/Point")]
        public Transform HeadPoint;
        [FoldoutGroup("#Character/Point")]
        public Transform BodyPoint;
        [FoldoutGroup("#Character/Point")]
        public Transform FootPoint;

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
    }
}