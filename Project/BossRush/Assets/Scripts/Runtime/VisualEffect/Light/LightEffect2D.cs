using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TeamSuneat
{
    public class LightEffect2D : XBehaviour
    {
        public Light2D Light;

        [ReadOnly]
        [SerializeField]
        private float _defaultIntensity;

        private Coroutine _fadeOutCoroutine;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Light = GetComponent<Light2D>();
        }

        private void Awake()
        {
            if (Light != null)
            {
                _defaultIntensity = Light.intensity;
            }
        }

        public void Activate()
        {
            StopXCoroutine(ref _fadeOutCoroutine);

            Light.intensity = _defaultIntensity;

            SetActive(true);
        }

        public void FadeOut(float duration)
        {
            if (_fadeOutCoroutine == null)
            {
                _fadeOutCoroutine = StartXCoroutine(ProcessFadeOut(duration));
            }
        }

        private IEnumerator ProcessFadeOut(float duration)
        {
            float elapsedTime = 0;

            while (duration > elapsedTime)
            {
                Light.intensity = _defaultIntensity * elapsedTime.SafeDivide01(duration);

                yield return null;

                elapsedTime += Time.deltaTime;
            }

            Light.intensity = 0;
            SetActive(false);

            _fadeOutCoroutine = null;
        }
    }
}