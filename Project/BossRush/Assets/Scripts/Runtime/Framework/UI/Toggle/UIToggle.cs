using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UIToggle : UIInteractiveElement
    {
        [FoldoutGroup("#UIToggle"), SerializeField] private Toggle _toggle;
        [FoldoutGroup("#UIToggle"), SerializeField] private Image _iconImage;
        [FoldoutGroup("#UIToggle"), SerializeField] private Image _lockImage;
        [FoldoutGroup("#UIToggle"), SerializeField] private Image _updateIndicatorImage;
        [FoldoutGroup("#UIToggle"), SerializeField] private Graphic _toggleGraphic;

        [FoldoutGroup("#UIToggle-Lock"), SerializeField] private bool _isLocked;
        [FoldoutGroup("#UIToggle-Lock"), SerializeField] private string _lockMessage;

        [FoldoutGroup("#UIToggle-Settings"), SerializeField] private Color _buttonImageActiveColor = new Color(1f, 1f, 1f, 0.5f);
        [FoldoutGroup("#UIToggle-Settings"), SerializeField] private Color _buttonImageInactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [FoldoutGroup("#UIToggle-Settings"), SerializeField] private Color _frameImageInactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        [FoldoutGroup("#UIToggle-Settings"), SerializeField] private float _colorTransitionDuration = 0.2f;

        [FoldoutGroup("#UIToggle-Events")]
        public UnityEvent<string> OnLockMessageRequested;

        private Tween _buttonColorTween;
        private Tween _frameColorTween;
        private Tween _textColorTween;

        public Toggle Toggle => _toggle;
        public bool IsLocked => _isLocked;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _toggle ??= GetComponent<Toggle>();
            _iconImage ??= this.FindComponent<Image>("Toggle Icon Image");
            _lockImage ??= this.FindComponent<Image>("Toggle Lock Image");
            _updateIndicatorImage ??= this.FindComponent<Image>("Toggle Update Image");

            if (_toggleGraphic == null && _toggle != null)
            {
                _toggleGraphic = _toggle.targetGraphic;
            }

            // UIToggle는 "Toggle Name Text"를 사용하므로 부모 클래스의 기본값을 오버라이드
            if (_nameText == null)
            {
                _nameText = this.FindComponent<TextMeshProUGUI>("Toggle Name Text");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitializeButtonImage();
            RegisterToggleEvent();
            UpdateLockVisual();
            UpdateToggleVisual(_toggle != null && _toggle.isOn);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            UnregisterToggleEvent();
        }

        private void InitializeButtonImage()
        {
            if (_buttonImage != null)
            {
                Color color = _buttonImage.color;
                color.a = 0f;
                _buttonImage.color = color;
            }
        }

        private void RegisterToggleEvent()
        {
            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }
        }

        private void UnregisterToggleEvent()
        {
            if (_toggle != null)
            {
                _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (_isLocked)
            {
                RequestLockMessage();
                return;
            }

            PlayPunchScaleAnimation();
            UpdateToggleVisual(isOn);

            if (!CheckClickCooldown())
            {
                Log.Warning(LogTags.UI_Toggle, $"(Toggle) {gameObject.name} 쿨타임 중이면 갱신하지 않습니다.");
                return;
            }

            Log.Info(LogTags.UI_Toggle, $"(Toggle) {gameObject.name} 값을 변경합니다: {isOn}");
            OnToggleValueChange(isOn);
        }

        protected virtual void OnToggleValueChange(bool isOn)
        {
            // 자식 클래스에서 오버라이드하여 토글 이벤트 구현
        }

        private void UpdateToggleVisual(bool isOn)
        {
            // 기존 트윈을 완료시키고 목표 색상으로 즉시 설정
            CompleteColorTweens();

            // 버튼 이미지 색상 변경 (켜져있을 때)
            if (_buttonImage != null)
            {
                Color targetColor = isOn ? _buttonImageActiveColor : _buttonImageInactiveColor;
                _buttonColorTween = _buttonImage.DOColor(targetColor, _colorTransitionDuration)
                    .SetEase(Ease.OutQuad);
            }

            // 프레임 이미지 색상 변경 (꺼져있을 때 회색)
            if (_frameImage != null)
            {
                Color targetColor = isOn ? _frameImageOriginalColor : _frameImageInactiveColor;
                _frameColorTween = _frameImage.DOColor(targetColor, _colorTransitionDuration)
                    .SetEase(Ease.OutQuad);
            }

            // 텍스트 색상 변경 (꺼져있을 때 회색)
            if (_nameText != null)
            {
                Color targetColor = isOn ? _nameTextOriginalColor : _frameImageInactiveColor;
                _textColorTween = _nameText.DOColor(targetColor, _colorTransitionDuration)
                    .SetEase(Ease.OutQuad);
            }

            // 아이콘 이미지 회색 처리
            SetIconGrayScale(!isOn);
        }

        private void SetIconGrayScale(bool isGray)
        {
            if (_iconImage != null)
            {
                _iconImage.color = isGray ? GameColors.Gray : GameColors.White;
            }
        }

        protected override void KillAllTweens()
        {
            base.KillAllTweens();
            KillColorTweens();
        }

        private void CompleteColorTweens()
        {
            // 트윈이 진행 중이면 목표 색상으로 즉시 완료
            if (_buttonColorTween != null && _buttonColorTween.IsActive())
            {
                _buttonColorTween.Complete();
            }
            if (_frameColorTween != null && _frameColorTween.IsActive())
            {
                _frameColorTween.Complete();
            }
            if (_textColorTween != null && _textColorTween.IsActive())
            {
                _textColorTween.Complete();
            }

            KillColorTweens();
        }

        private void KillColorTweens()
        {
            _buttonColorTween?.Kill();
            _frameColorTween?.Kill();
            _textColorTween?.Kill();
            _buttonColorTween = null;
            _frameColorTween = null;
            _textColorTween = null;
        }

        public void SetIsOn(bool isOn)
        {
            if (_toggle != null)
            {
                Log.Info(LogTags.UI_Toggle, $"(Toggle) {gameObject.name} 상태 설정: {isOn}");
                _toggle.isOn = isOn;
            }
        }

        public void SetGroup(ToggleGroup group)
        {
            if (_toggle != null)
            {
                _toggle.group = group;
            }
        }

        public void AddListener(UnityAction<bool> action)
        {
            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener(action);
            }
        }

        // 아이콘 관련 메서드
        public void ActivateIcon()
        {
            if (_iconImage != null)
            {
                _iconImage.gameObject.SetActive(true);
            }
        }

        public void DeactivateIcon()
        {
            if (_iconImage != null)
            {
                _iconImage.gameObject.SetActive(false);
            }
        }

        public void SetIcon(Sprite sprite)
        {
            AutoGetComponents();

            if (_iconImage == null)
            {
                return;
            }

            _iconImage.sprite = sprite;
            _iconImage.enabled = sprite != null;
        }

        // 잠금 관련 메서드
        public void SetLocked(bool locked)
        {
            Log.Info(LogTags.UI_Toggle, $"(Toggle) {gameObject.name} 잠금 상태 변경: {locked}");
            _isLocked = locked;
            UpdateLockVisual();
        }

        public void RequestLockMessage()
        {
            if (!_isLocked)
            {
                return;
            }

            Log.Warning(LogTags.UI_Toggle, $"(Toggle) {gameObject.name} 잠금 메시지 요청: {_lockMessage}");
            OnLockMessageRequested?.Invoke(_lockMessage);
        }

        public void UpdateLockVisual()
        {
            AutoGetComponents();

            SetGrayScale(_toggleGraphic, _isLocked);
            SetIconGrayScale(_isLocked);
            SetNameTextGrayScale(_isLocked);

            if (_lockImage != null)
            {
                _lockImage.gameObject.SetActive(_isLocked);
            }
        }

        private void SetGrayScale(Graphic graphic, bool isGray)
        {
            if (graphic == null)
            {
                return;
            }

            graphic.color = isGray ? GameColors.Gray : GameColors.White;
        }

        private void SetNameTextGrayScale(bool isGray)
        {
            if (_nameText != null)
            {
                _nameText.color = isGray ? GameColors.Gray : GameColors.White;
            }
        }

        // 업데이트 인디케이터 관련 메서드
        public void SetHasUpdate(bool hasUpdate)
        {
            if (_updateIndicatorImage != null)
            {
                _updateIndicatorImage.gameObject.SetActive(hasUpdate);
            }
        }
    }
}