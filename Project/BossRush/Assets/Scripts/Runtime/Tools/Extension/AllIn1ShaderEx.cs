using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public static class AllIn1ShaderEx
    {     // All in one shader
        public static void CreateCloneMaterial(this SpriteRenderer renderer)
        {
            if (renderer != null)
            {
                try
                {
                    if (renderer.material.shader.name.Contains("AllIn1SpriteShader"))
                    {
                        Material newMaterial = Material.Instantiate(renderer.material);
                        newMaterial.name = renderer.material.name + "(Clone)";
                        renderer.material = newMaterial;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        #region Outline

        public static void SetOutline(this SpriteRenderer[] renderers, bool value)
        {
            if (renderers.IsValid())
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    SetOutline(renderers[i], value);
                }
            }
        }

        public static void SetOutline(this SpriteRenderer renderer, bool value)
        {
            if (renderer != null)
            {
                if (value)
                {
                    renderer.material.EnableKeyword("OUTBASE_ON");
                }
                else
                {
                    renderer.material.DisableKeyword("OUTBASE_ON");
                }
            }
        }

        public static void SetOutlineColor(this SpriteRenderer[] renderers, Color color)
        {
            if (renderers.IsValid())
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    SetOutlineColor(renderers[i], color);
                }
            }
        }

        public static void SetOutlineColor(this SpriteRenderer renderer, Color color)
        {
            if (renderer != null)
            {
                renderer.material.SetColor("_OutlineColor", color);
            }
        }

        public static void SetOutlineAlpha(this SpriteRenderer renderer, float alpha)
        {
            if (renderer != null)
            {
                renderer.material.SetFloat("_OutlineAlpha", alpha);
            }
        }

        #endregion Outline

        #region InnerOutline

        public static void SetInnerOutline(this SpriteRenderer[] renderers, bool value)
        {
            if (renderers.IsValid())
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    SetInnerOutline(renderers[i], value);
                }
            }
        }

        public static void SetInnerOutline(this SpriteRenderer renderer, bool value)
        {
            if (renderer != null)
            {
                if (value)
                {
                    renderer.material.EnableKeyword("INNEROUTLINE_ON");
                }
                else
                {
                    renderer.material.DisableKeyword("INNEROUTLINE_ON");
                }
            }
        }

        public static void SetInnerOutlineColor(this SpriteRenderer[] renderers, Color color)
        {
            if (renderers.IsValid())
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    SetInnerOutlineColor(renderers[i], color);
                }
            }
        }

        public static void SetInnerOutlineColor(this SpriteRenderer renderer, Color color)
        {
            if (renderer != null)
            {
                renderer.material.SetColor("_InnerOutlineColor", color);
            }
        }

        public static void SetInnerOutlineAlpha(this SpriteRenderer renderer, float alpha)
        {
            if (renderer != null)
            {
                renderer.material.SetFloat("_InnerOutlineAlpha", alpha);
            }
        }

        #endregion InnerOutline

        #region Hue Shift

        public static void SetHueShift(this SpriteRenderer renderer, float value)
        {
            if (renderer != null)
            {
                renderer.material.SetFloat("_HsvShift", value);
            }
        }

        #endregion Hue Shift

        #region HitEffect

        public static void SetHitEffect(this SpriteRenderer renderer, bool value)
        {
            if (renderer != null)
            {
                if (value)
                {
                    renderer.material.EnableKeyword("HITEFFECT_ON");
                }
                else
                {
                    renderer.material.DisableKeyword("HITEFFECT_ON");
                }
            }
        }

        public static void SetHitEffectColor(this SpriteRenderer renderer, Color color, float blend)
        {
            if (renderer != null)
            {
                renderer.material.SetColor("_HitEffectColor", color);
                renderer.material.SetFloat("_HitEffectBlend", blend);
            }
        }

        public static IEnumerator FlashHitEffect(this SpriteRenderer renderer, float duration, UnityAction onCompleted = null)
        {
            if (renderer == null)
            {
                yield break;
            }

            renderer.SetHitEffect(true);
            yield return new WaitForSeconds(duration);
            renderer.SetHitEffect(false);

            onCompleted?.Invoke();
        }

        public static IEnumerator FlickerHitEffect(this SpriteRenderer renderer, float interval, float duration, UnityAction onCompleted = null)
        {
            if (renderer == null)
            {
                yield break;
            }

            InfiniteLoopDetector.Reset();

            float flickerStop = Time.time + duration;
            WaitForSeconds wait = new(interval);
            while (Time.time < flickerStop)
            {
                renderer.SetHitEffect(true);
                yield return wait;

                renderer.SetHitEffect(false);
                yield return wait;

                InfiniteLoopDetector.Run();
            }

            renderer.SetHitEffect(false);

            onCompleted?.Invoke();
        }

        #endregion HitEffect

        #region GreyScale

        public static void SetGreyScale(this SpriteRenderer renderer, bool value)
        {
            if (renderer != null)
            {
                if (value)
                {
                    renderer.material.EnableKeyword("GREYSCALE_ON");
                }
                else
                {
                    renderer.material.DisableKeyword("GREYSCALE_ON");
                }
            }
        }

        public static void SetGreyScale(this UnityEngine.UI.Image image, bool value)
        {
            if (image != null)
            {
                if (value)
                {
                    image.material.EnableKeyword("GREYSCALE_ON");
                }
                else
                {
                    image.material.DisableKeyword("GREYSCALE_ON");
                }
            }
        }

        #endregion GreyScale
    }
}