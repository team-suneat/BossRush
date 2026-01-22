using UnityEngine;

namespace TeamSuneat
{
    public class ParticleEffect2DGroup : MonoBehaviour
    {
        private ParticleEffect2D[] _particleEffects;

        protected void Awake()
        {
            _particleEffects = GetComponentsInChildren<ParticleEffect2D>();
        }

        protected void OnEnable()
        {
            PlayParticles();
        }

        protected void OnDisable()
        {
            StopParticles();
        }

        public void PlayParticles()
        {
            if (_particleEffects != null)
            {
                for (int i = 0; i < _particleEffects.Length; i++)
                {
                    if (_particleEffects[i] != null)
                    {
                        _particleEffects[i].Play();
                    }
                }
            }
        }

        public void StopParticles()
        {
            if (_particleEffects != null)
            {
                for (int i = 0; i < _particleEffects.Length; i++)
                {
                    _particleEffects[i].Stop();
                }
            }
        }

        public void SetDirection(bool isFacingRight)
        {
            if (_particleEffects != null)
            {
                for (int i = 0; i < _particleEffects.Length; i++)
                {
                    if (_particleEffects[i] != null)
                    {
                        _particleEffects[i].SetDirection(isFacingRight);
                    }
                }
            }
        }
    }
}