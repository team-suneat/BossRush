using System.Collections;
using TeamSuneat;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    [RequireComponent(typeof(UICanvasGroupFader))]
    public class UIScreenFader : XBehaviour
    {
        [SerializeField]
        private Image _effectImage;

        [SerializeField]
        private UICanvasGroupFader _fader;

        private Coroutine _coroutine;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _effectImage = GetComponentInChildren<Image>();
            _fader = GetComponent<UICanvasGroupFader>();
        }

        private void Awake()
        {
            if (_fader != null)
            {
                _fader.SetAlpha(0f);
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _fader?.KillFade();
        }

        public void WhiteOut(float fadeDuration, float colorDuration)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.UI, "화이트 아웃 효과 시작. 페이드 시간: {0}, 컬러 유지 시간: {1}", fadeDuration, colorDuration);
                _effectImage?.SetColor(GameColors.CreamIvory);
                _coroutine = StartCoroutine(FadeInOutCoroutine(fadeDuration, fadeDuration, 0f, colorDuration));
            }
        }

        public void FadeIn(Color color, float fadeDuration, float colorDuration)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.UI, "화면 페이드 인 시작. 색상: {0}, 페이드 시간: {1}, 컬러 유지 시간: {2}", color, fadeDuration, colorDuration);
                _effectImage?.SetColor(color);
                _coroutine = StartCoroutine(FadeInCoroutine(fadeDuration, colorDuration));
            }
        }

        public void FadeOut(Color color, float fadeDuration, float colorDuration)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.UI, "화면 페이드 아웃 시작. 색상: {0}, 페이드 시간: {1}, 컬러 유지 시간: {2}", color, fadeDuration, colorDuration);
                _effectImage?.SetColor(color);
                _coroutine = StartCoroutine(FadeOutCoroutine(fadeDuration, colorDuration));
            }
        }

        public void FadeInOut(Color color, float fadeDuration, float fadeInDelayTime, float fadeOutDelayTime)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.UI, "화면 페이드 인/아웃 시작. 색상: {0}, 페이드 시간: {1}, 페이드 인 딜레이: {2}, 페이드 아웃 딜레이: {3}",
                    color, fadeDuration, fadeInDelayTime, fadeOutDelayTime);
                _effectImage?.SetColor(color);
                _coroutine = StartCoroutine(FadeInOutCoroutine(fadeDuration, fadeDuration, fadeInDelayTime, fadeOutDelayTime));
            }
        }

        public void FadeInOut(Color color, float fadeInDuration, float fadeOutDuration, float fadeInDelayTime, float fadeOutDelayTime)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.UI, "화면 페이드 인/아웃 시작. 색상: {0}, 페이드 인 시간: {1}, 딜레이: {2}, 페이드 아웃 시간: {3}, 딜레이: {4}",
                    color, fadeInDuration, fadeInDelayTime, fadeOutDuration, fadeOutDelayTime);

                _effectImage?.SetColor(color);
                _coroutine = StartCoroutine(FadeInOutCoroutine(fadeInDuration, fadeOutDuration, fadeInDelayTime, fadeOutDelayTime));
            }
        }

        private IEnumerator FadeInCoroutine(float fadeDuration, float colorDuration)
        {
            if (_fader != null)
            {
                _fader.FadeInDelayTime = 0f;
                _fader.FadeInDuration = fadeDuration;
                _fader.FadeIn();
            }

            yield return new WaitForSeconds(fadeDuration + colorDuration);

            _coroutine = null;
        }

        private IEnumerator FadeOutCoroutine(float fadeDuration, float colorDuration)
        {
            // 페이드 아웃 전에 색상을 먼저 보여줌
            if (_fader != null)
            {
                _fader.SetAlpha(1f);
            }

            yield return new WaitForSeconds(colorDuration);

            if (_fader != null)
            {
                _fader.FadeOutDelayTime = 0f;
                _fader.FadeOutDuration = fadeDuration;
                _fader.FadeOut();
            }

            yield return new WaitForSeconds(fadeDuration);

            _coroutine = null;
        }

        private IEnumerator FadeInOutCoroutine(float fadeInDuration, float fadeOutDuration, float fadeInDelayTime, float fadeOutDelayTime)
        {
            if (_fader != null)
            {
                _fader.FadeInDelayTime = fadeInDelayTime;
                _fader.FadeInDuration = fadeInDuration;
                _fader.FadeOutDelayTime = fadeOutDelayTime;
                _fader.FadeOutDuration = fadeOutDuration;

                _fader.FadeInOut();
            }

            float totalTime = fadeInDelayTime + fadeInDuration + fadeOutDelayTime + fadeOutDuration;

            yield return new WaitForSeconds(totalTime);

            _coroutine = null;
        }
    }
}