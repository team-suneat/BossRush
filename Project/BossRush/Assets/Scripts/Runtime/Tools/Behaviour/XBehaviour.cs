using Sirenix.OdinInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public class XBehaviour : MonoBehaviour
    {
        private bool _isDestroyed;
        protected bool IsDestroyed => _isDestroyed;

        #region Helper Methods

        private T SafeAccess<T>(System.Func<T> accessor, T defaultValue, string propertyName)
        {
            // 객체가 파괴되었는지 먼저 확인
            if (_isDestroyed || !this)
            {
                Log.Warning($"{this.GetHierarchyPath()} - 객체가 파괴된 상태에서 {propertyName}에 접근하려고 합니다.");
                return defaultValue;
            }

            // transform이 null인지 확인
            if (transform == null)
            {
                Log.Warning($"{this.GetHierarchyPath()} - Transform이 null인 상태에서 {propertyName}에 접근하려고 합니다.");
                return defaultValue;
            }

            return accessor();
        }

        #endregion Helper Methods

        #region Parameter - Transform

        public Vector3 position
        {
            get => SafeAccess(() => transform.position, Vector3.zero, nameof(position));
            set
            {
                if (transform != null)
                {
                    transform.position = value;
                }
            }
        }

        public Vector3 localPosition
        {
            get => SafeAccess(() => transform.localPosition, Vector3.zero, nameof(localPosition));
            set
            {
                if (transform != null)
                {
                    transform.localPosition = value;
                }
            }
        }

        public Quaternion rotation
        {
            get => SafeAccess(() => transform.rotation, Quaternion.identity, nameof(rotation));
            set
            {
                if (transform != null)
                {
                    transform.rotation = value;
                }
            }
        }

        public Quaternion localRotation
        {
            get => SafeAccess(() => transform.localRotation, Quaternion.identity, nameof(localRotation));
            set
            {
                if (transform != null)
                {
                    transform.localRotation = value;
                }
            }
        }

        public Vector3 localScale
        {
            get => SafeAccess(() => transform.localScale, Vector3.one, nameof(localScale));
            set
            {
                if (transform != null)
                {
                    transform.localScale = value;
                }
            }
        }

        public Vector3 lossyScale => SafeAccess(() => transform.lossyScale, Vector3.one, nameof(lossyScale));

        #endregion Parameter - Transform

        #region Parameter - UI

        private RectTransform _rectTransform;

        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }

        public Vector3 anchoredPosition3D
        {
            get => rectTransform != null ? rectTransform.anchoredPosition3D : Vector3.zero;
            set
            {
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition3D = value;
                }
            }
        }

        public Vector2 sizeDelta
        {
            get => rectTransform != null ? rectTransform.sizeDelta : Vector2.zero;
            set
            {
                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = value;
                }
            }
        }

        #endregion Parameter - UI

        #region Parameter - GameObject

        public virtual bool ActiveSelf
        {
            get
            {
                // 객체가 파괴되었는지 먼저 확인
                if (!this)
                {
                    return false;
                }

                return gameObject.activeSelf;
            }
        }

        public virtual bool ActiveInHierarchy
        {
            get
            {
                if (!this)
                {
                    return false;
                }

                return gameObject.activeInHierarchy;
            }
        }

        public int Layer
        {
            get
            {
                if (!this)
                {
                    return 0;
                }

                return gameObject.layer;
            }
            set
            {
                if (!this)
                {
                    gameObject.layer = value;
                }
            }
        }

        #endregion Parameter - GameObject

        public bool IsStarted { get; set; }

        //────────────────────────────────────────────────────────────────────────────────────────────────

        #region Editor

        /// <summary>
        /// 컴포넌트 자동 추가
        /// </summary>
        [FoldoutGroup("#Buttons", 999)]
        [Button("Auto Get Components", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        public virtual void AutoGetComponents()
        {
        }

        [FoldoutGroup("#Buttons", 999)]
        [Button("Auto Add Components", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        public virtual void AutoAddComponents()
        {
            AutoGetComponents();
        }

        [FoldoutGroup("#Buttons", 999)]
        [Button("Auto Setting", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        public virtual void AutoSetting()
        {
        }

        [FoldoutGroup("#Buttons", 999)]
        [Button("Auto Naming", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        public virtual void AutoNaming()
        {
        }

        [Conditional("UNITY_EDITOR")]
        public void SetGameObjectName(string name)
        {
#if UNITY_EDITOR
            if (this)
                gameObject.name = name;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public void SetGameObjectName(string format, params object[] args)
        {
#if UNITY_EDITOR
            if (this)
                gameObject.name = string.Format(format, args);
#endif
        }

        #endregion Editor

        #region Unity Engine

        private void Awake()
        {
            _isDestroyed = false;
        }

        private void Start()
        {
            IsStarted = true;

            OnStart();
        }

        private void OnDestroy()
        {
            OnRelease();

            _isDestroyed = true;
        }

        protected void OnEnable()
        {
            OnEnabled();
        }

        private void OnDisable()
        {
            OnDisabled();

            StopAllCoroutines();
        }

        protected virtual void OnStart()
        {
            RegisterGlobalEvent();
        }

        protected virtual void OnRelease()
        {
        }

        protected virtual void OnEnabled()
        {
            if (IsStarted)
            {
                RegisterGlobalEvent();
            }
        }

        protected virtual void OnDisabled()
        {
            UnregisterGlobalEvent();
        }

        #endregion Unity Engine

        public virtual void SetActive(bool value)
        {
            if (false == Application.isPlaying)
            {
                return;
            }

            if (!this)
            {
                return;
            }

            if (gameObject.activeSelf != value)
            {
                gameObject.SetActive(value);
            }
        }

        public virtual void SetCenterAnchoredPosition()
        {
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition3D = rectTransform.sizeDelta * 0.5f;
            }
        }

        #region Global Event

        protected virtual void RegisterGlobalEvent()
        {
        }

        protected virtual void UnregisterGlobalEvent()
        {
        }

        #endregion Global Event

        #region Coroutine

        public Coroutine StartXCoroutine(IEnumerator routine)
        {
            if (ActiveInHierarchy)
            {
                return StartCoroutine(routine);
            }

            return null;
        }

        public void StopXCoroutine(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = null;
        }

        protected virtual Coroutine CoroutineNextFrame(UnityAction callback)
        {
            if (callback != null)
            {
                return StartXCoroutine(CoroutineEx.NextFrame(callback));
            }

            return null;
        }

        protected virtual Coroutine CoroutineNextTimer(float delay, UnityAction callback)
        {
            if (callback != null)
            {
                if (delay > 0)
                {
                    return StartXCoroutine(CoroutineEx.NextTimer(delay, callback));
                }
                else
                {
                    callback.Invoke();
                }
            }

            return null;
        }

        protected virtual Coroutine CoroutineNextRealTimer(float delay, UnityAction callback)
        {
            if (callback != null)
            {
                if (delay > 0)
                {
                    return StartXCoroutine(CoroutineEx.NextRealTimer(delay, callback));
                }
                else
                {
                    callback.Invoke();
                }
            }

            return null;
        }

        #endregion Coroutine
    }
}