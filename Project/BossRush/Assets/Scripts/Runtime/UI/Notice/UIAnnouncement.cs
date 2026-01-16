using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIAnnouncement : UINoticeBase
    {
        [SerializeField]
        private Image _iconImage;

        [SerializeField]
        private Image _backgroundImage;

        private Tweener _scaleTweener;
        private const float PUNCH_SCALE_VALUE = 0.1f;
        private const float PUNCH_SCALE_DURATION = 0.3f;
        private const int PUNCH_SCALE_VIBRATO = 1;
        private const float PUNCH_SCALE_ELASTICITY = 0.5f;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _iconImage ??= this.FindComponent<Image>("Icon Image");
            _backgroundImage ??= this.FindComponent<Image>("Background Image");
        }

        public void SetIcon(Sprite icon)
        {
            if (_iconImage != null)
            {
                _iconImage.sprite = icon;
                _iconImage.SetActive(icon != null);
            }
        }

        public void SetBackground(Sprite background)
        {
            if (_backgroundImage != null)
            {
                _backgroundImage.sprite = background;
            }
        }

        public void ShowAnnouncement(string content, float duration = 3f)
        {
            SetContent(content);

            // 펀치 스케일 애니메이션 시작
            StartPunchScale();

            // 페이드 설정 및 표시
            if (_fader != null)
            {
                _fader.FadeInDuration = 0.3f;
                _fader.FadeOutDuration = 0.3f;
                _fader.FadeOutDelayTime = duration;
            }

            Show();
        }

        public void ShowAnnouncementByKey(string stringKey, float duration = 3f)
        {
            SetStringKey(stringKey);

            StartPunchScale();

            if (_fader != null)
            {
                _fader.FadeInDuration = 0.3f;
                _fader.FadeOutDuration = 0.3f;
                _fader.FadeOutDelayTime = duration;
            }

            Show();
        }

        private void StartPunchScale()
        {
            if (_scaleTweener != null)
            {
                return;
            }

            transform.localScale = Vector3.one;
            Vector3 punch = Vector3.one * PUNCH_SCALE_VALUE;

            _scaleTweener = transform.DOPunchScale(punch, PUNCH_SCALE_DURATION, PUNCH_SCALE_VIBRATO, PUNCH_SCALE_ELASTICITY);
            _ = _scaleTweener.SetUpdate(true);
            _ = _scaleTweener.SetEase(Ease.OutQuad);
            _ = _scaleTweener.OnComplete(OnCompletedPunchScale);
        }

        private void OnCompletedPunchScale()
        {
            transform.localScale = Vector3.one;
            _scaleTweener = null;
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            if (_scaleTweener != null)
            {
                _scaleTweener.Kill();
                _scaleTweener = null;
            }
        }
    }
}
