using UnityEngine;

namespace TeamSuneat
{
    public static class SpriteRendererEx
    {
        //
        public static void SetSortingLayer(this SpriteRenderer[] renderers, SortingLayerNames layerName)
        {
            if (!renderers.IsValid())
            {
                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                SetSortingLayer(renderers[i], layerName);
            }
        }

        public static void SetSortingLayer(this SpriteRenderer[] renderers, SortingLayer layer)
        {
            if (!renderers.IsValid())
            {
                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                SetSortingLayer(renderers[i], layer.name);
            }
        }

        public static void SetSortingLayer(this SpriteRenderer renderer, string layerName)
        {
            if (renderer != null)
            {
                renderer.sortingLayerName = layerName;
            }
        }

        public static void SetSortingLayer(this SpriteRenderer renderer, SortingLayerNames layer)
        {
            if (renderer != null)
            {
                renderer.sortingLayerName = layer.ToString();
            }
        }

        public static void SetSortingOrder(this SpriteRenderer renderer, int sortingOrder)
        {
            if (renderer != null)
            {
                renderer.sortingOrder = sortingOrder;
            }
        }

        public static void SetSortingOrder(this SpriteRenderer[] renderers, int sortingOrder)
        {
            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].sortingOrder = sortingOrder;
                }
            }
        }

        public static bool TrySetSprite(this SpriteRenderer renderer, Sprite sprite)
        {
            if (renderer != null && sprite != null)
            {
                renderer.sprite = sprite;
                return true;
            }
            return false;
        }

        public static void SetAlpha(this SpriteRenderer renderer, float alpha)
        {
            if (renderer != null)
            {
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            }
        }

        #region Flip

        public static void SetFlipX(this SpriteRenderer renderer, bool isFlip)
        {
            if (renderer != null)
            {
                renderer.flipX = isFlip;
            }
        }

        public static void SetFlipY(this SpriteRenderer renderer, bool isFlip)
        {
            if (renderer != null)
            {
                renderer.flipY = isFlip;
            }
        }

        public static void SwitchFlipX(this SpriteRenderer renderer)
        {
            if (renderer != null)
            {
                renderer.flipX = !renderer.flipX;
            }
        }

        #endregion Flip
    }
}