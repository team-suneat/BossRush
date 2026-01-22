using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIFullScreenFader : XBehaviour
    {
        public Image EffectImage;
        private UICanvasGroupFader _fader;
        private Coroutine _coroutine;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            EffectImage = GetComponentInChildren<Image>();
        }

        protected void Awake()
        {
            _fader = GetComponent<UICanvasGroupFader>();

            if (_fader != null)
            {
                _fader.SetAlpha(0f);
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            _coroutine = null;
        }

        public void WhiteOut(float fadeDuration, float colorDuration)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.Camera, "카메라 컬러 이펙트 페이드 인이 시작됩니다. 색상: CreamIvory, 페이드 시간: {0}, 컬러 유지 시간: {1}", fadeDuration, colorDuration);
                EffectImage?.SetColor(GameColors.CreamIvory);
                _coroutine = StartXCoroutine(FadeOut(fadeDuration, colorDuration));
            }
        }

        public void FadeIn(Color color, float fadeDuration, float colorDuration)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.Camera, "카메라 컬러 이펙트 페이드 인이 시작됩니다. 색상: {0}, 페이드 시간: {1}, 컬러 유지 시간: {2}", color, fadeDuration, colorDuration);
                EffectImage?.SetColor(color);
                _coroutine = StartXCoroutine(FadeIn(fadeDuration, colorDuration));
            }
        }

        public void FadeOut(Color color, float fadeDuration, float colorDuration)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.Camera, "카메라 컬러 이펙트 페이드 아웃이 시작됩니다. 색상: {0}, 페이드 시간: {1}, 컬러 유지 시간: {2}", color, fadeDuration, colorDuration);
                EffectImage?.SetColor(color);
                _coroutine = StartXCoroutine(FadeOut(fadeDuration, colorDuration));
            }
        }

        public void FadeInOut(Color color, float fadeDuration, float fadeInDelayTime, float fadeOutDelayTime)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.Camera, "카메라 컬러 이펙트 페이드 인/아웃이 시작됩니다. 색상: {0}, 페이드 시간: {1}, 페이드 인 딜레이: {2}, 페이드 아웃 딜레이: {3}", color, fadeDuration, fadeInDelayTime, fadeOutDelayTime);
                EffectImage?.SetColor(color);
                _coroutine = StartXCoroutine(FadeInOut(fadeDuration, fadeDuration, fadeInDelayTime, fadeOutDelayTime));
            }
        }

        public void FadeInOut(Color color, float fadeInDuration, float fadeOutDuration, float fadeInDelayTime, float fadeOutDelayTime)
        {
            if (_coroutine == null)
            {
                Log.Info(LogTags.Camera, "카메라 컬러 이펙트 페이드 인/아웃이 시작됩니다. 색상: {0}, 페이드 인 시간: {1}, 딜레이: {2}, 페이드 아웃 시간: {3}, 딜레이: {4}",
                    color, fadeInDuration, fadeInDelayTime, fadeOutDuration, fadeOutDelayTime);

                EffectImage?.SetColor(color);
                _coroutine = StartXCoroutine(FadeInOut(fadeInDuration, fadeOutDuration, fadeInDelayTime, fadeOutDelayTime));
            }
        }

        private IEnumerator FadeIn(float fadeDuration, float colorDuration)
        {
            if (_fader != null)
            {
                _fader.FadeInDelayTime = colorDuration;
                _fader.FadeInDuration = fadeDuration;
                _fader.FadeIn();
            }

            yield return new WaitForSeconds(fadeDuration);

            _coroutine = null;
        }

        private IEnumerator FadeOut(float fadeDuration, float colorDuration)
        {
            if (_fader != null)
            {
                _fader.FadeOutDelayTime = colorDuration;
                _fader.FadeOutDuration = fadeDuration;
                _fader.FadeOut();
            }

            yield return new WaitForSeconds(fadeDuration);

            _coroutine = null;
        }

        private IEnumerator FadeInOut(float fadeInDuration, float fadeOutDuration, float fadeInDelayTime, float fadeOutDelayTime)
        {
            if (_fader != null)
            {
                _fader.FadeInDelayTime = fadeInDelayTime;
                _fader.FadeInDuration = fadeInDuration;
                _fader.FadeOutDelayTime = fadeOutDelayTime;
                _fader.FadeOutDuration = fadeOutDuration;

                _fader.FadeInOut();
            }

            float waitTime = fadeInDelayTime + fadeOutDelayTime + fadeInDuration + fadeOutDuration;

            yield return new WaitForSeconds(waitTime);

            _coroutine = null;
        }
    }
}