using UnityEngine;

namespace TeamSuneat
{
    public class ParticleEffect2DGroup : MonoBehaviour
    {
        private ParticleEffect2D[] _particleEffects;

        [SerializeField]
        private ParticleEffect2D[] _leftParticleEffects;

        [SerializeField]
        private ParticleEffect2D[] _rightParticleEffects;

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

        //

        public void SetDirection(bool facingRight)
        {
            if (_leftParticleEffects.IsValid() && _rightParticleEffects.IsValid())
            {
                for (int i = 0; i < _leftParticleEffects.Length; i++)
                {
                    if (_leftParticleEffects[i] != null)
                    {
                        _leftParticleEffects[i].SetActive(!facingRight);
                    }
                }

                for (int i = 0; i < _rightParticleEffects.Length; i++)
                {
                    if (_rightParticleEffects[i] != null)
                    {
                        _rightParticleEffects[i].SetActive(facingRight);
                    }
                }
            }
            else if (_particleEffects.IsValid())
            {
                for (int i = 0; i < _particleEffects.Length; i++)
                {
                    if (_particleEffects[i] != null)
                    {
                        _particleEffects[i].SetDirection(facingRight);
                    }
                }
            }
        }

        public void SetSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            if (_particleEffects != null)
            {
                for (int i = 0; i < _particleEffects.Length; i++)
                {
                    if (_particleEffects[i] != null)
                    {
                        _particleEffects[i].SetSpriteRenderer(spriteRenderer);
                    }
                }
            }
        }
    }
}