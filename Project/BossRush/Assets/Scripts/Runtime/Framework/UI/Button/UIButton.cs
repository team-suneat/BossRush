using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIButton : UIInteractiveElement, IPointerDownHandler, IPointerUpHandler
    {
        private const float ALPHA_TRANSPARENT = 0f;
        private const float ALPHA_SEMI_TRANSPARENT = 0.5f;
        private const float ALPHA_ANIMATION_DURATION_RATIO = 0.5f;
        private const float DEFAULT_HOLD_INTERVAL = 0.1f;
        private const float DEFAULT_HOLD_DELAY = 0.3f;
        private const float DEFAULT_BUTTON_IMAGE_ALPHA_DURATION = 0.15f;

        [FoldoutGroup("#UIButton")]
        [SerializeField] private Button _button;

        [FoldoutGroup("#UIButton/Event")]
        public UnityEvent OnClickSuccess;

        [FoldoutGroup("#UIButton/Event")]
        public UnityEvent OnClickFailure;

        private Tween _alphaTween;
        private Coroutine _holdCoroutine;
        private bool _isHolding;

        public Button Button => _button;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _button ??= GetComponent<Button>();
        }

        //

        protected override void OnStart()
        {
            base.OnStart();
            InitializeButtonImage();
        }

        private void InitializeButtonImage()
        {
            if (_buttonImage != null)
            {
                Color color = _buttonImage.color;
                color.a = ALPHA_TRANSPARENT;
                _buttonImage.color = color;
            }
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClick);
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnButtonClick);
            }

            StopHoldCoroutine();
            KillAllTweens();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            StopHoldCoroutine();
            KillAllTweens();
        }

        //

        protected virtual void OnButtonClick()
        {
            if (CheckClickable())
            {
                PlayPunchScaleAnimation();
                PlayButtonImageAlphaAnimation();
                OnClickSucceeded();
            }
            else
            {
                OnClickFailed();
            }
        }

        protected virtual void OnButtonHold()
        {
            if (CheckClickable())
            {
                PlayPunchScaleAnimation();
                PlayButtonImageAlphaAnimation();
                OnHoldSucceeded();
            }
            else
            {
                OnHoldFailed();
            }
        }

        protected virtual bool CheckClickable()
        {
            return CheckClickCooldown();
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

        private void PlayButtonImageAlphaAnimation()
        {
            KillAlphaTween();

            if (_buttonImage != null)
            {
                float halfDuration = DEFAULT_BUTTON_IMAGE_ALPHA_DURATION * ALPHA_ANIMATION_DURATION_RATIO;
                _alphaTween = _buttonImage.DOFade(ALPHA_SEMI_TRANSPARENT, halfDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        if (_buttonImage != null)
                        {
                            _buttonImage.DOFade(ALPHA_TRANSPARENT, halfDuration)
                                .SetEase(Ease.InQuad);
                        }
                    });
            }
        }

        protected override void KillAllTweens()
        {
            base.KillAllTweens();
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

        #region Pointer Event

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isClickable)
            {
                return;
            }

            _isHolding = true;
            StartHoldCoroutine();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopHoldCoroutine();
            _isHolding = false;
        }

        #endregion Pointer Event

        #region Hold

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
            // 첫 터치 후 홀드 시작까지 대기
            yield return new WaitForSeconds(DEFAULT_HOLD_DELAY);

            while (_isHolding && _isClickable)
            {
                yield return new WaitForSeconds(DEFAULT_HOLD_INTERVAL);

                if (_isHolding && _isClickable)
                {
                    OnButtonHold();
                }
            }
        }

        #endregion Hold
    }
}