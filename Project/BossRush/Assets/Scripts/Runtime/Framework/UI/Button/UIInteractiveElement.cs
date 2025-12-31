using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    public abstract class UIInteractiveElement : XBehaviour
    {
        private const float DEFAULT_CLICK_COOLDOWN = 0.15f;
        private const float DEFAULT_PUNCH_SCALE_DURATION = 0.1f;
        private const float DEFAULT_PUNCH_SCALE_VALUE = -0.1f;
        private const int PUNCH_SCALE_VIBRATO = 1;
        private const float PUNCH_SCALE_ELASTICITY = 0.5f;
        private const float ALPHA_OPAQUE = 1f;
        private const float DURATION_ZERO = 0f;

        [FoldoutGroup("#UIInteractiveElement"), SerializeField] protected Image _frameImage;
        [FoldoutGroup("#UIInteractiveElement"), SerializeField] protected Image _buttonImage;
        [FoldoutGroup("#UIInteractiveElement"), SerializeField] protected TextMeshProUGUI _nameText;

        protected Vector3 _punchScale =
            new Vector3(DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE);

        protected float _lastClickTime;
        protected Tween _scaleTween;
        protected bool _isClickable = true;

        public bool IsClickable => _isClickable;

        protected Color _frameImageOriginalColor;
        protected Color _nameTextOriginalColor;

        private Vector3 _originalScale;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _frameImage ??= this.FindComponent<Image>("Frame Image");
            _buttonImage ??= this.FindComponent<Image>("Button Image");
            _nameText ??= this.FindComponent<TextMeshProUGUI>("Button Name Text");
        }

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitializeVisuals();
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
        }

        protected bool CheckClickCooldown()
        {
            if (!_isClickable)
            {
                return false;
            }

            float currentTime = Time.time;
            if (currentTime - _lastClickTime < DEFAULT_CLICK_COOLDOWN)
            {
                return false;
            }

            _lastClickTime = currentTime;
            return true;
        }

        protected void PlayPunchScaleAnimation()
        {
            KillScaleTween();

            if (transform != null)
            {
                _scaleTween = transform.DOPunchScale(_punchScale, DEFAULT_PUNCH_SCALE_DURATION, PUNCH_SCALE_VIBRATO, PUNCH_SCALE_ELASTICITY)
                    .SetEase(Ease.OutQuad).OnComplete(OnCompletedPunchScale);
            }
        }

        private void OnCompletedPunchScale()
        {
            transform.localScale = _originalScale;
            _scaleTween = null;
        }

        protected virtual void KillAllTweens()
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
                _nameText.DOColor(color, duration).SetEase(Ease.OutQuad);
            }
            else
            {
                _nameText.color = color;
            }
        }

        protected void SetFrameImageColor(Color color, float alpha = ALPHA_OPAQUE, float duration = DURATION_ZERO)
        {
            if (_frameImage == null)
            {
                return;
            }

            color.a = alpha;

            if (duration > DURATION_ZERO)
            {
                _frameImage.DOColor(color, duration).SetEase(Ease.OutQuad);
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
                _buttonImage.DOColor(color, duration).SetEase(Ease.OutQuad);
            }
            else
            {
                _buttonImage.color = color;
            }
        }
    }
}