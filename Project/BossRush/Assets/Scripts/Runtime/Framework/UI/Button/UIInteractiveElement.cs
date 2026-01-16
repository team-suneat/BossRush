using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TeamSuneat;

namespace TeamSuneat.UserInterface
{
    public class UIInteractiveElement : XBehaviour
    {
        private const float DEFAULT_CLICK_COOLDOWN = 0.15f;
        private const float DEFAULT_PUNCH_SCALE_DURATION = 0.1f;
        private const float DEFAULT_PUNCH_SCALE_VALUE = -0.1f;
        private const int PUNCH_SCALE_VIBRATO = 1;
        private const float PUNCH_SCALE_ELASTICITY = 0.5f;
        private const float ALPHA_OPAQUE = 1f;
        private const float DURATION_ZERO = 0f;
        private const float DISABLED_ALPHA = 0.5f;

        [FoldoutGroup("#UIInteractiveElement"), SerializeField] protected Image _frameImage;
        [FoldoutGroup("#UIInteractiveElement"), SerializeField] protected Image _buttonImage;
        [FoldoutGroup("#UIInteractiveElement"), SerializeField] protected TextMeshProUGUI _nameText;
        [FoldoutGroup("#UIInteractiveElement"), SerializeField] protected Image _selectedImage;

        protected Vector3 _punchScale =
            new(DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE);

        protected float _lastClickTime;
        protected Tween _scaleTween;
        protected bool _isClickable = true;
        protected bool _isLockedForced = false;

        public bool IsClickable => _isClickable;
        public Image ButtonImage => _buttonImage;

        protected Color _frameImageOriginalColor;
        protected Color _nameTextOriginalColor;

        private Vector3 _originalScale;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _frameImage ??= this.FindComponent<Image>("Frame Image");
            _buttonImage ??= this.FindComponent<Image>("Button Image");
            _nameText ??= this.FindComponent<TextMeshProUGUI>("Button Name Text");
            _selectedImage ??= this.FindComponent<Image>("Selected Image");
        }

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitializeVisuals();
            Log.Info(LogTags.UI_SelectEvent, "OnStart: {0} 초기화됨", gameObject.name);
        }

        protected virtual void InitializeVisuals()
        {
            if (_frameImage != null)
            {
                _frameImageOriginalColor = _frameImage.color;
            }

            if (_nameText != null)
            {
                _nameTextOriginalColor = _nameText.color;
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            KillAllTweens();
            Log.Info(LogTags.UI_SelectEvent, "OnRelease: {0} 해제됨", gameObject.name);
        }

        // 클릭 가능 여부 설정 시 비주얼도 함께 처리
        public virtual void SetClickable(bool clickable)
        {
            // Locked 상태일 때는 강제로 false 유지
            if (_isLockedForced)
            {
                clickable = false;
            }

            if (_isClickable == clickable)
            {
                return;
            }

            _isClickable = clickable;
            UpdateClickableVisual(clickable);
            Log.Info(LogTags.UI_SelectEvent, "클릭 가능 여부 설정: {0} -> {1}", gameObject.name, clickable);
        }

        public void SetLockedForced(bool locked)
        {
            _isLockedForced = locked;
            Log.Info(LogTags.UI_SelectEvent, "강제 잠금 설정: {0} -> {1}", gameObject.name, locked);
            if (locked)
            {
                SetClickable(false);
            }
        }

        protected virtual void UpdateClickableVisual(bool clickable)
        {
            float targetAlpha = clickable ? ALPHA_OPAQUE : DISABLED_ALPHA;

            if (_frameImage != null)
            {
                Color color = _frameImage.color;
                color.a = targetAlpha;
                _frameImage.color = color;
            }

            if (_nameText != null)
            {
                Color color = _nameText.color;
                color.a = targetAlpha;
                _nameText.color = color;
            }
        }

        public bool CheckClickCooldown()
        {
            if (!_isClickable)
            {
                Log.Info(LogTags.UI_SelectEvent, "클릭 쿨다운 확인: {0} -> false (클릭 불가능)", gameObject.name);
                return false;
            }

            float currentTime = Time.time;
            if (currentTime - _lastClickTime < DEFAULT_CLICK_COOLDOWN)
            {
                float remainingTime = DEFAULT_CLICK_COOLDOWN - (currentTime - _lastClickTime);
                Log.Info(LogTags.UI_SelectEvent, "클릭 쿨다운 확인: {0} -> false (쿨다운 남은 시간: {1:F2}초)", gameObject.name, remainingTime);
                return false;
            }

            _lastClickTime = currentTime;
            Log.Info(LogTags.UI_SelectEvent, "클릭 쿨다운 확인: {0} -> true", gameObject.name);
            return true;
        }

        public void PlayPunchScaleAnimation()
        {
            KillScaleTween();

            if (transform != null)
            {
                _scaleTween = transform.DOPunchScale(_punchScale, DEFAULT_PUNCH_SCALE_DURATION, PUNCH_SCALE_VIBRATO, PUNCH_SCALE_ELASTICITY)
                    .SetEase(Ease.OutQuad).OnComplete(OnCompletedPunchScale);
                Log.Info(LogTags.UI_SelectEvent, "펀치 스케일 애니메이션 재생: {0}", gameObject.name);
            }
        }

        private void OnCompletedPunchScale()
        {
            transform.localScale = _originalScale;
            _scaleTween = null;
        }

        public virtual void KillAllTweens()
        {
            KillScaleTween();
        }

        protected void KillScaleTween()
        {
            _scaleTween?.Kill();
            _scaleTween = null;
        }

        public void SetName(string content)
        {
            AutoGetComponents();

            if (_nameText != null)
            {
                _nameText.text = content;
                Log.Info(LogTags.UI_SelectEvent, "이름 설정: {0} -> {1}", gameObject.name, content);
            }
        }

        protected void SetNameTextColor(Color color, float duration = DURATION_ZERO)
        {
            if (_nameText == null)
            {
                return;
            }

            if (duration > DURATION_ZERO)
            {
                _ = _nameText.DOColor(color, duration).SetEase(Ease.OutQuad);
            }
            else
            {
                _nameText.color = color;
            }
        }

        public void SetFrameImageColor(Color color, float alpha = ALPHA_OPAQUE, float duration = DURATION_ZERO)
        {
            if (_frameImage == null)
            {
                return;
            }

            color.a = alpha;

            if (duration > DURATION_ZERO)
            {
                _ = _frameImage.DOColor(color, duration).SetEase(Ease.OutQuad);
            }
            else
            {
                _frameImage.color = color;
            }
        }

        protected void SetButtonImageColor(Color color, float alpha = ALPHA_OPAQUE, float duration = DURATION_ZERO)
        {
            if (_buttonImage == null)
            {
                return;
            }

            color.a = alpha;

            if (duration > DURATION_ZERO)
            {
                _ = _buttonImage.DOColor(color, duration).SetEase(Ease.OutQuad);
            }
            else
            {
                _buttonImage.color = color;
            }
        }

        public void ActivateRaycast()
        {
            if (_frameImage != null)
            {
                _frameImage.raycastTarget = true;
            }

            if (_buttonImage != null)
            {
                _buttonImage.raycastTarget = true;
            }

            if (_nameText != null)
            {
                _nameText.raycastTarget = true;
            }
        }

        public void DeactivateRaycast()
        {
            if (_frameImage != null)
            {
                _frameImage.raycastTarget = false;
            }

            if (_buttonImage != null)
            {
                _buttonImage.raycastTarget = false;
            }

            if (_nameText != null)
            {
                _nameText.raycastTarget = false;
            }
        }

        // 상태별 비주얼 업데이트 (UISelectButton에서 호출)
        public virtual void UpdateStateVisual(ButtonState state)
        {
            bool isClickable = state != ButtonState.Locked;
            SetClickable(isClickable);

            Log.Info(LogTags.UI_SelectEvent, "상태별 비주얼 업데이트: {0} -> {1}", gameObject.name, state);

            if (state == ButtonState.UnlockedSelected)
            {
                UpdateSelectedVisual(true);
                SetNameTextColor(GameColors.White);
                SetFrameImageColor(GameColors.White);
            }
            else if (state == ButtonState.UnlockedUnselected)
            {
                UpdateSelectedVisual(false);
                SetNameTextColor(GameColors.Gray);
                SetFrameImageColor(GameColors.Gray);
            }
            else if (state == ButtonState.Locked)
            {
                UpdateSelectedVisual(false);
                SetNameTextColor(GameColors.BlackGray);
                SetFrameImageColor(GameColors.BlackGray);
            }
        }

        // 선택 상태 비주얼 업데이트
        protected virtual void UpdateSelectedVisual(bool isSelected)
        {
            if (_selectedImage != null)
            {
                _selectedImage.gameObject.SetActive(isSelected);
            }
        }
    }
}