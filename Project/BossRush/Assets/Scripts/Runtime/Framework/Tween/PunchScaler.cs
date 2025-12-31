using DG.Tweening;
using UnityEngine;

namespace TeamSuneat
{
    public class PunchScaler : XBehaviour
    {
        private const float DEFAULT_PUNCH_SCALE_DURATION = 0.1f;
        private const int PUNCH_SCALE_VIBRATO = 1;
        private const float PUNCH_SCALE_ELASTICITY = 0.5f;
        private const float DEFAULT_PUNCH_SCALE_VALUE = -0.1f;

        protected Vector3 _punchScale =
            new Vector3(DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE);
        protected Tween _scaleTween;
        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public void PlayPunchScaleAnimation()
        {
            KillScaleTween();

            if (transform != null)
            {
                _scaleTween = transform.DOPunchScale(_punchScale,
                    DEFAULT_PUNCH_SCALE_DURATION,
                    PUNCH_SCALE_VIBRATO,
                    PUNCH_SCALE_ELASTICITY)
                    .SetEase(Ease.OutQuad).OnComplete(OnCompletedPunchScale);
            }
        }

        public void OnCompletedPunchScale()
        {
            transform.localScale = _originalScale;
            _scaleTween = null;
        }

        protected void KillScaleTween()
        {
            _scaleTween?.Kill();
            _scaleTween = null;
        }
    }
}