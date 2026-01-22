using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class ParticleEffect2D : XBehaviour
    {
        [FoldoutGroup("#Particle System")]
        [SerializeField]
        private ParticleSystemStopBehavior _stopBehavior = ParticleSystemStopBehavior.StopEmitting;

        private ParticleSystem _particleSystem;
        private float _initialVelocityX;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            CacheInitialVelocity();
        }

        private void CacheInitialVelocity()
        {
            // 초기 velocity 값 저장
            if (_particleSystem != null)
            {
                ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = _particleSystem.velocityOverLifetime;
                if (velocityOverLifetime.enabled)
                {
                    _initialVelocityX = velocityOverLifetime.x.constant;
                }
            }
        }

        public void Play()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Play(true);
            }
        }

        public void Stop()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Stop(true, _stopBehavior);
            }
        }

        public void SetDirection(bool isFacingRight)
        {
            SetVelocityDirection(isFacingRight);
        }

        private void SetVelocityDirection(bool isFacingRight)
        {
            if (_particleSystem != null)
            {
                ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = _particleSystem.velocityOverLifetime;
                if (velocityOverLifetime.enabled)
                {
                    // 저장된 초기 값 사용 (원래 값의 부호를 고려하여 방향 설정)
                    float newXVelocity = isFacingRight ? _initialVelocityX : -_initialVelocityX;

                    velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(newXVelocity);
                }
            }
        }

    }
}