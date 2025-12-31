using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public static class ColorEx
    {
        ///Convert Color to Hex.
        private static readonly Dictionary<Color32, string> colorHexCache = new();

        ///Convert Hex to Color.
        private static readonly Dictionary<string, Color> hexColorCache = new(System.StringComparer.OrdinalIgnoreCase);

        public static string ColorToHex(Color32 color)
        {
            if (color == Color.white) { color = GameColors.CreamIvory; }
            else if (color == Color.black) { color = GameColors.DeepCharcoalBlack; }
            else if (color == Color.red) { color = GameColors.CherryRed; }

            if (colorHexCache.TryGetValue(color, out string result))
            {
                return result;
            }

            result = "#" + ColorUtility.ToHtmlStringRGBA(color);
            return colorHexCache[color] = result;
        }

        public static Color HexToColor(string hex, float alpha = 1)
        {
            if (hexColorCache.TryGetValue(hex, out Color result))
            {
                result.a = alpha;
                return result;
            }

            if (hex.Length != 7)
            {
                throw new System.Exception("Invalid length for hex color provided");
            }

            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                color.a = alpha;
                hexColorCache.Add(hex, color);
                return color;
            }

            return Color.black;
        }
    }
}