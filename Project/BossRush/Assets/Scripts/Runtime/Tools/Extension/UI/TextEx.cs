using TMPro;
using UnityEngine;

namespace TeamSuneat
{
    public static class TextEx
    {
        public static void SetFont(this TextMeshProUGUI text, TMP_FontAsset font)
        {
            if (text != null)
            {
                text.font = font;
            }
        }

        public static void SetText(this TextMeshProUGUI text, string content)
        {
            if (text != null)
            {
                text.text = content;
            }
        }

        public static void ResetText(this TextMeshProUGUI text)
        {
            if (text != null)
            {
                text.text = string.Empty;
            }
        }

        public static void SetTextColor(this TextMeshProUGUI text, Color color)
        {
            if (text != null)
            {
                text.color = color;
            }
        }

        public static void SetAlpha(this TextMeshProUGUI text, float alpha)
        {
            if (text != null)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            }
        }

        public static void UpdataSizeToTextLenth(this TextMeshProUGUI text)
        {
            text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight);
        }

        public static void UpdataSizeToTextLenthX(this TextMeshProUGUI text)
        {
            text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.rectTransform.sizeDelta.y);
        }

        public static void SetActive(this TextMeshProUGUI text, bool isActive)
        {
            if (text != null)
            {
                text.gameObject.SetActive(isActive);
            }
        }

        public static void SetUnderline(this TextMeshProUGUI text, bool isActive)
        {
            if (isActive)
            {
                text.fontStyle |= FontStyles.Underline;
            }
            else
            {
                text.fontStyle &= ~FontStyles.Underline;
            }
        }
    }
}