using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamSuneat.UserInterface
{
    public class UISelectButton : UISelectElement, IPointerDownHandler, IPointerUpHandler
    {
        private const float ALPHA_TRANSPARENT = 0f;
        private const float ALPHA_SEMI_TRANSPARENT = 0.5f;
        private const float ALPHA_ANIMATION_DURATION_RATIO = 0.5f;
        private const float DEFAULT_HOLD_INTERVAL = 0.1f;
        private const float DEFAULT_HOLD_DELAY = 0.3f;
        private const float DEFAULT_BUTTON_IMAGE_ALPHA_DURATION = 0.15f;

        [FoldoutGroup("#UISelectButton")]
        [SerializeField] private Button _button;

        [FoldoutGroup("#UISelectButton")]
        [SerializeField] protected UIInteractiveElement _interactive;

        [FoldoutGroup("#UISelectButton/Event")]
        public UnityEvent OnClickSuccess;

        [FoldoutGroup("#UISelectButton/Event")]
        public UnityEvent OnClickFailure;

        private Tween _alphaTween;
        private Coroutine _holdCoroutine;
        private bool _isHolding;

        public Button Button => _button;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _button ??= GetComponent<Button>();
            _interactive ??= GetComponent<UIInteractiveElement>();
        }

        protected override void OnStart()
        {
            base.OnStart();

            InitializeButtonImage();
        }

        private void InitializeButtonImage()
        {
            if (_interactive?.ButtonImage != null)
            {
                Color color = _interactive.ButtonImage.color;
                color.a = ALPHA_TRANSPARENT;
                _interactive.ButtonImage.color = color;
            }
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            OnPointerClickLeftEvent.AddListener(OnButtonClick);
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            OnPointerClickLeftEvent.RemoveListener(OnButtonClick);

            StopHold();
            KillAllTweens();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            StopHold();
            KillAllTweens();
        }

        public override void OnPointerClickLeft()
        {
            base.OnPointerClickLeft();
            PlayClickVisual();
        }

        public override void OnPointerClickRight()
        {
            base.OnPointerClickRight();
            PlayClickVisual();
        }

        // 실제 클릭 처리
        protected virtual void OnButtonClick()
        {
            if (!CheckClickable())
            {
                OnClickFailed();
                return;
            }

            OnClickSucceeded();
        }

        protected virtual void OnButtonHold()
        {
            if (!CheckClickable())
            {
                OnHoldFailed();
                return;
            }

            OnHoldSucceeded();
        }

        protected virtual bool CheckClickable()
        {
            if (_interactive == null)
            {
                return true;
            }

            return _interactive.IsClickable && _interactive.CheckClickCooldown();
        }

        private void PlayClickVisual()
        {
            if (_interactive == null)
            {
                return;
            }

            _interactive.PlayPunchScaleAnimation();
            PlayButtonImageAlphaAnimation();
        }

        private void PlayButtonImageAlphaAnimation()
        {
            KillAlphaTween();

            if (_interactive?.ButtonImage != null)
            {
                float halfDuration = DEFAULT_BUTTON_IMAGE_ALPHA_DURATION * ALPHA_ANIMATION_DURATION_RATIO;
                _alphaTween = _interactive.ButtonImage.DOFade(ALPHA_SEMI_TRANSPARENT, halfDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        if (_interactive?.ButtonImage != null)
                        {
                            _ = _interactive.ButtonImage.DOFade(ALPHA_TRANSPARENT, halfDuration)
                                .SetEase(Ease.InQuad);
                        }
                    });
            }
        }

        protected virtual void OnClickSucceeded()
        {
            OnClickSuccess?.Invoke();
            Log.Info(LogTags.UI_Button, "OnClickSucceeded");
        }

        protected virtual void OnClickFailed()
        {
            OnClickFailure?.Invoke();
        }

        protected virtual void OnHoldSucceeded()
        {
        }

        protected virtual void OnHoldFailed()
        {
        }

        private void KillAllTweens()
        {
            _interactive?.KillAllTweens();
            KillAlphaTween();
        }

        private void KillAlphaTween()
        {
            _alphaTween?.Kill();
            _alphaTween = null;
        }

        #region Click Events

        public void RegisterClickSuccessEvent(UnityAction action)
        {
            OnClickSuccess.AddListener(action);
        }

        public void UnregisterClickSuccessEvent(UnityAction action)
        {
            OnClickSuccess.RemoveListener(action);
        }

        public void RegisterClickFailureEvent(UnityAction action)
        {
            OnClickFailure.AddListener(action);
        }

        public void UnregisterClickFailureEvent(UnityAction action)
        {
            OnClickFailure.RemoveListener(action);
        }

        #endregion Click Events

        #region Pointer Event (홀드용으로만 유지)

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_interactive == null || !_interactive.IsClickable)
            {
                return;
            }

            _isHolding = true;
            StartHoldCoroutine();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopHold();
        }

        #endregion Pointer Event (홀드용으로만 유지)

        #region Hold

        public void StartHold()
        {
            if (!CheckClickable())
            {
                return;
            }

            _isHolding = true;
            StartHoldCoroutine();
        }

        public void StopHold()
        {
            _isHolding = false;
            StopHoldCoroutine();
        }

        private void StartHoldCoroutine()
        {
            StopHoldCoroutine();
            _holdCoroutine = StartCoroutine(HoldCoroutine());
        }

        private void StopHoldCoroutine()
        {
            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
                _holdCoroutine = null;
            }
        }

        private IEnumerator HoldCoroutine()
        {
            yield return new WaitForSeconds(DEFAULT_HOLD_DELAY);

            while (_isHolding && CheckClickable())
            {
                yield return new WaitForSeconds(DEFAULT_HOLD_INTERVAL);
                if (_isHolding && CheckClickable())
                {
                    OnButtonHold();
                }
            }
        }

        #endregion Hold

        public void ActivateRaycast()
        {
            _interactive?.ActivateRaycast();
        }

        public void DeactivateRaycast()
        {
            _interactive?.DeactivateRaycast();
        }
    }
}