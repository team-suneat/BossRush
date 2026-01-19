using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamSuneat.UserInterface
{
    public abstract class UISelectButtonBase : UISelectElement, IPointerDownHandler, IPointerUpHandler
    {
        private const float ALPHA_TRANSPARENT = 0f;
        private const float ALPHA_SEMI_TRANSPARENT = 0.5f;
        private const float ALPHA_ANIMATION_DURATION_RATIO = 0.5f;
        private const float DEFAULT_HOLD_INTERVAL = 0.1f;
        private const float DEFAULT_HOLD_DELAY = 0.3f;
        private const float DEFAULT_BUTTON_IMAGE_ALPHA_DURATION = 0.15f;

        [FoldoutGroup("#UISelectButtonBase")]
        [SerializeField] protected Button _button;

        [FoldoutGroup("#UISelectButtonBase")]
        [SerializeField] protected UIInteractiveElement _interactive;

        [FoldoutGroup("#UISelectButtonBase/State")]
        [SerializeField] protected ButtonState _currentState = ButtonState.UnlockedUnselected;

        [FoldoutGroup("#Event")]
        public UnityEvent OnClickSuccess;

        [FoldoutGroup("#Event")]
        public UnityEvent OnClickFailure;

        [FoldoutGroup("#Event")]
        public UnityEvent<ButtonState> OnStateChanged;

        private Tween _alphaTween;
        private Coroutine _holdCoroutine;
        private bool _isHolding;

        public Button Button => _button;
        public ButtonState CurrentState => _currentState;

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
            UpdateVisualByState();
        }

        private void InitializeButtonImage()
        {
            if (_interactive?.ButtonImage == null)
            {
                return;
            }

            Color color = _interactive.ButtonImage.color;
            color.a = ALPHA_TRANSPARENT;
            _interactive.ButtonImage.color = color;
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

        protected virtual void OnButtonClick()
        {
            if (!TryHandleClick())
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

        public override void OnSubmitDown()
        {
            // ButtonDown은 쿨다운과 딜레이 없이 즉시 처리
            // TryHandleClick()을 직접 호출하되, 쿨다운 체크를 우회
            if (!CheckClickableWithoutCooldown())
            {
                OnClickFailed();
                return;
            }

            // TryHandleClick()을 직접 호출하여 딜레이와 쿨다운을 우회
            if (!TryHandleClick())
            {
                OnClickFailed();
                return;
            }

            OnClickSucceeded();
        }

        public override void OnSubmit()
        {
            // ButtonPressed는 쿨다운 체크 후 처리
            // UISelectController에서 시간 간격으로 호출되므로
            // 여기서는 일반적인 클릭 처리와 동일하게 처리
            if (!CheckClickable())
            {
                OnClickFailed();
                return;
            }

            if (!TryHandleClick())
            {
                OnClickFailed();
                return;
            }

            OnClickSucceeded();
        }

        public override void OnSubmitUp()
        {
            // ButtonUp은 쿨다운 없이 즉시 처리
            base.OnSubmitUp();
        }

        protected virtual bool CheckClickable()
        {
            if (_currentState == ButtonState.Locked)
            {
                Log.Warning(LogTags.UI_Button, "버튼이 잠금 상태입니다. 클릭할 수 없습니다.");
                return false;
            }

            if (_interactive == null)
            {
                Log.Warning(LogTags.UI_Button, "UIInteractiveElement 컴포넌트가 비어있습니다. 클릭할 수 있습니다.");
                return true;
            }

            return _interactive.IsClickable && _interactive.CheckClickCooldown();
        }

        private bool CheckClickableWithoutCooldown()
        {
            if (_currentState == ButtonState.Locked)
            {
                Log.Warning(LogTags.UI_Button, "버튼이 잠금 상태입니다. 클릭할 수 없습니다.");
                return false;
            }

            if (_interactive == null)
            {
                Log.Warning(LogTags.UI_Button, "UIInteractiveElement 컴포넌트가 비어있습니다. 클릭할 수 있습니다.");
                return true;
            }

            // 쿨다운 체크 없이 클릭 가능 여부만 확인
            return _interactive.IsClickable;
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

            if (_interactive?.ButtonImage == null)
            {
                return;
            }

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

        #region State Management

        public void SetState(ButtonState newState)
        {
            if (_currentState == newState)
            {
                return;
            }

            _currentState = newState;

            Log.Info(LogTags.UI_Button, "버튼 상태 업데이트: {0} -> {1}", gameObject.name, newState);

            if (newState == ButtonState.Locked)
            {
                _interactive?.SetLockedForced(true);
                _interactive?.SetClickable(false);
            }
            else
            {
                _interactive?.SetLockedForced(false);
                _interactive?.SetClickable(true);
            }

            UpdateVisualByState();
            OnStateChanged?.Invoke(newState);
        }

        public void SetUnlocked(bool unlocked, bool selected = false)
        {
            if (unlocked)
            {
                SetState(selected ? ButtonState.UnlockedSelected : ButtonState.UnlockedUnselected);
            }
            else
            {
                SetState(ButtonState.Locked);
            }
        }

        public void UpdateVisualByState()
        {
            _interactive?.UpdateStateVisual(_currentState);
        }

        protected abstract bool TryHandleClick();

        #endregion State Management
    }
}
