using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    public static class FadeEx
    {
        // Fade In

        public static Tweener FadeIn(this TextMeshProUGUI textPro, float targetAlpha, float duration, float delayTime)
        {
            if (textPro == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                textPro.alpha = 0;
                tweener = textPro.DOFade(targetAlpha, duration);

                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }

        public static Tweener FadeIn(this Image image, float targetAlpha, float duration, float delayTime)
        {
            if (image == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
                tweener = image.DOFade(targetAlpha, duration);

                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }

        public static Tweener FadeIn(this SpriteRenderer spriteRenderer, float originAlpha, float targetAlpha, float duration, float delayTime)
        {
            if (spriteRenderer == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, originAlpha);
                tweener = spriteRenderer.DOFade(targetAlpha, duration);

                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }

        public static Tweener FadeIn(this SpriteRenderer spriteRenderer, float targetAlpha, float duration, float delayTime)
        {
            if (spriteRenderer == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
                tweener = spriteRenderer.DOFade(targetAlpha, duration);

                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }

        public static Tweener FadeIn(this UnityEngine.Rendering.Universal.Light2D light, float targetAlpha, float duration, float delayTime)
        {
            if (light == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                _ = DOTween.To(() => light.intensity, x => light.intensity = x, targetAlpha, duration);
                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }

        // Fade Out

        public static Tweener FadeOut(this TextMeshProUGUI textPro, float targetAlpha, float duration, float delayTime)
        {
            if (textPro == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                textPro.alpha = 1;
                tweener = textPro.DOFade(targetAlpha, duration);

                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }

        public static Tweener FadeOut(this Image image, float targetAlpha, float duration, float delayTime)
        {
            if (image == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
                tweener = image.DOFade(targetAlpha, duration);
                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }

        public static Tweener FadeOut(this SpriteRenderer spriteRenderer, float targetAlpha, float duration, float delayTime)
        {
            if (spriteRenderer == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                tweener = spriteRenderer.DOFade(targetAlpha, duration);
                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }

        public static Tweener FadeOut(this SpriteRenderer spriteRenderer, float originAlpha, float targetAlpha, float duration, float delayTime)
        {
            if (spriteRenderer == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, originAlpha);
                tweener = spriteRenderer.DOFade(targetAlpha, duration);
                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }

        public static Tweener FadeOut(this UnityEngine.Rendering.Universal.Light2D light, float targetAlpha, float duration, float delayTime)
        {
            if (light == null)
            {
                return null;
            }

            Tweener tweener = null;
            if (duration > 0)
            {
                _ = DOTween.To(() => light.intensity, x => light.intensity = x, targetAlpha, duration);
                if (delayTime > 0)
                {
                    _ = tweener.SetDelay(delayTime);
                }
            }

            return tweener;
        }
    }
}