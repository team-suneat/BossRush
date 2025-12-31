using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Effect
{
    public class VFXHitRenderer : XBehaviour
    {
        [SerializeField]
        [SuffixLabel("플레이어 피격 시 화이트 플래시 효과")]
        private bool enableWhiteFlashOnPlayerDamage;

        [SerializeField]
        [SuffixLabel("플레이어 피격 시 펀치 스케일 효과")]
        private bool enablePunchScaleOnPlayerDamage;

        [SerializeField]
        [SuffixLabel("화이트 플래시 효과 지속 시간")]
        private float effectDuration = 0.1f;

        [SerializeField]
        [SuffixLabel("스케일 효과 강도")]
        private float scaleForce = 0.1f;

        private SpriteRenderer[] _vfxRenderers;
        private Tweener _tweener;
        private List<Coroutine> _coroutines = new();

        private void Awake()
        {
            _vfxRenderers = GetComponentsInChildren<SpriteRenderer>();

            if (_vfxRenderers.IsValid())
            {
                foreach (SpriteRenderer renderer in _vfxRenderers)
                {
                    renderer.SetHitEffect(false);
                }
            }
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (enablePunchScaleOnPlayerDamage)
            {
                ResetLocalScale();
            }

            if (enableWhiteFlashOnPlayerDamage)
            {
                ResetHitEffectRenderer();
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            StopPunchScale();
            ClearFlashCoroutine();
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            if (enableWhiteFlashOnPlayerDamage || enablePunchScaleOnPlayerDamage)
            {
                GlobalEvent<DamageResult>.Register(GlobalEventType.PLAYER_CHARACTER_DAMAGED, OnPlayerCharacterDamage);
            }
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            if (enableWhiteFlashOnPlayerDamage || enablePunchScaleOnPlayerDamage)
            {
                GlobalEvent<DamageResult>.Unregister(GlobalEventType.PLAYER_CHARACTER_DAMAGED, OnPlayerCharacterDamage);
            }
        }

        private void OnPlayerCharacterDamage(DamageResult damageResult)
        {
            FlashWhite();
        }

        public void FlashWhite()
        {
            if (enablePunchScaleOnPlayerDamage)
            {
                StopPunchScale();
                ResetLocalScale();
                StartPunchScale();
            }

            if (enableWhiteFlashOnPlayerDamage && _vfxRenderers.IsValid())
            {
                ClearFlashCoroutine();

                foreach (SpriteRenderer renderer in _vfxRenderers)
                {
                    _coroutines.Add(StartXCoroutine(renderer.FlashHitEffect(effectDuration)));
                }
            }
        }

        private void StartPunchScale()
        {
            _tweener = transform.DOPunchScale(Vector2.one * scaleForce, effectDuration);
            _tweener.onComplete = OnCompletedPunchScale;
        }

        private void OnCompletedPunchScale()
        {
            _tweener = null;
            ResetLocalScale();
        }

        private void StopPunchScale()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }
        }

        private void ClearFlashCoroutine()
        {
            for (int i = 0; i < _coroutines.Count; i++)
            {
                StopCoroutine(_coroutines[i]);
            }

            _coroutines.Clear();
        }

        private void ResetLocalScale()
        {
            transform.localScale = Vector3.one;
        }

        private void ResetHitEffectRenderer()
        {
            if (enableWhiteFlashOnPlayerDamage && _vfxRenderers.IsValid())
            {
                for (int i = 0; i < _vfxRenderers.Length; i++)
                {
                    SpriteRenderer renderer = _vfxRenderers[i];
                    renderer.SetHitEffect(false);
                }
            }
        }
    }
}