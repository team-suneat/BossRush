using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class VFXObject : XBehaviour, IPoolable
    {
        [Title("VFXObject", "Component")]
        public AutoDespawn Despawner;

        public ParticleEffect2DGroup ParticleGroup;
        public VFXMover Mover;

        [Title("VFXObject", "Renderer")]
        public VFXAnimator Animator;

        public SpriteRenderer[] Renderers;

        [Title("VFXObject", "Values")]
        public bool OnlyOne;
        public bool HaveParent;
        [EnableIf("HaveParent")] public bool BreakParentPosition;
        [EnableIf("BreakParentPosition")] public float BreakDelayTime;

        [Title("VFXObject", "State Effect")]
        [SuffixLabel("상태이상 이펙트 반복 생성 여부")]
        public bool UseRepeatSpawnnOfStateEffect;

        [EnableIf("UseRepeatSpawnnOfStateEffect")]
        [SuffixLabel("상태이상 이펙트 생성 간격 시간")]
        public float SpawnIntervalOfStateEffect;

        public Character Owner { get; private set; }

        public UnityEvent<VFXObject> _onDespawnCallback;

        private Coroutine _coroutineDespawnCheck;
        private Vector3 _defaultLocalScale;

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Despawner = GetComponent<AutoDespawn>();
            Mover = GetComponent<VFXMover>();

            Animator = GetComponentInChildren<VFXAnimator>();
            Renderers = GetComponentsInChildren<SpriteRenderer>();
            ParticleGroup = GetComponent<ParticleEffect2DGroup>();
        }

        private void Awake()
        {
            _defaultLocalScale = transform.localScale;
        }

        public void OnSpawn()
        {
            // VFXManager.Register(this);
        }

        public void OnDespawn()
        {
            // VFXManager.Unregister(this);

            _coroutineDespawnCheck = null;
        }

        public void ForceDespawn()
        {
            Log.Info(LogTags.Effect, "{0}, 이펙트를 강제 삭제합니다.", this.GetHierarchyName());

            StopDespawnTimer();
            ResetDespawningState();
            Despawn();
        }

        public void InstantDespawn()
        {
            Log.Info(LogTags.Effect, "{0}, 이펙트를 즉시 삭제합니다.", this.GetHierarchyName());

            Despawner?.Despawn();
        }

        private void Despawn()
        {
            Log.Info(LogTags.Effect, "{0}, 이펙트를 삭제합니다.", this.GetHierarchyName());

            if (Animator != null)
            {
                if (Animator.PlayDespawnAnimation())
                {
                    return;
                }
            }

            Despawner?.Despawn();
        }

        private void StopDespawnTimer()
        {
            Despawner?.StopDespawnTimer();
        }

        private void ResetDespawningState()
        {
            if (Animator != null)
            {
                if (Animator.UseDespawnAnimation)
                {
                    Animator.ResetDespawningState();
                }
            }
        }

        /// <summary> 삭제 시 콜백 함수를 등록합니다. </summary>
        public void RegisterDespawnEvent(UnityAction onDespawn)
        {
            if (onDespawn != null)
            {
                Despawner?.OnDespawnEvent.AddListener(onDespawn);
            }
        }

        /// <summary> 삭제 시 콜백 함수를 등록합니다. </summary>
        public void RegisterDespawnEvent(UnityAction<VFXObject> onDespawn)
        {
            if (onDespawn != null)
            {
                _onDespawnCallback.AddListener(onDespawn);
            }
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void Initialize()
        {
            Log.Info(LogTags.Effect, "{0}, 이펙트를 초기화합니다.", this.GetHierarchyName());

            Animator?.Initialize();

            if (BreakParentPosition)
            {
                CoroutineNextTimer(BreakDelayTime, ResetParent);
            }

            RegisterDespawnEvent(CallDespawnCallback);
        }

        public void SetOwner(Character ownerCharacter)
        {
            Owner = ownerCharacter;
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetParent(Transform parent)
        {
            if (HaveParent)
            {
                transform.SetParent(parent, false);
                localPosition = Vector3.zero;
                localScale = _defaultLocalScale;
            }
        }

        public void SetParent(Character character, Transform parent = null)
        {
            if (parent != null)
            {
                SetParent(parent);
            }
            else
            {
                SetParent(character.transform);
            }
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetDirection(bool isFacingRight)
        {
            SetFlip(isFacingRight);

            if (ParticleGroup != null)
            {
                ParticleGroup.SetDirection(isFacingRight);
            }
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetPosition(Vector3 spawnPosition, Vector3 offset)
        {
            position = spawnPosition + offset;
        }

        private void ResetParent()
        {
            transform.SetParent(null);
            transform.SetAsLastSibling();
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetSortingName(SortingLayerNames sortingLayerName)
        {
            if (Renderers != null)
            {
                Renderers.SetSortingLayer(sortingLayerName);

                Log.Info(LogTags.Effect, "이펙트의 랜더링 오더를 설정합니다. {0}, SortingLayer:{1}", this.GetHierarchyName(), sortingLayerName);
            }
        }

        public void SetSortingOrder(int sortingOrder)
        {
            if (Renderers != null)
            {
                Renderers.SetSortingOrder(sortingOrder);

                Log.Info(LogTags.Effect, "이펙트의 랜더링 오더를 설정합니다. {0}, SortingOrder:{1}", this.GetHierarchyName(), sortingOrder);
            }
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        private void SetFlip(bool isFacingRight)
        {
            if (isFacingRight)
            {
                localScale = new Vector3(1, 1, 1);
            }
            else
            {
                localScale = new Vector3(-1, 1, 1);
            }
        }

        private void CallDespawnCallback()
        {
            _onDespawnCallback?.Invoke(this);
        }
    }
}