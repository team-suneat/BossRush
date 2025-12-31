using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    public static class ImageEx
    {
        public static bool TrySetSprite(this Image image, string spriteName)
        {
            return image.TrySetSprite(spriteName, false);
        }

        public static bool TrySetSprite(this Image image, string spriteName, bool useNativeSize)
        {
            if (image == null)
            {
                Log.Warning("이미지에 스프라이트를 적용할 수 없습니다. 이미지를 찾을 수 없습니다.");
                return false;
            }

            if (!string.IsNullOrEmpty(spriteName))
            {
                Sprite sprite = ResourcesManager.LoadResource<Sprite>(spriteName);
                if (sprite != null)
                {
                    image.sprite = sprite;
                    if (useNativeSize)
                    {
                        image.SetNativeSize();
                    }
                    return true;
                }
            }

            return false;
        }

        //

        public static void SetSprite(this Image image, Sprite sprite, bool useNativeSize = false)
        {
            if (sprite != null && image != null)
            {
                image.sprite = sprite;
                if (useNativeSize)
                {
                    image.SetNativeSize();
                }
            }
        }

        public static void ResetSprite(this Image image)
        {
            if (image != null)
            {
                image.sprite = null;
            }
        }

        public static void SetColor(this Image image, Color color)
        {
            if (image != null)
            {
                image.color = color;
            }
        }

        public static void SetColor(this Image image, Color color, float alpha)
        {
            if (image != null)
            {
                image.color = new Color(color.r, color.g, color.b, alpha);
            }
        }

        public static void SetAlpha(this Image image, float alpha)
        {
            if (image != null)
            {
                Color origin = image.color;

                image.color = new Color(origin.r, origin.g, origin.b, alpha);
            }
        }

        public static void SetFillAmount(this Image image, float fillAmount)
        {
            if (image != null)
            {
                image.fillAmount = Mathf.Clamp01(fillAmount);
            }
        }
    }
}