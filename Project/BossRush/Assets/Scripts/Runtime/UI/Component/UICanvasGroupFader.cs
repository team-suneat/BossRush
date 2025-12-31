using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UICanvasGroupFader : XBehaviour
    {
        public CanvasGroup CanvasGroup;

        public float FadeInDuration;
        public float FadeOutDuration;

        public float FadeInDelayTime;
        public float FadeOutDelayTime;

        private TweenerCore<float, float, FloatOptions> _tweener;
        private UnityAction _onCompleteCallback;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            CanvasGroup = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            AutoGetComponents();
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }
        }

        public void Show()
        {
            CanvasGroup.interactable = true;
            SetAlpha(1);
        }

        public void Hide()
        {
            CanvasGroup.interactable = false;
            SetAlpha(0);
        }

        public void SetAlpha(float alpha)
        {
            CanvasGroup.alpha = alpha;
        }

        public void SetCompletedCallback(UnityAction callback)
        {
            _onCompleteCallback = callback;
        }

        public void KillFade()
        {
            _tweener?.Kill();
        }

        public void KillFadeWithComplete()
        {
            _tweener?.Kill(true);
        }

        public void FadeIn()
        {
            StartFade(0f, 1f, FadeInDuration, FadeInDelayTime);
        }

        public void FadeOut()
        {
            StartFade(1f, 0f, FadeOutDuration, FadeOutDelayTime);
        }

        public void FadeInOut()
        {
            FadeIn(() =>
            {
                FadeOut(OnFadeComplete);
            });
        }

        public void FadeIn(TweenCallback onFadeComplete)
        {
            StartFade(0f, 1f, FadeInDuration, FadeInDelayTime, onFadeComplete);
        }

        public void FadeIn(TweenCallback onFadeDelay, TweenCallback onFadeComplete)
        {
            StartFade(0f, 1f, FadeInDuration, FadeInDelayTime, onFadeDelay, onFadeComplete);
        }

        public void FadeOut(TweenCallback onFadeComplete)
        {
            StartFade(1f, 0f, FadeOutDuration, FadeOutDelayTime, onFadeComplete);
        }

        private void StartFade(float startAlpha, float targetAlpha, float duration, float delay, TweenCallback onDelay, TweenCallback onComplete = null)
        {
            if (CanvasGroup == null)
            {
                return;
            }

            CanvasGroup.alpha = startAlpha;

            _tweener?.Kill();
            _tweener = CanvasGroup.DOFade(targetAlpha, duration);
            _tweener.SetUpdate(true);

            if (!Mathf.Approximately(0f, delay))
            {
                _tweener.SetDelay(delay);
                if (onDelay != null)
                    CoroutineNextRealTimer(delay, onDelay.Invoke);
            }

            if (onComplete != null)
            {
                _tweener.OnComplete(onComplete);
            }
            else
            {
                _tweener.OnComplete(OnFadeComplete);
            }
        }

        private void StartFade(float startAlpha, float targetAlpha, float duration, float delay, TweenCallback onComplete = null)
        {
            if (CanvasGroup == null)
            {
                return;
            }

            CanvasGroup.alpha = startAlpha;

            _tweener?.Kill();
            _tweener = CanvasGroup.DOFade(targetAlpha, duration);
            _tweener.SetUpdate(true);

            if (!Mathf.Approximately(0f, delay))
            {
                _tweener.SetDelay(delay);
            }

            if (onComplete != null)
            {
                _tweener.OnComplete(onComplete);
            }
            else
            {
                _tweener.OnComplete(OnFadeComplete);
            }
        }

        private void OnFadeComplete()
        {
            _tweener = null;
            _onCompleteCallback?.Invoke();
        }
    }
}