using System.Collections;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class CharacterRenderer : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private Coroutine _flickerCoroutine;

        public SortingLayerNames SortingLayerName { get; internal set; }

        public int SortingLayerMaxOrder { get; internal set; }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        internal void ResetRenderer()
        {
            StopFlickerCoroutine();
            DeactiveHitEffectAll();
        }

        internal void StartFlickerCoroutine(RendererFlickerNames flikerName)
        {
            if (!IsRendererValid()) { return; }

            FlickerAsset flickerAsset = ScriptableDataManager.Instance.FindFlicker(flikerName);
            if (flickerAsset == null) { return; }

            // 기존 깜빡임이 진행 중이면 강제로 멈추고 새로 시작
            StopFlickerCoroutine();

            SetHitEffectColor(_renderer, flickerAsset.FlickerColor, flickerAsset.FlickerBlend);

            _flickerCoroutine = StartCoroutine(FlickerHitEffect(_renderer, flickerAsset.FlickerSpeed, flickerAsset.FlickerDuration));
        }

        internal void DeactiveHitEffectAll()
        {
            if (IsRendererValid())
            {
                _renderer.SetHitEffect(false);
            }
        }

        internal void SetHitEffectColor(SpriteRenderer renderer, Color hitEffectColor, float hitEffectBlend)
        {
            if (renderer != null)
            {
                renderer.SetHitEffectColor(hitEffectColor, hitEffectBlend);
            }
        }

        #region Outline Methods

        internal void HideOutline()
        {
            SetOutlineKeyword("OUTLINE_ON", false);
        }

        internal void ShowOutline()
        {
            SetOutlineKeyword("OUTLINE_ON", true);
        }

        #endregion Outline Methods

        #region Private Methods

        private bool IsRendererValid()
        {
            return _renderer != null && _renderer.gameObject.activeInHierarchy;
        }

        private void SetOutlineKeyword(string keyword, bool enable)
        {
            if (!IsRendererValid()) { return; }

            if (enable)
            {
                _renderer.material.EnableKeyword(keyword);
            }
            else
            {
                _renderer.material.DisableKeyword(keyword);
            }
        }

        private void StopFlickerCoroutine()
        {
            if (_flickerCoroutine == null) { return; }

            StopCoroutine(_flickerCoroutine);
            _flickerCoroutine = null;
        }

        #endregion Private Methods

        #region Coroutines

        private IEnumerator FlickerHitEffect(SpriteRenderer renderer, float flickerSpeed, float flickerDuration)
        {
            if (renderer == null)
            {
                yield break;
            }

            float flickerStop = Time.time + flickerDuration;
            WaitForSeconds wait = new(flickerSpeed);

            while (Time.time < flickerStop)
            {
                renderer.SetHitEffect(true);
                yield return wait;

                renderer.SetHitEffect(false);
                yield return wait;
            }

            _renderer.SetHitEffect(false);
            _flickerCoroutine = null;
        }

        #endregion Coroutines
    }
}