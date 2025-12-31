using System.Text;
using UnityEngine;

namespace TeamSuneat
{
    public static partial class SpriteEx
    {
        // 아이콘 스트링 포맷
        private const string CHARACTER_ICON_FORMAT = "ui_character_icon_";
        private const string PASSIVE_ICON_FORMAT = "ui_passive_icon_";
        private const string ITEM_ICON_FORMAT = "ui_item_icon_";
        private const string CURRENCY_ICON_FORMAT = "ui_currency_icon_";

        // 아틀라스 이름
        private const string CHARACTER_ATLAS_NAME = "atlas_character";
        private const string ITEM_ATLAS_NAME = "atlas_item";

        // 공통 StringBuilder 인스턴스
        private static readonly StringBuilder _stringBuilder = new();

        public static string GetSpriteName(this CharacterNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(CHARACTER_ICON_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this PassiveNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(PASSIVE_ICON_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this ItemNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(ITEM_ICON_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this CurrencyNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(CURRENCY_ICON_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        //

        public static Sprite LoadSprite(this CharacterNames characterName)
        {
            if (characterName == CharacterNames.None)
            {
                return null;
            }

            string spriteName = GetSpriteName(characterName);
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            return ResourcesManager.LoadSprite(spriteName, CHARACTER_ATLAS_NAME);
        }

        public static Sprite LoadSprite(this ItemNames itemName)
        {
            if (itemName == ItemNames.None)
            {
                return null;
            }

            string spriteName = GetSpriteName(itemName);
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            return ResourcesManager.LoadSprite(spriteName, ITEM_ATLAS_NAME);
        }

        public static Sprite LoadSprite(this CurrencyNames currencyName)
        {
            if (currencyName == CurrencyNames.None)
            {
                return null;
            }

            string spriteName = GetSpriteName(currencyName);
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            return ResourcesManager.LoadSprite(spriteName, ITEM_ATLAS_NAME);
        }
    }
}