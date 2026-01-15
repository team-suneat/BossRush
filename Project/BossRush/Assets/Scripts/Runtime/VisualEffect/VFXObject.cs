using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class VFXObject : XBehaviour, IPoolable
    {
        public bool OnlyOne;
        public bool HaveParent;

        [InfoBox("일정 시간 뒤 부모 오브젝트와 분리됩니다.")]
        [ShowIf("HaveParent")]
        public float BreakDelayTime;

        private AutoDespawn _despawner;
        private ParticleEffect2DGroup _particleGroup;
        private VFXMover _mover;
        private VFXAnimator _animator;
        private SpriteRenderer[] _renderers;

        private UnityEvent<VFXObject> _onDespawnCallback;
        private Vector3 _defaultLocalScale;
        private Coroutine _breakParentCoroutine;

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
        }

        private void Awake()
        {
            _defaultLocalScale = transform.localScale;

            _despawner = GetComponent<AutoDespawn>();
            _mover = GetComponent<VFXMover>();
            _animator = GetComponentInChildren<VFXAnimator>();
            _renderers = GetComponentsInChildren<SpriteRenderer>();
            _particleGroup = GetComponent<ParticleEffect2DGroup>();
        }

        public void OnSpawn()
        {
            VFXManager.Register(this);
        }

        public void OnDespawn()
        {
            VFXManager.Unregister(this);
            StopBreakParentCoroutine();
        }

        public void ForceDespawn()
        {
            Log.Info(LogTags.Effect, "{0}, 이펙트를 강제 삭제합니다.", this.GetHierarchyName());

            StopDespawnTimer();
            StopBreakParentCoroutine();
            ResetDespawningState();
            Despawn();
        }

        public void InstantDespawn()
        {
            Log.Info(LogTags.Effect, "{0}, 이펙트를 즉시 삭제합니다.", this.GetHierarchyName());

            _despawner?.Despawn();
        }

        private void Despawn()
        {
            Log.Info(LogTags.Effect, "{0}, 이펙트를 삭제합니다.", this.GetHierarchyName());

            if (_animator != null && _animator.PlayDespawnAnimation())
            {
                return;
            }

            _despawner?.Despawn();
        }

        private void StopDespawnTimer()
        {
            _despawner?.StopDespawnTimer();
        }

        private void ResetDespawningState()
        {
            if (_animator != null && _animator.UseDespawnAnimation)
            {
                _animator.ResetDespawningState();
            }
        }

        public void RegisterDespawnEvent(UnityAction onDespawn)
        {
            if (onDespawn == null)
            {
                return;
            }

            _despawner?.OnDespawnEvent.AddListener(onDespawn);
        }

        public void RegisterDespawnEvent(UnityAction<VFXObject> onDespawn)
        {
            if (onDespawn == null)
            {
                return;
            }

            _onDespawnCallback.AddListener(onDespawn);
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void Initialize()
        {
            Log.Info(LogTags.Effect, "{0}, 이펙트를 초기화합니다.", this.GetHierarchyName());

            _animator?.Initialize();

            if (BreakDelayTime > 0f)
            {
                _breakParentCoroutine = CoroutineNextTimer(BreakDelayTime, ResetParent);
            }

            RegisterDespawnEvent(CallDespawnCallback);
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetParent(Transform parent)
        {
            if (!HaveParent)
            {
                return;
            }

            transform.SetParent(parent, false);
            localPosition = Vector3.zero;
            localScale = _defaultLocalScale;
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetDirection(bool isFacingRight)
        {
            SetFlip(isFacingRight);

            if (_particleGroup != null)
            {
                _particleGroup.SetDirection(isFacingRight);
            }
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetPosition(Transform parent)
        {
            if (!HaveParent)
            {
                position = parent.position;
            }
        }

        public void SetPosition(Vector3 spawnPosition, Vector3 offset)
        {
            position = spawnPosition + offset;
        }

        public void StartMove(Vector3 originPosition, Vector3 targetPosition)
        {
            if (_mover == null)
            {
                return;
            }

            _mover.SetOriginPosition(originPosition);
            _mover.SetTargetPosition(targetPosition);
            _mover.StartMove();
        }

        private void ResetParent()
        {
            transform.SetParent(null);
            transform.SetAsLastSibling();
            _breakParentCoroutine = null;
        }

        private void StopBreakParentCoroutine()
        {
            if (_breakParentCoroutine != null)
            {
                StopXCoroutine(ref _breakParentCoroutine);
            }
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetSortingName(SortingLayerNames sortingLayerName)
        {
            if (_renderers == null)
            {
                return;
            }

            _renderers.SetSortingLayer(sortingLayerName);
            Log.Info(LogTags.Effect, "이펙트의 랜더링 오더를 설정합니다. {0}, SortingLayer:{1}", this.GetHierarchyName(), sortingLayerName);
        }

        public void SetSortingOrder(int sortingOrder)
        {
            if (_renderers == null)
            {
                return;
            }

            _renderers.SetSortingOrder(sortingOrder);
            Log.Info(LogTags.Effect, "이펙트의 랜더링 오더를 설정합니다. {0}, SortingOrder:{1}", this.GetHierarchyName(), sortingOrder);
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