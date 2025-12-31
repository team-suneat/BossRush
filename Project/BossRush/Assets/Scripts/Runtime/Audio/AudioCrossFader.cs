using System;
using System.Collections;
using UnityEngine;

namespace TeamSuneat.Audio
{
    public class AudioCrossFader
    {
        private AudioObject _from;
        private AudioObject _to;
        private float _duration;
        private Coroutine _fadeCoroutine;
        private readonly MonoBehaviour _coroutineHost;

        public bool IsFading { get; private set; }

        public AudioCrossFader(MonoBehaviour coroutineHost)
        {
            _coroutineHost = coroutineHost;
        }

        public void StartFade(AudioObject from, AudioObject to, float duration, Action<AudioObject> onComplete = null)
        {
            StopFade();

            _from = from;
            _to = to;
            _duration = duration;

            if (_to == null || _duration <= 0f)
            {
                return;
            }

            IsFading = true;
            _fadeCoroutine = _coroutineHost.StartCoroutine(FadeCoroutine(onComplete));
        }

        public void StopFade()
        {
            if (_fadeCoroutine != null)
            {
                _coroutineHost.StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            IsFading = false;
        }

        private IEnumerator FadeCoroutine(Action<AudioObject> onComplete)
        {
            float fromStartVolume = _from != null ? _from.Volume : 0f;
            float toTargetVolume = _to.Volume;

            _to.SetVolume(0f);

            float timer = 0f;
            while (timer < _duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / _duration);

                if (_from != null)
                {
                    _from.SetVolume(Mathf.Lerp(fromStartVolume, 0f, t));
                }

                _to.SetVolume(Mathf.Lerp(0f, toTargetVolume, t));

                yield return null;
            }

            if (_from != null)
            {
                _from.Stop();
                _from.Despawn();
            }

            _to.SetVolume(toTargetVolume);
            _fadeCoroutine = null;
            IsFading = false;

            onComplete?.Invoke(_to);
        }
    }
}